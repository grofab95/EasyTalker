import React from 'react'
import Conversation from '../../../interfaces/Conversations/Conversation'
import { useDispatch, useSelector } from 'react-redux'
import { ApplicationState } from '../../../store'
import { getFiles } from '../../../store/files/api'
import {getFileUrl} from "../../../utils/helpers/fileHelpers";

const ConversationFiles: React.FC<{ conversation: Conversation }> = props => {
    
    const files = useSelector((state: ApplicationState) => state.file.files.filter(f => f.externalId === props.conversation.id.toString()))
    
    const dispatch = useDispatch()
    
    React.useEffect(() => {
        dispatch(getFiles(props.conversation.id.toString()))
    }, [dispatch])
    
    return <>
        {files && files.map((f, i) => <p key={i}><a href={getFileUrl(f)}>{f.fileName}</a></p>)}
    </>
}
export default ConversationFiles