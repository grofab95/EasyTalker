import React from "react";
import Conversation from "../../../../interfaces/Conversations/Conversation";
import { useSelector } from "react-redux";
import { ApplicationState } from "../../../../store";
import User from "../../../../interfaces/Users/User";
import ParticipantDetails from "./ParticipantDetails";
import styles from "../Participants/ParticipantsManagement.module.css"
import { ConversationAccessStatus } from "../../../../interfaces/Conversations/ConversationAccessStatus";
import { useState } from "react";
import ParticipantInfo from "./interfaces/ParticipantInfo";

const ParticipantsManagement: React.FC<{ conversation: Conversation, onUpdated: () => void }> = props => {

    const getAccessStatus = (user: User) => {
        return props.conversation.participants.find(u => u.id === user.id)?.accessStatus
    }

    const [filteringUsername, setFilteringUsername] = useState<string | undefined>(undefined)
    const [filteringAccessStatus, setfilteringAccessStatus] = useState<string | undefined>(undefined)

    const users = useSelector((state: ApplicationState) => state.user.userList
        .filter(u => props.conversation.participants.some(p => p.id === u.id && p.id !== props.conversation.creatorId)))

    const accessStatuses = Object.values(ConversationAccessStatus)
        .map(ac => ac.toString())
        .concat([ 'All' ])
        .sort()

    const particpants = users.map(user => {
        const accessStatus = getAccessStatus(user)

        return {
            user: user,
            accessStatus: accessStatus
        } as ParticipantInfo
    })

    const getFilteredParticipants = () => {
        let filteringUsers = [...particpants]

        if (filteringUsername) {
            filteringUsers = filteringUsers.filter(u => u.user.userName.includes(filteringUsername))
        }

        if (filteringAccessStatus) {
            filteringUsers = filteringAccessStatus === 'All'
                ? filteringUsers
                : filteringUsers.filter(u => u.accessStatus === filteringAccessStatus)
        }

        return filteringUsers
    }

    const getParticipantsList = () => {
        const participants = getFilteredParticipants()
        if (participants.length > 0) {
            return <>
                {getFilteredParticipants().sort((a, b) => (a.user.userName > b.user.userName ? 1 : -1)).map((participant, i) => 
                    <ParticipantDetails key={i}
                                        conversationId={props.conversation.id}
                                        participant={participant} />)}
            </>
        }

        return <h3 className="text-center">Participants not found</h3>
    }
   
    return <> 
        <div className="row w-100 mb-3">
            <div className="col">
                <input type="text" className="form-control" onChange={e => setFilteringUsername(e.target.value)} />
            </div>

            <div className="col">
                <select
                    className="form-select form-select-sm w-100"
                    style={{fontSize: "1rem"}}
                    onChange={e => setfilteringAccessStatus(e.target.value)}                     
                    defaultValue={'All'}>
                        {accessStatuses.map((x, i) => <option key={i}>{x}</option>)}
                </select>
            </div>
        </div>

        <div className={styles.participantsList}>
            {getParticipantsList()}

            
        </div>
    </>
}
export default ParticipantsManagement