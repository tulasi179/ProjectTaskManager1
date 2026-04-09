import { useNavigate , Link} from 'react-router-dom'
import api from './api/axios'
import React, { useState } from 'react'
import { useAuth } from './context/AuthContext'
import './Login.css'

const Login = () => {
    const [form, SetForm] = useState({ username : '', password: ''})
    const [error,setError] = useState('')//error handling stores error s like invalid username or password
    const [loading, setLoading] = useState(false)//while handling the request status-> disables the button and it shows sigining in...
    const { login } = useAuth()
    const navigate = useNavigate()// to navigate it to dashboard.

    const handleChange = (e) => {
        SetForm({...form, [e.target.name]: e.target.value})
    }

    //two way binding
    const handleSubmit = async (e) => {
        e.preventDefault()//forms default behaviour that when you submit the page reloads to stop that.
        //stops form form refreshing the page after submitting
        setError('')
        setLoading(true)

        try{
            const res = await api.post("/auth/login",{
                username: form.username,
                password: form.password,
                email:'',
                role:''
            })
    
            const {accessToken , refreshToken } = res.data//extract JWT token from the response


            //Decode the token to get the user information
            const payload = JSON.parse(atob(accessToken.split('.')[1]))
             //from the accesstoken extarct the data of the users like id , username, role
            const userData ={
                id: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
                username: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
                role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
            }

            login(userData, accessToken , refreshToken)
            if(userData.role=='Admin')
                navigate('/admindashboard')
            else
                navigate('/userdashboard')
        } catch(err) {
        const msg = err.response?.data
        setError(typeof msg === 'string' ? msg : 'Invalid username or password.')
        } finally{
                setLoading(false)
        }
        // SetForm("")
    }
  return (
    <div className='login-container'>
        <div className='login-box'>
            <h2> Project Task Manager</h2>
            <p className='login-subtitle'> Sign in to your account</p>

            {error && <div className='error-message'>{error}</div>}

            <form onSubmit={handleSubmit}>

                <div className='form-group'>
                    <label> username</label>
                    <input 
                    type ='text'
                    name = 'username'
                    value ={form.username}
                    onChange={handleChange}
                    placeholder='Enter your username'
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


                <button type='submit' className='login-btn' disabled={loading}>
                    {loading? 'Signing in...': 'Sign In'}
                </button>
            </form>

            <p className='register-link'>
                Dont have an account? <Link to='/register'>Register</Link>
            </p>

        </div>
    </div>
  )
}

export default Login