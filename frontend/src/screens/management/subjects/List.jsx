import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import Button from 'devextreme-react/button';
import LoadPanel from 'devextreme-react/load-panel';
import { subjectService } from '../../../_services';
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

const List = ({ match }) => {
    const { path } = match;
    const [subjects, setSubjects] = useState(null);

    useEffect(() => {
        subjectService.getAll().then(x => setSubjects(x));
    }, []);

    function deleteSubject(id) {
        if (confirm('Do You Want to delete')) {
            setSubjects(subjects.map(x => {
                if (x.id === id) { x.isDeleting = true; }
                return x;
            }));
            subjectService.delete(id).then(() => {
                setSubjects(subjects => subjects.filter(x => x.id !== id));
            });
        }
    }

    function deleteSubject(id) {
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
                setSubjects(subjects.map(x => {
                    if (x.id === id) { x.isDeleting = true; }
                    return x;
                }));
                subjectService.delete(id).then(() => {
                    setSubjects(subjects => subjects.filter(x => x.id !== id));
                    Swal.fire(
                        'Deleted!',
                        'Your record has been deleted.',
                        'success'
                    )
                });
            }
        })
    }

    return (
        <div>
            <h1>Subject</h1>
            <br />
            <Link to={`${path}/add`} className="btn btn-sm btn-success mb-2">Add Subject</Link>
            <DataGrid
                dataSource={subjects}
                showBorders={true}
                columnAutoWidth={true}
                noDataText="No subjects available"
                allowColumnResizing={true}
            >
                <HeaderFilter visible={true} />
                <Selection mode="single" />
                <GroupPanel visible={true} />
                <SearchPanel visible={true} highlightCaseSensitive={true} />
                <Grouping autoExpandAll={false} />
                <FilterRow visible={true} />

                <Column dataField="subjectCode" caption="Subject Code" width="25%" />
                <Column dataField="name" caption="Name" width="25%" />
                <Column dataField="description" caption="Description" width="25%" />
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
                                onClick={() => deleteSubject(data.id)}
                                width={79}
                                height={29}
                                hint="Delete Subject"
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
                visible={subjects === null}
                showIndicator={true}
                shading={true}
                position={{ of: 'body' }}
            />
        </div>
    );
}

export { List };