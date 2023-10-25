import React, { useState, useEffect } from "react";
import Button from "devextreme-react/button";
import LoadPanel from "devextreme-react/load-panel";
import { classService } from "../../../_services";
import Swal from "sweetalert2";
import DataGrid, {
  Column,
  Grouping,
  GroupPanel,
  Pager,
  Paging,
  SearchPanel,
  Selection,
  Summary,
  TotalItem,
  HeaderFilter,
  FilterRow,
} from "devextreme-react/data-grid";
import { AddEdit } from "./AddEdit";
import { Modal } from "../../../_components";
import ExcelUpload from "./ExcelUpload";

const List = ({ match }) => {
  const { path } = match;
  const [classes, setClasses] = useState(null);
  const [openModal, setOpenModal] = useState(false);
  const [addMode, setAddMode] = useState(false);
  const [id, setId] = useState(0);
  const [openImportModal, setOpenImportModal] = useState(false);

  useEffect(() => {
    getClass();
  }, []);

  const getClass = () => {
    classService.getAll().then((x) => setClasses(x));
  };

  function deleteClass(id) {
    Swal.fire({
      title: "Are you sure?",
      text: "You won't be able to revert this!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!",
    }).then((result) => {
      if (result.isConfirmed) {
        setClasses(
          classes.map((x) => {
            if (x.classId === id) {
              x.isDeleting = true;
            }
            return x;
          })
        );
        classService.delete(id).then(() => {
          setClasses((classes) => classes.filter((x) => x.classId !== id));
          Swal.fire("Deleted!", "Your record has been deleted.", "success");
        });
      }
    });
  }

  const addClass = () => {
    setAddMode(true);
    setOpenModal(true);
  };

  const updateClass = (id) => {
    setId(id);
    setAddMode(false);
    setOpenModal(true);
  };

  const onHide = () => {
    setAddMode(false);
    setOpenModal(false);
    setId(0);
    getClass();
  };

  return (
    <div>
      <h1>Class Management</h1>
      <br />
      <div className="d-flex">
        <button onClick={addClass} className="btn btn-sm btn-success mb-2 mr-2">
          Add Class
        </button>
        <button
          onClick={() => setOpenImportModal(true)}
          className="btn btn-sm btn-success mb-2"
        >
          Import Excel
        </button>
      </div>

      <DataGrid
        dataSource={classes}
        showBorders={true}
        columnAutoWidth={true}
        noDataText="No classes available"
        allowColumnResizing={true}
      >
        <HeaderFilter visible={true} />
        <Selection mode="single" />
        <GroupPanel visible={true} />
        <SearchPanel visible={true} highlightCaseSensitive={true} />
        <Grouping autoExpandAll={false} />
        <FilterRow visible={true} />

        <Column dataField="subjectCode" caption="Subject Code" width="25%" />
        <Column dataField="email" caption="Email" width="25%" />
        <Column
          width="30%"
          caption="Actions"
          cellRender={({ data }) => (
            <>
              <Button
                className="mr-1"
                type="default"
                width={79}
                height={29}
                text={"Edit"}
                onClick={() => updateClass(data.classId)}
              />
              <Button
                text={data.isDeleting ? "Deleting" : "Delete"}
                type="danger"
                disabled={data.isDeleting}
                onClick={() => deleteClass(data.classId)}
                width={79}
                height={29}
                hint="Delete Class"
              />
            </>
          )}
        />
        <Pager
          allowedPageSizes={[10, 25, 50, 100]}
          showPageSizeSelector={true}
        />
        <Paging defaultPageSize={10} />
        <Summary>
          <TotalItem column="subjectCode" summaryType="count" />
        </Summary>
      </DataGrid>
      <LoadPanel
        shadingColor="rgba(0,0,0,0.4)"
        visible={classes === null}
        showIndicator={true}
        shading={true}
        position={{ of: "body" }}
      />

      <Modal
        title={addMode ? "Add Class" : "Update Class"}
        show={openModal}
        onHide={() => setOpenModal(false)}
      >
        <AddEdit onHide={onHide} id={addMode ? 0 : id} />
      </Modal>

      <Modal
        title={"Import Excel"}
        show={openImportModal}
        onHide={() => setOpenImportModal(false)}
      >
        <ExcelUpload
          getClass={getClass}
          setOpenImportModal={setOpenImportModal}
        />
      </Modal>
    </div>
  );
};

export { List };