import { createAsyncThunk } from '@reduxjs/toolkit'
import apiClient from '../../utils/apiClient'
import ApiResponse from '../../interfaces/ApiResponse'
import RegisterUser from '../../interfaces/Users/RegisterUser'
import User from '../../interfaces/Users/User'

const apiUrls = {
    register: () => 'users/register',
    getUsers: () => 'users',
}

export const registerUser = createAsyncThunk('user/register', async (payload: { registeredUser: RegisterUser, onSuccessfulResponse: () => void }, thunkApi) => {
    try {
        console.log('registerUser')
        const response = await apiClient.post<ApiResponse<User>>(apiUrls.register(), payload.registeredUser)
        if (!response.data.isSuccess) {
            return thunkApi.rejectWithValue({ error: response.data.error })
        }

        payload.onSuccessfulResponse()        
        return await response.data.data
    } catch (error: any) {
        return thunkApi.rejectWithValue({ error: error.message })
    }
})

export const getUsers = createAsyncThunk('users', async (payload: {}, thunkAPI) => {
    try {
        console.log('getUsers')
        const response = await apiClient.get<ApiResponse<User[]>>(apiUrls.getUsers())
        if (!response.data.isSuccess) {
            return thunkAPI.rejectWithValue({error: response.data.error})
        }

        return response.data.data
    } catch (error: any) {
        return thunkAPI.rejectWithValue({error: error.message})
    }
})