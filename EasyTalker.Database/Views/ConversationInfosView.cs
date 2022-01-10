namespace EasyTalker.Database.Views;

public class ConversationInfosView
{
    public long ConversationId { get; set; }
    public string Title { get; set; }
    public string CreatorId { get; set; }
    public string ConversationParticipantsString { get; set; }
}