import React, { useState } from 'react';
import { useLocation } from 'react-router-dom';
import Header from '../components/Header';
import '../styles/Friends.css';

const Friends = () => {
  const location = useLocation();
  const { searchResults } = location.state || { searchResults: [] }; // Get search results from location state
  const DEFAULT_AVATAR = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+PHBhdGggZD0iTTEyIDEyYzIuMjEgMCA0LTEuNzkgNC00cy0xLjc5LTQtNC00LTQgMS43OS00IDQgMS43OSA0IDQgNHptMCAyYy0yLjY3IDAtOCAxLjM0LTggNHYyaDE2di0yYzAtMi42Ni01LjMzLTQtOC00eiIgZmlsbD0iI2FhYWFhYSIvPjwvc3ZnPg==';
  // State to track added friends
  const [addedFriends, setAddedFriends] = useState<{ [key: string]: boolean }>({});
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const handleSendRequest = async (friendId: string) => {
    const response = await fetch(`http://localhost:7888/api/Friends/Set/${friendId}`, {
      method: 'PUT',
      headers: {
        'accept': '*/*',
        'Authorization': `Bearer ${localStorage.getItem('token')}`, // Assuming token is stored in localStorage
      },
    });

    if (response.ok) {
      alert('Friend request sent successfully!'); // Notify user of success
      setAddedFriends((prev) => ({ ...prev, [friendId]: true })); // Mark friend as added
    } else {
      console.error('Failed to send friend request:', response.statusText);
    }
  };

  const handleFriendRequest = async (userId: string) => {
    setIsLoading(true);
    try {
      const response = await fetch('http://localhost:7888/api/friends/request', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({ friendId: userId })
      });

      if (!response.ok) {
        throw new Error('Failed to send friend request');
      }
      // Handle success
    } catch (error) {
      console.error('Error:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div>
      <Header />
      <h1>Friends</h1>
      {searchResults.length > 0 ? (
        <ul className="friends-list"> {/* Apply the friends-list class */}
          {searchResults.map((friend: any) => ( // Adjust type as necessary
            <li key={friend.id} className="friend-item"> {/* Apply the friend-item class */}
              <img 
                src={DEFAULT_AVATAR}
                alt={friend.name}
                className="friend-image"
              />
              <div className="friend-details"> {/* Container for friend details */}
                <div><span className="friend-name">First Name: {friend.first_name}</span></div> {/* Display friend's first name */}
                <div><span className="friend-name">Second Name: {friend.second_name}</span></div> {/* Display friend's second name */}
                <div><span className="friend-city">City: {friend.city}</span></div> {/* Display friend's city */}
              </div>
              <div className="friend-button-container"> {/* Container for buttons */}
                {!addedFriends[friend.id] && ( // Only show the send request button if friend is not added
                  <button 
                    className="friend-request-button" 
                    onClick={() => handleSendRequest(friend.id)} // Send friend request on button click
                  >
                    Send Friend Request
                  </button>
                )}
              </div>
            </li>
          ))}
        </ul>
      ) : (
        <p>No friends found.</p> // Message if no results
      )}
    </div>
  );
};

export default Friends; 