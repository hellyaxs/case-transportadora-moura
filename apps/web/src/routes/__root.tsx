import { HeadContent, Outlet, createRootRouteWithContext } from "@tanstack/react-router";
import { TanStackRouterDevtools } from "@tanstack/react-router-devtools";
import { Toaster } from "@transportadora-moura/ui/components/sonner";

import Header from "@/components/header";
import { AuthProvider, useAuth } from "@/modules/auth/hooks/auth-provider";
import { ThemeProvider } from "@/components/theme-provider";

import "../index.css";

export interface RouterAppContext {
  auth: ReturnType<typeof useAuth>;
}

export const Route = createRootRouteWithContext<RouterAppContext>()({
  component: RootComponent,
  head: () => ({
    meta: [
      {
        title: "transportadora-moura",
      },
      {
        name: "description",
        content: "transportadora-moura is a web application",
      },
    ],
    links: [
      {
        rel: "icon",
        href: "/favicon.ico",
      },
    ],
  }),
});

function RootComponent() {
  return (
    <>
      <HeadContent />
      <ThemeProvider
        attribute="class"
        defaultTheme="dark"
        disableTransitionOnChange
        storageKey="vite-ui-theme"
      >
        <AuthProvider>
          <div className="grid grid-rows-[auto_1fr] h-svh">
            <Header />
            <Outlet />
          </div>
          <Toaster richColors />
        </AuthProvider>
      </ThemeProvider>
      <TanStackRouterDevtools position="bottom-left" />
    </>
  );
}
