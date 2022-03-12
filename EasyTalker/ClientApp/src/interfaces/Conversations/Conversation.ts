import ConversationParticipant from './ConversationParticipant'
import Message from '../Messages/Message'
import {ConversationStatus} from "./ConversationStatus";

export default interface Conversation {
    creatorId: string,
    participants: ConversationParticipant[],
    id: number,
    title: string,
    lastSeenAt: Date,
    lastMessage: Message,
    status: ConversationStatus
}