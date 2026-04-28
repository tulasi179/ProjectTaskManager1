import React, { useState } from 'react'
import api from './api/axios'
import './Register.css' // reuse your existing styles

const OtpVerification = ({ email, purpose, onSuccess }) => {
    const [code, setCode] = useState('')
    const [error, setError] = useState('')
    const [loading, setLoading] = useState(false)
    const [resending, setResending] = useState(false)
    const [resent, setResent] = useState(false)

    const handleVerify = async (e) => {
        e.preventDefault()
        setLoading(true)
        setError('')

        try {
            await api.post('/otp/verify', {
                email,
                code,
                purpose
            })
            onSuccess() //  navigate to next page
        } catch (err) {
            setError(err.response?.data?.message || 'Invalid or expired OTP.')
        } finally {
            setLoading(false)
        }
    }

    const handleResend = async () => {
        setResending(true)
        setError('')
        setResent(false)

        try {
            await api.post('/otp/send', { email, purpose })
            setResent(true)
        } catch (err) {
            setError('Failed to resend OTP. Please try again.')
        } finally {
            setResending(false)
        }
    }

    return (
        <div className='register-container'>
            <div className='register-box'>
                <h2>Project Task Manager</h2>
                <p className='register-subtitle'>Enter the OTP sent to</p>
                <p><strong>{email}</strong></p>

                 <p style={{ color: '#6b7280', fontSize: '13px', textAlign: 'center', marginBottom: '12px' }}>
                If you don't receive the OTP within 2 minutes, 
                please go back and check your email address.
                </p>

                {error && <div className="error-message">{error}</div>}
                {resent && <div className="success-message">OTP resent successfully!</div>}

                <form onSubmit={handleVerify}>
                    <div className='form-group'>
                        <label>OTP Code</label>
                        <input
                            type='text'
                            value={code}
                            onChange={(e) => setCode(e.target.value)}
                            placeholder='Enter 6-digit OTP'
                            maxLength={6}
                            required
                        />
                    </div>

                    <button type='submit' className='register-btn' disabled={loading}>
                        {loading ? 'Verifying...' : 'Verify OTP'}
                    </button>
                </form>

                <p className='login-link'>
                    Didn't receive the OTP?{' '}
                    <button
                        onClick={handleResend}
                        disabled={resending}
                        style={{ background: 'none', border: 'none', color: '#4f46e5', cursor: 'pointer', padding: 0 }}
                    >
                        {resending ? 'Resending...' : 'Resend OTP'}
                    </button>
                </p>
            </div>
        </div>
    )
}
export default OtpVerification