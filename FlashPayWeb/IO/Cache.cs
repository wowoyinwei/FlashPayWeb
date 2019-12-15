using FlashPayWeb.libs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashPayWeb.IO
{
    public abstract class Cache<TKey, TValue>
        where TKey : ISerializable
        where TValue : class, ISerializable ,new()
    {
        public class Trackable
        {
            public TKey Key;
            public TValue Item;
            public TrackState State;
        }

        public readonly Dictionary<TKey, Trackable> dictionary = new Dictionary<TKey, Trackable>();

        public void Add(TKey key, TValue value)
        {
            lock (dictionary)
            {
                if (dictionary.TryGetValue(key, out Trackable trackable) && trackable.State != TrackState.Deleted)
                    throw new ArgumentException();
                dictionary[key] = new Trackable
                {
                    Key = key,
                    Item = value,
                    State = trackable == null ? TrackState.Added : TrackState.Changed
                };
            }
        }

        protected abstract void AddInternal(TKey key, TValue value);
        protected abstract void UpdateInternal(TKey key, TValue value);
        public abstract void DeleteInternal(TKey key);
        public abstract TValue GetInternal(TKey key);
        public abstract IEnumerable<KeyValuePair<TKey, TValue>> FindInternal(byte[] beginKey);

        public void Commit()
        {
            foreach (Trackable trackable in GetChangeSet())
                switch (trackable.State)
                {
                    case TrackState.Added:
                        AddInternal(trackable.Key, trackable.Item);
                        break;
                    case TrackState.Changed:
                        UpdateInternal(trackable.Key, trackable.Item);
                        break;
                    case TrackState.Deleted:
                        DeleteInternal(trackable.Key);
                        break;
                }
        }

        public void Delete(TKey key)
        {
            lock (dictionary)
            {
                if (dictionary.TryGetValue(key, out Trackable trackable))
                {
                    if (trackable.State == TrackState.Added)
                        dictionary.Remove(key);
                    else
                        trackable.State = TrackState.Deleted;
                }
                else
                {
                    TValue item = GetInternal(key);
                    if (item == null) return;
                    dictionary.Add(key, new Trackable
                    {
                        Key = key,
                        Item = item,
                        State = TrackState.Deleted
                    });
                }
            }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Find(byte[] beginKey = null)
        {
            IEnumerable<(byte[], TKey, TValue)> cached;
            lock (dictionary)
            {
                cached = dictionary
                    .Where(p => p.Value.State != TrackState.Deleted && (beginKey == null || p.Key.ToArray().Take(beginKey.Length).SequenceEqual(beginKey)))
                    .Select(p =>
                    (
                        KeyBytes: p.Key.ToArray(),
                        p.Key,
                        p.Value.Item
                    ))
                    .OrderBy(p => p.KeyBytes, ByteArrayComparer.Default)
                    .ToArray();
            }
            var uncached = FindInternal(beginKey ?? new byte[0])
                .Where(p => !dictionary.ContainsKey(p.Key))
                .Select(p =>
                (
                    KeyBytes: p.Key.ToArray(),
                    p.Key,
                    p.Value
                ));
            using (var e1 = cached.GetEnumerator())
            using (var e2 = uncached.GetEnumerator())
            {
                (byte[] KeyBytes, TKey Key, TValue Item) i1, i2;
                bool c1 = e1.MoveNext();
                bool c2 = e2.MoveNext();
                i1 = c1 ? e1.Current : default;
                i2 = c2 ? e2.Current : default;
                while (c1 || c2)
                {
                    if (!c2 || (c1 && ByteArrayComparer.Default.Compare(i1.KeyBytes, i2.KeyBytes) < 0))
                    {
                        this.Add(i1.Key, i1.Item);
                        yield return new KeyValuePair<TKey, TValue>(i1.Key, i1.Item);
                        c1 = e1.MoveNext();
                        i1 = c1 ? e1.Current : default;
                    }
                    else
                    {
                        this.Add(i2.Key, i2.Item);
                        yield return new KeyValuePair<TKey, TValue>(i2.Key, i2.Item);
                        c2 = e2.MoveNext();
                        i2 = c2 ? e2.Current : default;
                    }
                }
            }
        }

        public IEnumerable<Trackable> GetChangeSet()
        {
            lock (dictionary)
            {
                foreach (Trackable trackable in dictionary.Values.Where(p => p.State != TrackState.None))
                    yield return trackable;
            }
        }

        public TValue GetAndChange(TKey key, Func<TValue> factory = null)
        {
            lock (dictionary)
            {
                if (dictionary.TryGetValue(key, out Trackable trackable))
                {
                    if (trackable.State == TrackState.Deleted)
                    {
                        if (factory == null) throw new KeyNotFoundException();
                        trackable.Item = factory();
                        trackable.State = TrackState.Changed;
                    }
                    else if (trackable.State == TrackState.None)
                    {
                        trackable.State = TrackState.Changed;
                    }
                }
                else
                {
                    trackable = new Trackable
                    {
                        Key = key,
                        Item = GetInternal(key)
                    };
                    if (trackable.Item == null)
                    {
                        if (factory == null) throw new KeyNotFoundException();
                        trackable.Item = factory();
                        trackable.State = TrackState.Added;
                    }
                    else
                    {
                        trackable.State = TrackState.Changed;
                    }
                    dictionary.Add(key, trackable);
                }
                return trackable.Item;
            }
        }

        public TValue GetOrAdd(TKey key, Func<TValue> factory)
        {
            lock (dictionary)
            {
                if (dictionary.TryGetValue(key, out Trackable trackable))
                {
                    if (trackable.State == TrackState.Deleted)
                    {
                        trackable.Item = factory();
                        trackable.State = TrackState.Changed;
                    }
                }
                else
                {
                    trackable = new Trackable
                    {
                        Key = key,
                        Item = GetInternal(key)
                    };
                    if (trackable.Item == null)
                    {
                        trackable.Item = factory();
                        trackable.State = TrackState.Added;
                    }
                    else
                    {
                        trackable.State = TrackState.None;
                    }
                    dictionary.Add(key, trackable);
                }
                return trackable.Item;
            }
        }

        public TValue TryGet(TKey key)
        {
            lock (dictionary)
            {
                if (dictionary.TryGetValue(key, out Trackable trackable))
                {
                    if (trackable.State == TrackState.Deleted) return null;
                    return trackable.Item;
                }
                TValue value = GetInternal(key);
                if (value == null)
                    return null;
                dictionary.Add(key, new Trackable
                {
                    Key = key,
                    Item = value,
                    State = TrackState.None
                });
                return value;
            }
        }
    }
}
