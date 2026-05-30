import { createFileRoute, redirect } from "@tanstack/react-router";
import { z } from "zod";

import { LoginPage } from "@/modules/auth/pages/login-page";
import { authService } from "@/modules/auth/services/auth.service";
import { getSafeReturnPath } from "@/modules/auth/utils/safe-return-path";

const loginSearchSchema = z.object({
  redirect: z.string().optional(),
});

export const Route = createFileRoute("/login")({
  validateSearch: (search) => loginSearchSchema.parse(search),
  beforeLoad: async ({ search }) => {
    try {
      await authService.me();
      const destination = getSafeReturnPath(search.redirect) ?? "/";
      throw redirect({ to: destination });
    } catch (error) {
      if (error && typeof error === "object" && "to" in error) {
        throw error;
      }
    }
  },
  component: LoginPage,
});
