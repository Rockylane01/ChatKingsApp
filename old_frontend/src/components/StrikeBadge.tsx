import { cn } from "@/lib/utils";
import { ShieldAlert } from "lucide-react";

interface StrikeBadgeProps {
  strikes: number;
  showLabel?: boolean;
  size?: "sm" | "md";
}

const StrikeBadge = ({ strikes, showLabel = true, size = "md" }: StrikeBadgeProps) => {
  const bgClass =
    strikes === 0 ? "bg-ck-green/15 text-ck-green border-ck-green/30" :
    strikes === 1 ? "bg-ck-yellow/15 text-ck-yellow border-ck-yellow/30" :
    strikes === 2 ? "bg-ck-orange/15 text-ck-orange border-ck-orange/30" :
    "bg-ck-red/15 text-ck-red border-ck-red/30";

  const label =
    strikes === 0 ? "Clear" :
    strikes === 1 ? "1 strike" :
    strikes === 2 ? "Careful!" :
    "Locked";

  return (
    <div
      className={cn(
        "flex items-center gap-1.5 rounded-full border px-2.5 py-1 font-semibold",
        bgClass,
        size === "sm" ? "text-[10px] px-2 py-0.5" : "text-xs"
      )}
    >
      <ShieldAlert className={size === "sm" ? "h-3 w-3" : "h-3.5 w-3.5"} />
      <span>{strikes}/3</span>
      {showLabel && <span className="hidden sm:inline">· {label}</span>}
    </div>
  );
};

export default StrikeBadge;
