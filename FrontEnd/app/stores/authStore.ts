import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const currentUser = ref(null);
  
  // Access Token
  const token = useCookie<string | null>('token');

  // Refresh Token
  const refreshToken = useCookie<string | null>('refreshToken');

  const setTokens = (newToken: string, newRefreshToken: string, rememberMe: boolean = false) => {
    
    // beni hatırla işaretliyse cookie 1 ay kalsın, yoksa tarayıcı kapanınca silinsin
    const maxAge = rememberMe ? 60 * 60 * 24 * 30 : undefined;
    
    // ssr patlamasın diye tokenları cookie'ye basıyoruz
    const cookieOptions = { maxAge, path: '/', sameSite: 'lax' as const };
    useCookie<string | null>('token', cookieOptions).value = newToken;
    useCookie<string | null>('refreshToken', cookieOptions).value = newRefreshToken;
    
    // store state'ini de güncelleyelim
    token.value = newToken;
    refreshToken.value = newRefreshToken;
  };

  const clearAuth = () => {
    // çıkış yapınca her şeyi temizle
    useCookie<string | null>('token', { path: '/' }).value = null;
    useCookie<string | null>('refreshToken', { path: '/' }).value = null;

    token.value = null;
    refreshToken.value = null;
    currentUser.value = null;
  };
  const setUser = (user: any) => {
    currentUser.value = user;
  };

  return { currentUser, token, refreshToken, setTokens, clearAuth, setUser };
});
