import api from './api/axios'
import React, { useState } from 'react'
import OtpVerification from './OtpVerification'
import {useNavigate, Link} from 'react-router-dom'
import './Register.css'

const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
const passwordRegex = /^(?=.*[A-Z])(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]).{8,}$/

const Register = () => {

    const [form, setForm] = useState({
        username: '',
        password: '',
        confirmPassword: '',
        email: '',
        role: 'User'
    })
    const [error, setError] = useState('')
    const [loading, setLoading] = useState(false)
    const [showOtp, setShowOtp] = useState(false)
    const navigate = useNavigate()

    const handleChange = (e) => {
        setForm({...form, [e.target.name]: e.target.value})
    }

    const handleSubmit = async (e) => {
        e.preventDefault()
        setLoading(true)
        setError('')

        // Email validation
        if (!emailRegex.test(form.email)) {
            setError('Please enter a valid email address.')
            setLoading(false)
            return
        }

        // Password validation
        if (!passwordRegex.test(form.password)) {
            setError('Password must be at least 8 characters, include one uppercase letter and one special character.')
            setLoading(false)
            return
        }

        // Confirm password
        if (form.password !== form.confirmPassword) {
            setError('Passwords do not match.')
            setLoading(false)
            return
        }

       try {
        await api.post('/auth/register', {
            username: form.username,
            email: form.email,
            password: form.password,
            role: form.role
         })
        } catch (err) {
            setError(err.response?.data || 'Registration failed.')
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
            setError(err.response?.data?.message || 'Failed to send OTP. Check your email.')
            setLoading(false)
            return
        }

        // Step 3 — Show OTP screen
        setLoading(false)
        setShowOtp(true)
    }

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
                <p className='register-subtitle'>Create your account</p>

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

                    {/* Email with live hint */}
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
                        {form.email && (
                            <ul className='password-hints'>
                                <li style={{ color: emailRegex.test(form.email) ? 'green' : 'red' }}>
                                    {emailRegex.test(form.email) ? '✅' : '❌'} Valid email format
                                </li>
                            </ul>
                        )}
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
                                <li style={{ color: form.password === form.confirmPassword ? 'green' : 'red' }}>
                                    {form.password === form.confirmPassword ? '✅' : '❌'} Passwords match
                                </li>
                            </ul>
                        )}
                    </div>

                    <button type='submit' className='register-btn' disabled={loading}>
                        {loading ? 'Registering...' : 'Register'}
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