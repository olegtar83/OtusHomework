import Header from '../components/Header';
import '../styles/Feed.css';

const Feed = () => {
  return (
    <div className="feed-container">
      <Header />
      <div className="feed-content">
        <h2>Welcome to your feed!</h2>
        {/* Feed content here */}
      </div>
    </div>
  );
};

export default Feed; 