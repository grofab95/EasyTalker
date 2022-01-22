import userSessionSlice, { UserSessionState } from './userSession/state'
import userSlice, { UserState } from './users/state'
import conversationSlice, { ConversationState } from './conversations/state'
import fileSlice, { FileState } from './files/state'

export interface ApplicationState {
    userSession: UserSessionState,
    user: UserState,
    conversation: ConversationState,
    file: FileState
}

export const reducers = {
    userSession: userSessionSlice.reducer,
    user: userSlice.reducer,
    conversation: conversationSlice.reducer,
    file: fileSlice.reducer
}

