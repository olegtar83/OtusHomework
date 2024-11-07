import { useNavigate } from 'react-router-dom';
import './Header.css';

const Header = () => {
  const navigate = useNavigate();
  const userId = localStorage.getItem('userId');

  const handleLogout = () => {
    localStorage.clear();
    navigate('/login');
  };

  return (
    <nav className="navbar">
      <div className="nav-left">
        <h1 onClick={() => navigate('/feed')} style={{ cursor: 'pointer' }}>
          Legendary Social
        </h1>
      </div>
      <div className="nav-center">
        <input
          type="search"
          placeholder="Search..."
          onChange={(e) => {/* Add search functionality */}}
        />
      </div>
      <div className="nav-right">
        <button onClick={() => navigate('/feed')}>Feed</button>
        <button onClick={() => navigate('/friends')}>Friends</button>
        <button onClick={() => navigate('/messages')}>Messages</button>
        <button onClick={() => navigate(`/user/${userId}`)}>Profile</button>
        <button onClick={() => navigate('/settings')}>Settings</button>
        <button onClick={handleLogout}>Logout</button>
      </div>
    </nav>
  );
};

export default Header; 