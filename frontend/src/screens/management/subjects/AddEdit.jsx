import React, { useEffect } from 'react';
import { Formik, Field, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';

import { subjectService, alertService } from '../../../_services';

function AddEdit(props) {
  const { id, onHide } = props;
  const isAddMode = id === 0;

  const initialValues = {
    name: '',
    subjectCode: '',
    description: '',
  };

  const validationSchema = Yup.object().shape({
    name: Yup.string()
      .required('Name is required'),
    subjectCode: Yup.string()
      .required('Subject Code is required'),
  });

  function onSubmit(fields, { setStatus, setSubmitting }) {
    setStatus();
    if (isAddMode) {
      createSubject(fields, setSubmitting);
    } else {
      updateSuject(id, fields, setSubmitting);
    }
  }

  function createSubject(fields, setSubmitting) {
    subjectService.create(fields)
      .then(() => {
        alertService.success('Subject added successfully', { keepAfterRouteChange: true });
        onHide();
      })
      .catch(error => {
        setSubmitting(false);
        alertService.error(error);
      });
  }

  function updateSuject(id, fields, setSubmitting) {
    subjectService.update(id, fields)
      .then(() => {
        alertService.success('Update successful', { keepAfterRouteChange: true });
        onHide();
      })
      .catch(error => {
        setSubmitting(false);
        alertService.error(error);
      });
  }

  return (
    <>
      <Formik initialValues={initialValues} validationSchema={validationSchema} onSubmit={onSubmit}>
        {({ errors, touched, isSubmitting, setFieldValue }) => {
          useEffect(() => {
            if (!isAddMode) {
              // get user and set form fields
              subjectService.getById(id).then(subject => {
                const fields = ['name', 'subjectCode', 'description'];
                fields.forEach(field => {
                  setFieldValue(field, subject[field], false)
                });
              });
            }
          }, []);

          return (
            <Form>
              <div className="form-row">
                <label>Name</label>
                <Field name="name" type="text" className={'form-control' + (errors.name && touched.name ? ' is-invalid' : '')} />
                <ErrorMessage name="name" component="div" className="invalid-feedback" />
              </div>
              
              <div className="form-row">
                <label>Subject Code</label>
                <Field name="subjectCode" type="text" className={'form-control' + (errors.subjectCode && touched.subjectCode ? ' is-invalid' : '')} />
                <ErrorMessage name="subjectCode" component="div" className="invalid-feedback" />
              </div>

              <div className="form-row">
                <label>Description</label>
                <Field name="description" type="text" className={'form-control' + (errors.description && touched.description ? ' is-invalid' : '')} />
                <ErrorMessage name="description" component="div" className="invalid-feedback" />
              </div>

              <div className="form-group d-flex justify-content-center">
                <button type="submit" disabled={isSubmitting} className="btn btn-primary" style={{ width: "25%" }}>
                  {isSubmitting && <span className="spinner-border spinner-border-sm mr-1"></span>}
                  Save
                </button>
              </div>
            </Form>
          );
        }}
      </Formik>
    </>
  );
}

export { AddEdit };