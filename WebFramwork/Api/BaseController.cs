using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Filters;

namespace WebFramework.Api
{

    [ApiController]
    [Authorize]
    [ApiResultFilter]
    [Route("api/v{version:ApiVersion}/[controller]")]
    public class BaseController : ControllerBase
    {

    }
}