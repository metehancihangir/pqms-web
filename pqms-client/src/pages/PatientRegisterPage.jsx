import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { patientService } from '../services/patientService';
import '../styles/PatientRegisterPage.css';

const PatientRegisterPage = () => {
  const navigate = useNavigate();

  const [fullName, setFullName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState('');
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleRegister = async (e) => {
    e.preventDefault();

    if (!fullName.trim()) {
      setError('Ad-soyad zorunludur.');
      return;
    }

    setLoading(true);
    setError('');

    try {
      // dateOfBirth formatı "YYYY-MM-DD" olarak gönderilir (input type="date" default)
      const data = {
        fullName: fullName.trim(),
        phoneNumber: phoneNumber.trim() || null,
        dateOfBirth: dateOfBirth ? new Date(dateOfBirth).toISOString() : null
      };

      const response = await patientService.create(data);
      // Başarılı olursa checkin sayfasına git
      navigate(`/patient/checkin/${response.data.id}`);
    } catch (err) {
      setError(err.response?.data?.message || 'Kayıt işlemi başarısız oldu.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="register-wrapper">
      <nav className="register-navbar">
        <div className="register-navbar__logo">PQMS</div>
        <div className="register-navbar__title">Patient Registration</div>
      </nav>

      <div className="register-container">
        <div className="register-card">
          <h1>Register New Patient</h1>
          <p className="register-subtitle">Please enter your details to create a record.</p>

          <form className="register-form" onSubmit={handleRegister}>
            <div className="form-group">
              <label>Full Name *</label>
              <input 
                type="text" 
                className="register-input" 
                placeholder="John Doe"
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
                required
              />
            </div>

            <div className="form-group">
              <label>Phone Number</label>
              <input 
                type="tel" 
                className="register-input" 
                placeholder="05551234567"
                value={phoneNumber}
                onChange={(e) => setPhoneNumber(e.target.value)}
              />
            </div>

            <div className="form-group">
              <label>Date of Birth</label>
              <input 
                type="date" 
                className="register-input" 
                value={dateOfBirth}
                onChange={(e) => setDateOfBirth(e.target.value)}
              />
            </div>

            {error && <div className="alert alert--danger">{error}</div>}

            <button 
              type="submit" 
              className="btn-register-submit"
              disabled={loading}
            >
              {loading ? 'Registering...' : 'Register Patient'}
            </button>
          </form>

          <div className="register-footer">
            <Link to="/patient/search" className="back-link">
              ← Back to Search
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PatientRegisterPage;
