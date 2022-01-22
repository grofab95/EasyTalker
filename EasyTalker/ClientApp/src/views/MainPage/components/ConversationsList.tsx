import React, { useState } from 'react'
import Conversation from '../../../interfaces/Conversations/Conversation'
import styles from '../../MainPage/components/ConversationsList.module.css'
import { useSelector } from 'react-redux'
import { ApplicationState } from '../../../store'
import { getLoggedUserId } from '../../../utils/authUtils'

interface Props {
    conversationIdSelected: (id: number) => void
}

const ConversationsList: React.FC<Props> = (props) => {

    const conversations = useSelector((state: ApplicationState) => state.conversation.conversationList)
    
    React.useEffect(() => {
        const firstConversationId = [...conversations]?.sort((a, b) => (a.lastMessage?.createdAt > b.lastMessage?.createdAt ? -1 : 1))[0]?.id
        if (selectedId === 0 && firstConversationId > 0) {
            onConversationSelected(firstConversationId)
        }
    })

    const [selectedId, setSelectedId] = useState<number>(0)
        
    const isSeen = (conversation: Conversation) => {           
       return conversation.lastMessage.sender.id === getLoggedUserId() || conversation.lastMessage?.createdAt < conversation.lastSeenAt
    }
    
    const onConversationSelected = (id: number) => {
        document.title = `EasyTalker - ${conversations.find(x => x.id === id)?.title}`
        setSelectedId(id)  
        props.conversationIdSelected(id)
    }
        
    return <ul className={styles.conversationsList}> 
        {conversations && [...conversations]
            .sort((a, b) => (a.lastMessage?.createdAt > b.lastMessage?.createdAt ? -1 : 1))
            .map((c, i) =>
                <li key={i} className={`${selectedId === c.id ? styles.active : ''} ${isSeen(c) ? '' : styles.newMessage}`} onClick={e => onConversationSelected(c.id)}>
                    <b>{c.title}</b>
                    <p className={`mb-0 ${styles.textTruncate}`}>{c.lastMessage?.text}</p>
                </li>)}
    </ul>
}
export default ConversationsList