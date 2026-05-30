import { useNavigate } from "@tanstack/react-router";
import type { FormEvent } from "react";
import { useState } from "react";

import { Button } from "@transportadora-moura/ui/components/button";
import { Card, CardContent, CardHeader, CardTitle } from "@transportadora-moura/ui/components/card";
import { Input } from "@transportadora-moura/ui/components/input";

import { useAuth } from "../hooks/auth-provider";

export function LoginPage() {
  const navigate = useNavigate();
  const { login, setError, error } = useAuth();
  const [email, setEmail] = useState("operador@moura.local");
  const [password, setPassword] = useState("Moura@2026");
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSubmitting(true);
    setError(null);

    try {
      await login(email, password);
      await navigate({ to: "/" });
    } catch (err) {
      setError(err instanceof Error ? err.message : "Não foi possível autenticar.");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <main className="container mx-auto flex max-w-md flex-col gap-4 px-4 py-12">
      <Card>
        <CardHeader>
          <CardTitle>Entrar</CardTitle>
        </CardHeader>
        <CardContent>
          <form className="grid gap-3" onSubmit={handleSubmit}>
            <Input
              type="email"
              placeholder="E-mail"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              required
            />
            <Input
              type="password"
              placeholder="Senha"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              required
            />
            {error ? <p className="text-sm text-destructive">{error}</p> : null}
            <Button type="submit" disabled={submitting}>
              {submitting ? "Entrando..." : "Acessar operação"}
            </Button>
          </form>
        </CardContent>
      </Card>
    </main>
  );
}
