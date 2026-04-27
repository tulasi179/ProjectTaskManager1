import  { useState, useEffect } from 'react'
import Navbar from '../shared/Navbar'
import { useAuth } from '../../context/AuthContext'
import { useNavigate } from 'react-router-dom'
import api from '../../api/axios'
import Footer from '../shared/Footer'

import StatCard from "../admin/StatCard"; 
import TasksTable from '../admin/TasksTable'
import Projects from '../admin/Projects'

const UserDashboard = () => {
    const {user , authLoading} =useAuth()
    const navigate = useNavigate()

    const [projects, setProjects] = useState([])
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
    }, [authLoading, user, navigate])

      const fetchAll= async () => {
         //console.log('Token in storage:', localStorage.getItem('token')) -> debugging
        try{
          const [tasksRes ,notifRes , projectsRes] = await Promise.all([
            api.get('/task/my'),
            api.get('/notification/my'),
            api.get('/project')
          ]) 
          //console.log("Tasks API:", tasksRes.data) ->debugging

          const taskData = tasksRes.data.data
          const  notifData = notifRes.data.data
          const projectsData = projectsRes.data.data || projectsRes.data

          setTasks(Array.isArray(taskData) ? taskData: [])
          setNotifications(Array.isArray(notifData) ? notifData : [])
          setProjects(Array.isArray(projectsData) ? projectsData : [])
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
      


      // get only the project that the user is assigned to.
       const userProjectIds = [...new Set(tasks.map(t => t.projectId))]
       const userProjects = projects.filter(p => userProjectIds.includes(p.id))

  return (
     <div className='min-h-screen bg-gray-100'>
      <Navbar />

      <div className='max-w-7xl mx-auto px-6 py-8'>
        
        {/* Welcome */}
        <div className='mb-8'>
          <h2 className='text-2xl font-bold text-gray-800'>
            Welcome , {user?.username}! ✌️
          </h2>
          {/* for the user or admin in the dashboard */}
          <span className='inline-block mt-1 py-1 px-3 bg-indigo-600 text-white text-xs rounded-full'>
            User
          </span>
        </div>

        {/* Stat Cards */}
        <div className='grid grid-cols-4 md:grid-cols-5 gap-4 mb-8'>
          {cards.map(card => (
            <StatCard key={card.label} {...card} />
          ))}
        </div>


      {/* Project Descriptions */}
        {userProjects.length > 0 && (
          <div className='mb-6'>
            <h3 className='text-lg font-semibold text-gray-800 mb-3'>My Projects</h3>
            <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
              {userProjects.map(p => (
                <div key={p.id} className='bg-white rounded-xl shadow-sm p-5 border-l-4 border-indigo-500  onClick={() => navigate(`/projects/${p.id}/tasks`)}'>
                  <h4 className='font-semibold text-gray-800'>{p.name}</h4>
                  {p.description && (
                    <p className='text-sm text-gray-500 mt-1'>{p.description}</p>
                  )}
                  <div className='flex gap-4 mt-2 text-xs text-gray-400'>
                    <span>📅 Start: {new Date(p.startDate).toLocaleDateString()}</span>
                    <span>📅 End: {new Date(p.endDate).toLocaleDateString()}</span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
          
         <TasksTable tasks={tasks} role="User" onStatusUpdate={fetchAll} />

        <div>
          {/* your existing code */}
          <Footer />
        </div>

      </div>
    </div>
  )
}

export default UserDashboard