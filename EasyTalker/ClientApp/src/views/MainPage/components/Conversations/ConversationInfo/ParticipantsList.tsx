import { Row } from "react-bootstrap"
import styles from '../ConversationInfo/ParticipantsList.module.css'
import UserConnectionStatusIndicator from "../../../../../app/components/UserConnectionStatusIndicator"

interface Props {
    participantsIds: string[]
}

const ParticipantsList: React.FC<Props> = props => {
       
    return <>   
        <div className={styles.participantsList}>
            <Row>
                {props.participantsIds?.map((id, i) => <UserConnectionStatusIndicator key={i}
                                                                                                                userId={id}/>)}        
            </Row>
        </div>
    </>
}
export default ParticipantsList