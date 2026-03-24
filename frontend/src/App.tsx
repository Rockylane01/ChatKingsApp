import { useEffect, useState } from 'react'
import './App.css'
import Chat from './Chat'
import ChatList from './ChatList'
import CreateUser from './CreateUser'
import Login from './Login'
import type { User } from './types'

interface User {
  user_id: number
  username: string
  email: string
  phone_number: string | null
  add_code: string
  profile_image_url: string | null
  lifetime_points: number
}

function App() {
  const [page, setPage] = useState<'home' | 'chat' | 'chat-list' | 'create-user' | 'login'>('home')
  const [currentUser, setCurrentUser] = useState<User | null>(null)
  const [selectedChatId, setSelectedChatId] = useState<number | null>(null)

  // Restore session on mount
  useEffect(() => {
    const stored = sessionStorage.getItem('currentUser')
    if (stored) {
      try {
        const user: User = JSON.parse(stored)
        setCurrentUser(user)
        setPage('chat-list')
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
    return <CreateUser onBack={() => setPage('home')} />
  }

  if (page === 'login') {
    return (
      <Login
        onBack={() => setPage('home')}
        onLogin={user => {
          setCurrentUser(user)
          setPage('chat-list')
        }}
      />
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
  );
}

export default App;
