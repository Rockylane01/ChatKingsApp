import { useEffect, useState } from 'react'
import { apiUrl } from './apiBase'
import type { Chat, TickerGame, User } from './types'

interface HomePageProps {
  currentUser: User
  onOpenChats: () => void
  onOpenAccount: () => void
  onLogout: () => void
}

type WagersTotalResponse = { totalWagers?: number }
type PredictionResponse = {
  prediction_id: number
  chat_id: number
  created_by_user_id: number
  title: string
  status: string
  pot_points: number
  created_at: string
  options: { option_id: number; option_label: string; wager_count: number; total_points: number }[]
}
type HomeBet = {
  id: number
  chatName: string
  category: string
  pick: string
  points: number
  status: string
}

export default function HomePage({
  currentUser,
  onOpenChats,
  onOpenAccount,
  onLogout,
}: HomePageProps) {
  const [tickerGames, setTickerGames] = useState<TickerGame[]>([])
  const [totalWagers, setTotalWagers] = useState<number | null>(null)
  const [currentBets, setCurrentBets] = useState<HomeBet[]>([])
  const [betsLoading, setBetsLoading] = useState(true)

  useEffect(() => {
    let cancelled = false
    ;(async () => {
      try {
        const res = await fetch(apiUrl('/api/scoreboard/ncaam'))
        if (!res.ok) return
        const data = (await res.json()) as TickerGame[]
        if (!Array.isArray(data) || cancelled) return
        setTickerGames(data.filter((g) => g?.id && g?.matchup))
      } catch {
        /* ticker is optional */
      }
    })()
    return () => {
      cancelled = true
    }
  }, [])

  useEffect(() => {
    let cancelled = false
    ;(async () => {
      try {
        const res = await fetch(apiUrl('/api/stats/wagers-total'))
        if (!res.ok) return
        const data = (await res.json()) as WagersTotalResponse
        if (cancelled || typeof data.totalWagers !== 'number') return
        setTotalWagers(data.totalWagers)
      } catch {
        /* stats optional */
      }
    })()
    return () => {
      cancelled = true
    }
  }, [])

  useEffect(() => {
    let cancelled = false
    ;(async () => {
      setBetsLoading(true)
      try {
        const chatsRes = await fetch(apiUrl(`/api/chats?userId=${currentUser.user_id}`))
        if (!chatsRes.ok) {
          if (!cancelled) setCurrentBets([])
          return
        }

        const chats = (await chatsRes.json()) as Chat[]
        if (!Array.isArray(chats) || chats.length === 0) {
          if (!cancelled) setCurrentBets([])
          return
        }

        const predsPerChat = await Promise.all(
          chats.map(async (chat) => {
            try {
              const res = await fetch(apiUrl(`/api/predictions?chatId=${chat.chat_id}`))
              if (!res.ok) return [] as HomeBet[]
              const preds = (await res.json()) as PredictionResponse[]
              if (!Array.isArray(preds)) return [] as HomeBet[]

              return preds.map((pred) => ({
                id: pred.prediction_id,
                chatName: chat.chat_name ?? 'Unnamed Chat',
                category: pred.status === 'resolved' ? 'Resolved' : 'Active',
                pick: pred.title,
                points: pred.pot_points,
                status: pred.status,
                createdAt: pred.created_at,
              }))
            } catch {
              return [] as HomeBet[]
            }
          }),
        )

        if (cancelled) return

        const flattened = predsPerChat
          .flat()
          .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
          .slice(0, 8)
          .map(({ createdAt, ...rest }) => rest)

        setCurrentBets(flattened)
      } finally {
        if (!cancelled) setBetsLoading(false)
      }
    })()

    return () => {
      cancelled = true
    }
  }, [currentUser.user_id])

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

      {tickerGames.length > 0 ? (
        <section className="score-ticker" aria-label="NCAA men's basketball scores">
          <div className="score-ticker-track">
            {[0, 1].flatMap((loop) =>
              tickerGames.map((event) => {
                const stableId =
                  event.id != null && String(event.id).length > 0 ? String(event.id) : `fallback-${event.matchup}`
                return (
                  <article
                    className="score-ticker-item"
                    key={`score-ticker-${loop}-${stableId}`}
                  >
                    <span className="score-league">{event.league}</span>
                    <span className="score-matchup">{event.matchup}</span>
                    <span className="score-line">{event.score}</span>
                    <span className="score-status">{event.status}</span>
                  </article>
                )
              }),
            )}
          </div>
        </section>
      ) : null}

      <section className="dashboard-hero">
        <div className="dashboard-hero-row">
          <div className="dashboard-card">
            <div className="home-logo">CK</div>
            <h1 className="home-title">Welcome back, {currentUser.username}</h1>
            <p className="home-subtitle">
              Pick your games, drop predictions, and compete with your friends using points only.
            </p>

            <section className="current-bets" aria-label="Current bets with friends">
              <div className="current-bets-header">
                <h2>Current Bets With Friends</h2>
                <span className="current-bets-count">{currentBets.length} placed</span>
              </div>
              {betsLoading ? (
                <p className="current-bets-empty">Loading your bets…</p>
              ) : currentBets.length === 0 ? (
                <p className="current-bets-empty">
                  No bets placed yet. Create a prediction in chat and your bets will show up here.
                </p>
              ) : (
                <div className="current-bets-list">
                  {currentBets.map((bet) => (
                    <article className="current-bet-card" key={bet.id}>
                      <div className="current-bet-top">
                        <span className="current-bet-friend">{bet.chatName}</span>
                        <span className={`current-bet-status ${bet.status.toLowerCase()}`}>
                          {bet.status}
                        </span>
                      </div>
                      <p className="current-bet-matchup">{bet.category}</p>
                      <p className="current-bet-pick">{bet.pick}</p>
                      <p className="current-bet-points">{bet.points} pts wagered</p>
                    </article>
                  ))}
                </div>
              )}
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

          <aside className="okr-card" aria-label="Community OKR: total wagers">
            <p className="okr-label">Community OKR</p>
            <p className="okr-objective">Grow engagement through friendly competition</p>
            <p className="okr-value" aria-live="polite">
              {totalWagers === null ? '—' : totalWagers.toLocaleString()}
            </p>
            <p className="okr-metric-name">Total wagers placed</p>
            <p className="okr-footnote">All users · all time</p>
          </aside>
        </div>
      </section>
    </div>
  )
}

