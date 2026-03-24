import { useEffect, useState } from 'react'
import './App.css'
import Chat from './Chat'
import ChatList from './ChatList'
import CreateUser from './CreateUser'
import HomePage from './HomePage'
import Login from './Login'
import type { User } from './types'

function App() {
  const [page, setPage] = useState<
    'home' | 'chat' | 'chat-list' | 'create-user' | 'login' | 'dashboard' | 'account'
  >('home')
  const [currentUser, setCurrentUser] = useState<User | null>(null)
  const [selectedChatId, setSelectedChatId] = useState<number | null>(null)

  // Restore session on mount
  useEffect(() => {
    const stored = sessionStorage.getItem('currentUser')
    if (stored) {
      try {
        const user: User = JSON.parse(stored)
        setCurrentUser(user)
        setPage('dashboard')
      } catch {
        sessionStorage.removeItem('currentUser')
      }
    }
  }, [])

  const handleLogout = () => {
    sessionStorage.removeItem('currentUser')
    setCurrentUser(null)
    setSelectedChatId(null)
    setPage('home')
  }

  if (page === 'chat' && currentUser && selectedChatId) {
    return (
      <Chat
        currentUser={currentUser}
        chatId={selectedChatId}
        onBack={() => setPage('chat-list')}
      />
    )
  }

  if (page === 'chat-list' && currentUser) {
    return (
      <ChatList
        currentUser={currentUser}
        onSelectChat={(chatId: number) => {
          setSelectedChatId(chatId)
          setPage('chat')
        }}
        onLogout={handleLogout}
      />
    )
  }

  if (page === 'create-user') {
    return (
      <CreateUser
        onBack={() => setPage('home')}
        onCreated={user => {
          setCurrentUser(user)
          setPage('dashboard')
        }}
      />
    )
  }

  if (page === 'login') {
    return (
      <Login
        onBack={() => setPage('home')}
        onLogin={user => {
          setCurrentUser(user)
          setPage('dashboard')
        }}
      />
    )
  }

  if (page === 'dashboard' && currentUser) {
    return (
      <HomePage
        currentUser={currentUser}
        onOpenChats={() => setPage('chat-list')}
        onOpenAccount={() => setPage('account')}
        onLogout={handleLogout}
      />
    )
  }

  if (page === 'account' && currentUser) {
    return (
      <div className="account-page">
        <nav className="top-nav">
          <div className="top-nav-left">
            <span className="brand-mark">ChatKings</span>
          </div>
          <div className="top-nav-links">
            <button type="button" className="nav-link-button" onClick={() => setPage('dashboard')}>
              Home
            </button>
            <button type="button" className="nav-link-button" onClick={() => setPage('chat-list')}>
              Chats
            </button>
            <button type="button" className="nav-link-button" onClick={handleLogout}>
              Sign Out
            </button>
          </div>
        </nav>

        <div className="account-card">
          <h1 className="chat-title">Account</h1>
          <p className="chat-subtitle">Account page placeholder (profile/settings coming soon).</p>
          <div className="account-meta">
            <span>Username: {currentUser.username}</span>
            <span>Email: {currentUser.email}</span>
            <span>Add Code: {currentUser.add_code}</span>
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="home-page">
      <div className="home-card">
        <div className="home-logo">CK</div>
        <h1 className="home-title">ChatKings</h1>
        <p className="home-subtitle">Predict. Chat. Compete.</p>
        <div className="home-actions">
          <button className="modal-primary-button home-btn" onClick={() => setPage('login')}>
            Sign In
          </button>
          <button className="modal-secondary-button home-btn" onClick={() => setPage('create-user')}>
            Create Account
          </button>
        </div>
      </div>
    </div>
  )
}

export default App
