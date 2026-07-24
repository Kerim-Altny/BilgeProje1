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
    const response = await api('/api/auth/login', {
      method: 'POST',
      body: { ...credentials, rememberMe }
    });

    if (import.meta.client) {
      localStorage.setItem('auth_active', '1');
      if (rememberMe) {
        localStorage.setItem('remember_me', '1');
        sessionStorage.removeItem('session_active');
      } else {
        localStorage.removeItem('remember_me');
        sessionStorage.setItem('session_active', '1');
      }
    }

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
