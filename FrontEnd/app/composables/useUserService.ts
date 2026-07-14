export const useUserService = () => {
  const api = useApi();
  const authStore = useAuthStore();

  const getHeaders = () => {
    return { Authorization: `Bearer ${authStore.token}` };
  };

  const getUsers = async () => {
    return await api('/api/users', {
      method: 'GET',
      headers: getHeaders()
    });
  };

  const getUserById = async (id: string) => {
    return await api(`/api/users/${id}`, {
      method: 'GET',
      headers: getHeaders()
    });
  };

  const createUser = async (data: any) => {
    return await api('/api/users', {
      method: 'POST',
      headers: getHeaders(),
      body: data
    });
  };

  const updateUser = async (id: string, data: any) => {
    return await api(`/api/users/${id}`, {
      method: 'PUT',
      headers: getHeaders(),
      body: data
    });
  };

  const deleteUser = async (id: string) => {
    return await api(`/api/users/${id}`, {
      method: 'DELETE',
      headers: getHeaders()
    });
  };

  const deleteUsers = async (ids: string[]) => {
    return await api('/api/users', {
      method: 'DELETE',
      headers: getHeaders(),
      body: ids
    });
  };

  return { getUsers, getUserById, createUser, updateUser, deleteUser, deleteUsers };
};
