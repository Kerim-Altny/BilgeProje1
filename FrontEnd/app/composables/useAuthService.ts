export const useAuthService = () => {
  const api = useApi();
  const authStore = useAuthStore();

  const getMe = async () => {
    //  İŞLEV: Mevcut giriş yapmış kullanıcının profil bilgilerini getirir.
    //  Nereden Çekiliyor: Backend'deki GET /api/auth/me endpoint'inden.
    //  Nereye Yollanıyor: authStore -> setUser üzerinden 'currentUser' state'ine.
    const user = await api('/api/auth/me', {
      method: 'GET'
    });
    authStore.setUser(user);
    return user;
  };

  const login = async (credentials: any) => {
    //  İŞLEV: Kullanıcının email ve şifresiyle sisteme giriş yapmasını sağlar.
    //  Nereden Çekiliyor: Frontend'deki Login formundan (credentials).
    //  Nereye Yollanıyor: authStore -> setTokens üzerinden hem Token'ları saklar hem de yanıtı Component'e döner.
    const response: any = await api('/api/auth/login', {
      method: 'POST',
      body: credentials
    });
    
    authStore.setTokens(response.token, response.refreshToken);
    
    return response;
  };

  const register = async (userData: any) => {
    //  İŞLEV: Yeni bir kullanıcının sisteme kayıt olmasını sağlar.
    //  Nereden Çekiliyor: Frontend'deki Kayıt (Register) formundan.
    //  Nereye Yollanıyor: Backend'deki POST /api/auth/register endpoint'ine.
    return await api('/api/auth/register', {
      method: 'POST',
      body: userData
    });
  };

  return { getMe, login, register };
};
