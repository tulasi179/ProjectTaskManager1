import api from './api/axios'
import React, { useState } from 'react'
import Login from './Login'
import OtpVerification from './OtpVerification'
import {useNavigate, Link} from 'react-router-dom'
import './Register.css'

const Register = () => {

    const [form ,setForm] = useState({
        username: '',
        password: '',
        confirmPassword: '', 
        email: '',
        role: 'User'// Its always an user 
    })
    const [error, setError] = useState('')
    const [loading, setLoading]=useState(false)//while handling the request
     const [showOtp, setShowOtp] = useState(false)//otp handling
    const navigate = useNavigate()// to navigate to other pages after registering

    const handleChange = (e) => {
        setForm({...form, [e.target.name]: e.target.value})
    }

    const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setError('')

      const passwordRegex = /^(?=.*[A-Z])(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]).{8,}$/
    if (!passwordRegex.test(form.password)) {
        setError('Password must be at least 8 characters, include one uppercase letter and one special character.')
        setLoading(false)
        return
    }

       if (form.password !== form.confirmPassword) {
        setError('Passwords do not match.')
        setLoading(false)
        return
    }

    try {
        // Step 1 — Register
        await api.post('/auth/register', {
            username: form.username,
            email: form.email,
            password: form.password,
            role: form.role
        })

    } catch (err) {
        setError(err.response?.data?.message || 'Registration failed.')
        setLoading(false)
        return
    }

    try {
        // Step 2 — Send OTP
        await api.post('/otp/send', {
            email: form.email,
            purpose: 'registration'
        })
    } catch (err) {
        // ← You'll now see exactly what's failing here
        setError(err.response?.data?.message || 'Failed to send OTP. Check your email.')
        setLoading(false)
        return
    }

    // Step 3 — Show OTP screen
    setLoading(false)
    setShowOtp(true)
}

      // if OTP screen is active, show it instead of the form
    if (showOtp) {
        return (
            <OtpVerification
                email={form.email}
                purpose="registration"
                onSuccess={() => navigate('/login')}
            />
        )
    }

  return (
    <div className='register-container'>
        <div className='register-box'>
            <h2>Project Task Manager</h2>
            <p className='register-subtitle'>create your account</p>


            {error && <div className="error-message">{error}</div>}


            <form onSubmit={handleSubmit}>
                <div className='form-group'>
                    <label>Username</label>
                    <input 
                        type='text'
                        name='username'
                        value={form.username}
                        onChange={handleChange}
                        placeholder='Enter your username'
                        required
                    />
                </div>

                <div className='form-group'>
                    <label>Email</label>
                    <input
                      type='email'
                      name='email'
                      value={form.email}
                      onChange={handleChange}
                      placeholder='Enter your email'
                      required
                      />
                </div>

                <div className='form-group'>
                    <label>Password</label>
                    <input
                       type='password'
                       name='password'
                       value={form.password}
                       onChange={handleChange}
                       placeholder='Enter your password'
                       required
                    />

                     {/* Live password hints */}
                    {form.password && (
                        <ul className='password-hints'>
                            <li style={{ color: form.password.length >= 8 ? 'green' : 'red' }}>
                                {form.password.length >= 8 ? '✅' : '❌'} At least 8 characters
                            </li>
                            <li style={{ color: /[A-Z]/.test(form.password) ? 'green' : 'red' }}>
                                {/[A-Z]/.test(form.password) ? '✅' : '❌'} One uppercase letter
                            </li>
                            <li style={{ color: /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(form.password) ? 'green' : 'red' }}>
                                {/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(form.password) ? '✅' : '❌'} One special character
                            </li>
                        </ul>
                    )}
                </div>

                 <div className='form-group'>
                    <label>Confirm Password</label>
                    <input
                        type='password'
                        name='confirmPassword'  
                        value={form.confirmPassword}
                        onChange={handleChange}
                        placeholder='Re-enter your password'
                        required
                    />

                    {form.confirmPassword && (
                        <ul className='password-hints'>
                             <li style={{ color: form.password=== form.confirmPassword ? 'green' : 'red' }}>
                                {form.password=== form.confirmPassword ? '✅' : '❌'} confirm password should be same as the password
                            </li>
                        </ul>
                    )}
                </div>


                {/* <div className='form-group'>
                    <label>Role</label>
                    <select 
                    name='role' 
                    value={form.role} 
                    onChange={handleChange}>

                        <option value='User'>User</option>
                        <option value='Admin'>Admin</option>
                    </select>
                </div> */}

                <button type='submit' className='register-btn' disabled={loading}>
                    {loading ? 'Registering...':'Register'}
                </button>
            </form>

            <p className='login-link'>
                Already have an account? <Link to='/Login'>Sign In</Link>
            </p>
        </div>

    </div>
  )
}

export default Register