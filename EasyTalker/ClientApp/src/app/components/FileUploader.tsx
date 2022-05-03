import React, { useCallback, useState } from 'react'
import { useDropzone } from 'react-dropzone'
import UploadProgressBar from './UploadProgressBar'
import { convertBytesToReadableFileSize } from '../../utils/unitConverters'
import Button from 'react-bootstrap/Button'
import { v4 as uuid } from 'uuid';
import { useDispatch } from 'react-redux'
import { uploadFile } from '../../store/files/api'

const FileUploader: React.FC<{ externalId: string }> = props => {
    
    const dispatch = useDispatch()
    const [selectedFile, setSelectedFile] = useState<File>()
    const [uploadProgress, setUploadProgress] = useState(0)
    const [uploadId, setUploadId] = useState('')
    
    const onDrop = useCallback(acceptedFiles => {
        setSelectedFile(acceptedFiles[0])
    }, [])
    
    const {getRootProps, getInputProps} = useDropzone({onDrop: onDrop})
    
    const upload = async () => {
        if (selectedFile === undefined)
            return
        
        const currentUploadId = uuid()
        setUploadId(currentUploadId)
        
        dispatch(uploadFile({
            file: selectedFile,
            uploadId: currentUploadId,
            externalId: props.externalId,
            onUploadProgress: onUploadProgress,
            onUploadResponse: onSuccessfulUpload
        }))
    }
    
    const onUploadProgress = (event: ProgressEventInit) => {
        if (event === null)
            return
        
        const loaded = event.loaded ?? 0
        const total = event.total ?? 0
        
        setUploadProgress(Math.round((100 * loaded) / total))
    }
    
    const onSuccessfulUpload = () => {
        setUploadProgress(0)
        setSelectedFile(undefined)
    }
    
    return <>
        <div>
            {uploadProgress > 0
                ? <UploadProgressBar Progress={uploadProgress} />
                : <></>
            }
        </div>
        <div>
            <section style={{height: "70px", border: "1px solid black", borderRadius: "10px"}}>
                <div {...getRootProps({className: 'fileDrop h-100 w-100'})}>
                    <div>
                        <input {...getInputProps()} />
                        <p style={{textAlign: "center"}}>Drag and drop file or click to select</p>
                    </div>
                    <aside className="fileDropFilesInfo">
                        {selectedFile &&
                            <p style={{textAlign: "center"}}>{selectedFile.name} - {convertBytesToReadableFileSize(selectedFile.size)}</p>}
                    </aside>
                </div>
            </section>
            <div className="d-flex justify-content-center mt-2">
                <Button onClick={upload}
                        disabled={selectedFile === undefined}>
                    Upload File
                </Button>
            </div>
        </div>
    </>
}
export default FileUploader