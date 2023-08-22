import React from 'react';
import {SideBar, Alert} from '@/_components';

const DashBoardLayout = ({ children }) => {
    return (
        <div className='container-fluid w-100'>
            <div className='row w-100 h-100'>
                <nav className='col-2 d-block bg-dark'>
                    <div className='position-sticky'>
                        <SideBar />
                    </div>
                </nav>
                <main className='col-10 full-height'>
                    <div className='pt-3 pb-5'>
                        <Alert />
                        {children}
                    </div>
                </main>
            </div>
        </div>
    );
};

export default DashBoardLayout;