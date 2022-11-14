import { applyMiddleware, combineReducers, configureStore } from '@reduxjs/toolkit';
import thunkMiddleware from 'redux-thunk';
import { composeWithDevTools } from '@redux-devtools/extension';
import { leaseTimersReducer } from '../features/leasetimers/leasesSlice';
import { vehiclesReducer } from '../features/vehicles/vehiclesSlice';
import { notificationsReducer } from '../features/notifications/notificationsSlice';
import { signalRSliceReducer } from '../features/signalr/SignalRSlice';

const composedEnhancer = composeWithDevTools(applyMiddleware(thunkMiddleware));

const store = configureStore({
  reducer: {
    vehicles: vehiclesReducer,
    leaseTimers: leaseTimersReducer,
    notifications: notificationsReducer,
    signalR: signalRSliceReducer,
  },
  composedEnhancer,
});

const appReducer = combineReducers([vehiclesReducer, leaseTimersReducer, notificationsReducer]);

export const clearStateReducer = (_, action) => {
  return appReducer(undefined, action);
}

export default store;
