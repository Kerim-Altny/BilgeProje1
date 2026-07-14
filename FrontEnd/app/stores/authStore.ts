import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const currentUser = ref(null);
  const token = ref(localStorage.getItem('token') || null);

  const setToken = (newToken: string) => {
    token.value = newToken;
    localStorage.setItem('token', newToken);
  };

  const clearAuth = () => {
    token.value = null;
    currentUser.value = null;
    localStorage.removeItem('token');
  };

  const setUser = (user: any) => {
    currentUser.value = user;
  };

  return { currentUser, token, setToken, clearAuth, setUser };
});
