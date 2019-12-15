using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlashPayWeb.libs
{
    public class Monitor
    {
        /**
         * 统计
         */

        ConcurrentDictionary<string, long> dict = new ConcurrentDictionary<string, long>();


        public Monitor()
        {
            new Task(() => calcTps()).Start();
        }

        private void calcTps()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(1000);
                    process();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private void process()
        {
            DateTime now = DateTime.Now;
            var tmpDict = new Dictionary<string, long>(dict);

            var keys = new List<string>();
            keys.AddRange(tmpDict.Keys.ToArray());
            if (keys.Count == 0) return;

            //
            StringBuilder sb = new StringBuilder();
            long sum = 0;
            foreach (var key in keys.Distinct())
            {
                long t = tmpDict.GetValueOrDefault(key, 0);
                if (t == 0) continue;

                if (t > 0) dict.AddOrUpdate(key, 0, (k, oldV) => oldV - t);
                sum += t;
                sb.Append(key.PadLeft(48)).Append(": ").Append(t.ToString().PadLeft(6)).Append("/s, ").Append("\r\n");
            }
            if (sum == 0) return;

            sb.Append("sum:" + sum.ToString().PadLeft(6) + "\t");

            //
            log(now, sb);
        }

        private void log(DateTime now, StringBuilder sb)
        {
            string path = string.Format("tps_{0}.txt", now.ToString("yyyyMMdd"));
            string data = sb.Append(now.ToString("yyyyMMdd HH:mm:ss.fff")).Append("\r\n").ToString();
            data = data.Replace("\r\nsum:", "\tsum:");
            File.AppendAllText(path, data);
        }

        public void point(string method)
        {
            dict.AddOrUpdate(method, 1, (k, oldv) => oldv + 1);
        }
    }
}
