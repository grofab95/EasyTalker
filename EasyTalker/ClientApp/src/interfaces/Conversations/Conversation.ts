import ConversationParticipant from './ConversationParticipant'
import Message from '../Messages/Message'

export default interface Conversation {
    creatorId: string,
    participants: ConversationParticipant[],
    id: number,
    title: string,
    lastSeenAt: Date,
    lastMessage: Message
}