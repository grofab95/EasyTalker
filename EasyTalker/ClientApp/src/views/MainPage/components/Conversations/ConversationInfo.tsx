import React from 'react'
import {Card, Row} from 'react-bootstrap'
import {useSelector} from 'react-redux'
import ConversationFiles from './ConversationFiles'
import ConversationSettings from './ConversationSettings'
import {ApplicationState} from "../../../../store";
import {getLoggedUserId} from "../../../../utils/authUtils";
import UserConnectionStatusIndicator from "../../../../app/components/UserConnectionStatusIndicator";
import {ConversationAccessStatus} from "../../../../interfaces/Conversations/ConversationAccessStatus";
import FileUploader from "../../../../app/components/FileUploader";
import {getAccessStatus} from "../../../../utils/helpers/conversationHelpers";

const ConversationInfo: React.FC<{ conversationId: number }> = props => {

    const conversation = useSelector((state: ApplicationState) => state.conversation.conversationList).find(x => x.id === props.conversationId)
    const users = useSelector((state: ApplicationState) => state.user.userList)

    if (conversation === undefined) {
        return <></>
    }

    const getParticipantsId = () => {
        return conversation?.participants
            .filter(u => users.some(x => x.id === u.id))
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
        {getAccessStatus(conversation) == ConversationAccessStatus.ReadAndWrite ? <FileUploader externalId={props.conversationId.toString()} /> : <></>}
        <hr/>
        <ConversationFiles conversation={conversation}/>
    </Card>
}
export default ConversationInfo