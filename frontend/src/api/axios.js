import axios from 'axios'

const api = axios.create({
    baseURL : 'https://projecttaskmanager-api-bfb8f7facydedefs.centralindia-01.azurewebsites.net/api',
})


//This interceptors attach access token to every request
api.interceptors.request.use((config)=>{
    const token = localStorage.getItem('token')
    if(token)
        config.headers.Authorization = `Bearer ${token}`
    return config
})

//interceptors handle 401 error
api.interceptors.response.use(
    
        res => res,
    async (error) => {
        const originalRequest = error.config

        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true

            try {
                const refreshToken = localStorage.getItem("refreshToken")
                const userId = localStorage.getItem("userId")

                const response = await axios.post(
                    "https://projecttaskmanager-api-bfb8f7facydedefs.centralindia-01.azurewebsites.net/api/auth/refresh-token", 
                    { userId, refreshToken }
                )

                localStorage.setItem("token", response.data.accessToken)
                localStorage.setItem("refreshToken", response.data.refreshToken)

                originalRequest.headers.Authorization =
                  `Bearer ${response.data.accessToken}`

                return api(originalRequest)

            } catch (err) {
                // force logout
                localStorage.clear()
                window.location.href = "/login"
            }
        }
    //     console.log(" 401 detected");
    // console.log("calling refresh token");

        return Promise.reject(error)
})

export default api