import React from 'react';
import PropTypes from 'prop-types';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';

const Layout = function (props) {
  return (
    <div>
      <NavMenu />
      <Container>
        {props.children}
      </Container>
      <footer className="footer border-top pl-3 text-muted">
        <div className="container">
          &copy; 2022 - Аренда автомобилей
        </div>
      </footer>
    </div>
  );
};

Layout.displayName = Layout.name;
Layout.propTypes = {
  children: PropTypes.node,
}

export { Layout };
