import React, { useEffect, useState } from "react";
import { Formik, Field, Form, ErrorMessage } from "formik";
import * as Yup from "yup";

import { slotService, alertService, roomsService, subjectService } from "../../../_services";

function AddEdit(props) {
  const { id, onHide } = props;
  const isAddMode = id === 0;

  const initialValues = {
    name: "",
    startTime: new Date(),
    duration: 0,
    subjectId: 0,
    roomId: "",
  };

  const validationSchema = Yup.object().shape({
    name: Yup.string().required("Name is required"),
    startTime: Yup.date()
      .min(new Date(), "Start Time must be in the future")
      .required("Start Time is required"),
    duration: Yup.number().integer().min(0).required("Duration is required"),
    subjectId: Yup.number().integer().min(0).required("Subject Code is required"),
    roomId: Yup.number().integer().min(0).required("Room Name is required"),
  });

  const [rooms, setRooms]= useState([]);
  const [subjects, setSubjects]= useState([]);

  useEffect(()=>{
    getRooms();
    getSubjects();
  },[]);

  const getRooms = ()=>{
    roomsService.getAll().then(x => setRooms(x));
  }

  const getSubjects = ()=>{
    subjectService.getAll().then(x => setSubjects(x));
  }

  function onSubmit(fields, { setStatus, setSubmitting }) {
    setStatus();
    if (isAddMode) {
      createSlot(fields, setSubmitting);
    } else {
      updateSlot(id, fields, setSubmitting);
    }
  }

  function createSlot(fields, setSubmitting) {
    slotService
      .create(fields)
      .then(() => {
        alertService.success("Slot added successfully", {
          keepAfterRouteChange: true,
        });
        onHide();
      })
      .catch((error) => {
        setSubmitting(false);
        alertService.error(error);
      });
  }

  function updateSlot(id, fields, setSubmitting) {
    slotService
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
              slotService.getById(id).then((obj) => {
                const fields = ["name", "duration","startTime", "subjectId", "roomId"];
                fields.forEach((field) => {
                  if (field === "startTime") {
                    setFieldValue(field, new Date([field]), false);
                  }
                  setFieldValue(field, obj[field], false);
                });
              });
            }
          }, []);

          return (
            <Form>
              <div className="form-row">
                <label>Name</label>
                <Field
                  name="name"
                  type="text"
                  className={
                    "form-control" +
                    (errors.name && touched.name ? " is-invalid" : "")
                  }
                />
                <ErrorMessage
                  name="name"
                  component="div"
                  className="invalid-feedback"
                />
              </div>

              <div className="form-row">
                <label>Start Time</label>
                <Field
                  name="startTime"
                  type="datetime-local"
                  className={
                    "form-control" +
                    (errors.startTime && touched.startTime ? " is-invalid" : "")
                  }
                />
                <ErrorMessage
                  name="startTime"
                  component="div"
                  className="invalid-feedback"
                />
              </div>

              <div className="form-row">
                <label>Duration</label>
                <Field
                  name="duration"
                  type="number"
                  className={
                    "form-control" +
                    (errors.duration && touched.duration ? " is-invalid" : "")
                  }
                />
                <ErrorMessage
                  name="duration"
                  component="div"
                  className="invalid-feedback"
                />
              </div>

              <div className="form-row">
                <label>Subject Code</label>
                  <Field name="subjectId" as="select" className={'form-control' + (errors.subjectId && touched.subjectId ? ' is-invalid' : '')}>
                    <option value=""></option>
                    {subjects.map((obj, i)=>(
                      <option key={i} value={obj.id}>{obj.subjectCode}</option>
                    ))}
                    
                </Field>
                <ErrorMessage
                  name="subjectId"
                  component="div"
                  className="invalid-feedback"
                />
              </div>

              <div className="form-row">
                <label>Room Name</label>
                <Field name="roomId" as="select" className={'form-control' + (errors.roomId && touched.roomId ? ' is-invalid' : '')}>
                    <option value=""></option>
                    {rooms.map((obj, i)=>(
                      <option key={i} value={obj.id}>{obj.name}</option>
                    ))}
                </Field>
                <ErrorMessage
                  name="roomId"
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
