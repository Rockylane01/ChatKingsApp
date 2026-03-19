import { useState } from 'react'
import './App.css'
import Chat from './Chat'
import CreateUser from './CreateUser'
import Login from './Login'

interface User {
  user_id: number
  username: string
  email: string
  phone_number: string | null
  add_code: string
  profile_image_url: string | null
  all_time_points: number
}

function App() {
  const [page, setPage] = useState<'home' | 'chat' | 'create-user' | 'login'>('home')
  const [currentUser, setCurrentUser] = useState<User | null>(null)

  if (page === 'chat') {
    return <Chat />
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
          setPage('chat')
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
        {currentUser && (
          <p style={{ marginTop: '1rem', fontSize: '0.85rem', color: '#9ca3af' }}>
            Signed in as <strong style={{ color: '#e5e7eb' }}>{currentUser.username}</strong>
          </p>
        )}
      </div>
    </div>
  );
}

export default App;
