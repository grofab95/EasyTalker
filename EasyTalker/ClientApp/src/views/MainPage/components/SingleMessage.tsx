import React from 'react'
import Message from '../../../interfaces/Messages/Message'
import { getLoggedUserId } from '../../../utils/authUtils'
import { formatDate } from '../../../utils/unitFormaters'

const SingleMessage: React.FC<Message> = message => {
    
    if (message.sender === undefined) {
        return <p><b>Undefined Sender.</b></p>
    }
    
    const textAlign = message.sender.id === getLoggedUserId() ? 'right' : 'left'
          
    return <div style={{textAlign: textAlign}}>        
        <p className="mb-0"><b>{message.sender.userName} ({formatDate(message.createdAt, "DD.MM.YYYY HH:mm")}):</b></p>
        {message.isImage ? <img style={{maxWidth: '30%'}} src={message.text} alt="image" /> : <p>{message.text}</p>}
    </div>
}
export default SingleMessage