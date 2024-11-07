import { type FC, useState } from 'react';
import { useNavigate } from 'react-router-dom';

interface LoginResponse {
  token: string;
}

interface LoginFormData {
  id: string;
  password: string;
}

interface UserData {
  id: string;
  first_name: string;
  second_name: string;
  age: number;
  sex: string;
  biography: string;
  city: string;
}

const Login: FC = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<LoginFormData>({
    id: '',
    password: '',
  });
  const [error, setError] = useState<string>('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    try {
      const loginResponse = await fetch('http://localhost:7888/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'accept': '*/*'
        },
        body: JSON.stringify(formData)
      });

      if (!loginResponse.ok) {
        throw new Error('Login failed');
      }

      const data: LoginResponse = await loginResponse.json();
      localStorage.setItem('token', data.token);
      localStorage.setItem('userId', formData.id);
      
      // Fetch user data
      const userResponse = await fetch(`http://localhost:7888/user/get/${formData.id}`, {
        headers: {
          'accept': '*/*',
          'Authorization': `Bearer ${data.token}`
        }
      });

      if (!userResponse.ok) {
        throw new Error('Failed to fetch user data');
      }

      const userData: UserData = await userResponse.json();
      localStorage.setItem('userData', JSON.stringify(userData));
      
      navigate('/feed'); // Redirect to feed page
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  return (
    <div className="auth-container">
      <div className="auth-box">
        <h1>Login</h1>
        
        {error && <div className="error-message">{error}</div>}
        
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <input
              type="text"
              name="id"
              placeholder="User ID"
              value={formData.id}
              onChange={handleInputChange}
              required
            />
          </div>
          
          <div className="form-group">
            <input
              type="password"
              name="password"
              placeholder="Password"
              value={formData.password}
              onChange={handleInputChange}
              required
            />
          </div>

          <button type="submit">Login</button>
        </form>

        <p className="toggle-auth">
          Don't have an account?{' '}
          <button 
            className="toggle-btn"
            onClick={() => navigate('/register')}
          >
            Register
          </button>
        </p>
      </div>
    </div>
  );
};

export default Login;