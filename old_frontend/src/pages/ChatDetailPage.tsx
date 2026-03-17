import { useParams, Link } from "react-router-dom";
import { getChatById, getUserById, currentUser } from "@/data/mock";
import { ArrowLeft, Crown, Trophy, Send, Zap, Lock } from "lucide-react";
import { cn } from "@/lib/utils";
import { useState, useEffect, useRef } from "react";
import StrikeBadge from "@/components/StrikeBadge";
import { useToast } from "@/hooks/use-toast";

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------
interface ApiMessage {
  id: number;
  chat_id: number;
  user_id: number | null;
  user_name: string | null;
  type: "user" | "system" | "prediction";
  text: string;
  timestamp: string;
}

interface LiveBetState {
  potTotal: number;
  optionTotals: Record<string, number>; // optionId -> total points wagered
}

// ---------------------------------------------------------------------------
// Config
// ---------------------------------------------------------------------------
const API_BASE = import.meta.env.VITE_API_URL || "http://localhost:3001";

// Current user's numeric DB id (u1 in mock → user_id 1 in DB)
const CURRENT_USER_DB_ID = 1;

// ---------------------------------------------------------------------------
// Component
// ---------------------------------------------------------------------------
const ChatDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const { toast } = useToast();

  // Chat-level data still driven by mock
  const chat = getChatById(id || "");

  const [msgInput, setMsgInput]             = useState("");
  const [selectedOption, setSelectedOption] = useState<string | null>(null);
  const [wagerAmount, setWagerAmount]       = useState("");

  // ---- message state ----
  const [messages, setMessages]           = useState<ApiMessage[]>([]);
  const [loadingMsgs, setLoadingMsgs]     = useState(true);
  const [sending, setSending]             = useState(false);
  const [sendError, setSendError]         = useState<string | null>(null);
  const bottomRef = useRef<HTMLDivElement>(null);

  // ---- live bet state (wager totals from DB) ----
  const [liveBet, setLiveBet]             = useState<LiveBetState | null>(null);
  const [placingWager, setPlacingWager]   = useState(false);

  // Numeric chat id: "c1" → 1, "c2" → 2, etc.
  const numericChatId = id ? parseInt(id.replace("c", ""), 10) : NaN;

  // ---- fetch messages on mount ----
  useEffect(() => {
    if (isNaN(numericChatId)) return;
    setLoadingMsgs(true);
    fetch(`${API_BASE}/api/chats/${numericChatId}/messages`)
      .then((r) => r.json())
      .then((data: ApiMessage[]) => { setMessages(data); setLoadingMsgs(false); })
      .catch(() => setLoadingMsgs(false));
  }, [numericChatId]);

  // ---- fetch active bet totals on mount ----
  useEffect(() => {
    if (isNaN(numericChatId)) return;
    fetch(`${API_BASE}/api/chats/${numericChatId}/bets/active`)
      .then((r) => r.json())
      .then((data) => {
        if (data) setLiveBet({ potTotal: data.potTotal, optionTotals: data.optionTotals });
      })
      .catch(() => {}); // silently fall back to mock totals
  }, [numericChatId]);

  // ---- scroll to latest message ----
  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  // ---- send a chat message ----
  const handleSend = async () => {
    const text = msgInput.trim();
    if (!text || sending || isNaN(numericChatId)) return;
    setSending(true);
    setSendError(null);
    try {
      const res = await fetch(`${API_BASE}/api/chats/${numericChatId}/messages`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ userId: CURRENT_USER_DB_ID, messageText: text, messageType: "user" }),
      });
      if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        throw new Error(err.error || "Failed to send message");
      }
      const newMsg: ApiMessage = await res.json();
      setMessages((prev) => [...prev, newMsg]);
      setMsgInput("");
    } catch (e: unknown) {
      setSendError(e instanceof Error ? e.message : "Failed to send");
    } finally {
      setSending(false);
    }
  };

  const handleMsgKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") handleSend();
  };

  // ---- place a wager ----
  const handlePlaceWager = async () => {
    if (!selectedOption || !wagerAmount || placingWager || isNaN(numericChatId)) return;
    const amount = parseInt(wagerAmount, 10);
    if (isNaN(amount) || amount <= 0) {
      toast({ title: "Invalid amount", description: "Enter a positive number of points.", variant: "destructive" });
      return;
    }

    setPlacingWager(true);
    try {
      const res = await fetch(`${API_BASE}/api/chats/${numericChatId}/bets/active/wagers`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ userId: CURRENT_USER_DB_ID, optionId: selectedOption, amount }),
      });

      const data = await res.json();

      if (!res.ok) {
        toast({ title: "Wager failed", description: data.error || "Something went wrong.", variant: "destructive" });
        return;
      }

      // Update live totals from the server response
      setLiveBet({ potTotal: data.potTotal, optionTotals: data.optionTotals });
      setSelectedOption(null);
      setWagerAmount("");
      toast({ title: "Wager placed!", description: `${amount} pts on your pick. Good luck!` });
    } catch {
      toast({ title: "Network error", description: "Could not reach the server.", variant: "destructive" });
    } finally {
      setPlacingWager(false);
    }
  };

  // ---- early return if chat not found ----
  if (!chat) {
    return (
      <div className="flex flex-col items-center justify-center h-64 text-muted-foreground">
        <p>Chat not found</p>
        <Link to="/chats" className="text-primary text-sm mt-2">Back to chats</Link>
      </div>
    );
  }

  const king         = chat.members.find((m) => m.isKing);
  const kingUser     = king ? getUserById(king.userId) : null;
  const myMembership = chat.members.find((m) => m.userId === currentUser.id);
  const isKing       = myMembership?.isKing || false;
  const isLockedOut  = currentUser.strikes >= 3;
  const prediction   = chat.activePrediction;

  // Use live pot total from DB when available; fall back to mock sum
  const totalPot = liveBet?.potTotal
    ?? prediction?.options.reduce((sum, opt) => sum + opt.wagers.reduce((s, w) => s + w.amount, 0), 0)
    ?? 0;

  // ---- render a message from the API ----
  const renderMessage = (msg: ApiMessage) => {
    const isMe          = msg.user_id === CURRENT_USER_DB_ID;
    const isSystem      = msg.type === "system";
    const displayName   = msg.user_name || "Unknown";
    const avatarLetter  = displayName.charAt(0).toUpperCase();
    const formattedTime = new Date(msg.timestamp).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });

    if (isSystem) {
      return (
        <div key={msg.id} className="flex justify-center">
          <span className="rounded-full bg-secondary px-3 py-1 text-[10px] text-muted-foreground">
            {msg.text}
          </span>
        </div>
      );
    }

    return (
      <div key={msg.id} className={cn("flex gap-2", isMe && "flex-row-reverse")}>
        {!isMe && (
          <div className="flex h-7 w-7 flex-shrink-0 items-center justify-center rounded-full bg-secondary text-[10px] font-bold text-foreground">
            {avatarLetter}
          </div>
        )}
        <div className={cn("max-w-[75%]", isMe && "items-end")}>
          {!isMe && (
            <span className="text-[10px] text-muted-foreground mb-0.5 block">{displayName}</span>
          )}
          <div
            className={cn(
              "rounded-xl px-3 py-2 text-sm",
              isMe
                ? "bg-primary text-primary-foreground rounded-br-md"
                : "bg-secondary text-foreground rounded-bl-md"
            )}
          >
            {msg.text}
          </div>
          <span className="text-[9px] text-muted-foreground mt-0.5 block">{formattedTime}</span>
        </div>
      </div>
    );
  };

  return (
    <div className="flex flex-col h-full">
      {/* Chat Header */}
      <div className="flex items-center gap-3 border-b border-border bg-card px-3 py-2.5">
        <Link to="/chats" className="text-muted-foreground hover:text-foreground">
          <ArrowLeft className="h-5 w-5" />
        </Link>
        <div className="flex-1 min-w-0">
          <h2 className="font-heading font-bold text-sm truncate">{chat.name}</h2>
          <div className="flex items-center gap-1.5 text-[10px] text-muted-foreground">
            {kingUser && (
              <span className="flex items-center gap-0.5">
                <Crown className="h-2.5 w-2.5 text-ck-gold" />
                {kingUser.name}
              </span>
            )}
            <span className="text-border">|</span>
            <span>{chat.members.length} members</span>
          </div>
        </div>
        <div className="flex items-center gap-2">
          {myMembership && (
            <div className="text-right">
              <div className="text-xs font-bold tabular-nums">{myMembership.points.toLocaleString()}</div>
              <div className="text-[9px] text-muted-foreground">your pts</div>
            </div>
          )}
          <Link
            to={`/chat/${id}/leaderboard`}
            className="flex h-8 w-8 items-center justify-center rounded-lg bg-secondary hover:bg-sb-surface-hover transition-colors"
          >
            <Trophy className="h-4 w-4 text-ck-gold" />
          </Link>
        </div>
      </div>

      {/* Strike Warning */}
      {currentUser.strikes >= 2 && (
        <div className={cn(
          "flex items-center justify-center gap-2 px-4 py-1.5 text-xs font-medium",
          currentUser.strikes === 2 ? "bg-ck-orange/10 text-ck-orange" : "bg-ck-red/10 text-ck-red"
        )}>
          {currentUser.strikes === 3 ? (
            <><Lock className="h-3 w-3" /> Locked out — predictions disabled until midnight reset</>
          ) : (
            <><StrikeBadge strikes={currentUser.strikes} size="sm" showLabel={false} /> 2 strikes — one more and you're locked out!</>
          )}
        </div>
      )}

      {/* Active Prediction Card */}
      {prediction && (
        <div className="mx-3 mt-3 rounded-xl border border-primary/30 bg-card overflow-hidden">
          <div className="flex items-center justify-between bg-primary/10 px-3 py-2">
            <div className="flex items-center gap-1.5">
              <Zap className="h-3.5 w-3.5 text-primary" />
              <span className="text-xs font-bold text-primary uppercase tracking-wide">Active Prediction</span>
            </div>
            <span className="text-[10px] font-semibold text-muted-foreground">{totalPot} pts in pot</span>
          </div>

          <div className="p-3">
            <p className="font-semibold text-sm mb-3">{prediction.question}</p>

            {/* Option buttons — totals update live from DB after each wager */}
            <div className="flex gap-2 mb-3">
              {prediction.options.map((option) => {
                // Prefer DB-fetched total; fall back to mock sum
                const optionTotal = liveBet?.optionTotals[option.id]
                  ?? option.wagers.reduce((s, w) => s + w.amount, 0);
                const isSelected  = selectedOption === option.id;
                return (
                  <button
                    key={option.id}
                    onClick={() => !isLockedOut && setSelectedOption(isSelected ? null : option.id)}
                    disabled={isLockedOut || placingWager}
                    className={cn(
                      "flex-1 flex flex-col items-center gap-1 rounded-lg border py-2.5 px-2 transition-all",
                      isSelected
                        ? "border-primary bg-primary/15 text-primary"
                        : "border-border bg-secondary hover:bg-sb-surface-hover text-foreground",
                      (isLockedOut || placingWager) && "opacity-40 cursor-not-allowed"
                    )}
                  >
                    <span className="text-xs font-bold">{option.text}</span>
                    <span className="text-[10px] text-muted-foreground">{optionTotal} pts</span>
                  </button>
                );
              })}
            </div>

            {/* Wager input */}
            {selectedOption && !isLockedOut && (
              <div className="flex gap-2">
                <input
                  type="number"
                  placeholder={`Min ${prediction.minWager} pts`}
                  value={wagerAmount}
                  onChange={(e) => setWagerAmount(e.target.value)}
                  disabled={placingWager}
                  className="flex-1 rounded-lg border border-border bg-secondary px-3 py-2 text-sm placeholder:text-muted-foreground focus:outline-none focus:border-primary disabled:opacity-60"
                />
                <button
                  onClick={handlePlaceWager}
                  disabled={placingWager || !wagerAmount}
                  className="rounded-lg bg-primary px-4 py-2 text-sm font-bold text-primary-foreground hover:bg-primary/90 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {placingWager ? "Placing…" : "Place"}
                </button>
              </div>
            )}
          </div>
        </div>
      )}

      {/* Make Prediction Button (King only) */}
      {isKing && !prediction && (
        <div className="mx-3 mt-3">
          <Link
            to={`/chat/${id}/create-prediction`}
            className="flex items-center justify-center gap-2 rounded-xl border border-ck-gold/30 bg-ck-gold/10 py-3 text-sm font-bold text-ck-gold hover:bg-ck-gold/20 transition-colors"
          >
            <Crown className="h-4 w-4" />
            Create Prediction
          </Link>
        </div>
      )}

      {/* Messages */}
      <div className="flex-1 overflow-y-auto px-3 py-3 space-y-2">
        {loadingMsgs ? (
          <div className="flex justify-center pt-8">
            <span className="text-xs text-muted-foreground">Loading messages…</span>
          </div>
        ) : messages.length === 0 ? (
          <div className="flex justify-center pt-8">
            <span className="text-xs text-muted-foreground">No messages yet. Say something!</span>
          </div>
        ) : (
          messages.map(renderMessage)
        )}
        <div ref={bottomRef} />
      </div>

      {/* Message Input */}
      <div className="border-t border-border bg-card px-3 py-2.5">
        {sendError && <p className="text-[10px] text-ck-red mb-1">{sendError}</p>}
        <div className="flex gap-2">
          <input
            type="text"
            placeholder="Message..."
            value={msgInput}
            onChange={(e) => setMsgInput(e.target.value)}
            onKeyDown={handleMsgKeyDown}
            disabled={sending}
            className="flex-1 rounded-lg border border-border bg-secondary px-3 py-2 text-sm placeholder:text-muted-foreground focus:outline-none focus:border-primary disabled:opacity-60"
          />
          <button
            onClick={handleSend}
            disabled={sending || !msgInput.trim()}
            className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary text-primary-foreground hover:bg-primary/90 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <Send className="h-4 w-4" />
          </button>
        </div>
      </div>
    </div>
  );
};

export default ChatDetailPage;
