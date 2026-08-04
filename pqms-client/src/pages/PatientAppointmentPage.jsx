import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { appointmentService } from '../services/appointmentService';
import '../styles/PatientCheckInPage.css'; // Reusing the same CSS for forms

function PatientAppointmentPage() {
  const { patientId } = useParams();
  const navigate = useNavigate();

  const [appointmentDate, setAppointmentDate] = useState('');
  const [appointmentTime, setAppointmentTime] = useState('');
  const [reason, setReason] = useState('');
  const [notes, setNotes] = useState('');

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  const handleBookAppointment = async (e) => {
    e.preventDefault();
    if (!appointmentDate || !appointmentTime) {
      setError('Please select both date and time.');
      return;
    }

    // Combine date and time
    const dateTimeString = `${appointmentDate}T${appointmentTime}:00.000Z`;
    const combinedDate = new Date(dateTimeString);

    setLoading(true);
    setError('');

    try {
      await appointmentService.createAppointment(
        parseInt(patientId),
        combinedDate.toISOString(),
        reason,
        notes
      );
      setSuccess(true);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to book appointment.');
    } finally {
      setLoading(false);
    }
  };

  if (success) {
    return (
      <div className="checkin-container">
        <div className="checkin-success-card">
          <h2>📅 Appointment Booked!</h2>
          <p>Your appointment has been successfully scheduled.</p>
          <div className="checkin-queue-ticket" style={{ marginTop: '24px' }}>
            <span className="queue-label">Date & Time</span>
            <span className="queue-number" style={{ fontSize: '32px' }}>
              {appointmentDate} {appointmentTime}
            </span>
          </div>
          <button className="btn-checkin-home" onClick={() => navigate('/patient/welcome')} style={{ marginTop: '24px' }}>
            Return to Home
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="checkin-container">
      <div className="checkin-card">
        <h1>Book an Appointment</h1>
        <p className="subtitle">Schedule your next visit</p>

        <form onSubmit={handleBookAppointment} className="checkin-form">
          <div className="form-group">
            <label>Date</label>
            <input 
              type="date" 
              className="checkin-select" 
              value={appointmentDate} 
              onChange={(e) => setAppointmentDate(e.target.value)} 
              required
            />
          </div>

          <div className="form-group">
            <label>Time</label>
            <input 
              type="time" 
              className="checkin-select" 
              value={appointmentTime} 
              onChange={(e) => setAppointmentTime(e.target.value)} 
              required
            />
          </div>

          <div className="form-group">
            <label>Reason (Optional)</label>
            <input 
              type="text" 
              className="checkin-select" 
              placeholder="e.g. Routine Checkup"
              value={reason} 
              onChange={(e) => setReason(e.target.value)} 
            />
          </div>

          <div className="form-group">
            <label>Additional Notes (Optional)</label>
            <textarea
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              rows={3}
              className="checkin-textarea"
              placeholder="Any additional details..."
            />
          </div>

          {error && <div className="alert alert--danger">{error}</div>}

          <button type="submit" className="btn-checkin-submit" disabled={loading}>
            {loading ? 'Processing...' : 'Confirm Appointment'}
          </button>
        </form>
      </div>
    </div>
  );
}

export default PatientAppointmentPage;
