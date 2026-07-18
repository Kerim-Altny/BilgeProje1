export const usePermissionService = () => {
  const api = useApi();

  const getPermissions = async () => {
    return await api('/api/permissions', {
      method: 'GET'
    });
  };

  return { getPermissions };
};
