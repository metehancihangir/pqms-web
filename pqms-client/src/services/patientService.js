import api from './api';

export const patientService = {
  search: async (searchTerm, dateOfBirth) => {
    const params = {};
    if (searchTerm) params.searchTerm = searchTerm;
    if (dateOfBirth) params.dateOfBirth = dateOfBirth;

    const response = await api.get('/patients/search', { params });
    return response.data;
  }
};
