export const useUserService = () => {
  const api = useApi();

  const getUsers = async () => {
    //  İŞLEV: Sistemdeki tüm kullanıcıları listeler.
    //  Nereden Çekiliyor: Backend'deki GET /api/users endpoint'inden.
    //  Nereye Yollanıyor: Çağrıldığı Component'e veya Store'a dizi olarak döner.
    return await api('/api/users', {
      method: 'GET'
    });
  };

  const getUserById = async (id: string) => {
    //  İŞLEV: ID'si verilen tekil kullanıcının detaylarını getirir.
    //  Nereden Çekiliyor: Backend'deki GET /api/users/{id} endpoint'inden.
    //  Nereye Yollanıyor: Çağrıldığı Component'e obje olarak döner.
    return await api(`/api/users/${id}`, {
      method: 'GET'
    });
  };

  const createUser = async (data: any) => {
    //  İŞLEV: Sisteme yeni bir kullanıcı ekler.
    //  Nereden Çekiliyor: Frontend'deki kullanıcı ekleme formundan (data parametresi).
    //  Nereye Yollanıyor: Backend'deki POST /api/users endpoint'ine yollanır.
    return await api('/api/users', {
      method: 'POST',
      body: data
    });
  };

  const updateUser = async (id: string, data: any) => {
    //  İŞLEV: Mevcut bir kullanıcının bilgilerini günceller.
    //  Nereden Çekiliyor: Frontend'deki kullanıcı düzenleme formundan.
    //  Nereye Yollanıyor: Backend'deki PUT /api/users/{id} endpoint'ine yollanır.
    return await api(`/api/users/${id}`, {
      method: 'PUT',
      body: data
    });
  };

  const deleteUser = async (id: string) => {
    //  İŞLEV: Tek bir kullanıcıyı sistemden kalıcı olarak siler.
    //  Nereden Çekiliyor: Silme butonuna basılan kullanıcının ID'si.
    //  Nereye Yollanıyor: Backend'deki DELETE /api/users/{id} endpoint'ine yollanır.
    return await api(`/api/users/${id}`, {
      method: 'DELETE'
    });
  };

  const deleteUsers = async (ids: string[]) => {
    //  İŞLEV: Birden fazla kullanıcıyı aynı anda siler (Toplu silme).
    //  Nereden Çekiliyor: Tablodaki seçili satırların ID listesi.
    //  Nereye Yollanıyor: Backend'deki DELETE /api/users endpoint'ine body içerisinde yollanır.
    return await api('/api/users', {
      method: 'DELETE',
      body: ids
    });
  };

  return { getUsers, getUserById, createUser, updateUser, deleteUser, deleteUsers };
};
