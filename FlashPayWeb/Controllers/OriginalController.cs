using FlashPayWeb.Apis;
using Microsoft.AspNetCore.Mvc;

namespace FlashPayWeb.Controllers
{
    [Route("[controller]")]
    public class OriginalController : BaseController
    {
        protected override BaseApi api { get { return new OriginalApi("Original"); } }
    }
}