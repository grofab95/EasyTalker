using Easy.MessageHub;

namespace EasyTalker.Core.EventHandlers;

public interface IEventHandler
{
    void Subscribe(IMessageHub messageHub);
}