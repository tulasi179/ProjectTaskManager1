import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import api from '../../api/axios'

export const useProjectTasks = (projectId) => {
  const { user } = useAuth()
  const navigate = useNavigate()
  const isAdmin = user?.role === 'Admin'

  const [project, setProject] = useState(null)
  const [tasks, setTasks] = useState([])
  const [users, setUsers] = useState([])
  const [dependencies, setDependencies] = useState([])
  const [loading, setLoading] = useState(true)

  const fetchAll = useCallback(async () => {
    try {
      const [projectRes, tasksRes] = await Promise.all([
        api.get(`/project/${projectId}`),
        api.get(`/task/project/${projectId}`).catch(e =>
          e.response?.status === 404 ? { data: [] } : Promise.reject(e)
        )
      ])
      setProject(projectRes.data)
      const allTasks = Array.isArray(tasksRes.data) ? tasksRes.data : []

      if (isAdmin) {
        setTasks(allTasks)
        const [usersRes, depsRes] = await Promise.all([
          api.get('/users'),
          api.get('/taskdependency')
        ])
        setUsers(Array.isArray(usersRes.data) ? usersRes.data : usersRes.data.data || [])
        setDependencies(Array.isArray(depsRes.data) ? depsRes.data : [])
      } else {
        const myTasks = allTasks.filter(t => t.assigneeId === user.id)
        setTasks(myTasks)
        const depResults = await Promise.all(
          myTasks.map(t =>
            api.get(`/taskdependency/${t.id}/dependents`).catch(() => ({ data: [] }))
          )
        )
        setDependencies(depResults.flatMap(r => Array.isArray(r.data) ? r.data : []))
      }
    } catch (err) {
      console.error(err)
    } finally {
      setLoading(false)
    }
  }, [projectId, isAdmin, user?.id])

  useEffect(() => {
    if (!user) { navigate('/login'); return }
    fetchAll()
  }, [projectId])

  const createTask = (payload) => api.post('/task', payload)
  const updateTask = (id, payload) => api.put(`/task/${id}`, payload)
  const deleteTask = (id) => api.delete(`/task/${id}`)
  const updateStatus = (id, nextStatus) =>
    api.patch(`/task/${id}/status`, { Status: nextStatus })

  return { project, tasks, users, dependencies, loading, isAdmin, fetchAll,
           createTask, updateTask, deleteTask, updateStatus }
}