import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { getFiles, uploadFile } from './api'
import { FileInfo } from '../../interfaces/Files/FileInfo'
import { errorNotification, successNotification } from '../../utils/notifications/notificationFactory'
import User from '../../interfaces/Users/User'
import { getLoggedUserId } from '../../utils/authUtils'

interface ExternalIdFiles {
    externalId: string,
    files: FileInfo[]
}

export interface FileState {
    isBusy: boolean,
    files: FileInfo[]
    //externalIdFiles: ExternalIdFiles[]
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
            const index = state.files?.findIndex(x => x.dbId === action.payload.dbId)
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
                state.isBusy = true
            })
            .addCase(getFiles.rejected, (state, action) => {
                //errorNotification((action.payload as any).error ?? 'Server Error')
                state.isBusy = false
            })
            .addCase(getFiles.fulfilled, (state, action: PayloadAction<FileInfo[]>) => {
                //successNotification(`File ${action.payload.fileName} has been successfully uploaded`)
                
                const existingDbIds = state.files.map(f => f.dbId)
                const newFiles = action.payload.filter(f => !existingDbIds.includes(f.dbId))                 
                
                state.files.push(...newFiles)
                
                state.isBusy = false
            })
    }
})
export default fileSlice