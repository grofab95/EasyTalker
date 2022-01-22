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
import { HashRouter , Redirect, Route, Router } from 'react-router-dom'
import dashboard from './app/dashboard'
import { LoginView } from './views/LoginPage'
import RegisterView from './views/RegisterPage/RegisterView'

const history = createBrowserHistory()

ReactDOM.render(
    <Provider store={store}>
        <HashRouter basename='/'>
            <React.StrictMode>
                <ReactNotification />
                <Router history={history}>
                    <Route exact path="/" render={() => (<Redirect to="/app/dashboard/" />)} />
                    <Route exact path="/app" render={() => (<Redirect to="/app/dashboard/" />)} />
                    
                    <Route path="/login" component={LoginView} />
                    <Route path="/register" component={RegisterView} />
                    <Route path="/app/dashboard" component={dashboard} />
                </Router>
            </React.StrictMode>
        </HashRouter>
    </Provider>,
    document.getElementById('root')
)

