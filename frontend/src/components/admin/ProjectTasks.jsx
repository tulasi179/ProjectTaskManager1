import React, { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
//params for url like taks/5 , id is 5
import { useAuth } from '../../context/AuthContext'
import Navbar from '../Navbar'
import api from '../../api/axios'
import './ProjectTasks.css'

const ProjectTasks = () => {
  const { id } = useParams()//project id
  const { user } = useAuth()
  const navigate = useNavigate()

  const [tasks, setTasks] = useState([])
  const [project, setProject] = useState(null)

  const [users, setUsers] = useState([])
  const [dependencies, setDependencies] = useState([])

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [depError, setDepError] = useState('')

  const [showModal, setShowModal] = useState(false)
  const [editTask, setEditTask] = useState(null)
  const [existingDeps, setExistingDeps] = useState([])
  const [selectedDep, setSelectedDep] = useState('')
  const [form, setForm] = useState({
    title: '',
    description: '',
    assigneeId: '',
    projectId: id
  })

  const isAdmin = user?.role === 'Admin'//optional chaining ?.
  //if user is null its undefined and if exists then get the role of the user and it it is admin it true

  useEffect(() => {
    if (!user) { navigate('/login'); return }
    fetchAll()
  }, [id])
  //rerenders

  const fetchAll = async () => {
    try {
      const [projectRes, tasksRes] = await Promise.all([
        api.get(`/project/${id}`),
        api.get(`/task/project/${id}`).catch(err => {
          if (err.response?.status === 404) return { data: [] }
          throw err
        })
      ])

      setProject(projectRes.data)
      const taskData = tasksRes.data
      const allTasks = Array.isArray(taskData) ? taskData : []

      if (isAdmin) {
        //admin can see all tasks
        setTasks(allTasks)
        const [usersRes, depsRes] = await Promise.all([
          api.get('/users'),
          api.get('/taskdependency')
        ])
        setUsers(Array.isArray(usersRes.data) ? usersRes.data : usersRes.data.data || [])
        setDependencies(Array.isArray(depsRes.data) ? depsRes.data : [])
      } else {
        //for user only their task are visable
        const myTasks = allTasks.filter(t => t.assigneeId === user.id)
        //     //debugging
        // console.log('allTasks:', allTasks)        // check all tasks
        // console.log('user.id:', user.id)          // check user id
        // console.log('myTasks:', myTasks)    
        setTasks(myTasks)

        // fetch dep for each of user's tasks
        const depResults = await Promise.all(
          myTasks.map(t =>
            api.get(`/taskdependency/${t.id}/dependents`).catch(() => ({ data: [] }))
          )
        )
        const allDeps = depResults.flatMap(r => Array.isArray(r.data) ? r.data : [])
        console.log('User deps:', allDeps) // ← check this in console
        setDependencies(allDeps)
      }

    } catch (err) {
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  // blocker task ids for a given task
  const getBlockingTasks = (taskId) => {
    return dependencies
      .filter(d => d.dependentTaskId === taskId)
      //checks is the taskid and dependenttask id are same
      .map(d => d.taskId)
  }

  // checks any blocker is not completed
  const isTaskBlocked = (taskId) => {
    const blockingIds = getBlockingTasks(taskId)
    if (!blockingIds.length) return false
    return blockingIds.some(bid => {
      const blockingTask = tasks.find(t => t.id === bid)
      return blockingTask && blockingTask.status !== 'Completed'
    })
  }

  // get incomplete blocking tasks
  const getBlockingNames = (taskId) => {
    const blockingIds = getBlockingTasks(taskId)
    return blockingIds
      .map(bid => tasks.find(t => t.id === bid))
      .filter(t => t && t.status !== 'Completed')
      .map(t => t.title)
  }

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value })
  }

  const openCreateModal = () => {
    setEditTask(null)
    setExistingDeps([])
    setSelectedDep('')
    setDepError('')
    setForm({ title: '', description: '', assigneeId: '', projectId: id })
    setError('')
    setShowModal(true)
  }

  const openEditModal = async (task) => {
    setEditTask(task)
    setSelectedDep('')
    setDepError('')
    setForm({
      title: task.title,
      description: task.description,
      assigneeId: task.assigneeId,
      projectId: id
    })

    try {
      const depRes = await api.get(`/taskdependency/${task.id}/dependents`)
      setExistingDeps(Array.isArray(depRes.data) ? depRes.data : [])
    } catch {
      setExistingDeps([])
    }

    setError('')
    setShowModal(true)
  }

  const handleAddDependency = async () => {
    if (!selectedDep) return
    setDepError('')
    try {
      await api.post('/taskdependency', {
        taskId: parseInt(selectedDep), //this task should blocbe completed first
        dependentTaskId: editTask.id   // until then you cant complete this 
      })
      const depRes = await api.get(`/taskdependency/${editTask.id}/dependents`)
      setExistingDeps(Array.isArray(depRes.data) ? depRes.data : [])
      setSelectedDep('')
      fetchAll() // refresh dependencies in table too
    } catch (err) {
      const msg = err.response?.data?.message || err.response?.data
      setDepError(typeof msg === 'string' ? msg : 'Failed to add dependency.')
    }
  }

  const handleRemoveDependency = async (taskId, dependentTaskId) => {
    try {
      await api.delete(`/taskdependency/${taskId}/${dependentTaskId}`)
      setExistingDeps(prev => prev.filter(d => d.taskId !== taskId))
      //removes one dependency from the existingDeps state
      fetchAll() // refresh dependencies in table too
    } catch (err) {
      setError('Failed to remove dependency.')
    }
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    try {
      const payload = {
        ...form,
        projectId: parseInt(form.projectId),
        assigneeId: parseInt(form.assigneeId),
      }

      let savedTask
      if (editTask) {
        await api.put(`/task/${editTask.id}`, payload)
        savedTask = editTask
      } else {
        const res = await api.post('/task', payload)
        savedTask = res.data

        // only add dependency on CREATE, edit 
        if (selectedDep) {
          await api.post('/taskdependency', {
            taskId: parseInt(selectedDep),
            dependentTaskId: savedTask.id
          }).catch(err => {
            const msg = err.response?.data?.message || err.response?.data
            setDepError(typeof msg === 'string' ? msg : 'Failed to add dependency.')
          })
        }
      }

      setShowModal(false)
      fetchAll()
    } catch (err) {
      setError(err.response?.data?.message || 'Something went wrong. Please try again later.')
    }
  }

  const handleDelete = async (taskId) => {
    if (!window.confirm('Are you sure you want to delete this task?')) return
    try {
      await api.delete(`/task/${taskId}`)
      fetchAll()
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to delete task.')
    }
  }

  const handleStatusUpdate = async (taskId, currentStatus) => {
    const nextStatus = currentStatus === 'Pending' ? 'InProgress' : 'Completed'
    try {
      await api.patch(`/task/${taskId}/status`, { Status: nextStatus })
      fetchAll()
    } catch (err) {

         console.log(err.response?.data) 
      alert(err.response?.data|| 'Failed to update status.')
    }
  }

  const getStatusClass = (status) => {
    if (status === 'Pending') return 'status-pending'
    if (status === 'InProgress') return 'status-inprogress'
    if (status === 'Completed') return 'status-completed'
    return ''
  }

  if (loading) return <div className='loading'>Loading...</div>

  return (
    <div className='tasks-container'>
      <Navbar />

      <div className='tasks-content'>

        {/* Project Info */}
        {project && (
          <div className='project-info'>
            <button className='back-btn' onClick={() => navigate('/projects')}>
              ← Back to Projects
            </button>
            <h2>{project.name}</h2>
            {project.description && (
              <p className='project-desc'>{project.description}</p>
            )}
            <div className='project-dates'>
              <span>📅 Start: {new Date(project.startDate).toLocaleDateString()}</span>
              <span>📅 End: {new Date(project.endDate).toLocaleDateString()}</span>
            </div>
          </div>
        )}

        {/* Page Header */}
        <div className='page-header'>
          <h3>Tasks</h3>
          {isAdmin && (
            <button className='create-btn' onClick={openCreateModal}>+ New Task</button>
          )}
        </div>

        {/* Tasks Table */}
        {tasks.length === 0 ? (
          <div className='empty-state'>No tasks found.</div>
        ) : (
          <div className='tasks-table-wrapper'>
            <table className='tasks-table'>
              <thead>
                <tr>
                  <th>Title</th>
                  <th>Description</th>
                  {isAdmin && <th>Assigned To</th>}
                  <th>Status</th>
                  <th>Blocked By</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {tasks.map(t => {
                  const assignee = users.find(u => u.id === t.assigneeId)
                  const isCompleted = t.status === 'Completed'
                  const blocked = isTaskBlocked(t.id)
                  const blockingNames = getBlockingNames(t.id)

                  return (
                    <tr key={t.id} className={blocked ? 'blocked-row' : ''}>
                      <td>{t.title}</td>
                      <td>{t.description}</td>
                      {isAdmin && (
                        <td>{assignee?.username || `User #${t.assigneeId}`}</td>
                      )}
                      <td>
                        <span className={`status-badge ${getStatusClass(t.status)}`}>
                          {t.status}
                        </span>
                      </td>
                      <td>
                        {blockingNames.length > 0 ? (
                          <span className='blocked-badge' title={blockingNames.join(', ')}>
                            🔒 {blockingNames.join(', ')}
                          </span>
                        ) : (
                          <span className='clear-badge'>✓ Clear</span>
                        )}
                      </td>
                      <td className='action-btns'>
                        {isAdmin && (
                          <>
                            <button className='edit-btn' onClick={() => openEditModal(t)}>Edit</button>
                            <button className='delete-btn' onClick={() => handleDelete(t.id)}>Delete</button>
                          </>
                        )}


                        {!isAdmin && !isCompleted && !blocked && (
                          <button
                            className='status-btn'
                            onClick={() => handleStatusUpdate(t.id, t.status)}>
                            Move to {t.status === 'Pending' ? 'In Progress' : 'Completed'}
                          </button>
                        )}
                        {!isAdmin && !isCompleted && blocked && (
                          <span className='blocked-status-btn'>🔒 Blocked</span>
                        )}
                        {!isAdmin && isCompleted && (
                          <span className='done-label'>Done ✓</span>
                        )}
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Modal - Admin only */}
      {showModal && isAdmin && (
        <div className='modal-overlay' onClick={() => setShowModal(false)}>
          <div className='modal' onClick={e => e.stopPropagation()}>
            <h3>{editTask ? 'Edit Task' : 'Create New Task'}</h3>

            {error && <div className='error-message'>{error}</div>}

            <form onSubmit={handleSubmit}>
              <div className='form-group'>
                <label>Title</label>
                <input
                  type='text'
                  name='title'
                  value={form.title}
                  onChange={handleChange}
                  placeholder='Enter task title'
                  required
                />
              </div>

              <div className='form-group'>
                <label>Description</label>
                <textarea
                  name='description'
                  value={form.description}
                  onChange={handleChange}
                  placeholder='Enter task description'
                  rows={3}
                  required
                />
              </div>

              <div className='form-group'>
                <label>Assign To</label>
                <select
                  name='assigneeId'
                  value={form.assigneeId}
                  onChange={handleChange}
                  required>
                  <option value=''>Select a user</option>
                  {users.map(u => (
                    <option key={u.id} value={u.id}>{u.username}</option>
                  ))}
                </select>
              </div>

              {/* Dependencies */}
              <div className='form-group'>
                <label>Blocked By <span className='optional'>(must complete first)</span></label>

                {depError && <div className='error-message'>{depError}</div>}

                {/* Show existing dependencies when editing */}
                {existingDeps.length > 0 && (
                  <div className='existing-deps'>
                    {existingDeps.map(dep => {
                      const blockerTask = tasks.find(t => t.id === dep.taskId)
                      return (
                        <div key={dep.taskId} className='dep-tag'>
                          <span>{blockerTask?.title || `Task #${dep.taskId}`} — {blockerTask?.status}</span>
                          <button
                            type='button'
                            className='dep-remove-btn'
                            onClick={() => handleRemoveDependency(dep.taskId, dep.dependentTaskId)}>
                            ✕
                          </button>
                        </div>
                      )
                    })}
                  </div>
                )}

                {/* Add new dependency */}
                <div className='dep-add-row'>
                  <select
                    value={selectedDep}
                    onChange={e => setSelectedDep(e.target.value)}>
                    <option value=''>Select a blocker task</option>
                    {tasks
                      .filter(t => !editTask || t.id !== editTask.id)
                      .filter(t => !existingDeps.find(d => d.taskId === t.id))
                      .map(t => (
                        <option key={t.id} value={t.id}>
                          {t.title} — {t.status}
                        </option>
                      ))}
                  </select>
                  {editTask && (
                    <button type='button' className='dep-add-btn' onClick={handleAddDependency}>
                      + Add
                    </button>
                  )}
                </div>
              </div>

              <div className='modal-actions'>
                <button type='button' className='cancel-btn' onClick={() => setShowModal(false)}>
                  Cancel
                </button>
                <button type='submit' className='submit-btn'>
                  {editTask ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}

export default ProjectTasks