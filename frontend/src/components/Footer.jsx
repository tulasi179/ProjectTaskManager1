// import './Footer.css'

// const Footer = () => {
//   return (
//     <footer className='footer'>
//       <p>© 2026 Project Task Manager. All rights reserved.</p>
//     </footer>
//   )
// }

// export default Footer


import { useState } from 'react'
import './Footer.css'

const Footer = () => {
  const [visible, setVisible] = useState(false)

  const handleMouseMove = (e) => {
    const threshold = window.innerHeight - 50
    if (e.clientY >= threshold) {//cursor near the footer or not
      setVisible(true)
    } else {
      setVisible(false)
    }
  }

  window.onmousemove = handleMouseMove//observes every change of the cursor

  return (
    <footer className={`footer ${visible ? 'visible' : ''}`}>
      <p>© 2026 Project Task Manager. All rights reserved.</p>
    </footer>
  )
}

export default Footer