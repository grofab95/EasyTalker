import { FileType } from './FileType'

export interface FileInfo {
    id: number,
    externalId: string,
    ownerId: string,
    fileName: string,
    fileStatus: string
    fileType: FileType,
    createdAt: Date
}