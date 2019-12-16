using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlashPayWeb.libs
{
    public class CrawlerHelper
    {
        string url;

        public CrawlerHelper(string url)
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

        public bool AddCareAddress(UInt160 ddress)
        {
            string postDataStr = PackPostDataStr("addCareAddr", new JArray() { ddress.ToString()});
            string result = HttpHelper.Post(url, postDataStr);
            return true;
        }
    }
}
