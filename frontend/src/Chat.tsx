import { useState } from 'react'
import './App.css'

type Message = {
  id: number
  sender: 'You' | 'Teammate' | 'Coach'
  text: string
  timestamp: string
  isOwn: boolean
}

const placeholderMessages: Message[] = [
  {
    id: 1,
    sender: 'Teammate',
    text: 'Locked in a 25-point friendly bet on the Sunday night game. Loser brings snacks next week.',
    timestamp: '9:12 AM',
    isOwn: false,
  },
  {
    id: 2,
    sender: 'Teammate',
    text: 'I&apos;m tossing 10 points on the under with you instead of hitting the real book. Team parlay energy only.',
    timestamp: '9:14 AM',
    isOwn: false,
  },
  {
    id: 3,
    sender: 'You',
    text: 'Deal. Let&apos;s keep it all in points tonight and see who ends up the Team King by Monday.',
    timestamp: '9:16 AM',
    isOwn: true,
  },
]

export default function Chat() {
  const [messages, setMessages] = useState<Message[]>(placeholderMessages)
  const [input, setInput] = useState('')

  const handleSend = (e: React.FormEvent) => {
    e.preventDefault()
    if (!input.trim()) return

    const newMessage: Message = {
      id: Date.now(),
      sender: 'You',
      text: input.trim(),
      timestamp: new Date().toLocaleTimeString([], {
        hour: 'numeric',
        minute: '2-digit',
      }),
      isOwn: true,
    }

    // This will later be replaced with a call to your backend/database
    setMessages((prev) => [...prev, newMessage])
    setInput('')
  }

  return (
    <div className="chat-page">
      <nav className="top-nav">
        <div className="top-nav-left">
          <span className="brand-mark">ChatKings</span>
        </div>
        <div className="top-nav-links">
          <button
            type="button"
            className="nav-link-button"
            // TODO: Replace with real navigation when the homepage route exists
            onClick={() => {
              // Placeholder for future navigation to Homepage
            }}
          >
            Home (coming soon)
          </button>
        </div>
      </nav>

      <header className="chat-header">
        <div className="chat-title-block">
          <div className="chat-avatar">GK</div>
          <div>
            <h1 className="chat-title">Your Team Chat</h1>
            <p className="chat-subtitle">
              Squad up with real friends, place friendly point bets, and track who wears the crown.
            </p>
          </div>
        </div>
        <div className="chat-meta">
          <span className="chat-pill live-pill">Live</span>
          <span className="chat-pill points-pill">Points Only · Zero Cash Risk</span>
        </div>
      </header>

      <main className="chat-layout">
        <section className="chat-panel">
          <div className="chat-messages">
            {messages.map((m) => (
              <div
                key={m.id}
                className={`chat-message-row ${m.isOwn ? 'own' : 'other'}`}
              >
                {!m.isOwn && (
                  <div className="chat-message-avatar">{m.sender.charAt(0)}</div>
                )}
                <div className="chat-message-bubble">
                  <div className="chat-message-header">
                    <span className="chat-message-sender">{m.sender}</span>
                    <span className="chat-message-time">{m.timestamp}</span>
                  </div>
                  <p className="chat-message-text">{m.text}</p>
                </div>
              </div>
            ))}
          </div>

          <form className="chat-input-row" onSubmit={handleSend}>
            <input
              type="text"
              className="chat-input"
              placeholder="Share your pick, odds, or a new points bet..."
              value={input}
              onChange={(e) => setInput(e.target.value)}
            />
            <button
              type="submit"
              className="chat-send-button"
              disabled={!input.trim()}
            >
              Send
            </button>
          </form>
        </section>

        <aside className="chat-sidebar">
          <div className="sidebar-card">
            <h2>Team Focus</h2>
            <p>
              This is your private team locker room. Spin up friendly point bets, call your picks,
              and keep the sweat fun without touching real money.
            </p>
          </div>
          <div className="sidebar-card">
            <h3>How Your Team Uses Points</h3>
            <ul>
              <li>Place head-to-head or squad bets in points, never cash.</li>
              <li>Set fun stakes: bragging rights, snacks, or picking next week&apos;s game.</li>
              <li>Track who&apos;s hot and who&apos;s on a cold streak — all inside ChatKings.</li>
            </ul>
          </div>
        </aside>
      </main>
    </div>
  )
}

