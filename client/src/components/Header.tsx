import { useNavigate } from 'react-router-dom';
import './Header.css';
import { useState } from 'react';

const Header = () => {
  const navigate = useNavigate();
  const userId = localStorage.getItem('userId');
  const userName = localStorage.getItem('userName');
  const [searchTerm, setSearchTerm] = useState('');

  const handleLogout = () => {
    localStorage.clear();
    navigate('/login');
  };

  const handleSearch = async () => {
    if (searchTerm.trim().split(' ').length <= 2) {
      const [firstName, lastName] = searchTerm.split(' ');
      const response = await fetch(`http://localhost:7888/api/User/Search?firstName=${firstName || ''}&lastName=${lastName || ''}`, {
        method: 'GET',
        headers: {
          'accept': '*/*',
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
        },
      });

      if (response.ok) {
        const data = await response.json();
        navigate('/friends', { state: { searchResults: data } });
      } else {
        console.error('Search failed:', response.statusText);
      }
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      handleSearch(); // Trigger search on Enter key
    }
  };

  const handleBlur = () => {
    handleSearch(); // Trigger search on blur event
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
          onChange={(e) => setSearchTerm(e.target.value)} // Update search term state
          onKeyDown={handleKeyDown} // Handle key down event
          onBlur={handleBlur} // Handle blur event
          value={searchTerm} // Bind input value to state
        />
      </div>
      <div className="nav-right">
        <span className="user-name">{userName}</span>
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