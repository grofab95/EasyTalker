import React, {useState} from 'react'
import {Card} from 'react-bootstrap'
import {useDispatch, useSelector} from 'react-redux'
import {ApplicationState} from '../../../store'
import {getMessages} from '../../../store/conversations/api'
import SingleMessage from './SingleMessage'
import MessageCreate from './MessageCreate'
import {getLoggedUserId} from '../../../utils/authUtils'
import styles from '../../MainPage/components/ConversationView.module.css'
import {FileType} from '../../../interfaces/Files/FileType'
import Message from '../../../interfaces/Messages/Message'
import {FileInfo} from '../../../interfaces/Files/FileInfo'
import {apiUrl} from '../../../store/config'
import {ConversationStatus} from "../../../interfaces/Conversations/ConversationStatus";

const ConversationView: React.FC<{ conversationId: number }> = props => {

    if (props.conversationId === undefined) {
        return <></>
    }

    const files = useSelector((state: ApplicationState) => state.file.files)
    const messages = useSelector((state: ApplicationState) => state.conversation.messageList.find(x => x.conversationId === props.conversationId)?.messages)
    const [allMessages, setAllMessages] = useState<Message[] | undefined>(messages)
    const conversation = useSelector((state: ApplicationState) => state.conversation.conversationList).find(x => x.id === props.conversationId)
    const users = useSelector((state: ApplicationState) => state.user.userList)

    const dispatch = useDispatch()

    const toMessage = (file: FileInfo): Message => {
        return {
            id: 999,
            sender: users.find(u => u.id === file.ownerId),
            conversationId: props.conversationId,
            text: getFileUrl(file),
            createdAt: file.createdAt,
            isImage: true
        }
    }

    const getFileUrl = (file: FileInfo) => {
        return `${apiUrl}/static/${file.externalId}/${file.fileName}`
    }

    React.useEffect(() => {
        dispatch(getMessages(props.conversationId))
    }, [dispatch, props.conversationId])

    React.useEffect(() => {
        const imageMessages = files
            .filter(f => f.externalId === props.conversationId.toString() && f.fileType === FileType.Image)
            .map(x => toMessage(x))
        
        if (messages && imageMessages) {
            setAllMessages([...messages, ...imageMessages])
        } else {
            setAllMessages(messages)
        }
    }, [messages, files])
    
    if (!conversation) {
        return <></>
    }

    const hasAccess = conversation.status === ConversationStatus.Open && conversation.participants.find(x => x.id === getLoggedUserId())?.hasAccess
    
    return <>
        <Card style={{borderRadius: '1rem', border: 0}}>
            <div className={styles.conversationView}>
                {allMessages && allMessages.sort((a, b) => (a.createdAt > b.createdAt ? 1 : -1)).map((m, i) => <SingleMessage key={i} {...m} />)}
            </div>
            
            {hasAccess && <MessageCreate conversationId={props.conversationId}/>}
        </Card>        
    </>
}
export default ConversationView