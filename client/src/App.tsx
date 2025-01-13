import { useEffect, useState } from 'react';
import { Routes, Route, useNavigate, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import User from './pages/User';
import Feed from './pages/Feed';
import Friends from './pages/Friends';
import './App.css';

const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const navigate = useNavigate();
  const token = localStorage.getItem('token');
  const userId = localStorage.getItem('userId');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const checkAuth = async () => {
      if (!token || !userId) {
        navigate('/login');
        return;
      }

      try {
        const response = await fetch(`http://localhost:7888/api/user/get/${userId}`, {
          headers: {
            'accept': '*/*',
            'Authorization': `Bearer ${token}`
          }
        });

        if (response.ok) {
          const userData = await response.json();
          localStorage.setItem('userData', JSON.stringify(userData));
        } else {
          localStorage.clear();
          navigate('/login');
        }
      } catch (error) {
        console.error('Error fetching user data:', error);
        localStorage.clear();
        navigate('/login');
      } finally {
        setLoading(false);
      }
    };

    checkAuth();
  }, [navigate, token, userId]);

  if (loading) {
    return <div>Loading...</div>;
  }

  return <>{children}</>;
};

function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/feed" replace />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route
        path="/feed"
        element={
          <ProtectedRoute>
            <Feed />
          </ProtectedRoute>
        }
      />
      <Route
        path="/user/:id"
        element={
          <ProtectedRoute>
            <User />
          </ProtectedRoute>
        }
      />
      <Route path="/friends" element={<Friends />} />
    </Routes>
  );
}

export default App;