namespace EasyTalker.Database.Entities;

public class UserConversationDb : EntityDb
{
    public string UserId { get; set; }
    public long ConversationId { get; set; }

    public UserConversationDb(string userId, long conversationId)
    {
        UserId = userId;
        ConversationId = conversationId;
    }
}