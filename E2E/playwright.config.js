import { defineConfig } from '@playwright/test';
import { defineBddConfig } from 'playwright-bdd';

const testDir = defineBddConfig({
    features: 'features/**/*.feature',
    steps: 'features/steps/**/*.js',
});

export default defineConfig({
    testDir,
    use: {
        baseURL: 'http://localhost:5000',
    },
});