import { useEffect } from 'react';
import { Routes, Route, useNavigate, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import User from './pages/User';
import Feed from './pages/Feed';
import './App.css';

const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const navigate = useNavigate();
  const token = localStorage.getItem('token');
  const userId = localStorage.getItem('userId');

  useEffect(() => {
    const checkAuth = async () => {
      if (!token || !userId) {
        navigate('/login');
        return;
      }

      try {
        const response = await fetch(`http://localhost:7888/user/get/${userId}`, {
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
        localStorage.clear();
        navigate('/login');
      }
    };

    checkAuth();
  }, [navigate, token, userId]);

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
    </Routes>
  );
}

export default App;