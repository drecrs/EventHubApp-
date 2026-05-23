using CommunityToolkit.Mvvm.Messaging.Messages;

namespace EventHubApp.Messages
{
    public class EventUpdatedMessage : ValueChangedMessage<int>
    {
        public EventUpdatedMessage(int eventId) : base(eventId)
        {
        }
    }
}
