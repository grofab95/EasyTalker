import './Header.css'
import { useLocation } from 'react-router-dom'
import { useDispatch, useSelector } from 'react-redux'
import { logout } from '../../store/userSession/api'
import React, { useState } from 'react'
import Logo from './Logo'
import { ApplicationState } from '../../store'
import { Container, Modal, Nav, Navbar, NavDropdown } from 'react-bootstrap'
import ChangePassword from '../../views/Users/components/ChangePassword'

const Header: React.FC = () => {
    const location = useLocation()
    const dispatch = useDispatch()

    const loggedUser = useSelector((state: ApplicationState) => state.user.loggedUser)
    const [showChangePasswordModal, setShowChangePasswordModal] = useState(false)

    const onLogoutClick = React.useCallback(() => {
        dispatch(logout())
    }, [dispatch])

    return <>
            <Modal show={showChangePasswordModal} onHide={() => setShowChangePasswordModal(false)} centered backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>
                        Change password
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <ChangePassword onSuccessful={() => setShowChangePasswordModal(false)} />
                </Modal.Body>
                <Modal.Footer/>
            </Modal>
            
            <Navbar expand="lg" className="navbar">
            <Container fluid>
                <Navbar.Brand>
                    <div className="d-flex align-items-center">
                        <Logo isNav={true}/>
                    </div>
                </Navbar.Brand>
                <Navbar.Toggle aria-controls="navbarScroll"/>
                <Navbar.Collapse id="navbarScroll" className="justify-content-end">
                    <Nav
                        className="d-flex"
                        style={{maxHeight: '100px', fontSize: '20px'}}
                        navbarScroll
                    >
                        <NavDropdown title={`Hello ${loggedUser.userName}`} id="navbarScrollingDropdown">
                            <NavDropdown.Item onClick={() => setShowChangePasswordModal(true)}>Change password</NavDropdown.Item>
                            <NavDropdown.Divider/>
                            <NavDropdown.Item onClick={onLogoutClick}>
                                Logout
                            </NavDropdown.Item>
                        </NavDropdown>
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    </>
}
export default Header