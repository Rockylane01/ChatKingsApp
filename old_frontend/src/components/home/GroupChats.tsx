import { chats, currentUser, getUserById } from "@/data/mock";
import { useNavigate } from "react-router-dom";
import { Crown, ChevronRight, Zap } from "lucide-react";

const GroupChats = () => {
  const navigate = useNavigate();

  return (
    <section className="px-4 pt-5">
      <h2 className="mb-3 font-heading text-base font-bold uppercase tracking-wide text-foreground">
        My Groups
      </h2>
      <div className="flex flex-col gap-2">
        {chats.map((chat) => {
          const king = chat.members.find((m) => m.isKing);
          const kingUser = king ? getUserById(king.userId) : null;
          const myMembership = chat.members.find((m) => m.userId === currentUser.id);
          const totalPot = chat.activePrediction?.options.reduce(
            (sum, opt) => sum + opt.wagers.reduce((s, w) => s + w.amount, 0),
            0
          );

          return (
            <div
              key={chat.id}
              className="flex items-center gap-3 rounded-xl border border-border bg-card p-3 transition-colors hover:bg-secondary/50 active:bg-secondary"
              role="link"
              tabIndex={0}
              onClick={() => navigate(`/chat/${chat.id}`)}
              onKeyDown={(e) => {
                if (e.key === "Enter" || e.key === " ") {
                  e.preventDefault();
                  navigate(`/chat/${chat.id}`);
                }
              }}
            >
              {/* Avatar */}
              <div className="relative flex h-11 w-11 flex-shrink-0 items-center justify-center rounded-lg bg-secondary font-heading font-bold text-base text-foreground">
                {chat.name.charAt(0)}
                {chat.activePrediction && (
                  <span className="absolute -top-1 -right-1 flex h-3.5 w-3.5 items-center justify-center rounded-full bg-primary">
                    <Zap className="h-2 w-2 text-primary-foreground" />
                  </span>
                )}
              </div>

              {/* Info */}
              <div className="flex-1 min-w-0">
                <div className="flex items-center gap-2">
                  <span className="font-semibold text-sm truncate">{chat.name}</span>
                </div>
                <div className="flex items-center gap-1.5 text-[11px] text-muted-foreground mt-0.5">
                  {kingUser && (
                    <span className="flex items-center gap-0.5">
                      <Crown className="h-2.5 w-2.5 text-ck-gold" />
                      {kingUser.name}
                    </span>
                  )}
                  <span className="text-border">|</span>
                  <span>{chat.members.length} members</span>
                  {chat.activePrediction && totalPot && (
                    <>
                      <span className="text-border">|</span>
                      <span className="text-primary font-medium">{totalPot} pts in pot</span>
                    </>
                  )}
                </div>
              </div>

              {/* Points + Arrow */}
              <div className="flex flex-col items-end gap-1">
                <div className="flex items-center gap-1.5">
                  {myMembership && (
                    <div className="text-right">
                      <div className="text-sm font-bold text-foreground tabular-nums">
                        {myMembership.points.toLocaleString()}
                      </div>
                      <div className="text-[10px] text-muted-foreground">pts</div>
                    </div>
                  )}
                  <ChevronRight className="h-4 w-4 text-muted-foreground" />
                </div>
              </div>
            </div>
          );
        })}
      </div>
    </section>
  );
};

export default GroupChats;
