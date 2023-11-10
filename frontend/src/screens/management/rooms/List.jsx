import React, { useState, useEffect } from 'react';
import Button from 'devextreme-react/button';
import LoadPanel from 'devextreme-react/load-panel';
import { roomsService } from '../../../_services';
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
    const [rooms, setRooms] = useState(null);
    const [openModal, setOpenModal] = useState(false);
    const [addMode, setAddMode] = useState(false);
    const [id, setId] = useState(0);
    const [openImportModal, setOpenImportModal] = useState(false);

    useEffect(() => {
        getRooms();
    }, []);

    const getRooms = () => {
        roomsService.getAll().then(x => setRooms(x));
    }

    function deleteRoom(id) {
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
                setRooms(rooms.map(x => {
                    if (x.id === id) { x.isDeleting = true; }
                    return x;
                }));
                roomsService.delete(id).then(() => {
                    setRooms(rooms => rooms.filter(x => x.id !== id));
                    Swal.fire(
                        'Deleted!',
                        'Your record has been deleted.',
                        'success'
                    )
                });
            }
        })
    }

    const addRoom = () => {
        setAddMode(true);
        setOpenModal(true);
    }

    const updateRoom = (id) => {
        setId(id);
        setAddMode(false);
        setOpenModal(true);
    }

    const onHide = () => {
        setAddMode(false);
        setOpenModal(false);
        setId(0);
        getRooms();
    }

    return (
        <div>
            <h1>Room Management</h1>
            <br />
            <div className="d-flex">
                <button onClick={addRoom} className="btn btn-sm btn-success mb-2 mr-2">Add Room</button> 
                <a href="https://drive.google.com/uc?id=189GevltUrUz191AJH3N4U0dVs7aJOOrm" rel="noopener noreferrer" className="mr-2 btn btn-sm btn-warning mb-2">Excel Template</a>
                <button onClick={()=>setOpenImportModal(true)} className="btn btn-sm btn-success mb-2">Import Excel</button>
            </div>
           
            <DataGrid
                dataSource={rooms}
                showBorders={true}
                columnAutoWidth={true}
                noDataText="No rooms available"
                allowColumnResizing={true}
            >
                <HeaderFilter visible={true} />
                <Selection mode="single" />
                <GroupPanel visible={true} />
                <SearchPanel visible={true} highlightCaseSensitive={true} />
                <Grouping autoExpandAll={false} />
                <FilterRow visible={true} />

                <Column dataField="name" caption="Name" width="25%" />
                <Column dataField="numberOfSeat" caption="NumberOfSeat" width="25%" />
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
                                onClick={() => updateRoom(data.id)}
                            />
                            <Button
                                text={data.isDeleting ? "Deleting" : "Delete"}
                                type="danger"
                                disabled={data.isDeleting}
                                onClick={() => deleteRoom(data.id)}
                                width={79}
                                height={29}
                                hint="Delete Room"
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
                visible={rooms === null}
                showIndicator={true}
                shading={true}
                position={{ of: 'body' }}
            />

            <Modal title={addMode ? "Add Room" : "Update Room"} show={openModal} onHide={() => setOpenModal(false)} >
                <AddEdit onHide={onHide} id={addMode ? 0 : id} />
            </Modal>

            <Modal title={"Import Excel"} show={openImportModal} onHide={() => setOpenImportModal(false)} >
                <ExcelUpload getRooms={getRooms} setOpenImportModal={setOpenImportModal}/>
            </Modal>
        </div>
    );
}

export { List };