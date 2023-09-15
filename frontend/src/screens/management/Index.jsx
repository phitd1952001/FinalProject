import React from 'react';
import { Route, Switch } from 'react-router-dom';

import { Subjects } from './subjects/Index';
import { Rooms } from './rooms/Index';
import { Classes } from './classes/Index';

function Management({ match }) {
    const { path } = match;

    return (
        <div className="p-4">
            <div className="container">
                <Switch>
                    <Route path={`${path}/subjects`} component={Subjects} />
                    <Route path={`${path}/rooms`} component={Rooms} />
                    <Route path={`${path}/classes`} component={Classes} />
                </Switch>
            </div>
        </div>
    );
}

export { Management };