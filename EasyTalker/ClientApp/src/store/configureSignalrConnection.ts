﻿import { Callback, HubConnectionBuilder, LogLevel, RetryContext, signalMiddleware, withCallbacks } from 'redux-signalr'
import { hubUrl } from './config'
import { getAccessToken } from '../utils/authUtils'
import { userSessionCallbacks } from './userSession/hubEvents'
import { conversationCallbacks } from './conversations/hubEvents'
import { userCallbacks } from './users/hubEvents'

export const connection = new HubConnectionBuilder()
    .configureLogging(LogLevel.Debug)
    .withUrl(hubUrl, {
        accessTokenFactory(): string | Promise<string> {
            return getAccessToken()
        }
    })
    .withAutomaticReconnect({
        nextRetryDelayInMilliseconds(retryContext: RetryContext): number | null {
            return 5000
        }
    })
    .build()

const callbacksCombine = (callbacks: Map<string, Callback<any, any>>[]) => {
    const combined = withCallbacks()
    callbacks.forEach(callback => {
        callback.forEach((v, k) => {
            combined.add(k, v)
        })
    })
    return combined
}

const callbacks = callbacksCombine([conversationCallbacks, userCallbacks])

export const signalrMiddleware = signalMiddleware({callbacks, connection, shouldConnectionStartImmediately: false})
