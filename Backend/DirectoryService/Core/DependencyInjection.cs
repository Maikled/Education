using Contracts.DTOs;
using Core.Decorators;
using Core.Departments.AddDepartmentLocation;
using Core.Departments.AddDepartmentPosition;
using Core.Departments.CreateDepartment;
using Core.Departments.DeleteDepartment;
using Core.Departments.RemoveDepartmentLocation;
using Core.Departments.RemoveDepartmentPosition;
using Core.Departments.UpdateDepartment;
using Core.Interfaces;
using Core.Locations.CreateLocation;
using Core.Locations.DeleteLocation;
using Core.Locations.UpdateLocation;
using Core.Positions.CreatePosition;
using Core.Positions.DeletePosition;
using Core.Positions.UpdatePosition;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IValidator<CreateLocationDto>, CreateLocationValidator>();
            services.AddScoped<IValidator<UpdateLocationDto>, UpdateLocationValidator>();
            services.AddScoped<IValidator<CreateDepartmentCommand>, CreateDepartmentValidator>();
            services.AddScoped<IValidator<UpdateDepartmentCommand>, UpdateDepartmentValidator>();
            services.AddScoped<IValidator<CreatePositionCommand>, CreatePositionValidator>();
            services.AddScoped<IValidator<UpdatePositionCommand>, UpdatePositionValidator>();

            services.AddScoped<ICommandHandler<Guid, CreateDepartmentCommand>, CreateDepartmentHandler>();
            services.AddScoped<ICommandHandler<UpdateDepartmentCommand>, UpdateDepartmentHandler>();
            services.AddScoped<ICommandHandler<DeleteDepartmentCommand>, DeleteDepartmentHandler>();
            services.AddScoped<ICommandHandler<AddDepartmentLocationCommand>, AddDepartmentLocationHandler>();
            services.AddScoped<ICommandHandler<RemoveDepartmentLocationCommand>, RemoveDepartmentLocationHandler>();
            services.AddScoped<ICommandHandler<AddDepartmentPositionCommand>, AddDepartmentPositionHandler>();
            services.AddScoped<ICommandHandler<RemoveDepartmentPositionCommand>, RemoveDepartmentPositionHandler>();

            services.AddScoped<ICommandHandler<Guid, CreateLocationCommand>, CreateLocationHandler>();
            services.AddScoped<ICommandHandler<UpdateLocationCommand>, UpdateLocationHandler>();
            services.AddScoped<ICommandHandler<DeleteLocationCommand>, DeleteLocationHandler>();

            services.AddScoped<ICommandHandler<Guid, CreatePositionCommand>, CreatePositionHandler>();
            services.AddScoped<ICommandHandler<UpdatePositionCommand>, UpdatePositionHandler>();
            services.AddScoped<ICommandHandler<DeletePositionCommand>, DeletePositionHandler>();

            services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationResultCommandHandlerDecorator<,>));
            services.Decorate(typeof(ICommandHandler<>), typeof(ValidationUnitResultCommandHandlerDecorator<>));

            return services;
        }
    }
}
