import React from 'react';
import { Route, Switch } from 'react-router-dom';

import { CalendarScreen } from './CalendarScreen';

function Calendar({ match }) {
    const { path } = match;
    
    return (
        <Switch>
            <Route exact path={path} component={CalendarScreen} />
        </Switch>
    );
}

export { Calendar };