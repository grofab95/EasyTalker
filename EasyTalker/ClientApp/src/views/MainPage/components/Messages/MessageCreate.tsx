import React, { useState } from 'react'
import { Col, FormControl, InputGroup, Row } from 'react-bootstrap'
import { useDispatch } from 'react-redux'
import {addMessage} from "../../../../store/conversations/api";
import {getLoggedUserId} from "../../../../utils/authUtils";
import NewMessage from "../../../../interfaces/Messages/NewMessage";

const MessageCreate: React.FC<{ conversationId: number }> = (props) => {
    const dispatch = useDispatch()

    const [text, setText] = useState('')
    
    const onSubmit = () => {
        if (!text) {
            return;
        }
        
        dispatch(addMessage({
            newMessage: {
                senderId: getLoggedUserId(),
                conversationId: props.conversationId,
                text: text
            } as NewMessage
        }))
        
        setText('')
    }
    
    return <>
        <InputGroup>
            <FormControl
                onKeyDown={(e) => e.key === 'Enter' ? onSubmit() : ''}
                onChange={e => setText(e.target.value)}
                value={text}
                placeholder="Enter message..."
                aria-label="Enter message..."
                aria-describedby="basic-addon2"
            />
        </InputGroup>
    </>
}
export default MessageCreate