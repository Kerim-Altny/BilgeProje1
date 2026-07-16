import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const currentUser = ref(null);
  const token = ref<string | null>(
    import.meta.client ? (localStorage.getItem('token') ?? null) : null
  );

  const setToken = (newToken: string) => {
    token.value = newToken;
    if (import.meta.client) localStorage.setItem('token', newToken);
  };

  const clearAuth = () => {
    token.value = null;
    currentUser.value = null;
    if (import.meta.client) localStorage.removeItem('token');
  };

  const setUser = (user: any) => {
    currentUser.value = user;
  };

  return { currentUser, token, setToken, clearAuth, setUser };
});
