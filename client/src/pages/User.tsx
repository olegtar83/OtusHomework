import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import Header from '../components/Header';
import '../styles/User.css';
import { HubConnectionBuilder } from '@microsoft/signalr';

interface UserData {
  id: string;
  first_name: string;
  second_name: string;
  age: number;
  sex: string;
  biography: string;
  city: string;
}

const User = () => {
  const { id } = useParams();
  const [userData, setUserData] = useState<UserData | null>(null);
  const [error, setError] = useState('');
  const token = localStorage.getItem('token');

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        debugger;
        const response = await fetch(`http://localhost:7888/api/user/get/${id}`, {
          headers: {
            'Accept': '*/*',
            'Authorization': `Bearer ${token}`
          }
        });

        if (!response.ok) {
          throw new Error('Failed to fetch user data');
        }

        const data = await response.json();
        setUserData(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load user data');
      }
    };

    if (id && token) {
      fetchUserData();
    }
  }, [id, token]);

  if (error) {
    return (
      <div className="page-container">
        <Header />
        <div className="error-container">Error: {error}</div>
      </div>
    );
  }

  if (!userData) {
    return (
      <div className="page-container">
        <Header />
        <div className="loading-container">Loading...</div>
      </div>
    );
  }

  return (
    <div className="page-container">
      <Header />
      <div className="content-container">
        <div className="profile-content">
          <div className="profile-header-card">
            <div className="profile-avatar">
              {userData.first_name[0]}{userData.second_name[0]}
            </div>
            <h2>{userData.first_name} {userData.second_name}</h2>
            <p className="profile-location">{userData.city}</p>
          </div>

          <div className="profile-details-card">
            <div className="profile-card info-card">
              <h3>Personal Information</h3>
              <div className="info-item">
                <span className="label">Age:</span>
                <span className="value">{userData.age}</span>
              </div>
              <div className="info-item">
                <span className="label">Gender:</span>
                <span className="value">{userData.sex}</span>
              </div>
              <div className="info-item">
                <span className="label">Location:</span>
                <span className="value">{userData.city}</span>
              </div>
            </div>

            <div className="profile-card bio-card">
              <h3>Biography</h3>
              <p>{userData.biography}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default User; 