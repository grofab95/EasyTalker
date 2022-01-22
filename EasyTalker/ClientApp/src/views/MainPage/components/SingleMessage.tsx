import React from 'react'
import Message from '../../../interfaces/Messages/Message'
import { getLoggedUserId } from '../../../utils/authUtils'
import { formatDate } from '../../../utils/unitFormaters'

const SingleMessage: React.FC<Message> = props => {
    
    const textAlign = props.sender.id === getLoggedUserId() ? 'right' : 'left'
       
    return <div style={{textAlign: textAlign}}>
        <p className="mb-0"><b>{props.sender.userName} ({formatDate(props.createdAt, "DD.MM.YYYY HH:mm")}):</b></p>
        <p>{props.text}</p>
    </div>
}
export default SingleMessage