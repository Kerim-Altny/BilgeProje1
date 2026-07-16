import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useRoleStore = defineStore('role', () => {
  const roles = ref<any[]>([]);
  const loading = ref(false);

  const setRoles = (newRoles: any[]) => {
    // Toplu rol listesini store'a kaydeder.
    // (Nereden çekiliyor: useRoleService (Backend /api/roles) yanıtından) 
    // (Nereye yollanıyor: roleStore -> roles dizisine)
    roles.value = newRoles;
  };

  const addRole = (role: any) => {
    // Yeni oluşturulan rolü mevcut listeye ekler.
    // (Nereden çekiliyor: useRoleService createRole yanıtından) 
    // (Nereye yollanıyor: roleStore -> roles dizisine)
    roles.value.push(role);
  };

  const updateRole = (id: string, updatedRole: any) => {
    // Mevcut bir rolü listede günceller.
    // (Nereden çekiliyor: Düzenleme işlemi sonrası dönen/güncellenen veri) 
    // (Nereye yollanıyor: roleStore -> ilgili id'ye sahip role)
    const index = roles.value.findIndex(r => r.id === id);
    if (index !== -1) {
      roles.value[index] = { ...roles.value[index], ...updatedRole };
    }
  };

  const removeRole = (id: string) => {
    // Silinen rolü listeden çıkartır.
    // (Nereden çekiliyor: Silme aksiyonu id'si) 
    // (Nereye yollanıyor: roleStore -> roles dizisinden silinir)
    roles.value = roles.value.filter(r => r.id !== id);
  };

  const clearRoles = () => {
    // Mevcut rolleri temizler.
    roles.value = [];
  };

  return { roles, loading, setRoles, addRole, updateRole, removeRole, clearRoles };
});
