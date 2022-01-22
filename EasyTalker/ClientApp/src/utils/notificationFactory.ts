import { ReactNotificationOptions, store } from 'react-notifications-component'

type NotificationTypes = 'success' | 'danger' | 'info' | 'default' | 'warning'

const createNotification = (title: string, message: string, type: NotificationTypes): ReactNotificationOptions => {
    return {
        title: title,
        message: message,
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