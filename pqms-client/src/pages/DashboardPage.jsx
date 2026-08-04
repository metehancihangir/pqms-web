import { useAuth } from '../context/AuthContext';

const DashboardPage = () => {
  const { user, logout } = useAuth();

  return (
    <div style={{ padding: '40px', fontFamily: 'Outfit, sans-serif' }}>
      <h1>Dashboard'a Hoş Geldiniz</h1>
      <p>Merhaba, <strong>{user?.fullName}</strong>! Rolünüz: <strong>{user?.role}</strong></p>
      <button 
        onClick={logout} 
        style={{ padding: '10px 20px', background: '#e53e3e', color: 'white', border: 'none', borderRadius: '8px', cursor: 'pointer', marginTop: '20px' }}>
        Çıkış Yap
      </button>
    </div>
  );
};

export default DashboardPage;
