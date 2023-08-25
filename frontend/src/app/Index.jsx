import React from 'react';
import { Route, Switch, Redirect, useLocation } from 'react-router-dom';

import { Role } from '@/_helpers';
import { PrivateRoute } from '@/_components';
import { Home } from '@/screens/home';
import { Profile } from '@/screens/profile';
import { Admin } from '@/screens/admin';
import { Account } from '@/screens/account';
import { Management } from '@/screens/management';

function App() {
    const { pathname } = useLocation();  

    return (
        <div>
            <Switch>
                <Redirect from="/:url*(/+)" to={pathname.slice(0, -1)} />
                <PrivateRoute exact path="/" component={Home} />
                <PrivateRoute path="/profile" component={Profile} />
                <PrivateRoute path="/admin" roles={[Role.Admin]} component={Admin} />
                <PrivateRoute path="/management" roles={[Role.Admin, Role.Staff]} component={Management} />
                <Route path="/account" component={Account} />
                <Redirect from="*" to="/" />
            </Switch>
        </div>
    );
}

export { App }; 