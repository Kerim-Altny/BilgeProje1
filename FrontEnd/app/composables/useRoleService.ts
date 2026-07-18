export const useRoleService = () => {
  const api = useApi();

  const getRoles = async () => {
    return await api('/api/roles', {
      method: 'GET'
    });
  };

  const getRoleById = async (id: string) => {
    return await api(`/api/roles/${id}`, {
      method: 'GET'
    });
  };

  const createRole = async (data: any) => {
    return await api('/api/roles', {
      method: 'POST',
      body: data
    });
  };

  const updateRole = async (id: string, data: any) => {
    return await api(`/api/roles/${id}`, {
      method: 'PUT',
      body: data
    });
  };

  const updateRolePermissions = async (id: string, permissions: string[]) => {
    return await api(`/api/roles/${id}/permissions`, {
      method: 'PUT',
      body: { Permissions: permissions }
    });
  };

  const deleteRole = async (id: string) => {
    return await api(`/api/roles/${id}`, {
      method: 'DELETE'
    });
  };

  const deleteRoles = async (ids: string[]) => {
    return await api('/api/roles', {
      method: 'DELETE',
      body: ids
    });
  };

  return { getRoles, getRoleById, createRole, updateRole, updateRolePermissions, deleteRole, deleteRoles };
};
