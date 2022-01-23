using System.Collections.Generic;
using Easy.MessageHub;
using EasyTalker.Core.EventHandlers;

namespace EasyTalker.Core.Events;

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