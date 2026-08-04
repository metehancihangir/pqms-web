import { useState, useEffect } from 'react';
import { adminService } from '../services/adminService';
import '../styles/AdminPage.css';

const AdminPage = () => {
  const [activeTab, setActiveTab] = useState('users');
  
  // Data States
  const [users, setUsers] = useState([]);
  const [visitReasons, setVisitReasons] = useState([]);
  const [queueHistory, setQueueHistory] = useState([]);
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // Filters for Queue History
  const [filters, setFilters] = useState({ startDate: '', endDate: '', status: '' });

  // Visit Reason Modal State
  const [showVRModal, setShowVRModal] = useState(false);
  const [editingVR, setEditingVR] = useState({ id: null, name: '', isActive: true });

  // User Role Modal State
  const [showRoleModal, setShowRoleModal] = useState(false);
  const [editingRole, setEditingRole] = useState({ id: null, fullName: '', role: '', isActive: true });

  useEffect(() => {
    loadData();
  }, [activeTab]);

  const loadData = async () => {
    setLoading(true);
    setError('');
    try {
      if (activeTab === 'users') {
        const data = await adminService.getUsers();
        setUsers(data);
      } else if (activeTab === 'visitReasons') {
        const data = await adminService.getVisitReasons();
        setVisitReasons(data);
      } else if (activeTab === 'history') {
        const data = await adminService.getQueueHistory(filters);
        setQueueHistory(data);
      }
    } catch (err) {
      setError('Veriler yüklenirken bir hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  // --- User Handlers ---
  const handleToggleUserStatus = async (id) => {
    try {
      await adminService.toggleUserStatus(id);
      loadData();
    } catch (err) {
      alert('Hata oluştu.');
    }
  };

  const handleDeleteUser = async (id) => {
    if (window.confirm("Bu kullanıcıyı silmek istediğinize emin misiniz? (Bu işlem geri alınamaz)")) {
      try {
        await adminService.deleteUser(id);
        loadData();
      } catch (err) {
        alert('Silme başarısız.');
      }
    }
  };

  const openRoleModal = (user) => {
    setEditingRole({ id: user.id, fullName: user.fullName, role: user.role || 'Patient', isActive: user.isActive });
    setShowRoleModal(true);
  };

  const handleSaveRole = async () => {
    try {
      await adminService.updateUserRole(editingRole.id, { role: editingRole.role, isActive: editingRole.isActive });
      setShowRoleModal(false);
      loadData();
    } catch (err) {
      alert('Kayıt başarısız.');
    }
  };

  // --- Visit Reason Handlers ---
  const openVRModal = (vr = null) => {
    if (vr) {
      setEditingVR({ id: vr.id, name: vr.name, isActive: vr.isActive });
    } else {
      setEditingVR({ id: null, name: '', isActive: true });
    }
    setShowVRModal(true);
  };

  const handleSaveVR = async () => {
    try {
      if (editingVR.id) {
        await adminService.updateVisitReason(editingVR.id, { id: editingVR.id, name: editingVR.name, isActive: editingVR.isActive });
      } else {
        await adminService.addVisitReason({ name: editingVR.name, isActive: editingVR.isActive });
      }
      setShowVRModal(false);
      loadData();
    } catch (err) {
      alert('Kayıt başarısız.');
    }
  };

  const handleDeleteVR = async (id) => {
    if (window.confirm("Bu sebebi silmek istediğinize emin misiniz? (Soft delete uygulanır)")) {
      try {
        await adminService.deleteVisitReason(id);
        loadData();
      } catch (err) {
        alert('Silme başarısız.');
      }
    }
  };

  return (
    <div className="admin-container">
      <div className="admin-sidebar">
        <h2 className="admin-logo">Admin Panel</h2>
        <ul className="admin-menu">
          <li className={activeTab === 'users' ? 'active' : ''} onClick={() => setActiveTab('users')}>Users</li>
          <li className={activeTab === 'visitReasons' ? 'active' : ''} onClick={() => setActiveTab('visitReasons')}>Visit Reasons</li>
          <li className={activeTab === 'history' ? 'active' : ''} onClick={() => setActiveTab('history')}>Queue History</li>
        </ul>
      </div>

      <div className="admin-content">
        <div className="admin-header">
          <h1>{activeTab.charAt(0).toUpperCase() + activeTab.slice(1).replace(/([A-Z])/g, ' $1')} Management</h1>
          {error && <div className="admin-error">{error}</div>}
        </div>

        {loading ? (
          <div className="admin-loading">Yükleniyor...</div>
        ) : (
          <div className="admin-card">
            {activeTab === 'users' && (
              <table className="admin-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Name</th>
                    <th>Email</th>
                    <th>Role</th>
                    <th>Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {users.map(u => (
                    <tr key={u.id}>
                      <td>{u.id}</td>
                      <td>{u.fullName}</td>
                      <td>{u.email}</td>
                      <td><span className={`badge role-${(u.role || 'Patient').toLowerCase()}`}>{u.role || 'Patient'}</span></td>
                      <td>
                        <span className={`badge status-${u.isActive ? 'active' : 'inactive'}`}>
                          {u.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </td>
                      <td style={{ display: 'flex', gap: '8px', flexWrap: 'wrap' }}>
                        <button className="btn-sm btn-outline" onClick={() => openRoleModal(u)}>Edit Role</button>
                        <button className="btn-sm btn-outline-danger" onClick={() => handleToggleUserStatus(u.id)}>Toggle Status</button>
                        <button className="btn-sm btn-outline-danger" onClick={() => handleDeleteUser(u.id)}>Delete</button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}

            {activeTab === 'visitReasons' && (
              <div>
                <button className="btn-primary mb-4" onClick={() => openVRModal()}>+ Add New Reason</button>
                <table className="admin-table">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Name</th>
                      <th>Status</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {visitReasons.map(vr => (
                      <tr key={vr.id}>
                        <td>{vr.id}</td>
                        <td>{vr.name}</td>
                        <td>
                          <span className={`badge status-${vr.isActive ? 'active' : 'inactive'}`}>
                            {vr.isActive ? 'Active' : 'Inactive'}
                          </span>
                        </td>
                        <td>
                          <button className="btn-sm btn-outline" onClick={() => openVRModal(vr)}>Edit</button>
                          {vr.isActive && <button className="btn-sm btn-outline-danger" onClick={() => handleDeleteVR(vr.id)}>Delete</button>}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}

            {activeTab === 'history' && (
              <div>
                <div className="filters-bar">
                  <span style={{alignSelf:'center', fontWeight:'600', color:'#475569'}}>From:</span>
                  <input type="date" value={filters.startDate} onChange={e => setFilters({...filters, startDate: e.target.value})} />
                  <span style={{alignSelf:'center', fontWeight:'600', color:'#475569'}}>To:</span>
                  <input type="date" value={filters.endDate} onChange={e => setFilters({...filters, endDate: e.target.value})} />
                  <select value={filters.status} onChange={e => setFilters({...filters, status: e.target.value})}>
                    <option value="">All Statuses</option>
                    <option value="Waiting">Waiting</option>
                    <option value="InProgress">InProgress</option>
                    <option value="Completed">Completed</option>
                  </select>
                  <button className="btn-primary" onClick={loadData}>Filter</button>
                </div>

                <table className="admin-table">
                  <thead>
                    <tr>
                      <th>Date</th>
                      <th>Queue No</th>
                      <th>Patient</th>
                      <th>Type</th>
                      <th>Reason</th>
                      <th>Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {queueHistory.map(q => (
                      <tr key={q.id}>
                        <td>{new Date(q.queueDate).toLocaleDateString()}</td>
                        <td><strong>{q.queueNumber}</strong></td>
                        <td>{q.patientName}</td>
                        <td>{q.checkInType}</td>
                        <td>{q.visitReason}</td>
                        <td><span className={`badge status-${q.status.toLowerCase()}`}>{q.status}</span></td>
                      </tr>
                    ))}
                    {queueHistory.length === 0 && <tr><td colSpan="6" style={{textAlign:'center'}}>No records found.</td></tr>}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        )}
      </div>

      {/* --- Modals --- */}
      {showVRModal && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h3>{editingVR.id ? 'Edit Visit Reason' : 'Add Visit Reason'}</h3>
            <div className="form-group">
              <label>Name</label>
              <input type="text" value={editingVR.name} onChange={e => setEditingVR({...editingVR, name: e.target.value})} />
            </div>
            <div className="form-group checkbox-group">
              <label>
                <input type="checkbox" checked={editingVR.isActive} onChange={e => setEditingVR({...editingVR, isActive: e.target.checked})} />
                Is Active
              </label>
            </div>
            <div className="modal-actions">
              <button className="btn-outline" onClick={() => setShowVRModal(false)}>Cancel</button>
              <button className="btn-primary" onClick={handleSaveVR}>Save</button>
            </div>
          </div>
        </div>
      )}

      {showRoleModal && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h3>Edit User Role</h3>
            <p className="mb-4">User: <strong>{editingRole.fullName}</strong></p>
            <div className="form-group">
              <label>Role</label>
              <select value={editingRole.role} onChange={e => setEditingRole({...editingRole, role: e.target.value})}>
                <option value="Patient">Patient</option>
                <option value="Doctor">Doctor</option>
                <option value="Admin">Admin</option>
              </select>
            </div>
            <div className="form-group checkbox-group">
              <label>
                <input type="checkbox" checked={editingRole.isActive} onChange={e => setEditingRole({...editingRole, isActive: e.target.checked})} />
                Account Active
              </label>
            </div>
            <div className="modal-actions">
              <button className="btn-outline" onClick={() => setShowRoleModal(false)}>Cancel</button>
              <button className="btn-primary" onClick={handleSaveRole}>Save</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminPage;
