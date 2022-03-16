import {ConversationAccessStatus} from "./ConversationAccessStatus";

export default interface ConversationParticipant {
    id: string,
    accessStatus: ConversationAccessStatus
}