import api from './api';

export const queueService = {
  getTodayQueue: async () => {
    const response = await api.get('/queue/today');
    return response.data;
  },

  callNextPatient: async () => {
    const response = await api.post('/queue/call-next');
    return response.data;
  },

  completePatient: async (id) => {
    const response = await api.put(`/queue/${id}/complete`);
    return response.data;
  },

  getVisitReasons: async () => {
    const response = await api.get('/visit-reasons');
    return response.data;
  },

  checkIn: async (data) => {
    const response = await api.post('/queue/checkin', data);
    return response.data;
  }
};
