import React, { useState, useEffect } from 'react'
import Navbar from '../Navbar'
import { useAuth } from '../../context/AuthContext'
import { useNavigate } from 'react-router-dom'
import api from '../../api/axios'

import StatCard from './Statcard'
import ProjectsSection from './ProjectsSection'
import TasksTable from './TasksTable'

const AdminDashboard = () => {
  const { user, authLoading } = useAuth()//user - current logged in user.
  const navigate = useNavigate()

  const [projects, setProjects] = useState([])//stroing list of projects
  const [tasks, setTasks] = useState([])//list of tasks
  const [users, setUsers] = useState([])//users
  const [notifications, setNotifications] = useState([])//noti
  const [loading, setLoading] = useState(true)//loading and disabling the button

  useEffect(() => {
    if (authLoading) return
    if (!user || user.role !== 'Admin') {
      navigate('/login')
      return
    }
    fetchAll()
  }, [authLoading, user, navigate])
  //component based side effect 
  // the effects run
  // When component mounts (first render)
  // Whenever any value in [authLoading, user, navigate] changes

  const fetchAll = async () => {
    try {
      const [projectsRes, tasksRes, usersRes, notifRes] = await Promise.all([
        api.get('/project'),
        api.get('/task'),
        api.get('/users'),
        api.get('/notification')
      ])


      //If i add paggination in my backend then it will be array i mean the datat that comes from the backend will be array
      // if not projectres.data
      // it handles both the cases.
      const projectsData = projectsRes.data.data || projectsRes.data
      const tasksData = tasksRes.data.data || tasksRes.data
      const usersData = usersRes.data.data || usersRes.data
      const notifData = notifRes.data.data || notifRes.data

      setProjects(Array.isArray(projectsData) ? projectsData : [])
      setTasks(Array.isArray(tasksData) ? tasksData : [])
      setUsers(Array.isArray(usersData) ? usersData : [])
      setNotifications(Array.isArray(notifData) ? notifData : [])
    } catch (err) {
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  //using the filter method for cards..
  const pendingTasks = tasks.filter(t => t.status === 'Pending')
  const inProgressTasks = tasks.filter(t => t.status === 'InProgress')
  const completedTasks = tasks.filter(t => t.status === 'Completed')
  const unreadNotifs = notifications.filter(n => !n.readStatus)

  if (authLoading || loading) {
    return (
      <div className='flex items-center justify-center h-screen text-gray-500 text-lg'>
        Loading...
      </div>
    )
  }



  //the cardssssssssss in dashboard.
  const cards = [
    { label: 'Projects', value: projects.length, color: 'border-indigo-500' },
    { label: 'Pending', value: pendingTasks.length, color: 'border-yellow-400' },
    { label: 'In Progress', value: inProgressTasks.length, color: 'border-blue-400' },
    { label: 'Completed', value: completedTasks.length, color: 'border-green-400' },
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
            Admin
          </span>
        </div>

        {/* Stat Cards */}
        <div className='grid grid-cols-2 md:grid-cols-5 gap-4 mb-8'>
          {cards.map(card => (
            <StatCard key={card.label} {...card} />
          ))}
        </div>

        {/* Projects */}
        <ProjectsSection projects={projects} navigate={navigate} />

        {/* Tasks */}
        <TasksTable tasks={tasks} users={users} />

      </div>
    </div>
  )
}

export default AdminDashboard