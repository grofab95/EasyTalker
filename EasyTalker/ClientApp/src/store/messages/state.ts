import { createSlice } from '@reduxjs/toolkit'
import Message from '../../interfaces/Messages/Message'

export interface MessageState {
    isBusy: boolean
    messages: Message[]
}

const getDefaultState = () => {
    return {
        isBusy: false
    } as MessageState
}

const messageSlice = createSlice({
    name: 'message',
    initialState: getDefaultState(),
    reducers: {},
    extraReducers: builder => {
        
    }
})
export default messageSlice