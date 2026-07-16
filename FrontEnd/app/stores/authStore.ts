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
    //  İŞLEV: Kullanıcının giriş yapması veya token yenilemesi durumunda Token'ları saklar.
    //  Nereden Çekiliyor: useAuthService (login) veya useApi (refresh-token) yanıtlarından.
    //  Nereye Yollanıyor: Hem Store'daki 'token' ve 'refreshToken' state'lerine hem de LocalStorage'a.
    token.value = newToken;
    refreshToken.value = newRefreshToken;
    if (import.meta.client) {
      localStorage.setItem('token', newToken);
      localStorage.setItem('refreshToken', newRefreshToken);
    }
  };

  const clearAuth = () => {
    //  İŞLEV: Kullanıcının çıkış yapması durumunda tüm yetki verilerini siler.
    //  Nereden Çekiliyor: Çıkış (logout) işleminde veya token tamamen geçersiz olduğunda çağrılır.
    //  Nereye Yollanıyor: Store state'leri null yapılır ve LocalStorage'dan silinir.
    token.value = null;
    refreshToken.value = null;
    currentUser.value = null;
    if (import.meta.client) {
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
    }
  };

  const setUser = (user: any) => {
    //  İŞLEV: Giriş yapmış olan kullanıcının profil bilgilerini saklar.
    //  Nereden Çekiliyor: useAuthService içindeki getMe (GET /api/auth/me) fonksiyonundan.
    //  Nereye Yollanıyor: Store'daki 'currentUser' state'ine.
    currentUser.value = user;
  };

  return { currentUser, token, refreshToken, setTokens, clearAuth, setUser };
});
