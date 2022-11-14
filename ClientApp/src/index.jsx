import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import { createRoot } from 'react-dom/client';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { Provider } from 'react-redux';
import { App } from './App';
import store from './app/store';
import { populateVehiclesData } from './features/vehicles/vehiclesSlice';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';

const rootElement = document.getElementById('root');
const root = createRoot(rootElement);

const router = createBrowserRouter([
  {
    path: "*",
    element: (<Provider store={store}>
                <App />
              </Provider>),
    loader: async () => {
      store.dispatch(populateVehiclesData(() => router.navigate(ApplicationPaths.Login)));
    },
  },
]);

root.render(
  <RouterProvider router={router} />
);
