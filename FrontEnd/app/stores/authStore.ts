import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const currentUser = ref<any>(null);

  const setUser = (user: any) => {
    currentUser.value = user;
  };

  const clearAuth = async () => {
    const { apiBase } = useRuntimeConfig().public;
    try {
      await $fetch(`${apiBase}/api/auth/logout`, {
        method: 'POST',
        credentials: 'include'
      });
    } catch {
      // Sunucu isteği başarısız olsa da yerel oturumu temizlemeye devam ediyoruz.
    }

    currentUser.value = null;

    if (import.meta.client) {
      localStorage.removeItem('role');
      localStorage.removeItem('isAdmin');
      localStorage.removeItem('remember_me');
    }
  };

  return { currentUser, setUser, clearAuth };
});
