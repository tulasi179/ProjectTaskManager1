import React, { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import Navbar from '../Navbar'
import DependencyGraph from './DependencyGraph'
import TaskRow from './TaskRow'
import TaskModal from './TaskModal'
import { useTaskDeps } from '../hooks/useTaskDeps'
import { nextStatus } from '../Utils/taskUtils'
import './ProjectTasks.css'
import { useProjectTasks } from '../hooks/useProjectTasks'
import api from '../../api/axios'

const EMPTY_FORM = (projectId) => ({ title: '', description: '', assigneeId: '', projectId })

const ProjectTasks = () => {
  const { id } = useParams()
  const navigate = useNavigate()
  const { project, tasks, users, dependencies, loading, isAdmin,
          fetchAll, createTask, updateTask, deleteTask, updateStatus } =
    useProjectTasks(id)

  const deps = useTaskDeps(tasks, dependencies, fetchAll)

  const [showModal, setShowModal] = useState(false)
  const [editTask, setEditTask] = useState(null)
  const [form, setForm] = useState(EMPTY_FORM(id))
  const [error, setError] = useState('')

  const openCreate = () => {
    setEditTask(null); setForm(EMPTY_FORM(id)); setError('')
    deps.setSelectedDep('');
    deps.clearPendingDeps()
    setShowModal(false) 
    setShowModal(true)
  }

  const openEdit = async (task) => {
    setEditTask(task)
    setForm({ title: task.title, description: task.description, assigneeId: task.assigneeId, projectId: id })
    deps.clearPendingDeps()
    await deps.loadDepsForTask(task.id)
    setError(''); setShowModal(true)
  }

  // const handleSubmit = async (e) => {
  //   e.preventDefault(); setError('')
  //   const payload = { ...form, projectId: parseInt(id), assigneeId: parseInt(form.assigneeId) }
  //   try {
  //     if (editTask) {
  //       await updateTask(editTask.id, payload)
  //     } else {
  //       const res = await createTask(payload)
  //       if (deps.selectedDep)
  //         await api.post('/taskdependency', { taskId: parseInt(deps.selectedDep), dependentTaskId: res.data.id }).catch(() => {})
  //     }
  //     setShowModal(false); fetchAll()
  //   } catch (err) {
  //     setError(err.response?.data?.message || 'Something went wrong.')
  //   }
  // }


const handleSubmit = async (e) => {
  e.preventDefault(); setError('')
  const payload = { ...form, projectId: parseInt(id), assigneeId: parseInt(form.assigneeId) }
  try {
    if (editTask) {
      await updateTask(editTask.id, payload)
    } else {
      const res = await createTask(payload)
      const newTaskId = res.data.id

      // Save all pending deps after task is created
      for (const depTaskId of deps.pendingDeps) {
        await api.post('/taskdependency', {
          taskId: parseInt(depTaskId),
          dependentTaskId: newTaskId
        }).catch(() => {})
      }
      deps.clearPendingDeps()
    }
    setShowModal(false); fetchAll()
  } catch (err) {
    setError(err.response?.data?.message || 'Something went wrong.')
  }
}

// Add this handler in ProjectTasks.jsx
const handleAddDep = () => {
  if (editTask) {
    deps.addDep(editTask.id)  // edit mode → save to DB immediately
  } else {
    deps.addPendingDep()       // create mode → store locally
  }
}

  const handleDelete = async (taskId) => {
    if (!window.confirm('Delete this task?')) return
    await deleteTask(taskId).catch(err => alert(err.response?.data?.message || 'Failed to delete.'))
    fetchAll()
  }

  const handleStatusUpdate = async (taskId, status) => {
    await updateStatus(taskId, nextStatus(status)).catch(err => alert(err.response?.data || 'Failed to update.'))
    fetchAll()
  }

  if (loading) return <div className='loading'>Loading...</div>

  return (
    <div className='tasks-container'>
      <Navbar />
      <div className='tasks-content'>
        {project && (
          <div className='project-info'>
            <button className='back-btn' onClick={() => navigate('/projects')}>← Back to Projects</button>
            <h2>{project.name}</h2>
            {project.description && <p className='project-desc'>{project.description}</p>}
            <div className='project-dates'>
              <span>📅 Start: {new Date(project.startDate).toLocaleDateString()}</span>
              <span>📅 End: {new Date(project.endDate).toLocaleDateString()}</span>
            </div>
          </div>
        )}

        <div className='page-header'>
          <h3>Tasks</h3>
          {isAdmin && <button className='create-btn' onClick={openCreate}>+ New Task</button>}
        </div>

        {tasks.length === 0 ? <div className='empty-state'>No tasks found.</div> : (
          <div className='tasks-table-wrapper'>
            <table className='tasks-table'>
              <thead>
                <tr>
                  <th>Title</th><th>Description</th>
                  {isAdmin && <th>Assigned To</th>}
                  <th>Status</th><th>Blocked By</th><th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {tasks.map(t => (
                  <TaskRow key={t.id} task={t} users={users} isAdmin={isAdmin}
                    isBlocked={deps.isBlocked} blockingNames={deps.getBlockingNames}
                    onEdit={openEdit} onDelete={handleDelete} onStatusUpdate={handleStatusUpdate} />
                ))}
              </tbody>
            </table>
          </div>
        )}

        {tasks.length > 0 && (
          <div className='dep-graph-section'>
            <h3 className='dep-graph-title'>Dependency Graph</h3>
            <p className='dep-graph-subtitle'>Arrows show which tasks must be completed before another can start.</p>
            <DependencyGraph tasks={tasks} dependencies={dependencies} users={users} />
          </div>
        )}
      </div>

      {showModal && isAdmin && (
  <TaskModal
    tasks={tasks} 
    users={users} 
    editTask={editTask} 
    form={form}
    onChange={e => setForm(f => ({ ...f, [e.target.name]: e.target.value }))}
    onSubmit={handleSubmit} onClose={() => setShowModal(false)}
    existingDeps={deps.existingDeps} 
    selectedDep={deps.selectedDep}
    setSelectedDep={deps.setSelectedDep} 
    depError={deps.depError}
    onAddDep={handleAddDep}          
    onRemoveDep={deps.removeDep}     
    error={error}
  />
)}
    </div>
  )
}

export default ProjectTasks