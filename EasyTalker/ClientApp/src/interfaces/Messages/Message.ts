import User from '../Users/User'

export default interface Message {
    id: number,
    conversationId: number,
    sender: User | undefined,
    text: string,
    createdAt: Date,
    isImage: boolean
}