import { MsalProvider } from "@azure/msal-react";
import { Route, Routes } from "react-router-dom";
import { Layout } from "./components/Layout";
import { HomePage } from "./pages/HomePage";
import { NewPostPage } from "./pages/NewPostPage";
import { PostsPage } from "./pages/PostsPage";

const Pages = () => {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/posts" element={<PostsPage />} />
      <Route path="/new" element={<NewPostPage />} />
    </Routes>
  );
};

const App = ({ instance }) => {
  return (
    <MsalProvider instance={instance}>
      <Layout>
        <Pages />
      </Layout>
    </MsalProvider>
  );
};

export default App;