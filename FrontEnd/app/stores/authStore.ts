import { defineStore } from 'pinia';
import { ref } from 'vue';

// sessionStorage'da tutacağımız bayrağın anahtarı
const SESSION_FLAG = 'session_active';

export const useAuthStore = defineStore('auth', () => {
  const currentUser = ref(null);

  // SSR'da da okunabilsin diye cookie'den başlangıç değerini alıyoruz
  const token = ref<string | null>(useCookie<string | null>('token').value);
  const refreshToken = ref<string | null>(useCookie<string | null>('refreshToken').value);

  const setTokens = (newToken: string, newRefreshToken: string, rememberMe: boolean = false) => {
    token.value = newToken;
    refreshToken.value = newRefreshToken;

    if (import.meta.client) {
      if (rememberMe) {
        // Beni Hatırla seçili → 30 günlük kalıcı cookie
        document.cookie = `token=${newToken}; path=/; samesite=lax; max-age=${60 * 60 * 24 * 30}`;
        document.cookie = `refreshToken=${newRefreshToken}; path=/; samesite=lax; max-age=${60 * 60 * 24 * 30}`;
        localStorage.setItem('remember_me', '1');
        sessionStorage.removeItem(SESSION_FLAG);
      } else {
        // Beni Hatırla seçili DEĞİL → session cookie + sessionStorage bayrağı
        document.cookie = `token=${newToken}; path=/; samesite=lax`;
        document.cookie = `refreshToken=${newRefreshToken}; path=/; samesite=lax`;
        localStorage.removeItem('remember_me');
        sessionStorage.setItem(SESSION_FLAG, '1');
      }
    }
  };

  const clearAuth = () => {
    token.value = null;
    refreshToken.value = null;
    currentUser.value = null;

    if (import.meta.client) {
      document.cookie = `token=; path=/; max-age=0`;
      document.cookie = `refreshToken=; path=/; max-age=0`;
      sessionStorage.removeItem(SESSION_FLAG);
      localStorage.removeItem('remember_me');
    } else {
      useCookie('token', { path: '/' }).value = null;
      useCookie('refreshToken', { path: '/' }).value = null;
    }
  };

  const setUser = (user: any) => {
    currentUser.value = user;
  };

  return { currentUser, token, refreshToken, setTokens, clearAuth, setUser };
});
