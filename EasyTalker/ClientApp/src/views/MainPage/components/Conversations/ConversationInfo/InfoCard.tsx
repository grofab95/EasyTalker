import { Card } from 'react-bootstrap'
import styles from '../ConversationInfo/InfoCard.module.css'

interface Props {
    title?: string
}

const InfoCard: React.FC<Props> = props => {
    return <>        
        <Card className={styles.card}>
            {props.title && <>
                <h5 style={{marginBottom: '0px'}}>{props.title}</h5>
                <hr />
            </>}
            
            {props.children}
        </Card>
    </>
}
export default InfoCard