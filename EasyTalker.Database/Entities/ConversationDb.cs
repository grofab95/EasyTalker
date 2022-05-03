using EasyTalker.Core.Enums;
using EasyTalker.Database.Helpers;

namespace EasyTalker.Database.Entities;

public class ConversationDb : EntityDb
{
    public string CreatorId { get; set; }
    public string Title { get; set; }
    public ConversationStatus Status { get; set; }
}