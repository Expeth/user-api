using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Application.Handler.Command;
using UserAPI.Application.Handler.Query;
using UserAPI.Contracts.Request;
using UserAPI.Contracts.Response;
using UserAPI.Host.Extensions;

namespace UserAPI.Host.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : GeneralController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserRequest request)
        {
            var domainRequest = new RegisterUser.Request(request.Email, request.Username, request.Password,
                request.FirstName,
                request.LastName, request.MiddleName);
            
            var domainResponse = await _mediator.Send(domainRequest);
            return domainResponse.Match(
                response => Ok(new RegisterUserResponse(response.Id)), BadRequest, Conflict, InternalError);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateUserRequest request)
        {
            var domainRequest = new AuthenticateUser.Request(request.Login, request.Password);

            var domainResponse = await _mediator.Send(domainRequest);
            return domainResponse.Match(response =>
                Ok(new AuthenticateUserResponse(response.Jwt, response.RefreshToken)), BadRequest, InternalError);
        }
        
        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshJwtRequest request)
        {
            var domainRequest = new RefreshJwt.Request(HttpContext.User.Claims.ToClaimsObject(), request.RefreshToken);

            var domainResponse = await _mediator.Send(domainRequest);
            return domainResponse.Match(response =>
                    Ok(new RefreshJwtResponse(response.Jwt, response.RefreshToken)), BadRequest, InternalError);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var domainRequest = new LogoutUser.Request(HttpContext.User.Claims.ToClaimsObject());

            var domainResponse = await _mediator.Send(domainRequest);
            return domainResponse.Match(Ok, BadRequest, InternalError);
        }
    }
}