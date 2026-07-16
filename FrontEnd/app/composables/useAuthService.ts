export const useAuthService = () => {
  const api = useApi();
  const authStore = useAuthStore();

  const getMe = async () => {
    // Interceptor zaten Authorization header'ını ekleyeceği için burada token'ı manuel eklememize gerek kalmadı,
    // Ancak geriye dönük uyumluluk veya API kullanımı gereği interceptor çalışana kadar burada da bırakılabiliriz.
    // Interceptor ayarlandığı için header kısmını temizliyoruz.
    const user = await api('/api/auth/me', {
      method: 'GET'
    });
    
    // Alınan kullanıcı verisi store'a kaydediliyor (Nereden çekiliyor: Backend /api/auth/me) (Nereye yollanıyor: authStore -> currentUser)
    authStore.setUser(user);
    return user;
  };

  const login = async (credentials: any) => {
    const response: any = await api('/api/auth/login', {
      method: 'POST',
      body: credentials
    });
    
    // Backend'den gelen token ve refreshToken store'a (ve localStorage'a) kaydediliyor.
    // (Nereden çekiliyor: Backend /api/auth/login yanıtı) (Nereye yollanıyor: authStore -> token, refreshToken)
    authStore.setTokens(response.token, response.refreshToken);
    
    return response;
  };

  const register = async (userData: any) => {
    return await api('/api/auth/register', {
      method: 'POST',
      body: userData
    });
  };

  return { getMe, login, register };
};
