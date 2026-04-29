import React, { useState } from 'react'
import api from './api/axios'
import { useNavigate, Link } from 'react-router-dom'
import OtpVerification from './OtpVerification'
import './Register.css'

const ForgotPassword = () => {
    const [email, setEmail] = useState('')
    const [newPassword, setNewPassword] = useState('')
    const [error, setError] = useState('')
    const [loading, setLoading] = useState(false)
    const [showOtp, setShowOtp] = useState(false)
    const [otpVerified, setOtpVerified] = useState(false)
    const navigate = useNavigate()

    //enter email, send OTP
    const handleSendOtp = async (e) => {
        e.preventDefault()
        setLoading(true)
        setError('')

        try {
            await api.post('/otp/send', {
                email,
                purpose: 'forgot-password'
            })
            setShowOtp(true)
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to send OTP.')
        } finally {
            setLoading(false)
        }
    }

    //after OTP verified, reset password
    const handleResetPassword = async (e) => {
        e.preventDefault()
        setLoading(true)
        setError('')

        try {
            await api.post('/auth/reset-password', {
                email,
                newPassword
            })
            navigate('/login')
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to reset password.')
        } finally {
            setLoading(false)
        }
    }

    //show OTP screen
    if (showOtp && !otpVerified) {
        return (
            <OtpVerification
                email={email}
                purpose="forgot-password"
                onSuccess={() => setOtpVerified(true)} // show reset password form
            />
        )
    }

    // show new password form
    if (otpVerified) {
        return (
            <div className='register-container'>
                <div className='register-box'>
                    <h2>Project Task Manager</h2>
                    <p className='register-subtitle'>Set your new password</p>

                    {error && <div className="error-message">{error}</div>}

                    <form onSubmit={handleResetPassword}>
                        <div className='form-group'>
                            <label>New Password</label>
                            <input
                                type='password'
                                value={newPassword}
                                onChange={(e) => setNewPassword(e.target.value)}
                                placeholder='Enter new password'
                                required
                            />
                        </div>

                        <button type='submit' className='register-btn' disabled={loading}>
                            {loading ? 'Resetting...' : 'Reset Password'}
                        </button>
                    </form>
                </div>
            </div>
        )
    }

    // enter email
    return (
        <div className='register-container'>
            <div className='register-box'>
                <h2>Project Task Manager</h2>
                <p className='register-subtitle'>Forgot your password?</p>

                {error && <div className="error-message">{error}</div>}

                <form onSubmit={handleSendOtp}>
                    <div className='form-group'>
                        <label>Email</label>
                        <input
                            type='email'
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder='Enter your registered email'
                            required
                        />
                    </div>

                    <button type='submit' className='register-btn' disabled={loading}>
                        {loading ? 'Sending OTP...' : 'Send OTP'}
                    </button>
                </form>

                <p className='login-link'>
                    Remember your password? <Link to='/login'>Sign In</Link>
                </p>
            </div>
        </div>
    )
}

export default ForgotPassword