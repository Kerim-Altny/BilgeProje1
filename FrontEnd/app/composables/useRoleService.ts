export const useRoleService = () => {
  const api = useApi();

  const getRoles = async () => {
    // Tüm rolleri getirir.
    // (Nereden çekiliyor: Backend /api/roles) (Nereye yollanıyor: Component veya store'a döner)
    return await api('/api/roles', {
      method: 'GET'
    });
  };

  const getRoleById = async (id: string) => {
    // ID'ye göre tekil rol bilgisi getirir.
    // (Nereden çekiliyor: Backend /api/roles/{id}) (Nereye yollanıyor: Component'e döner)
    return await api(`/api/roles/${id}`, {
      method: 'GET'
    });
  };

  const createRole = async (data: any) => {
    // Yeni rol oluşturur.
    // (Nereden çekiliyor: Kullanıcının girdiği form datası) (Nereye yollanıyor: Backend /api/roles)
    return await api('/api/roles', {
      method: 'POST',
      body: data
    });
  };

  const updateRole = async (id: string, data: any) => {
    // Mevcut rolün temel bilgilerini günceller.
    // (Nereden çekiliyor: Düzenleme formu datası) (Nereye yollanıyor: Backend /api/roles/{id})
    return await api(`/api/roles/${id}`, {
      method: 'PUT',
      body: data
    });
  };

  const updateRolePermissions = async (id: string, permissions: string[]) => {
    // Role ait yetkileri (permissions) günceller.
    // (Nereden çekiliyor: İzin seçme ekranındaki seçili liste) (Nereye yollanıyor: Backend /api/roles/{id}/permissions)
    return await api(`/api/roles/${id}/permissions`, {
      method: 'PUT',
      body: { Permissions: permissions }
    });
  };

  const deleteRole = async (id: string) => {
    // Tekil rolü siler.
    // (Nereden çekiliyor: Silme aksiyonu id'si) (Nereye yollanıyor: Backend /api/roles/{id})
    return await api(`/api/roles/${id}`, {
      method: 'DELETE'
    });
  };

  const deleteRoles = async (ids: string[]) => {
    // Birden fazla rolü siler.
    // (Nereden çekiliyor: Tablodaki seçili id'ler) (Nereye yollanıyor: Backend /api/roles)
    return await api('/api/roles', {
      method: 'DELETE',
      body: ids
    });
  };

  return { getRoles, getRoleById, createRole, updateRole, updateRolePermissions, deleteRole, deleteRoles };
};
