export const useAuthService = () => {
  const api = useApi();
  const authStore = useAuthStore();

  const getMe = async () => {
    const token = authStore.token;
    if (!token) throw new Error('No token');
    const user = await api('/api/auth/me', {
      method: 'GET',
      headers: { Authorization: `Bearer ${token}` }
    });
    authStore.setUser(user);
    return user;
  };

  const login = async (credentials: any) => {
    const response: any = await api('/api/auth/login', {
      method: 'POST',
      body: credentials
    });
    authStore.setToken(response.token);
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
