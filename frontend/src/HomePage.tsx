import type { User } from './types'

interface HomePageProps {
  currentUser: User
  onOpenChats: () => void
  onOpenAccount: () => void
  onLogout: () => void
}

const liveEvents = [
  { league: 'NBA', matchup: 'Lakers vs Warriors', score: '102 - 99', status: 'Q4 6:12' },
  { league: 'NFL', matchup: 'Chiefs vs Bills', score: '24 - 21', status: 'Q3 2:04' },
  { league: 'MLB', matchup: 'Yankees vs Red Sox', score: '5 - 4', status: 'Top 8' },
  { league: 'NHL', matchup: 'Rangers vs Bruins', score: '3 - 2', status: '3rd 11:09' },
  { league: 'NCAAF', matchup: 'Texas vs Alabama', score: '17 - 20', status: 'Q2 0:42' },
]

const currentBets = [
  {
    id: 1,
    friend: 'Mason',
    matchup: 'Lakers vs Warriors',
    pick: 'Lakers -4.5',
    points: 25,
    status: 'Pending',
  },
  {
    id: 2,
    friend: 'Ava',
    matchup: 'Chiefs vs Bills',
    pick: 'Over 48.5',
    points: 15,
    status: 'Winning',
  },
  {
    id: 3,
    friend: 'Jordan',
    matchup: 'Yankees vs Red Sox',
    pick: 'Red Sox ML',
    points: 20,
    status: 'Pending',
  },
]

export default function HomePage({
  currentUser,
  onOpenChats,
  onOpenAccount,
  onLogout,
}: HomePageProps) {
  return (
    <div className="dashboard-page">
      <nav className="top-nav">
        <div className="top-nav-left">
          <span className="brand-mark">ChatKings</span>
        </div>
        <div className="top-nav-links">
          <button type="button" className="nav-link-button" onClick={onOpenChats}>
            Chats
          </button>
          <button type="button" className="nav-link-button" onClick={onOpenAccount}>
            Account
          </button>
          <button type="button" className="nav-link-button" onClick={onLogout}>
            Sign Out
          </button>
        </div>
      </nav>

      <section className="score-ticker" aria-label="Live sporting events">
        <div className="score-ticker-track">
          {[...liveEvents, ...liveEvents].map((event, idx) => (
            <article className="score-ticker-item" key={`${event.matchup}-${idx}`}>
              <span className="score-league">{event.league}</span>
              <span className="score-matchup">{event.matchup}</span>
              <span className="score-line">{event.score}</span>
              <span className="score-status">{event.status}</span>
            </article>
          ))}
        </div>
      </section>

      <section className="dashboard-hero">
        <div className="dashboard-card">
          <div className="home-logo">CK</div>
          <h1 className="home-title">Welcome back, {currentUser.username}</h1>
          <p className="home-subtitle">
            Pick your games, drop predictions, and compete with your friends using points only.
          </p>

          <section className="current-bets" aria-label="Current bets with friends">
            <div className="current-bets-header">
              <h2>Current Bets With Friends</h2>
              <span className="current-bets-count">{currentBets.length} active</span>
            </div>
            <div className="current-bets-list">
              {currentBets.map((bet) => (
                <article className="current-bet-card" key={bet.id}>
                  <div className="current-bet-top">
                    <span className="current-bet-friend">vs {bet.friend}</span>
                    <span className={`current-bet-status ${bet.status.toLowerCase()}`}>
                      {bet.status}
                    </span>
                  </div>
                  <p className="current-bet-matchup">{bet.matchup}</p>
                  <p className="current-bet-pick">{bet.pick}</p>
                  <p className="current-bet-points">{bet.points} pts at stake</p>
                </article>
              ))}
            </div>
          </section>

          <div className="home-actions">
            <button type="button" className="modal-primary-button home-btn" onClick={onOpenChats}>
              Open Team Chats
            </button>
            <button type="button" className="modal-secondary-button home-btn" onClick={onOpenAccount}>
              Go to Account
            </button>
          </div>
        </div>
      </section>
    </div>
  )
}

