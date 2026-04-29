import './Skeleton.css'

export const SkeletonCard = () => (
  <div className='skeleton-card'>
    <div className='skeleton skeleton-title'></div>
    <div className='skeleton skeleton-text'></div>
    <div className='skeleton skeleton-text short'></div>
    <div className='skeleton skeleton-btn'></div>
  </div>
)

export const SkeletonStatCard = () => (
  <div className='skeleton-stat-card'>
    <div className='skeleton skeleton-stat-value'></div>
    <div className='skeleton skeleton-stat-label'></div>
  </div>
)

export const SkeletonNotif = () => (
  <div className='skeleton-notif'>
    <div className='skeleton-notif-dot'></div>
    <div className='skeleton-notif-body'>
      <div className='skeleton skeleton-text'></div>
      <div className='skeleton skeleton-text short'></div>
    </div>
  </div>
)