using Contracts.Errors;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http.Metadata;
using System.Reflection;
using Web.Endpoints.Helpers;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Web.Endpoints.Models
{
    internal class EndpointResult : IResult, IEndpointMetadataProvider
    {
        private readonly UnitResult<AppError> _result;

        public EndpointResult(UnitResult<AppError> result)
        {
            _result = result;
        }

        private IResult CreateResult()
        {
            return _result.IsSuccess ? CreateSuccessResult(_result) : CreateErrorResult(_result);
        }

        protected virtual IResult CreateSuccessResult(UnitResult<AppError> result) => new SuccessResult<object>(StatusCodes.Status200OK, null);
        protected virtual IResult CreateErrorResult(UnitResult<AppError> result) => new ErrorResult(result.Error);

        public Task ExecuteAsync(HttpContext httpContext) => CreateResult().ExecuteAsync(httpContext);

        public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(method);
            ArgumentNullException.ThrowIfNull(builder);

            EndpointMetadata.AddSuccess<object>(builder, StatusCodes.Status200OK);
            EndpointMetadata.AddErrors(builder);
        }

        public static EndpointResult Empty = new EndpointResult(UnitResult.Success<AppError>());
        public static implicit operator EndpointResult(UnitResult<AppError> result) => new EndpointResult(result);
    }

    internal class EndpointResult<TValue> : IResult, IEndpointMetadataProvider
    {
        private readonly Result<TValue, AppError> _result;

        public EndpointResult(Result<TValue, AppError> result)
        {
            _result = result;
        }

        private IResult CreateResult()
        {
            return _result.IsSuccess? CreateSuccessResult(_result) : CreateErrorResult(_result);
        }

        protected virtual IResult CreateSuccessResult(Result<TValue, AppError> result) => new SuccessResult<TValue>(StatusCodes.Status200OK, result.Value);
        protected virtual IResult CreateErrorResult(Result<TValue, AppError> result) => new ErrorResult(result.Error);

        public Task ExecuteAsync(HttpContext httpContext) => CreateResult().ExecuteAsync(httpContext);

        public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(method);
            ArgumentNullException.ThrowIfNull(builder);

            EndpointMetadata.AddSuccess<object>(builder, StatusCodes.Status200OK);
            EndpointMetadata.AddErrors(builder);
        }

        public static implicit operator EndpointResult<TValue>(Result<TValue, AppError> result) => new EndpointResult<TValue>(result);
    }
}
