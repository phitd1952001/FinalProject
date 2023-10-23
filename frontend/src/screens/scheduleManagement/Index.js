import React from 'react';
import { Route, Switch } from 'react-router-dom';
import { Settings } from './settings/Index';

function Schedule({ match }) {
    const { path } = match;

    return (
        <div className="p-4">
            <div className="container">
                <Switch>
                    <Route path={`${path}/setting`} component={Settings} />
                </Switch>
            </div>
        </div>
    );
}

export { Schedule };