import api from "../../api/axios"
import { Navigate } from "react-router-dom"
import { useNavigate } from "react-router-dom"
import './ProjectsSection.css'
const ProjectsSection = ({ projects, navigate }) => {
  return (
    <div className='bg-white rounded-xl shadow-sm p-6 mb-6'>
      <div className='flex justify-between items-center mb-4'>
        <h3 className='text-lg font-semibold text-gray-800'>Projects</h3>
        <button
          onClick={() => navigate('/projects')}
          className='text-sm text-indigo-600 hover:underline'
        >
          View All →
        </button>
      </div>

      <div className='grid grid-cols-1 md:grid-cols-3 gap-4'>
        {projects.slice(0, 3).map(p => (
          <div
            key={p.id}
            onClick={() => navigate(`/projects/${p.id}/tasks`)}
            className='border border-gray-200 rounded-lg p-4 cursor-pointer hover:border-indigo-400 hover:shadow transition'
          >
            <h4 className='font-medium text-gray-800 mb-2'>{p.name}</h4>
            <p className='text-xs text-gray-500'>
              Start: {new Date(p.startDate).toLocaleDateString()}
            </p>
            <p className='text-xs text-gray-500'>
              End: {new Date(p.endDate).toLocaleDateString()}
            </p>
          </div>
        ))}
      </div>
    </div>
  )
}

export default ProjectsSection