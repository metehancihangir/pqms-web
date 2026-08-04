import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { patientService } from '../services/patientService';
import PatientResultCard from '../components/PatientResultCard';
import '../styles/PatientSearchPage.css';

const PatientSearchPage = () => {
  const navigate = useNavigate();

  const [searchTerm, setSearchTerm] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState('');
  const [results, setResults] = useState([]);
  const [hasSearched, setHasSearched] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!searchTerm.trim() && !dateOfBirth) {
      setError('Lütfen arama terimi veya doğum tarihi giriniz.');
      return;
    }

    setLoading(true);
    setError('');
    setHasSearched(true);

    try {
      const data = await patientService.search(searchTerm, dateOfBirth);
      setResults(data);
    } catch (err) {
      setError(err.response?.data?.message || 'Arama sırasında bir hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  const handleBookAppointment = (patientId) => {
    navigate(`/patient/appointment/${patientId}`);
  };

  const handleCheckIn = (patientId) => {
    navigate(`/patient/checkin/${patientId}`);
  };

  return (
    <div className="search-wrapper">
      <nav className="search-navbar">
        <div className="search-navbar__logo">PQMS</div>
        <div className="search-navbar__title">Patient Search</div>
      </nav>

      <div className="search-container">
        <div className="search-card">
          <div className="search-header">
            <h1>Find Patient</h1>
            <p>Search by name or phone number</p>
          </div>

          <form className="search-form" onSubmit={handleSearch}>
            <div className="search-form__row">
              <input
                type="text"
                className="search-input"
                placeholder="Search Term (Name or Phone)"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
              <input
                type="date"
                className="search-input search-input--date"
                value={dateOfBirth}
                onChange={(e) => setDateOfBirth(e.target.value)}
              />
            </div>
            
            {error && <div className="alert alert--danger">{error}</div>}
            
            <button 
              type="submit" 
              className="btn-search-submit"
              disabled={loading}
            >
              {loading ? 'Searching...' : '🔍 Search Patient'}
            </button>
          </form>

          {hasSearched && !loading && (
            <div className="search-results">
              <h2>Search Results</h2>
              
              {results.length === 0 ? (
                <div className="alert alert--warning">
                  No patients found. <Link to="/patient/register" className="alert-link">Register new patient</Link>
                </div>
              ) : (
                <div className="results-list">
                  {results.map(patient => (
                    <PatientResultCard
                      key={patient.id}
                      patient={patient}
                      onBookAppointment={() => handleBookAppointment(patient.id)}
                      onCheckIn={() => handleCheckIn(patient.id)}
                    />
                  ))}
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default PatientSearchPage;
