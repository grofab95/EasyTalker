import { combineReducers, ReducersMapObject } from 'redux'
import { ApplicationState, reducers } from './index'
import { signalrMiddleware } from './configureSignalrConnection'
import { configureStore, getDefaultMiddleware } from '@reduxjs/toolkit'

const buildRootReducer = (allReducers: ReducersMapObject<ApplicationState>) => {
    return combineReducers<ApplicationState>(Object.assign({}, allReducers))
}

const allReducers = buildRootReducer(reducers)
export const store = configureStore({ reducer: allReducers, middleware: [signalrMiddleware, ...getDefaultMiddleware()]})