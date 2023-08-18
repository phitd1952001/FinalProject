import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Formik, Field, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import images from '../../_asset/images';
import { accountService, alertService } from '../../_services';

const styles = {
    avatarContainer: {
        position: 'relative',
        width: '150px',
        height: '150px',
        borderRadius: '50%',
        overflow: 'hidden',
    },
    avatar: {
        width: '100%',
        height: '100%',
        objectFit: 'cover',
    },
    changeAvatar: {
        position: 'absolute',
        bottom: '10px',
        left: '50%',
        transform: 'translateX(-50%)',
        backgroundColor: '#007bff',
        color: '#fff',
        border: 'none',
        padding: '5px 10px',
        borderRadius: '5px',
        cursor: 'pointer',
    },
    deleteIcon: {
        position: 'absolute',
        top: '10px',
        right: '10px',
        color: '#dc3545',
        cursor: 'pointer',
        zIndex: 1,
    },
};

function Update({ history }) {
    const user = accountService.userValue;

    const initialValues = {
        title: user.title,
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
        password: '',
        confirmPassword: '',
        address: user.address,
        ccid: user.ccid,
        phone: user.phone,
        position: user.position,
        dateOfBirth: new Date(user.dateOfBirth).toISOString().substr(0, 10),
        sex: user.sex,
        managementCode: user.managementCode
    };

    const validationSchema = Yup.object().shape({
        title: Yup.string()
            .required('Title is required'),
        firstName: Yup.string()
            .required('First Name is required'),
        lastName: Yup.string()
            .required('Last Name is required'),
        email: Yup.string()
            .email('Email is invalid')
            .required('Email is required'),
        password: Yup.string()
            .min(6, 'Password must be at least 6 characters'),
        confirmPassword: Yup.string()
            .when('password', (password, schema) => {
                if (password) return schema.required('Confirm Password is required');
            })
            .oneOf([Yup.ref('password')], 'Passwords must match'),
        address: Yup.string()
            .required('Address is required'),
        ccid: Yup.string()
            .required('ccid is required'), // done
        phone: Yup.string()
            .required('Phone is required'),
        position: Yup.string()
            .required('Position is required'),
        dateOfBirth: Yup.date()
            .max(new Date(), "Date Of Birth must be in the past")
            .required('Date of birth is required'),
        sex: Yup.string()
            .oneOf(["true", "false"], "You must choose Sex")
            .required('Sex is required'), // done
        managementCode: Yup.string()
            .required('ManagementCode is required'), // done

    });

    function onSubmit(fields, { setStatus, setSubmitting }) {
        setStatus();

        accountService.update(user.id, fields)
            .then(() => {
                alertService.success('Update successful', { keepAfterRouteChange: true });
                history.push('.');
            })
            .catch(error => {
                setSubmitting(false);
                alertService.error(error);
            });
    }

    const [isDeleting, setIsDeleting] = useState(false);
    function onDelete() {
        if (confirm('Are you sure?')) {
            setIsDeleting(true);
            accountService.delete(user.id)
                .then(() => alertService.success('Account deleted successfully'));
        }
    }

    function uploadAvatar() {
        accountService.handleUpload(event.target.files[0])
            .then(() => {
                alertService.success('Upload successful', { keepAfterRouteChange: true });
            })
            .catch(error => {
                alertService.error(error);
            });
    }

    const [noAvatarImage, setNoAvatarImage] = useState(null);

    useEffect(() => {
      images.noAvatar.then((img) => setNoAvatarImage(img));
    }, []);

    return (
        <>
            <div>
                <h1>Avatar Upload</h1>
            </div>

            <div className="container my-5">
                <div style={styles.avatarContainer}>
                    {
                        user.avatar ? (
                            <img style={styles.avatar} src={user.avatar} alt="Avatar" />
                        ) : (
                            <img style={styles.avatar} src={noAvatarImage} alt="Avatar" />
                        )
                    }                   
                </div>
            </div>
            <input type="file" onChange={uploadAvatar} />

            <Formik initialValues={initialValues} validationSchema={validationSchema} onSubmit={onSubmit}>
                {({ errors, touched, isSubmitting }) => (
                    <Form>
                        <h1>Update Profile</h1>
                        <div className="form-row">
                            <div className="form-group col">
                                <label>Title</label>
                                <Field name="title" as="select" className={'form-control' + (errors.title && touched.title ? ' is-invalid' : '')}>
                                    <option value=""></option>
                                    <option value="Mr">Mr</option>
                                    <option value="Mrs">Mrs</option>
                                    <option value="Miss">Miss</option>
                                    <option value="Ms">Ms</option>
                                </Field>
                                <ErrorMessage name="title" component="div" className="invalid-feedback" />
                            </div>
                            <div className="form-group col-5">
                                <label>First Name</label>
                                <Field name="firstName" type="text" className={'form-control' + (errors.firstName && touched.firstName ? ' is-invalid' : '')} />
                                <ErrorMessage name="firstName" component="div" className="invalid-feedback" />
                            </div>
                            <div className="form-group col-5">
                                <label>Last Name</label>
                                <Field name="lastName" type="text" className={'form-control' + (errors.lastName && touched.lastName ? ' is-invalid' : '')} />
                                <ErrorMessage name="lastName" component="div" className="invalid-feedback" />
                            </div>
                        </div>
                        <div className="form-group">
                            <label>Email</label>
                            <Field name="email" type="text" className={'form-control' + (errors.email && touched.email ? ' is-invalid' : '')} />
                            <ErrorMessage name="email" component="div" className="invalid-feedback" />
                        </div>
                        <div className="form-row">
                            <div className="form-group col">
                                <label>Sex</label>
                                <Field name="sex" as="select" className={'form-control' + (errors.sex && touched.sex ? ' is-invalid' : '')}>
                                    <option value=""></option>
                                    <option value="true">Male</option>
                                    <option value="false">Female</option>
                                </Field>
                                <ErrorMessage name="sex" component="div" className="invalid-feedback" />
                            </div>
                            <div className="form-group col-5">
                                <label>CCID</label>
                                <Field name="ccid" type="text" className={'form-control' + (errors.ccid && touched.ccid ? ' is-invalid' : '')} />
                                <ErrorMessage name="ccid" component="div" className="invalid-feedback" />
                            </div>
                            <div className="form-group col-5">
                                <label>Management Code</label>
                                <Field name="managementCode" type="text" className={'form-control' + (errors.managementCode && touched.managementCode ? ' is-invalid' : '')} />
                                <ErrorMessage name="managementCode" component="div" className="invalid-feedback" />
                            </div>
                        </div>
                        <div className="form-row">
                            <div className="form-group col">
                                <label>Position</label>
                                <Field name="position" as="select" className={'form-control' + (errors.position && touched.position ? ' is-invalid' : '')}>
                                    <option value=""></option>
                                    <option value="Teacher">Teacher</option>
                                    <option value="Student">Student</option>
                                    <option value="Staff">Staff</option>
                                    <option value="Director">Director</option>
                                </Field>
                                <ErrorMessage name="position" component="div" className="invalid-feedback" />
                            </div>
                            <div className="form-group col-5">
                                <label>Phone Number</label>
                                <Field name="phone" type="text" className={'form-control' + (errors.phone && touched.phone ? ' is-invalid' : '')} />
                                <ErrorMessage name="phone" component="div" className="invalid-feedback" />
                            </div>
                            <div className="form-group col-5">
                                <label>Date of Birth</label>
                                <Field name="dateOfBirth" type="date" className={'form-control' + (errors.dateOfBirth && touched.dateOfBirth ? ' is-invalid' : '')} />
                                <ErrorMessage name="dateOfBirth" component="div" className="invalid-feedback" />
                            </div>
                        </div>
                        <div className="form-group">
                            <label>Address</label>
                            <Field name="address" type="text" className={'form-control' + (errors.address && touched.address ? ' is-invalid' : '')} />
                            <ErrorMessage name="address" component="div" className="invalid-feedback" />
                        </div>
                        <h3 className="pt-3">Change Password</h3>
                        <p>Leave blank to keep the same password</p>
                        <div className="form-row">
                            <div className="form-group col">
                                <label>Password</label>
                                <Field name="password" type="password" className={'form-control' + (errors.password && touched.password ? ' is-invalid' : '')} />
                                <ErrorMessage name="password" component="div" className="invalid-feedback" />
                            </div>
                            <div className="form-group col">
                                <label>Confirm Password</label>
                                <Field name="confirmPassword" type="password" className={'form-control' + (errors.confirmPassword && touched.confirmPassword ? ' is-invalid' : '')} />
                                <ErrorMessage name="confirmPassword" component="div" className="invalid-feedback" />
                            </div>
                        </div>
                        <div className="form-group">
                            <button type="submit" disabled={isSubmitting} className="btn btn-primary mr-2">
                                {isSubmitting && <span className="spinner-border spinner-border-sm mr-1"></span>}
                                Update
                            </button>
                            <button type="button" onClick={() => onDelete()} className="btn btn-danger" style={{ width: '75px' }} disabled={isDeleting}>
                                {isDeleting
                                    ? <span className="spinner-border spinner-border-sm"></span>
                                    : <span>Delete</span>
                                }
                            </button>
                            <Link to="." className="btn btn-link">Cancel</Link>
                        </div>
                    </Form>
                )}
            </Formik>
        </>
    )
}

export { Update };