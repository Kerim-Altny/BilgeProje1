export const useUserService = () => {
  const api = useApi();

  const getUsers = async () => {
    return await api('/api/users', {
      method: 'GET'
    });
  };

  const getUserById = async (id: string) => {
    return await api(`/api/users/${id}`, {
      method: 'GET'
    });
  };

  const createUser = async (data: any) => {
    return await api('/api/users', {
      method: 'POST',
      body: data
    });
  };

  const updateUser = async (id: string, data: any) => {
    return await api(`/api/users/${id}`, {
      method: 'PUT',
      body: data
    });
  };

  const deleteUser = async (id: string) => {
    return await api(`/api/users/${id}`, {
      method: 'DELETE'
    });
  };

  const deleteUsers = async (ids: string[]) => {
    return await api('/api/users', {
      method: 'DELETE',
      body: ids
    });
  };

  return { getUsers, getUserById, createUser, updateUser, deleteUser, deleteUsers };
};
