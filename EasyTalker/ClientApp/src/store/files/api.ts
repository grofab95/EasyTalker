import { createAsyncThunk } from '@reduxjs/toolkit'
import { AxiosRequestConfig } from 'axios'
import apiClient from '../../utils/apiClient'
import ApiResponse from '../../interfaces/ApiResponse'
import { FileInfo } from '../../interfaces/Files/FileInfo'

const apiUrls = {
    uploadFile: () => 'files',
    getFiles: (externalId: string) => `files/external-id/${externalId}`
}

export const uploadFile = createAsyncThunk(
    'files/upload', 
    async ( payload: { file: File, uploadId: string, externalId: string, onUploadProgress: (e: ProgressEventInit) => void, onUploadResponse: () => void }, thunkAPI) => {
    try {
        console.log('uploadFile')
        
        const formData = new FormData()
        formData.append('file', payload.file)
        formData.append('uploadId', payload.uploadId)
        formData.append('externalId', payload.externalId)

        const config: AxiosRequestConfig = {
            headers: {
                'Content-Type': 'multipart/form-data'
            },
            onUploadProgress: payload.onUploadProgress
        }
        
        const response = await apiClient.post<ApiResponse<FileInfo>>(apiUrls.uploadFile(), formData, config)
        
        payload.onUploadResponse()
        
        if (!response.data.isSuccess) {
            return thunkAPI.rejectWithValue({ error: response.data.error })
        }
        
        return await response.data.data
    } catch (error: any) {
        return thunkAPI.rejectWithValue({ error: error.message })
    }
})

export const getFiles = createAsyncThunk('files/getFiles', async (externalId: string, thunkAPI) => {
    try {
        console.log('getFiles')
        const response = await apiClient.get<ApiResponse<FileInfo[]>>(apiUrls.getFiles(externalId))
        if (!response.data.isSuccess) {
            return thunkAPI.rejectWithValue({error: response.data.error})
        }

        return response.data.data
    } catch (error: any) {
        return thunkAPI.rejectWithValue({error: error.message})
    }
})