import { createSlice } from '@reduxjs/toolkit';

const initialState = { connectionId: null };

const signalRSlice = createSlice({
  name: 'signalR',
  initialState,
  reducers: {
    setConnectionId: (state, action) => {
      state.connectionId = action.payload;
    },
  },
});

const { reducer } = signalRSlice;

export const { setConnectionId } = signalRSlice.actions;

export { reducer as signalRSliceReducer };
