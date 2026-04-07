import React from 'react'
import Register from './Register'
import Login from './Login'
import {BrowserRouter, Routes, Route, Navigate} from 'react-router-dom'
//import Dashboard from './components/Dashboard/Dashboard'
import AdminDashboard from './components/admin/AdminDashboard'
import UserDashboard from './components/user/UserDashboard'
import { AuthProvider } from './context/AuthContext'
import './App.css'
import Notifications from './Notifications'
import Projects from './components/admin/Projects'
import ProjectTasks from './components/admin/ProjectTasks' 
import { useAuth } from './context/AuthContext'


 const App = () => {
  //  const { user } = useAuth()
  // console.log('App level user:', user)
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
            <Route path='/' element={<Navigate to='/login'/>}/>
            <Route path='/register' element={<Register/>}/>
            <Route path='/login' element={<Login/>}/>
            <Route path ='/notifications' element = {<Notifications/>}/>
            <Route path='/admindashboard' element={<AdminDashboard/>} />
            <Route path='/userdashboard' element={<UserDashboard />} />
            <Route path='/projects' element={<Projects/>}/>
            <Route path='/projects/:id/tasks' element={<ProjectTasks/>}/>

            <Route path='*' element={<Navigate to='/login' />} />
          {/* unknown url will route to login */}
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App