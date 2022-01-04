using System.Threading.Tasks;
using EasyTalker.Core.Dto.Message;

namespace EasyTalker.Core.Adapters;

public interface IMessageStore
{
    Task<MessageDto> Add(MessageCreateDto messageCreateDto);
}