export const useRoleService = () => {
  const api = useApi();

  const getRoles = async () => {
    //  İŞLEV: Tüm rolleri listelemek için kullanılır.
    //  Nereden Çekiliyor: Backend'deki GET /api/roles endpoint'inden.
    //  Nereye Yollanıyor: Çağrıldığı Component'e veya Store'a dizi olarak döner.
    return await api('/api/roles', {
      method: 'GET'
    });
  };

  const getRoleById = async (id: string) => {
    //  İŞLEV: ID'si verilen tekil rolün detaylarını getirir.
    //  Nereden Çekiliyor: Backend'deki GET /api/roles/{id} endpoint'inden.
    //  Nereye Yollanıyor: Çağrıldığı Component'e obje olarak döner.
    return await api(`/api/roles/${id}`, {
      method: 'GET'
    });
  };

  const createRole = async (data: any) => {
    //  İŞLEV: Sisteme yeni bir rol ekler.
    //  Nereden Çekiliyor: Frontend'deki rol ekleme formundan (data parametresi).
    //  Nereye Yollanıyor: Backend'deki POST /api/roles endpoint'ine yollanır.
    return await api('/api/roles', {
      method: 'POST',
      body: data
    });
  };

  const updateRole = async (id: string, data: any) => {
    //  İŞLEV: Mevcut bir rolün bilgilerini günceller.
    //  Nereden Çekiliyor: Frontend'deki rol düzenleme formundan.
    //  Nereye Yollanıyor: Backend'deki PUT /api/roles/{id} endpoint'ine yollanır.
    return await api(`/api/roles/${id}`, {
      method: 'PUT',
      body: data
    });
  };

  const updateRolePermissions = async (id: string, permissions: string[]) => {
    //  İŞLEV: Belirli bir role ait yetkileri (permissions) günceller.
    //  Nereden Çekiliyor: Frontend yetki seçme ekranındaki checklist'ten (permissions dizisi).
    //  Nereye Yollanıyor: Backend'deki PUT /api/roles/{id}/permissions endpoint'ine yollanır.
    return await api(`/api/roles/${id}/permissions`, {
      method: 'PUT',
      body: { Permissions: permissions }
    });
  };

  const deleteRole = async (id: string) => {
    //  İŞLEV: Tek bir rolü sistemden kalıcı olarak siler.
    //  Nereden Çekiliyor: Silme butonuna basılan rolün ID'si.
    //  Nereye Yollanıyor: Backend'deki DELETE /api/roles/{id} endpoint'ine yollanır.
    return await api(`/api/roles/${id}`, {
      method: 'DELETE'
    });
  };

  const deleteRoles = async (ids: string[]) => {
    //  İŞLEV: Birden fazla rolü aynı anda siler (Toplu silme).
    //  Nereden Çekiliyor: Tablodaki seçili satırların ID listesi.
    //  Nereye Yollanıyor: Backend'deki DELETE /api/roles endpoint'ine body içerisinde yollanır.
    return await api('/api/roles', {
      method: 'DELETE',
      body: ids
    });
  };

  return { getRoles, getRoleById, createRole, updateRole, updateRolePermissions, deleteRole, deleteRoles };
};
