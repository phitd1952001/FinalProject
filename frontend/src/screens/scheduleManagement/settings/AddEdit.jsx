import React, { useEffect } from 'react';
import { Formik, Field, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';

import { settingsService, alertService } from '../../../_services';
import { DateTime } from 'luxon';

function AddEdit(props) {
  const { id, onHide } = props;
  const isAddMode = id === 0;

  const initialValues = {
    startDate: new Date(),
    endDate: new Date(),
    concurrencyLevelDefault: 0,
    internalDistance: 0,
    externalDistance: 0,
    noOfTimeSlot: 0,
    noOfSlotAllowOneStudentInDay: 0
  };

  const validationSchema = Yup.object().shape({
    startDate: Yup.date()
      .min(new Date(), "Start Date must be in the future")
      .required("Start Date is required"),
    endDate: Yup.date()
      .min(new Date(), "End Date must be in the future")
      .required("End Date is required"),
    concurrencyLevelDefault: Yup.number().integer().min(0)
      .required('Concurrency Level Default is required'),
    internalDistance: Yup.number().integer().min(0)
      .required('Internal Distance is required'),
    externalDistance: Yup.number().integer().min(0)
      .required('External Distance is required'),
    noOfTimeSlot: Yup.number().integer().min(0)
      .required('No Of Time Slot is required'),
    noOfSlotAllowOneStudentInDay: Yup.number().integer().min(0)
      .required('No Of Slot Allow One Student In Day is required'),
  });

  function onSubmit(fields, { setStatus, setSubmitting }) {
    setStatus();
    if (isAddMode) {
      createSetting(fields, setSubmitting);
    } else {
      updateSetting(id, fields, setSubmitting);
    }
  }

  function createSetting(fields, setSubmitting) {
    settingsService.create(fields)
      .then(() => {
        alertService.success('Setting added successfully', { keepAfterRouteChange: true });
        onHide();
      })
      .catch(error => {
        setSubmitting(false);
        alertService.error(error);
      });
  }

  function updateSetting(id, fields, setSubmitting) {
    settingsService.update(id, fields)
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
              // get setting and set form fields
              settingsService.getById(id).then(obj => {
                const fields = ['startDate', 'endDate', 'concurrencyLevelDefault', 'internalDistance','externalDistance','noOfTimeSlot', 'noOfSlotAllowOneStudentInDay'];
                fields.forEach(field => {
                  if (field === 'startDate' || field === 'endDate') {
                    //const vietnamTime = DateTime.fromISO(obj[field], { zone: 'Asia/Ho_Chi_Minh' });
                    setFieldValue(field,new Date(obj[field]).toISOString().substr(0, 10), false);
                  }else{
                    setFieldValue(field, obj[field], false);
                  }
                });
              });
            }
          }, []);

          return (
            <Form>
              <div className="form-row">
                <label>Start Date</label>
                <Field
                  name="startDate"
                  type="date"
                  className={
                    "form-control" +
                    (errors.startDate && touched.startDate ? " is-invalid" : "")
                  }
                />
                <ErrorMessage
                  name="startDate"
                  component="div"
                  className="invalid-feedback"
                />
              </div>

              <div className="form-row">
                <label>End Date</label>
                <Field
                  name="endDate"
                  type="date"
                  className={
                    "form-control" +
                    (errors.endDate && touched.endDate ? " is-invalid" : "")
                  }
                />
                <ErrorMessage
                  name="endDate"
                  component="div"
                  className="invalid-feedback"
                />
              </div>
              
              <div className="form-row">
                <label>Concurrency Level</label>
                <Field name="concurrencyLevelDefault" type="text" className={'form-control' + (errors.concurrencyLevelDefault && touched.concurrencyLevelDefault ? ' is-invalid' : '')} />
                <ErrorMessage name="concurrencyLevelDefault" component="div" className="invalid-feedback" />
              </div>

              <div className="form-row">
                <label>Internal Distance</label>
                <Field name="internalDistance" type="text" className={'form-control' + (errors.internalDistance && touched.internalDistance ? ' is-invalid' : '')} />
                <ErrorMessage name="internalDistance" component="div" className="invalid-feedback" />
              </div>


              <div className="form-row">
                <label>External Distance</label>
                <Field name="externalDistance" type="text" className={'form-control' + (errors.externalDistance && touched.externalDistance ? ' is-invalid' : '')} />
                <ErrorMessage name="externalDistance" component="div" className="invalid-feedback" />
              </div>

              <div className="form-row">
                <label>No Of Time Slot</label>
                <Field name="noOfTimeSlot" type="text" className={'form-control' + (errors.noOfTimeSlot && touched.noOfTimeSlot ? ' is-invalid' : '')} />
                <ErrorMessage name="noOfTimeSlot" component="div" className="invalid-feedback" />
              </div>

              <div className="form-row">
                <label>No Of Slot Allow One Student In Day</label>
                <Field name="noOfSlotAllowOneStudentInDay" type="text" className={'form-control' + (errors.noOfSlotAllowOneStudentInDay && touched.noOfSlotAllowOneStudentInDay ? ' is-invalid' : '')} />
                <ErrorMessage name="noOfSlotAllowOneStudentInDay" component="div" className="invalid-feedback" />
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