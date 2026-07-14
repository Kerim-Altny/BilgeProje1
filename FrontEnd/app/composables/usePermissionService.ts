export const usePermissionService = () => {
  const api = useApi();
  const authStore = useAuthStore();

  const getHeaders = () => {
    return { Authorization: `Bearer ${authStore.token}` };
  };

  const getPermissions = async () => {
    return await api('/api/permissions', {
      method: 'GET',
      headers: getHeaders()
    });
  };

  return { getPermissions };
};
