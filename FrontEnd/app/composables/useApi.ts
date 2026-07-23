export const useApi = () => {
  const { apiBase } = useRuntimeConfig().public;

  // Composable (senkron) seviyesinde çağırarak Nuxt context'ini kaybetmiyoruz
  const router = useRouter();
  const authStore = useAuthStore();

  return $fetch.create({
    baseURL: apiBase as string,
    credentials: 'include',
    // 401'de otomatik olarak orijinal isteği bir kez tekrar dener (ofetch'in kendi mekanizması).
    // Cookie tabanlı auth'ta yeni token da cookie üzerinden otomatik gittiği için manuel
    // header/token yönetimine gerek yok.
    retry: 1,
    retryStatusCodes: [401],
    async onResponseError({ request, response, options }: any) {
      if (response.status !== 401) return;

      const url = typeof request === 'string' ? request : request?.url ?? '';
      // login/register/refresh-token'ın kendi 401'i bir "token yenileme" denemesini tetiklemesin.
      if (url.includes('/api/auth/')) {
        options.retry = 0;
        return;
      }

      try {
        const rememberMe = import.meta.client && localStorage.getItem('remember_me') === '1';
        await $fetch(`${apiBase}/api/auth/refresh-token`, {
          method: 'POST',
          credentials: 'include',
          body: { rememberMe }
        });
        // Başarılı: cookie'ler backend tarafından yenilendi, ofetch orijinal isteği otomatik tekrar dener.
      } catch {
        // Refresh de başarısız oldu, oturum gerçekten bitmiş: tekrar denemeye gerek yok, çıkışa yönlendir.
        options.retry = 0;
        await authStore.clearAuth();
        if (import.meta.client) router.push('/');
      }
    }
  });
};
