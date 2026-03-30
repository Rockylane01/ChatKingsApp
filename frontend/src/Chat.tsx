import { useCallback, useEffect, useRef, useState } from 'react';
import './App.css';
import { apiUrl } from './apiBase';
import type { User } from './types';

type Message = {
  id: number;
  sender: string;
  senderUserId: number;
  text: string;
  timestamp: string;
  isOwn: boolean;
};

type Member = {
  user_id: number;
  username: string;
  points_balance: number;
  is_king: boolean;
};

type Sport = 'Basketball' | 'Football';
type PredictionCategory = 'Points' | 'Stats';

type Prediction = {
  sport: Sport;
  category: PredictionCategory;
  text: string;
  minPoints: number;
  maxPoints: number;
  dueBy: string;
  createdAt: string;
  createdBy: string;
};

type BetResponse = {
  bet_id: number;
  chat_id: number;
  game_id: number;
  user_id: number;
  bet_category: string;
  prediction_details_json: string;
  points_wagered: number;
  status: string;
  placed_at: string;
  resolved_at: string | null;
};

interface ChatProps {
  currentUser: User;
  chatId: number;
  onBack: () => void;
}

export default function Chat({ currentUser, chatId, onBack }: ChatProps) {
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState('');
  const [isPredictionOpen, setIsPredictionOpen] = useState(false);
  const [currentPrediction, setCurrentPrediction] = useState<Prediction | null>(
    null
  );
  const [currentBetId, setCurrentBetId] = useState<number | null>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isEditingPrediction, setIsEditingPrediction] = useState(false);
  const [memberNames, setMemberNames] = useState<Map<number, string>>(
    new Map()
  );
  const [kingUsername, setKingUsername] = useState<string | null>(null);
  const [kingUserId, setKingUserId] = useState<number | null>(null);
  const [isCurrentUserKing, setIsCurrentUserKing] = useState(false);
  const [members, setMembers] = useState<Member[]>([]);
  const [isMembersOpen, setIsMembersOpen] = useState(false);
  const [predictionDraft, setPredictionDraft] = useState({
    sport: 'Basketball' as Sport,
    category: 'Points' as PredictionCategory,
    text: '',
    minPoints: 10,
    maxPoints: 100,
    dueBy: '',
  });
  const [predictionTouched, setPredictionTouched] = useState(false);
  const predictionSportRef = useRef<HTMLSelectElement | null>(null);
  const lastMessageIdRef = useRef<number>(0);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const closePrediction = useCallback(() => {
    setIsPredictionOpen(false);
    setPredictionTouched(false);
    setIsEditingPrediction(false);
    setSubmitError(null);
  }, []);

  // Fetch members for name resolution + king info
  useEffect(() => {
    fetch(apiUrl(`/api/chats/${chatId}/members`))
      .then((res) => res.json())
      .then((data: Member[]) => {
        const map = new Map<number, string>();
        data.forEach((m) => map.set(m.user_id, m.username));
        setMemberNames(map);
        setMembers(data);

        const king = data.find((m) => m.is_king);
        if (king) {
          setKingUsername(king.username);
          setKingUserId(king.user_id);
          setIsCurrentUserKing(king.user_id === currentUser.user_id);
        } else {
          setKingUsername(null);
          setKingUserId(null);
          setIsCurrentUserKing(false);
        }
      })
      .catch(() => {});
  }, [chatId, currentUser.user_id]);

  const resolveMessage = useCallback(
    (m: {
      message_id: number;
      user_id: number;
      message_text: string;
      sent_at: string;
    }) => ({
      id: m.message_id,
      sender:
        m.user_id === currentUser.user_id
          ? 'You'
          : (memberNames.get(m.user_id) ?? 'Unknown'),
      senderUserId: m.user_id,
      text: m.message_text,
      timestamp: new Date(m.sent_at).toLocaleTimeString([], {
        hour: 'numeric',
        minute: '2-digit',
      }),
      isOwn: m.user_id === currentUser.user_id,
    }),
    [currentUser.user_id, memberNames]
  );

  // Initial message fetch
  useEffect(() => {
    fetch(apiUrl(`/api/messages?chatId=${chatId}`))
      .then((res) => res.json())
      .then(
        (
          data: Array<{
            message_id: number;
            user_id: number;
            message_text: string;
            sent_at: string;
          }>
        ) => {
          setMessages(data.map(resolveMessage));
          if (data.length > 0) {
            lastMessageIdRef.current = Math.max(
              ...data.map((m) => m.message_id)
            );
          }
        }
      )
      .catch(() => {});
  }, [chatId, resolveMessage]);

  // Polling for new messages every 3 seconds
  useEffect(() => {
    const interval = setInterval(async () => {
      try {
        const res = await fetch(
          apiUrl(
            `/api/messages?chatId=${chatId}&after=${lastMessageIdRef.current}`
          )
        );
        if (!res.ok) return;
        const data: Array<{
          message_id: number;
          user_id: number;
          message_text: string;
          sent_at: string;
        }> = await res.json();
        if (data.length === 0) return;

        setMessages((prev) => {
          const existingIds = new Set(prev.map((m) => m.id));
          const newMsgs = data
            .filter((m) => !existingIds.has(m.message_id))
            .map(resolveMessage);
          return newMsgs.length > 0 ? [...prev, ...newMsgs] : prev;
        });

        lastMessageIdRef.current = Math.max(
          lastMessageIdRef.current,
          ...data.map((m) => m.message_id)
        );
      } catch {
        // swallow network errors during polling
      }
    }, 3000);

    return () => clearInterval(interval);
  }, [chatId, resolveMessage]);

  // Auto-scroll to bottom on new messages
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  useEffect(() => {
    fetch(apiUrl(`/api/bets?chatId=${chatId}`))
      .then((res) => res.json())
      .then((bets: BetResponse[]) => {
        const pending = bets.find((b) => b.status === 'pending');
        if (!pending) return;
        const [sport, category] = pending.bet_category.split(':') as [
          Sport,
          PredictionCategory,
        ];
        const details = JSON.parse(pending.prediction_details_json) as {
          text: string;
          dueBy: string;
          minPoints: number;
          maxPoints: number;
        };
        setCurrentPrediction({
          sport,
          category,
          text: details.text,
          minPoints: details.minPoints,
          maxPoints: details.maxPoints,
          dueBy: details.dueBy,
          createdAt: new Date(pending.placed_at).toLocaleTimeString([], {
            hour: 'numeric',
            minute: '2-digit',
          }),
          createdBy: 'You',
        });
        setCurrentBetId(pending.bet_id);
      })
      .catch(() => {
        /* backend not reachable yet */
      });
  }, []);

  useEffect(() => {
    if (!isPredictionOpen) return;

    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape') closePrediction();
    };

    const previousOverflow = document.body.style.overflow;
    document.addEventListener('keydown', handleKeyDown);
    document.body.style.overflow = 'hidden';
    predictionSportRef.current?.focus();

    return () => {
      document.removeEventListener('keydown', handleKeyDown);
      document.body.style.overflow = previousOverflow;
    };
  }, [closePrediction, isPredictionOpen]);

  const handleSend = async (e: React.SyntheticEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!input.trim()) return;

    const text = input.trim();
    setInput('');

    try {
      const res = await fetch(apiUrl('/api/messages'), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          chat_id: chatId,
          user_id: currentUser.user_id,
          message_type: 'text',
          message_text: text,
        }),
      });

      if (!res.ok) {
        console.error('Failed to send message:', await res.text());
        return;
      }

      const saved = await res.json();
      const newMessage: Message = {
        id: saved.message_id,
        sender: 'You',
        senderUserId: currentUser.user_id,
        text: saved.message_text,
        timestamp: new Date(saved.sent_at).toLocaleTimeString([], {
          hour: 'numeric',
          minute: '2-digit',
        }),
        isOwn: true,
      };

      setMessages((prev) => [...prev, newMessage]);
      lastMessageIdRef.current = Math.max(
        lastMessageIdRef.current,
        saved.message_id
      );
    } catch {
      console.error('Network error sending message.');
    }
  };

  const predictionErrors = (() => {
    const errors: string[] = [];
    if (!predictionDraft.text.trim()) errors.push('Enter a prediction.');
    if (!predictionDraft.dueBy) errors.push('Add a bet due-by time.');

    const min = Number(predictionDraft.minPoints);
    const max = Number(predictionDraft.maxPoints);

    if (!Number.isFinite(min) || !Number.isFinite(max))
      errors.push('Enter valid point values.');
    if (min < 0 || max < 0) errors.push('Points must be 0 or more.');
    if (Number.isFinite(min) && Number.isFinite(max) && min > max) {
      errors.push(
        'Minimum points must be less than or equal to maximum points.'
      );
    }

    return errors;
  })();

  const canSubmitPrediction = predictionErrors.length === 0;

  const handleSubmitPrediction = async (
    e: React.SyntheticEvent<HTMLFormElement>
  ) => {
    e.preventDefault();
    setPredictionTouched(true);
    if (!canSubmitPrediction) return;

    const predictionDetails = {
      text: predictionDraft.text.trim(),
      dueBy: predictionDraft.dueBy,
      minPoints: Number(predictionDraft.minPoints),
      maxPoints: Number(predictionDraft.maxPoints),
    };

    const betPayload = {
      chat_id: chatId,
      game_id: 1,
      user_id: currentUser.user_id,
      bet_category: `${predictionDraft.sport}:${predictionDraft.category}`,
      prediction_details_json: JSON.stringify(predictionDetails),
      points_wagered: Number(predictionDraft.minPoints),
      status: 'pending',
    };

    try {
      let res: Response;
      if (isEditingPrediction && currentBetId !== null) {
        res = await fetch(apiUrl(`/api/bets/${currentBetId}`), {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ ...betPayload, bet_id: currentBetId }),
        });
      } else {
        res = await fetch(apiUrl('/api/bets'), {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(betPayload),
        });
      }

      if (!res.ok) {
        const msg = await res.text();
        setSubmitError(msg || 'Failed to save bet.');
        return;
      }

      const saved: BetResponse = await res.json();
      const [sport, category] = saved.bet_category.split(':') as [
        Sport,
        PredictionCategory,
      ];
      const details = JSON.parse(saved.prediction_details_json);

      setCurrentPrediction({
        sport,
        category,
        text: details.text,
        minPoints: details.minPoints,
        maxPoints: details.maxPoints,
        dueBy: details.dueBy,
        createdAt: new Date(saved.placed_at).toLocaleTimeString([], {
          hour: 'numeric',
          minute: '2-digit',
        }),
        createdBy:
          saved.user_id === currentUser.user_id
            ? 'You'
            : (memberNames.get(saved.user_id) ?? 'Unknown'),
      });
      setCurrentBetId(saved.bet_id);
      closePrediction();
      setPredictionDraft((prev) => ({ ...prev, text: '', dueBy: '' }));
    } catch {
      setSubmitError('Network error. Please try again.');
    }
  };

  const formatDueBy = (dueBy: string) => {
    const parsed = new Date(dueBy);
    if (!Number.isFinite(parsed.getTime())) return dueBy;
    return parsed.toLocaleString([], {
      month: 'short',
      day: 'numeric',
      hour: 'numeric',
      minute: '2-digit',
    });
  };

  const handleDeletePrediction = async () => {
    if (currentBetId !== null) {
      try {
        await fetch(apiUrl(`/api/bets/${currentBetId}`), {
          method: 'DELETE',
        });
      } catch {
        // best-effort delete; clear locally regardless
      }
    }
    setCurrentPrediction(null);
    setCurrentBetId(null);
  };

  const handleEditPrediction = () => {
    if (!currentPrediction) return;
    setPredictionDraft({
      sport: currentPrediction.sport,
      category: currentPrediction.category,
      text: currentPrediction.text,
      minPoints: currentPrediction.minPoints,
      maxPoints: currentPrediction.maxPoints,
      dueBy: currentPrediction.dueBy,
    });
    setPredictionTouched(false);
    setIsEditingPrediction(true);
    setIsPredictionOpen(true);
  };

  return (
    <div className="chat-page">
      <nav className="top-nav">
        <div className="top-nav-left">
          <span className="brand-mark">ChatKings</span>
        </div>
        <div className="top-nav-links">
          <span
            style={{
              fontSize: '0.8rem',
              color: '#9ca3af',
              marginRight: '0.5rem',
            }}
          >
            {currentUser.username}
          </span>
          <button type="button" className="nav-link-button" onClick={onBack}>
            <svg
              width="14"
              height="14"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2.5"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <path d="M19 12H5M12 5l-7 7 7 7" />
            </svg>
            Back to Chats
          </button>
        </div>
      </nav>

      <header className="chat-header">
        <div className="chat-title-block">
          <div className="chat-avatar">GK</div>
          <div>
            <h1 className="chat-title">Your Team Chat</h1>
            <p className="chat-subtitle">
              Squad up with real friends, place friendly point bets, and track
              who wears the crown.
            </p>
            <div className="chat-title-actions">
              {isCurrentUserKing ? (
                <button
                  type="button"
                  className="make-prediction-button"
                  onClick={() => setIsPredictionOpen(true)}
                >
                  Make a Prediction
                </button>
              ) : (
                <span className="king-only-hint">
                  Only the Chat King can make predictions
                </span>
              )}
              <button
                type="button"
                className="members-button"
                onClick={() => setIsMembersOpen(true)}
              >
                Members ({members.length})
              </button>
            </div>
          </div>
        </div>
        <div className="chat-meta">
          {kingUsername && (
            <span className="chat-pill king-pill">
              {isCurrentUserKing
                ? 'You hold the Crown'
                : `${kingUsername} holds the Crown`}
            </span>
          )}
          <span className="chat-pill live-pill">Live</span>
          <span className="chat-pill points-pill">
            Points Only · Zero Cash Risk
          </span>
        </div>
      </header>

      <main className="chat-layout">
        <section className="chat-panel">
          {currentPrediction && (
            <div className="prediction-banner" role="status" aria-live="polite">
              <div className="prediction-banner-top">
                <span className="prediction-badge">Current Prediction</span>
                <div className="prediction-right">
                  <span className="prediction-meta">
                    {currentPrediction.sport} · {currentPrediction.category} ·
                    Bet due by {formatDueBy(currentPrediction.dueBy)}
                  </span>
                  {currentPrediction.createdBy === 'You' && (
                    <div
                      className="prediction-actions"
                      aria-label="Prediction actions"
                    >
                      <button
                        type="button"
                        className="prediction-icon-button"
                        onClick={handleEditPrediction}
                        aria-label="Edit prediction"
                        title="Edit"
                      >
                        <svg
                          viewBox="0 0 24 24"
                          width="18"
                          height="18"
                          aria-hidden="true"
                          focusable="false"
                        >
                          <path
                            fill="currentColor"
                            d="M16.862 3.487a2.25 2.25 0 0 1 3.182 3.182l-9.94 9.94a2.25 2.25 0 0 1-.953.57l-3.25 1.083a.75.75 0 0 1-.949-.949l1.083-3.25a2.25 2.25 0 0 1 .57-.953l9.94-9.94Zm2.121 1.061a.75.75 0 0 0-1.06 0l-.88.879 1.06 1.061.88-.88a.75.75 0 0 0 0-1.06ZM16.0 7.55l-8.84 8.84a.75.75 0 0 0-.19.317l-.61 1.83 1.83-.61a.75.75 0 0 0 .317-.19l8.84-8.84L16 7.55Z"
                          />
                          <path
                            fill="currentColor"
                            d="M4.5 20.25A1.5 1.5 0 0 1 3 18.75V9A1.5 1.5 0 0 1 4.5 7.5h6a.75.75 0 0 1 0 1.5h-6v9.75h9.75v-6a.75.75 0 0 1 1.5 0v6a1.5 1.5 0 0 1-1.5 1.5H4.5Z"
                            opacity="0.35"
                          />
                        </svg>
                        <span className="prediction-action-text">Edit</span>
                      </button>
                      <button
                        type="button"
                        className="prediction-icon-button danger"
                        onClick={handleDeletePrediction}
                        aria-label="Delete prediction"
                        title="Delete"
                      >
                        <svg
                          viewBox="0 0 24 24"
                          width="18"
                          height="18"
                          aria-hidden="true"
                          focusable="false"
                        >
                          <path
                            fill="currentColor"
                            d="M9 3.75A2.25 2.25 0 0 1 11.25 1.5h1.5A2.25 2.25 0 0 1 15 3.75V5h4.5a.75.75 0 0 1 0 1.5H18.2l-1.02 14.02A2.25 2.25 0 0 1 14.94 22.5H9.06a2.25 2.25 0 0 1-2.24-1.98L5.8 6.5H4.5a.75.75 0 0 1 0-1.5H9V3.75Zm1.5 0V5h3V3.75a.75.75 0 0 0-.75-.75h-1.5a.75.75 0 0 0-.75.75Z"
                          />
                          <path
                            fill="currentColor"
                            d="M9.75 10.5a.75.75 0 0 1 .75.75v7.5a.75.75 0 0 1-1.5 0v-7.5a.75.75 0 0 1 .75-.75Zm4.5 0a.75.75 0 0 1 .75.75v7.5a.75.75 0 0 1-1.5 0v-7.5a.75.75 0 0 1 .75-.75Z"
                            opacity="0.8"
                          />
                        </svg>
                        <span className="prediction-action-text">Delete</span>
                      </button>
                    </div>
                  )}
                </div>
              </div>
              <div className="prediction-banner-body">
                <p className="prediction-text">{currentPrediction.text}</p>
                <div className="prediction-range">
                  Bet range: <strong>{currentPrediction.minPoints}</strong>–
                  <strong>{currentPrediction.maxPoints}</strong> pts
                </div>
              </div>
            </div>
          )}
          <div className="chat-messages">
            {messages.map((m) => (
              <div
                key={m.id}
                className={`chat-message-row ${m.isOwn ? 'own' : 'other'}`}
              >
                {!m.isOwn && (
                  <div className="chat-message-avatar">
                    {m.sender.charAt(0)}
                  </div>
                )}
                <div className="chat-message-bubble">
                  <div className="chat-message-header">
                    <span className="chat-message-sender">
                      {m.sender}
                      {m.senderUserId === kingUserId && (
                        <svg className="crown-icon" viewBox="0 0 24 24" width="14" height="14" aria-label="Chat King">
                          <path fill="#facc15" d="M2.5 19.5h19v2h-19v-2Zm19-11-5.5 4-4-6.5-4 6.5-5.5-4v11h19v-11Z" />
                        </svg>
                      )}
                    </span>
                    <span className="chat-message-time">{m.timestamp}</span>
                  </div>
                  <p className="chat-message-text">{m.text}</p>
                </div>
              </div>
            ))}
            <div ref={messagesEndRef} />
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
              This is your private team locker room. Spin up friendly point
              bets, call your picks, and keep the sweat fun without touching
              real money.
            </p>
          </div>
          <div className="sidebar-card">
            <h3>How Your Team Uses Points</h3>
            <ul>
              <li>Place head-to-head or squad bets in points, never cash.</li>
              <li>
                Set fun stakes: bragging rights, snacks, or picking next
                week&apos;s game.
              </li>
              <li>
                Track who&apos;s hot and who&apos;s on a cold streak — all
                inside ChatKings.
              </li>
            </ul>
          </div>
        </aside>
      </main>

      {isMembersOpen && (
        <div
          className="modal-overlay"
          role="presentation"
          onMouseDown={(e) => {
            if (e.target === e.currentTarget) setIsMembersOpen(false);
          }}
        >
          <div
            className="modal-shell"
            role="dialog"
            aria-modal="true"
            aria-label="Chat Members"
          >
            <div className="modal-header">
              <h2 className="modal-title">Members</h2>
              <button
                type="button"
                className="modal-close-button"
                aria-label="Close"
                onClick={() => setIsMembersOpen(false)}
              >
                ×
              </button>
            </div>
            <div className="modal-body members-list">
              {members.map((m) => (
                <div
                  key={m.user_id}
                  className={`member-row${m.user_id === currentUser.user_id ? ' member-row-self' : ''}`}
                >
                  <div className="member-row-left">
                    <div className="chat-message-avatar" style={{ width: 34, height: 34, fontSize: '0.8rem' }}>
                      {m.username.charAt(0).toUpperCase()}
                    </div>
                    <span className="member-name">
                      {m.username}
                      {m.user_id === currentUser.user_id && (
                        <span className="member-you-tag"> (You)</span>
                      )}
                    </span>
                    {m.is_king && (
                      <span className="member-king-badge">
                        <svg className="crown-icon" viewBox="0 0 24 24" width="14" height="14" aria-label="Chat King">
                          <path fill="#facc15" d="M2.5 19.5h19v2h-19v-2Zm19-11-5.5 4-4-6.5-4 6.5-5.5-4v11h19v-11Z" />
                        </svg>
                        Chat King
                      </span>
                    )}
                  </div>
                  <span className="member-points">{m.points_balance} pts</span>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}

      {isPredictionOpen && (
        <div
          className="modal-overlay"
          role="presentation"
          onMouseDown={(e) => {
            if (e.target === e.currentTarget) closePrediction();
          }}
        >
          <div
            className="modal-shell"
            role="dialog"
            aria-modal="true"
            aria-label="Make a Prediction"
          >
            <div className="modal-header">
              <h2 className="modal-title">
                {isEditingPrediction ? 'Edit Prediction' : 'Make a Prediction'}
              </h2>
              <button
                type="button"
                className="modal-close-button"
                aria-label="Close"
                onClick={closePrediction}
              >
                ×
              </button>
            </div>

            <form className="modal-body" onSubmit={handleSubmitPrediction}>
              <div className="modal-row">
                <label className="modal-label" htmlFor="prediction-sport">
                  Select Sport
                </label>
                <select
                  id="prediction-sport"
                  className="modal-control"
                  ref={predictionSportRef}
                  value={predictionDraft.sport}
                  onChange={(e) =>
                    setPredictionDraft((prev) => ({
                      ...prev,
                      sport: e.target.value as Sport,
                    }))
                  }
                >
                  <option value="Basketball">Basketball</option>
                  <option value="Football">Football</option>
                </select>
              </div>

              <div className="modal-row">
                <label className="modal-label" htmlFor="prediction-category">
                  Prediction Category
                </label>
                <select
                  id="prediction-category"
                  className="modal-control"
                  value={predictionDraft.category}
                  onChange={(e) =>
                    setPredictionDraft((prev) => ({
                      ...prev,
                      category: e.target.value as PredictionCategory,
                    }))
                  }
                >
                  <option value="Points">Points</option>
                  <option value="Stats">Stats</option>
                </select>
              </div>

              <div className="modal-row">
                <label className="modal-label" htmlFor="prediction-text">
                  Enter Your Prediction
                </label>
                <input
                  id="prediction-text"
                  type="text"
                  className="modal-control"
                  placeholder="E.g., Over 100 points, Team A wins, etc."
                  value={predictionDraft.text}
                  onChange={(e) =>
                    setPredictionDraft((prev) => ({
                      ...prev,
                      text: e.target.value,
                    }))
                  }
                  onBlur={() => setPredictionTouched(true)}
                />
              </div>

              <div className="modal-row">
                <label className="modal-label" htmlFor="prediction-dueby">
                  Bet due by
                </label>
                <input
                  id="prediction-dueby"
                  type="datetime-local"
                  className="modal-control"
                  value={predictionDraft.dueBy}
                  onChange={(e) =>
                    setPredictionDraft((prev) => ({
                      ...prev,
                      dueBy: e.target.value,
                    }))
                  }
                  onBlur={() => setPredictionTouched(true)}
                />
                <div className="modal-help">
                  For now, use the game start time so everyone knows when picks
                  lock.
                </div>
              </div>

              <div className="modal-grid">
                <div className="modal-row">
                  <label className="modal-label" htmlFor="prediction-min">
                    Minimum Points to Bet
                  </label>
                  <input
                    id="prediction-min"
                    type="number"
                    className="modal-control"
                    min={0}
                    step={1}
                    value={predictionDraft.minPoints}
                    onChange={(e) =>
                      setPredictionDraft((prev) => ({
                        ...prev,
                        minPoints: Number(e.target.value),
                      }))
                    }
                    onBlur={() => setPredictionTouched(true)}
                  />
                </div>
                <div className="modal-row">
                  <label className="modal-label" htmlFor="prediction-max">
                    Maximum Points to Bet
                  </label>
                  <input
                    id="prediction-max"
                    type="number"
                    className="modal-control"
                    min={0}
                    step={1}
                    value={predictionDraft.maxPoints}
                    onChange={(e) =>
                      setPredictionDraft((prev) => ({
                        ...prev,
                        maxPoints: Number(e.target.value),
                      }))
                    }
                    onBlur={() => setPredictionTouched(true)}
                  />
                </div>
              </div>

              {predictionTouched && predictionErrors.length > 0 && (
                <div className="modal-error" role="alert">
                  {predictionErrors[0]}
                </div>
              )}

              {submitError && (
                <div className="modal-error" role="alert">
                  {submitError}
                </div>
              )}

              <div className="modal-actions">
                <button
                  type="button"
                  className="modal-secondary-button"
                  onClick={closePrediction}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="modal-primary-button"
                  disabled={!canSubmitPrediction}
                >
                  {isEditingPrediction ? 'Save Prediction' : 'Make Prediction'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
