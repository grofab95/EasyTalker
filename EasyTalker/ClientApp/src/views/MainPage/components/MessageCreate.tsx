import React, { useState } from 'react'
import { Col, FormControl, InputGroup, Row } from 'react-bootstrap'
import { useDispatch } from 'react-redux'
import { addMessage } from '../../../store/conversations/api'
import NewMessage from '../../../interfaces/Messages/NewMessage'
import { getLoggedUserId } from '../../../utils/authUtils'

const MessageCreate: React.FC<{ conversationId: number }> = (props) => {
    const dispatch = useDispatch()

    const [text, setText] = useState('')
    
    const onSubmit = () => {
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
                placeholder="Napisz coś..."
                aria-label="Napisz coś..."
                aria-describedby="basic-addon2"
            />
        </InputGroup>
    </>
}
export default MessageCreate