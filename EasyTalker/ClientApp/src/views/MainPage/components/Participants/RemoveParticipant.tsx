import React, { useState } from 'react'
import Conversation from "../../../../interfaces/Conversations/Conversation";

const RemoveParticipant: React.FC<{ conversation: Conversation, onUpdated: () => void }> = props => {
    return <></>
    // const users = useSelector((state: ApplicationState) => state.user.userList)
    //     .filter(u => props.conversation.participants
    //         .filter(u => u.hasAccess && u.id !== getLoggedUserId())
    //         .map(p => p.id).includes(u.id))
    //
    // const [selectedParticipantsIds, setSelectedParticipantsIds] = useState<string[]>([])
    // const dispatch = useDispatch()
    //
    // const remove = () => {
    //     dispatch(removeParticipants({
    //         conversationId: props.conversation.id,
    //         participantsIds: selectedParticipantsIds,
    //         onUpdated: () => props.onUpdated()
    //     }))
    // }
    //
    // return <>
    //     <UsersSelection users={users} onSelectionChanged={(u) => setSelectedParticipantsIds(u.map(x => x.id))}/>
    //     <Button disabled={selectedParticipantsIds.length === 0}
    //             onClick={() => remove()}>
    //         Remove
    //     </Button>
    // </>
}
export default RemoveParticipant