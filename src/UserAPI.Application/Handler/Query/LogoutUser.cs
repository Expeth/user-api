using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using UserAPI.Application.Common.Model.Result;
using UserAPI.Domain.ValueObject;
using OneOf;
using Serilog;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Application.Common.Extension;
using UserAPI.Application.Common.Model.Constant;
using UserAPI.Application.Handler.Command;

namespace UserAPI.Application.Handler.Query
{
    public static class LogoutUser
    {
        public sealed class Request : IRequest<OneOf<Response, ValidationFail, InternalError>>
        {
            public JwtClaims JwtClaims { get; }

            public Request(JwtClaims jwtClaims)
            {
                JwtClaims = jwtClaims;
            }
        }

        public sealed class Response
        {
            public Response()
            {
            }
        }
        
        public sealed class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(i => i.JwtClaims.UserId)
                    .NotNull()
                    .NotEmpty();

                RuleFor(i => i.JwtClaims.IssuedAt)
                    .NotNull()
                    .Must(i => i.IsPassed())
                    .WithMessage(ErrorMessage.InvalidJWT);
                
                RuleFor(i => i.JwtClaims.ExpiresAt)
                    .NotNull()
                    .Must(i => !i.IsPassed())
                    .WithMessage(ErrorMessage.InvalidJWT);
            }
        }
        
        public sealed class Handler : IRequestHandler<Request,
            OneOf<Response, ValidationFail, InternalError>>
        {
            private static readonly ILogger Logger = Log.ForContext(typeof(LogoutUser));
            
            private readonly IValidator<Request> _requestValidator;
            private readonly ISessionsRepository _sessionsRepository;

            public Handler(ISessionsRepository sessionsRepository,
                IValidator<Request> requestValidator)
            {
                _sessionsRepository = sessionsRepository;
                _requestValidator = requestValidator;
            }

            public async Task<OneOf<Response, ValidationFail, InternalError>> Handle(Request request, 
                CancellationToken cancellationToken)
            {
                try
                {
                    var requestValidationResult = await _requestValidator.ValidateAsync(request, cancellationToken);
                    if (!requestValidationResult.IsValid)
                        return ValidationFail.FromValidationResult(requestValidationResult);

                    var session = await _sessionsRepository.GetAsync(request.JwtClaims.SessionId);
                    if (session == null || session.UserId != request.JwtClaims.UserId)
                    {
                        return ValidationFail.FromMessage(ErrorMessage.InvalidSession);
                    }

                    if (session.EndTime.HasValue && session.EndTime.Value.IsPassed())
                    {
                        return ValidationFail.FromMessage(ErrorMessage.InvalidSession);
                    }

                    var isUpdated = await _sessionsRepository.SetEndedAsync(session.Id);

                    if (!isUpdated)
                    {
                        Logger.Error("Session wasn't able to update for userId: {userId}, sessionId: {sessionId}",
                            request.JwtClaims.UserId, request.JwtClaims.SessionId);
                        return InternalError.Default;
                    }

                    return new Response();
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Exception during logout for userId: {userId}, sessionId: {sessionId}",
                        request.JwtClaims.UserId, request.JwtClaims.SessionId);
                    return InternalError.Default;
                }
            }
        }
    }
}