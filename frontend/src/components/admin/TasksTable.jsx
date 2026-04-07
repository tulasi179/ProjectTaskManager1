import api from '../../api/axios'

const TasksTable = ({ tasks, users = [], role = 'Admin', onStatusUpdate }) => {

  const handleStatusChange = async (taskId, currentStatus) => {
    const nextStatus = currentStatus === 'Pending' ? 'InProgress' : 'Completed'

    try {
      await api.patch(`/task/${taskId}/status`, { Status: nextStatus })
      onStatusUpdate() // refetch tasks from parent
    } catch (err) {
        console.log(err.response?.data)
      console.log(err)
    }
  }

  return (
    <div className='bg-white rounded-xl shadow-sm p-6'>
      <h3 className='text-lg font-semibold text-gray-800 mb-4'>Recent Tasks</h3>

      <div className='overflow-x-auto'>
        <table className='w-full text-sm'>
          <thead>
            <tr className='bg-gray-50 text-left text-gray-500'>
              <th className='px-4 py-3'>Title</th>
              <th className='px-4 py-3'>Description</th>
              {role === 'Admin' && <th className='px-4 py-3'>Assigned To</th>}
              <th className='px-4 py-3'>Status</th>
              {role === 'User' && <th className='px-4 py-3'>Action</th>}
            </tr>
          </thead>

          <tbody>
            {tasks.slice(0, 8).map(t => {
              const assignee = users.find(u => u.id === t.assigneeId)
              const isCompleted = t.status === 'Completed'

              return (
                <tr key={t.id} className='border-t hover:bg-gray-50'>
                  <td className='px-4 py-3'>{t.title}</td>
                  <td className='px-4 py-3'>{t.description}</td>
                  {role === 'Admin' && (
                    <td className='px-4 py-3'>
                      {assignee?.username || `User #${t.assigneeId}`}
                    </td>
                  )}
                  <td className='px-4 py-3'>
                    <span className={`px-2 py-1 rounded-full text-xs font-medium
                      ${t.status === 'Pending' ? 'bg-yellow-100 text-yellow-700' : ''}
                      ${t.status === 'InProgress' ? 'bg-blue-100 text-blue-700' : ''}
                      ${t.status === 'Completed' ? 'bg-green-100 text-green-700' : ''}
                    `}>
                      {t.status}
                    </span>
                  </td>

                  {role === 'User' && (
                    <td className='px-4 py-3'>
                      {!isCompleted ? (
                        <button
                          onClick={() => handleStatusChange(t.id, t.status)}
                          className='px-3 py-1 text-xs rounded-full bg-indigo-100 text-indigo-700 hover:bg-indigo-200'
                        >
                          Move to {t.status === 'Pending' ? 'In Progress' : 'Completed'}
                        </button>
                      ) : (
                        <span className='text-xs text-gray-400'>Done</span>
                      )}
                    </td>
                  )}
                </tr>
              )
            })}

            {tasks.length === 0 && (
              <tr>
                <td colSpan={role === 'Admin' ? 4 : 3} className='text-center py-6 text-gray-400'>
                  No tasks found
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}

export default TasksTable