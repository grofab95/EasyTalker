using System.Collections.Generic;
using Easy.MessageHub;

namespace EasyTalker.Core.EventHandlers;

public class EventHandlerCollector
{
    private readonly IEnumerable<IEventHandler> _handlers;
    private readonly IMessageHub _messageHub;

    public EventHandlerCollector(IEnumerable<IEventHandler> handlers, IMessageHub messageHub)
    {
        _handlers = handlers;
        _messageHub = messageHub;
    }

    public void RegisterHandlers()
    {
        foreach (var handler in _handlers)
        {
            handler.Subscribe(_messageHub);
        }
    }
}