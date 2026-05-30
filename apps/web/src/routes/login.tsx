import { createFileRoute, redirect } from "@tanstack/react-router";

import { LoginPage } from "@/modules/auth/pages/login-page";
import { authService } from "@/modules/auth/services/auth.service";

export const Route = createFileRoute("/login")({
  beforeLoad: async () => {
    try {
      await authService.me();
      throw redirect({ to: "/" });
    } catch (error) {
      if (error && typeof error === "object" && "to" in error) {
        throw error;
      }
    }
  },
  component: LoginPage,
});
