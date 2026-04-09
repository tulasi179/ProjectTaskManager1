import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { useState, useEffect } from 'react'
import api from '../api/axios'
import './Navbar.css'

function Navbar() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const [unreadCount, setUnreadCount] = useState(0)
  const [showProfile, setShowProfile] = useState(false)
  const [profileData, setProfileData] = useState(null)
  const [loadingProfile, setLoadingProfile] = useState(false)
  const [showChangePassword, setShowChangePassword] = useState(false)
  const [pwForm, setPwForm] = useState({ currentPassword: '', newPassword: '', confirmPassword: '' })
  const [pwError, setPwError] = useState('')
  const [pwSuccess, setPwSuccess] = useState('')
  const [pwLoading, setPwLoading] = useState(false)

  // useEffect MUST be before any early return
  useEffect(() => {
    if (user?.role === 'User') {
      fetchUnreadCount()
    }
  }, [user])

  // if no user, return null AFTER all hooks
  if (!user) return null

  const fetchUnreadCount = async () => {
    try {
      const res = await api.get('/notification/my')
      const data = res.data.data || res.data
      const unread = Array.isArray(data) ? data.filter(n => !n.readStatus) : []
      setUnreadCount(unread.length)
    } catch (err) {
      console.error(err)
    }
  }

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  const openProfile = async () => {
    setShowProfile(true)
    setShowChangePassword(false)
    setPwError('')
    setPwSuccess('')
    setPwForm({ currentPassword: '', newPassword: '', confirmPassword: '' })

    if (!profileData) {
      setLoadingProfile(true)
      try {
        const res = await api.get(`/users/${user.id}`)
        setProfileData(res.data)
      } catch (err) {
        console.error(err)
      } finally {
        setLoadingProfile(false)
      }
    }
  }

  const handlePwChange = (e) => {
    setPwForm({ ...pwForm, [e.target.name]: e.target.value })
  }

  const handleChangePassword = async (e) => {
    e.preventDefault()
    setPwError('')
    setPwSuccess('')

    if (pwForm.newPassword !== pwForm.confirmPassword) {
      setPwError('New passwords do not match.')
      return
    }
    if (pwForm.newPassword.length < 6) {
      setPwError('New password must be at least 6 characters.')
      return
    }

    setPwLoading(true)
    try {
      await api.patch('/users/change-password', {
        currentPassword: pwForm.currentPassword,
        newPassword: pwForm.newPassword
      })
      setPwSuccess('Password changed successfully!')
      setPwForm({ currentPassword: '', newPassword: '', confirmPassword: '' })
      setShowChangePassword(false)
    } catch (err) {
      const msg = err.response?.data?.message || err.response?.data
      setPwError(typeof msg === 'string' ? msg : 'Failed to change password.')
    } finally {
      setPwLoading(false)
    }
  }

  const dashboardPath = user?.role === 'Admin' ? '/admindashboard' : '/userdashboard'

  return (
    <>
      <nav className='navbar'>
        <h1>Project Task Manager</h1>

        <div className='nav-links'>
          <Link to={dashboardPath}>Dashboard</Link>

          {user?.role === 'Admin' && (
            <Link to='/projects'>Projects</Link>
          )}

          {user?.role === 'User' && (
            <Link to='/notifications' className='notif-link'>
              Notifications
              {unreadCount > 0 && (
                <span className='notif-badge'>{unreadCount}</span>
              )}
            </Link>
          )}

          <span
            className='nav-user'
            onClick={openProfile}
            style={{ cursor: 'pointer' }}>
            👤 {user?.username}
          </span>
          <button onClick={handleLogout} className='logout-btn'>Logout</button>
        </div>
      </nav>

      {/* Profile Modal */}
      {showProfile && (
        <div className='modal-overlay' onClick={() => setShowProfile(false)}>
          <div className='modal profile-modal' onClick={e => e.stopPropagation()}>

            {/* Header */}
            <div className='profile-header'>
              <div className='profile-avatar'>
                {user.username?.charAt(0).toUpperCase()}
              </div>
              <div>
                <h3>{user.username}</h3>
                <span className='profile-role-badge'>{user.role}</span>
              </div>
              <button className='modal-close' onClick={() => setShowProfile(false)}>✕</button>
            </div>

            {/* Profile Info */}
            {loadingProfile ? (
              <p className='profile-loading'>Loading...</p>
            ) : profileData ? (
              <div className='profile-info'>
                <div className='profile-field'>
                  <label>Username</label>
                  <span>{profileData.username || profileData.Username}</span>
                </div>
                <div className='profile-field'>
                  <label>Email</label>
                  <span>{profileData.email || profileData.Email}</span>
                </div>
                <div className='profile-field'>
                  <label>Role</label>
                  <span>{profileData.role || profileData.Role}</span>
                </div>
              </div>
            ) : null}

            {/* Change Password Toggle */}
            <button
              className='change-pw-toggle'
              onClick={() => {
                setShowChangePassword(!showChangePassword)
                setPwError('')
                setPwSuccess('')
              }}>
              {showChangePassword ? '✕ Cancel' : '🔒 Change Password'}
            </button>

            {pwSuccess && <div className='pw-success'>{pwSuccess}</div>}

            {/* Change Password Form */}
            {showChangePassword && (
              <form onSubmit={handleChangePassword} className='pw-form'>
                {pwError && <div className='error-message'>{pwError}</div>}
                <div className='form-group'>
                  <label>Current Password</label>
                  <input
                    type='password'
                    name='currentPassword'
                    value={pwForm.currentPassword}
                    onChange={handlePwChange}
                    placeholder='Enter current password'
                    required
                  />
                </div>
                <div className='form-group'>
                  <label>New Password</label>
                  <input
                    type='password'
                    name='newPassword'
                    value={pwForm.newPassword}
                    onChange={handlePwChange}
                    placeholder='Enter new password'
                    required
                  />
                </div>
                <div className='form-group'>
                  <label>Confirm New Password</label>
                  <input
                    type='password'
                    name='confirmPassword'
                    value={pwForm.confirmPassword}
                    onChange={handlePwChange}
                    placeholder='Confirm new password'
                    required
                  />
                </div>
                <button type='submit' className='submit-btn' disabled={pwLoading}>
                  {pwLoading ? 'Updating...' : 'Update Password'}
                </button>
              </form>
            )}
          </div>
        </div>
      )}
    </>
  )
}

export default Navbar