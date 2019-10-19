using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Common.Extensions
{
    public static class MediatRExtensions
    {
        public static void AddMediatR(this IServiceCollection services)
        {
            services.AddTransient<IMediator, Mediator>();
            services.AddTransient<ServiceFactory>(sp => sp.GetService);
        }
    }
}
