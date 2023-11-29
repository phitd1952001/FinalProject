import React, { useEffect } from "react";
import { Formik, Field, Form, ErrorMessage } from "formik";
import * as Yup from "yup";

import { classService, alertService } from "../../../_services";

function AddEdit(props) {
  const { id, onHide } = props;
  const isAddMode = id === 0;

  const initialValues = {
    subjectCode: "",
    email: "",
  };

  const validationSchema = Yup.object().shape({
    subjectCode: Yup.string().required("Subject Code is required"),
    email: Yup.string().email("Email is invalid").required("Email is required"),
  });

  function onSubmit(fields, { setStatus, setSubmitting }) {
    setStatus();
    if (isAddMode) {
      createClass(fields, setSubmitting);
    } else {
      updateClass(id, fields, setSubmitting);
    }
  }

  function createClass(fields, setSubmitting) {
    classService
      .create(fields)
      .then(() => {
        alertService.success("Class added successfully", {
          keepAfterRouteChange: true,
        });
        onHide();
      })
      .catch((error) => {
        setSubmitting(false);
        alertService.error(error);
      });
  }

  function updateClass(id, fields, setSubmitting) {
    classService
      .update(id, fields)
      .then(() => {
        alertService.success("Update successful", {
          keepAfterRouteChange: true,
        });
        onHide();
      })
      .catch((error) => {
        setSubmitting(false);
        alertService.error(error);
      });
  }

  return (
    <>
      <Formik
        initialValues={initialValues}
        validationSchema={validationSchema}
        onSubmit={onSubmit}
      >
        {({ errors, touched, isSubmitting, setFieldValue }) => {
          useEffect(() => {
            if (!isAddMode) {
              // get class and set form fields
              classService.getById(id).then((obj) => {
                const fields = ["subjectCode", "email"];
                fields.forEach((field) => {
                  setFieldValue(field, obj[field], false);
                });
              });
            }
          }, []);

          return (
            <Form>
              <div className="form-row">
                <label>Subject Code</label>
                <Field
                  name="subjectCode"
                  type="text"
                  className={
                    "form-control" +
                    (errors.subjectCode && touched.subjectCode ? " is-invalid" : "")
                  }
                />
                <ErrorMessage
                  name="subjectCode"
                  component="div"
                  className="invalid-feedback"
                />
              </div>

              <div className="form-row">
                <label>Email</label>
                <Field
                  name="email"
                  type="text"
                  className={
                    "form-control" +
                    (errors.email && touched.email
                      ? " is-invalid"
                      : "")
                  }
                />
                <ErrorMessage
                  name="email"
                  component="div"
                  className="invalid-feedback"
                />
              </div>

              <div className="form-group d-flex justify-content-center mt-3">
                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="btn btn-primary"
                  style={{ width: "25%" }}
                >
                  {isSubmitting && (
                    <span className="spinner-border spinner-border-sm mr-1"></span>
                  )}
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
