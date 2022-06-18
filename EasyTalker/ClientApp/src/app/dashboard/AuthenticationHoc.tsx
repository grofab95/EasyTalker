import React, { useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { ApplicationState } from '../../store'
import { deleteTokens, getAccessToken } from '../../utils/authUtils'
import * as jwt from 'react-jwt'
import { Redirect } from 'react-router-dom'
import { connection } from '../../store/configureSignalrConnection'
import { HubConnectionState } from '@microsoft/signalr'
import { Modal, Spinner } from 'react-bootstrap'

const AuthenticationHoc: React.FC<React.ReactNode> = props => {
    const user = useSelector((state: ApplicationState) => state.userSession.currentUser)
    const token = getAccessToken()
    const dispatch = useDispatch()
    
    const [isHubReconnecting, setIsHubReconnecting] = useState(false)
    
    if (!token || token === '' || jwt.isExpired(token)) {
        deleteTokens()
        return <Redirect to="/login" />
    }
    
    if (connection.state === HubConnectionState.Disconnected) {
        connection.start()
    }

    connection.onreconnecting(() => switchSetIsHubReconnecting(true))    
    connection.onreconnected(() => switchSetIsHubReconnecting(false))
    
    const switchSetIsHubReconnecting = (flag: boolean) => {
        if (isHubReconnecting === flag)
            return
        
        setIsHubReconnecting(flag)
    }
    
    return <>
        <Modal show={isHubReconnecting} centered={true}>
            <Modal.Body className="text-center">
                <h2 style={{textAlign: "center"}}>Please Wait</h2>
                <hr />
                <h3>Reconnecting to the server...</h3>
                <Spinner animation="border" variant="primary" />
            </Modal.Body>
        </Modal>
        {props.children}
    </>
}
export default AuthenticationHoc