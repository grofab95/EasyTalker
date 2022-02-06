using Easy.MessageHub;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Events;

namespace EasyTalker.Core.EventHandlers;

public class FilesEventHandler : IEventHandler
{
    private readonly IWebUiNotifier _webUiNotifier;

    public FilesEventHandler(IWebUiNotifier webUiNotifier)
    {
        _webUiNotifier = webUiNotifier;
    }

    public void Subscribe(IMessageHub messageHub)
    {
        messageHub.Subscribe<FileUploaded>(m => _webUiNotifier.FileUploaded(m.File));
    }
}