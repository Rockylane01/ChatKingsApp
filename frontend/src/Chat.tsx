import { useCallback, useEffect, useRef, useState } from 'react';
import './App.css';
import { apiUrl } from './apiBase';
import type {
  User,
  TickerGame,
  PredictionDetail,
  PredictionOption,
  LeaderboardEntry,
  StrikeInfo,
} from './types';

/* ------------------------------------------------------------------ */
/*  Local types                                                        */
/* ------------------------------------------------------------------ */

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

type RawMessage = {
  message_id: number;
  chat_id: number;
  user_id: number;
  message_type: string;
  message_text: string;
  prediction_id: number | null;
  sent_at: string;
};

type ModalStep = 'pick-game' | 'configure' | 'creator-wager';

/* ------------------------------------------------------------------ */
/*  Props                                                              */
/* ------------------------------------------------------------------ */

interface ChatProps {
  currentUser: User;
  chatId: number;
  onBack: () => void;
}

/* ------------------------------------------------------------------ */
/*  Helpers                                                            */
/* ------------------------------------------------------------------ */

function formatLockTime(iso: string | null): string {
  if (!iso) return '';
  const d = new Date(iso);
  if (!Number.isFinite(d.getTime())) return iso;
  return d.toLocaleString([], {
    month: 'short',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
  });
}

function parseTeamsFromMatchup(matchup: string): [string, string] {
  // "Duke @ UNC" or "Duke vs UNC"
  const sep = matchup.includes(' @ ') ? ' @ ' : ' vs ';
  const parts = matchup.split(sep);
  if (parts.length >= 2) return [parts[0].trim(), parts[1].trim()];
  return [matchup, ''];
}

