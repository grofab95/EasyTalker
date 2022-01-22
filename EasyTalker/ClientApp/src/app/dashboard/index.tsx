import { RouteComponentProps } from 'react-router'
import { Route, useRouteMatch } from 'react-router-dom'
import ensureNonExpiredTokens from '../../utils/ensureNonExpiredTokens'
import React from 'react'
import AuthenticationHoc from './AuthenticationHoc'
import Layout from './Layout'
import FetchDashboardData from './FetchDashboardData'
import MainView from '../../views/MainPage/MainView'

const Dashboard: React.FC<RouteComponentProps> = props => {
    React.useEffect(() => {
        ensureNonExpiredTokens()
    }, [])
    let { path, url } = useRouteMatch();
    return <AuthenticationHoc>
        <Layout>
            <Route exact path={`${path}/`} component={MainView} />
        </Layout>
        <FetchDashboardData />
    </AuthenticationHoc>
}
export default Dashboard