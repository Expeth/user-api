using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Application.Handler.Command;
using UserAPI.Contracts.Request;
using UserAPI.Contracts.Response;

namespace UserAPI.Host.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
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
                response => Ok(response),
                validationFail => BadRequest(validationFail),
                internalError => StatusCode(500, internalError));
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateUserRequest request)
        {
            var domainRequest = new AuthenticateUser.Request(request.Login, request.Password);

            var domainResponse = await _mediator.Send(domainRequest);
            return domainResponse.Match(response =>
                Ok(new AuthenticateUserResponse(response.Jwt, response.RenewToken)),
                validationFail => BadRequest(validationFail),
                invalidCredentials => BadRequest(invalidCredentials),
                internalError => StatusCode(500, internalError));
        }
    }
}