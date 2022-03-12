import apiClient from "./apiClient";
import {ConversationStatus} from "../interfaces/Conversations/ConversationStatus";
import ApiResponseWithoutData from "../interfaces/ApiResponseWithoutData";

const apiUrls = {
    updateStatusUrl: (conversationId: number) => `conversations/${conversationId}/update-status`
}

export const updateConversationStatus = async (payload: { conversationId: number, status: ConversationStatus }) => {
    try {
        console.log('updateConversationStatus')
        const response = await apiClient.put<ApiResponseWithoutData>(apiUrls.updateStatusUrl(payload.conversationId), payload.status)

        return response.data
    } catch (error) {
        return {
            isSuccess: false,
            error: error
        } as unknown as ApiResponseWithoutData
    }
}