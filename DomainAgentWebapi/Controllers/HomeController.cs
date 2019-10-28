using Microsoft.AspNetCore.Mvc;

namespace DomainAgentWebapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [Route("[action]")]
        public string Index()
        {
            return "success";
        }

    }
}
