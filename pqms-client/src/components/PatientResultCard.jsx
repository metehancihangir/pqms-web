import '../styles/PatientSearchPage.css';

const PatientResultCard = ({ patient, onBookAppointment, onCheckIn }) => {
  const formatDate = (dateString) => {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  return (
    <div className="patient-result-card">
      <div className="patient-result-card__info">
        <h3>{patient.fullName}</h3>
        <span>📞 {patient.phoneNumber || 'No phone number'}</span>
      </div>
      <span className="patient-result-card__dob">
        {formatDate(patient.dateOfBirth)}
      </span>
      <div className="patient-result-card__actions">
        <button className="btn-search btn-search--primary" onClick={onBookAppointment}>
          📅 Book Appointment
        </button>
        <button className="btn-search btn-search--success" onClick={onCheckIn}>
          ✓ Proceed to Check-In
        </button>
      </div>
    </div>
  );
};

export default PatientResultCard;
