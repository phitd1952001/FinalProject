import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Formik, Field, Form, ErrorMessage } from "formik";
import * as Yup from "yup";
import { accountService, alertService } from "../../_services";

function Update({ history }) {
  const [user, setUser] = useState(accountService.userValue);

  useEffect(() => {
    const subscription = accountService.user.subscribe((newUser) => {
      setUser(newUser);
    });

    // Cleanup the subscription when the component unmounts
    return () => subscription.unsubscribe();
  }, []);

  const initialValues = {
    password: "",
    confirmPassword: "",
    address: user.address,
    phone: user.phone,
  };

  const validationSchema = Yup.object().shape({
    password: Yup.string().min(6, "Password must be at least 6 characters"),
    confirmPassword: Yup.string()
      .when("password", (password, schema) => {
        if (password) return schema.required("Confirm Password is required");
      })
      .oneOf([Yup.ref("password")], "Passwords must match"),
    address: Yup.string().required("Address is required"),
    phone: Yup.string().required("Phone is required"),
  });

  function onSubmit(fields, { setStatus, setSubmitting }) {
    setStatus();

    accountService
      .updateSelf(user.id, fields)
      .then(() => {
        alertService.success("Update successful", {
          keepAfterRouteChange: true,
        });
        history.push(".");
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
        {({ errors, touched, isSubmitting }) => (
          <Form>
            <h1>Update Profile</h1>
            <div className="form-row">
              <div className="form-group col-6">
                <label>Phone Number</label>
                <Field
                  name="phone"
                  type="text"
                  className={
                    "form-control" +
                    (errors.phone && touched.phone ? " is-invalid" : "")
                  }
                />
                <ErrorMessage
                  name="phone"
                  component="div"
                  className="invalid-feedback"
                />
              </div>
              <div className="form-group col-6">
                <label>Address</label>
                <Field
                  name="address"
                  type="text"
                  className={
                    "form-control" +
                    (errors.address && touched.address ? " is-invalid" : "")
                  }
                />
                <ErrorMessage
                  name="address"
                  component="div"
                  className="invalid-feedback"
                />
              </div>
            </div>
            <div className="form-group"></div>
            <h3 className="pt-3">Change Password</h3>
            <p>Leave blank to keep the same password</p>
            <div className="form-row">
              <div className="form-group col">
                <label>Password</label>
                <Field
                  name="password"
                  type="password"
                  className={
                    "form-control" +
                    (errors.password && touched.password ? " is-invalid" : "")
                  }
                />
                <ErrorMessage
                  name="password"
                  component="div"
                  className="invalid-feedback"
                />
              </div>
              <div className="form-group col">
                <label>Confirm Password</label>
                <Field
                  name="confirmPassword"
                  type="password"
                  className={
                    "form-control" +
                    (errors.confirmPassword && touched.confirmPassword
                      ? " is-invalid"
                      : "")
                  }
                />
                <ErrorMessage
                  name="confirmPassword"
                  component="div"
                  className="invalid-feedback"
                />
              </div>
            </div>
            <div className="form-group">
              <button
                type="submit"
                disabled={isSubmitting}
                className="btn btn-primary mr-2"
              >
                {isSubmitting && (
                  <span className="spinner-border spinner-border-sm mr-1"></span>
                )}
                Update
              </button>
              <Link to="/profile" className="btn btn-link">
                Cancel
              </Link>
            </div>
          </Form>
        )}
      </Formik>
    </>
  );
}

export { Update };
