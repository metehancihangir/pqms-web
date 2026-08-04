import { useState, useEffect } from 'react';
import Navbar from '../components/Navbar';
import QueueColumn from '../components/QueueColumn';
import { queueService } from '../services/queueService';
import '../styles/DashboardPage.css';

const DashboardPage = () => {
  const [queueEntries, setQueueEntries] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [isCalling, setIsCalling] = useState(false);

  const fetchTodayQueue = async () => {
    try {
      const data = await queueService.getTodayQueue();
      setQueueEntries(data);
      setError('');
    } catch (err) {
      setError('Kuyruk verileri yüklenemedi. Lütfen sayfayı yenileyin.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTodayQueue();
  }, []);

  const handleCallNext = async () => {
    setIsCalling(true);
    try {
      await queueService.callNextPatient();
      await fetchTodayQueue();
    } catch (err) {
      setError(err.response?.data?.message || 'Sıradaki hasta çağrılamadı.');
    } finally {
      setIsCalling(false);
    }
  };

  const handleCompletePatient = async (queueId) => {
    try {
      await queueService.completePatient(queueId);
      await fetchTodayQueue();
    } catch (err) {
      setError(err.response?.data?.message || 'Hasta muayenesi tamamlanamadı.');
    }
  };

  const formatDate = (date) => {
    return new Intl.DateTimeFormat('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    }).format(date);
  };

  const waitingPatients = queueEntries.filter(q => q.status === 'Waiting');
  const inProgressPatients = queueEntries.filter(q => q.status === 'InProgress');
  const completedPatients = queueEntries.filter(q => q.status === 'Completed');

  return (
    <div className="dashboard-container">
      <Navbar />
      
      <main className="dashboard-main">
        <div className="dashboard-header">
          <div>
            <h1 className="dashboard-title">Today's Queue</h1>
            <p className="dashboard-date">{formatDate(new Date())}</p>
          </div>
          
          <button 
            className="btn-call-next" 
            onClick={handleCallNext}
            disabled={isCalling || waitingPatients.length === 0}
          >
            {isCalling ? 'Calling...' : 'Call Next Patient'}
          </button>
        </div>

        {error && <div className="dashboard-error">{error}</div>}

        {loading ? (
          <div className="dashboard-loading">Yükleniyor...</div>
        ) : (
          <div className="queue-board">
            <QueueColumn 
              title="Waiting Room" 
              color="blue" 
              patients={waitingPatients} 
            />
            <QueueColumn 
              title="In Progress" 
              color="yellow" 
              patients={inProgressPatients} 
              onCompletePatient={handleCompletePatient}
            />
            <QueueColumn 
              title="Completed" 
              color="green" 
              patients={completedPatients} 
            />
          </div>
        )}
      </main>
    </div>
  );
};

export default DashboardPage;
