import { createEnv } from "@t3-oss/env-core";
import { z } from "zod";

const runtimeEnv = (import.meta as ImportMeta & { env: Record<string, string | undefined> }).env;

export const env = createEnv({
  clientPrefix: "VITE_",
  client: {
    VITE_API_BASE_URL: z.string().url(),
  },
  runtimeEnv,
  emptyStringAsUndefined: true,
});
