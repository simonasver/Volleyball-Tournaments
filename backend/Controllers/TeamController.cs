using Backend.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Authorize(Policy = PolicyNames.ResourceOwner)]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        
    }
}