function toLocalDatetimeString(iso: string): string {
  const d = new Date(iso);
  if (!Number.isFinite(d.getTime())) return '';
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

/* ------------------------------------------------------------------ */
/*  Component                                                          */
/* ------------------------------------------------------------------ */

export default function Chat({ currentUser, chatId, onBack }: ChatProps) {
  /* ---- messaging state ---- */
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState('');
  const [memberNames, setMemberNames] = useState<Map<number, string>>(new Map());
  const [kingUsername, setKingUsername] = useState<string | null>(null);
  const [kingUserId, setKingUserId] = useState<number | null>(null);
  const [isCurrentUserKing, setIsCurrentUserKing] = useState(false);
  const [members, setMembers] = useState<Member[]>([]);
  const [isMembersOpen, setIsMembersOpen] = useState(false);

  // Invite state (members modal)
  const [inviteQuery, setInviteQuery] = useState('');
  const [inviteResults, setInviteResults] = useState<{ user_id: number; username: string }[]>([]);
  const [inviteStatus, setInviteStatus] = useState<Record<number, 'sending' | 'sent' | 'already'>>({});
  const [pendingInvites, setPendingInvites] = useState<{ user_id: number; username: string }[]>([]);
  const [isLeaveConfirmOpen, setIsLeaveConfirmOpen] = useState(false);
  const inviteSearchTimeout = useRef<ReturnType<typeof setTimeout> | null>(null);
  const lastMessageIdRef = useRef<number>(0);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  /* ---- prediction state ---- */
  const [activePrediction, setActivePrediction] = useState<PredictionDetail | null>(null);
  const [resolvedPredictions, setResolvedPredictions] = useState<PredictionDetail[]>([]);
  const [wagerOptionId, setWagerOptionId] = useState<number | null>(null);
  const [wagerPoints, setWagerPoints] = useState<number>(0);
  const [wagerError, setWagerError] = useState<string | null>(null);
  const [showWagerForm, setShowWagerForm] = useState(false);

  /* ---- create-prediction modal state ---- */
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [modalStep, setModalStep] = useState<ModalStep>('pick-game');
  const [games, setGames] = useState<TickerGame[]>([]);
  const [gamesLoading, setGamesLoading] = useState(false);
  const [selectedGame, setSelectedGame] = useState<TickerGame | null>(null);
  const [isCustom, setIsCustom] = useState(false);
  const [draftTitle, setDraftTitle] = useState('');
  const [draftOptionA, setDraftOptionA] = useState('');
  const [draftOptionB, setDraftOptionB] = useState('');
  const [draftLockAt, setDraftLockAt] = useState('');
  const [draftBetMin, setDraftBetMin] = useState(20);
  const [draftBetMax, setDraftBetMax] = useState(150);
  const [creatorOptionIndex, setCreatorOptionIndex] = useState<0 | 1>(0);
  const [creatorWagerAmount, setCreatorWagerAmount] = useState(20);
  const [modalError, setModalError] = useState<string | null>(null);

  /* ---- sidebar state ---- */
  const [leaderboard, setLeaderboard] = useState<LeaderboardEntry[]>([]);
  const [strikeInfo, setStrikeInfo] = useState<StrikeInfo | null>(null);
  const [historyOpen, setHistoryOpen] = useState(false);

  /* ================================================================ */
  /*  Data fetching                                                    */
  /* ================================================================ */

  // Fetch member names
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
    (m: RawMessage): Message => ({
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
    [currentUser.user_id, memberNames],
  );

  // Initial message fetch
  useEffect(() => {
    fetch(apiUrl(`/api/messages?chatId=${chatId}`))
      .then((res) => res.json())
      .then((data: RawMessage[]) => {
        setMessages(data.map(resolveMessage));
        if (data.length > 0) {
          lastMessageIdRef.current = Math.max(...data.map((m) => m.message_id));
        }
      })
      .catch(() => {});
  }, [chatId, resolveMessage]);

  // Poll messages every 3s
  useEffect(() => {
    const interval = setInterval(async () => {
      try {
        const res = await fetch(
          apiUrl(`/api/messages?chatId=${chatId}&after=${lastMessageIdRef.current}`),
        );
        if (!res.ok) return;
        const data: RawMessage[] = await res.json();
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
          ...data.map((m) => m.message_id),
        );
      } catch {
        // swallow network errors during polling
      }
    }, 3000);
    return () => clearInterval(interval);
  }, [chatId, resolveMessage]);

  // Auto-scroll on new messages
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  /* ---- Predictions fetching ---- */

  const fetchActivePrediction = useCallback(async () => {
    try {
      const res = await fetch(apiUrl(`/api/predictions?chatId=${chatId}&status=pending`));
      if (!res.ok) return;
      const data: PredictionDetail[] = await res.json();
      setActivePrediction(data.length > 0 ? data[0] : null);
    } catch {
      // swallow
    }
  }, [chatId]);

  const fetchResolvedPredictions = useCallback(async () => {
    try {
      const res = await fetch(apiUrl(`/api/predictions?chatId=${chatId}&status=resolved`));
      if (!res.ok) return;
      const data: PredictionDetail[] = await res.json();
      setResolvedPredictions(data);
    } catch {
      // swallow
    }
  }, [chatId]);

  // Initial prediction fetch
  useEffect(() => {
    fetchActivePrediction();
    fetchResolvedPredictions();
  }, [fetchActivePrediction, fetchResolvedPredictions]);

  // Poll active prediction every 5s
  useEffect(() => {
    const interval = setInterval(fetchActivePrediction, 5000);
    return () => clearInterval(interval);
  }, [fetchActivePrediction]);

  /* ---- Leaderboard & Strikes fetching ---- */

  const fetchLeaderboard = useCallback(async () => {
    try {
      const res = await fetch(apiUrl(`/api/chats/${chatId}/leaderboard`));
      if (!res.ok) return;
      const data: LeaderboardEntry[] = await res.json();
      setLeaderboard(data);
    } catch {
      // swallow
    }
  }, [chatId]);

  const fetchStrikes = useCallback(async () => {
    try {
      const res = await fetch(
        apiUrl(`/api/chats/${chatId}/strikes?userId=${currentUser.user_id}`),
      );
      if (!res.ok) return;
      const data: StrikeInfo = await res.json();
      setStrikeInfo(data);
    } catch {
      // swallow
    }
  }, [chatId, currentUser.user_id]);

  useEffect(() => {
    fetchLeaderboard();
    fetchStrikes();
  }, [fetchLeaderboard, fetchStrikes]);

  // Poll leaderboard every 10s
  useEffect(() => {
    const interval = setInterval(fetchLeaderboard, 10000);
    return () => clearInterval(interval);
  }, [fetchLeaderboard]);

  /* ================================================================ */
  /*  Handlers                                                         */
  /* ================================================================ */

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
      lastMessageIdRef.current = Math.max(lastMessageIdRef.current, saved.message_id);
    } catch {
      console.error('Network error sending message.');
    }
  };

  /* ---- Wager placement ---- */

  const userWager = activePrediction?.wagers?.find(
    (w) => w.user_id === currentUser.user_id,
  );

  const handlePlaceWager = async () => {
    if (!activePrediction || wagerOptionId === null) return;
    setWagerError(null);

    try {
      const res = await fetch(apiUrl(`/api/predictions/${activePrediction.prediction_id}/wager`), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          user_id: currentUser.user_id,
          option_id: wagerOptionId,
          points_wagered: wagerPoints,
        }),
      });

      if (!res.ok) {
        const text = await res.text();
        setWagerError(text || 'Failed to place wager.');
        return;
      }

      setShowWagerForm(false);
      setWagerOptionId(null);
      setWagerPoints(0);
      await fetchActivePrediction();
    } catch {
      setWagerError('Network error. Please try again.');
    }
  };

  /* ---- Delete / Edit active prediction ---- */

  const handleDeletePrediction = async () => {
    if (!activePrediction) return;
    try {
      await fetch(apiUrl(`/api/predictions/${activePrediction.prediction_id}`), {
        method: 'DELETE',
      });
    } catch {
      // best-effort
    }
    setActivePrediction(null);
  };

  const handleEditPrediction = () => {
    if (!activePrediction) return;
    // Open modal in configure step with current values pre-filled
    setIsCustom(true);
    setSelectedGame(null);
    setDraftTitle(activePrediction.title);
    const opts = activePrediction.options;
    setDraftOptionA(opts[0]?.option_label ?? '');
    setDraftOptionB(opts[1]?.option_label ?? '');
    setDraftLockAt(activePrediction.lock_at ? toLocalDatetimeString(activePrediction.lock_at) : '');
    setDraftBetMin(activePrediction.initial_bet_min);
    setDraftBetMax(activePrediction.initial_bet_max);
    setModalStep('configure');
    setModalError(null);
    setIsModalOpen(true);
  };

  /* ---- Create Prediction Modal ---- */

  const openCreateModal = () => {
    setSelectedGame(null);
    setIsCustom(false);
    setDraftTitle('');
    setDraftOptionA('');
    setDraftOptionB('');
    setDraftLockAt('');
    setDraftBetMin(20);
    setDraftBetMax(150);
    setCreatorOptionIndex(0);
    setCreatorWagerAmount(20);
    setModalStep('pick-game');
    setModalError(null);
    setIsModalOpen(true);

    // Fetch games
    setGamesLoading(true);
    fetch(apiUrl('/api/scoreboard/ncaam'))
      .then((res) => res.json())
      .then((data: TickerGame[]) => setGames(data))
      .catch(() => setGames([]))
      .finally(() => setGamesLoading(false));
  };

  const closeModal = useCallback(() => {
    setIsModalOpen(false);
    setModalError(null);
  }, []);

  // Escape key and body scroll lock for modal
  useEffect(() => {
    if (!isModalOpen) return;

    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape') closeModal();
    };

    const prevOverflow = document.body.style.overflow;
    document.addEventListener('keydown', handleKeyDown);
    document.body.style.overflow = 'hidden';

    return () => {
      document.removeEventListener('keydown', handleKeyDown);
      document.body.style.overflow = prevOverflow;
    };
  }, [isModalOpen, closeModal]);

  const handleSelectGame = (game: TickerGame) => {
    setSelectedGame(game);
    setIsCustom(false);
    const [teamA, teamB] = parseTeamsFromMatchup(game.matchup);
    setDraftTitle(`Who wins: ${game.matchup}?`);
    setDraftOptionA(teamA);
    setDraftOptionB(teamB);
    // Try to use the game status as a lock hint; default empty
    setDraftLockAt('');
    setModalStep('configure');
  };

  const handleSelectCustom = () => {
    setSelectedGame(null);
    setIsCustom(true);
    setDraftTitle('');
    setDraftOptionA('');
    setDraftOptionB('');
    setDraftLockAt('');
    setModalStep('configure');
  };

  const handleConfigureNext = () => {
    if (!draftTitle.trim() || !draftOptionA.trim() || !draftOptionB.trim()) {
      setModalError('Fill in the prediction title and both options.');
      return;
    }
    if (draftBetMin <= 0 || draftBetMax <= 0 || draftBetMin > draftBetMax) {
      setModalError('Enter valid bet min/max values.');
      return;
    }
    setModalError(null);
    setCreatorWagerAmount(draftBetMin);
    setModalStep('creator-wager');
  };

  const handleSubmitPrediction = async () => {
    if (creatorWagerAmount < draftBetMin || creatorWagerAmount > draftBetMax) {
      setModalError(`Wager must be between ${draftBetMin} and ${draftBetMax}.`);
      return;
    }
    setModalError(null);

    const payload = {
      chat_id: chatId,
      user_id: currentUser.user_id,
      title: draftTitle.trim(),
      espn_event_id: selectedGame?.id ?? null,
      lock_at: draftLockAt ? new Date(draftLockAt).toISOString() : null,
      initial_bet_min: draftBetMin,
      initial_bet_max: draftBetMax,
      options: [
        { option_label: draftOptionA.trim(), team_id: null, display_order: 1 },
        { option_label: draftOptionB.trim(), team_id: null, display_order: 2 },
      ],
      creator_wager: {
        option_index: creatorOptionIndex,
        points_wagered: creatorWagerAmount,
      },
    };

    try {
      const res = await fetch(apiUrl('/api/predictions'), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        const text = await res.text();
        setModalError(text || 'Failed to create prediction.');
        return;
      }

      closeModal();
      await fetchActivePrediction();
    } catch {
      setModalError('Network error. Please try again.');
    }
  };


  // Debounced invite search — tries username and add code simultaneously
  useEffect(() => {
    if (inviteSearchTimeout.current) clearTimeout(inviteSearchTimeout.current);
    if (inviteQuery.trim().length < 2) { setInviteResults([]); return; }
    inviteSearchTimeout.current = setTimeout(async () => {
      try {
        const q = inviteQuery.trim();
        console.log('[invite-search] searching for:', q);
        const memberIds = new Set(members.map((m) => m.user_id));
        const seen = new Set<number>();
        const combined: { user_id: number; username: string }[] = [];

        const [usernameRes, codeRes] = await Promise.allSettled([
          fetch(apiUrl(`/api/users/search?username=${encodeURIComponent(q)}&excludeUserId=${currentUser.user_id}`)),
          fetch(apiUrl(`/api/users/by-add-code/${encodeURIComponent(q)}`)),
        ]);

        if (usernameRes.status === 'fulfilled' && usernameRes.value.ok) {
          const results: { user_id: number; username: string }[] = await usernameRes.value.json();
          for (const u of results) {
            if (!memberIds.has(u.user_id) && !seen.has(u.user_id)) {
              combined.push(u);
              seen.add(u.user_id);
            }
          }
        }

        if (codeRes.status === 'fulfilled' && codeRes.value.ok) {
          const u: { user_id: number; username: string } = await codeRes.value.json();
          if (u.user_id !== currentUser.user_id && !memberIds.has(u.user_id) && !seen.has(u.user_id)) {
            combined.push(u);
          }
        }

        console.log('[invite-search] combined results:', combined, 'memberIds:', [...memberIds]);
        setInviteResults(combined);
      } catch (err) { console.error('[invite-search] error:', err); }
    }, 300);
  }, [inviteQuery, members, currentUser.user_id]);

  const handleInviteUser = async (userId: number) => {
    setInviteStatus((prev) => ({ ...prev, [userId]: 'sending' }));
    try {
      const res = await fetch(apiUrl(`/api/chats/${chatId}/invite`), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ invited_user_id: userId, invited_by_user_id: currentUser.user_id }),
      });
      if (res.ok) {
        setInviteStatus((prev) => ({ ...prev, [userId]: 'sent' }));
        const invited = inviteResults.find((u) => u.user_id === userId);
        if (invited) setPendingInvites((prev) => [...prev, invited]);
      } else {
        setInviteStatus((prev) => ({ ...prev, [userId]: 'already' }));
      }
    } catch {
      setInviteStatus((prev) => ({ ...prev, [userId]: 'already' }));
    }
  };

  const handleLeaveChat = async () => {
    try {
      const res = await fetch(apiUrl(`/api/chats/${chatId}/leave?userId=${currentUser.user_id}`), { method: 'POST' });
      if (res.ok) onBack();
    } catch { /* ignore */ }
  };

  /* ================================================================ */
  /*  Derived data                                                     */
  /* ================================================================ */

  const myLeaderboardEntry = leaderboard.find((e) => e.user_id === currentUser.user_id);

  /* ================================================================ */
  /*  Render helpers                                                   */
  /* ================================================================ */

  const renderPredictionBanner = () => {
    if (!activePrediction) return null;

    const isCreator = activePrediction.created_by_user_id === currentUser.user_id;
    const isLocked =
      activePrediction.lock_at && new Date(activePrediction.lock_at) <= new Date();

    return (
      <div className="prediction-banner" role="status" aria-live="polite">
        <div className="prediction-banner-top">
          <span className="prediction-badge">Current Prediction</span>
          <div className="prediction-right">
            {activePrediction.lock_at && (
              <span className="prediction-meta">
                Locks at {formatLockTime(activePrediction.lock_at)}
              </span>
            )}
            {isCreator && (
              <div className="prediction-actions" aria-label="Prediction actions">
                <button
                  type="button"
                  className="prediction-icon-button"
                  onClick={handleEditPrediction}
                  aria-label="Edit prediction"
                  title="Edit"
                >
                  <svg viewBox="0 0 24 24" width="18" height="18" aria-hidden="true">
                    <path
                      fill="currentColor"
                      d="M16.862 3.487a2.25 2.25 0 0 1 3.182 3.182l-9.94 9.94a2.25 2.25 0 0 1-.953.57l-3.25 1.083a.75.75 0 0 1-.949-.949l1.083-3.25a2.25 2.25 0 0 1 .57-.953l9.94-9.94Z"
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
                  <svg viewBox="0 0 24 24" width="18" height="18" aria-hidden="true">
                    <path
                      fill="currentColor"
                      d="M9 3.75A2.25 2.25 0 0 1 11.25 1.5h1.5A2.25 2.25 0 0 1 15 3.75V5h4.5a.75.75 0 0 1 0 1.5H18.2l-1.02 14.02A2.25 2.25 0 0 1 14.94 22.5H9.06a2.25 2.25 0 0 1-2.24-1.98L5.8 6.5H4.5a.75.75 0 0 1 0-1.5H9V3.75Z"
                    />
                  </svg>
                  <span className="prediction-action-text">Delete</span>
                </button>
              </div>
            )}
          </div>
        </div>

        <div className="prediction-banner-body">
          <p className="prediction-text">{activePrediction.title}</p>

          <div style={{ display: 'flex', gap: '1rem', marginTop: '0.75rem' }}>
            {activePrediction.options.map((opt) => {
              const isUserChoice = userWager?.option_id === opt.option_id;
              return (
                <div
                  key={opt.option_id}
                  className={`prediction-option-card${isUserChoice ? ' prediction-option-selected' : ''}`}
                >
                  <div style={{ fontWeight: 600 }}>
                    {opt.option_label}
                    {isUserChoice && ' \u2713'}
                  </div>
                  <div style={{ fontSize: '0.85rem', color: '#9ca3af' }}>
                    {opt.total_points} pts wagered &middot; {opt.wager_count} bettor
                    {opt.wager_count !== 1 ? 's' : ''}
                  </div>
                </div>
              );
            })}
          </div>

          {/* Wager form — only if user hasn't wagered and not locked */}
          {!userWager && !isLocked && (
            <div style={{ marginTop: '0.75rem' }}>
              {!showWagerForm ? (
                <button
                  type="button"
                  className="make-prediction-button"
                  style={{ fontSize: '0.85rem', padding: '0.4rem 1rem' }}
                  onClick={() => {
                    setShowWagerForm(true);
                    setWagerPoints(activePrediction.initial_bet_min);
                    setWagerOptionId(activePrediction.options[0]?.option_id ?? null);
                  }}
                >
                  Place Wager
                </button>
              ) : (
                <div className="wager-form">
                  <div style={{ display: 'flex', gap: '1rem', alignItems: 'center', flexWrap: 'wrap' }}>
                    {activePrediction.options.map((opt) => (
                      <label key={opt.option_id} style={{ display: 'flex', alignItems: 'center', gap: '0.3rem', cursor: 'pointer' }}>
                        <input
                          type="radio"
                          name="wager-option"
                          checked={wagerOptionId === opt.option_id}
                          onChange={() => setWagerOptionId(opt.option_id)}
                        />
                        {opt.option_label}
                      </label>
                    ))}
                  </div>
                  <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center', marginTop: '0.5rem' }}>
                    <input
                      type="number"
                      className="wager-input"
                      min={activePrediction.initial_bet_min}
                      max={activePrediction.initial_bet_max}
                      value={wagerPoints}
                      onChange={(e) => setWagerPoints(Number(e.target.value))}
                    />
                    <span style={{ fontSize: '0.8rem', color: '#9ca3af' }}>
                      ({activePrediction.initial_bet_min}–{activePrediction.initial_bet_max} pts)
                    </span>
                    <button
                      type="button"
                      className="modal-primary-button"
                      style={{ padding: '0.4rem 1rem', fontSize: '0.85rem' }}
                      onClick={handlePlaceWager}
                    >
                      Submit
                    </button>
                    <button
                      type="button"
                      className="modal-secondary-button"
                      style={{ padding: '0.4rem 1rem', fontSize: '0.85rem' }}
                      onClick={() => {
                        setShowWagerForm(false);
                        setWagerError(null);
                      }}
                    >
                      Cancel
                    </button>
                  </div>
                  {wagerError && (
                    <div className="modal-error" role="alert" style={{ marginTop: '0.4rem' }}>
                      {wagerError}
                    </div>
                  )}
                </div>
              )}
            </div>
          )}

          <div className="prediction-range" style={{ marginTop: '0.5rem' }}>
            Bet range: <strong>{activePrediction.initial_bet_min}</strong>–
            <strong>{activePrediction.initial_bet_max}</strong> pts
          </div>
        </div>
      </div>
    );
  };

  const renderSidebar = () => (
    <aside className="chat-sidebar">
      {/* Leaderboard */}
      <div className="sidebar-card">
        <h2>Leaderboard</h2>
        {leaderboard.length === 0 ? (
          <p style={{ fontSize: '0.85rem', color: '#9ca3af' }}>No data yet.</p>
        ) : (
          <div className="leaderboard-list">
            {leaderboard.map((entry, idx) => {
              const isMe = entry.user_id === currentUser.user_id;
              return (
                <div
                  key={entry.user_id}
                  className={`leaderboard-row${entry.is_king ? ' leaderboard-king' : ''}${isMe ? ' leaderboard-me' : ''}`}
                >
                  <span style={{ width: '1.5rem', textAlign: 'right', marginRight: '0.5rem' }}>
                    {idx + 1}.
                  </span>
                  <span style={{ flex: 1 }}>
                    {entry.is_king && '\uD83D\uDC51 '}
                    {entry.username}
                    {isMe && ' (You)'}
                  </span>
                  <span style={{ fontWeight: 600 }}>{entry.points_balance} pts</span>
                </div>
              );
            })}
          </div>
        )}
      </div>

      {/* Your Stats */}
      <div className="sidebar-card">
        <h3>Your Stats</h3>
        <p style={{ fontSize: '0.9rem', marginBottom: '0.5rem' }}>
          Points: <strong>{myLeaderboardEntry?.points_balance ?? '—'}</strong>
        </p>
        {strikeInfo && (
          <div>
            <p style={{ fontSize: '0.9rem', marginBottom: '0.3rem' }}>Strikes:</p>
            <div className="strike-dots">
              {Array.from({ length: strikeInfo.max_strikes }).map((_, i) => {
                const used = i < strikeInfo.strikes_today;
                const colors = ['#eab308', '#f97316', '#ef4444'];
                return (
                  <span
                    key={i}
                    className={`strike-dot${used ? ' strike-dot-used' : ''}`}
                    style={used ? { backgroundColor: colors[i] ?? '#ef4444' } : undefined}
                  />
                );
              })}
            </div>
            {strikeInfo.locked && (
              <p style={{ fontSize: '0.8rem', color: '#ef4444', marginTop: '0.3rem' }}>
                Locked out — max strikes reached.
              </p>
            )}
          </div>
        )}
      </div>

      {/* Prediction History */}
      <div className="sidebar-card">
        <h3
          style={{ cursor: 'pointer', userSelect: 'none' }}
          onClick={() => setHistoryOpen((v) => !v)}
        >
          Prediction History {historyOpen ? '\u25B2' : '\u25BC'}
        </h3>
        {historyOpen && (
          <div className="prediction-history">
            {resolvedPredictions.length === 0 ? (
              <p style={{ fontSize: '0.85rem', color: '#9ca3af' }}>No resolved predictions.</p>
            ) : (
              resolvedPredictions.map((pred) => {
                const winningOptId = pred.resolution?.winning_option_id;
                const winningOpt = pred.options.find((o) => o.option_id === winningOptId);
                const myWager = pred.wagers?.find((w) => w.user_id === currentUser.user_id);
                const didWin = myWager && myWager.option_id === winningOptId;
                const didLose = myWager && myWager.option_id !== winningOptId;

                return (
                  <div key={pred.prediction_id} className="prediction-history-item">
                    <div style={{ fontWeight: 500 }}>{pred.title}</div>
                    <div style={{ fontSize: '0.8rem', color: '#9ca3af' }}>
                      Winner: {winningOpt?.option_label ?? '—'}
                    </div>
                    {myWager && (
                      <div
                        style={{
                          fontSize: '0.8rem',
                          fontWeight: 600,
                          color: didWin ? '#22c55e' : didLose ? '#ef4444' : '#9ca3af',
                        }}
                      >
                        {didWin ? 'Won' : didLose ? 'Lost' : '—'}
                      </div>
                    )}
                  </div>
                );
              })
            )}
          </div>
        )}
      </div>
    </aside>
  );

  const renderModal = () => {
    if (!isModalOpen) return null;

    return (
      <div
        className="modal-overlay"
        role="presentation"
        onMouseDown={(e) => {
          if (e.target === e.currentTarget) closeModal();
        }}
      >
        <div className="modal-shell" role="dialog" aria-modal="true" aria-label="Make a Prediction">
          <div className="modal-header">
            <h2 className="modal-title">Make a Prediction</h2>
            <button
              type="button"
              className="modal-close-button"
              aria-label="Close"
              onClick={closeModal}
            >
              &times;
            </button>
          </div>

          <div className="modal-body">
            {/* Step 1: Pick a Game */}
            {modalStep === 'pick-game' && (
              <div className="modal-step">
                <h3 className="modal-step-title">Step 1: Pick a Game</h3>
                {gamesLoading ? (
                  <p style={{ color: '#9ca3af' }}>Loading games...</p>
                ) : (
                  <div className="game-picker">
                    {games.map((game) => (
                      <div
                        key={game.id}
                        className={`game-card${selectedGame?.id === game.id ? ' game-card-selected' : ''}`}
                        onClick={() => handleSelectGame(game)}
                      >
                        <div style={{ fontWeight: 600 }}>{game.matchup}</div>
                        <div style={{ fontSize: '0.8rem', color: '#9ca3af' }}>
                          {game.status}
                          {game.score ? ` — ${game.score}` : ''}
                        </div>
                      </div>
                    ))}
                    <div
                      className={`game-card${isCustom ? ' game-card-selected' : ''}`}
                      onClick={handleSelectCustom}
                    >
                      <div style={{ fontWeight: 600 }}>Custom Prediction</div>
                      <div style={{ fontSize: '0.8rem', color: '#9ca3af' }}>
                        Type your own question
                      </div>
                    </div>
                  </div>
                )}
              </div>
            )}

            {/* Step 2: Configure Prediction */}
            {modalStep === 'configure' && (
              <div className="modal-step">
                <h3 className="modal-step-title">Step 2: Configure Prediction</h3>

                <div className="modal-row">
                  <label className="modal-label" htmlFor="pred-title">
                    Prediction Title
                  </label>
                  <input
                    id="pred-title"
                    type="text"
                    className="modal-control"
                    placeholder="E.g., Who wins: Duke vs UNC?"
                    value={draftTitle}
                    onChange={(e) => setDraftTitle(e.target.value)}
                  />
                </div>

                <div className="modal-grid">
                  <div className="modal-row">
                    <label className="modal-label" htmlFor="pred-opt-a">
                      Option A
                    </label>
                    <input
                      id="pred-opt-a"
                      type="text"
                      className="modal-control"
                      placeholder="E.g., Duke"
                      value={draftOptionA}
                      onChange={(e) => setDraftOptionA(e.target.value)}
                    />
                  </div>
                  <div className="modal-row">
                    <label className="modal-label" htmlFor="pred-opt-b">
                      Option B
                    </label>
                    <input
                      id="pred-opt-b"
                      type="text"
                      className="modal-control"
                      placeholder="E.g., UNC"
                      value={draftOptionB}
                      onChange={(e) => setDraftOptionB(e.target.value)}
                    />
                  </div>
                </div>

                <div className="modal-row">
                  <label className="modal-label" htmlFor="pred-lock">
                    Lock Time
                  </label>
                  <input
                    id="pred-lock"
                    type="datetime-local"
                    className="modal-control"
                    value={draftLockAt}
                    onChange={(e) => setDraftLockAt(e.target.value)}
                  />
                </div>

                <div className="modal-grid">
                  <div className="modal-row">
                    <label className="modal-label" htmlFor="pred-min">
                      Min Bet (pts)
                    </label>
                    <input
                      id="pred-min"
                      type="number"
                      className="modal-control"
                      min={1}
                      value={draftBetMin}
                      onChange={(e) => setDraftBetMin(Number(e.target.value))}
                    />
                  </div>
                  <div className="modal-row">
                    <label className="modal-label" htmlFor="pred-max">
                      Max Bet (pts)
                    </label>
                    <input
                      id="pred-max"
                      type="number"
                      className="modal-control"
                      min={1}
                      value={draftBetMax}
                      onChange={(e) => setDraftBetMax(Number(e.target.value))}
                    />
                  </div>
                </div>

                {modalError && (
                  <div className="modal-error" role="alert">
                    {modalError}
                  </div>
                )}

                <div className="modal-actions">
                  <button
                    type="button"
                    className="modal-secondary-button"
                    onClick={() => {
                      setModalError(null);
                      setModalStep('pick-game');
                    }}
                  >
                    Back
                  </button>
                  <button
                    type="button"
                    className="modal-primary-button"
                    onClick={handleConfigureNext}
                  >
                    Next
                  </button>
                </div>
              </div>
            )}

            {/* Step 3: Creator's Wager */}
            {modalStep === 'creator-wager' && (
              <div className="modal-step">
                <h3 className="modal-step-title">Step 3: Your Wager</h3>
                <p style={{ marginBottom: '0.75rem', color: '#d1d5db' }}>
                  As the creator, pick your side and place your wager.
                </p>

                <div style={{ display: 'flex', gap: '1rem', marginBottom: '1rem' }}>
                  <label style={{ display: 'flex', alignItems: 'center', gap: '0.3rem', cursor: 'pointer' }}>
                    <input
                      type="radio"
                      name="creator-pick"
                      checked={creatorOptionIndex === 0}
                      onChange={() => setCreatorOptionIndex(0)}
                    />
                    {draftOptionA}
                  </label>
                  <label style={{ display: 'flex', alignItems: 'center', gap: '0.3rem', cursor: 'pointer' }}>
                    <input
                      type="radio"
                      name="creator-pick"
                      checked={creatorOptionIndex === 1}
                      onChange={() => setCreatorOptionIndex(1)}
                    />
                    {draftOptionB}
                  </label>
                </div>

                <div className="modal-row">
                  <label className="modal-label" htmlFor="creator-wager-amt">
                    Wager Amount ({draftBetMin}–{draftBetMax} pts)
                  </label>
                  <input
                    id="creator-wager-amt"
                    type="number"
                    className="modal-control"
                    min={draftBetMin}
                    max={draftBetMax}
                    value={creatorWagerAmount}
                    onChange={(e) => setCreatorWagerAmount(Number(e.target.value))}
                  />
                </div>

                {modalError && (
                  <div className="modal-error" role="alert">
                    {modalError}
                  </div>
                )}

                <div className="modal-actions">
                  <button
                    type="button"
                    className="modal-secondary-button"
                    onClick={() => {
                      setModalError(null);
                      setModalStep('configure');
                    }}
                  >
                    Back
                  </button>
                  <button
                    type="button"
                    className="modal-primary-button"
                    onClick={handleSubmitPrediction}
                  >
                    Create Prediction
                  </button>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    );
  };

  /* ================================================================ */
  /*  Main render                                                      */
  /* ================================================================ */

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
              Squad up with real friends, place friendly point bets, and track who wears
              the crown.
            </p>
            <div className="chat-title-actions">
              <button
                type="button"
                className="make-prediction-button"
                onClick={openCreateModal}
              >
                Make a Prediction
              </button>
              <button
                type="button"
                className="make-prediction-button"
                onClick={() => {
                  setIsMembersOpen(true);
                  fetch(apiUrl(`/api/chats/${chatId}/pending-invites`))
                    .then((r) => r.ok ? r.json() : [])
                    .then(setPendingInvites)
                    .catch(() => {});
                }}
              >
                Members ({members.length})
              </button>
              <button
                type="button"
                className="make-prediction-button"
                style={{ color: '#f87171', borderColor: 'rgba(239,68,68,0.4)' }}
                onClick={() => setIsLeaveConfirmOpen(true)}
              >
                Leave Chat
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
          <span className="chat-pill points-pill">Points Only &middot; Zero Cash Risk</span>
        </div>
      </header>

      <main className="chat-layout">
        <section className="chat-panel">
          {renderPredictionBanner()}

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

        {renderSidebar()}
      </main>

      {renderModal()}
      {isMembersOpen && (
        <div
          className="modal-overlay"
          role="presentation"
          onMouseDown={(e) => {
            if (e.target === e.currentTarget) {
              setIsMembersOpen(false);
              setInviteQuery(''); setInviteResults([]);
            }
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
                onClick={() => {
                  setIsMembersOpen(false);
                  setInviteQuery(''); setInviteResults([]);
                }}
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

              {/* Pending Invites section */}
              {pendingInvites.length > 0 && (
                <div style={{ borderTop: '1px solid rgba(148,163,184,0.12)', paddingTop: '0.5rem' }}>
                  <div style={{ padding: '0.5rem 1rem 0.4rem', fontSize: '0.75rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                    Invited
                  </div>
                  {pendingInvites.map((u) => (
                    <div key={u.user_id} className="member-row">
                      <div className="member-row-left">
                        <div className="chat-message-avatar" style={{ width: 34, height: 34, fontSize: '0.8rem', opacity: 0.5 }}>
                          {u.username.charAt(0).toUpperCase()}
                        </div>
                        <span className="member-name" style={{ color: '#9ca3af' }}>{u.username}</span>
                      </div>
                      <span style={{ fontSize: '0.75rem', color: '#6b7280' }}>Pending</span>
                    </div>
                  ))}
                </div>
              )}

              {/* Invite Friends section */}
              <div style={{ borderTop: '1px solid rgba(148,163,184,0.12)', paddingTop: '0.5rem' }}>
                <div style={{ padding: '0.5rem 1rem 0.4rem', fontSize: '0.75rem', fontWeight: 600, color: '#6b7280', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                  Invite Friends
                </div>
                <div style={{ padding: '0 1rem 0.6rem' }}>
                  <input
                    type="text"
                    className="modal-control"
                    style={{ width: '100%', boxSizing: 'border-box' }}
                    placeholder="Search by username or add code…"
                    value={inviteQuery}
                    onChange={(e) => setInviteQuery(e.target.value)}
                  />
                </div>
                {inviteResults.map((u) => (
                  <div key={u.user_id} className="member-row">
                    <div className="member-row-left">
                      <div className="chat-message-avatar" style={{ width: 34, height: 34, fontSize: '0.8rem' }}>
                        {u.username.charAt(0).toUpperCase()}
                      </div>
                      <span className="member-name">{u.username}</span>
                    </div>
                    <button
                      type="button"
                      className="modal-primary-button"
                      style={{ padding: '0.25rem 0.9rem', fontSize: '0.78rem' }}
                      disabled={inviteStatus[u.user_id] === 'sending' || inviteStatus[u.user_id] === 'sent'}
                      onClick={() => handleInviteUser(u.user_id)}
                    >
                      {inviteStatus[u.user_id] === 'sent' ? 'Invited!' :
                       inviteStatus[u.user_id] === 'already' ? 'Already invited' :
                       inviteStatus[u.user_id] === 'sending' ? '…' : 'Invite'}
                    </button>
                  </div>
                ))}
              </div>

            </div>
          </div>
        </div>
      )}

      {/* Leave Chat confirm popup */}
      {isLeaveConfirmOpen && (
        <div className="modal-overlay" role="presentation" onMouseDown={(e) => { if (e.target === e.currentTarget) setIsLeaveConfirmOpen(false); }}>
          <div className="modal-shell" role="dialog" aria-modal="true" style={{ maxWidth: 360 }}>
            <div className="modal-header">
              <h2 className="modal-title">Leave Chat</h2>
              <button type="button" className="modal-close-button" aria-label="Close" onClick={() => setIsLeaveConfirmOpen(false)}>×</button>
            </div>
            <div style={{ padding: '1.25rem 1.1rem 0.5rem', color: '#d1d5db', fontSize: '0.92rem' }}>
              Are you sure you want to leave this chat? You'll need to be invited again to rejoin.
            </div>
            <div style={{ display: 'flex', gap: '0.6rem', padding: '1rem 1.1rem 1.25rem', justifyContent: 'flex-end' }}>
              <button type="button" className="nav-link-button" style={{ padding: '0.45rem 1rem' }} onClick={() => setIsLeaveConfirmOpen(false)}>
                Cancel
              </button>
              <button
                type="button"
                className="modal-primary-button"
                style={{ padding: '0.45rem 1.1rem', background: 'rgba(239,68,68,0.15)', borderColor: 'rgba(239,68,68,0.5)', color: '#f87171' }}
                onClick={() => { setIsLeaveConfirmOpen(false); handleLeaveChat(); }}
              >
                Leave
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Old prediction modal removed — replaced by the 3-step modal (renderModal) */}
    </div>
  );
}
