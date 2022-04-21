import React from "react";
import { ConversationAccessStatus } from "../../../../interfaces/Conversations/ConversationAccessStatus";
import { updateUserConversationAccessStatus } from "../../../../utils/apiCalls";
import { errorNotification, successNotification } from "../../../../utils/notifications/notificationFactory";
import styles from "../Participants/ParticipantDetails.module.css"
import ParticipantInfo from "./interfaces/ParticipantInfo";

interface Props {
	conversationId: number,
	participant: ParticipantInfo
}

const ParticipantDetails: React.FC<Props> = props => {

	const onSelect = async (accessStatus: ConversationAccessStatus) => {
		const response = await updateUserConversationAccessStatus({
			conversationId: props.conversationId, 
			participantId: props.participant.user.id, 
			accessStatus: accessStatus
		})

		if (response.isSuccess) {
			successNotification(`User ${props.participant.user.userName} has successfully changed access status to ${accessStatus}`)
		} else {
			errorNotification(response.error)
		}
	}

	return <div className={`row mb-3 ${styles.participantDetailRow}`}>
		<div className={`col ${styles.participantUsernameCol}`}>
			{props.participant.user.userName}
		</div>

		<div className="col">
			<select
				className={`form-select form-select-sm w-100 ${styles.participantAccessStatusSelect}`}
				onChange={e => onSelect((ConversationAccessStatus as any)[e.target.value])}
				value={props.participant.accessStatus}>
					{Object.values(ConversationAccessStatus).map((x, i) => <option key={i}>{x}</option>)}
			</select>
		</div>
	</div>
}
export default ParticipantDetails