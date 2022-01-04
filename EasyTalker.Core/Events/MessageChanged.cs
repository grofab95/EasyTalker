using EasyTalker.Core.Dto.Message;

namespace EasyTalker.Core.Events;

public record MessageChanged(MessageDto Message);