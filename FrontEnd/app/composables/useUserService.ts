export const useUserService = () => {
  const api = useApi();

  const getUsers = async () => {
    // Tüm kullanıcıları getirir.
    // (Nereden çekiliyor: Backend /api/users) (Nereye yollanıyor: Component veya store'a döner)
    return await api('/api/users', {
      method: 'GET'
    });
  };

  const getUserById = async (id: string) => {
    // ID'ye göre tekil kullanıcı bilgisi getirir.
    // (Nereden çekiliyor: Backend /api/users/{id}) (Nereye yollanıyor: Component'e döner)
    return await api(`/api/users/${id}`, {
      method: 'GET'
    });
  };

  const createUser = async (data: any) => {
    // Yeni kullanıcı oluşturur.
    // (Nereden çekiliyor: Kullanıcı ekleme formu datası) (Nereye yollanıyor: Backend /api/users)
    return await api('/api/users', {
      method: 'POST',
      body: data
    });
  };

  const updateUser = async (id: string, data: any) => {
    // Mevcut kullanıcının bilgilerini günceller.
    // (Nereden çekiliyor: Düzenleme formu datası) (Nereye yollanıyor: Backend /api/users/{id})
    return await api(`/api/users/${id}`, {
      method: 'PUT',
      body: data
    });
  };

  const deleteUser = async (id: string) => {
    // Tekil kullanıcıyı siler.
    // (Nereden çekiliyor: Silme aksiyonu id'si) (Nereye yollanıyor: Backend /api/users/{id})
    return await api(`/api/users/${id}`, {
      method: 'DELETE'
    });
  };

  const deleteUsers = async (ids: string[]) => {
    // Birden fazla kullanıcıyı siler.
    // (Nereden çekiliyor: Tablodaki seçili id'ler) (Nereye yollanıyor: Backend /api/users)
    return await api('/api/users', {
      method: 'DELETE',
      body: ids
    });
  };

  return { getUsers, getUserById, createUser, updateUser, deleteUser, deleteUsers };
};
