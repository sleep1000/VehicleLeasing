import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import authService, { AuthenticationResultStatus } from './AuthorizeService';

import { QueryParameterNames, LogoutActions, ApplicationPaths } from './ApiAuthorizationConstants';
import { Navigate } from 'react-router-dom';

const logout = async function (returnUrl, setState) {
  const state = { returnUrl };
  const isauthenticated = await authService.isAuthenticated();
  if (isauthenticated) {
    const result = await authService.signOut(state);
    switch (result.status) {
      case AuthenticationResultStatus.Redirect:
        break;
      case AuthenticationResultStatus.Success:
        await navigateToReturnUrl(returnUrl);
        break;
      case AuthenticationResultStatus.Fail:
        setState({ message: result.message });
        break;
      default:
        throw new Error('Invalid authentication result status.');
    }
  } else {
    setState({ message: 'You successfully logged out!' });
  }
};

const processLogoutCallback = async function (setState) {
  const url = window.location.href;
  const result = await authService.completeSignOut(url);
  switch (result.status) {
    case AuthenticationResultStatus.Redirect:
      // There should not be any redirects as the only time completeAuthentication finishes
      // is when we are doing a redirect sign in flow.
      throw new Error('Should not redirect.');
    case AuthenticationResultStatus.Success:
      await navigateToReturnUrl(getReturnUrl(result.state));
      break;
    case AuthenticationResultStatus.Fail:
      setState({ message: result.message });
      break;
    default:
      throw new Error('Invalid authentication result status.');
  }
};

const populateAuthenticationState = async function (setState) {
  const authenticated = await authService.isAuthenticated();
  setState({ isReady: true, authenticated });
};

const getReturnUrl = function (state) {
  const params = new URLSearchParams(window.location.search);
  const fromQuery = params.get(QueryParameterNames.ReturnUrl);
  if (fromQuery && !fromQuery.startsWith(`${window.location.origin}/`)) {
    // This is an extra check to prevent open redirects.
    throw new Error('Invalid return url. The return url needs to have the same origin as the current page.');
  }
  return (state && state.returnUrl)
        || fromQuery
        || `${window.location.origin}${ApplicationPaths.LoggedOut}`;
};

const navigateToReturnUrl = function (returnUrl) {
  return window.location.replace(returnUrl);
};

// The main responsibility of this component is to handle the user's logout process.
// This is the starting point for the logout process, which is usually initiated when a
// user clicks on the logout button on the LoginMenu component.
const Logout = function (props) {
  const [state, setState] = useState({
    message: undefined,
    isReady: false,
    authenticated: false,
  });

  useEffect(() => {
    const { action } = props;

    switch (action) {
      case LogoutActions.Logout:
        if (window.history.state.usr.local) {
          logout(getReturnUrl(), setState);
        } else {
          // This prevents regular links to <app>/authentication/logout from triggering a logout
          setState({ isReady: true, message: 'The logout was not initiated from within the page.' });
        }
        break;
      case LogoutActions.LogoutCallback:
        processLogoutCallback(setState);
        break;
      case LogoutActions.LoggedOut:
        setState({ isReady: true, message: 'You successfully logged out!' });
        break;
      default:
        throw new Error(`Invalid action '${action}'`);
    }

    populateAuthenticationState(setState);
  }, []);

  const { isReady, message } = state;
  if (!isReady) {
    return <div />;
  }
  if (message) {
    return (<div>{message}</div>);
  }
  const { action } = props;
  switch (action) {
    case LogoutActions.Logout:
      return (<div>Processing logout</div>);
    case LogoutActions.LogoutCallback:
      return (<div>Processing logout callback</div>);
    case LogoutActions.LoggedOut:
      return (<Navigate to="/"/>);
    default:
      throw new Error(`Invalid action '${action}'`);
  }
};

Logout.propTypes = {
  action: PropTypes.string
}

export { Logout };
