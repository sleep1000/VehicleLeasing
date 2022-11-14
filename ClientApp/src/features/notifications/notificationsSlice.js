import { createSlice } from '@reduxjs/toolkit';
import { maxNotificationsCount } from '../../app/constants';

const initialState = [];

const notificationsSlice = createSlice({
  name: 'notifications',
  initialState,
  reducers: {
    pushNotification: (state, action) => {
      if (state.length === maxNotificationsCount) {
        state.shift();
      }
      state.push(action.payload);
    },
    popNotification: (state) => {
      state.shift();
    },
    resetState: () => {
      return initialState;
    }
  },
});

const { reducer } = notificationsSlice;

export const { pushNotification, popNotification } = notificationsSlice.actions;

export { reducer as notificationsReducer };
