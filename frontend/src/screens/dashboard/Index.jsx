import React from 'react';
import { Route, Switch } from 'react-router-dom';
import Dashboard from './dashboard';

function DashboardIndex({ match }) {
    const { path } = match;

    return (
        <div className="p-4">
            <div className="container">
                <Switch>
                    <Route path={`${path}`} component={Dashboard} />
                </Switch>
            </div>
        </div>
    );
}

export { DashboardIndex };