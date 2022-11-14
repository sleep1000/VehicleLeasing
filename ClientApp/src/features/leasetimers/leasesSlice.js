import { createSlice } from '@reduxjs/toolkit';
import {
  LeasesLimitExceededErr, leaseVehicleURL, localizedNotifications, Ok, VehicleInLeaseErr,
} from '../../app/constants';
import authService from '../../components/api-authorization/AuthorizeService';
import { pushNotification } from '../notifications/notificationsSlice';

const initialState = {};

const cleanupCall = (cleanupFn) => {
  if (typeof cleanupFn === 'function') {
    cleanupFn();
  }
};

const leaseTimersSlice = createSlice({
  name: 'leaseTimers',
  initialState,
  reducers: {
    populateTimersFromServer: (state, action) => ({ ...action.payload }),
    startTimer: (state, action) => {
      state[action.payload].isStarted = true;
    },
    removeTimer: (state, action) => {
      delete state[action.payload];
    },
    decrementTimer: (state, action) => {
      state[action.payload].remain -= 1;
    },
    addNewTimer: (state, action) => {
      state[action.payload.vehicleId] = {
        remain: action.payload.remain,
      };
    },
    updateTimers: (state, action) => {
      action.payload.forEach((lease) => {
        state[lease.vehicleId] = {
          remain: lease.remain,
        };
      });
    },
    resetState: () => {
      return initialState;
    }
  },
});

export const startTimer = (timerId, cleanup) => (dispatch, getState) => {
  let intervalId = 0;

  const timerCallback = () => {
    const state = getState();

    if (state.leaseTimers[timerId].remain === 0) {
      dispatch(leaseTimersSlice.actions.removeTimer(timerId));

      if (intervalId !== 0) {
        clearInterval(intervalId);
      }

      cleanupCall(cleanup);

      return;
    }

    dispatch(leaseTimersSlice.actions.decrementTimer(timerId));
  };

  dispatch(leaseTimersSlice.actions.startTimer(timerId));

  intervalId = setInterval(timerCallback, 1000);
};

const updateTimersFromServerResponse = (timers, cleanup) => (dispatch) => {
  dispatch(leaseTimersSlice.actions.updateTimers(timers));

  cleanupCall(cleanup);
};

export const leaseVehicle = (vehicleId, leaseDuration, cleanup) => async (dispatch, getState) => {
  const signalRConnectionId = getState()?.signalR?.connectionId;

  const token = await authService.getAccessToken();

  if (!token)
    return;

  try {
    const response = await fetch(leaseVehicleURL, {
      method: 'POST',
      headers: !token ? {} : {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ vehicleId, leaseDuration, signalRConnectionId }),
    });

    const { status, payload } = await response.json();

    switch (status) {
      case Ok:
        dispatch(leaseTimersSlice.actions.addNewTimer(payload));
        dispatch(startTimer(payload.vehicleId, cleanup));
        break;
      case LeasesLimitExceededErr:
        dispatch(pushNotification({ text: localizedNotifications[LeasesLimitExceededErr] }));
        dispatch(() => cleanupCall(cleanup));
        break;
      case VehicleInLeaseErr:
        dispatch(pushNotification({ text: localizedNotifications[VehicleInLeaseErr] }));
        dispatch(updateTimersFromServerResponse(payload, cleanup));
        break;
      default:
        console.error('Unexpected status when loading vehicles data');
        break;
    }
  } catch (e) {
    console.error(e);
  }
};

const { reducer } = leaseTimersSlice;

export const { populateTimersFromServer, updateTimers, addNewTimer } = leaseTimersSlice.actions;
export { reducer as leaseTimersReducer };
