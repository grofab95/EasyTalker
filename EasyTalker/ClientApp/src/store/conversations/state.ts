import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { errorNotification, successNotification } from '../../utils/notificationFactory'
import Conversation from '../../interfaces/Conversations/Conversation'
import {
    addMessage,
    addParticipants,
    createConversation,
    getMessages,
    getUserConversations,
    removeParticipants, updateLatSeenAt
} from './api'
import Message from '../../interfaces/Messages/Message'
import ConversationLastSeen from '../../interfaces/Conversations/ConversationLastSeen'
import conversationView from '../../views/MainPage/components/ConversationView'

interface ConversationMessages {
    conversationId: number,
    messages: Message[]   
}

export interface ConversationState {
    conversationList: Conversation[]
    messageList: ConversationMessages[]
    isBusy: boolean,
    selectedConversationId: number
}

const getDefaultState = () => {
    return {
        conversationList: [],
        messageList: [],
        isBusy: false,
        selectedConversationId: 0
    } as ConversationState
}

const conversationSlice = createSlice({
    name: 'conversation',
    initialState: getDefaultState(),
    reducers: {
        conversationCreated(state, action: PayloadAction<Conversation>) {
            const conversation = action.payload

            const index = state.conversationList.findIndex(x => x.id === conversation.id)
            if (index === -1) {
                state.conversationList.push(conversation)
            }
        },
        conversationUpdated(state, action: PayloadAction<Conversation>) {
            const conversation = action.payload

            const index = state.conversationList.findIndex(x => x.id === conversation.id)
            if (index !== -1) {
                state.conversationList.splice(index, 1)
            }
            state.conversationList.push(conversation)
        },
        messageCreated(state, action: PayloadAction<Message>) {
            const message = action.payload
                        
            const conversationMessages = state.messageList.find(x => x.conversationId === message.conversationId)?.messages            
            const index = conversationMessages?.findIndex(x => x.id === message.id)
            if (index === -1) {
                conversationMessages?.push(message)
            }
            else {               
                state.messageList.push({
                    conversationId: message.conversationId,
                    messages: new Array<Message>(message)
                } as ConversationMessages)
            }

            const conversation = state.conversationList.find(x => x.id == message.conversationId)
            if (conversation !== undefined) {
                conversation.lastMessage = message
            }
            
            // const conversation = state.conversationList.find(x => x.id == message.conversationId)
            // if (conversation !== undefined) {
            //     conversation.lastMessageAt = message.createdAt
            // }
            // console.log(state.selectedConversationId)
            // if (message.conversationId === state.selectedConversationId) {
            //     console.log('dupa')
            //     updateLatSeenAt({
            //             conversationId: message.conversationId,
            //             seenAt: new Date(Date.now())
            //         })
            // }
        }, //payload: { conversationId: number, seenAt: Date
        updateSelectedConversationId(state, action: PayloadAction<number>) {
            console.log(`updateSelectedConversationId id=${action.payload}`)
            state.selectedConversationId = action.payload
        }        
    },
    extraReducers: builder => {
        builder
            .addCase(createConversation.pending, (state) => {
                state.isBusy = true
            })
            .addCase(createConversation.fulfilled, (state, action: PayloadAction<Conversation>) => {
                successNotification(`The ${action.payload.title} conversation has been successfully created`)
                //state.conversationList.push(action.payload)
                state.isBusy = false
            })
            .addCase(createConversation.rejected, (state, action) => {
                errorNotification((action.payload as any).error ?? 'Server Error')
                state.isBusy = false
            })
            .addCase(getUserConversations.pending, (state) => {
                state.isBusy = true
            })
            .addCase(getUserConversations.fulfilled, (state, action: PayloadAction<Conversation[]>) => {
                state.conversationList = action.payload
                state.isBusy = false
            })
            .addCase(getUserConversations.rejected, (state, action) => {
                errorNotification((action.payload as any).error ?? 'Server Error')
                state.isBusy = false
            })
            .addCase(addMessage.pending, (state) => {
                state.isBusy = true
            })
            .addCase(addMessage.fulfilled, (state, action: PayloadAction<Message>) => {
              
                // const conversationMessages = state.messageList.find(x => x.conversationId === action.payload.conversationId)?.messages
                //
                // if (conversationMessages === undefined)
                // {
                //     state.messageList.push({
                //         conversationId = action.payload.conversationId,
                //         messages = action.payload
                //     } as ConversationMessages)
                // }
                // else
                // {
                //    
                // }
                
                state.isBusy = false
            })
            .addCase(addMessage.rejected, (state, action) => {
                errorNotification((action.payload as any).error ?? 'Server Error')
                state.isBusy = false
            })
            .addCase(getMessages.pending, (state) => {
                state.isBusy = true
            })
            .addCase(getMessages.fulfilled, (state, action: PayloadAction<Message[]>) => {
                const conversationId = action.payload[0]?.conversationId
                if (conversationId === undefined)
                {
                    return;
                }
                
                state.selectedConversationId = conversationId
                
                //console.log(action.payload)
                
                const conversationMessages = state.messageList.find(x => x.conversationId === conversationId)?.messages

                if (conversationMessages === undefined)
                {
                    state.messageList.push({
                        conversationId: conversationId,
                        messages: action.payload
                    } as ConversationMessages)
                }
                else
                {
                    state.messageList.splice(conversationId, 1)
                    
                    state.messageList.push({
                        conversationId: conversationId,
                        messages: action.payload
                    } as ConversationMessages)
                }
                                
                state.isBusy = false
            })
            .addCase(getMessages.rejected, (state, action) => {
                errorNotification((action.payload as any).error ?? 'Server Error')
                state.isBusy = false
            })
            .addCase(addParticipants.pending, (state) => {
                state.isBusy = true
            })
            .addCase(addParticipants.fulfilled, (state, action: PayloadAction<Conversation>) => {
                successNotification(`The ${action.payload.title} conversation has been successfully updated`)
                               
                const index = state.conversationList.findIndex(x => x.id === action.payload.id)
                if (index !== -1) {
                    state.conversationList.splice(index, 1)
                }

                state.conversationList.push(action.payload)                
                state.isBusy = false
            })
            .addCase(addParticipants.rejected, (state, action) => {
                errorNotification((action.payload as any).error ?? 'Server Error')
                state.isBusy = false
            })
            .addCase(removeParticipants.pending, (state) => {
                state.isBusy = true
            })
            .addCase(removeParticipants.fulfilled, (state, action: PayloadAction<Conversation>) => {
                successNotification(`The ${action.payload.title} conversation has been successfully updated`)

                const index = state.conversationList.findIndex(x => x.id === action.payload.id)
                if (index !== -1) {
                    state.conversationList.splice(index, 1)
                }

                state.conversationList.push(action.payload)
                state.isBusy = false
            })
            .addCase(removeParticipants.rejected, (state, action) => {
                errorNotification((action.payload as any).error ?? 'Server Error')
                state.isBusy = false
            })
            .addCase(updateLatSeenAt.fulfilled, (state, action: PayloadAction<ConversationLastSeen>) => {
                const conversation = state.conversationList.find(x => x.id == action.payload.conversationId)
                if (conversation !== undefined) {
                    conversation.lastSeenAt = action.payload.lastSeenAt
                }
            })
    }
})
export default conversationSlice