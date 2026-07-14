export const useRoleService = () => {
  const api = useApi();
  const authStore = useAuthStore();

  const getHeaders = () => {
    return { Authorization: `Bearer ${authStore.token}` };
  };

  const getRoles = async () => {
    return await api('/api/roles', {
      method: 'GET',
      headers: getHeaders()
    });
  };

  const getRoleById = async (id: string) => {
    return await api(`/api/roles/${id}`, {
      method: 'GET',
      headers: getHeaders()
    });
  };

  const createRole = async (data: any) => {
    return await api('/api/roles', {
      method: 'POST',
      headers: getHeaders(),
      body: data
    });
  };

  const updateRole = async (id: string, data: any) => {
    return await api(`/api/roles/${id}`, {
      method: 'PUT',
      headers: getHeaders(),
      body: data
    });
  };

  const updateRolePermissions = async (id: string, permissions: string[]) => {
    return await api(`/api/roles/${id}/permissions`, {
      method: 'PUT',
      headers: getHeaders(),
      body: { Permissions: permissions }
    });
  };

  const deleteRole = async (id: string) => {
    return await api(`/api/roles/${id}`, {
      method: 'DELETE',
      headers: getHeaders()
    });
  };

  const deleteRoles = async (ids: string[]) => {
    return await api('/api/roles', {
      method: 'DELETE',
      headers: getHeaders(),
      body: ids
    });
  };

  return { getRoles, getRoleById, createRole, updateRole, updateRolePermissions, deleteRole, deleteRoles };
};
