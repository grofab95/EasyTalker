import { createAsyncThunk } from '@reduxjs/toolkit'
import apiClient from '../../utils/apiClient'
import ApiResponse from '../../interfaces/ApiResponse'
import Conversation from '../../interfaces/Conversations/Conversation'
import Message from '../../interfaces/Messages/Message'
import NewMessage from '../../interfaces/Messages/NewMessage'
import NewConversation from '../../interfaces/Conversations/NewConversation'
import ApiResponseWithoutData from '../../interfaces/ApiResponseWithoutData'
import ConversationLastSeen from '../../interfaces/Conversations/ConversationLastSeen'

const apiUrls = {
    createConversation: () => 'conversations',
    getUserConversations: (userId: string) => `conversations/users/${userId}`,
    addMessage: () => 'conversations/messages',
    getMessages: (conversationId: number) => `conversations/${conversationId}/messages`,
    addParticipants: (conversationId: number) => `conversations/${conversationId}/participants/add`,
    removeParticipants: (conversationId: number) => `conversations/${conversationId}/participants/remove`,
    updateLastSeenAt: (conversationId: number) => `conversations/${conversationId}`,
}

export const createConversation = createAsyncThunk('conversations', async (payload: { newConversation: NewConversation, onSuccess: () => void }, thunkAPI) => {
    try {
        console.log('createConversation')
        const response = await apiClient.post<ApiResponse<Conversation>>(apiUrls.createConversation(), payload.newConversation)
        if (!response.data.isSuccess) {
            return thunkAPI.rejectWithValue({error: response.data.error})
        }

        payload.onSuccess()
        
        return response.data.data
    } catch (error: any) {
        return thunkAPI.rejectWithValue({error: error.message})
    }
})

export const getUserConversations = createAsyncThunk('conversations/getUserConversations', async (userId: string, thunkAPI) => {
    try {
        console.log('getUserConversations')
        const response = await apiClient.get<ApiResponse<Conversation[]>>(apiUrls.getUserConversations(userId))
        if (!response.data.isSuccess) {
            return thunkAPI.rejectWithValue({error: response.data.error})
        }

        return response.data.data
    } catch (error: any) {
        return thunkAPI.rejectWithValue({error: error.message})
    }
})

export const addMessage = createAsyncThunk('message/addMessage', async (payload: { newMessage: NewMessage }, thunkAPI) => {
    try {
        console.log('addMessage')
        const response = await apiClient.post<ApiResponse<Message>>(apiUrls.addMessage(), payload.newMessage)
        if (!response.data.isSuccess) {
            return thunkAPI.rejectWithValue({error: response.data.error})
        }

        return response.data.data
    } catch (error: any) {
        return thunkAPI.rejectWithValue({error: error.message})
    }
})

export const getMessages = createAsyncThunk('conversations/getMessages', async (conversationId: number, thunkAPI) => {
    try {
        console.log('getMessages')
        const response = await apiClient.get<ApiResponse<Message[]>>(apiUrls.getMessages(conversationId))
        if (!response.data.isSuccess) {
            return thunkAPI.rejectWithValue({error: response.data.error})
        }

        return response.data.data
    } catch (error: any) {
        return thunkAPI.rejectWithValue({error: error.message})
    }
})

export const addParticipants = createAsyncThunk('conversations/addParticipants', 
    async (payload: { conversationId: number, usersIds: string[], onUpdated: () => void }, 
           thunkAPI) => {
    try {
        console.log('addParticipants')
        const response = await apiClient.put<ApiResponse<Conversation>>(apiUrls.addParticipants(payload.conversationId), payload.usersIds)
        if (!response.data.isSuccess) {
            return thunkAPI.rejectWithValue({error: response.data.error})
        }

        payload.onUpdated()
        
        return response.data.data
    } catch (error: any) {
        return thunkAPI.rejectWithValue({error: error.message})
    }
})

export const removeParticipants = createAsyncThunk('conversations/removeParticipants', 
    async (payload: { conversationId: number, participantsIds: string[], onUpdated: () => void }, 
           thunkAPI) => {
    try {
        console.log('removeParticipants')
        const response = await apiClient.put<ApiResponse<Conversation>>(apiUrls.removeParticipants(payload.conversationId), payload.participantsIds)
        if (!response.data.isSuccess) {
            return thunkAPI.rejectWithValue({error: response.data.error})
        }

        payload.onUpdated()
        
        return response.data.data
    } catch (error: any) {
        return thunkAPI.rejectWithValue({error: error.message})
    }
})

export const updateLatSeenAt = createAsyncThunk('conversations/updateLatSeenAt', async (payload: { conversationId: number }, thunkAPI) => {
    try {
        console.log('updateLatSeenAt')
        
        const response = await apiClient.patch<ApiResponse<ConversationLastSeen>>(apiUrls.updateLastSeenAt(payload.conversationId))
        if (!response.data.isSuccess) {
            return thunkAPI.rejectWithValue({error: response.data.error})
        }

        return response.data.data
    } catch (error: any) {
        return thunkAPI.rejectWithValue({error: error.message})
    }
})