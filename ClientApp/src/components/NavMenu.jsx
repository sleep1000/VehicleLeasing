import React, { useState } from 'react';
import {
  Collapse, Navbar, NavbarBrand, NavbarToggler,
} from 'reactstrap';
import { Link } from 'react-router-dom';
import { LoginMenu } from './api-authorization/LoginMenu';
import './NavMenu.css';

const NavMenu = function () {
  const [state, setState] = useState({
    collapsed: true,
  });

  return (
    <header>
      <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
        <NavbarBrand tag={Link} to="/">Аренда автомобилей</NavbarBrand>
        <NavbarToggler onClick={() => setState({ collapsed: !state.collapsed })} className="mr-2" />
        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!state.collapsed} navbar>
          <ul className="navbar-nav flex-grow">
            <LoginMenu />
          </ul>
        </Collapse>
      </Navbar>
    </header>
  );
};

NavMenu.displayName = NavMenu.name;

export { NavMenu };
