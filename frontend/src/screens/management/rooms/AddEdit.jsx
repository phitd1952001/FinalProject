import React, { useEffect } from 'react';
import { Formik, Field, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';

import { roomsService, alertService } from '../../../_services';

function AddEdit(props) {
  const { id, onHide } = props;
  const isAddMode = id === 0;

  const initialValues = {
    name: '',
    numberOfSeat: 0,
  };

  const validationSchema = Yup.object().shape({
    name: Yup.string()
      .required('Name is required'),
    numberOfSeat: Yup.number().integer().min(0)
      .required('Number Of Seat is required'),
  });

  function onSubmit(fields, { setStatus, setSubmitting }) {
    setStatus();
    if (isAddMode) {
      createRoom(fields, setSubmitting);
    } else {
        updateRoom(id, fields, setSubmitting);
    }
  }

  function createRoom(fields, setSubmitting) {
    roomsService.create(fields)
      .then(() => {
        alertService.success('Room added successfully', { keepAfterRouteChange: true });
        onHide();
      })
      .catch(error => {
        setSubmitting(false);
        alertService.error(error);
      });
  }

  function updateRoom(id, fields, setSubmitting) {
    roomsService.update(id, fields)
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
              // get room and set form fields
              roomsService.getById(id).then(obj => {
                const fields = ['name', 'numberOfSeat'];
                fields.forEach(field => {
                  setFieldValue(field, obj[field], false)
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
                <label>Number of seats</label>
                <Field name="numberOfSeat" type="text" className={'form-control' + (errors.numberOfSeat && touched.numberOfSeat ? ' is-invalid' : '')} />
                <ErrorMessage name="numberOfSeat" component="div" className="invalid-feedback" />
              </div>

              <div className="form-group d-flex justify-content-center mt-3">
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