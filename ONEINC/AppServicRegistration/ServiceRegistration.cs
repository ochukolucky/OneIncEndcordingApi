using Application.Implementation;
using Application.Interface;

namespace ONEINC.AppServicRegistration
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register my services here

            services.AddScoped<IEncoderService, EncoderService>();
            // Add more service registrations as needed

            return services;
        }

    }
}
