import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { queueService } from '../services/queueService';
import '../styles/PatientCheckInPage.css';

function PatientCheckInPage() {
  const { patientId } = useParams();
  const navigate = useNavigate();

  const [checkInType, setCheckInType] = useState('Appointment');
  const [visitReason, setVisitReason] = useState('');
  const [additionalInfo, setAdditionalInfo] = useState('');
  const [termsAccepted, setTermsAccepted] = useState(false);
  const [visitReasons, setVisitReasons] = useState([]);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [checkInResult, setCheckInResult] = useState(null);

  useEffect(() => {
    const loadVisitReasons = async () => {
      try {
        const response = await queueService.getVisitReasons();
        setVisitReasons(response); // response.data is returned by axios in api.js? wait queueService returns response.data directly!
      } catch (err) {
        setError('Ziyaret sebepleri yüklenemedi.');
      }
    };
    loadVisitReasons();
  }, []);

  const handleCheckIn = async (e) => {
    e.preventDefault();
    if (!visitReason) {
      setError('Lütfen bir ziyaret sebebi seçin.');
      return;
    }
    if (!termsAccepted) {
      setError('Koşulları kabul etmelisiniz.');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const response = await queueService.checkIn({
        patientId: parseInt(patientId),
        checkInType,
        visitReason,
        additionalInfo,
        termsAccepted,
      });
      setCheckInResult(response);
    } catch (err) {
      setError(err.response?.data?.message || 'Check-in başarısız.');
    } finally {
      setLoading(false);
    }
  };

  if (checkInResult) {
    return (
      <div className="checkin-container">
        <div className="checkin-success-card">
          <h2>🎉 You're Checked In!</h2>
          <p>Please wait for your queue number to be called.</p>
          <div className="queue-display">
            <span className="queue-label">Your Queue Number</span>
            <span className="queue-number">{checkInResult.queueNumber}</span>
          </div>
          <p className="patient-name">Patient: {checkInResult.patientName}</p>
          <button className="btn-checkin-home" onClick={() => navigate('/patient/welcome')}>
            Return to Home
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="checkin-container">
      <div className="checkin-card">
        <h1>Welcome to Our Clinic</h1>
        <p className="subtitle">Please complete your check-in process</p>

        <div className="checkin-tab-bar">
          <button
            type="button"
            className={`checkin-tab ${checkInType === 'Appointment' ? 'checkin-tab--active' : ''}`}
            onClick={() => setCheckInType('Appointment')}
          >
            Appointment
          </button>
          <button
            type="button"
            className={`checkin-tab ${checkInType === 'WalkIn' ? 'checkin-tab--active' : ''}`}
            onClick={() => setCheckInType('WalkIn')}
          >
            Walk-In
          </button>
        </div>

        <form onSubmit={handleCheckIn} className="checkin-form">
          <div className="form-group">
            <label>Reason for Visit</label>
            <select value={visitReason} onChange={(e) => setVisitReason(e.target.value)} className="checkin-select">
              <option value="">Select reason</option>
              {visitReasons.map(r => (
                <option key={r.id} value={r.name}>{r.name}</option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label>Additional Info</label>
            <textarea
              value={additionalInfo}
              onChange={(e) => setAdditionalInfo(e.target.value)}
              rows={4}
              className="checkin-textarea"
              placeholder="Any additional details..."
            />
          </div>

          <label className="checkbox-container">
            <input
              type="checkbox"
              checked={termsAccepted}
              onChange={(e) => setTermsAccepted(e.target.checked)}
            />
            <span className="checkmark"></span>
            I agree to the clinic's <a href="#terms">terms and conditions</a> *
          </label>

          {error && <div className="alert alert--danger">{error}</div>}

          <button type="submit" className="btn-checkin-submit" disabled={loading}>
            {loading ? 'Processing...' : 'Check Ins'}
          </button>
        </form>
      </div>
    </div>
  );
}

export default PatientCheckInPage;
