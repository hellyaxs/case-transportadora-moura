import { createFileRoute } from "@tanstack/react-router";

import { ColetasPage } from "@/modules/coletas/pages/coletas-page";

export const Route = createFileRoute("/")({
  component: HomeComponent,
});

function HomeComponent() {
  return <ColetasPage />;
}
