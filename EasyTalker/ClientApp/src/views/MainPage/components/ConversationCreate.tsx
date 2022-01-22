import React, { useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { ApplicationState } from '../../../store'
import { getUsers } from '../../../store/users/api'
import User from '../../../interfaces/Users/User'
import Button from 'react-bootstrap/Button'
import { createConversation } from '../../../store/conversations/api'
import NewConversation from '../../../interfaces/Conversations/NewConversation'
import { getLoggedUserId } from '../../../utils/authUtils'
import UsersSelection from '../../Users/components/UsersSelection'

interface Props {
    onCreate: () => void
}

const ConversationCreate: React.FC<Props> = (props) => {

    const users = useSelector((state: ApplicationState) => state.user.userList.filter(u => u.id !== getLoggedUserId()))

    const [title, setTitle] = useState('')
    const [selectedUsers, setSelectedUsers] = useState<User[]>([])
    const dispatch = useDispatch()

    // const handleUserSelect = (user: User) => {
    //     const selectedUserId = selectedUsers.findIndex(x => x.id === user.id)
    //     if (selectedUserId === -1) {
    //         setSelectedUsers((selectedUsers) => [
    //             ...selectedUsers,
    //             user,
    //         ])
    //     } else {
    //         const arr = selectedUsers
    //         arr.splice(selectedUserId, 1)
    //
    //         setSelectedUsers(arr)
    //     }
    // }

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

    console.log(title)

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
            {/*{users && users.map((u, i) => <p key={i}>{u.userName} <input type='checkbox'*/}
            {/*                                                             onChange={((e) => handleUserSelect(u))}/></p>)}*/}
        </div>
        <hr/>
        <div style={{textAlign: 'right'}}>
            <Button onClick={() => create()}>Create</Button>
        </div>
    </>
}
export default ConversationCreate