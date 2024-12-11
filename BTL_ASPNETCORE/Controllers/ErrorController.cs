using Microsoft.AspNetCore.Mvc;

namespace BTL_ASPNETCORE.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            return statusCode switch
            {
                404 => View("Error"),
                _ => View("Error")
            };
        }
    }
}
