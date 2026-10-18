using Contracts.Errors;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http.Metadata;
using System.Reflection;
using Web.Endpoints.Helpers;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Web.Endpoints.Models
{
    internal class CreatedEndpointResult<TValue> : EndpointResult<TValue>, IEndpointMetadataProvider
    {
        public CreatedEndpointResult(Result<TValue, AppError> result) : base(result)
        {
        }

        protected override IResult CreateSuccessResult(Result<TValue, AppError> result) => new SuccessResult<TValue>(StatusCodes.Status201Created, result.Value);

        public static implicit operator CreatedEndpointResult<TValue>(Result<TValue, AppError> result) => new CreatedEndpointResult<TValue>(result);

        public static new void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(method);
            ArgumentNullException.ThrowIfNull(builder);

            EndpointMetadata.AddSuccess<TValue>(builder, StatusCodes.Status201Created);
            EndpointMetadata.AddErrors(builder);
        }
    }
}
