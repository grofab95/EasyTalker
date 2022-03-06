import React from 'react'
import Message from '../../../interfaces/Messages/Message'
import { getLoggedUserId } from '../../../utils/authUtils'
import { formatDate } from '../../../utils/unitFormaters'
import styles from '../../MainPage/components/SingleMessage.module.css'

const SingleMessage: React.FC<Message> = message => {
    
    if (message.sender === undefined) {
        return <p><b>Undefined Sender.</b></p>
    }
    
    const isSenderLeggedUser = message.sender.id === getLoggedUserId()    
    const textAlign = isSenderLeggedUser ? 'right' : 'left'    
    const imageAlign = isSenderLeggedUser ? 'flex-start' : 'flex-end'

    const imageMessage = () =>
        <div className={styles.imageMessage} style={{alignItems: imageAlign}}>
            <img style={{maxWidth: '30%'}} src={message.text} alt="image" />
            <a href={message.text}>{message.text.split('/').slice(-1).pop()}</a>
        </div>
          
    return <div style={{textAlign: textAlign}}>        
        <p className="mb-0"><b>{message.sender.userName} ({formatDate(message.createdAt, "DD.MM.YYYY HH:mm")}):</b></p>
        {message.isImage ? imageMessage() : <p>{message.text}</p>}
    </div>
}
export default SingleMessage