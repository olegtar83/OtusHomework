import { useEffect, useState, useRef } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import Header from '../components/Header';
import '../styles/Feed.css';

interface Post {
  id: string;
  name: string;
  userId: string;
  text: string;
  created: Date;
}

const Feed = () => {
  const [posts, setPosts] = useState<Post[]>([]);
  const [newPostText, setNewPostText] = useState('');
  const [isPosting, setIsPosting] = useState(false);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const fetchInitialPosts = async () => {
      const token = localStorage.getItem('token');
      const userId = localStorage.getItem('userId');
      
      try {
        const response = await fetch(`http://localhost:7888/api/Post/Feed/${userId}`, {
          headers: {
            'accept': '*/*',
            'Authorization': `Bearer ${token}`
          }
        });

        if (response.ok) {
          const data = await response.json();
          // Sort posts by date, newest first
          const sortedPosts = data.sort((a: Post, b: Post) => 
            new Date(b.created).getTime() - new Date(a.created).getTime()
          );
          setPosts(sortedPosts);
        } else {
          console.error('Failed to fetch posts');
        }
      } catch (err) {
        console.error('Error fetching posts:', err);
      }
    };

    // First fetch initial posts
    fetchInitialPosts();

    // Then set up SignalR connection
    const connectToSignalR = async () => {
      if (connectionRef.current) {
        return;
      }

      try {
        const token = localStorage.getItem('token');
        if (!token) {
          console.error('No token found');
          return;
        }

        connectionRef.current = new HubConnectionBuilder()
          .withUrl('http://localhost:7888/feed', {
            accessTokenFactory: () => token,
            withCredentials: false,
          })
          .withAutomaticReconnect([0, 2000, 5000, 10000, 20000])
          .build();

        // Set up connection event handlers
        connectionRef.current.onreconnecting(() => console.log('Reconnecting to SignalR...'));
        connectionRef.current.onreconnected(() => console.log('Reconnected to SignalR'));
        connectionRef.current.onclose(() => console.log('Connection closed'));

        connectionRef.current.on('PushToFeed', (post: Post) => {
          console.log('New post received:', post);
          setPosts(prevPosts => [post, ...prevPosts]);
        });

        await connectionRef.current.start();
        console.log('Connected to SignalR hub');
      } catch (err) {
        console.error('Error in SignalR connection:', err);
        connectionRef.current = null;
        // Try to reconnect after 5 seconds
        setTimeout(connectToSignalR, 5000);
      }
    };

    connectToSignalR();

    return () => {
      if (connectionRef.current?.state === 'Connected') {
        connectionRef.current.stop()
          .then(() => {
            console.log('Successfully disconnected');
            connectionRef.current = null;
          })
          .catch(err => console.error('Error disconnecting:', err));
      }
    };
  }, []);

  // Function to handle post creation
  const handleCreatePost = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsPosting(true);
    const token = localStorage.getItem('token');
    
    try {
      const response = await fetch('http://localhost:7888/api/Post/Create', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({ text: newPostText })
      });

      if (response.ok) {
        setNewPostText('');
      } else {
        console.error('Failed to create post');
      }
    } catch (err) {
      console.error('Error creating post:', err);
    } finally {
      setIsPosting(false);
    }
  };

  return (
    <div className="feed-container">
      <Header />
      <div className="feed-content">
        <div className="welcome-section">
          <h2>Welcome to your feed!</h2>
        </div>

        <div className="main-content">
          <div className="post-form-container">
            <form onSubmit={handleCreatePost} className="post-form">
              <textarea
                value={newPostText}
                onChange={(e) => setNewPostText(e.target.value)}
                placeholder="What's on your mind?"
                rows={3}
              />
              <div className="post-form-footer">
                <button type="submit" disabled={isPosting}>
                  {isPosting ? 'Posting...' : 'Post'}
                </button>
              </div>
            </form>
          </div>
          
          <div className="feed-posts">
            {posts.map(post => (
              <div key={post.id} className="post">
                <h3>{post.name || post.userId}</h3>
                <p>{post.text}</p>
                <small>
                  {new Date(post.created).toLocaleDateString('en-US', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit'
                  })}
                </small>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Feed; 