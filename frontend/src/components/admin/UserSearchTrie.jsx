import { useState, useEffect } from 'react'
import api from '../../api/axios'
import './UserSearchTrie.css'

let trieBuilt = false


const UserSearchTrie = ({ onSelectUser, initialValue = '' }) => {
    const [query, setQuery] = useState(initialValue) //  shows existing user in edit mode
    const [results, setResults] = useState([])
    const [loading, setLoading] = useState(false)


    // Set initial value when editing
    useEffect(() => {
        setQuery(initialValue)
        setResults([])//clear old dropdown results
    }, [initialValue])

    // Search as user types
    useEffect(() => {
        if (query.length < 1) { setResults([]); return }

        const delay = setTimeout(async () => {
            setLoading(true)
            try {
                const res = await api.get(`/usersearch/search?prefix=${query}`)
                setResults(res.data)
            } catch (err) {
                console.error(err)
            } finally {
                setLoading(false)
            }
        }, 300)

        return () => clearTimeout(delay)
    }, [query])

    return (
        <div className='user-search'>
            <input
                type='text'
                placeholder='Search user to assign...'
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                className='search-input'
            />

            {loading && <div className='searching'>Searching...</div>}

            {results.length > 0 && (
                <ul className='search-results'>
                    {results.map(user => (
                        <li
                            key={user.id}
                            onClick={() => {
                                onSelectUser(user)
                                setQuery(user.username) // ✅ show selected name in input
                                setResults([])           // ✅ close dropdown
                            }}
                            className='search-result-item'
                        >
                            <span className='username'>{user.username}</span>
                            <span className='email'>{user.email}</span>
                        </li>
                    ))}
                </ul>
            )}

            {results.length === 0 && query.length > 0 && !loading && (
                <div className='no-results'>No users found</div>
            )}
        </div>
    )
}

export default UserSearchTrie