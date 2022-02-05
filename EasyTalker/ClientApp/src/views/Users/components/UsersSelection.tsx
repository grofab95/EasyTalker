import React, { useState } from 'react'
import User from '../../../interfaces/Users/User'

interface Props {
    users: User[],
    onSelectionChanged: (users: User[]) => void
}

const UsersSelection: React.FC<Props> = props => {

    const [selectedUsers, setSelectedUsers] = useState<User[]>([])
    
    if (!props.users) {
        return <>No users</>
    }

    const [users, setUsers] = useState<User[]>(props.users)
    const handleUserSelection = (user: User, isSelected: boolean) => {
        const currentSelectedUsers = selectedUsers
        if (isSelected) {
            currentSelectedUsers.push(user)
        } else {
            const id = currentSelectedUsers.indexOf(user)
            currentSelectedUsers.splice(id, 1)
        }
        
        setSelectedUsers(currentSelectedUsers)        
        props.onSelectionChanged(selectedUsers)
    }
    
    const onSearchedTextChanged = (text: string) => {
        const filteredUsers = text
            ? props.users.filter(u => u.userName.includes(text))
            : props.users
        
        setUsers(filteredUsers)
    }

    return <>
        <input placeholder='Search' onChange={e => onSearchedTextChanged(e.target.value)} />
        <ul>
            {users.map((u, i) => <li><p key={i}>{u.userName} <input type='checkbox'
                                                                         onChange={((e) => handleUserSelection(u, e.target.checked))}/></p> </li>)}
        </ul>
    </>
}
export default UsersSelection