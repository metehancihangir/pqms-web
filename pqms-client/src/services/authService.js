import api from './api';

export const authService = {
  login: async (email, password) => {
    const response = await api.post('/auth/login', { email, password });
    return response.data;
  },

  register: async (fullName, email, password, role = 'Patient') => {
    const response = await api.post('/auth/register', { fullName, email, password, role });
    return response.data;
  }
};
