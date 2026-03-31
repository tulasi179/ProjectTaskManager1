import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import api from '../../api/axios'
import Navbar from '../Navbar'

const Projects = () => {
  const { user, authLoading } = useAuth()
  const navigate = useNavigate()

  const [projects, setProjects] = useState([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [editProject, setEditProject] = useState(null)
  const [form, setForm] = useState({ name: '', ownerId: '', startDate: '', endDate: '' })
  const [error, setError] = useState('')
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => {
    if (authLoading) return
    if (!user || user.role !== 'Admin') { navigate('/login'); return }
    fetchProjects()
  }, [authLoading])

  const fetchProjects = async () => {
    try {
      const res = await api.get('/project')
      const data = res.data.data || res.data
      setProjects(Array.isArray(data) ? data : [])
    } catch (err) {
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value })
  }

  const openCreateModal = () => {
    setEditProject(null)
    setForm({ name: '', ownerId: user.id, startDate: '', endDate: '' })
    setError('')
    setShowModal(true)
  }

  const openEditModal = (p) => {
    setEditProject(p)
    setForm({
      name: p.name,
      ownerId: p.ownerId,
      startDate: p.startDate?.split('T')[0],
      endDate: p.endDate?.split('T')[0]
    })
    setError('')
    setShowModal(true)
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setSubmitting(true)
    try {
      if (editProject) {
        await api.put(`/project/${editProject.id}`, form)
      } else {
        await api.post('/project', form)
      }
      setShowModal(false)
      fetchProjects()
    } catch (err) {
      setError(err.response?.data?.message || 'Something went wrong.')
    } finally {
      setSubmitting(false)
    }
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this project?')) return
    try {
      await api.delete(`/project/${id}`)
      fetchProjects()
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to delete.')
    }
  }

  if (authLoading || loading) return (
    <div className='flex items-center justify-center h-screen text-gray-500 text-lg'>
      Loading...
    </div>
  )

  return (
    <div className='min-h-screen bg-gray-100'>
      <Navbar />

      <div className='max-w-7xl mx-auto px-6 py-8'>

        {/* Header */}
        <div className='flex justify-between items-center mb-8'>
          <div>
            <h2 className='text-2xl font-bold text-gray-800'>Projects</h2>
            <p className='text-sm text-gray-500 mt-1'>{projects.length} total projects</p>
          </div>
          <button
            onClick={openCreateModal}
            className='bg-indigo-600 text-white px-5 py-2 rounded-lg text-sm font-medium hover:bg-indigo-700 transition'
          >
            + New Project
          </button>
        </div>

        {/* Projects Grid */}
        {projects.length === 0 ? (
          <div className='bg-white rounded-xl p-16 text-center text-gray-400 shadow-sm'>
            <p className='text-lg'>No projects yet</p>
            <p className='text-sm mt-1'>Click "New Project" to create one</p>
          </div>
        ) : (
          <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6'>
            {projects.map(p => (
              <div key={p.id} className='bg-white rounded-xl shadow-sm hover:shadow-md transition p-6'>

                {/* Project Name */}
                <div className='flex justify-between items-start mb-4'>
                  <h3 className='text-lg font-semibold text-gray-800'>{p.name}</h3>
                  <div className='flex gap-2'>
                    <button
                      onClick={() => openEditModal(p)}
                      className='text-xs bg-gray-100 hover:bg-gray-200 text-gray-600 px-3 py-1 rounded-lg transition'
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => handleDelete(p.id)}
                      className='text-xs bg-red-50 hover:bg-red-100 text-red-500 px-3 py-1 rounded-lg transition'
                    >
                      Delete
                    </button>
                  </div>
                </div>

                {/* Dates */}
                <div className='space-y-1 mb-4'>
                  <p className='text-xs text-gray-500'>
                    📅 Start: <span className='text-gray-700'>{new Date(p.startDate).toLocaleDateString()}</span>
                  </p>
                  <p className='text-xs text-gray-500'>
                    📅 End: <span className='text-gray-700'>{new Date(p.endDate).toLocaleDateString()}</span>
                  </p>
                </div>

                {/* View Tasks Button */}
                <button
                  onClick={() => navigate(`/projects/${p.id}/tasks`)}
                  className='w-full mt-2 bg-indigo-50 hover:bg-indigo-100 text-indigo-600 text-sm font-medium py-2 rounded-lg transition'
                >
                  View Tasks →
                </button>

              </div>
            ))}
          </div>
        )}
      </div>

      {/* Modal */}
      {showModal && (
        <div
          className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50'
          onClick={() => setShowModal(false)}
        >
          <div
            className='bg-white rounded-xl p-8 w-full max-w-md shadow-xl'
            onClick={e => e.stopPropagation()}
          >
            <h3 className='text-xl font-semibold text-gray-800 mb-6'>
              {editProject ? 'Edit Project' : 'Create New Project'}
            </h3>

            {error && (
              <div className='bg-red-50 text-red-600 text-sm px-4 py-3 rounded-lg mb-4'>
                {error}
              </div>
            )}

            <form onSubmit={handleSubmit} className='space-y-4'>
              <div>
                <label className='block text-sm font-medium text-gray-700 mb-1'>
                  Project Name
                </label>
                <input
                  type='text'
                  name='name'
                  value={form.name}
                  onChange={handleChange}
                  placeholder='Enter project name'
                  required
                  className='w-full border border-gray-300 rounded-lg px-4 py-2 text-sm focus:outline-none focus:border-indigo-500'
                />
              </div>

              <div>
                <label className='block text-sm font-medium text-gray-700 mb-1'>
                  Start Date
                </label>
                <input
                  type='date'
                  name='startDate'
                  value={form.startDate}
                  onChange={handleChange}
                  required
                  className='w-full border border-gray-300 rounded-lg px-4 py-2 text-sm focus:outline-none focus:border-indigo-500'
                />
              </div>

              <div>
                <label className='block text-sm font-medium text-gray-700 mb-1'>
                  End Date
                </label>
                <input
                  type='date'
                  name='endDate'
                  value={form.endDate}
                  onChange={handleChange}
                  required
                  className='w-full border border-gray-300 rounded-lg px-4 py-2 text-sm focus:outline-none focus:border-indigo-500'
                />
              </div>

              <div className='flex justify-end gap-3 pt-2'>
                <button
                  type='button'
                  onClick={() => setShowModal(false)}
                  className='px-5 py-2 text-sm bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-lg transition'
                >
                  Cancel
                </button>
                <button
                  type='submit'
                  disabled={submitting}
                  className='px-5 py-2 text-sm bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg transition disabled:opacity-60'
                >
                  {submitting ? 'Saving...' : editProject ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}

export default Projects