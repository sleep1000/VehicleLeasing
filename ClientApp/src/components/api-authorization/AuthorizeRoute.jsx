import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Navigate } from 'react-router-dom';
import { ApplicationPaths, QueryParameterNames } from './ApiAuthorizationConstants';
import authService from './AuthorizeService';

const populateAuthenticationState = async function (setState) {
  const authenticated = await authService.isAuthenticated();
  setState({ ready: true, authenticated });
};

const authenticationChanged = async function (setState) {
  setState({ ready: false, authenticated: false });
  await populateAuthenticationState(setState);
};

const AuthorizeRoute = function (props) {
  const [authState, setAuthState] = useState({
    ready: false,
    authenticated: false,
  });

  useEffect(() => {
    const _subscription = authService.subscribe(() => authenticationChanged(setAuthState));
    populateAuthenticationState(setAuthState);

    return () => authService.unsubscribe(_subscription);
  }, []);

  const link = document.createElement('a');

  link.href = props.path;

  const returnUrl = `${link.protocol}//${link.host}${link.pathname}${link.search}${link.hash}`;
  const redirectUrl = `${ApplicationPaths.Login}?${QueryParameterNames.ReturnUrl}=${encodeURIComponent(returnUrl)}`;

  if (!authState.ready) {
    return <div />;
  }
  const { element } = props;
  return authState.authenticated ? element : <Navigate replace to={redirectUrl} />;
};

AuthorizeRoute.propTypes = {
  path: PropTypes.string,
  element: PropTypes.element
}

export { AuthorizeRoute };
