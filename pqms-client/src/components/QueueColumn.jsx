import PatientCard from './PatientCard';
import '../styles/QueueColumn.css';

const QueueColumn = ({ title, color, patients, onCompletePatient }) => {
  return (
    <div className={`queue-column queue-column--${color}`}>
      <div className="queue-column__header">
        <h2>{title}</h2>
        <span className="queue-column__count">{patients.length}</span>
      </div>
      
      <div className="queue-column__list">
        {patients.length === 0 ? (
          <div className="queue-column__empty">No patients</div>
        ) : (
          patients.map(patient => (
            <PatientCard 
              key={patient.id} 
              patient={patient} 
              onComplete={onCompletePatient}
            />
          ))
        )}
      </div>
      
      <p className="queue-column__footer">
        Total {title.toLowerCase()}: {patients.length} patients
      </p>
    </div>
  );
};

export default QueueColumn;
