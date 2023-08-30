import { AuthenticatedTemplate, useMsal } from "@azure/msal-react";

export const HomePage = () => {
  const { instance } = useMsal();
  const activeUser = instance.getActiveAccount();

  return (
    <>
      <AuthenticatedTemplate>
        {
          activeUser ? activeUser.username : 'No active user'
        }
      </AuthenticatedTemplate>
    </>
  );
};