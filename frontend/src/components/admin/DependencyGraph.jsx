import { useState } from "react"
const DependencyGraph = ({ tasks, dependencies, users }) => {
  const [tooltip, setTooltip] = useState(null)

  const NODE_W = 160
  const NODE_H = 52
  const COL_GAP = 80
  const ROW_GAP = 36

  const getDepth = (taskId, memo = {}) => {
    if (memo[taskId] !== undefined) return memo[taskId]
    const blockers = dependencies.filter(d => d.dependentTaskId === taskId).map(d => d.taskId)
    if (!blockers.length) { memo[taskId] = 0; return 0 }
    memo[taskId] = Math.max(...blockers.map(b => getDepth(b, memo))) + 1
    return memo[taskId]
  }

  const memo = {}
  const depths = {}
  tasks.forEach(t => { depths[t.id] = getDepth(t.id, memo) })

  const maxDepth = Math.max(...Object.values(depths), 0)

  const columns = {}
  tasks.forEach(t => {
    const col = depths[t.id]
    if (!columns[col]) columns[col] = []
    columns[col].push(t)
  })

  const positions = {}
  for (let col = 0; col <= maxDepth; col++) {
    const colTasks = columns[col] || []
    colTasks.forEach((t, row) => {
      positions[t.id] = {
        x: col * (NODE_W + COL_GAP) + 20,
        y: row * (NODE_H + ROW_GAP) + 20
      }
    })
  }

  const maxRows = Math.max(...Object.values(columns).map(c => c.length), 1)
  const svgW = (maxDepth + 1) * (NODE_W + COL_GAP) + 20
  const svgH = maxRows * (NODE_H + ROW_GAP) + 20

  const isBlocked = (taskId) => {
    const blockers = dependencies.filter(d => d.dependentTaskId === taskId).map(d => d.taskId)
    return blockers.some(bid => {
      const t = tasks.find(t => t.id === bid)
      return t && t.status !== 'Completed'
    })
  }

  const getNodeColor = (task) => {
    if (task.status === 'Completed') return { bg: '#dcfce7', border: '#16a34a', text: '#15803d' }
    if (task.status === 'InProgress') return { bg: '#dbeafe', border: '#2563eb', text: '#1d4ed8' }
    if (isBlocked(task.id)) return { bg: '#fee2e2', border: '#dc2626', text: '#b91c1c' }
    return { bg: '#f3f4f6', border: '#9ca3af', text: '#374151' }
  }

  const truncate = (str, n) => str.length > n ? str.slice(0, n - 1) + '…' : str

  return (
    <div className='dep-graph-scroll'>
      <svg width={svgW} height={svgH} style={{ display: 'block', minWidth: svgW }}>
        <defs>
          <marker id='dep-arrow' viewBox='0 0 10 10' refX='8' refY='5'
            markerWidth='6' markerHeight='6' orient='auto-start-reverse'>
            <path d='M2 1L8 5L2 9' fill='none' stroke='#6946e5'
              strokeWidth='1.5' strokeLinecap='round' strokeLinejoin='round' />
          </marker>
        </defs>

        {/* edges first so nodes render on top */}
        {dependencies.map((dep, i) => {
          const from = positions[dep.taskId]
          const to = positions[dep.dependentTaskId]
          if (!from || !to) return null
          const x1 = from.x + NODE_W
          const y1 = from.y + NODE_H / 2
          const x2 = to.x
          const y2 = to.y + NODE_H / 2
          const mx = (x1 + x2) / 2
          return (
            <path
              key={i}
              d={`M${x1},${y1} C${mx},${y1} ${mx},${y2} ${x2},${y2}`}
              fill='none'
              stroke='#6946e5'
              strokeWidth='1.5'
              strokeDasharray='5 3'
              markerEnd='url(#dep-arrow)'
              opacity='0.6'
            />
          )
        })}

        {/* nodes */}
        {tasks.map(t => {
          const pos = positions[t.id]
          if (!pos) return null
          const c = getNodeColor(t)
          const blocked = isBlocked(t.id)
          const blockers = dependencies
            .filter(d => d.dependentTaskId === t.id)
            .map(d => tasks.find(tk => tk.id === d.taskId)?.title)
            .filter(Boolean)
          const unlocks = dependencies
            .filter(d => d.taskId === t.id)
            .map(d => tasks.find(tk => tk.id === d.dependentTaskId)?.title)
            .filter(Boolean)
          const assigneeName = users.find(u => u.id === t.assigneeId)?.username || `User #${t.assigneeId}`

          return (
            <g
              key={t.id}
              style={{ cursor: 'pointer' }}
              onMouseEnter={() => setTooltip({ task: t, blocked, blockers, unlocks, x: pos.x, y: pos.y })}
              onMouseLeave={() => setTooltip(null)}
            >
              <rect
                x={pos.x} y={pos.y}
                width={NODE_W} height={NODE_H}
                rx='8'
                fill={c.bg}
                stroke={c.border}
                strokeWidth={blocked ? 2 : 1}
              />
              <text
                x={pos.x + NODE_W / 2}
                y={pos.y + 18}
                textAnchor='middle'
                fontSize='12'
                fontWeight='500'
                fill={c.text}
                fontFamily='inherit'
              >
                {truncate(t.title, 20)}
              </text>
              <text
                x={pos.x + NODE_W / 2}
                y={pos.y + 34}
                textAnchor='middle'
                fontSize='11'
                fill={c.text}
                opacity='0.75'
                fontFamily='inherit'
              >
                {blocked ? 'Blocked' : t.status} · {assigneeName}
              </text>
            </g>
          )
        })}

        {/* tooltip */}
        {tooltip && (() => {
          const tx = tooltip.x + NODE_W + 8
          const ty = tooltip.y
          const { task, blocked, blockers, unlocks } = tooltip
          const lines = [
            task.title,
            `Status: ${blocked ? 'Blocked' : task.status}`,
            blockers.length ? `Blocked by: ${blockers.join(', ')}` : null,
            unlocks.length ? `Unlocks: ${unlocks.join(', ')}` : null,
          ].filter(Boolean)
          const ttW = 200
          const ttH = lines.length * 18 + 16
          const safeX = tx + ttW > svgW ? tooltip.x - ttW - 8 : tx
          return (
            <g pointerEvents='none'>
              <rect x={safeX} y={ty} width={ttW} height={ttH}
                rx='6' fill='white' stroke='#e5e7eb' strokeWidth='1' />
              {lines.map((line, i) => (
                <text key={i} x={safeX + 10} y={ty + 14 + i * 18}
                  fontSize='11' fill={i === 0 ? '#1f2937' : '#6b7280'}
                  fontWeight={i === 0 ? '500' : '400'}
                  fontFamily='inherit'>
                  {line}
                </text>
              ))}
            </g>
          )
        })()}
      </svg>
    </div>
  )
}
export default DependencyGraph