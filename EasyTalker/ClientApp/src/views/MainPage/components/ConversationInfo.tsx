import React from 'react'
import { Card, Row } from 'react-bootstrap'
import UserConnectionStatusIndicator from '../../../app/components/UserConnectionStatusIndicator'
import { useSelector } from 'react-redux'
import { ApplicationState } from '../../../store'
import ConversationFiles from './ConversationFiles'
import FileUploader from '../../../app/components/FileUploader'
import { getLoggedUserId } from '../../../utils/authUtils'
import ConversationSettings from './ConversationSettings'

const ConversationInfo: React.FC<{ conversationId: number }> = props => {

    const conversation = useSelector((state: ApplicationState) => state.conversation.conversationList).find(x => x.id === props.conversationId)
    const users = useSelector((state: ApplicationState) => state.user.userList)

    if (conversation === undefined) {
        return <></>
    }

    const getParticipantsId = () => {
        return conversation?.participants
            .filter(u => users.some(x => x.id === u.id) && u.hasAccess)
            .map(u => u.id)
    }

    return <Card className='border-0 rounded p-2' style={{marginBottom: '10px'}}>
        <h3>{conversation?.title} (ID: {conversation?.id})</h3>
        <hr/>
        {conversation.creatorId === getLoggedUserId() &&
            <ConversationSettings conversation={conversation} users={users}/>
        }
        <hr/>
        <Row>
            <h5>Participants:</h5> {getParticipantsId()?.map((id, i) => <UserConnectionStatusIndicator key={i}
                                                                                                       userId={id}/>)}
        </Row>
        <hr/>
        <FileUploader externalId={props.conversationId.toString()}/>
        <hr/>
        <ConversationFiles conversation={conversation}/>
    </Card>
}
export default ConversationInfo