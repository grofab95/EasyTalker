import React from 'react'

const UploadProgressBar: React.FC<{ Progress: number }> = props => {
    return <>
        <div className="progress">
            <div
                className="progress-bar progress-bar-info progress-bar-striped"
                role="progressbar"
                aria-valuenow={props.Progress}
                aria-valuemin={0}
                aria-valuemax={100}
                style={{width: props.Progress + '%'}}
            >
                {props.Progress}%
            </div>
        </div>
    </>
}
export default UploadProgressBar