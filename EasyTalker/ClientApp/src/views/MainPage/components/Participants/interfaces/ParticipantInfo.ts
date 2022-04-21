import { ConversationAccessStatus } from "../../../../../interfaces/Conversations/ConversationAccessStatus";
import User from "../../../../../interfaces/Users/User";

export default interface ParticipantInfo {
    user: User,
    accessStatus: ConversationAccessStatus
}