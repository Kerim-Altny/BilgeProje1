export default defineNuxtConfig({
  // Nuxt'a kaynak kodlarının 'app' klasöründe olduğunu söylüyoruz
  srcDir: 'app/', 
  
  modules: [
    '@pinia/nuxt',
  ],
  css: [
    'vuetify/lib/styles/main.sass',
    '@mdi/font/css/materialdesignicons.min.css'
  ],
  build: {
    transpile: ['vuetify'],
  },
  vite: {
    define: {
      'process.env.DEBUG': false,
    },
  },
})