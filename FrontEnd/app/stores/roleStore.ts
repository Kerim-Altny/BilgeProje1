import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useRoleStore = defineStore('role', () => {
  const roles = ref<any[]>([]);
  const loading = ref(false);

  const setRoles = (newRoles: any[]) => {
    //  İŞLEV: Toplu rol listesini store'a kaydeder (State'i günceller).
    //  Nereden Çekiliyor: useRoleService'in getRoles fonksiyonu başarılı çalıştığında.
    //  Nereye Yollanıyor: Store içindeki 'roles' dizisine.
    roles.value = newRoles;
  };

  const addRole = (role: any) => {
    //  İŞLEV: Yeni oluşturulan rolü mevcut state listesine ekler.
    //  Nereden Çekiliyor: useRoleService'in createRole fonksiyonundan dönen veri.
    //  Nereye Yollanıyor: Store içindeki 'roles' dizisinin sonuna eklenir.
    roles.value.push(role);
  };

  const updateRole = (id: string, updatedRole: any) => {
    //  İŞLEV: Mevcut bir rolü listede bularak günceller.
    //  Nereden Çekiliyor: useRoleService'in updateRole fonksiyonundan dönen veri.
    //  Nereye Yollanıyor: 'roles' dizisindeki ilgili id'ye sahip objenin üzerine yazılır.
    const index = roles.value.findIndex(r => r.id === id);
    if (index !== -1) {
      roles.value[index] = { ...roles.value[index], ...updatedRole };
    }
  };

  const removeRole = (id: string) => {
    //  İŞLEV: Silinen rolü state listesinden çıkartır.
    //  Nereden Çekiliyor: useRoleService'in deleteRole fonksiyonunda kullanılan silinen id.
    //  Nereye Yollanıyor: 'roles' dizisi filtrelenerek yeni liste atanır.
    roles.value = roles.value.filter(r => r.id !== id);
  };

  const clearRoles = () => {
    //  İŞLEV: Mevcut rolleri temizler (Örn: Logout olduğunda).
    roles.value = [];
  };

  return { roles, loading, setRoles, addRole, updateRole, removeRole, clearRoles };
});
