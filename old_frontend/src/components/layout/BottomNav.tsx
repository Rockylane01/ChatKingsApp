import { Home, MessageSquare, Users, Menu } from "lucide-react";
import { Link, useLocation } from "react-router-dom";
import { cn } from "@/lib/utils";

const navItems = [
  { icon: Home, label: "Home", path: "/" },
  { icon: MessageSquare, label: "Chats", path: "/chats" },
  { icon: Users, label: "Friends", path: "/friends" },
  { icon: Menu, label: "Menu", path: "/menu" },
];

const BottomNav = () => {
  const location = useLocation();

  return (
    <nav className="fixed bottom-0 left-1/2 z-50 w-full max-w-md -translate-x-1/2 border-t border-border bg-card/95 backdrop-blur-sm">
      <div className="flex items-center justify-around py-1.5">
        {navItems.map((item) => {
          const isActive =
            item.path === "/"
              ? location.pathname === "/"
              : location.pathname.startsWith(item.path);
          return (
            <Link
              key={item.path}
              to={item.path}
              className={cn(
                "flex flex-col items-center gap-0.5 px-4 py-1.5 text-[11px] font-medium transition-colors relative",
                isActive
                  ? "text-primary"
                  : "text-muted-foreground hover:text-foreground"
              )}
            >
              {isActive && (
                <span className="absolute -top-1.5 left-1/2 -translate-x-1/2 h-0.5 w-6 rounded-full bg-primary" />
              )}
              <item.icon className={cn("h-5 w-5", isActive && "drop-shadow-[0_0_6px_hsl(153,80%,48%)]")} />
              <span>{item.label}</span>
            </Link>
          );
        })}
      </div>
    </nav>
  );
};

export default BottomNav;
