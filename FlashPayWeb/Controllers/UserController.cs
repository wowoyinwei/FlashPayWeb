using FlashPayWeb.Apis;
using Microsoft.AspNetCore.Mvc;


namespace FlashPayWeb.Controllers
{
    [Route("[controller]")]
    public class UserController : BaseController
    {
        protected override BaseApi api { get { return new UserApi("User"); } }
    }
}
