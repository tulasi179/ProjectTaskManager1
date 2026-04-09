const TaskModal = ({ tasks, users, editTask, form, onChange, onSubmit, onClose,
                     existingDeps, selectedDep, setSelectedDep, depError, onAddDep, onRemoveDep, error }) => (
  <div className='modal-overlay' onClick={onClose}>
    <div className='modal' onClick={e => e.stopPropagation()}>
      <h3>{editTask ? 'Edit Task' : 'Create New Task'}</h3>
      {error && <div className='error-message'>{error}</div>}

      <form onSubmit={onSubmit}>
        <div className='form-group'>
          <label>Title</label>
          <input name='title' value={form.title} onChange={onChange} placeholder='Enter task title' required />
        </div>
        <div className='form-group'>
          <label>Description</label>
          <textarea name='description' value={form.description} onChange={onChange} rows={3} required />
        </div>
        <div className='form-group'>
          <label>Assign To</label>
          <select name='assigneeId' value={form.assigneeId} onChange={onChange} required>
            <option value=''>Select a user</option>
            {users.map(u => <option key={u.id} value={u.id}>{u.username}</option>)}
          </select>
        </div>

        <div className='form-group'>
          <label>Blocked By <span className='optional'>(must complete first)</span></label>
          {depError && <div className='error-message'>{depError}</div>}
          {existingDeps.map(dep => {
            const blocker = tasks.find(t => t.id === dep.taskId)
            return (
              <div key={dep.taskId} className='dep-tag'>
                <span>{blocker?.title ?? `Task #${dep.taskId}`} — {blocker?.status}</span>
                <button type='button' className='dep-remove-btn'
                  onClick={() => onRemoveDep(dep.taskId, dep.dependentTaskId)}>✕</button>
              </div>
            )
          })}
          <div className='dep-add-row'>
            <select value={selectedDep} onChange={e => setSelectedDep(e.target.value)}>
              <option value=''>Select a blocker task</option>
              {tasks
                .filter(t => !editTask || t.id !== editTask.id)
                .filter(t => !existingDeps.find(d => d.taskId === t.id))
                .map(t => <option key={t.id} value={t.id}>{t.title} — {t.status}</option>)}
            </select>
            {editTask && <button type='button' className='dep-add-btn' onClick={onAddDep}>+ Add</button>}
          </div>
        </div>

        <div className='modal-actions'>
          <button type='button' className='cancel-btn' onClick={onClose}>Cancel</button>
          <button type='submit' className='submit-btn'>{editTask ? 'Update' : 'Create'}</button>
        </div>
      </form>
    </div>
  </div>
)

export default TaskModal