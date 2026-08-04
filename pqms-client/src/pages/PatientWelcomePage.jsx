import { useNavigate } from 'react-router-dom';
import '../styles/PatientWelcomePage.css';

const PatientWelcomePage = () => {
  const navigate = useNavigate();

  const handleExistingPatient = () => {
    navigate('/patient/search');
  };

  const handleNewPatient = () => {
    navigate('/patient/register');
  };

  return (
    <div className="welcome-wrapper">
      <nav className="welcome-navbar">
        <div className="welcome-navbar__logo">PQMS</div>
        <div className="welcome-navbar__title">Patient Welcome</div>
      </nav>

      <div className="welcome-container">
        <div className="welcome-card">
          <div className="welcome-card__icon">
            <span>?</span>
          </div>
          
          <h1 className="welcome-card__title">
            Have you been here before, or is this your first visit?
          </h1>
          <p className="welcome-card__subtitle">
            Please select one of the options below
          </p>

          <div className="welcome-card__actions">
            <button 
              className="btn-welcome btn-welcome--primary" 
              onClick={handleExistingPatient}
            >
              <span className="btn-icon">✓</span> Yes, I've been here before
            </button>
            
            <button 
              className="btn-welcome btn-welcome--outlined" 
              onClick={handleNewPatient}
            >
              <span className="btn-icon">👤+</span> No, this is my first visit
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PatientWelcomePage;
