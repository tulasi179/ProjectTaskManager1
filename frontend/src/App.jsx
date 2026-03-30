import React from 'react'
import Register from './Register'
import Login from './Login'
import {BrowserRouter, Routes, Route, Navigate} from 'react-router-dom'
import Dashboard from './Dashboard'
import { AuthProvider } from './context/AuthContext'
import './App.css'

 const App = () => {
  return (
    <AuthProvider>
    <BrowserRouter>
     <Routes>
      <Route path='/register' element={<Register/>}/>
      <Route path='/login' element={<Login/>}/>
      <Route path = '/dashboard' element={<Dashboard/>}/>
      <Route path='*' element={<Navigate to="/register" />} />
     </Routes>
    </BrowserRouter>
    </AuthProvider>
  )
}

export default App