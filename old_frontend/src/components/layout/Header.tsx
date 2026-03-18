import { currentUser } from "@/data/mock";
import chatKingsLogo from "@/assets/chatkings-logo.png";
import StrikeBadge from "@/components/StrikeBadge";
import { Crown } from "lucide-react";

const Header = () => {
  return (
    <header className="sticky top-0 z-50 flex items-center justify-between sb-gradient-header border-b border-border px-4 py-3">
      <div className="flex items-center gap-2.5">
        <div className="relative">
          <img src={chatKingsLogo} alt="ChatKings" className="h-8 w-8 rounded-lg" />
          <Crown className="absolute -top-1 -right-1 h-3 w-3 text-ck-gold" />
        </div>
        <h1 className="font-heading text-lg font-bold text-foreground tracking-tight">
          Chat<span className="text-primary">Kings</span>
        </h1>
      </div>
      <StrikeBadge strikes={currentUser.strikes} />
    </header>
  );
};

export default Header;
