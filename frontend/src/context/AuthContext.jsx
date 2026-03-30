import { useContext, useState , createContext} from 'react'

////context object with thedefault value null
const AuthContext  = createContext()

//provider will be a component which will wrap the rest of the application and provides auth context to all of its childern
// it accept children prop
export const AuthProvider = ({children}) => {
    // takes the data form the browser storage if not found null
    const [user ,setUser] = useState(
        JSON.parse(localStorage.getItem('user')) || null
    )
    const [token ,setToken] = useState(
        localStorage.getItem('token') || null
    )

    //login function
    const login = (userData , accessToken , refreshToken) =>{

        setUser(userData)
        setToken(accessToken)
        localStorage.setItem('user', JSON.stringify(userData))
        localStorage.setItem('token', accessToken)
        localStorage.setItem('refreshToken', refreshToken)

    }

    //logout function
    const logout= () => {
        setUser(null)
        setToken(null)
        localStorage.removeItem('user')
        localStorage.removeItem('token')
        localStorage.removeItem('refreshToken')
    }

    return (

        <AuthContext.Provider value ={{user, token, login, logout}}>
            {children}
        </AuthContext.Provider>
    )
}

//userAuth hook shortcut instead of user = useContext(AuthContext) -> we use user = UseAuth()

export const useAuth=()=>{
    return useContext(AuthContext)
}

