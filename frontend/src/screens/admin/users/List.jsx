import React, { useState, useEffect } from 'react';
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
import Button from 'devextreme-react/button';
import LoadPanel from 'devextreme-react/load-panel';

import { accountService } from '../../../_services';
import { Role } from "../../../_helpers/role";
import { Modal } from '../../../_components';
import { AddEdit } from './AddEdit';
import ExcelUpload from './ExcelUpload';
import images from "../../../_asset/images";
import UploadAvatar from './UploadAvatar';

function List({ match }) {
    const { path } = match;
    const [users, setUsers] = useState(null);
    const [openModal, setOpenModal] = useState(false);
    const [addMode, setAddMode] = useState(false);
    const [selectedUser, setSelectedUser] = useState(undefined);
    const [id, setId] = useState(0);
    const [openImportModal, setOpenImportModal] = useState(false);
    const [openAvatarModal, setOpenAvatarModal] = useState(false);

    const [noAvatarImage, setNoAvatarImage] = useState(null);
    const [noQrImage, setNoQrImage] = useState(null);

    useEffect(() => {
        images.noAvatar.then((img) => setNoAvatarImage(img));
        images.noQrCode.then((img) => setNoQrImage(img));
    }, []);

    useEffect(() => {
        getUser();
    }, []);

    const getUser = () =>{
        accountService.getAll().then(x => setUsers(x));
    }

    function deleteUser(id) {
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
                setUsers(users.map(x => {
                    if (x.id === id) { x.isDeleting = true; }
                    return x;
                }));
                accountService.delete(id).then(() => {
                    setUsers(users => users.filter(x => x.id !== id));
                    Swal.fire(
                        'Deleted!',
                        'Your record has been deleted.',
                        'success'
                    )
                })
            }
        })
    }

    const addUser = () => {
        setAddMode(true);
        setOpenModal(true);
    }

    const updateUser = (id) => {
        setId(id);
        setAddMode(false);
        setOpenModal(true);
    }

    const onHide = () => {
        setAddMode(false);
        setOpenModal(false);
        setId(0);
        getUser();
    }

    const updateAvatar = (user) => {
        setSelectedUser(user);
        setOpenAvatarModal(true);
    }

    const onHideAvatar = () => {
        setOpenAvatarModal(false);
        setSelectedUser(undefined);
        getUser();
    }

    return (
        <div>
            <h1 className="text-red-400">Users Management</h1>
            <br />
            <div className="d-flex">
                <button onClick={addUser} className="btn btn-sm btn-success mb-2 mr-2">Add User</button>
                <a href="https://drive.google.com/uc?id=11mFDixqCzvrlhPlMsNlD-qQLPLaaF7Me" rel="noopener noreferrer" className="mr-2 btn btn-sm btn-warning mb-2">Excel Template</a>
                <button onClick={()=>setOpenImportModal(true)} className="btn btn-sm btn-success mb-2">Import Excel</button>
            </div>
            <DataGrid
                dataSource={users}
                showBorders={true}
                columnAutoWidth={true}
                noDataText="No users available"
                allowColumnResizing={true}
            >
                <HeaderFilter visible={true} />
                <Selection mode="single" />
                <GroupPanel visible={true} />
                <SearchPanel visible={true} highlightCaseSensitive={true} />
                <Grouping autoExpandAll={false} />
                <FilterRow visible={true} />

                <Column dataField="title" caption="Name" width="15%" />
                <Column dataField="email" caption="Email" width="30%" />
                <Column
                    caption="avatar"
                    width="20%"
                    cellRender={({ data }) => (
                        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
                            <img src={data.avatar ? data.avatar : noAvatarImage} alt="Avatar" style={{ width: '80px', height: '80px' }} />
                        </div>
                    )}
                />
                <Column
                    caption="QRcode"
                    width="20%"
                    cellRender={({ data }) => (
                        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
                            <img src={data.qrCode ? data.qrCode : noQrImage} alt="QrCode" style={{ width: '80px', height: '80px' }} />
                        </div>
                    )}
                />
                <Column
                    caption="Role"
                    width="25%"
                    cellRender={({ data }) =>
                        Object.keys(Role).find(roleName => Role[roleName] === data.role)
                    }
                />
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
                                onClick={() => updateUser(data.id)}
                            />
                            <Button
                                className="mr-1"
                                type="success"
                                width={79}
                                height={29} 
                                text={"Avatar"} 
                                onClick={() => updateAvatar(data)}
                            />
                            <Button
                                text={data.isDeleting ? "Deleting" : "Delete"}
                                type="danger"
                                disabled={data.isDeleting}
                                onClick={() => deleteUser(data.id)}
                                width={79}
                                height={29}
                                hint="Delete User"
                            />
                        </>
                    )}
                />
                <Pager allowedPageSizes={[10, 25, 50, 100]} showPageSizeSelector={true} />
                <Paging defaultPageSize={10} />
                <Summary>
                    <TotalItem
                        column="email"
                        summaryType="count" />
                </Summary>
            </DataGrid>
            <LoadPanel
                shadingColor="rgba(0,0,0,0.4)"
                visible={users === null}
                showIndicator={true}
                shading={true}
                position={{ of: 'body' }}
            />

            <Modal title={addMode ? "Add User" : "Update User"} show={openModal} onHide={() => setOpenModal(false)} >
                <AddEdit onHide={onHide} id={addMode ? 0 : id} />
            </Modal>

            <Modal title={"Import Excel"} show={openImportModal} onHide={() => setOpenImportModal(false)} >
                <ExcelUpload getAccounts={getUser} setOpenImportModal={setOpenImportModal}/>
            </Modal>

            <Modal title={"Upload Image"} show={openAvatarModal} onHide={onHideAvatar} >
                <UploadAvatar user={selectedUser} onHide={onHideAvatar}/>
            </Modal>
        </div>
    );
}

export { List };