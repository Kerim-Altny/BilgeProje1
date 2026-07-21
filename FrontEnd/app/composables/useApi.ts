export const useApi = () => {
  const { apiBase } = useRuntimeConfig().public;
  
  // Composable (senkron) seviyesinde çağırarak Nuxt context'ini kaybetmiyoruz
  const router = useRouter();
  const authStore = useAuthStore();
  
  return $fetch.create({ 
    baseURL: apiBase as string,
    async onRequest({ options }: any) {
      // ssr da patlamamak için token'ı cookie'den alıyoruz
      const token = useCookie('token').value || authStore.token;
      if (token) {
        // header tipleri karışmasın diye native Headers kullanıyoruz
        const headers = new Headers(options.headers);
        headers.set('Authorization', `Bearer ${token}`);
        options.headers = headers;
      }
    },
    async onResponseError(context: any) {
      const { request, response, options } = context;
      
      // 401 yedik, token patlamış, yenilemeyi deneyelim
      if (response.status === 401) {
        const isRemembered = !!useCookie('refreshToken').value;
        const token = useCookie('token').value || authStore.token;
        const refreshToken = useCookie('refreshToken').value || authStore.refreshToken;

        // refresh token da yoksa direkt login'e şutla
        if (!token || !refreshToken) {
          authStore.clearAuth();
          if (import.meta.client) router.push('/login');
          return;
        }

        try {
          const data: any = await $fetch(`${apiBase}/api/auth/refresh-token`, {
            method: 'POST',
            body: { token, refreshToken }
          });

          if (data && data.token && data.refreshToken) {
            // yeni tokenları cookie'ye yazıyoruz, eski remember durumunu koruyarak
            authStore.setTokens(data.token, data.refreshToken, isRemembered);

            const headers = new Headers(options.headers as HeadersInit);
            headers.set('Authorization', `Bearer ${data.token}`);
            options.headers = headers;
            
            // patlayan isteği yeni token ile bi daha gönder
            await $fetch(request as string, options as any);
          } else {
            throw new Error("Token alınamadı");
          }
        } catch (error: any) {
          // yenileme de patladıysa her şeyi sil login'e at
          authStore.clearAuth();
          if (import.meta.client) router.push('/login');
        }
      }
    }
  });
};

