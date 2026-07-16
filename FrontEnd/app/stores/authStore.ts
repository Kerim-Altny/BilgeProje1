import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const currentUser = ref(null);
  
  // Access Token
  const token = ref<string | null>(
    import.meta.client ? (localStorage.getItem('token') ?? null) : null
  );

  // Refresh Token
  const refreshToken = ref<string | null>(
    import.meta.client ? (localStorage.getItem('refreshToken') ?? null) : null
  );

  const setTokens = (newToken: string, newRefreshToken: string) => {
    // Token'ları store'a ve localStorage'a kaydeder.
    // (Nereden çekiliyor: useAuthService (login veya refresh yanıtı)) 
    // (Nereye yollanıyor: authStore -> token, refreshToken statelerine ve localStorage'a)
    token.value = newToken;
    refreshToken.value = newRefreshToken;
    if (import.meta.client) {
      localStorage.setItem('token', newToken);
      localStorage.setItem('refreshToken', newRefreshToken);
    }
  };

  const clearAuth = () => {
    // Tüm yetkilendirme bilgilerini temizler.
    // (Nereden çekiliyor: Çıkış (logout) aksiyonu veya token geçersiz/süresi dolmuş hatası) 
    // (Nereye yollanıyor: store sıfırlanır ve localStorage temizlenir)
    token.value = null;
    refreshToken.value = null;
    currentUser.value = null;
    if (import.meta.client) {
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
    }
  };

  const setUser = (user: any) => {
    // Giriş yapan kullanıcının profil bilgilerini store'a kaydeder.
    // (Nereden çekiliyor: useAuthService (getMe /api/auth/me yanıtı)) 
    // (Nereye yollanıyor: authStore -> currentUser state'ine)
    currentUser.value = user;
  };

  return { currentUser, token, refreshToken, setTokens, clearAuth, setUser };
});
