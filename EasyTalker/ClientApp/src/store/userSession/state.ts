import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { deleteTokens, saveTokens } from '../../utils/authUtils'
import { getUser, login, logout } from './api'
import { AuthenticationResult } from '../../interfaces/AuthTokens'
import { errorNotification, successNotification } from '../../utils/notificationFactory'
import User from '../../interfaces/Users/User'

export interface UserSessionState {
    currentUser: User,
    currentToken: string,
    showUserChangedPopup: boolean,
    accountDeactivated: boolean,
    passwordChangedByAdmin: boolean,
    isBusy: boolean
}

const getDefaultState = () => {
    return {
        currentUser: {},
        currentToken: '',
        showUserChangedPopup: false,
        accountDeactivated: false,
        passwordChangedByAdmin: false,
        isBusy: false
    } as UserSessionState
}

const userSessionSlice = createSlice({
    name: 'userSession',
    initialState: getDefaultState(),
    reducers: {
        currentUserChanged(state, action: PayloadAction<User>) {
            //const user = action.payload

            state.showUserChangedPopup = true
            state.currentUser = action.payload
        },
        userLogout() {
            deleteTokens()
            window.location.reload()
        }
    },
    extraReducers: builder => {
        builder
            .addCase(login.fulfilled, (state, action: PayloadAction<AuthenticationResult>) => {
                state.currentUser = action.payload.user
                state.currentToken = action.payload.accessToken
                saveTokens({ accessToken: action.payload.accessToken, refreshToken: action.payload.refreshToken })
                state.isBusy = false
                
                if (action.payload.user.isActive)
                    successNotification(`Logged as ${action.payload.user.userName}`)
            })
            .addCase(login.rejected, (state, action) => {
                state.isBusy = false
                errorNotification((action.payload as any).error ?? 'Server Error')
            })
            .addCase(login.pending, (state) => {
                state.isBusy = true
            })
            .addCase(logout.fulfilled, (state) => {
                state.isBusy = true
                deleteTokens()
                window.location.reload()
            })
            .addCase(logout.pending, (state) => {
                deleteTokens()
                state.isBusy = false
                window.location.reload()
            })
            .addCase(logout.rejected, (state) => {
                deleteTokens()
                state.isBusy = false
                window.location.reload()
            })
            .addCase(getUser.fulfilled, (state, action: PayloadAction<User>) => {
                state.currentUser = action.payload
            })
    }
})
export default userSessionSlice