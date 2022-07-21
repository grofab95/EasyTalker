import React, {useState} from 'react'
import styles from '../Conversations/ConversationsList.module.css'
import {useDispatch, useSelector} from 'react-redux'
import {ApplicationState} from "../../../../store";
import Conversation from "../../../../interfaces/Conversations/Conversation";
import {FileType} from "../../../../interfaces/Files/FileType";
import {getLoggedUserId} from "../../../../utils/authUtils";
import {ConversationAccessStatus} from "../../../../interfaces/Conversations/ConversationAccessStatus";

interface Props {
    conversationIdSelected: (id: number) => void
}

const ConversationsList: React.FC<Props> = (props) => {

    const conversations = useSelector((state: ApplicationState) => state.conversation.conversationList)
    const files = useSelector((state: ApplicationState) => state.file.files)
    const dispatch = useDispatch()
    
    React.useEffect(() => {
        const firstConversationId = [...conversations]?.sort((a, b) => (a.lastMessage?.createdAt > b.lastMessage?.createdAt ? -1 : 1))[0]?.id
        if (selectedId === 0 && firstConversationId > 0) {
            onConversationSelected(firstConversationId)
        }
    })
    
    React.useEffect(() => {
        if (!isConversationVisible(selectedId)) {
            const firstVisibleConversation = conversations.filter(c => isConversationVisible(c.id))[0]
            if (firstVisibleConversation) {
                onConversationSelected(firstVisibleConversation.id)
            }
        }
    }, [conversations])
    
    const [selectedId, setSelectedId] = useState<number>(0)

    const getLastMessage = (conversation: Conversation) => {
        return [...files]
            .sort((a, b) => (a.createdAt > b.createdAt ? -1 : 1))
            .find(f => f.externalId === conversation.id.toString() && f.fileType === FileType.Image)    
    }
        
    const isSeen = (conversation: Conversation) => {      
        if (conversation.lastMessage === null) {
            return false            
        }

        const lastImage = getLastMessage(conversation)
        
        if (lastImage && lastImage.createdAt >= conversation.lastSeenAt) {
            return false
        }
        
       return conversation.lastMessage?.sender?.id === getLoggedUserId() || conversation.lastMessage?.createdAt < conversation.lastSeenAt
    }
    
    const isConversationVisible = (conversationId: number) => {
        return conversations
            .find(c => c.id === conversationId)?.participants?.find(p => p.id === getLoggedUserId())?.accessStatus !== ConversationAccessStatus.Hidden
    }
    
    const onConversationSelected = (id: number) => {
        document.title = `EasyTalker - ${conversations.find(x => x.id === id)?.title}`
        setSelectedId(id)  
        props.conversationIdSelected(id)
    }
        
    return <ul className={styles.conversationsList}> 
        {conversations && [...conversations]
            .filter(c => isConversationVisible(c.id))
            .sort((a, b) => (a.lastMessage?.createdAt > b.lastMessage?.createdAt ? -1 : 1))
            .map((c, i) =>
                <li key={i} className={`${selectedId === c.id ? styles.active : ''} ${isSeen(c) ? '' : styles.newMessage}`} onClick={e => onConversationSelected(c.id)}>
                    <b>{c.title}</b>
                    <p className={`mb-0 ${styles.textTruncate}`}>{c.lastMessage?.text}</p>
                </li>)}
    </ul>
}
export default ConversationsList