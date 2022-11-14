import React from 'react';
import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { Home } from './components/Home';

const AppRoutes = [
  {
    index: true,
    element: <Home />,
    requireAuth: true,
    path: '/',
  },
  ...ApiAuthorzationRoutes,
];

export default AppRoutes;
