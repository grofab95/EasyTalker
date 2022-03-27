import React from "react";
import User from "../../../../interfaces/Users/User";
import {ConversationAccessStatus} from "../../../../interfaces/Conversations/ConversationAccessStatus";
import {updateUserConversationAccessStatus} from "../../../../utils/apiCalls";
import {errorNotification, successNotification} from "../../../../utils/notifications/notificationFactory";
import styles from "../Participants/ParticipantDetails.module.css"

interface Props {
    conversationId: number,
    user: User,
    accessStatus: ConversationAccessStatus | undefined
}

const ParticipantDetails: React.FC<Props> = props => {
    
    const accessStatus = props.accessStatus ?? ''
    
    const onSelect = async (accessStatus: ConversationAccessStatus) => {
        const response = await updateUserConversationAccessStatus({
            conversationId: props.conversationId,
            participantId: props.user.id,
            accessStatus: accessStatus
        })
        
        if (response.isSuccess) {
            successNotification(`User ${props.user.userName} has successfully changed access status to ${accessStatus}`)
        } else {
            errorNotification(response.error)
        }
    }
    
    return <div className={`row mb-3 ${styles.participantDetailRow}`}>
        <div className={`col ${styles.participantUsernameCol}`}>
            {props.user.userName}
        </div>

        <div className="col">
            <select className={`form-select ${styles.participantAccessStatusSelect}`} onChange={e => onSelect((ConversationAccessStatus as any)[e.target.value])} defaultValue={accessStatus}>
                {Object.values(ConversationAccessStatus).map((x, i) => <option key={i}>{x}</option>)}
            </select>
        </div>
    </div>
}
export default ParticipantDetails