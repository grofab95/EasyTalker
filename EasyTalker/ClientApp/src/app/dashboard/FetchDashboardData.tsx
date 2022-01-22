import React, { useCallback } from 'react'
import { useDispatch } from 'react-redux'
import { getUser } from '../../store/userSession/api'
import { getLoggedUserId } from '../../utils/authUtils'
import { connection } from '../../store/configureSignalrConnection'
import { getUserConversations } from '../../store/conversations/api'
import { getUsers } from '../../store/users/api'

const FetchDashboardData: React.FC = () => {
    const dispatch = useDispatch()
    
    const fetchData = useCallback(() => {
        //dispatch(getSystemInfo())
        // if permission -> dispatch ...
        
        const loggedUserId = getLoggedUserId();
        
        dispatch(getUser(loggedUserId))
        dispatch(getUserConversations(loggedUserId))
        dispatch(getUsers({}))
    }, [dispatch])
    
    React.useEffect(fetchData, [fetchData])
    
    React.useEffect(() => {
        connection.onreconnected(fetchData)
    }, [fetchData])
    
    return <></>
}
export default FetchDashboardData