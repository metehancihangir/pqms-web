import api from './api';

export const appointmentService = {
  createAppointment: async (patientId, appointmentDate, reason, notes) => {
    const response = await api.post('/appointments', {
      patientId,
      appointmentDate,
      reason,
      notes
    });
    return response.data;
  },

  getPatientAppointments: async (patientId) => {
    const response = await api.get(`/appointments/patient/${patientId}`);
    return response.data;
  }
};
