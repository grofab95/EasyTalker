using EasyTalker.Core.Dto.Message;

namespace EasyTalker.Core.Adapters;

public interface IWebUiNotifier
{
    void MessageChanged(MessageDto message);
}