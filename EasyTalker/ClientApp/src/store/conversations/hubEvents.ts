import { withCallbacks } from 'redux-signalr'
import Message from '../../interfaces/Messages/Message'
import conversationSlice from './state'
import Conversation from '../../interfaces/Conversations/Conversation'

export const conversationCallbacks = withCallbacks()
    .add('ConversationCreated', (conversation: Conversation) => dispatch => {
        console.log('ConversationCreated', JSON.stringify(conversation))
        dispatch(conversationSlice.actions.conversationCreated(conversation))
    })
    .add('ConversationUpdated', (conversation: Conversation) => dispatch => {
        console.log('ConversationUpdated', JSON.stringify(conversation))
        dispatch(conversationSlice.actions.conversationUpdated(conversation))
    })
    .add('MessageCreated', (message: Message) => dispatch => {
        console.log('MessageCreated', JSON.stringify(message))
        dispatch(conversationSlice.actions.messageCreated(message))
    }).callbackMap