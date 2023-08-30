import { AuthenticatedTemplate } from "@azure/msal-react";
import useFetchWithMsal from "../useFetchWithMsal";
import { resources } from "../auth-config";

import './NewPostPage.scss';
import { useNavigate } from "react-router-dom";

export const NewPostPage = () => {
  const { execute } = useFetchWithMsal({
    scopes: resources.api.scopes.default
  });

  const navigate = useNavigate();

  const handleCreatePost = (e) => {
    e.preventDefault();

    const data = new FormData(e.target);

    const postData = {
      title: data.get('title'),
      content: data.get('content'),
    }

    execute('POST', resources.api.endpoint + '/posts', postData)
      .then(() => navigate('/posts'));
  }

  return (
    <>
      <AuthenticatedTemplate>
        <h5>Add new post</h5>
        <form onSubmit={handleCreatePost} id="form">
          <div className="new-post">
            Title:<br />
            <input type="text" name="title"></input>
            <br />
            Content:<br />
            <textarea rows="20" name="content"></textarea>
          </div>
        </form>
        <button type="submit" form="form">Post!</button>
      </AuthenticatedTemplate>
    </>
  );
};