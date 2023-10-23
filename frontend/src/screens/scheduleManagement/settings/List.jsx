import React, { useState, useEffect } from 'react';
import Button from 'devextreme-react/button';
import LoadPanel from 'devextreme-react/load-panel';
import { settingsService } from '../../../_services';
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

const List = ({ match }) => {
    const { path } = match;
    const [settings, setSettings] = useState([]);
    const [openModal, setOpenModal] = useState(false);
    const [addMode, setAddMode] = useState(false);
    const [id, setId] = useState(0);

    useEffect(() => {
        getSettings();
    }, []);

    const getSettings = () => {
        settingsService.getAll().then(x => setSettings(x));
    }

    function deleteSetting(id) {
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
                setSettings(settings.map(x => {
                    if (x.id === id) { x.isDeleting = true; }
                    return x;
                }));
                settingsService.delete(id).then(() => {
                    setSettings(settings => settings.filter(x => x.id !== id));
                    Swal.fire(
                        'Deleted!',
                        'Your record has been deleted.',
                        'success'
                    )
                });
            }
        })
    }

    const addSetting = () => {
        setAddMode(true);
        setOpenModal(true);
    }

    const updateSetting = (id) => {
        setId(id);
        setAddMode(false);
        setOpenModal(true);
    }

    const onHide = () => {
        setAddMode(false);
        setOpenModal(false);
        setId(0);
        getSettings();
    }

    return (
        <div>
            <h1>Setting Management</h1>
            <br />
            <div className="d-flex">
               {settings.length === 0 && (<button onClick={addSetting} className="btn btn-sm btn-success mb-2 mr-2">Add Setting</button> )} 
            </div>
           
            <DataGrid
                dataSource={settings}
                showBorders={true}
                columnAutoWidth={true}
                noDataText="No Setting available"
                allowColumnResizing={true}
            >
                <HeaderFilter visible={true} />
                <Selection mode="single" />
                <GroupPanel visible={true} />
                <SearchPanel visible={true} highlightCaseSensitive={true} />
                <Grouping autoExpandAll={false} />
                <FilterRow visible={true} />

                <Column dataField="startDate" caption="Start Date" width="25%" dataType="date" />
                <Column dataField="endDate" caption="End Date" width="25%" dataType="date" />
                <Column dataField="concurrencyLevelDefault" caption="Concurrency Level" width="25%" />
                <Column dataField="internalDistance" caption="Internal Distance" width="25%" />
                <Column dataField="externalDistance" caption="External Distance" width="25%" />
                <Column dataField="noOfTimeSlot" caption="No Of Time Slot" width="25%" />
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
                                onClick={() => updateSetting(data.id)}
                            />
                            <Button
                                text={data.isDeleting ? "Deleting" : "Delete"}
                                type="danger"
                                disabled={data.isDeleting}
                                onClick={() => deleteSetting(data.id)}
                                width={79}
                                height={29}
                                hint="Delete Setting"
                            />
                        </>
                    )}
                />
                <Pager allowedPageSizes={[10, 25, 50, 100]} showPageSizeSelector={true} />
                <Paging defaultPageSize={10} />
                <Summary>
                    <TotalItem
                        column="startDate"
                        summaryType="count" />
                </Summary>
            </DataGrid>
            <LoadPanel
                shadingColor="rgba(0,0,0,0.4)"
                visible={settings === null}
                showIndicator={true}
                shading={true}
                position={{ of: 'body' }}
            />

            <Modal title={addMode ? "Add Setting" : "Update Setting"} show={openModal} onHide={() => setOpenModal(false)} >
                <AddEdit onHide={onHide} id={addMode ? 0 : id} />
            </Modal>
        </div>
    );
}

export { List };