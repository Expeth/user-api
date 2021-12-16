using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Application.Common.Model.Result;
using OneOf;
using Serilog;
using UserAPI.Application.Common.Abstraction.Service;
using UserAPI.Application.Common.Model.Constant;
using UserAPI.Domain.Entity;

namespace UserAPI.Application.Handler.Command
{
    public static class AuthenticateUser
    {
        public sealed class Request : IRequest<OneOf<Response, ValidationFail, InternalError>>
        {
            public string Login { get; }
            public string Password { get; }

            public Request(string login, string password)
            {
                Login = login.ToLower();
                Password = password;
            }
        }

        public sealed class Response
        {
            public string Jwt { get; }
            public string RefreshToken { get; }

            public Response(string jwt, string refreshToken)
            {
                Jwt = jwt;
                RefreshToken = refreshToken;
            }
        }

        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(i => i.Login)
                    .NotNull()
                    .NotEmpty()
                    .Must(i => i.Length < 30);
                
                RuleFor(i => i.Password)
                    .NotNull()
                    .NotEmpty()
                    .Must(i => i.Length < 30);
            }
        }

        public sealed class Handler : IRequestHandler<Request,
            OneOf<Response, ValidationFail, InternalError>>
        {
            private static readonly ILogger Logger = Log.ForContext(typeof(AuthenticateUser));
            
            private readonly IJwtFactory _jwtFactory;
            private readonly IValidator<Request> _validator;
            private readonly IRefreshTokenFactory _refreshTokenFactory;
            private readonly IRefreshTokenRepository _refreshTokenRepository;
            private readonly IUserRepository _userRepository;
            private readonly IPasswordService _passwordService;

            public Handler(
                IJwtFactory jwtFactory,
                IUserRepository userRepository,
                IPasswordService passwordService,
                IRefreshTokenRepository refreshTokenRepository,
                IRefreshTokenFactory refreshTokenFactory,
                IValidator<Request> validator)
            {
                _jwtFactory = jwtFactory;
                _userRepository = userRepository;
                _passwordService = passwordService;
                _refreshTokenRepository = refreshTokenRepository;
                _refreshTokenFactory = refreshTokenFactory;
                _validator = validator;
            }

            public async Task<OneOf<Response, ValidationFail, InternalError>> Handle(
                Request request,
                CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid) 
                    return ValidationFail.FromValidationResult(validationResult);

                try
                {
                    var user = await _userRepository.GetAsync(request.Login);
                    if (user == null)
                    {
                        return ValidationFail.FromMessage(ErrorMessage.InvalidCredentials);
                    }
                    
                    if (!_passwordService.ValidateHash(user.PasswordHash, request.Password, user.PasswordSalt))
                    {
                        return ValidationFail.FromMessage(ErrorMessage.InvalidCredentials);
                    }

                    var jwt = await _jwtFactory.CreateAsync(new SessionEntity(user.Id));
                    var refreshToken = _refreshTokenFactory.Create(user.Id);
                    await _refreshTokenRepository.CreateAsync(refreshToken);
                    
                    return new Response(jwt, refreshToken.Id);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Exception during processing request for login: {login}", request.Login);
                    return InternalError.Default;
                }
            }
        }
    }
}