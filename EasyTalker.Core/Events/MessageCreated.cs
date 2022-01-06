using EasyTalker.Core.Dto.Message;

namespace EasyTalker.Core.Events;

public record MessageCreated(MessageDto Message);