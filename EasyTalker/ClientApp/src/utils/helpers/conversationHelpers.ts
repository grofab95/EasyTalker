import Conversation from "../../interfaces/Conversations/Conversation";
import {getLoggedUserId} from "../authUtils";

export const getAccessStatus = (conversation: Conversation) => {
    return conversation.participants.find(x => x.id === getLoggedUserId())?.accessStatus
}