import React, {useState, useEffect} from "react";
import Scheduler from "devextreme-react/scheduler";

import { calendarService } from "../../../_services";

const CalendarScreen = () => {
  const currentDate = new Date();
  const views = ["day", "week", "workWeek", "month"];
  const [data, setData] = useState([]);

  useEffect(()=>{
    calendarService.then(res=>{
      const result = res.map(convertObject);
      setData(result)
    }).catch(e=>console.log(e))
  },[])

  function convertObject(obj) {
    obj.startDate = new Date(obj.startDate);
    obj.endDate = new Date(obj.endDate);
    return {
      text: obj.text,
      startDate: `new Date("${obj.startDate.toISOString()}")`,
      endDate: `new Date("${obj.endDate.toISOString()}")`,
      description: `'${obj.description}'`
    };
  }

  return (
    <>
      <div style={{ textAlign: "center", fontSize: "30px" }}>
        Exam Schedule Overview
      </div>
      <br />
      <Scheduler
        timeZone="America/Los_Angeles"
        editing={{
          allowAdding: false,
          allowDeleting: false,
          allowResizing: false,
          allowDragging: false,
          allowUpdating: false,
        }}
        dataSource={data}
        views={views}
        defaultCurrentView="week"
        defaultCurrentDate={currentDate}
        height={840}
        startDayHour={7}
        endDayHour={17}
      />
    </>
  );
};

export { CalendarScreen };
