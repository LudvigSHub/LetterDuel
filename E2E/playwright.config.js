import { defineConfig } from "@playwright/test";
import { defineBddConfig } from "playwright-bdd";

const testDir = defineBddConfig({
  features: "features/**/*.feature",
  steps: "features/steps/**/*.js",
  outputDir: ".features-gen",
});

export default defineConfig({
  testDir: ".features-gen",
  timeout: 60000,
  use: {
    baseURL: "http://localhost:5239",
    headless: false,
  },
});
