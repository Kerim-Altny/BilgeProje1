export default defineNuxtConfig({
  ssr: false,
  compatibilityDate: "2025-07-15",
  devtools: { enabled: true },
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || "http://localhost:5163",
    },
  },

  css: ["~/assets/css/main.css", "@mdi/font/css/materialdesignicons.css"],
  modules: ["vuetify-nuxt-module", "@pinia/nuxt"],
  vuetify: {
    vuetifyOptions: {
      theme: {
        defaultTheme: "light",
      },
    },
  },
  app: {
    head: {
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' }
      ],
      link: [
        {
          rel: "stylesheet",
          href: "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css",
        },
      ],
    },
  },
});
