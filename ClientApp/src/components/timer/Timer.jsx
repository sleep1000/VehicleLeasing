import React, { useEffect } from 'react';
import PropTypes from 'prop-types';
import { useDispatch, useSelector } from 'react-redux';
import { Button } from 'reactstrap';
import { startTimer } from '../../features/leasetimers/leasesSlice';

function Timer({ vehicleId }) {
  const timerSelector = (state) => state.leaseTimers[vehicleId];
  const timer = useSelector(timerSelector);

  const dispatch = useDispatch();

  useEffect(() => {
    if (timer.isStarted !== true) {
      dispatch(startTimer(vehicleId));
    }
  }, []);

  return (
    <Button block outline disabled>
      {timer.remain}
    </Button>
  );
}

Timer.propTypes = {
  vehicleId: PropTypes.number.isRequired
}

export { Timer };
