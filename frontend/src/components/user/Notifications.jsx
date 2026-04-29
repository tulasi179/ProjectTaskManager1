import  { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import api from '../../api/axios'
import './Notifications.css'
import Navbar from '../shared/Navbar'
import { SkeletonNotif } from '../shared/Skeleton'

const Notifications = () => {
  const { user, authLoading } = useAuth()
  const navigate = useNavigate()

  const [notifications, setNotifications] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    if (authLoading) return
    if (!user || user.role !== 'User') { navigate('/login'); return }
    fetchNotifications()
  }, [authLoading, user])

  const fetchNotifications = async () => {
    try {
      const res = await api.get('/notification/my')
      const data = res.data.data || res.data
      setNotifications(Array.isArray(data) ? data : [])
    } catch (err) {
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  const handleMarkAsRead = async (id) => {
    try {
      await api.patch(`/notification/${id}/read`)
      // update locally without refetching
      setNotifications(prev =>
        prev.map(n => n.id === id ? { ...n, readStatus: true } : n)
      )
    } catch (err) {
      console.error(err)
    }
  }

  const handleMarkAllAsRead = async () => {
    try {
      const unread = notifications.filter(n => !n.readStatus)
      await Promise.all(unread.map(n => api.patch(`/notification/${n.id}/read`)))
      setNotifications(prev => prev.map(n => ({ ...n, readStatus: true })))
    } catch (err) {
      console.error(err)
    }
  }

  const unread = notifications.filter(n => !n.readStatus)
  const read = notifications.filter(n => n.readStatus)


if (authLoading || loading) return (
  <div className='notif-container'>
    <Navbar />
    <div className='notif-content'>
      <div className='notif-header'>
        <div>
          <div style={{height:'24px', width:'150px', borderRadius:'6px', background:'#e0e0e0', marginBottom:'8px'}}></div>
          <div style={{height:'16px', width:'80px', borderRadius:'6px', background:'#e0e0e0'}}></div>
        </div>
      </div>
      {[1,2,3,4,5].map(i => <SkeletonNotif key={i} />)}
    </div>
  </div>
)

  return (
    <div className='notif-container'>
      <Navbar />

      <div className='notif-content'>
        <div className='notif-header'>
          <div>
            <h2>Notifications</h2>
            <span className='notif-count'>{unread.length} unread</span>
          </div>
          {unread.length > 0 && (
            <button className='mark-all-btn' onClick={handleMarkAllAsRead}>
              Mark all as read
            </button>
          )}
        </div>

        {notifications.length === 0 ? (
          <div className='notif-empty'>
            <p>🔔 No notifications yet.</p>
          </div>
        ) : (
          <>
            {/* Unread */}
            {unread.length > 0 && (
              <div className='notif-section'>
                <h3 className='notif-section-title'>Unread</h3>
                {unread.map(n => (
                  <div key={n.id} className='notif-card unread'>
                    <div className='notif-dot' />
                    <div className='notif-body'>
                      <p className='notif-message'>{n.message}</p>
                      <div className='notif-footer'>
                        <span className='notif-time'>
                          {new Date(n.createdAt).toLocaleString()}
                        </span>
                        <button
                          className='mark-read-btn'
                          onClick={() => handleMarkAsRead(n.id)}>
                          Mark as read
                        </button>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}

            {/* Read */}
            {read.length > 0 && (
              <div className='notif-section'>
                <h3 className='notif-section-title'>Read</h3>
                {read.map(n => (
                  <div key={n.id} className='notif-card read'>
                    <div className='notif-body'>
                      <p className='notif-message'>{n.message}</p>
                      <span className='notif-time'>
                        {new Date(n.createdAt).toLocaleString()}
                      </span>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </>
        )}
      </div>
    </div>
  )
}

export default Notifications