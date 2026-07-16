import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useUserStore = defineStore('user', () => {
  const users = ref<any[]>([]);
  const loading = ref(false);

  const setUsers = (newUsers: any[]) => {
    // Toplu kullanıcı listesini store'a kaydeder.
    // (Nereden çekiliyor: useUserService (Backend /api/users) yanıtından) 
    // (Nereye yollanıyor: userStore -> users dizisine)
    users.value = newUsers;
  };

  const addUser = (user: any) => {
    // Yeni oluşturulan kullanıcıyı mevcut listeye ekler.
    // (Nereden çekiliyor: useUserService createUser yanıtından) 
    // (Nereye yollanıyor: userStore -> users dizisine)
    users.value.push(user);
  };

  const updateUser = (id: string, updatedUser: any) => {
    // Mevcut bir kullanıcıyı listede günceller.
    // (Nereden çekiliyor: Düzenleme işlemi sonrası dönen/güncellenen veri) 
    // (Nereye yollanıyor: userStore -> ilgili id'ye sahip kullanıcıya)
    const index = users.value.findIndex(u => u.id === id);
    if (index !== -1) {
      users.value[index] = { ...users.value[index], ...updatedUser };
    }
  };

  const removeUser = (id: string) => {
    // Silinen kullanıcıyı listeden çıkartır.
    // (Nereden çekiliyor: Silme aksiyonu id'si) 
    // (Nereye yollanıyor: userStore -> users dizisinden silinir)
    users.value = users.value.filter(u => u.id !== id);
  };

  const clearUsers = () => {
    // Mevcut kullanıcıları temizler.
    users.value = [];
  };

  return { users, loading, setUsers, addUser, updateUser, removeUser, clearUsers };
});
