export default defineNuxtConfig({
  ssr: false,
  compatibilityDate: "2025-07-15",
  devtools: { enabled: true },
  css: ["~/assets/css/main.css", "@mdi/font/css/materialdesignicons.css"],
  modules: ["vuetify-nuxt-module"],
  vuetify: {
    vuetifyOptions: {
      theme: {
        defaultTheme: "light",
      },
    },
  },
  app: {
    head: {
      link: [
        {
          rel: "stylesheet",
          href: "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css",
          integrity:
            "sha512-SnH5WK+bZxgPHs44uWIX+LLJAJ9/2PkPKZ5QiAj6Ta86w+fsb2TkcmfRyVX3pBnMFcV7oQPJkl9QevSCWr3W6A==",
          crossorigin: "anonymous",
          referrerpolicy: "no-referrer",
        },
      ],
    },
  },
});
