import { useEffect, useState } from 'react'
import type { User, Chat } from './types'

interface ChatListProps {
  currentUser: User
  onSelectChat: (chatId: number) => void
  onLogout: () => void
}

export default function ChatList({ currentUser, onSelectChat, onLogout }: ChatListProps) {
  const [myChats, setMyChats] = useState<Chat[]>([])
  const [allChats, setAllChats] = useState<Chat[]>([])
  const [loading, setLoading] = useState(true)
  const [showCreateForm, setShowCreateForm] = useState(false)
  const [newChatName, setNewChatName] = useState('')
  const [createError, setCreateError] = useState('')
  const [joiningId, setJoiningId] = useState<number | null>(null)

  const fetchChats = async () => {
    try {
      const [myRes, allRes] = await Promise.all([
        fetch(`/api/chats?userId=${currentUser.user_id}`),
        fetch('/api/chats'),
      ])

      if (myRes.ok) setMyChats(await myRes.json())
      if (allRes.ok) setAllChats(await allRes.json())
    } catch {
      // backend not reachable
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchChats()
  }, [currentUser.user_id])

  const myChatIds = new Set(myChats.map((c) => c.chat_id))
  const joinableChats = allChats.filter((c) => !myChatIds.has(c.chat_id))

  const handleCreateChat = async (e: React.FormEvent) => {
    e.preventDefault()
    setCreateError('')

    if (!newChatName.trim()) {
      setCreateError('Chat name is required.')
      return
    }

    try {
      const res = await fetch('/api/chats', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          chat_name: newChatName.trim(),
          chat_type: 'group',
          created_by_user_id: currentUser.user_id,
          status: 'active',
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

  const handleJoinChat = async (chatId: number) => {
    setJoiningId(chatId)
    try {
      const res = await fetch(`/api/chats/${chatId}/join?userId=${currentUser.user_id}`, {
        method: 'POST',
      })

      if (res.ok) {
        await fetchChats()
      }
    } catch {
      // network error
    } finally {
      setJoiningId(null)
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
          <button type="button" className="nav-link-button" onClick={onLogout}>
            Sign Out
          </button>
        </div>
      </nav>

      <div className="chat-list-header">
        <h1 className="chat-list-title">Your Chats</h1>
        <button
          type="button"
          className="modal-primary-button"
          style={{ padding: '0.45rem 1.2rem', fontSize: '0.82rem' }}
          onClick={() => setShowCreateForm(!showCreateForm)}
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
          {myChats.length === 0 ? (
            <p className="chat-list-empty">
              You haven't joined any chats yet. Create one or join an existing chat below!
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

          {joinableChats.length > 0 && (
            <>
              <h2 className="chat-list-section-title">Join a Chat</h2>
              <div className="chat-list-grid">
                {joinableChats.map((chat) => (
                  <div key={chat.chat_id} className="chat-list-item" style={{ cursor: 'default' }}>
                    <div className="chat-list-item-left">
                      <div className="chat-avatar" style={{ width: 40, height: 40, fontSize: '0.9rem' }}>
                        {(chat.chat_name ?? 'C').charAt(0).toUpperCase()}
                      </div>
                      <div>
                        <div className="chat-list-item-name">{chat.chat_name ?? 'Unnamed Chat'}</div>
                        <div className="chat-list-item-date">Created {formatDate(chat.created_at)}</div>
                      </div>
                    </div>
                    <button
                      type="button"
                      className="modal-primary-button"
                      style={{ padding: '0.3rem 0.9rem', fontSize: '0.75rem' }}
                      disabled={joiningId === chat.chat_id}
                      onClick={() => handleJoinChat(chat.chat_id)}
                    >
                      {joiningId === chat.chat_id ? 'Joining…' : 'Join'}
                    </button>
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
