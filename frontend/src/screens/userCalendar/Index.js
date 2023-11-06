import React from 'react';
import { Route, Switch } from 'react-router-dom';
import { Calendar } from './calendars/Index';

function UserCalendar({ match }) {
    const { path } = match;

    return (
        <div className="p-4">
            <div className="container">
                <Switch>
                    <Route path={`${path}`} component={Calendar} />
                </Switch>
            </div>
        </div>
    );
}

export { UserCalendar };