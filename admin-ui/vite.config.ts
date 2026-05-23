import { defineConfig } from 'vite';
import { getAngularAliases, getAngularPlugin } from '@angular/build';

export default defineConfig({
  plugins: [
    getAngularPlugin({
      jit: false
    })
  ],
  resolve: {
    alias: {
      ...getAngularAliases()
    }
  },
  optimizeDeps: {
    disabled: true
  }
});