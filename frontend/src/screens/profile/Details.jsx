import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

import { accountService } from '../../_services';
import images from '../../_asset/images';

function Details({ match }) {
    const { path } = match;
    const user = accountService.userValue;

    const [noAvatarImage, setNoAvatarImage] = useState(null);

    useEffect(() => {
        if (user) {
            images.noAvatar.then((img) => setNoAvatarImage(img));
        }
    }, []);

    return (
        <>
            <div className="container" style={styles.body}>
                <div className="main-body" style={styles.mainBody}>
                    <div className="row gutters-sm" style={styles.guttersSm}>
                        <div className="col-md-8" style={styles.col}>
                            <div className="card mb-3" style={styles.card}>
                                <div className="card-body" style={styles.cardBody}>

                                    <div className="row">
                                        <div className="col-sm-3">
                                            <h6 className="mb-0">Full Name</h6>
                                        </div>
                                        <div className="col-sm-9 text-secondary">
                                            {user.title} {user.firstName} {user.lastName}
                                        </div>
                                    </div>
                                    <hr />

                                    <div className="row">
                                        <div className="col-sm-3">
                                            <h6 className="mb-0">Email</h6>
                                        </div>
                                        <div className="col-sm-9 text-secondary">
                                            {user.email}
                                        </div>
                                    </div>
                                    <hr />

                                    <div className="row">
                                        <div className="col-sm-3">
                                            <h6 className="mb-0">Sex</h6>
                                        </div>
                                        <div className="col-sm-9 text-secondary">
                                            {user.sex ? (<>Male</>): (<>Female</>)}
                                        </div>
                                    </div>
                                    <hr />

                                    <div className="row">
                                        <div className="col-sm-3">
                                            <h6 className="mb-0">CCID</h6>
                                        </div>
                                        <div className="col-sm-9 text-secondary">
                                            {user.ccid}
                                        </div>
                                    </div>
                                    <hr />

                                    <div className="row">
                                        <div className="col-sm-3">
                                            <h6 className="mb-0">Management Code</h6>
                                        </div>
                                        <div className="col-sm-9 text-secondary">
                                            {user.managementCode}
                                        </div>
                                    </div>
                                    <hr />

                                    <div className="row">
                                        <div className="col-sm-3">
                                            <h6 className="mb-0">Possition</h6>
                                        </div>
                                        <div className="col-sm-9 text-secondary">
                                            {user.position}
                                        </div>
                                    </div>
                                    <hr />

                                    <div className="row">
                                        <div className="col-sm-3">
                                            <h6 className="mb-0">Phone</h6>
                                        </div>
                                        <div className="col-sm-9 text-secondary">
                                            {user.phone}
                                        </div>
                                    </div>
                                    <hr />

                                    <div className="row">
                                        <div className="col-sm-3">
                                            <h6 className="mb-0">Date Of Birth</h6>
                                        </div>
                                        <div className="col-sm-9 text-secondary">
                                            {new Date(user.dateOfBirth).toISOString().substr(0, 10)}
                                        </div>
                                    </div>
                                    <hr />

                                    <div className="row">
                                        <div className="col-sm-3">
                                            <h6 className="mb-0">Address</h6>
                                        </div>
                                        <div className="col-sm-9 text-secondary">
                                            {user.address}
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                        <div className="col-md-4" style={styles.col}>
                            <div className="h-100 w-100 d-flex flex-column justify-content-center align-items-center">
                                <div className="card w-100" style={styles.card}>
                                    <div className="card-body" style={styles.cardBody}>
                                        <div className="d-flex flex-column align-items-center text-center">
                                            {user && user.avatar ? (
                                                <img src={user.avatar} alt="Admin" className="rounded-circle" width="150" height="150" />
                                            ) : (
                                                <img
                                                    src={noAvatarImage}
                                                    alt="" className="rounded-circle" width="150" height="150"
                                                />
                                            )}

                                            <div className="mt-3">
                                                <h4>{user.firstName} {user.lastName}</h4>
                                                <p className="text-secondary mb-1">{user.managementCode}</p>
                                                <p className="text-muted font-size-sm">{user.address}</p>
                                                <Link to={`${path}/update`} className="btn btn-primary">Update</Link>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </>

    );
}

const styles = {
    body: {
        marginTop: '20px',
        color: '#1a202c',
        textAlign: 'left',
        backgroundColor: '#e2e8f0',
        boxShadow: 'rgba(0, 0, 0.3) 0px 6px 8px -4px, rgba(0, 0.5, 0.4, 0.06) 0px 2px 4px -1px',
    },
    mainBody: {
        padding: '15px',
    },
    card: {
        boxShadow: '0 4px 6px -1px rgba(0,0,0,.1), 0 2px 4px -1px rgba(0,0,0,.06)',
        position: 'relative',
        display: 'flex',
        flexDirection: 'column',
        minWidth: '0',
        wordWrap: 'break-word',
        backgroundColor: '#fff',
        backgroundClip: 'border-box',
        border: '0 solid rgba(0,0,0,.125)',
        borderRadius: '.25rem',
    },
    cardBody: {
        flex: '1 1 auto',
        minHeight: '1px',
        padding: '1rem',
    },
    guttersSm: {
        marginRight: '-8px',
        marginLeft: '-8px',
    },
    col: {
        paddingRight: '8px',
        paddingLeft: '8px',
    },
    mb3: {
        marginBottom: '1rem',
    },
    bgGray300: {
        backgroundColor: '#e2e8f0',
    },
    h100: {
        height: '100%',
    },
    shadowNone: {
        boxShadow: 'none',
    },
};


export { Details };