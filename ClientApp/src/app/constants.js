export const rowSize = 3;
export const leaseMinDuration = 1;
export const leaseMaxDuration = 10;
export const vehiclesDataURL = 'api/Vehicles';
export const leaseVehicleURL = 'api/Leases';
export const leasesHubURL = '/leasesHub';
export const available = 'Свободно';
export const leased = 'Арендовано';
export const Ok = 'OK';
export const VehicleInLeaseErr = 'VEHICLE_IN_LEASE';
export const LeasesLimitExceededErr = 'LEASES_LIMIT_EXCEEDED';
export const maxNotificationsCount = 3;
export const VehicleLeasedEvent = 'VehicleLeased';

export const localizedNotifications = {
  [LeasesLimitExceededErr]: 'Превышен предел аренды',
  [VehicleInLeaseErr]: 'Автомобиль уже арендован',
};
