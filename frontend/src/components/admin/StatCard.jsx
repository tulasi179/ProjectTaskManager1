const StatCard = ({ label, value, color }) => {
  return (
    <div className={`bg-white rounded-xl p-5 shadow-sm border-t-4 ${color} text-center`}>
      <p className='text-3xl font-bold text-gray-800'>{value}</p>
      <p className='text-sm text-gray-500 mt-1'>{label}</p>
    </div>
  )
}

export default StatCard