import React, { Fragment, useEffect, useState } from 'react';
import { NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import authService from './AuthorizeService';
import { ApplicationPaths } from './ApiAuthorizationConstants';

const populateState = async function (setState) {
  const [isAuthenticated, user] = await Promise.all([authService.isAuthenticated(), authService.getUser()]);
  setState({
    isAuthenticated,
    userName: user && user.name,
  });
};

const authenticatedView = function (userName, profilePath, logoutPath, logoutState) {
  return (
    <>
      <NavItem>
        <NavLink tag={Link} className="text-dark" to={profilePath}>{userName}</NavLink>
      </NavItem>
      <NavItem>
        <NavLink replace tag={Link} className="text-dark" to={logoutPath} state={logoutState}>Выйти</NavLink>
      </NavItem>
    </>
  );
};

const anonymousView = function (registerPath, loginPath) {
  return (
    <>
      <NavItem>
        <NavLink tag={Link} className="text-dark" to={registerPath}>Зарегистрироваться</NavLink>
      </NavItem>
      <NavItem>
        <NavLink tag={Link} className="text-dark" to={loginPath}>Войти</NavLink>
      </NavItem>
    </>
  );
};

const LoginMenu = function () {
  const [state, setState] = useState({
    isAuthenticated: false,
    userName: null,
  });

  useEffect(() => {
    const _subscription = authService.subscribe(() => populateState(setState));
    populateState(setState);

    return () => authService.unsubscribe(_subscription);
  }, []);

  const { isAuthenticated, userName } = state;

  if (!isAuthenticated) {
    const registerPath = `${ApplicationPaths.Register}`;
    const loginPath = `${ApplicationPaths.Login}`;

    return anonymousView(registerPath, loginPath);
  }
  const profilePath = `${ApplicationPaths.Profile}`;
  const logoutPath = `${ApplicationPaths.LogOut}`;
  const logoutState = { local: true };

  return authenticatedView(userName, profilePath, logoutPath, logoutState);
};

export { LoginMenu };
