import React, { useState } from 'react'
import Conversation from '../../../interfaces/Conversations/Conversation'
import User from '../../../interfaces/Users/User'
import { Modal } from 'react-bootstrap'
import AddParticipants from './AddParticipants'
import RemoveParticipant from './RemoveParticipant'
import Button from 'react-bootstrap/Button'

const ConversationSettings: React.FC<{ conversation: Conversation, users: User[] }> = props => {

    const [showAddParticipant, setShowAddParticipant] = useState<boolean>(false)
    const [showRemoveParticipant, setShowRemoveParticipant] = useState<boolean>(false)
    
    return <>
        <Modal show={showAddParticipant} onHide={() => setShowAddParticipant(false)} centered backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>
                    Add participants
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <AddParticipants conversation={props.conversation} onUpdated={() => setShowAddParticipant(false)} />
            </Modal.Body>
            <Modal.Footer/>
        </Modal>

        <Modal show={showRemoveParticipant} onHide={() => setShowRemoveParticipant(false)} centered backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>
                    Remove participants
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <RemoveParticipant conversation={props.conversation} onUpdated={() => setShowRemoveParticipant(false)} />
            </Modal.Body>
            <Modal.Footer/>
        </Modal>
        
        <div className="btn-group" role="group" aria-label="Basic example">
            <Button size="sm" className="btn btn-secondary" onClick={() => setShowAddParticipant(true)} >Add participant</Button>
            <Button size="sm" className="btn btn-secondary" onClick={() => setShowRemoveParticipant(true)}>Remove participant</Button>
            <Button size="sm" className="btn btn-danger">Close conversation</Button>
        </div>
    </>
}
export default ConversationSettings