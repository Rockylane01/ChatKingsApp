import { Outlet } from "react-router-dom";
import Header from "./Header";
import BottomNav from "./BottomNav";

const AppShell = () => {
  return (
    <div className="mx-auto flex min-h-screen max-w-md flex-col bg-background">
      <Header />
      <main className="flex-1 overflow-y-auto pb-20">
        <Outlet />
      </main>
      <BottomNav />
    </div>
  );
};

export default AppShell;
