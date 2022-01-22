import './Logo.css'
import React from 'react'

interface Props {
    isNav: boolean
}
const Logo: React.FC<Props> = props => {     
    return <>
        {[...'EasyTalker'] .map((ch, i) => <span key={i} className={`LogoTxt ${props.isNav ? 'nav' : ''}`}>{ch}</span>)}
    </>
}
export default Logo