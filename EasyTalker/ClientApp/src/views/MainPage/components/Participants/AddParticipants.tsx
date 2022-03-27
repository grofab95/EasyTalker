import React, {useState} from 'react'
import {useDispatch, useSelector} from 'react-redux'
import Button from 'react-bootstrap/Button'
import Conversation from "../../../../interfaces/Conversations/Conversation";
import {ApplicationState} from "../../../../store";
import User from "../../../../interfaces/Users/User";
import UsersSelection from "../../../Users/components/UsersSelection";
import {addParticipants} from "../../../../store/conversations/api";

const AddParticipants: React.FC<{ conversation: Conversation, onUpdated: () => void }> = props => {
    const newUsers = useSelector((state: ApplicationState) => state.user.userList)
        .filter(u => !props.conversation.participants.some(p => p.id === u.id))

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
        <UsersSelection users={newUsers} onSelectionChanged={(u) => setSelectedUsers(u)}/>
        <Button disabled={selectedUsers.length === 0}
                onClick={() => add()}>
            Add
        </Button>
    </>
}
export default AddParticipants