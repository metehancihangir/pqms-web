import { createContext, useState, useEffect, useContext } from 'react';
import { authService } from '../services/authService';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem('token');
    const fullName = localStorage.getItem('fullName');
    const role = localStorage.getItem('role');

    if (token && fullName && role) {
      setUser({ token, fullName, role });
    }
    setLoading(false);
  }, []);

  const login = async (email, password) => {
    const data = await authService.login(email, password);
    localStorage.setItem('token', data.token);
    localStorage.setItem('fullName', data.fullName);
    localStorage.setItem('role', data.role);
    setUser(data);
  };

  const register = async (fullName, email, password, role) => {
    const data = await authService.register(fullName, email, password, role);
    localStorage.setItem('token', data.token);
    localStorage.setItem('fullName', data.fullName);
    localStorage.setItem('role', data.role);
    setUser(data);
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('fullName');
    localStorage.removeItem('role');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, register, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
