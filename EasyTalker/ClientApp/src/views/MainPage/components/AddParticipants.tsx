import React, { useState } from 'react'
import Conversation from '../../../interfaces/Conversations/Conversation'
import { useDispatch, useSelector } from 'react-redux'
import { ApplicationState } from '../../../store'
import UsersSelection from '../../Users/components/UsersSelection'
import { addMessage, addParticipants } from '../../../store/conversations/api'
import User from '../../../interfaces/Users/User'
import Button from 'react-bootstrap/Button'

const AddParticipants: React.FC<{ conversation: Conversation, onUpdated: () => void }> = props => {
    const newUsers = useSelector((state: ApplicationState) => state.user.userList)
        .filter(u => !props.conversation.participants.some(p => p.id === u.id))

    const oldUsers = useSelector((state: ApplicationState) => state.user.userList)
        .filter(u => props.conversation.participants.some(p => p.id === u.id && !p.hasAccess))
    
    const allUsers = [...newUsers, ...oldUsers]
    
    const [selectedUsers, setSelectedUsers] = useState<User[]>([]);
    const dispatch = useDispatch()
    
    const add = () => {
        dispatch(addParticipants({
            conversationId: props.conversation.id,
            usersIds: selectedUsers.map(u => u.id),
            onUpdated: () => props.onUpdated()
        }))
    }
    
    return <>
        <UsersSelection users={allUsers} onSelectionChanged={(u) => setSelectedUsers(u)} />
        <Button disabled={selectedUsers.length === 0}
                onClick={() => add()}>
            Add
        </Button>
    </>
}
export default AddParticipants