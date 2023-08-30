import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from '@azure/msal-react';
import { NavLink, Navbar } from 'react-bootstrap';
import { resources } from '../auth-config';

import './NavigationBar.scss';

export const NavigationBar = () => {
  const { instance } = useMsal();
  const activeUser = instance.getActiveAccount();

  const loginScopes = resources.api.scopes;

  const handleLogin = () => {
    instance.loginPopup({
      loginScopes
    }).catch(error => console.log(error));
  };

  const handleLogout = () => {
    instance.logoutPopup({
      mainWindowRedirectUri: '/',
      account: activeUser
    })
  };

  return (
    <>
      <Navbar className="navigation">
        <NavLink className="navlink" href="/">Home</NavLink>

        <AuthenticatedTemplate>
          <NavLink className="navlink" href="/posts">Posts</NavLink>
          <NavLink className="navlink" href="/new">Add Post</NavLink>
        </AuthenticatedTemplate>

        <div className="spacer"></div>

        <AuthenticatedTemplate>
          <div className="username">{ activeUser ? activeUser.name : 'N/A' }</div>
          <button onClick={handleLogout}>Logout</button>
        </AuthenticatedTemplate>

        <UnauthenticatedTemplate>
          <button onClick={handleLogin}>Login</button>
        </UnauthenticatedTemplate>
      </Navbar>
    </>
  );
}