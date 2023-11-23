import React from 'react';
import { Route, Switch, Redirect, useLocation } from 'react-router-dom';

import { Role } from '@/_helpers';
import { PrivateRoute } from '@/_components';
import { Home } from '@/screens/home';
import { Profile } from '@/screens/profile';
import { Admin } from '@/screens/admin';
import { Account } from '@/screens/account';
import { Management } from '@/screens/management';
import { DashboardIndex } from '../screens/dashboard/Index';
import { Schedule } from '../screens/scheduleManagement/Index';
import { UserCalendar } from '../screens/userCalendar/Index';
import Chat from '../screens/chat/Chat';

import ThemeProvider from '../theme';

function App() {
    const { pathname } = useLocation();  

    return (
        <ThemeProvider>
            <Switch>
                <Redirect from="/:url*(/+)" to={pathname.slice(0, -1)} />
                <PrivateRoute exact path="/" component={Home} />
                <PrivateRoute path="/profile" component={Profile} />
                <PrivateRoute path="/dashboard" component={DashboardIndex} />
                <PrivateRoute path="/admin" roles={[Role.Admin]} component={Admin} />
                <PrivateRoute path="/management" roles={[Role.Admin, Role.Staff, Role.Supervisor]} component={Management} />
                <PrivateRoute path="/schedule" roles={[Role.Admin, Role.Staff, Role.Supervisor]} component={Schedule} />
                <PrivateRoute path="/calendar" component={UserCalendar} />
                <PrivateRoute path="/chat" component={Chat} />
                <Route path="/account" component={Account} />
                <Redirect from="*" to="/" />
            </Switch>
        </ThemeProvider>
    );
}

export { App }; 