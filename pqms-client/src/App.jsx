import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import AuthPage from './pages/AuthPage';
import DashboardPage from './pages/DashboardPage';
import PatientWelcomePage from './pages/PatientWelcomePage';
import PatientSearchPage from './pages/PatientSearchPage';
import PatientCheckInPage from './pages/PatientCheckInPage';
import ProtectedRoute from './components/ProtectedRoute';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<AuthPage />} />
        
        {/* Korumalı Rotalar */}
        <Route element={<ProtectedRoute />}>
          <Route path="/dashboard" element={<DashboardPage />} />
        </Route>

        {/* Hasta Yönlendirme (Kiosk/Public Rotaları) */}
        <Route path="/patient/welcome" element={<PatientWelcomePage />} />
        <Route path="/patient/search" element={<PatientSearchPage />} />
        <Route path="/patient/checkin/:patientId" element={<PatientCheckInPage />} />
        <Route path="/patient/register" element={<div style={{padding: '50px', textAlign: 'center'}}><h2>Patient Register Page (Faz 5 Placeholder)</h2></div>} />

        {/* Varsayılan Rota */}
        <Route path="*" element={<Navigate to="/patient/welcome" replace />} />
      </Routes>
    </Router>
  );
}

export default App;
