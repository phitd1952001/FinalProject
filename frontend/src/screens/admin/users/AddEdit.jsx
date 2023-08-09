import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Formik, Field, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';

import { accountService, alertService } from '../../../_services';

function AddEdit({ history, match }) {
    const { id } = match.params;
    const isAddMode = !id;

    const updateinitialValues = {
        title: '',
        firstName: '',
        lastName: '',
        email: '',
        role: 0,
        password: '',
        confirmPassword: '',
        address: '',
        ccid: '',
        phone: '',
        position: '',
        dateOfBirth: new Date().toISOString().substr(0, 10),
        sex: '',
        managementCode: '',
    };

    const createinitialValues = {
        title: '',
        firstName: '',
        lastName: '',
        email: '',
        role: 3,
        password: '',
        confirmPassword: ''
    };

    const updateValidationSchema = Yup.object().shape({
        title: Yup.string()
            .required('Title is required'),
        firstName: Yup.string()
            .required('First Name is required'),
        lastName: Yup.string()
            .required('Last Name is required'),
        email: Yup.string()
            .email('Email is invalid')
            .required('Email is required'),
        role: Yup.number()
            .required('Role is required'),
        password: Yup.string()
            .concat(isAddMode ? Yup.string().required('Password is required') : null)
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
        sex: Yup.boolean()
            .oneOf([true], "You must choose Sex")
            .required('Sex is required'), // done
        managementCode: Yup.string()
            .required('ManagementCode is required'), // done
    });

    const createValidationSchema = Yup.object().shape({
        title: Yup.string()
            .required('Title is required'),
        firstName: Yup.string()
            .required('First Name is required'),
        lastName: Yup.string()
            .required('Last Name is required'),
        email: Yup.string()
            .email('Email is invalid')
            .required('Email is required'),
        role: Yup.number()
            .required('Role is required'),
        password: Yup.string()
            .concat(isAddMode ? Yup.string().required('Password is required') : null)
            .min(6, 'Password must be at least 6 characters'),
        confirmPassword: Yup.string()
            .when('password', (password, schema) => {
                if (password) return schema.required('Confirm Password is required');
            })
            .oneOf([Yup.ref('password')], 'Passwords must match')
    });

    function onSubmit(fields, { setStatus, setSubmitting }) {
        setStatus();
        fields.role = parseInt(fields.role);
        if (isAddMode) {
            createUser(fields, setSubmitting);
        } else {
            updateUser(id, fields, setSubmitting);
        }
    }

    function createUser(fields, setSubmitting) {
        accountService.create(fields)
            .then(() => {
                alertService.success('User added successfully', { keepAfterRouteChange: true });
                history.push('.');
            })
            .catch(error => {
                setSubmitting(false);
                alertService.error(error);
            });
    }

    function updateUser(id, fields, setSubmitting) {
        accountService.update(id, fields)
            .then(() => {
                alertService.success('Update successful', { keepAfterRouteChange: true });
                history.push('..');
            })
            .catch(error => {
                setSubmitting(false);
                alertService.error(error);
            });
    }

    return (
        <>
            <Formik initialValues={isAddMode ? createinitialValues : updateinitialValues} validationSchema={isAddMode ? createValidationSchema : updateValidationSchema} onSubmit={onSubmit}>
                {({ errors, touched, isSubmitting, setFieldValue }) => {
                    useEffect(() => {
                        if (!isAddMode) {
                            // get user and set form fields
                            accountService.getById(id).then(user => {
                                const fields = ['title', 'firstName', 'lastName', 'email', 'role', 'address', 'ccid', 'phone', 'position', 'dateOfBirth', 'sex', 'managementCode'];
                                fields.forEach(field => {
                                    if (field === 'dateOfBirth') {
                                        setFieldValue(field, new Date(user[field]).toISOString().substr(0, 10), false)
                                    }
                                    else {
                                        setFieldValue(field, user[field], false)
                                    }
                                });
                            });
                        }
                    }, []);

                    return (
                        <Form>
                            <h1>{isAddMode ? 'Add User' : 'Edit User'}</h1>
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
                            <div className="form-row">
                                <div className="form-group col-7">
                                    <label>Email</label>
                                    <Field name="email" type="text" className={'form-control' + (errors.email && touched.email ? ' is-invalid' : '')} />
                                    <ErrorMessage name="email" component="div" className="invalid-feedback" />
                                </div>
                                <div className="form-group col-5">
                                    <label>Role</label>
                                    <Field name="role" as="select" className={'form-control' + (errors.role && touched.role ? ' is-invalid' : '')}>
                                        <option value=""></option>
                                        <option value={3}>User</option>
                                        <option value={0}>Admin</option>
                                        <option value={2}>Supervisor</option>
                                        <option value={1}>Staff</option>
                                    </Field>
                                    <ErrorMessage name="role" component="div" className="invalid-feedback" />
                                </div>
                            </div>
                            {!isAddMode &&
                                <div>
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
                                    <div>
                                        <h3 className="pt-3">Change Password</h3>
                                        <p>Leave blank to keep the same password</p>
                                    </div>
                                </div>
                            }
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
                                <button type="submit" disabled={isSubmitting} className="btn btn-primary">
                                    {isSubmitting && <span className="spinner-border spinner-border-sm mr-1"></span>}
                                    Save
                                </button>
                                <Link to={isAddMode ? '.' : '..'} className="btn btn-link">Cancel</Link>
                            </div>
                        </Form>
                    );
                }}
            </Formik>
        </>
    );
}

export { AddEdit };