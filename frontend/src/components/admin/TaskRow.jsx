import { getStatusClass } from "../Utils/taskUtils"

const TaskRow = ({ task, users, isAdmin, isBlocked, blockingNames, onEdit, onDelete, onStatusUpdate }) => {
  const assignee = users.find(u => u.id === task.assigneeId)
  const blocked = isBlocked(task.id)
  const names = blockingNames(task.id)

  return (
    <tr className={blocked ? 'blocked-row' : ''}>
      <td>{task.title}</td>
      <td>{task.description}</td>
      {isAdmin && <td>{assignee?.username ?? `User #${task.assigneeId}`}</td>}
      <td>
        <span className={`status-badge ${getStatusClass(task.status)}`}>{task.status}</span>
      </td>
      <td>
        {names.length > 0
          ? <span className='blocked-badge' title={names.join(', ')}>🔒 {names.join(', ')}</span>
          : <span className='clear-badge'>✓ Clear</span>}
      </td>
      <td className='action-btns'>
        {isAdmin ? (
          <>
            <button className='edit-btn' onClick={() => onEdit(task)}>Edit</button>
            <button className='delete-btn' onClick={() => onDelete(task.id)}>Delete</button>
          </>
        ) : task.status === 'Completed' ? (
          <span className='done-label'>Done ✓</span>
        ) : blocked ? (
          <span className='blocked-status-btn'>🔒 Blocked</span>
        ) : (
          <button className='status-btn' onClick={() => onStatusUpdate(task.id, task.status)}>
            Move to {task.status === 'Pending' ? 'In Progress' : 'Completed'}
          </button>
        )}
      </td>
    </tr>
  )
}

export default TaskRow