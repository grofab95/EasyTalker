import React from 'react'
import { Card } from 'react-bootstrap'
import { useDispatch, useSelector } from 'react-redux'
import { ApplicationState } from '../../../store'
import { getMessages, updateLatSeenAt } from '../../../store/conversations/api'
import SingleMessage from './SingleMessage'
import MessageCreate from './MessageCreate'
import { getLoggedUserId } from '../../../utils/authUtils'
import styles from '../../MainPage/components/ConversationView.module.css'

const ConversationView: React.FC<{ conversationId: number }> = props => {

    if (props.conversationId === undefined) {
        return <></>
    }

    const messages = useSelector((state: ApplicationState) => state.conversation.messageList.find(x => x.conversationId === props.conversationId)?.messages)
    const conversation = useSelector((state: ApplicationState) => state.conversation.conversationList).find(x => x.id === props.conversationId)
    const users = useSelector((state: ApplicationState) => state.user.userList)

    const dispatch = useDispatch()

    React.useEffect(() => {
        dispatch(getMessages(props.conversationId))
    }, [dispatch, props.conversationId])
    
    if (!conversation) {
        return <></>
    }

    const hasAccess = conversation.participants.find(x => x.id === getLoggedUserId())?.hasAccess

    return <>
        <Card style={{borderRadius: '1rem', border: 0}}>
            <div className={styles.conversationView}>
                {messages && messages.map((m, i) => <SingleMessage key={i} {...m} />)}
            </div>
            
            {hasAccess && <MessageCreate conversationId={props.conversationId}/>}
        </Card>        
    </>
}
export default ConversationView