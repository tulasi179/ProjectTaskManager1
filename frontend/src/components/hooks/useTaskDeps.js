import { useState } from 'react'
import api from '../../api/axios'

export const useTaskDeps = (tasks, dependencies, onRefresh) => {
  const [existingDeps, setExistingDeps] = useState([])
  const [selectedDep, setSelectedDep] = useState('')
  const [depError, setDepError] = useState('')

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
      const res = await api.get(`/taskdependency/${taskId}/dependents`)
      setExistingDeps(Array.isArray(res.data) ? res.data : [])
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
    setExistingDeps(prev => prev.filter(d => d.taskId !== taskId))
    onRefresh()
  }

  return { existingDeps, selectedDep, setSelectedDep, depError,
           isBlocked, getBlockingNames, loadDepsForTask, addDep, removeDep }
}