import { useState } from 'react'
// import { apiUrl } from './apiBase'
import type { User } from './types'

interface LoginProps {
  onLogin: (user: User) => void
  onBack: () => void
}

export default function Login({ onLogin, onBack }: LoginProps) {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: { preventDefault: () => void }) {
    e.preventDefault()
    setError('')
    setLoading(true)

    try {
      const res = await fetch('/api/users/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password }),
      })

      if (res.status === 401) {
        setError('Invalid username or password.')
        return
      }

      if (!res.ok) {
        setError('Something went wrong. Try again.')
        return
      }

      const user: User = await res.json()
      sessionStorage.setItem('currentUser', JSON.stringify(user))
      onLogin(user)
    } catch {
      setError('Network error. Is the backend running?')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="create-user-page">
      <div className="top-nav">
        <span className="brand-mark">ChatKings</span>
        <div className="top-nav-links">
          <button className="nav-link-button" onClick={onBack}>
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
              <path d="M19 12H5M12 5l-7 7 7 7" />
            </svg>
            Back
          </button>
        </div>
      </div>

      <div className="create-user-card">
        <div className="create-user-header">
          <div className="chat-avatar" style={{ width: 48, height: 48, fontSize: '1.2rem' }}>
            <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
              <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
              <circle cx="12" cy="7" r="4" />
            </svg>
          </div>
          <div>
            <h1 className="create-user-title">Sign In</h1>
            <p className="chat-subtitle">Welcome back to ChatKings</p>
          </div>
        </div>

        <form className="create-user-form" onSubmit={handleSubmit}>
          <div className="modal-row">
            <label className="modal-label" htmlFor="login-username">Username</label>
            <input
              id="login-username"
              className="modal-control"
              type="text"
              placeholder="e.g. kingslayer99"
              value={username}
              onChange={e => setUsername(e.target.value)}
              required
              autoFocus
            />
          </div>

          <div className="modal-row">
            <label className="modal-label" htmlFor="login-password">Password</label>
            <input
              id="login-password"
              className="modal-control"
              type="password"
              placeholder="Your password"
              value={password}
              onChange={e => setPassword(e.target.value)}
              required
            />
          </div>

          {error && <div className="modal-error">{error}</div>}

          <div className="modal-actions">
            <button
              type="submit"
              className="modal-primary-button"
              disabled={loading}
              style={{ flex: 1, padding: '0.7rem' }}
            >
              {loading ? 'Signing in…' : 'Sign In'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
