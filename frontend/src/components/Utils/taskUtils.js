export const STATUS_CLASSES = {
  Pending: 'status-pending',
  InProgress: 'status-inprogress',
  Completed: 'status-completed'
}

export const getStatusClass = (status) => STATUS_CLASSES[status] ?? ''

export const nextStatus = (current) =>
  current === 'Pending' ? 'InProgress' : 'Completed'