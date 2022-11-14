import { HubConnectionBuilder } from '@microsoft/signalr';
import React, { useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';
import { Route, Routes } from 'react-router-dom';
import { leasesHubURL, Ok, VehicleLeasedEvent } from './app/constants';
import AppRoutes from './AppRoutes';
import { AuthorizeRoute } from './components/api-authorization/AuthorizeRoute';
import { Layout } from './components/Layout';
import './custom.css';
import { addNewTimer } from './features/leasetimers/leasesSlice';
import { setConnectionId } from './features/signalr/SignalRSlice';

const App = function () {
  const dispatch = useDispatch();
  const [leasesHubConnection, setLeaseHubConnection] = useState(null);

  useEffect(() => {
    if (leasesHubConnection === null) {
      const newLeasesHubConnection = new HubConnectionBuilder()
        .withUrl(leasesHubURL)
      // .configureLogging(LogLevel.Information)
        .build();

      const start = async function start() {
        try {
          await newLeasesHubConnection.start();
          console.log('SignalR Connected.');

          newLeasesHubConnection.on(VehicleLeasedEvent, ({ status, payload }) => {
            if (status === Ok) {
              dispatch(addNewTimer(payload));
            }
          });

          setLeaseHubConnection(newLeasesHubConnection);
          dispatch(setConnectionId(newLeasesHubConnection.connectionId));
        } catch (err) {
          console.log(err);
          setTimeout(start, 5000);
        }
      }

      newLeasesHubConnection.onclose(async () => {
        await start();
      });

      start();
    }
  });
  
  return (
    <Layout>
      <Routes>
        {AppRoutes.map((route, index) => {
          const { element, requireAuth, ...rest } = route;
          return <Route key={index} {...rest} element={requireAuth ? <AuthorizeRoute {...rest} element={element} /> : element} />;
        })}
      </Routes>
    </Layout>
  );
};

export { App };
