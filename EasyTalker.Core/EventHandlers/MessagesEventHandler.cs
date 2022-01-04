using Easy.MessageHub;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Events;

namespace EasyTalker.Core.EventHandlers;

public class MessagesEventHandler : IEventHandler
{
    private readonly IWebUiNotifier _webUiNotifier;

    public MessagesEventHandler(IWebUiNotifier webUiNotifier)
    {
        _webUiNotifier = webUiNotifier;
    }
    
    public void Subscribe(IMessageHub messageHub)
    {
        messageHub.Subscribe<MessageChanged>(OnMessageChanged);
    }

    private void OnMessageChanged(MessageChanged message)
    {
        _webUiNotifier.MessageChanged(message.Message);
    }
}