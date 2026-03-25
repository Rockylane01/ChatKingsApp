import { useState } from 'react'
import type { User } from './types'

interface AccountPageProps {
  currentUser: User
  onUserUpdated: (user: User) => void
  onGoHome: () => void
  onGoChats: () => void
  onLogout: () => void
}

export default function AccountPage({ currentUser, onUserUpdated, onGoHome, onGoChats, onLogout }: AccountPageProps) {
  // Profile fields
  const [username, setUsername] = useState(currentUser.username)
  const [email, setEmail] = useState(currentUser.email)
  const [phone, setPhone] = useState(currentUser.phone_number ?? '')
  const [addCode, setAddCode] = useState(currentUser.add_code)
  const [profileImageUrl, setProfileImageUrl] = useState(currentUser.profile_image_url ?? '')
  const [profileSaving, setProfileSaving] = useState(false)
  const [profileMsg, setProfileMsg] = useState<{ type: 'success' | 'error'; text: string } | null>(null)

  // Password fields
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [passwordSaving, setPasswordSaving] = useState(false)
  const [passwordMsg, setPasswordMsg] = useState<{ type: 'success' | 'error'; text: string } | null>(null)

  async function saveProfile(e: { preventDefault: () => void }) {
    e.preventDefault()
    setProfileMsg(null)
    setProfileSaving(true)

    try {
      const res = await fetch(`/api/users/${currentUser.user_id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          username,
          email,
          phone_number: phone || null,
          add_code: addCode,
          profile_image_url: profileImageUrl || null,
          lifetime_points: currentUser.lifetime_points,
        }),
      })

      if (!res.ok) {
        const text = await res.text()
        setProfileMsg({ type: 'error', text: text || 'Failed to save profile.' })
        return
      }

      const updated: User = await res.json()
      sessionStorage.setItem('currentUser', JSON.stringify(updated))
      onUserUpdated(updated)
      setProfileMsg({ type: 'success', text: 'Profile updated.' })
    } catch {
      setProfileMsg({ type: 'error', text: 'Network error.' })
    } finally {
      setProfileSaving(false)
    }
  }

  async function savePassword(e: { preventDefault: () => void }) {
    e.preventDefault()
    setPasswordMsg(null)

    if (newPassword.length < 8) {
      setPasswordMsg({ type: 'error', text: 'Password must be at least 8 characters.' })
      return
    }
    if (newPassword !== confirmPassword) {
      setPasswordMsg({ type: 'error', text: 'Passwords do not match.' })
      return
    }

    setPasswordSaving(true)

    try {
      const res = await fetch(`/api/users/${currentUser.user_id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          username: currentUser.username,
          email: currentUser.email,
          phone_number: currentUser.phone_number,
          add_code: currentUser.add_code,
          profile_image_url: currentUser.profile_image_url,
          lifetime_points: currentUser.lifetime_points,
          password: newPassword,
        }),
      })

      if (!res.ok) {
        setPasswordMsg({ type: 'error', text: 'Failed to update password.' })
        return
      }

      setNewPassword('')
      setConfirmPassword('')
      setPasswordMsg({ type: 'success', text: 'Password updated.' })
    } catch {
      setPasswordMsg({ type: 'error', text: 'Network error.' })
    } finally {
      setPasswordSaving(false)
    }
  }

  const avatarSrc = profileImageUrl || currentUser.profile_image_url

  return (
    <div className="account-page">
      <nav className="top-nav">
        <div className="top-nav-left">
          <span className="brand-mark">ChatKings</span>
        </div>
        <div className="top-nav-links">
          <button type="button" className="nav-link-button" onClick={onGoHome}>Home</button>
          <button type="button" className="nav-link-button" onClick={onGoChats}>Chats</button>
          <button type="button" className="nav-link-button" onClick={onLogout}>Sign Out</button>
        </div>
      </nav>

      {/* Profile header */}
      <div className="account-card">
        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
          <div className="account-avatar">
            {avatarSrc
              ? <img src={avatarSrc} alt="Profile" style={{ width: '100%', height: '100%', objectFit: 'cover', borderRadius: '50%' }} />
              : <span>{currentUser.username.slice(0, 2).toUpperCase()}</span>
            }
          </div>
          <div>
            <div style={{ fontWeight: 700, fontSize: '1.1rem' }}>{currentUser.username}</div>
            <div style={{ color: '#9ca3af', fontSize: '0.85rem' }}>{currentUser.email}</div>
            <div style={{ color: '#6b7280', fontSize: '0.8rem', marginTop: '0.2rem' }}>
              Add code: <span style={{ color: '#d1d5db', fontFamily: 'monospace' }}>{currentUser.add_code}</span>
              &nbsp;·&nbsp;{currentUser.lifetime_points.toLocaleString()} pts
            </div>
          </div>
        </div>
      </div>

      {/* Profile info */}
      <div className="account-card">
        <h2 className="account-section-title">Profile Info</h2>
        <form className="create-user-form" onSubmit={saveProfile}>
          <div className="modal-grid">
            <div className="modal-row">
              <label className="modal-label" htmlFor="acc-username">Username</label>
              <input id="acc-username" className="modal-control" type="text" value={username}
                onChange={e => setUsername(e.target.value)} required />
            </div>
            <div className="modal-row">
              <label className="modal-label" htmlFor="acc-add-code">Add Code</label>
              <input id="acc-add-code" className="modal-control" type="text" value={addCode}
                onChange={e => setAddCode(e.target.value)} required />
              <span className="modal-help">Friends use this to add you.</span>
            </div>
          </div>

          <div className="modal-row">
            <label className="modal-label" htmlFor="acc-email">Email</label>
            <input id="acc-email" className="modal-control" type="email" value={email}
              onChange={e => setEmail(e.target.value)} required />
          </div>

          <div className="modal-row">
            <label className="modal-label" htmlFor="acc-phone">Phone Number</label>
            <input id="acc-phone" className="modal-control" type="tel" placeholder="Optional"
              value={phone} onChange={e => setPhone(e.target.value)} />
          </div>

          <div className="modal-row">
            <label className="modal-label" htmlFor="acc-avatar">Profile Image URL</label>
            <input id="acc-avatar" className="modal-control" type="url" placeholder="https://…"
              value={profileImageUrl} onChange={e => setProfileImageUrl(e.target.value)} />
            {profileImageUrl && (
              <div style={{ marginTop: '0.5rem' }}>
                <img src={profileImageUrl} alt="Preview"
                  style={{ width: 56, height: 56, borderRadius: '50%', objectFit: 'cover', border: '2px solid rgba(148,163,184,0.3)' }}
                  onError={e => { (e.target as HTMLImageElement).style.display = 'none' }}
                />
              </div>
            )}
          </div>

          {profileMsg && (
            <div className={profileMsg.type === 'success' ? 'create-user-success' : 'modal-error'}>
              {profileMsg.text}
            </div>
          )}

          <div className="modal-actions">
            <button type="submit" className="modal-primary-button" disabled={profileSaving}
              style={{ flex: 1, padding: '0.7rem' }}>
              {profileSaving ? 'Saving…' : 'Save Profile'}
            </button>
          </div>
        </form>
      </div>

      {/* Change password */}
      <div className="account-card">
        <h2 className="account-section-title">Change Password</h2>
        <form className="create-user-form" onSubmit={savePassword}>
          <div className="modal-row">
            <label className="modal-label" htmlFor="acc-new-pw">New Password</label>
            <input id="acc-new-pw" className="modal-control" type="password"
              placeholder="Min. 8 characters" value={newPassword}
              onChange={e => setNewPassword(e.target.value)} minLength={8} required />
          </div>

          <div className="modal-row">
            <label className="modal-label" htmlFor="acc-confirm-pw">Confirm New Password</label>
            <input id="acc-confirm-pw" className="modal-control" type="password"
              placeholder="Re-enter new password" value={confirmPassword}
              onChange={e => setConfirmPassword(e.target.value)} required />
          </div>

          {passwordMsg && (
            <div className={passwordMsg.type === 'success' ? 'create-user-success' : 'modal-error'}>
              {passwordMsg.text}
            </div>
          )}

          <div className="modal-actions">
            <button type="submit" className="modal-primary-button" disabled={passwordSaving}
              style={{ flex: 1, padding: '0.7rem' }}>
              {passwordSaving ? 'Updating…' : 'Update Password'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
