import { Row } from "react-bootstrap"
import styles from '../ConversationInfo/ParticipantsList.module.css'
import UserConnectionStatusIndicator from "../../../../../app/components/UserConnectionStatusIndicator"

interface Props {
    participantsIds: string[]
}

const ParticipantsList: React.FC<Props> = props => {
       
    return <>   
        <div className={styles.participantsList}>
            <ul>
                {props.participantsIds?.map((id, i) => <UserConnectionStatusIndicator key={i} userId={id}/>)}  
            </ul>
        </div>
    </>
}
export default ParticipantsList