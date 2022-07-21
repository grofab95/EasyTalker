import React from 'react'
import { useSelector } from 'react-redux'
import { ApplicationState } from '../../store'
import { Modal, Spinner } from 'react-bootstrap'
import Header from './Header'

const Layout: React.FC = props => {
    const anyBusy = useSelector((state: ApplicationState) => state.file.isBusy)
    
    return <>
        <div className="wrapper">
            <Modal show={anyBusy} centered={true}>
                <Modal.Body className="text-center">
                    <h2>Please Wait</h2>
                    <Spinner animation="border" variant="primary" />
                </Modal.Body>
            </Modal> 
            <Header />
            <div className="page-wrapper">
                <div className="page-content">
                    {props.children}                    
                </div>
            </div>
            <div className="page-footer d-flex justify-content-between">
                <span>EasyTalker</span>
                <div>
                    <span className="me-4">Version: 1.0.0</span>
                </div>
            </div>
        </div>
    </>
}
export default Layout