import React from "react";
import Conversation from "../../../../interfaces/Conversations/Conversation";
import {ApplicationState} from "../../../../store";
import {useDispatch, useSelector} from "react-redux";
import {getFiles} from "../../../../store/files/api";
import {getFileUrl} from "../../../../utils/helpers/fileHelpers";
import styles from '../Conversations/ConversationFiles.module.css'

const ConversationFiles: React.FC<{ conversation: Conversation }> = props => {
    
    const files = useSelector((state: ApplicationState) => state.file.files.filter(f => f.externalId === props.conversation.id.toString()))
    
    const dispatch = useDispatch()
    
    React.useEffect(() => {
        dispatch(getFiles(props.conversation.id.toString()))
    }, [dispatch])
    
    if (!files) {
        return <p>No files</p>
    }

    return <div className={styles.filesList}>
        {files.map((f, i) => <p key={i}><a href={getFileUrl(f)}>{f.fileName}</a></p>)}
    </div>
}
export default ConversationFiles