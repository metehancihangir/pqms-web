import { useState, useEffect } from 'react';
import { queueService } from '../services/queueService';
import '../styles/QueueDisplayPage.css';

const QueueDisplayPage = () => {
  const [currentPatient, setCurrentPatient] = useState(null);
  const [waitingList, setWaitingList] = useState([]);
  const [loading, setLoading] = useState(true);

  // Otomatik yenileme — her 10 saniyede bir
  useEffect(() => {
    fetchQueueDisplay(); // İlk yükleme

    const interval = setInterval(() => {
      fetchQueueDisplay();
    }, 10000); // 10 saniye

    return () => clearInterval(interval); // Cleanup
  }, []);

  const fetchQueueDisplay = async () => {
    try {
      const response = await queueService.getQueueDisplay();
      setCurrentPatient(response.currentPatient);
      setWaitingList(response.waitingList);
    } catch (err) {
      console.error('Queue display fetch failed:', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="queue-display-loading">Loading Screen...</div>;
  }

  return (
    <div className="queue-display">
      <div className="queue-display__current">
        <h2>Now Serving</h2>
        {currentPatient ? (
          <div className="current-patient-card pulse">
            <span className="queue-display__number">{currentPatient.queueNumber}</span>
            <span className="queue-display__name">{currentPatient.patientName}</span>
          </div>
        ) : (
          <div className="no-patient">
            <span>No patient currently being served</span>
          </div>
        )}
      </div>

      <div className="queue-display__waiting">
        <h2>Up Next</h2>
        <div className="waiting-list-container">
          {waitingList.length > 0 ? (
            waitingList.map((patient, index) => (
              <div key={patient.queueNumber} className="queue-display__item">
                <span className="item-index">{index + 1}.</span>
                <span className="item-number">{patient.queueNumber}</span>
                <span className="item-name">{patient.patientName}</span>
              </div>
            ))
          ) : (
            <div className="no-waiting">
              <span>No patients waiting in the queue</span>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default QueueDisplayPage;
