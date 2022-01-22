import User from '../Users/User'

export default interface Message {
    id: number,
    conversationId: number,
    sender: User,
    text: string,
    status: string,
    createdAt: Date
}