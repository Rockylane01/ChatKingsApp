import { useEffect, useState } from 'react'
import { apiUrl } from './apiBase'
import type { User, Chat, ChatInvitation } from './types'

interface ChatListProps {
  currentUser: User
  onSelectChat: (chatId: number) => void
  onGoHome: () => void
  onLogout: () => void
}

export default function ChatList({ currentUser, onSelectChat, onGoHome, onLogout }: ChatListProps) {
  const [myChats, setMyChats] = useState<Chat[]>([])
  const [invitations, setInvitations] = useState<ChatInvitation[]>([])
  const [loading, setLoading] = useState(true)
  const [showCreateForm, setShowCreateForm] = useState(false)
  const [newChatName, setNewChatName] = useState('')
  const [newChatTimezone, setNewChatTimezone] = useState('America/New_York')
  const [createError, setCreateError] = useState('')

  const fetchChats = async () => {
    try {
      const [myRes, invRes] = await Promise.all([
        fetch(apiUrl(`/api/chats?userId=${currentUser.user_id}`)),
        fetch(apiUrl(`/api/chats/invitations?userId=${currentUser.user_id}`)),
      ])
      if (myRes.ok) setMyChats(await myRes.json())
      if (invRes.ok) setInvitations(await invRes.json())
    } catch {
      // backend not reachable
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchChats()
  }, [currentUser.user_id])

  const handleCreateChat = async (e: React.FormEvent) => {
    e.preventDefault()
    setCreateError('')

    if (!newChatName.trim()) {
      setCreateError('Chat name is required.')
      return
    }

    try {
      const res = await fetch(apiUrl('/api/chats'), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          chat_name: newChatName.trim(),
          chat_type: 'group',
          created_by_user_id: currentUser.user_id,
          status: 'active',
          timezone: newChatTimezone,
        }),
      })

      if (!res.ok) {
        const text = await res.text()
        setCreateError(text || 'Failed to create chat.')
        return
      }

      const created: Chat = await res.json()
      setNewChatName('')
      setShowCreateForm(false)
      onSelectChat(created.chat_id)
    } catch {
      setCreateError('Network error. Is the backend running?')
    }
  }

  const handleAcceptInvite = async (chatId: number) => {
    try {
      const res = await fetch(
        apiUrl(`/api/chats/${chatId}/accept-invite?userId=${currentUser.user_id}`),
        { method: 'POST' }
      )
      if (res.ok) {
        await fetchChats()
        onSelectChat(chatId)
      }
    } catch {
      // network error
    }
  }

  const handleDeclineInvite = async (chatId: number) => {
    try {
      await fetch(
        apiUrl(`/api/chats/${chatId}/decline-invite?userId=${currentUser.user_id}`),
        { method: 'POST' }
      )
      setInvitations((prev) => prev.filter((i) => i.chat_id !== chatId))
    } catch {
      // network error
    }
  }

  const formatDate = (dateStr: string) => {
    const d = new Date(dateStr)
    if (!Number.isFinite(d.getTime())) return ''
    return d.toLocaleDateString([], { month: 'short', day: 'numeric', year: 'numeric' })
  }

  return (
    <div className="chat-list-page">
      <nav className="top-nav">
        <div className="top-nav-left">
          <span className="brand-mark">ChatKings</span>
        </div>
        <div className="top-nav-links">
          <span style={{ fontSize: '0.8rem', color: '#9ca3af', marginRight: '0.5rem' }}>
            {currentUser.username}
          </span>
          <button type="button" className="nav-link-button" onClick={onGoHome}>Home</button>
          <button type="button" className="nav-link-button" onClick={onLogout}>Sign Out</button>
        </div>
      </nav>

      <div className="chat-list-header">
        <h1 className="chat-list-title">Your Chats</h1>
        <button
          type="button"
          className="modal-primary-button"
          style={{ padding: '0.45rem 1.2rem', fontSize: '0.82rem' }}
          onClick={() => { setShowCreateForm(!showCreateForm); setCreateError('') }}
        >
          {showCreateForm ? 'Cancel' : '+ New Chat'}
        </button>
      </div>

      {showCreateForm && (
        <form className="chat-list-create-form" onSubmit={handleCreateChat}>
          <input
            type="text"
            className="modal-control"
            placeholder="Chat name, e.g. Sunday Squad"
            value={newChatName}
            onChange={(e) => setNewChatName(e.target.value)}
            autoFocus
          />
          <select
            className="modal-control"
            value={newChatTimezone}
            onChange={(e) => setNewChatTimezone(e.target.value)}
          >
            <option value="America/New_York">Eastern (ET)</option>
            <option value="America/Chicago">Central (CT)</option>
            <option value="America/Denver">Mountain (MT)</option>
            <option value="America/Los_Angeles">Pacific (PT)</option>
          </select>
          {createError && <div className="modal-error">{createError}</div>}
          <button type="submit" className="modal-primary-button" style={{ padding: '0.5rem 1.5rem' }}>
            Create Chat
          </button>
        </form>
      )}

      {loading ? (
        <p className="chat-list-empty">Loading chats…</p>
      ) : (
        <>
          {myChats.length === 0 && invitations.length === 0 ? (
            <p className="chat-list-empty">
              You haven't joined any chats yet. Create one or ask a friend to invite you!
            </p>
          ) : (
            <div className="chat-list-grid">
              {myChats.map((chat) => (
                <button
                  key={chat.chat_id}
                  type="button"
                  className="chat-list-item"
                  onClick={() => onSelectChat(chat.chat_id)}
                >
                  <div className="chat-list-item-left">
                    <div className="chat-avatar" style={{ width: 40, height: 40, fontSize: '0.9rem' }}>
                      {(chat.chat_name ?? 'C').charAt(0).toUpperCase()}
                    </div>
                    <div>
                      <div className="chat-list-item-name">{chat.chat_name ?? 'Unnamed Chat'}</div>
                      <div className="chat-list-item-date">Created {formatDate(chat.created_at)}</div>
                    </div>
                  </div>
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <path d="M9 18l6-6-6-6" />
                  </svg>
                </button>
              ))}
            </div>
          )}

          {invitations.length > 0 && (
            <>
              <h2 className="chat-list-section-title">Invitations</h2>
              <div className="chat-list-grid">
                {invitations.map((inv) => (
                  <div key={inv.chat_id} className="chat-list-item" style={{ cursor: 'default' }}>
                    <div className="chat-list-item-left">
                      <div className="chat-avatar" style={{ width: 40, height: 40, fontSize: '0.9rem' }}>
                        {(inv.chat_name ?? 'C').charAt(0).toUpperCase()}
                      </div>
                      <div>
                        <div className="chat-list-item-name">{inv.chat_name ?? 'Unnamed Chat'}</div>
                        <div className="chat-list-item-date">
                          {inv.invited_by_username
                            ? `${inv.invited_by_username} invited you to join`
                            : `Created ${formatDate(inv.created_at)}`}
                        </div>
                      </div>
                    </div>
                    <div style={{ display: 'flex', gap: '0.4rem' }}>
                      <button
                        type="button"
                        className="modal-primary-button"
                        style={{ padding: '0.3rem 0.9rem', fontSize: '0.75rem' }}
                        onClick={() => handleAcceptInvite(inv.chat_id)}
                      >
                        Accept
                      </button>
                      <button
                        type="button"
                        className="nav-link-button"
                        style={{ padding: '0.3rem 0.7rem', fontSize: '0.75rem' }}
                        onClick={() => handleDeclineInvite(inv.chat_id)}
                      >
                        Decline
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </>
          )}
        </>
      )}
    </div>
  )
}
