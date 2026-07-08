import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'

// Pinia'yı oluşturuyoruz
const pinia = createPinia()
const app = createApp(App)

// Uygulamaya entegre ediyoruz
app.use(pinia)
app.mount('#app')