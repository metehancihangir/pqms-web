import '../styles/PatientCard.css';

const PatientCard = ({ patient, onComplete, isCompleting }) => {
  const badgeClass = patient.checkInType === 'WalkIn' ? 'badge--blue' : 'badge--green';

  const formatTime = (dateString) => {
    if (!dateString) return '';
    return new Date(dateString).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  };

  return (
    <div className="patient-card">
      <div className="patient-card__main">
        <div className="patient-card__info">
          <h3>{patient.patientName || 'Bilinmeyen Hasta'}</h3>
          <span className="patient-card__time">
            Giriş: {formatTime(patient.checkInTime)}
            <span style={{ 
              marginLeft: '8px', 
              fontSize: '12px', 
              padding: '2px 6px', 
              borderRadius: '4px', 
              background: patient.checkInType === 'Appointment' ? '#dcfce7' : '#e0f2fe',
              color: patient.checkInType === 'Appointment' ? '#166534' : '#075985'
            }}>
              {patient.checkInType === 'Appointment' ? 'Appointment' : 'Walk-In'}
            </span>
          </span>
          {patient.visitReason && <span className="patient-card__reason">Reason: {patient.visitReason}</span>}
        </div>
        <span className={`badge ${badgeClass}`}>
          {patient.queueNumber}
        </span>
      </div>
      
      {patient.status === 'InProgress' && onComplete && (
        <button 
          className="btn-complete" 
          onClick={() => onComplete(patient.id)}
          disabled={isCompleting}
        >
          {isCompleting ? 'Completing...' : 'Finish Examination'}
        </button>
      )}
    </div>
  );
};

export default PatientCard;
