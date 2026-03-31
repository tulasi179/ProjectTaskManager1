import React, { useState, useEffect } from 'react'
import Navbar from '../Navbar'
import { useAuth } from '../../context/AuthContext'
import { useNavigate } from 'react-router-dom'
import api from '../../api/axios'

import StatCard from '../admin/StatCard'
import TasksTable from '../admin/TasksTable'

const UserDashboard = () => {
    const {user , authLoading} =useAuth()
    const navigate = useNavigate()

   
    const [tasks, setTasks] = useState([])
    const [notifications, setNotifications] = useState([])
    const [loading, setLoading] = useState(true)//default true..


    useEffect(() => {
      if(authLoading) return
      if(!user || user.role!=='User'){
        navigate('/login')
        return
      }
      fetchAll()
    }, [authLoading])

      const fetchAll= async () => {
        try{
          const [tasksRes, notifRes] = await Promise.all([
            api.get('/task/my'),
            api.get('/notification/my')
          ]) 

          const taskData = tasksRes.data.data
          const  notifData = notifRes.data.data

          setTasks(Array.isArray(taskData) ? taskData: [])
          setNotifications(Array.isArray(notifRes) ? notifdata : [])
        }
        catch (err)
        {
          console.log(err)
        } finally {
          setLoading(false)
        }
      }


      const pendingTasks = tasks.filter(t=> t.status === 'Pending')
      const inProgressTasks = tasks.filter(t => t.status === 'InProgress')
      const completedTasks = tasks.filter(t => t.status === 'Completed')
      const unreadNotifs = notifications.filter(n => !n.readStatus)

      if(authLoading || loading)
      {
        return (
          <div className='flex items-center justify-center h-screen text-gray-500 text-lg'>
            Loading....
          </div>
        )
      }
  

      const cards = [
        {label: 'Pending tasks', value :pendingTasks.length, color : 'border-indigo-500'},
        {label: 'In Progress' , value :inProgressTasks.length, color: 'border-yellow-400'},
        {label : 'completed tasks' , value : completedTasks.length, color :'border-blue-400'},
        { label: 'Unread Notifs', value: unreadNotifs.length, color: 'border-red-400' }
      ]
      

  return (
     <div className='min-h-screen bg-gray-100'>
      <Navbar />

      <div className='max-w-7xl mx-auto px-6 py-8'>
        
        {/* Welcome */}
        <div className='mb-8'>
          <h2 className='text-2xl font-bold text-gray-800'>
            Welcome back, {user?.username}! ✌️
          </h2>
          {/* for the user or admin in the dashboard */}
          <span className='inline-block mt-1 py-1 px-3 bg-indigo-600 text-white text-xs rounded-full'>
            User
          </span>
        </div>

        {/* Stat Cards */}
        <div className='grid grid-cols-2 md:grid-cols-5 gap-4 mb-8'>
          {cards.map(card => (
            <StatCard key={card.label} {...card} />
          ))}
        </div>


      </div>
    </div>
  )
}

export default UserDashboard