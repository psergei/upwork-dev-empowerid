import { useEffect, useState } from 'react';
import './PostDetails.scss';
import useFetchWithMsal from '../useFetchWithMsal';
import { resources } from '../auth-config';

export const PostDetails = ({ post }) => {
  const [details, setDetails] = useState();
  const { execute } = useFetchWithMsal({
    scopes: resources.api.scopes.default
  });
  const [lastUpdated, setLastUpdated] = useState(new Date());

  useEffect(() => {
    execute ('GET', resources.api.endpoint + `/posts/${post.id}`)
    .then((response) => setDetails(response));
  }, [execute, post, lastUpdated])

  const commentsNodes = details?.comments?.map(c => (
    <div className="comment" key={c.id}>
      <div className="comment-body">{c.content}</div>
      <div className="comment-info">by {c.author} on {new Date(c.createdDate).toLocaleString()}</div>
    </div>
  ));

  const handleComment = (e) => {
    e.preventDefault();

    const data = new FormData(e.target);

    const commentData = {
      postId: post.id,
      content: data.get('content'),
    }

    execute('POST', resources.api.endpoint + '/comments', commentData)
      .then(() => {
        setLastUpdated(new Date());
        e.target.reset();
      });
  }

  return (
    <div className="post-details">
      <h4>{post.title} by {post.author}</h4>
      <div className='content'>
        {post.content}
      </div>

      <h4>Comments:</h4>
      <div className="comments">
        {commentsNodes}
      </div>

      <b>New comment</b>
      <form onSubmit={handleComment} id="form">
        <div className="new-comment">
          <textarea rows="20" name="content"></textarea>
        </div>
      </form>
      <button type="submit" form="form">Comment!</button>
    </div>
  );
}