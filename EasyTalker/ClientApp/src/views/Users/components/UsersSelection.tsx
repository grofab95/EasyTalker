import styles from './UsersSelection.module.css'
import React, { useEffect, useState } from 'react'
import User from '../../../interfaces/Users/User'
import { Button } from 'react-bootstrap'

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
        
        setSelectedUsers([...selectedUsers])   
        props.onSelectionChanged(selectedUsers)
    }

    useEffect(() => {
        console.log(selectedUsers)
    }, [selectedUsers])

    
    const onSearchedTextChanged = (text: string) => {
        const filteredUsers = text
            ? props.users.filter(u => u.userName.includes(text))
            : props.users
        
        setUsers(filteredUsers)
    }

    return <>
        <div className={styles.selectedUsers}>
            {selectedUsers && selectedUsers.map((u, i) => <Button key={i}
                                                                  style={{margin: '5px'}}
                                                                  variant='danger' size='sm'
                                                                  onClick={(() => handleUserSelection(u, false))}>{u.userName}</Button>)}
        </div>
        <hr />
        <input placeholder='Search'
               style={{width: '100%'}}
               onChange={e => onSearchedTextChanged(e.target.value)} />
        <hr />
        <ul style={{height: '400px', overflow: 'auto'}}>
            {users.filter(u => !selectedUsers.includes(u)).map((u, i) => <li key={i}><Button variant='success'
                                                     style={{marginBottom: '5px'}}
                                                     size='sm'
                                                     onClick={(() => handleUserSelection(u, true))}>{u.userName}</Button> </li>)}
        </ul>
        <hr />
    </>
}
export default UsersSelection