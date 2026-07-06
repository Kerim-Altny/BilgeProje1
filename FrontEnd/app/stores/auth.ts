import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null as string | null,
    isAuthenticated: false,
  }),
  actions: {
    async login(payload: any) {
      console.log("Login API'ye gönderilecek veri:", payload);
      // Şimdilik sadece state güncelliniyor. buraya API isteği gelcek
      this.user = payload.username;
      this.isAuthenticated = true;
    },
    async register(payload: any) {
      console.log("Register API'ye gönderilecek veri:", payload);
      // Backend'e kayıt isteği atıldıktan sonra otomatik giriş yapmış sayalır
      this.user = payload.username;
      this.isAuthenticated = true;
    },
    logout() {
      this.user = null;
      this.isAuthenticated = false;
    }
  }
})