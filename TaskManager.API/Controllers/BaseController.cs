using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        // For queries — success returns 200 OK
        protected IActionResult OkOrNotFound<T>(Result<T> result)
            => result.IsSuccess
                ? Ok(result.Value)
                : NotFound(new { message = result.Error });

        // For commands that create — success returns 201 Created
        protected IActionResult CreatedOrBadRequest<T>(Result<T> result, string actionName, object routeValues)
            => result.IsSuccess
                ? CreatedAtAction(actionName, routeValues, result.Value)
                : BadRequest(new { message = result.Error });

        // For commands that don't return data — success returns 204 No Content
        protected IActionResult NoContentOrNotFound(Result result)
            => result.IsSuccess
                ? NoContent()
                : NotFound(new { message = result.Error });

        // For commands that fail due to bad input
        protected IActionResult NoContentOrBadRequest(Result result)
            => result.IsSuccess
                ? NoContent()
                : BadRequest(new { message = result.Error });
    }
}
