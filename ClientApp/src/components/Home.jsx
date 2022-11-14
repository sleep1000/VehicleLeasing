import React from 'react';
import { NotificationsArea } from './notificationsarea/NotificationsArea';
import { Vehicles } from './vehicles/Vehicles';

const Home = function () {
  return (
    <>
      <Vehicles />
      <NotificationsArea />
    </>
  );
};

Home.displayName = Home.name;

export { Home };
