import React from 'react'
import {useSelector} from 'react-redux'
import FileUploader from '../../../../../app/components/FileUploader'
import { ConversationAccessStatus } from '../../../../../interfaces/Conversations/ConversationAccessStatus'
import { ApplicationState } from '../../../../../store'
import { getLoggedUserId } from '../../../../../utils/authUtils'
import { getAccessStatus } from '../../../../../utils/helpers/conversationHelpers'
import ConversationFiles from '../ConversationFiles'
import ConversationSettings from './ConversationSettings'
import InfoCard from './InfoCard'
import ParticipantsList from './ParticipantsList'

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

    return <div className='border-0 rounded' style={{marginBottom: '10px'}}>
        <InfoCard>
            <h3 className='text-center'>{conversation?.title}</h3>
        </InfoCard>
               
        {conversation.creatorId === getLoggedUserId() &&
            <InfoCard>
                <ConversationSettings conversation={conversation} users={users} />
            </InfoCard>
        }

        <InfoCard title='Participants'>
            <ParticipantsList participantsIds={getParticipantsId()} />
        </InfoCard>

        {getAccessStatus(conversation) == ConversationAccessStatus.ReadAndWrite &&
            <InfoCard title='Upload file'>
                <FileUploader externalId={props.conversationId.toString()} />
            </InfoCard>
        }
       
       <InfoCard title='Files'>
            <ConversationFiles conversation={conversation} />
        </InfoCard>
    </div>
}
export default ConversationInfo