import { useState, useEffect } from 'react'
import api from '../../api/axios'
import './UserSearchTrie.css'

const UserSearchTrie = ({ onSelectUser, initialValue = '' }) => {
    const [query, setQuery] = useState(initialValue)
    const [results, setResults] = useState([])
    const [loading, setLoading] = useState(false)
    const [selected, setSelected] = useState(!!initialValue) // tracks if user is selected

    useEffect(() => {
        setQuery(initialValue)
        setResults([])
        setSelected(!!initialValue) // if initialValue exists, mark as selected
    }, [initialValue])

    useEffect(() => {
        if (selected) return // don't search if user already selected
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
    }, [query, selected])

    return (
        <div className='user-search'>
            <input
                type='text'
                placeholder='Search user to assign...'
                value={query}
                onChange={(e) => {
                    setQuery(e.target.value)
                    setSelected(false) // user is typing again, reset selected
                }}
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
                                setQuery(user.username)
                                setResults([])
                                setSelected(true) // mark as selected
                            }}
                            className='search-result-item'
                        >
                            <span className='username'>{user.username}</span>
                            <span className='email'>{user.email}</span>
                        </li>
                    ))}
                </ul>
            )}

            {/* Only show "No users found" when NOT selected */}
            {!selected && results.length === 0 && query.length > 0 && !loading && (
                <div className='no-results'>No users found</div>
            )}
        </div>
    )
}

export default UserSearchTrie