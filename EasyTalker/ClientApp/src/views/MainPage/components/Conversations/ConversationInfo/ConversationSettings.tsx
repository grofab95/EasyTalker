import React, {useState} from 'react'
import {Modal} from 'react-bootstrap'
import Button from 'react-bootstrap/Button'
import Conversation from '../../../../../interfaces/Conversations/Conversation'
import { ConversationStatus } from '../../../../../interfaces/Conversations/ConversationStatus'
import User from '../../../../../interfaces/Users/User'
import { updateConversationStatus } from '../../../../../utils/apiCalls'
import { successNotification, errorNotification } from '../../../../../utils/notifications/notificationFactory'
import AddParticipants from '../../Participants/AddParticipants'
import ParticipantsManagement from '../../Participants/ParticipantsManagement'

const ConversationSettings: React.FC<{ conversation: Conversation, users: User[] }> = props => {

    const [showAddParticipant, setShowAddParticipant] = useState<boolean>(false)
    const [showParticipantsManagement, setShowParticipantsManagement] = useState<boolean>(false)
    
    const updateStatus = async (status: ConversationStatus) => {
        const response = await updateConversationStatus({ conversationId: props.conversation.id, status: status })
        if (response.isSuccess) {
            successNotification('Status updated successfully')
        } else {
            errorNotification(response.error)
        }
    }
    
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

        <Modal show={showParticipantsManagement} onHide={() => setShowParticipantsManagement(false)} centered backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>
                    Participants managing
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <ParticipantsManagement conversation={props.conversation} onUpdated={() => setShowParticipantsManagement(false)} />
            </Modal.Body>
            <Modal.Footer/>
        </Modal>
        
        <div className="btn-group" role="group" aria-label="Basic example">
            <Button size="sm" className="btn btn-secondary" onClick={() => setShowAddParticipant(true)}>Add participant</Button>
            <Button size="sm" className="btn primary" onClick={() => setShowParticipantsManagement(true)}>Participants managing</Button>
            {props.conversation.status === ConversationStatus.Open 
                ? <Button size="sm" onClick={async () => await updateStatus(ConversationStatus.Closed)} className="btn btn-danger">Close conversation</Button> 
                : <Button size="sm" onClick={async () => await updateStatus(ConversationStatus.Open)} className="btn btn-success">Open conversation</Button>}
            
        </div>
    </>
}
export default ConversationSettings