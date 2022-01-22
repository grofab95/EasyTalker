import './Header.css'

import { useLocation } from 'react-router-dom'
import { useDispatch, useSelector } from 'react-redux'
import { getUser, logout } from '../../store/userSession/api'
import React from 'react'
import Logo from './Logo'
import { getLoggedUserId } from '../../utils/authUtils'
import { ApplicationState } from '../../store'

const Header: React.FC = () => {
    const location = useLocation()
    const dispatch = useDispatch()
    
    const loggedUser = useSelector((state: ApplicationState) => state.user.loggedUser)
    
    const onLogoutClick = React.useCallback(() => {
        dispatch(logout())
    }, [dispatch])
        
    return <header className="topbar">
        <div className="d-flex align-items-center">
            <Logo isNav={true} />
        </div>
        {/*<nav className="navbar-expand w-100 d-flex flex-row justify-content-start ms-3">*/}
        {/*    /!*menu*!/*/}
        {/*</nav>*/}
        {loggedUser.userName}
        <button className="btn btn-outline-info" onClick={onLogoutClick}>
            <i className="bx bx-log-out-circle me-2" />
            Logout
        </button>
        
    </header>
}
export default Header