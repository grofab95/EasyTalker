﻿import { ReactNotificationOptions, store } from 'react-notifications-component'
import * as React from 'react'
import MultiErrorWrapper from './MultiErrorWrapper'

type NotificationTypes = 'success' | 'danger' | 'info' | 'default' | 'warning'

const createNotification = (title: string, message: string | React.FunctionComponent | null, type: NotificationTypes, content: React.ComponentClass | React.FunctionComponent | React.ReactNode | undefined = undefined): ReactNotificationOptions => {
    return {
        title: title,
        message: message ?? content,
        type: type,
        insert: 'top',
        container: 'top-right',
        animationIn: ['animate__animated', 'animate__backInDown'],
        animationOut: ['animate__animated', 'animate__backOutDown'],
        dismiss: {
            duration: 5000,
            onScreen: true,
            showIcon: true
        }
    }
}

export const errorNotificationFromMany = (messages: string[]) => {
    store.addNotification(createNotification('Error', null, 'danger', MultiErrorWrapper({ messages: messages })))
}

export const errorNotification = (message: string) => {
    store.addNotification(createNotification('Error', message, 'danger'))
}

export const warningNotification = (message: string) => {
    store.addNotification(createNotification('Warning', message, 'warning'))
}

export const infoNotification = (message: string) => {
    store.addNotification(createNotification('Info', message, 'info'))
}

export const successNotification = (message: string) => {
    store.addNotification(createNotification('Success', message, 'success'))
}