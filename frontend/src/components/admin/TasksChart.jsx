import { useEffect, useRef } from 'react'
import { Chart, ArcElement, DoughnutController, Tooltip, Legend } from 'chart.js'

Chart.register(ArcElement, DoughnutController, Tooltip, Legend)

// unique color per project (cycles if more than 8 projects)
const PROJECT_COLORS = [
  '#534AB7', // purple
  '#D85A30', // coral
  '#1D9E75', // teal
  '#378ADD', // blue
  '#D4537E', // pink
  '#BA7517', // amber
  '#639922', // green
  '#E24B4A', // red
]

const STATUS_COLORS = {
  Completed:  '#1D9E75',
  InProgress: '#378ADD',
  Pending:    '#F5C4B3',
}

const STATUS_LABELS = {
  Completed:  'Completed',
  InProgress: 'In Progress',
  Pending:    'Pending',
}

const TasksChart = ({ tasks, projects }) => {
  const donutRef = useRef(null)
  const donutInstance = useRef(null)

  const completed  = tasks.filter(t => t.status === 'Completed').length
  const inProgress = tasks.filter(t => t.status === 'InProgress').length
  const pending    = tasks.filter(t => t.status === 'Pending').length
  const total      = tasks.length || 1

  // Tasks per project
  const projectStats = projects.map((p, i) => {
    const projectTasks = tasks.filter(t => t.projectId === p.id)
    const done = projectTasks.filter(t => t.status === 'Completed').length
    return {
      name: p.name,
      total: projectTasks.length,
      done,
      color: PROJECT_COLORS[i % PROJECT_COLORS.length],
      pct: projectTasks.length ? Math.round((done / projectTasks.length) * 100) : 0,
    }
  })

  useEffect(() => {
    if (!donutRef.current) return
    if (donutInstance.current) donutInstance.current.destroy()

    donutInstance.current = new Chart(donutRef.current, {
      type: 'doughnut',
      data: {
        labels: ['Completed', 'In Progress', 'Pending'],
        datasets: [{
          data: [completed, inProgress, pending],
          backgroundColor: [
            STATUS_COLORS.Completed,
            STATUS_COLORS.InProgress,
            STATUS_COLORS.Pending,
          ],
          borderWidth: 4,
          borderColor: 'transparent',
          hoverOffset: 8,
        }],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '72%',
        plugins: {
          legend: { display: false },
          tooltip: {
            callbacks: {
              label: ctx => ` ${ctx.label}: ${ctx.raw} tasks (${Math.round(ctx.raw / total * 100)}%)`,
            },
          },
        },
      },
    })

    return () => donutInstance.current?.destroy()
  }, [tasks])

  return (
    <div style={{
      background: 'white',
      borderRadius: '16px',
      padding: '1.5rem',
      marginTop: '2rem',
      boxShadow: '0 1px 3px rgba(0,0,0,0.08)',
      fontFamily: 'system-ui, sans-serif',
    }}>
      <h3 style={{ margin: '0 0 1.25rem', fontSize: '16px', fontWeight: 600, color: '#1a1a2e' }}>
        Task Overview
      </h3>

      {/* Summary cards */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '10px', marginBottom: '1.5rem' }}>
        {[
          { label: 'Completed',  value: completed,  color: STATUS_COLORS.Completed },
          { label: 'In Progress', value: inProgress, color: STATUS_COLORS.InProgress },
          { label: 'Pending',    value: pending,    color: STATUS_COLORS.Pending },
        ].map(s => (
          <div key={s.label} style={{
            background: '#f8f9fb',
            borderRadius: '12px',
            padding: '14px 16px',
            borderLeft: `4px solid ${s.color}`,
          }}>
            <div style={{ fontSize: '12px', color: '#888', marginBottom: '4px' }}>{s.label}</div>
            <div style={{ fontSize: '22px', fontWeight: 700, color: '#1a1a2e' }}>{s.value}</div>
            <div style={{ fontSize: '11px', color: '#aaa' }}>{Math.round(s.value / total * 100)}% of total</div>
          </div>
        ))}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '2rem', alignItems: 'center' }}>

        {/* Donut chart */}
        <div style={{ position: 'relative', height: '240px' }}>
          <canvas
            ref={donutRef}
            role="img"
            aria-label={`Donut chart: ${completed} completed, ${inProgress} in progress, ${pending} pending`}
          >
            Tasks: {completed} completed, {inProgress} in progress, {pending} pending.
          </canvas>
          {/* Center label */}
          <div style={{
            position: 'absolute', top: '50%', left: '50%',
            transform: 'translate(-50%, -50%)',
            textAlign: 'center', pointerEvents: 'none',
          }}>
            <div style={{ fontSize: '28px', fontWeight: 700, color: '#1a1a2e' }}>{total}</div>
            <div style={{ fontSize: '11px', color: '#888', marginTop: '2px' }}>total tasks</div>
          </div>
        </div>

        {/* Right side */}
        <div>
          {/* Legend */}
          <div style={{ display: 'flex', gap: '12px', marginBottom: '1.25rem', flexWrap: 'wrap' }}>
            {Object.entries(STATUS_COLORS).map(([key, color]) => (
              <span key={key} style={{ display: 'flex', alignItems: 'center', gap: '5px', fontSize: '12px', color: '#555' }}>
                <span style={{ width: '10px', height: '10px', borderRadius: '2px', background: color, display: 'inline-block' }} />
                {STATUS_LABELS[key]}
              </span>
            ))}
          </div>

          {/* Per project progress */}
          <div style={{ fontSize: '12px', fontWeight: 600, color: '#aaa', marginBottom: '10px', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
            By Project
          </div>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
            {projectStats.length === 0 && (
              <div style={{ fontSize: '13px', color: '#aaa' }}>No projects found.</div>
            )}
            {projectStats.map(p => (
              <div key={p.name}>
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '5px' }}>
                  <span style={{ fontSize: '13px', color: '#333', fontWeight: 500 }}>{p.name}</span>
                  <span style={{ fontSize: '12px', color: '#888' }}>{p.done}/{p.total} done</span>
                </div>
                <div style={{ height: '7px', background: '#f0f0f0', borderRadius: '999px', overflow: 'hidden' }}>
                  <div style={{
                    height: '100%',
                    width: `${p.pct}%`,
                    background: p.color,
                    borderRadius: '999px',
                    transition: 'width 0.6s ease',
                  }} />
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  )
}

export default TasksChart