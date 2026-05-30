import { createFileRoute } from "@tanstack/react-router";

import { requireAuthenticatedUser } from "@/modules/auth/guards/require-authenticated-user";
import { CollectionsPage } from "@/modules/collections/pages/collections-page";

export const Route = createFileRoute("/")({
  beforeLoad: ({ location }) => requireAuthenticatedUser(location),
  component: CollectionsPage,
});
