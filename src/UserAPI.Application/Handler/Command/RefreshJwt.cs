using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using UserAPI.Application.Common.Model.Result;
using OneOf;
using Serilog;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Application.Common.Extension;
using UserAPI.Application.Common.Model.Constant;
using UserAPI.Domain.Entity;
using UserAPI.Domain.ValueObject;

namespace UserAPI.Application.Handler.Command
{
    public static class RefreshJwt
    {
        public sealed class Request : IRequest<OneOf<Response, ValidationFail, InternalError>>
        {
            public JwtClaims JwtClaims { get; }
            public string RefreshToken { get; }

            public Request(JwtClaims jwtClaims, string refreshToken)
            {
                JwtClaims = jwtClaims;
                RefreshToken = refreshToken;
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

        public sealed class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(i => i.RefreshToken)
                    .NotNull()
                    .NotEmpty();

                RuleFor(i => i.JwtClaims.UserId)
                    .NotNull()
                    .NotEmpty();

                RuleFor(i => i.JwtClaims.IssuedAt)
                    .NotNull()
                    .Must(i => i.IsPassed())
                    .WithMessage(ErrorMessage.InvalidJWT);
                
                RuleFor(i => i.JwtClaims.ExpiresAt)
                    .NotNull()
                    .Must(i => i.IsPassed())
                    .WithMessage(ErrorMessage.InvalidJWT);
            }
        }

        public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenEntity>
        {
            public RefreshTokenValidator()
            {
                RuleFor(i => i.IsUsed)
                    .Equal(_ => false)
                    .WithMessage(ErrorMessage.InvalidRefreshToken);
                
                RuleFor(i => i.IsDeclined)
                    .Equal(_ => false)
                    .WithMessage(ErrorMessage.InvalidRefreshToken);
                
                RuleFor(i => i.ExpiresAt)
                    .Must(i => !i.IsPassed())
                    .WithMessage(ErrorMessage.InvalidRefreshToken);
                
                RuleFor(i => i.IssuedAt)
                    .Must(i => i.IsPassed())
                    .WithMessage(ErrorMessage.InvalidRefreshToken);
            }
        }

        public sealed class Handler : IRequestHandler<Request,
            OneOf<Response, ValidationFail, InternalError>>
        {
            private static readonly ILogger Logger = Log.ForContext(typeof(RegisterUser));

            private readonly IRefreshTokenRepository _refreshTokenRepository;
            private readonly IRefreshTokenFactory _refreshTokenFactory;
            private readonly IValidator<RefreshTokenEntity> _rtValidator;
            private readonly ISessionsRepository _sessionsRepository;
            private readonly IValidator<Request> _requestValidator;
            private readonly ISessionFactory _sessionFactory;
            private readonly IJwtFactory _jwtFactory;
            
            public Handler(
                IRefreshTokenRepository refreshTokenRepository,
                IRefreshTokenFactory refreshTokenFactory,
                IJwtFactory jwtFactory,
                IValidator<Request> requestValidator,
                IValidator<RefreshTokenEntity> rtValidator,
                ISessionFactory sessionFactory,
                ISessionsRepository sessionsRepository)
            {
                _refreshTokenRepository = refreshTokenRepository;
                _refreshTokenFactory = refreshTokenFactory;
                _jwtFactory = jwtFactory;
                _requestValidator = requestValidator;
                _rtValidator = rtValidator;
                _sessionFactory = sessionFactory;
                _sessionsRepository = sessionsRepository;
            }

            public async Task<OneOf<Response, ValidationFail, InternalError>> Handle(
                Request request,
                CancellationToken cancellationToken)
            {
                var requestValidationResult = await _requestValidator.ValidateAsync(request, cancellationToken);
                if (!requestValidationResult.IsValid) 
                    return ValidationFail.FromValidationResult(requestValidationResult);
                
                try
                {
                    var session = await _sessionsRepository.GetAsync(request.JwtClaims.SessionId);
                    if (session == null || session.UserId != request.JwtClaims.UserId)
                    {
                        return ValidationFail.FromMessage(ErrorMessage.InvalidSession);
                    }

                    if (session.EndTime.HasValue && session.EndTime.Value.IsPassed())
                    {
                        return ValidationFail.FromMessage(ErrorMessage.InvalidSession);
                    }

                    var refreshToken = await _refreshTokenRepository.GetAsync(request.RefreshToken);
                    if (refreshToken == null)
                    {
                        return ValidationFail.FromMessage(ErrorMessage.InvalidRefreshToken);
                    }
                    
                    var rtValidationResult = await _rtValidator.ValidateAsync(refreshToken, cancellationToken);
                    if (!rtValidationResult.IsValid) 
                        return ValidationFail.FromValidationResult(rtValidationResult);

                    if (refreshToken.UserId != request.JwtClaims.UserId)
                    {
                        return ValidationFail.FromMessage(ErrorMessage.InvalidRefreshToken);
                    }

                    if (refreshToken.SessionId != session.Id)
                    {
                        return ValidationFail.FromMessage(ErrorMessage.InvalidRefreshToken);
                    }
                    
                    var isUpdated = await _refreshTokenRepository.SetUsedAsync(refreshToken.Id);
                    
                    if (!isUpdated)
                    {
                        Logger.Error("Refresh Token wasn't able to update for userId: {userId}, tokenId: {tokenId}",
                            request.JwtClaims.UserId, request.RefreshToken);
                        return InternalError.Default;
                    }

                    var jwt = await _jwtFactory.CreateAsync(session);
                    var newRefreshToken = _refreshTokenFactory.Create(request.JwtClaims.UserId, session.Id);

                    await _refreshTokenRepository.CreateAsync(newRefreshToken);

                    return new Response(jwt, newRefreshToken.Id);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Exception during refreshing JWT for userId: {userId}, tokenId: {tokenId}",
                        request.JwtClaims.UserId, request.RefreshToken);
                    return InternalError.Default;
                }
            }
        }
    }
}