import api from './api';

export const adminService = {
  // --- Users ---
  getUsers: async () => {
    const response = await api.get('/admin/users');
    return response.data;
  },
  updateUserRole: async (id, data) => {
    const response = await api.put(`/admin/users/${id}/role`, data);
    return response.data;
  },
  toggleUserStatus: async (id) => {
    const response = await api.put(`/admin/users/${id}/status`);
    return response.data;
  },
  deleteUser: async (id) => {
    const response = await api.delete(`/admin/users/${id}`);
    return response.data;
  },

  // --- Visit Reasons ---
  getVisitReasons: async () => {
    const response = await api.get('/admin/visit-reasons');
    return response.data;
  },
  addVisitReason: async (data) => {
    const response = await api.post('/admin/visit-reasons', data);
    return response.data;
  },
  updateVisitReason: async (id, data) => {
    const response = await api.put(`/admin/visit-reasons/${id}`, data);
    return response.data;
  },
  deleteVisitReason: async (id) => {
    const response = await api.delete(`/admin/visit-reasons/${id}`);
    return response.data;
  },

  // --- Queue History ---
  getQueueHistory: async (filters) => {
    const response = await api.get('/admin/queue-history', { params: filters });
    return response.data;
  }
};
