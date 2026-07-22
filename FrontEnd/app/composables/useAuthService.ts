export const useAuthService = () => {
  const api = useApi();
  const authStore = useAuthStore();

  const getMe = async (): Promise<any> => {
    const user = await api('/api/auth/me', {
      method: 'GET'
    });
    authStore.setUser(user);
    return user;
  };

  const login = async (credentials: any, rememberMe: boolean = false) => {
    const response: any = await api('/api/auth/login', {
      method: 'POST',
      body: credentials
    });
    
    authStore.setTokens(response.token, response.refreshToken, rememberMe);
    
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
