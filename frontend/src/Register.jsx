import api from './api/axios'
import React, { useState } from 'react'
import Login from './Login'
import {useNavigate, Link} from 'react-router-dom'
import './Register.css'

const Register = () => {

    const [form ,setForm] = useState({
        username: '',
        password: '',
        email: '',
        role: 'User'//default user.
    })
    const [error, setError] = useState('')
    const [loading, setLoading]=useState(false)//while handling the request
    const navigate = useNavigate()// to navigate to other pages after registering

    const handleChange = (e) => {
        setForm({...form, [e.target.name]: e.target.value})
    }

    const handleSubmit = async (e) => {
        e.preventDefault()
        setLoading(true)

        try{
            await api.post('/auth/register', form)
            navigate('/login')
        }catch(err) {
            setError(err.response?.data || 'Registration failed. Please try again.')
        } finally{
            setLoading(false)
        }
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
                </div>


                <div className='form-group'>
                    <label>Role</label>
                    <select 
                    name='role' 
                    value={form.role} 
                    onChange={handleChange}>

                        <option value='User'>User</option>
                        <option value='Admin'>Admin</option>
                    </select>
                </div>

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