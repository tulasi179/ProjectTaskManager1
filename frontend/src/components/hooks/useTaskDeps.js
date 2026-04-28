import { useState } from 'react'
import api from '../../api/axios'

export const useTaskDeps = (tasks, dependencies, onRefresh) => {
  const [existingDeps, setExistingDeps] = useState([])
  const [selectedDep, setSelectedDep] = useState('')
  const [depError, setDepError] = useState('')
  const [pendingDeps, setPendingDeps] = useState([])

  const isBlocked = (taskId) =>
    dependencies
      .filter(d => d.dependentTaskId === taskId)
      .some(d => tasks.find(t => t.id === d.taskId)?.status !== 'Completed')

  const getBlockingNames = (taskId) =>
    dependencies
      .filter(d => d.dependentTaskId === taskId)
      .map(d => tasks.find(t => t.id === d.taskId))
      .filter(t => t && t.status !== 'Completed')
      .map(t => t.title)

 const loadDepsForTask = async (taskId) => {
    try {
        const res = await api.get('/taskdependency')
        const deps = Array.isArray(res.data) ? res.data : []
        setExistingDeps(deps.filter(d => parseInt(d.dependentTaskId) === parseInt(taskId)))
    } catch {
        setExistingDeps([])
    }
}

  const addDep = async (dependentTaskId) => {
    if (!selectedDep) return
    setDepError('')
    try {
      await api.post('/taskdependency', { taskId: parseInt(selectedDep), dependentTaskId })
      await loadDepsForTask(dependentTaskId)
      setSelectedDep('')
      onRefresh()
    } catch (err) {
      const msg = err.response?.data?.message || err.response?.data
      setDepError(typeof msg === 'string' ? msg : 'Failed to add dependency.')
    }
  }

  const removeDep = async (taskId, dependentTaskId) => {
    await api.delete(`/taskdependency/${taskId}/${dependentTaskId}`)
    setExistingDeps(prev => prev.filter(d => !(d.taskId === taskId && d.dependentTaskId === dependentTaskId)))
    onRefresh()
}

  const addPendingDep = () => {
  if (!selectedDep) return
  if (pendingDeps.includes(selectedDep)) return // no duplicates

  setPendingDeps(prev => [...prev, selectedDep])
  // Show it visually in the modal
  setExistingDeps(prev => [...prev, { taskId: parseInt(selectedDep), dependentTaskId: null }])
  setSelectedDep('')
}

const clearPendingDeps = () => {
  setPendingDeps([])
  setExistingDeps([])
}

  return { existingDeps, selectedDep, setSelectedDep, depError,
           isBlocked, getBlockingNames, loadDepsForTask, addDep, removeDep, pendingDeps, addPendingDep, clearPendingDeps }
}