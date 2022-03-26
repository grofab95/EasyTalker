import React, {useState} from "react";
import {useDispatch, useSelector} from "react-redux";
import { ApplicationState } from "../../store";
import {FileType} from "../../interfaces/Files/FileType";
import {Col, Modal, Row} from "react-bootstrap";
import {updateLatSeenAt} from "../../store/conversations/api";
import ConversationCreate from "./components/Conversations/ConversationCreate";
import Button from "react-bootstrap/Button";
import ConversationsList from "./components/Conversations/ConversationsList";
import ConversationView from "./components/Conversations/ConversationView";
import ConversationInfo from "./components/Conversations/ConversationInfo";


const MainView: React.FC = () => {

    const conversations = useSelector((state: ApplicationState) => state.conversation.conversationList)
    const files = useSelector((state: ApplicationState) => state.file.files)
    const dispatch = useDispatch()
    const [showNewConversationModal, setNewConversationModal] = useState(false)
    const [selectedConversationId, setSelectedConversationId] = useState<number>(0)

    if (conversations === null) {
        console.log('conversations === undefined')
        return <p>loading</p>
    }

    const onConversationSelected = (id: number) => {
        setSelectedConversationId(id)
        updateLastSeenAt(id)
    }

    const updateLastSeenAt = (id?: number) => {
        const conversationId = id ?? selectedConversationId
        const conversation = conversations.find(x => x.id === conversationId)
        if (!conversation)
            return

        const lastImage = [...files]
            .sort((a, b) => (a.createdAt > b.createdAt ? -1 : 1))
            .find(f => f.externalId === conversation.id.toString() && f.fileType === FileType.Image)

        let lastMessageDate = conversation.lastMessage?.createdAt

        if (lastImage && lastImage.createdAt > lastMessageDate) {
            lastMessageDate = lastImage.createdAt
        }

        if (lastMessageDate < conversation.lastSeenAt)
            return

        dispatch(updateLatSeenAt({
            conversationId: conversationId
        }))
    }

    return <>
        <Modal show={showNewConversationModal} onHide={() => setNewConversationModal(false)} centered backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>
                    New conversation
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <ConversationCreate onCreate={() => setNewConversationModal(false)}/>
            </Modal.Body>
            <Modal.Footer/>
        </Modal>

        <Row>
            <Col lg="2">
                <Button onClick={() => setNewConversationModal(true)} className="w-100 btn-secondary mb-2">New
                    Conversation</Button>
                <ConversationsList conversationIdSelected={(id) => onConversationSelected(id)}/>
            </Col>

            <Col lg="7" onClick={() => updateLastSeenAt()}>
                <ConversationView conversationId={selectedConversationId}/>
            </Col>

            <Col lg="3" onClick={() => updateLastSeenAt()}>
                <ConversationInfo conversationId={selectedConversationId}/>
            </Col>
        </Row>
    </>
}

export default MainView