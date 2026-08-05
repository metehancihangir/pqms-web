import { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import '../styles/Navbar.css';

const Navbar = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  return (
    <nav className="navbar">
      <div className="navbar__brand">
        <span className="navbar__logo">PQMS</span>
      </div>
      
      <button className="navbar__toggle" onClick={toggleMenu} aria-label="Toggle menu">
        <span className="hamburger"></span>
      </button>

      <div className={`navbar__menu ${isMenuOpen ? 'active' : ''}`}>
        <div className="navbar__links">
          <a href="/dashboard" className="navbar__link active">Dashboard</a>
          <a href="/queue-display" className="navbar__link">Queue Display</a>
          {user?.role === 'Admin' && <a href="/admin" className="navbar__link">Admin</a>}
        </div>

        <div className="navbar__user">
          <span className="navbar__user-name">
            {user?.fullName === 'Sistem Yöneticisi' ? 'System Administrator' : user?.fullName} ({user?.role})
          </span>
          <button className="navbar__logout-btn" onClick={handleLogout}>Logout</button>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
