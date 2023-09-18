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

const List = ({ match }) => {
    const { path } = match;
    const [slots, setSlots] = useState(null);
    const [openModal, setOpenModal] = useState(false);
    const [addMode, setAddMode] = useState(false);
    const [id, setId] = useState(0);
    const [openImportModal, setOpenImportModal] = useState(false);

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

    const onHide = () => {
        setAddMode(false);
        setOpenModal(false);
        setId(0);
        getSlots();
    }

    return (
        <div>
            <h1>Slot Management</h1>
            <br />
            <div className="d-flex">
                <button onClick={addSlot} className="btn btn-sm btn-success mb-2 mr-2">Add Slot</button> 
                <button onClick={()=>setOpenImportModal(true)} className="btn btn-sm btn-success mb-2">Import Excel</button>
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
                
                <Column dataField="name" caption="Name" width="15%" />
                <Column dataField="startTime" caption="Start Time" width="15%" dataType="datetime" />
                <Column dataField="duration" caption="Duration" width="15%" />
                <Column dataField="subjectName" caption="Subject Name" width="15%" />
                <Column dataField="roomName" caption="Room Name" width="15%" />
                <Column
                    width="25%"
                    caption="Actions"
                    cellRender={({ data }) => (
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
        </div>
    );
}

export { List };