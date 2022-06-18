import React, { useCallback } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { getUser } from '../../store/userSession/api'
import { getLoggedUserId } from '../../utils/authUtils'
import { connection } from '../../store/configureSignalrConnection'
import { getUserConversations } from '../../store/conversations/api'
import { getUsers } from '../../store/users/api'
import { ApplicationState } from '../../store'
import { getFiles } from '../../store/files/api'

const FetchDashboardData: React.FC = () => {
    const dispatch = useDispatch()
    
    const conversations = useSelector((state: ApplicationState) => state.conversation.conversationList)
    
    const fetchData = useCallback(() => {        
        const loggedUserId = getLoggedUserId();
        
        dispatch(getUser(loggedUserId))
        dispatch(getUserConversations(loggedUserId))
        dispatch(getUsers({}))        
    }, [dispatch])
    
    React.useEffect(() => {
        if (!conversations)
            return
        
        conversations.forEach(c => dispatch(getFiles(c.id.toString())))
    }, [conversations])
    
    React.useEffect(fetchData, [fetchData])
    
    React.useEffect(() => {
        connection.onreconnected(fetchData)
    }, [fetchData])
    
    return <></>
}
export default FetchDashboardData