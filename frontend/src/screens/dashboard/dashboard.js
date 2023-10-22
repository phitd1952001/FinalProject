import React, {useEffect, useState} from 'react'

import Container from '@mui/material/Container';
import Grid from '@mui/material/Unstable_Grid2';
import Typography from '@mui/material/Typography';

import AppWidgetSummary from "./components/AppWidgetSummary";
import AppWebsiteVisits from "./components/AppWebsiteVisits";
import AppCurrentVisits from "./components/AppCurrentVisits";
import { dashboardService } from '../../_services';

const Dashboard = () => {
  const [data, setData] = useState({});
  useEffect(()=>{
    dashboardService.get().then((res)=>{
      setData(res)
    }).catch(err=>console.log(err))
  },[])
  return (
    <Container maxWidth="xl">
      <Typography variant="h4" sx={{ mb: 5 }}>
        Hi, Welcome back 👋
      </Typography>

      <Grid container spacing={3}>
        <Grid xs={12} sm={6} md={3}>
          <AppWidgetSummary
            title="Users"
            total={data.users}
            color="info"
            icon={<img alt="icon" src="https://res.cloudinary.com/dafhoj5q5/image/upload/v1697905476/nkqbtqtbeqc4mdxfgpej.png" />}
          />
        </Grid>

        <Grid xs={12} sm={6} md={3}>
          <AppWidgetSummary
            title="Slots"
            total={data.slots}
            color="success"
            icon={<img alt="icon" src="https://res.cloudinary.com/dafhoj5q5/image/upload/v1697992233/mrsbbcwr0jwioxppfohn.png" />}
          />
        </Grid>

        <Grid xs={12} sm={6} md={3}>
          <AppWidgetSummary
            title="Subjects"
            total={data.subjects}
            color="warning"
            icon={<img alt="icon" src="https://res.cloudinary.com/dafhoj5q5/image/upload/v1697992233/mrvgiiwaccduhm7kpl45.png" />}
          />
        </Grid>

        <Grid xs={12} sm={6} md={3}>
          <AppWidgetSummary
            title="Rooms"
            total={data.rooms}
            color="error"
            icon={<img alt="icon" src="https://res.cloudinary.com/dafhoj5q5/image/upload/v1697992233/qvxfaweeoq7fdipq2cjk.png" />}
          />
        </Grid>

        <Grid xs={12} md={6} lg={8}>
          <AppWebsiteVisits
            title="CheckIn Ratio"
            subheader="(+43%) than last year"
            chart={{
              labels: data.labels,
              series: [
                {
                  name: 'Number Of Slot In A Day',
                  type: 'column',
                  fill: 'solid',
                  data: data.totalSlotInDay,
                },
                {
                  name: 'Total Of Students Check In',
                  type: 'area',
                  fill: 'gradient',
                  data: data.totalCheckInInDay,
                },
                {
                  name: 'Total Of Students Being Rejected',
                  type: 'line',
                  fill: 'solid',
                  data: data.totalRejectedInDay,
                },
              ],
            }}
          />
        </Grid>

        <Grid xs={12} md={6} lg={4}>
          <AppCurrentVisits
            title="Current Visits"
            chart={{
              series: [
                { label: 'Student', value: 4344 },
                { label: 'Supervisor', value: 5435 },
                { label: 'Staff', value: 1443 },
                { label: 'User', value: 4443 },
              ],
            }}
          />
        </Grid>
      </Grid>
    </Container>
  )
}

export default Dashboard