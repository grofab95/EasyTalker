import React from "react";
import Conversation from "../../../../interfaces/Conversations/Conversation";
import {useSelector} from "react-redux";
import {ApplicationState} from "../../../../store";
import User from "../../../../interfaces/Users/User";
import ParticipantDetails from "./ParticipantDetails";
import styles from "../Participants/ParticipantsManagement.module.css"

const ParticipantsManagement: React.FC<{ conversation: Conversation, onUpdated: () => void }> = props => {

    const users = useSelector((state: ApplicationState) => state.user.userList
        .filter(u => props.conversation.participants.some(p => p.id === u.id && p.id !== props.conversation.creatorId)))

    const getAccessStatus = (user: User) => {
        return props.conversation.participants.find(u => u.id === user.id)?.accessStatus
    }
   
    return <div className={styles.participantsList}>
        {users && users.sort((a, b) => (a.userName > b.userName ? 1 : -1)).map((user, i) => 
                <ParticipantDetails key={i}
                                    conversationId={props.conversation.id}
                                    user={user}
                                    accessStatus={getAccessStatus(user)} />)}
    </div>
}
export default ParticipantsManagement