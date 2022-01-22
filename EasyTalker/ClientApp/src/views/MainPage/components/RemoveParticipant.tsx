import React, { useState } from 'react'
import Conversation from '../../../interfaces/Conversations/Conversation'
import { useDispatch, useSelector } from 'react-redux'
import { ApplicationState } from '../../../store'
import UsersSelection from '../../Users/components/UsersSelection'
import Button from 'react-bootstrap/Button'
import { removeParticipants } from '../../../store/conversations/api'
import { getLoggedUserId } from '../../../utils/authUtils'

const RemoveParticipant: React.FC<{ conversation: Conversation, onUpdated: () => void }> = props => {
    const users = useSelector((state: ApplicationState) => state.user.userList)
        .filter(u => props.conversation.participants
            .filter(u => u.hasAccess && u.id !== getLoggedUserId())
            .map(p => p.id).includes(u.id))

    const [selectedParticipantsIds, setSelectedParticipantsIds] = useState<string[]>([])
    const dispatch = useDispatch()

    const remove = () => {
        dispatch(removeParticipants({
            conversationId: props.conversation.id,
            participantsIds: selectedParticipantsIds,
            onUpdated: () => props.onUpdated()
        }))
    }

    return <>
        <UsersSelection users={users} onSelectionChanged={(u) => setSelectedParticipantsIds(u.map(x => x.id))}/>
        <Button disabled={selectedParticipantsIds.length === 0}
                onClick={() => remove()}>
            Remove
        </Button>
    </>
}
export default RemoveParticipant