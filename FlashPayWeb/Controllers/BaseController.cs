using System;
using System.Threading.Tasks;
using FlashPayWeb.Apis;
using FlashPayWeb.libs;
using FlashPayWeb.RPC;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlashPayWeb.Controllers
{
    [Route("[controller]")]
    public class BaseController : Controller
    {

        //接口返回最大忍受时间，超过则记录日志
        protected int logExeTimeMax = 15;
        /*
        Api api = new Api("mainnet");
        */
        protected ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(BaseController));

        protected virtual BaseApi api { get { return new BaseApi("Base"); } }

        [HttpGet]
        public JsonResult Get(string @jsonrpc, string @method, string @params, long @id)
        {
            JsonRPCrequest req = null;
            DateTime start = DateTime.Now;

            try
            {
                req = new JsonRPCrequest
                {
                    jsonrpc = @jsonrpc,
                    method = @method,
                    @params = JsonConvert.DeserializeObject<object[]>(JsonConvert.SerializeObject(JArray.Parse(@params))),
                    id = @id
                };

                string ipAddr = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                var result = Json(api.getRes(req, ipAddr));
                if (DateTime.Now.Subtract(start).TotalSeconds > logExeTimeMax)
                {
                    log.Info(LogHelper.logInfoFormat(req, result, start));
                }
                return result;
            }
            catch (Exception e)
            {
                JsonPRCresponse_Error resE = new JsonPRCresponse_Error(0, -100, "Parameter Error", e.Message);

                var result = Json(resE);
                log.Error(LogHelper.logInfoFormat(req, result, start));
                return Json(result);

            }
        }

        [HttpPost]
        public async Task<JsonResult> Post()
        {
            JsonRPCrequest req = null;
            DateTime start = DateTime.Now;

            try
            {
                var ctype = HttpContext.Request.ContentType;
                FormData form = null;
                if (ctype == "application/x-www-form-urlencoded" ||
                         (ctype.IndexOf("multipart/form-data;", StringComparison.CurrentCulture) == 0))
                {
                    form = await FormData.FromRequest(HttpContext.Request);
                    var _jsonrpc = form.mapParams["jsonrpc"];
                    var _id = long.Parse(form.mapParams["id"]);
                    var _method = form.mapParams["method"];
                    var _strparams = form.mapParams["params"];
                    var _params = JArray.Parse(_strparams);
                    req = new JsonRPCrequest
                    {
                        jsonrpc = _jsonrpc,
                        method = _method,
                        @params = JsonConvert.DeserializeObject<object[]>(JsonConvert.SerializeObject(_params)),
                        id = _id
                    };
                }
                else
                {
                    var text = await FormData.GetStringFromRequest(HttpContext.Request);
                    req = JsonConvert.DeserializeObject<JsonRPCrequest>(text);
                }

                string ipAddr = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                var result = Json(api.getRes(req, ipAddr));
                if (DateTime.Now.Subtract(start).TotalSeconds > logExeTimeMax)
                {
                    log.Info(LogHelper.logInfoFormat(req, result, start));
                }
                return result;
            }
            catch (Exception e)
            {
                JsonPRCresponse_Error resE = new JsonPRCresponse_Error(0, -100, "Parameter Error", e.Message);

                var result = Json(resE);
                log.Error(LogHelper.logInfoFormat(req, result, start));
                return Json(result);
            }
        }
    }
}