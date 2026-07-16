export const useApi = () => {
  const { apiBase } = useRuntimeConfig().public;
  
  // Composable (senkron) seviyesinde çağırarak Nuxt context'ini kaybetmiyoruz
  const router = useRouter();
  const authStore = useAuthStore();
  
  return $fetch.create({ 
    baseURL: apiBase as string,
    async onRequest({ options }) {
      if (import.meta.client) {
        const token = localStorage.getItem('token');
        if (token) {
          // ofetch tip uyumsuzluklarını önlemek için native Headers kullanımı
          const headers = new Headers(options.headers);
          headers.set('Authorization', `Bearer ${token}`);
          options.headers = headers;
        }
      }
    },
    async onResponseError(context) {
      const { request, response, options } = context;
      
      if (response.status === 401 && import.meta.client) {
        const token = localStorage.getItem('token');
        const refreshToken = localStorage.getItem('refreshToken');

        if (!token || !refreshToken) {
          authStore.clearAuth();
          router.push('/login');
          return;
        }

        try {
          const data: any = await $fetch(`${apiBase}/api/auth/refresh-token`, {
            method: 'POST',
            body: { token, refreshToken }
          });

          if (data && data.token && data.refreshToken) {
            authStore.setTokens(data.token, data.refreshToken);

            const headers = new Headers(options.headers as HeadersInit);
            headers.set('Authorization', `Bearer ${data.token}`);
            options.headers = headers;
            
            await $fetch(request as string, options as any);
          } else {
            throw new Error("Token alınamadı");
          }
        } catch (error) {
          authStore.clearAuth();
          router.push('/login');
        }
      }
    }
  });
};

