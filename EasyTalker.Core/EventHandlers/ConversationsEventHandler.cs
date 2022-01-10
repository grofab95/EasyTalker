using Easy.MessageHub;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Events;

namespace EasyTalker.Core.EventHandlers;

public class ConversationsEventHandler : IEventHandler
{
    private readonly IWebUiNotifier _webUiNotifier;

    public ConversationsEventHandler(IWebUiNotifier webUiNotifier)
    {
        _webUiNotifier = webUiNotifier;
    }
    
    public void Subscribe(IMessageHub messageHub)
    {
        messageHub.Subscribe<MessageCreated>(m => _webUiNotifier.MessageCreated(m.Message));
        messageHub.Subscribe<ConversationCreated>(m => _webUiNotifier.ConversationCreated(m.Conversation));
        messageHub.Subscribe<ConversationUpdated>(m => _webUiNotifier.ConversationUpdated(m.Conversation));
    }
}