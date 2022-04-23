import React from 'react'
import { useSelector } from 'react-redux'
import { ApplicationState } from '../../store'
import { getLoggedUserId } from '../../utils/authUtils'
import User from '../../interfaces/Users/User'

const UserConnectionStatusIndicator: React.FC<{ userId: string }> = (props) => {
    const user = useSelector((state: ApplicationState) => state.user.userList.find(x => x.id == props.userId))

    if (user === undefined) {
        return <p>None</p>
    }

    const isOnline = (user: User) => {
        if (user.id === getLoggedUserId())
            return true

        return user.isOnline
    }

    if (isOnline(user)) {
        return <span style={{color: 'green', fontSize: 'large'}}>{user.userName}</span>
    } else {
        return <span style={{color: 'red', fontSize: 'large'}}>{user.userName}</span>
    }
}
export default UserConnectionStatusIndicator