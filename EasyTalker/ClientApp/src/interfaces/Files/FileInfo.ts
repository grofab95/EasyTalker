import { FileType } from './FileType'

export interface FileInfo {
    dbId: number,
    externalId: string,
    ownerId: string,
    fileName: string,
    fileStatus: string
    fileType: FileType,
    createdAt: Date
}