import { Link, useNavigate } from "@tanstack/react-router";

import { Button } from "@transportadora-moura/ui/components/button";

import { useAuth } from "@/modules/auth/hooks/auth-provider";

import { ModeToggle } from "./mode-toggle";

export default function Header() {
  const navigate = useNavigate();
  const { user, logout, loading } = useAuth();
  const links = [{ to: "/", label: "Coletas" }] as const;

  async function handleLogout() {
    await logout();
    await navigate({ to: "/login" });
  }

  return (
    <div>
      <div className="flex flex-row items-center justify-between px-2 py-1">
        <nav className="flex gap-4 text-lg">
          {links.map(({ to, label }) => {
            return (
              <Link key={to} to={to}>
                {label}
              </Link>
            );
          })}
        </nav>
        <div className="flex items-center gap-2">
          {!loading && user ? (
            <>
              <span className="text-sm text-muted-foreground">{user.name}</span>
              <Button size="sm" variant="outline" onClick={() => void handleLogout()}>
                Sair
              </Button>
            </>
          ) : null}
          <ModeToggle />
        </div>
      </div>
      <hr />
    </div>
  );
}
