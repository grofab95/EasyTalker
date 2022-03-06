import {FileInfo} from "../../interfaces/Files/FileInfo";
import {apiUrl} from "../../store/config";

export const getFileUrl = (file: FileInfo) => {
    return `${apiUrl}/static/${file.externalId}/${file.fileName}`
}