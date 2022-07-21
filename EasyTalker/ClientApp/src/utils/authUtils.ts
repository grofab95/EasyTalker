import AuthTokens from '../interfaces/AuthTokens'
import * as jwt from 'react-jwt'

export const ClaimType = {
    PERMISSION: 'https://easytalker.pl/identity/claims/permission',
    ROLE: 'https://easytalker.pl/identity/claims/role',
    NAME_IDENTIFIER: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
}

const TOKENS_KEY = 'tokens'

export const getTokens = (): AuthTokens => {
    try {
        const storedTokens = localStorage.getItem(TOKENS_KEY)
        
        if (!storedTokens)
            return {} as AuthTokens

        return JSON.parse(storedTokens) as AuthTokens
    } catch (error) {
        return {} as AuthTokens
    }
}

export const getAccessToken = () => {
    const allTokens = getTokens()

    return allTokens?.accessToken ?? ''
}

export const saveTokens = (authTokens: AuthTokens) => {
    localStorage.setItem(TOKENS_KEY, JSON.stringify(authTokens))
}

export const deleteTokens = () => {
    console.log('deleteTokens')
    localStorage.removeItem(TOKENS_KEY)
}

export const getPermissions = (): string[] => {
    const token = getAccessToken()
    if (token === '')
        return []
    
    const permissions = jwt.decodeToken(token)[ClaimType.PERMISSION]
    if (Array.isArray(permissions))
        return permissions
    
    return [permissions] as string[]
}

export const hasPermissions = (permission: string): boolean => {
    const userPermission = getPermissions()
    console.log(userPermission)
    return userPermission.includes(permission)
}

export const getLoggedUserId = () => {
    const token = getAccessToken()
    if (token === '')
        return []
    
    return jwt.decodeToken(token)[ClaimType.NAME_IDENTIFIER]
}

export const getRoles = (): string[] => {
    const token = getAccessToken()
    if (token === '')
        return []
    
    const roles = jwt.decodeToken(token)[ClaimType.ROLE]
    if (Array.isArray(roles))
        return roles
    
    return [roles] as string[]
}