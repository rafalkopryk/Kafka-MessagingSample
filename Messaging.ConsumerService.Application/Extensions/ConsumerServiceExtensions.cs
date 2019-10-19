using MediatR;
using Messaging.ConsumerService.Application.Handlers.Event;
using Messaging.ConsumerService.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.ConsumerService.Application.Extensions
{
    public static class ConsumerServiceExtensions
    {
        public static void AddConsumerServiceEventHandlers(this IServiceCollection services)
        {
            services.AddTransient<INotificationHandler<MessagePublishedEvent>, MessagePublishedEventHandler>();
        }
    }
}
