using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Application.Common.Model.Result;
using OneOf;
using UserAPI.Application.Common.Abstraction.Service;
using UserAPI.Domain.Entity;

namespace UserAPI.Application.Handler.Command
{
    public static class AuthenticateUser
    {
        public sealed class Request : IRequest<OneOf<Response, ValidationFail, InvalidCredentials, InternalError>>
        {
            public string Login { get; }
            public string Password { get; }

            public Request(string login, string password)
            {
                Login = login;
                Password = password;
            }
        }

        public sealed class Response
        {
            public string Jwt { get; }
            public string RenewToken { get; }

            public Response(string jwt, string renewToken)
            {
                Jwt = jwt;
                RenewToken = renewToken;
            }
        }

        //TODO: add validator for request

        public sealed class Handler : IRequestHandler<Request,
            OneOf<Response, ValidationFail, InvalidCredentials, InternalError>>
        {
            private readonly IJwtFactory _jwtFactory;
            private readonly IUserRepository _userRepository;
            private readonly IPasswordService _passwordService;

            public Handler(IJwtFactory jwtFactory, IUserRepository userRepository,
                IPasswordService passwordService)
            {
                _jwtFactory = jwtFactory;
                _userRepository = userRepository;
                _passwordService = passwordService;
            }

            public async Task<OneOf<Response, ValidationFail, InvalidCredentials, InternalError>> Handle(
                Request request,
                CancellationToken cancellationToken)
            {
                //TODO: validate request

                try
                {
                    var user = await _userRepository.GetUserAsync(request.Login);
                    if (user == null)
                    {
                        //User not found
                        return new InvalidCredentials();
                    }
                    
                    if (!_passwordService.ValidateHash(user.PasswordHash, request.Password, user.PasswordSalt))
                    {
                        //Password doesn't match
                        return new InvalidCredentials();
                    }

                    var jwt = await _jwtFactory.CreateAsync(new SessionEntity(user.Id));
                    //TODO: implement renew token
                    return new Response(jwt, null);
                }
                catch (Exception e)
                {
                    //TODO: add logging
                    return InternalError.FromException(e);
                }
            }
        }
    }
}