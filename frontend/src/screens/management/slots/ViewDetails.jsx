import React, { useState, useEffect, useRef } from "react";
import { checkInService, alertService, slotService } from "../../../_services";
import Button from "devextreme-react/button";
import { Workbook } from 'exceljs';
import { exportDataGrid } from 'devextreme/excel_exporter';
import { saveAs } from 'file-saver-es';
import DataGrid, {
  Export,
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
import Swal from 'sweetalert2';

const ViewDetails = ({ id }) => {
  const [checkInData, setCheckInData] = useState([]);

  useEffect(() => {
    getAll();
  }, [id]);

  const dataGridRef = useRef(null);

  const getAll = () => {
    // Call API to get check-in list based on slotId
    checkInService
      .getAccountInfoBySlotId(id)
      .then((res) => {
        console.log(res);
        setCheckInData(res); // Save data to state

        alertService.success("Load Info successfully", {
          keepAfterRouteChange: true,
        });
      })
      .catch((error) => {
        alertService.error(error);
      });
  };

  const deleteCheckIn = (id) => {
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
        setCheckInData(
          checkInData.map((x) => {
            if (x.id === id) {
              x.isDeleting = true;
            }
            return x;
          })
        );
        checkInService.deleteCheckIn(id).then(() => {
          setCheckInData((checkInData) =>
            checkInData.filter((x) => x.id !== id)
          );
          Swal.fire("Deleted!", "Your record has been deleted.", "success");
        });
      }
    });
  };

  const exportToExcel = () => {
    slotService.getById(id).then((res)=>{
      let name = res.name + '.xlsx'
      const workbook = new Workbook();
      const worksheet = workbook.addWorksheet('Main sheet');
  
      exportDataGrid({
        component: dataGridRef.current.instance,
        worksheet,
        autoFilterEnabled: true,
      }).then(() => {
        workbook.xlsx.writeBuffer().then((buffer) => {
          saveAs(new Blob([buffer], { type: 'application/octet-stream' }), name);
        });
      });
    }).catch((err)=>console.log(err))
    
  };
  

  return (
    <div>
      <Button
        text="Export to Excel"
        onClick={exportToExcel}
        width={180}
        icon="export"
      />
      <DataGrid
        ref={dataGridRef}
        dataSource={checkInData}
        showBorders={true}
        columnAutoWidth={true}
        noDataText="No check-in data available"
        allowColumnResizing={true}
      >
        <Export enabled={false} fileName="CheckInData" excelFilterEnabled={true} />
        <HeaderFilter visible={true} />
        <Selection mode="single" />
        <GroupPanel visible={true} />
        <SearchPanel visible={true} highlightCaseSensitive={true} />
        <Grouping autoExpandAll={false} />
        <FilterRow visible={true} />

        <Column
          caption="Full Name"
          calculateDisplayValue={(data) =>
            data.user.firstName + " " + data.user.lastName
          }
          width="15%"
        />
        <Column
          dataField="user.managementCode"
          caption="Management Code"
          width="20%"
        />
        <Column
          dataField="dateTime"
          caption="Date Time"
          width="20%"
          dataType="datetime"
          format="yyyy-MM-dd HH:mm:ss"
        />
        <Column dataField="isAccept" caption="Is Accept" width="15%" />
        <Column dataField="note" caption="Note" width="17%" />
        <Column
          width="13%"
          caption="Actions"
          cellRender={({ data }) => (
            <>
              <Button
                text={data.isDeleting ? "Deleting" : "Delete"}
                type="danger"
                disabled={data.isDeleting}
                onClick={() => deleteCheckIn(data.id)}
                width={79}
                height={29}
                hint="Delete CheckIn"
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
          <TotalItem column="user.managementCode" summaryType="count" />
        </Summary>
      </DataGrid>
    </div>
  );
};

export default ViewDetails;
