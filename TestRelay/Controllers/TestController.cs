using Microsoft.AspNetCore.Mvc;
using TestRelay.Models;

namespace TestRelay.Controllers
{
    [Route("[controller]/[action]")]
    public sealed class TestController : Controller
    {
        public TestController() { }

        [HttpGet]
        public IActionResult QueryForThings()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult DoSignificantWork([FromBody] SomeUnitOfWork body)
        {
            return Ok(new SomeUnitOfWorkResponse { Response = $"Hello {body.ImportantMessage}" });
        }
    }
}
