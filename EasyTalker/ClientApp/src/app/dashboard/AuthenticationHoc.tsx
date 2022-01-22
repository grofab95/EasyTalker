import React from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { ApplicationState } from '../../store'
import { deleteTokens, getAccessToken, hasPermissions } from '../../utils/authUtils'
import * as jwt from 'react-jwt'
import { Redirect } from 'react-router-dom'
import { connection } from '../../store/configureSignalrConnection'
import { HubConnectionState } from '@microsoft/signalr'
import { permissionType } from '../../constants/PermissionType'

const AuthenticationHoc: React.FC<React.ReactNode> = props => {
    const user = useSelector((state: ApplicationState) => state.userSession.currentUser)
    const token = getAccessToken()
    const dispatch = useDispatch()
    
    if (!token || token === '' || jwt.isExpired(token)) {
        deleteTokens()
        return <Redirect to="/login" />
    }
    
    // if (!getUserData().isActive) {
    //     return <Redirect to="/account-not-active" />
    // }
    if (connection.state === HubConnectionState.Disconnected) {
        connection.start()
    }
    
    // if (user.forcePasswordChange) {
    //     console.log('force password change')
    //     return <PasswordChange />
    // }
    
    // if (!hasPermissions(permissionType.ALL)) {
    //     return <>
    //         <h1>You are not authorized to view this page.</h1>
    //     </>
    // }
    
    return <>{props.children}</>
}
export default AuthenticationHoc