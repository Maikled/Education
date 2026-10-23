using Contracts.DTOs;
using Core.Departments.AddDepartmentLocation;
using Core.Departments.CreateDepartment;
using Core.Departments.RemoveDepartmentLocation;
using Core.Departments.UpdateDepartment;
using Core.Interfaces;
using Core.Locations.CreateLocation;
using Core.Locations.UpdateLocation;
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
            services.AddScoped<IValidator<CreateDepartmentDto>, CreateDepartmentValidator>();
            services.AddScoped<IValidator<UpdateDepartmentDto>, UpdateDepartmentValidator>();

            services.AddScoped<ICommandHandler<Guid, CreateDepartmentCommand>, CreateDepartmentHandler>();
            services.AddScoped<ICommandHandler<UpdateDepartmentCommand>, UpdateDepartmentHandler>();
            services.AddScoped<ICommandHandler<AddDepartmentLocationCommand>, AddDepartmentLocationHandler>();
            services.AddScoped<ICommandHandler<RemoveDepartmentLocationCommand>, RemoveDepartmentLocationHandler>();

            services.AddScoped<ICommandHandler<Guid, CreateLocationCommand>, CreateLocationHandler>();
            services.AddScoped<ICommandHandler<UpdateLocationCommand>, UpdateLocationHandler>();

            return services;
        }
    }
}
