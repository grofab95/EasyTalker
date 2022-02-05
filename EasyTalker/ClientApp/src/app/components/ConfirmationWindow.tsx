import React from 'react'
import { Modal } from 'react-bootstrap'
import Button from 'react-bootstrap/Button'

interface Props {
    isOpen: boolean,
    onConfirm: () => void,
    onClose: () => void
}

const ConfirmationWindow: React.FC<Props> = props => {    
    return <>
        <Modal show={props.isOpen} center onHide={props.onClose} centered backdrop="static">
            <Modal.Header>
                <Modal.Title>
                    Please confirm
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {props.children}
            </Modal.Body>
            <Modal.Footer>
                <Button variant="success" onClick={props.onConfirm}>Ok</Button>
                <Button variant="danger" onClick={props.onClose}>Cancel</Button>
            </Modal.Footer>
        </Modal>
    </>
}
export default ConfirmationWindow