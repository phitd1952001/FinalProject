import React, { useState, useEffect } from 'react';
import Button from 'devextreme-react/button';
import LoadPanel from 'devextreme-react/load-panel';
import { slotService } from '../../../_services';
import Swal from 'sweetalert2';
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
    FilterRow
} from 'devextreme-react/data-grid';
import { AddEdit } from './AddEdit';
import { Modal } from '../../../_components';
import ExcelUpload from './ExcelUpload';
import CheckIn from './CheckIn';
import ViewDetails from './ViewDetails';
import { accountService } from "../../../_services";
import {Role} from '../../../_helpers';

const List = ({ match }) => {
    const { path } = match;
    const [slots, setSlots] = useState(null);
    const [openModal, setOpenModal] = useState(false);
    const [addMode, setAddMode] = useState(false);
    const [id, setId] = useState(0);
    const [openImportModal, setOpenImportModal] = useState(false);
    const [openCheckInModal, setOpenCheckInModal] = useState(false);
    const [openViewDetailsModal, setOpenViewDetailsModal] = useState(false);
    const user = accountService.userValue;

    useEffect(() => {
        getSlots();
    }, []);

    const getSlots = () => {
        slotService.getAll().then(x => setSlots(x));
    }

    function deleteSlot(id) {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                setSlots(slots.map(x => {
                    if (x.slotId === id) { x.isDeleting = true; }
                    return x;
                }));
                slotService.delete(id).then(() => {
                    setSlots(slots => slots.filter(x => x.slotId !== id));
                    Swal.fire(
                        'Deleted!',
                        'Your record has been deleted.',
                        'success'
                    )
                });
            }
        })
    }

    const addSlot = () => {
        setAddMode(true);
        setOpenModal(true);
    }

    const updateSlot= (id) => {
        setId(id);
        setAddMode(false);
        setOpenModal(true);
    }

    // Slot
    const onHide = () => {
        setAddMode(false);
        setOpenModal(false);
        setId(0);
        getSlots();
    }

    // CheckIn
    const onOpenCheckIn = (id) => {
        setId(id);
        setOpenCheckInModal(true);
    }

    const onHideCheckIn = () => {
        setId(0);
        setOpenCheckInModal(false);
    }

    // view details
    const onOpenViewDetail = (id) => {
        setId(id);
        setOpenViewDetailsModal(true);
    }

    const onHideViewDetail = () => {
        setId(0);
        setOpenViewDetailsModal(false);
    }


    return (
        <div>
            <h1>Slot Management</h1>
            <br />
            <div className="d-flex">
            {(user.role === Role.Admin || user.role == Role.Staff) && (
                <>
                <button onClick={addSlot} className="btn btn-sm btn-success mb-2 mr-2">Add Slot</button> 
                <a href="https://drive.google.com/uc?id=1MbSHZgDSihLW-2dwy3QmuGYgS1yasoHS" rel="noopener noreferrer" className="mr-2 btn btn-sm btn-warning mb-2">Excel Template</a>
                <button onClick={()=>setOpenImportModal(true)} className="btn btn-sm btn-success mb-2">Import Excel</button>
                </>
            )}
            </div>
           
            <DataGrid
                dataSource={slots}
                showBorders={true}
                columnAutoWidth={true}
                noDataText="No Slots available"
                allowColumnResizing={true}
            >
                <HeaderFilter visible={true} />
                <Selection mode="single" />
                <GroupPanel visible={true} />
                <SearchPanel visible={true} highlightCaseSensitive={true} />
                <Grouping autoExpandAll={false} />
                <FilterRow visible={true} />
                
                <Column dataField="name" caption="Name" width="8%" />
                <Column dataField="startTime" caption="Start Time" width="15%" dataType="datetime" />
                <Column dataField="duration" caption="Duration" width="8%" />
                <Column dataField="subjectName" caption="Subject Name" width="22%" />
                <Column dataField="roomName" caption="Room Name" width="10%" />
                <Column
                    width="20%"
                    caption="Actions"
                    cellRender={({ data }) => (
                        <>
                            <Button
                                className="mr-1"
                                type="success"
                                width={100}
                                height={29}
                                text={"CheckIn"}
                                onClick={() => onOpenCheckIn(data.slotId)}
                            />
                             <Button
                                className="mr-1"
                                type="normal"
                                width={150}
                                height={29}
                                text={"View Details"}
                                onClick={() => onOpenViewDetail(data.slotId)}
                            />
                            {(user.role === Role.Admin || user.role == Role.Staff) && (
                                <>
                                <Button
                                className="mr-1"
                                type="default"
                                width={79}
                                height={29}
                                text={"Edit"}
                                onClick={() => updateSlot(data.slotId)}
                            />
                            <Button
                                text={data.isDeleting ? "Deleting" : "Delete"}
                                type="danger"
                                disabled={data.isDeleting}
                                onClick={() => deleteSlot(data.slotId)}
                                width={79}
                                height={29}
                                hint="Delete Slot"
                            />
                                </>
                            )}
                        </>
                    )}
                />
                <Pager allowedPageSizes={[10, 25, 50, 100]} showPageSizeSelector={true} />
                <Paging defaultPageSize={10} />
                <Summary>
                    <TotalItem
                        column="name"
                        summaryType="count" />
                </Summary>
            </DataGrid>
            <LoadPanel
                shadingColor="rgba(0,0,0,0.4)"
                visible={slots === null}
                showIndicator={true}
                shading={true}
                position={{ of: 'body' }}
            />

            <Modal title={addMode ? "Add Slot" : "Update Slot"} show={openModal} onHide={() => setOpenModal(false)} >
                <AddEdit onHide={onHide} id={addMode ? 0 : id} />
            </Modal>

            <Modal title={"Import Excel"} show={openImportModal} onHide={() => setOpenImportModal(false)} >
                <ExcelUpload getSlots={getSlots} setOpenImportModal={setOpenImportModal}/>
            </Modal>

            <Modal title={"CheckIn"} show={openCheckInModal} onHide={() => onHideCheckIn()} >
                <CheckIn id={id}/>
            </Modal>

            <Modal title={"View Details"} show={openViewDetailsModal} onHide={() => onHideViewDetail()} >
                <ViewDetails id={id} />
            </Modal>
        </div>
    );
}

export { List };