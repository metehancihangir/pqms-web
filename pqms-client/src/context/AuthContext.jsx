import { createContext, useState, useEffect, useContext } from 'react';
import { authService } from '../services/authService';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem('token') || sessionStorage.getItem('token');
    const fullName = localStorage.getItem('fullName') || sessionStorage.getItem('fullName');
    const role = localStorage.getItem('role') || sessionStorage.getItem('role');

    if (token && fullName && role) {
      setUser({ token, fullName, role });
    }
    setLoading(false);
  }, []);

  const login = async (email, password, rememberMe = false) => {
    const data = await authService.login(email, password);
    const storage = rememberMe ? localStorage : sessionStorage;

    // Clear old data just in case
    localStorage.removeItem('token'); localStorage.removeItem('fullName'); localStorage.removeItem('role');
    sessionStorage.removeItem('token'); sessionStorage.removeItem('fullName'); sessionStorage.removeItem('role');

    storage.setItem('token', data.token);
    storage.setItem('fullName', data.fullName);
    storage.setItem('role', data.role);
    setUser(data);
    return data;
  };

  const register = async (fullName, email, password, role) => {
    const data = await authService.register(fullName, email, password, role);
    localStorage.setItem('token', data.token);
    localStorage.setItem('fullName', data.fullName);
    localStorage.setItem('role', data.role);
    setUser(data);
    return data;
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('fullName');
    localStorage.removeItem('role');
    
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('fullName');
    sessionStorage.removeItem('role');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, register, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
