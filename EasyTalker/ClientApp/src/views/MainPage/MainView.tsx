import React, { useState } from 'react'
import { Col, Modal, Row } from 'react-bootstrap'
import ConversationsList from './components/ConversationsList'
import Button from 'react-bootstrap/Button'
import ConversationView from './components/ConversationView'
import ConversationCreate from './components/ConversationCreate'
import { useDispatch, useSelector } from 'react-redux'
import { ApplicationState } from '../../store'
import { updateLatSeenAt } from '../../store/conversations/api'
import ConversationInfo from './components/ConversationInfo'

const MainView: React.FC = () => {

    const conversations = useSelector((state: ApplicationState) => state.conversation.conversationList)        
    const dispatch = useDispatch()
    const [showNewConversationModal, setNewConversationModal] = useState(false)
    const [selectedConversationId, setSelectedConversationId] = useState<number>(0)

    if (conversations === null) {
        console.log('conversations === undefined')
        return <p>loading</p>
    }
    
    const onConversationSelected = (id: number) => {
        console.log(`onConversationSelected id=${id}`)
        setSelectedConversationId(id)
        updateLastSeenAt(id)
    }

    const updateLastSeenAt = (id?: number) => {                
        const conversationId = id ?? selectedConversationId
        const conversation = conversations.find(x => x.id === conversationId)
        
        if (conversation && conversation.lastMessage?.createdAt < conversation.lastSeenAt)
            return
        
        dispatch(updateLatSeenAt({
            conversationId: conversationId
        }))
    }
    
    return <>
        <Modal show={showNewConversationModal} onHide={() => setNewConversationModal(false)} centered>
            <Modal.Header closeButton>
                <Modal.Title>
                    New conversation
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <ConversationCreate onCreate={() => setNewConversationModal(false)} />
            </Modal.Body>
            <Modal.Footer/>
        </Modal>

        <Row>
            <Col lg="2">
                <Button onClick={() => setNewConversationModal(true)} className="w-100 btn-secondary mb-2">New Conversation</Button>
                <ConversationsList conversationIdSelected={(id) => onConversationSelected(id)} />
            </Col>
            
            <Col lg="7" onClick={() => updateLastSeenAt()}>
                <ConversationView conversationId={selectedConversationId} />
            </Col>

            <Col lg="3" onClick={() => updateLastSeenAt()}>
                <ConversationInfo conversationId={selectedConversationId} />
            </Col>
        </Row>
    </>
}

export default MainView