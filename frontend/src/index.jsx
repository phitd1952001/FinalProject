import React from 'react';
import { Router } from 'react-router-dom';
import { render } from 'react-dom';

import { history } from './_helpers';
import { accountService } from './_services';
import { App } from './app/Index';
import { Provider } from "react-redux";
import { store } from './redux';

import './styles.less';
import 'devextreme/dist/css/dx.common.css';
import 'devextreme/dist/css/dx.light.css'; 

// attempt silent token refresh before startup
accountService.refreshToken().finally(startApp);

function startApp() { 
    render(
        <Provider store={store}>
            <Router history={history}>
                <App />
            </Router>
        </Provider>,
        document.getElementById('app')
    );
}