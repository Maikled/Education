using Contracts.DTOs;
using Core.Departments.Interfaces;
using Core.Departments.Services;
using Core.Departments.Validators;
using Core.Locations.Interfaces;
using Core.Locations.Services;
using Core.Locations.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IValidator<CreateLocationDto>, CreateLocationValidator>();
            services.AddScoped<IValidator<CreateDepartmentDto>, CreateDepartmentValidator>();

            services.AddScoped<ILocationsService, LocationsService>();
            services.AddScoped<IDepartmentService, DepartmentsService>();

            return services;
        }
    }
}
