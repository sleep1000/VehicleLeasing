import React from 'react';
import { useSelector } from 'react-redux';
import { Row, Col } from 'reactstrap';
import { rowSize } from '../../app/constants';
import './Vehicles.css';
import { VehicleCard } from '../vehiclecard/VehicleCard';

const makeGrid = function (vehicles, timers) {
  const rows = [];

  let currentRow;
  for (let i = 0; i < vehicles.length; i++) {
    if (i % rowSize === 0) {
      if (i > 0) {
        rows.push(<Row key={`row-${~~(i / rowSize)}`}>{currentRow}</Row>);
      }

      currentRow = [];
    }

    const vehicle = vehicles.at(i);
    const inLease = Object.prototype.hasOwnProperty.call(timers, vehicle.id);

    currentRow.push((
      <Col xs={`${~~(12 / rowSize)}`} key={`vehicle-${vehicle.id}`}>
        <VehicleCard vehicle={vehicle} inLease={inLease} />
      </Col>
    ));
  }

  if (vehicles.length > 0) {
    rows.push(<Row key={`row-${~~(vehicles.length / rowSize) + 1}`}>{currentRow}</Row>);
  }

  return rows;
};

function Vehicles() {
  const vehicleSelector = (state) => state.vehicles;
  const timersSelector = (state) => state.leaseTimers;
  
  const vehicles = useSelector(vehicleSelector);
  const timers = useSelector(timersSelector);

  return (
    <>
      { makeGrid(vehicles, timers) }
    </>
  );
}

export { Vehicles };
