import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { useDispatch } from 'react-redux';
import {
  Card, CardBody, Button, Badge, Row, Col, Input, Tooltip,
} from 'reactstrap';
import {
  leaseMinDuration, leaseMaxDuration, available, leased,
} from '../../app/constants';
import { Timer } from '../timer/Timer';
import { leaseVehicle } from '../../features/leasetimers/leasesSlice';
import './VehicleCard.css';

const leaseDurationOnChange = (e, setInputState) => {
  e.preventDefault();

  if (e.currentTarget.value > leaseMaxDuration) {
    // eslint-disable-next-line no-param-reassign
    e.currentTarget.value = leaseMaxDuration;
  }

  if (e.currentTarget.value < leaseMinDuration) {
    // eslint-disable-next-line no-param-reassign
    e.currentTarget.value = leaseMinDuration;
  }

  setInputState({ enabled: true, value: e.currentTarget.value });
};

const createLease = (vehicleId, inputState, dispatch, setInputState) => {
  setInputState({ ...inputState, enabled: false });

  const cleanUpAction = () => {
    setInputState({ value: 1, enabled: true });
  };
  dispatch(leaseVehicle(vehicleId, inputState.value, cleanUpAction));
};

function VehicleCard({ vehicle, inLease }) {
  const dispatch = useDispatch();
  const [inputState, setInputState] = useState({ value: 1, enabled: true });

  const [tooltipOpen, setTooltipOpen] = useState(false);
  const toggle = () => setTooltipOpen(!tooltipOpen);

  return (
    <Card className="card-outline">
      <CardBody>
        {/* <Row>
                    <CardTitle className='text-center' tag="h5">
                        {vehicle.type}
                    </CardTitle>
                </Row> */}
        <Row className="img-row">
          <Col xs="12">
            <img id={`img-${vehicle.id}`} className="vehicle-img" src={`${vehicle.imageSrc}`} alt={vehicle.type} />
            <Tooltip
              autohide
              flip
              isOpen={tooltipOpen}
              target={`img-${vehicle.id}`}
              toggle={toggle}
            >
              {vehicle.type}
            </Tooltip>
          </Col>
        </Row>
        <Row>
          <Col xs="12">
            <h3>
              <Badge className="badge-lease">{inLease ? leased : available}</Badge>
            </h3>
          </Col>
        </Row>
        <Row>
          {inLease === true
            ? (
              <Col xs="12">
                <Timer vehicleId={vehicle.id} />
              </Col>
            )
            : (
              <>
                <Col xs="6">
                    <Button
                        block
                        onClick={createLease.bind(createLease, vehicle.id, inputState, dispatch, setInputState)}
                        disabled={!inputState.enabled}
                      >
                                Подтвердить
                      </Button>
                  </Col>
                <Col xs="6">
                    <Input
                        type="number"
                        onChange={(e) => leaseDurationOnChange(e, setInputState)}
                        value={inputState.value}
                        disabled={!inputState.enabled}
                      />
                  </Col>
              </>
            )}
        </Row>
      </CardBody>
    </Card>
  );
}

VehicleCard.propTypes = {
  vehicle: PropTypes.shape({
    id: PropTypes.number.isRequired,
    type: PropTypes.string.isRequired,
    imageSrc: PropTypes.string.isRequired
  }),
  inLease: PropTypes.bool,
}

export { VehicleCard };
