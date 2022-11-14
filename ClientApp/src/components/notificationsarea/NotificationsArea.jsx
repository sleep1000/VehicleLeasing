import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Alert } from 'reactstrap';
import { popNotification } from '../../features/notifications/notificationsSlice';

function NotificationsArea() {
  const notificationsSelector = (state) => state.notifications;
  const notifications = useSelector(notificationsSelector);
  const dispatch = useDispatch();

  const onToggle = () => {
    dispatch(popNotification());
  };

  return (
    <div>
      {notifications.map((notification, index) => (<Alert key={`notification-${index}`} toggle={onToggle}>{notification.text}</Alert>))}
    </div>
  );
}

export { NotificationsArea };
