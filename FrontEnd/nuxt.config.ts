export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  css: [
    '~/assets/css/main.css',
    '@mdi/font/css/materialdesignicons.css',
  ],
  modules: [
    'vuetify-nuxt-module'
  ],
  vuetify: {
    vuetifyOptions: {
      theme: {
        defaultTheme: 'light'
      }
    }
  }
})