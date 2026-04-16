// components/ProtectedRoute.jsx
import { Navigate } from 'react-router-dom'

const ProtectedRoute = ({ children, requiredRole }) => {
    const token = localStorage.getItem('token')
    const role = localStorage.getItem('role')

    // Not logged in → send to login page
    if (!token) {
        return <Navigate to='/login' />
    }

    // Wrong role → send to unauthorized page
    if (requiredRole && role !== requiredRole) {
        return <Navigate to='/unauthorized' />
    }

    // All good → show the page
    return children
}

export default ProtectedRoute