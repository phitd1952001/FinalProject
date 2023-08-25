import React from 'react';
import { Route, Switch } from 'react-router-dom';

import { Subjects } from './subjects/Index';

function Management({ match }) {
    const { path } = match;

    return (
        <div className="p-4">
            <div className="container">
                <Switch>
                    <Route path={`${path}/subjects`} component={Subjects} />
                </Switch>
            </div>
        </div>
    );
}

export { Management };