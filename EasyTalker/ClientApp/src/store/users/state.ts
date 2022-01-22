import { createSlice, current, PayloadAction } from '@reduxjs/toolkit'
import { errorNotification, infoNotification, successNotification } from '../../utils/notificationFactory'
import { getUsers, registerUser } from './api'
import User from '../../interfaces/Users/User'
import { getLoggedUserId } from '../../utils/authUtils'
import UserConnectionStatus from '../../interfaces/Users/UserConnectionStatus'

export interface UserState {
    isBusy: boolean
    userList: User[],
    loggedUser: User
}

const getDefaultState = () => {
    return {
        isBusy: false,
        userList: [],
        loggedUser: {} as User
    } as UserState
}

const userSlice = createSlice({
    name: 'user',
    initialState: getDefaultState(),
    reducers: {
        userRegistered(state, action: PayloadAction<User>) {            
            const user = action.payload
            const index = state.userList.findIndex(x => x.id === user.id) 
            if (index === -1) {
                state.userList.push(user)
            }
        },

        userConnectionStatusChanged(state, action: PayloadAction<UserConnectionStatus>) {             
            const user = state.userList.find(x => x.id === action.payload.userId)                        
            if (user !== undefined) {
                user.isOnline = action.payload.isOnline
                infoNotification(`${user.userName} is ${user.isOnline ? 'online' : 'offline'}`)
            }
        }
    },
    extraReducers: builder => {
        builder
            .addCase(registerUser.fulfilled, (state, action: PayloadAction<User>) => {
                state.isBusy = false
                state.userList.push(action.payload)
                successNotification(`Account for ${action.payload.userName} created successfully`)
            })
            .addCase(registerUser.rejected, (state, action) => {
                state.isBusy = false
                errorNotification((action.payload as any).error ?? 'Server Error')
            })
            .addCase(registerUser.pending, (state) => {
                state.isBusy = true
            })
            .addCase(getUsers.pending, (state) => {
                state.isBusy = true
            })
            .addCase(getUsers.fulfilled, (state, action: PayloadAction<User[]>) => {
                state.userList = action.payload

                const loggedUser = state.userList.find(u => u.id === getLoggedUserId())
                if (loggedUser !== undefined) {
                    state.loggedUser = loggedUser
                }

                state.isBusy = false
            })
            .addCase(getUsers.rejected, (state, action) => {
                errorNotification((action.payload as any).error ?? 'Server Error')
                state.isBusy = false
            })
    }
})
export default userSlice