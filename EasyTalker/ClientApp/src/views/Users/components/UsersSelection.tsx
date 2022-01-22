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

    return <>
        <input placeholder='Search'/>
        <div>
            {props.users.map((u, i) => <p key={i}>{u.userName} <input type='checkbox'
                                                                         onChange={((e) => handleUserSelection(u, e.target.checked))}/></p>)}
        </div>
    </>
}
export default UsersSelection