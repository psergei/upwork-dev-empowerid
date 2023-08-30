import { NavigationBar } from "./NavigationBar";

export const Layout = (props) => {
  return (
    <>
      <NavigationBar />

      {props.children}
    </>
  );
};