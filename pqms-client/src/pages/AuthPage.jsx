import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import '../styles/AuthPage.css';
import medicineLogo from '../assets/medicine.png';

const AuthPage = () => {
  const [isLogin, setIsLogin] = useState(true);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [fullName, setFullName] = useState('');
  const [rememberMe, setRememberMe] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const { user, login, register } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (user) {
      if (user.role === 'Patient') {
        navigate('/patient/welcome');
      } else {
        navigate('/dashboard');
      }
    }
  }, [user, navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      let responseData;
      if (isLogin) {
        responseData = await login(email, password, rememberMe);
      } else {
        responseData = await register(fullName, email, password, 'Patient');
      }

      if (responseData?.role === 'Patient') {
        navigate('/patient/welcome');
      } else {
        navigate('/dashboard'); 
      }
    } catch (err) {
      if (isLogin) {
        setError('Invalid email or password.');
      } else {
        setError(err.response?.data?.message || 'An error occurred. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-header-logo">
        <img src={medicineLogo} alt="PQMS Logo" className="auth-logo-img" />
        <h1 className="auth-title">PQMS</h1>
        <p className="auth-subtitle">Patient Queue Management System</p>
      </div>

      <div className="auth-card">
        <div className="auth-tabs">
          <button 
            type="button"
            className={`auth-tab ${isLogin ? 'active' : ''}`}
            onClick={() => { setIsLogin(true); setError(''); }}
          >
            <span className="tab-icon">➜</span> Login
          </button>
          <button 
            type="button"
            className={`auth-tab ${!isLogin ? 'active' : ''}`}
            onClick={() => { setIsLogin(false); setError(''); }}
          >
            <span className="tab-icon">👤+</span> Register
          </button>
        </div>

        {error && <div className="auth-error">{error}</div>}

        <form className="auth-form" onSubmit={handleSubmit}>
          {!isLogin && (
            <div className="form-group">
              <label>Full Name</label>
              <input 
                type="text" 
                value={fullName} 
                onChange={(e) => setFullName(e.target.value)} 
                required 
              />
            </div>
          )}
          
          <div className="form-group">
            <label>Email Address</label>
            <input 
              type="email" 
              value={email} 
              onChange={(e) => setEmail(e.target.value)} 
              required 
            />
          </div>

          <div className="form-group">
            <label>Password</label>
            <input 
              type="password" 
              value={password} 
              onChange={(e) => setPassword(e.target.value)} 
              required 
            />
          </div>

          {isLogin && (
            <div className="auth-options">
              <label className="remember-me">
                <input 
                  type="checkbox" 
                  checked={rememberMe}
                  onChange={(e) => setRememberMe(e.target.checked)}
                />
                Remember me
              </label>
              <a href="#" className="forgot-password" onClick={(e) => e.preventDefault()}>
                Forgot password?
              </a>
            </div>
          )}

          <button type="submit" className="auth-submit-btn" disabled={loading}>
            {isLogin ? <><span className="btn-icon">➜</span> Login</> : 'Register'}
          </button>

          {isLogin && (
            <>
              <div className="auth-divider">
                <span>or</span>
              </div>
              <button 
                type="button" 
                className="auth-outline-btn"
                onClick={() => { setIsLogin(false); setError(''); }}
              >
                Create new account
              </button>
            </>
          )}
        </form>
      </div>
    </div>
  );
};

export default AuthPage;
