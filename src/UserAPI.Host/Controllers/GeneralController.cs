using Microsoft.AspNetCore.Mvc;

namespace UserAPI.Host.Controllers
{
    public abstract class GeneralController : ControllerBase
    {
        protected IActionResult InternalError(object error) => StatusCode(500, error);
    }
}