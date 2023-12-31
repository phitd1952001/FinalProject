import React from 'react';
import { Route, Switch } from 'react-router-dom';
import { Settings } from './settings/Index';
import { Calendar } from './calendars/Index';

function Schedule({ match }) {
    const { path } = match;

    return (
        <div className="p-4">
            <div className="container">
                <Switch>
                    <Route path={`${path}/setting`} component={Settings} />
                    <Route path={`${path}/calendar`} component={Calendar} />
                </Switch>
            </div>
        </div>
    );
}

export { Schedule };