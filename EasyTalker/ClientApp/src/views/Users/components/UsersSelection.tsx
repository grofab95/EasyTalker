import React, { useEffect, useState } from 'react'
import User from '../../../interfaces/Users/User'
import { Dropdown } from 'react-bootstrap'

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

    const getSelectedList = () => {
        if (selectedUsers.length === 0) {
            return <></>
        }

        return  <Dropdown.Menu show style={{height: '150px', width: '465px', overflow: 'auto', marginBottom: '5px'}}>
                    {selectedUsers.map((u, i) =><> 
                        <Dropdown.Item key={i} onClick={(() => handleUserSelection(u, false))}>
                            {u.userName}
                        </Dropdown.Item>
                    </>)}          
                </Dropdown.Menu> 
    }

    const getNotSelectedList = () => {
        return  <Dropdown.Menu show style={{height: '315px', width: '465px', overflow: 'auto'}}>
                    {users.filter(u => !selectedUsers.includes(u)).map((u, i) => <>
                        <Dropdown.Item key={i} onClick={(() => handleUserSelection(u, true))}>
                            {u.userName}
                        </Dropdown.Item>
                    </>)}               
                </Dropdown.Menu>                
    }

    return <>
        <input placeholder='Search'
               className='form-control mb-4'
               style={{width: '100%'}}
               onChange={e => onSearchedTextChanged(e.target.value)} />
               
        {selectedUsers.length > 0
            ?   <div style={{height: '150px', width: '465px', marginBottom: '5px'}}>
                    {getSelectedList()} 
                </div>
            : <></>} 
        <div style={{height: '315px', width: '465px', marginBottom: '10px'}}>
            {getNotSelectedList()}
        </div>
    </>
}
export default UsersSelection