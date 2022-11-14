import { createSlice } from '@reduxjs/toolkit';
import authService from '../../components/api-authorization/AuthorizeService';
import { vehiclesDataURL, Ok } from '../../app/constants';
import { populateTimersFromServer } from '../leasetimers/leasesSlice';

const initialState = [];

const vehiclesSlice = createSlice({
  name: 'vehicles',
  initialState,
  reducers: {
    populateStateFromServer: (state, action) => {
      const vehicles = [...action.payload];
      vehicles.sort((a, b) => a.id - b.id);
      return vehicles;
    },
    resetState: () => {
      return initialState;
    }
  },
});

export const populateVehiclesData = (onUnauthorized) => async (dispatch) => {
  const token = await authService.getAccessToken();

  if (!token)
    return;

  try {
    const response = await fetch(vehiclesDataURL, {
      headers: !token ? {} : { Authorization: `Bearer ${token}` },
    });

    switch (response.status) {
      case 200: {
        const { status, payload } = await response.json();

        switch (status) {
          case Ok: {
            dispatch(vehiclesSlice.actions.populateStateFromServer(payload));
            const timersState = payload
              .filter((vehicle) => vehicle.currentLease !== null)
              .reduce((acc, vehicle) => {
                // eslint-disable-next-line no-param-reassign
                acc[vehicle.id] = { remain: vehicle.currentLease.remain };
                return acc;
              }, {});

            dispatch(populateTimersFromServer(timersState));
            break;
          }
          default:
            console.error('Unexpected status when loading vehicles data');
            break;
        }
        
        break;
      }
      case 401:
        if (typeof onUnauthorized === 'function') {
          dispatch(() => onUnauthorized());
        }
        
        break;
    }
  } catch (e) {
    console.error(e);
  }
};

const { reducer } = vehiclesSlice;

export { reducer as vehiclesReducer };
