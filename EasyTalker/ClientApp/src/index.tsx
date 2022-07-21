import './assets/css/perfect-scrollbar.css'
import './assets/css/pace.min.css'
import './assets/css/header-colors.css'
import './assets/css/dark-theme.css'
import './assets/css/semi-dark.css'
import 'react-notifications-component/dist/theme.css'
import 'simplebar/dist/simplebar.min.css'
import 'animate.css/animate.min.css'
import 'bootstrap/dist/css/bootstrap.min.css'
import './assets/css/app.css'
import ReactNotification from 'react-notifications-component'
import React from 'react'
import ReactDOM from 'react-dom'
import './index.css'
import { Provider } from 'react-redux'
import { store } from './store/configureStore'
import { createBrowserHistory } from 'history'
import { BrowserRouter, Redirect, Route, Router } from 'react-router-dom'
import dashboard from './app/dashboard'
import { LoginView } from './views/LoginPage'

const history = createBrowserHistory()

ReactDOM.render(
    <Provider store={store}>
        <BrowserRouter basename='/'>
            <React.StrictMode>
                <ReactNotification/>
                <Router history={history}>
                    <Route path="/" component={dashboard}/>
                    <Route path="/login" component={LoginView}/>
                </Router>
            </React.StrictMode>
        </BrowserRouter>
    </Provider>,
    document.getElementById('root')
)

