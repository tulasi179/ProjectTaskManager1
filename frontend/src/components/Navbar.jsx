import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import './Navbar.css'

function Navbar() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <nav className='navbar'>
      <h1>Project Task Manager</h1>
      
      <div className='nav-links'>
        <Link to='/dashboard'>Dashboard</Link>

        {user?.role === 'Admin' && (
          <Link to='/projects'>Projects</Link>
        )}

        <Link to='/notifications'>Notifications</Link>

        <span className='nav-user'>👤 {user?.username}</span>
        <button onClick={handleLogout} className='logout-btn'>Logout</button>
      </div>
    </nav>
  )
}

export default Navbar