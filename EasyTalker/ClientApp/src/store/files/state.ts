import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { getFiles, uploadFile } from './api'
import { FileInfo } from '../../interfaces/Files/FileInfo'
import { errorNotification, successNotification } from '../../utils/notifications/notificationFactory'
import User from '../../interfaces/Users/User'
import { getLoggedUserId } from '../../utils/authUtils'

export interface FileState {
    isBusy: boolean,
    files: FileInfo[]
}

const getDefaultState = () => {
    return {
        isBusy: false,
        files: []
    } as FileState
}

const fileSlice = createSlice({
    name: 'file',
    initialState: getDefaultState(),
    reducers: {
        fileUploaded(state, action: PayloadAction<FileInfo>) {
            const index = state.files?.findIndex(x => x.id === action.payload.id)
            if (index === -1 && action.payload.ownerId !== getLoggedUserId()) {
                state.files?.push(action.payload)
            }
        }        
    },
    extraReducers: builder => {
        builder
            .addCase(uploadFile.pending, (state) => {
                state.isBusy = true
            })
            .addCase(uploadFile.rejected, (state, action) => {
                errorNotification((action.payload as any).error ?? 'Server Error')
                state.isBusy = false
            })
            .addCase(uploadFile.fulfilled, (state, action: PayloadAction<FileInfo>) => {
                successNotification(`File ${action.payload.fileName} has been successfully uploaded`)

                state.files.push(action.payload)                
                state.isBusy = false
            })
            .addCase(getFiles.pending, (state) => {
                
            })
            .addCase(getFiles.rejected, (state, action) => {
                
            })
            .addCase(getFiles.fulfilled, (state, action: PayloadAction<FileInfo[]>) => {
                
                const existingDbIds = state.files.map(f => f.id)
                const newFiles = action.payload.filter(f => !existingDbIds.includes(f.id))                 
                
                state.files.push(...newFiles)
            })
    }
})
export default fileSlice