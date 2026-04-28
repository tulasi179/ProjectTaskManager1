import { Navigate } from 'react-router-dom'

const ProtectedRoute = ({ children, requiredRole }) => {
    const token = localStorage.getItem('token')
    const user = JSON.parse(localStorage.getItem('user'))
    const role = user?.role

    if (!token) {
        return <Navigate to='/login' />
    }

    if (requiredRole && role !== requiredRole) {
        return <Navigate to='/unauthorized' />
    }
    return children
}

export default ProtectedRoute