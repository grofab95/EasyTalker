import apiClient from "./apiClient";
import {ConversationStatus} from "../interfaces/Conversations/ConversationStatus";
import ApiResponseWithoutData from "../interfaces/ApiResponseWithoutData";
import {ConversationAccessStatus} from "../interfaces/Conversations/ConversationAccessStatus";

const apiUrls = {
    updateConversationStatusUrl: (conversationId: number) => `conversations/${conversationId}/update-status`,
    updateUserConversationAccessStatusUrl: 
        (conversationId: number, participantId: string) => `conversations/${conversationId}/participants/${participantId}/update-access-status`,
}

export const updateConversationStatus = async (payload: { conversationId: number, status: ConversationStatus }) => {
    try {
        console.log('updateConversationStatus')
        const response = await apiClient.put<ApiResponseWithoutData>(apiUrls.updateConversationStatusUrl(payload.conversationId), payload.status)

        return response.data
    } catch (error) {
        return {
            isSuccess: false,
            error: error
        } as unknown as ApiResponseWithoutData
    }
}

export const updateUserConversationAccessStatus = async (payload: { conversationId: number, participantId: string, accessStatus: ConversationAccessStatus }) => {
    try {
        console.log('updateUserConversationAccessStatus')
        const response = await apiClient.put<ApiResponseWithoutData>(
            apiUrls.updateUserConversationAccessStatusUrl(payload.conversationId, payload.participantId),
            payload.accessStatus)

        return response.data
    } catch (error) {
        return {
            isSuccess: false,
            error: error
        } as unknown as ApiResponseWithoutData
    }
}