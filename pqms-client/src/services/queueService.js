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

  completePatient: async (queueId) => {
    const response = await api.put(`/queue/${queueId}/complete`);
    return response.data;
  }
};
