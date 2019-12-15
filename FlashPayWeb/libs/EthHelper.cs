using Newtonsoft.Json.Linq;
using FlashPayWeb.IO;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FlashPayWeb.libs
{
    public class EthHelper
    {
        string url;

        public EthHelper(string url)
        {
            this.url = url;
        }

        public string PackPostDataStr(string method, JArray @params)
        {
            JObject postData = new JObject();
            postData.Add("jsonrpc", "2.0");
            postData.Add("method", method);
            postData.Add("params", @params);
            postData.Add("id", 1);
            return Newtonsoft.Json.JsonConvert.SerializeObject(postData);

        }

        public object ProcessResult(string result)
        {
            JObject json = JObject.Parse(result);
            if (json.ContainsKey("result"))
                return json["result"];
            else if (json.ContainsKey("error"))
                throw new FormatException((string)json["error"]);
            else
                throw new FormatException();

        }

        public bool IsBolckExist(uint blockNumber)
        {
            string postDataStr = PackPostDataStr("eth_getBlockByNumber", new JArray() { blockNumber.ToString("x").FormatHexStr(),true });
            string result = HttpHelper.Post(url, postDataStr);
            return ProcessResult(result) != null;
        }

        public uint GetBlockTransactionCountByNumber(uint blockNumber)
        {
            string postDataStr = PackPostDataStr("eth_getBlockTransactionCountByNumber", new JArray() { blockNumber.ToString("x").FormatHexStr() });
            string result = HttpHelper.Post(url, postDataStr);
            return Convert.ToUInt32(ProcessResult(result).ToString(),16);
        }

        public UInt256 GetTransactionHashByBlockNumberAndIndex(uint blockNumber, uint index)
        {
            string postDataStr = PackPostDataStr(
                "eth_getTransactionByBlockNumberAndIndex",
                new JArray() { blockNumber.ToString("x").FormatHexStr(), index.ToString("x").FormatHexStr() }
                );
            string result = HttpHelper.Post(url, postDataStr);
            return new UInt256(((JObject)ProcessResult(result))["hash"].ToString());
        }



        public UInt256 GetSha3(string hexStr)
        {
            hexStr = hexStr.FormatHexStr();
            string postDataStr = PackPostDataStr("web3_sha3", new JArray() { hexStr });
            string result = HttpHelper.Post(url, postDataStr);
            return new UInt256(ProcessResult(result).ToString());
        }
    }
}
