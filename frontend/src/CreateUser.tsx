import { useState } from 'react'
import { apiUrl } from './apiBase'
import type { User } from './types'

interface CreateUserProps {
  onBack: () => void
  onCreated: (user: User) => void
}

export default function CreateUser({ onBack, onCreated }: CreateUserProps) {
  const [username, setUsername] = useState('')
  const [email, setEmail] = useState('')
  const [phoneNumber, setPhoneNumber] = useState('')
  const [addCode, setAddCode] = useState('')
  const [profileImageUrl, setProfileImageUrl] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')

  const [error, setError] = useState('')
  const [success, setSuccess] = useState(false)
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: { preventDefault: () => void }) {
    e.preventDefault()
    setError('')
    setSuccess(false)

    if (password !== confirmPassword) {
      setError('Passwords do not match.')
      return
    }

    setLoading(true)

    try {
      const res = await fetch(apiUrl('/api/users'), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          username,
          email,
          phone_number: phoneNumber || null,
          add_code: addCode,
          profile_image_url: profileImageUrl || null,
          lifetime_points: 0,
          password,
        }),
      })

      if (!res.ok) {
        if (res.status >= 500) {
          setError('Service is temporarily unavailable. Please try again shortly.')
          return
        }
        const text = await res.text()
        setError(text || 'Failed to create user.')
        return
      }

      const createdUser: User = await res.json()
      sessionStorage.setItem('currentUser', JSON.stringify(createdUser))
      onCreated(createdUser)

      setSuccess(true)
      setUsername('')
      setEmail('')
      setPhoneNumber('')
      setAddCode('')
      setProfileImageUrl('')
      setPassword('')
      setConfirmPassword('')
    } catch (err) {
      setError(toNetworkErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  function toNetworkErrorMessage(err: unknown): string {
    const message = err instanceof Error ? err.message.toLowerCase() : ''
    if (message.includes('failed to fetch') || message.includes('network')) {
      return 'Unable to reach the API right now. Please try again in a minute.'
    }
    return 'Network error. Please try again.'
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
          <div className="chat-avatar" style={{ width: 48, height: 48, fontSize: '1.2rem' }}>U</div>
          <div>
            <h1 className="create-user-title">Create Account</h1>
            <p className="chat-subtitle">Set up a new ChatKings profile</p>
          </div>
        </div>

        <form className="create-user-form" onSubmit={handleSubmit}>
          <div className="modal-grid">
            <div className="modal-row">
              <label className="modal-label" htmlFor="username">Username *</label>
              <input
                id="username"
                className="modal-control"
                type="text"
                placeholder="e.g. kingslayer99"
                value={username}
                onChange={e => setUsername(e.target.value)}
                required
              />
            </div>

            <div className="modal-row">
              <label className="modal-label" htmlFor="add-code">Add Code *</label>
              <input
                id="add-code"
                className="modal-control"
                type="text"
                placeholder="e.g. ABC123"
                value={addCode}
                onChange={e => setAddCode(e.target.value)}
                required
              />
              <span className="modal-help">Friends use this to add you.</span>
            </div>
          </div>

          <div className="modal-row">
            <label className="modal-label" htmlFor="email">Email *</label>
            <input
              id="email"
              className="modal-control"
              type="email"
              placeholder="you@example.com"
              value={email}
              onChange={e => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="modal-row">
            <label className="modal-label" htmlFor="phone">Phone Number</label>
            <input
              id="phone"
              className="modal-control"
              type="tel"
              placeholder="555-123-4567 (optional)"
              value={phoneNumber}
              onChange={e => setPhoneNumber(e.target.value)}
            />
          </div>

          <div className="modal-row">
            <label className="modal-label" htmlFor="profile-image">Profile Image URL</label>
            <input
              id="profile-image"
              className="modal-control"
              type="url"
              placeholder="https://… (optional)"
              value={profileImageUrl}
              onChange={e => setProfileImageUrl(e.target.value)}
            />
          </div>

          <div className="modal-row">
            <label className="modal-label" htmlFor="password">Password *</label>
            <input
              id="password"
              className="modal-control"
              type="password"
              placeholder="Min. 8 characters"
              value={password}
              onChange={e => setPassword(e.target.value)}
              minLength={8}
              required
            />
          </div>

          <div className="modal-row">
            <label className="modal-label" htmlFor="confirm-password">Confirm Password *</label>
            <input
              id="confirm-password"
              className="modal-control"
              type="password"
              placeholder="Re-enter your password"
              value={confirmPassword}
              onChange={e => setConfirmPassword(e.target.value)}
              required
            />
          </div>

          {error && <div className="modal-error">{error}</div>}

          {success && (
            <div className="create-user-success">
              Account created successfully!
            </div>
          )}

          <div className="modal-actions" style={{ justifyContent: 'stretch' }}>
            <button
              type="submit"
              className="modal-primary-button"
              disabled={loading}
              style={{ flex: 1, padding: '0.7rem' }}
            >
              {loading ? 'Creating…' : 'Create Account'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
