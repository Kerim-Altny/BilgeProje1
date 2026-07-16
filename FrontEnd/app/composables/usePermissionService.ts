export const usePermissionService = () => {
  const api = useApi();

  const getPermissions = async () => {
    //  İŞLEV: Sistemdeki tüm yetkileri (permissions) listeler.
    //  Nereden Çekiliyor: Backend'deki GET /api/permissions endpoint'inden.
    //  Nereye Yollanıyor: Çağrıldığı Component'e dizi olarak döner.
    return await api('/api/permissions', {
      method: 'GET'
    });
  };

  return { getPermissions };
};
