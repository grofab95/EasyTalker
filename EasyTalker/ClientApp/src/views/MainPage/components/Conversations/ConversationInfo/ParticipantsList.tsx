import styles from '../ConversationInfo/ParticipantsList.module.css'
import UserConnectionStatusIndicator from "../../../../../app/components/UserConnectionStatusIndicator"
import { useSelector } from 'react-redux'
import { ApplicationState } from '../../../../../store'
import React from "react";

interface Props {
    participantsIds: string[]
}

const ParticipantsList: React.FC<Props> = props => {
       
    const onlineUsersIds = useSelector((state: ApplicationState) => state.user.userList
        .filter(x => x.isOnline && props.participantsIds.includes(x.id))
        .map(y => y.id))
        
    const offlineUsersIds = props.participantsIds.filter(x => !onlineUsersIds.includes(x))
    const participants = [...onlineUsersIds, ...offlineUsersIds]

    return <>   
        <div className={styles.participantsList}>
            <ul>
                {participants?.map((id, i) => <UserConnectionStatusIndicator key={i} userId={id}/>)}  
            </ul>
        </div>
    </>
}
export default ParticipantsList