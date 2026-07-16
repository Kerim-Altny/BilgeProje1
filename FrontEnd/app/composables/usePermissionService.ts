export const usePermissionService = () => {
  const api = useApi();

  const getPermissions = async () => {
    // Tüm izinler (permissions) listesini getirir.
    // (Nereden çekiliyor: Backend /api/permissions) 
    // (Nereye yollanıyor: Componentlere veya doğrudan çağıran yere dizi olarak döner)
    return await api('/api/permissions', {
      method: 'GET'
    });
  };

  return { getPermissions };
};
