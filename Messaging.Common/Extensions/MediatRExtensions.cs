namespace Messaging.Common.Extensions
{
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;

    public static class MediatRExtensions
    {
        public static void ConfigureMediatR(this IServiceCollection services)
        {
            services.AddTransient<IMediator, Mediator>();
            services.AddTransient<ServiceFactory>(sp => sp.GetService);
        }
    }
}
