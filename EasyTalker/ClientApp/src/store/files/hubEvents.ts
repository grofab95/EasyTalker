import { withCallbacks } from 'redux-signalr'
import { FileInfo } from '../../interfaces/Files/FileInfo'
import fileSlice from './state'

export const fileCallbacks = withCallbacks()
    .add('FileUploaded', (file: FileInfo) => dispatch => {
        console.log('FileUploaded', JSON.stringify(file))
        dispatch(fileSlice.actions.fileUploaded(file))
    })
    .callbackMap