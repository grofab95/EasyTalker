using Easy.MessageHub;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Events;

namespace EasyTalker.Core.EventHandlers;

public class UsersEventHandler : IEventHandler
{
    private readonly IWebUiNotifier _webUiNotifier;

    public UsersEventHandler(IWebUiNotifier webUiNotifier)
    {
        _webUiNotifier = webUiNotifier;
    }
    
    public void Subscribe(IMessageHub messageHub)
    {
        messageHub.Subscribe<UserRegistered>(OnUserRegistered);
    }

    private void OnUserRegistered(UserRegistered message)
    {
        _webUiNotifier.UserRegistered(message.User);
    }
}