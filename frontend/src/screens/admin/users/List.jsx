import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

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

function List({ match }) {
    const { path } = match;
    const [users, setUsers] = useState(null);

    useEffect(() => {
        accountService.getAll().then(x => setUsers(x));
    }, []);

    function deleteUser(id) {
        if (confirm('Do You Want to delete')) {
            setUsers(users.map(x => {
                if (x.id === id) { x.isDeleting = true; }
                return x;
            }));
            accountService.delete(id).then(() => {
                setUsers(users => users.filter(x => x.id !== id));
            });
        }
    }

    return (
        <div>
            <h1>Users</h1>
            <br />
            <Link to={`${path}/add`} className="btn btn-sm btn-success mb-2">Add User</Link>
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
                            <Link to={`${path}/edit/${data.id}`} className="btn btn-sm btn-primary mr-1">Edit</Link>
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
        </div>
    );
}

export { List };