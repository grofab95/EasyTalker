import React, { useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import Button from 'react-bootstrap/Button'
import {ApplicationState} from "../../../../store";
import {getLoggedUserId} from "../../../../utils/authUtils";
import User from "../../../../interfaces/Users/User";
import {createConversation} from "../../../../store/conversations/api";
import NewConversation from "../../../../interfaces/Conversations/NewConversation";
import {getUsers} from "../../../../store/users/api";
import UsersSelection from "../../../Users/components/UsersSelection";

interface Props {
    onCreate: () => void
}

const ConversationCreate: React.FC<Props> = (props) => {

    const users = useSelector((state: ApplicationState) => state.user.userList.filter(u => u.id !== getLoggedUserId()))

    const [title, setTitle] = useState('')
    const [selectedUsers, setSelectedUsers] = useState<User[]>([])
    const dispatch = useDispatch()

    const create = () => {
        dispatch(createConversation({
            newConversation: {
                creatorId: getLoggedUserId(),
                participantsId: selectedUsers.map(u => u.id),
                title: title
            } as NewConversation,
            onSuccess: props.onCreate
        }))
    }

    React.useEffect(() => {
        dispatch(getUsers({}))
    }, [dispatch])

    return <>
        <div>
            <span>Title: </span>
            <input onChange={e => setTitle(e.target.value)}/>
        </div>
        <hr/>
        <p style={{textAlign: 'center'}}><b>Participants</b></p>
        <div>
            <UsersSelection users={users} onSelectionChanged={(u) => setSelectedUsers(u)} />
        </div>
        <hr/>
        <div style={{textAlign: 'right'}}>
            <Button onClick={() => create()}>Create</Button>
        </div>
    </>
}
export default ConversationCreate