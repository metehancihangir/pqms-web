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
          <span className="patient-card__time">Giriş: {formatTime(patient.checkInTime)}</span>
          {patient.visitReason && <span className="patient-card__reason">Nedeni: {patient.visitReason}</span>}
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
          {isCompleting ? 'Tamamlanıyor...' : 'Muayeneyi Bitir'}
        </button>
      )}
    </div>
  );
};

export default PatientCard;
