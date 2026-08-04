import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import '../styles/Navbar.css';

const Navbar = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="navbar">
      <div className="navbar__brand">
        <span className="navbar__logo">PQMS</span>
      </div>
      
      <div className="navbar__links">
        <a href="/dashboard" className="navbar__link active">Dashboard</a>
        <a href="#" className="navbar__link">Queue Display</a>
        {user?.role === 'Admin' && <a href="#" className="navbar__link">Admin</a>}
      </div>

      <div className="navbar__user">
        <span className="navbar__user-name">{user?.fullName} ({user?.role})</span>
        <button className="navbar__logout-btn" onClick={handleLogout}>Çıkış Yap</button>
      </div>
    </nav>
  );
};

export default Navbar;
