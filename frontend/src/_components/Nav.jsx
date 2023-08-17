import React, { useState, useEffect } from 'react';
import { NavLink, Route } from 'react-router-dom';

import { Role } from '../_helpers';
import { accountService } from '../_services';
import images from '../_asset/images';


const styles = {
    avatar: {
        borderRadius: '50%',
        overflow: 'hidden',
    },
};

function Nav() {
    const [user, setUser] = useState({});
    const [noAvatarImage, setNoAvatarImage] = useState(null);

    useEffect(() => {
        if (user) {
            images.noAvatar.then((img) => setNoAvatarImage(img));
        }
        const subscription = accountService.user.subscribe(x => setUser(x));
        return subscription.unsubscribe;
    }, []);

    // only show nav when logged in
    if (!user) return null;

    return (
        <div>
            <nav className="navbar navbar-expand navbar-dark bg-dark">

                <div className="navbar-nav">
                    <NavLink exact to="/" className="nav-item nav-link">Home</NavLink>
                    {user.role === Role.Admin &&
                        <NavLink to="/admin" className="nav-item nav-link">Admin</NavLink>
                    }
                </div>

                <div className="collapse navbar-collapse justify-content-end" id="navbarNav">
                    <ul className="navbar-nav">
                        <li className="nav-item dropdown">
                            <a className="nav-link dropdown-toggle dflex" href="#" id="avatarDropdown" role="button" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">

                                {
                                    user && user.avatar ? (
                                        <img style={styles.avatar} src={user.avatar} alt="Avatar" class="avatar-img" width="30" height="30" />
                                    ) : (
                                        <img src={noAvatarImage} alt="Avatar" class="avatar-img" width="30" height="30" />
                                    )
                                }
                                {
                                    user && user.firstName ? (<span>Hi {user.firstName}!</span>) : (<>Hi!</>)
                                }
                            </a>
                            <div className="dropdown-menu dropdown-menu-end" aria-labelledby="avatarDropdown">
                                <NavLink to="/profile" className="dropdown-item">Profile</NavLink>
                                <div className="dropdown-divider"></div>
                                <a onClick={accountService.logout} className="dropdown-item">Logout</a>
                            </div>
                        </li>
                    </ul>
                </div>

            </nav>
            <Route path="/admin" component={AdminNav} />
        </div>
    );
}

function AdminNav({ match }) {
    const { path } = match;

    return (
        <nav className="admin-nav navbar navbar-expand navbar-light">
            <div className="navbar-nav">
                <NavLink to={`${path}/users`} className="nav-item nav-link">Users</NavLink>
            </div>
        </nav>
    );
}

export { Nav }; 