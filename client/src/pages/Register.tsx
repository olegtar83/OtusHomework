import { type FC, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { jwtDecode } from "jwt-decode";

interface RegisterResponse {
  userId: string;
  token: string;
}

interface RegisterFormData {
  id: string;
  first_name: string;
  second_name: string;
  age: number;
  sex: string;
  biography: string;
  city: string;
  password: string;
  confirmPassword: string;
}

const Register: FC = () => {
  const navigate = useNavigate();

  // Generate a random GUID function
  const generateGuid = () => {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  };

  const [formData, setFormData] = useState<RegisterFormData>({
    id: generateGuid(),  // Set initial GUID
    first_name: '',
    second_name: '',
    age: 0,
    sex: '',
    biography: '',
    city: '',
    password: '',
    confirmPassword: ''
  });
  const [error, setError] = useState<string>('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    try {
      if (formData.password !== formData.confirmPassword) {
        throw new Error('Passwords do not match');
      }

      const registerResponse = await fetch('http://localhost:7888/api/user/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'accept': '*/*'
        },
        body: JSON.stringify({
          id: formData.id,
          first_name: formData.first_name,
          second_name: formData.second_name,
          age: Number(formData.age),
          sex: formData.sex,
          biography: formData.biography,
          city: formData.city,
          password: formData.password
        })
      });

      if (!registerResponse.ok) {
        const errorData = await registerResponse.json();
        throw new Error(errorData.message || 'Registration failed');
      }
      const data = await registerResponse.json();
      
      // Store the JWT token
      localStorage.setItem('token', data.token);
      
      // Decode and store user info from JWT
      const decodedToken: any = jwtDecode(data.token);
      const userName = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
      localStorage.setItem('userName', userName);
      localStorage.setItem('userId', data.userId);
      
      navigate('/feed');
    } catch (err) {
      console.error('Registration error:', err);
      setError(err instanceof Error ? err.message : 'An error occurred');
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  return (
    <div className="auth-container">
      <div className="auth-box">
        <h1>Register</h1>
        
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
              readOnly
            />
          </div>

          <div className="form-group">
            <input
              type="text"
              name="first_name"
              placeholder="First Name"
              value={formData.first_name}
              onChange={handleInputChange}
              required
            />
          </div>

          <div className="form-group">
            <input
              type="text"
              name="second_name"
              placeholder="Second Name"
              value={formData.second_name}
              onChange={handleInputChange}
              required
            />
          </div>

          <div className="form-group">
            <input
              type="number"
              name="age"
              placeholder="Age"
              value={formData.age}
              onChange={handleInputChange}
              required
            />
          </div>

          <div className="form-group">
            <select
              name="sex"
              value={formData.sex}
              onChange={handleInputChange}
              required
            >
              <option value="">Select Gender</option>
              <option value="male">Male</option>
              <option value="female">Female</option>
              <option value="other">Other</option>
            </select>
          </div>

          <div className="form-group">
            <textarea
              name="biography"
              placeholder="Biography"
              value={formData.biography}
              onChange={handleInputChange}
              required
            />
          </div>

          <div className="form-group">
            <input
              type="text"
              name="city"
              placeholder="City"
              value={formData.city}
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

          <div className="form-group">
            <input
              type="password"
              name="confirmPassword"
              placeholder="Confirm Password"
              value={formData.confirmPassword}
              onChange={handleInputChange}
              required
            />
          </div>

          <button type="submit">Register</button>
        </form>

        <p className="toggle-auth">
          Already have an account?{' '}
          <button 
            className="toggle-btn"
            onClick={() => navigate('/login')}
          >
            Login
          </button>
        </p>
      </div>
    </div>
  );
};

export default Register;