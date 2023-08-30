import { AuthenticatedTemplate } from "@azure/msal-react";
import { useEffect, useState } from "react";
import useFetchWithMsal from "../useFetchWithMsal";
import { resources } from "../auth-config";

import './PostsPage.scss';
import { PostDetails } from "../components/PostDetails";

export const PostsPage = () => {
  const [posts, setPosts] = useState();
  const { execute } = useFetchWithMsal({
    scopes: resources.api.scopes.default
  });

  const [selectedPost, setSelectedPost] = useState();

  const handlePostSelect = (post) => {
    setSelectedPost(post);
  };

  const handleDelete = (post) => {
    execute('DELETE', resources.api.endpoint + `/posts/${post.id}`)
    .then(response => {
      setSelectedPost(null);
      const index = posts.indexOf(post);
      posts.splice(index, 1);
      setPosts(posts);
    })
    .catch(err => console.log(err.message));
  }

  useEffect(() => {
    if (!posts) {
      execute('GET', resources.api.endpoint + '/posts')
        .then(response => setPosts(response))
        .catch(err => console.log(err.message));
    }
  }, [execute, posts]);

  const postsNodes = posts?.map(p => (
    <div className="post-item" key={p.id}>
      <div className="post-title" onClick={() => handlePostSelect(p)}>{p.title}
        <div className="post-info">by {p.author} on {new Date(p.createdDate).toLocaleString()}</div>
      </div>
      
      <button onClick={() => handleDelete(p)}>Delete</button>
    </div>
  ));

  return (
    <>
      <AuthenticatedTemplate>
        <h5>Posts</h5>
        <div className="blog">
          <div className="posts-list">
            {postsNodes}
          </div>

          {
            selectedPost ?
              <PostDetails post={selectedPost} />
              : null
          }
        </div>
      </AuthenticatedTemplate>
    </>
  );
};