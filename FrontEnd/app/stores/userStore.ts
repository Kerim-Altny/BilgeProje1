import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useUserStore = defineStore('user', () => {
  const users = ref<any[]>([]);
  const loading = ref(false);

  const setUsers = (newUsers: any[]) => {
    //  İŞLEV: Toplu kullanıcı listesini store'a kaydeder (State'i günceller).
    //  Nereden Çekiliyor: useUserService'in getUsers fonksiyonu başarılı çalıştığında.
    //  Nereye Yollanıyor: Store içindeki 'users' dizisine.
    users.value = newUsers;
  };

  const addUser = (user: any) => {
    //  İŞLEV: Yeni oluşturulan kullanıcıyı mevcut state listesine ekler.
    //  Nereden Çekiliyor: useUserService'in createUser fonksiyonundan dönen veri.
    //  Nereye Yollanıyor: Store içindeki 'users' dizisinin sonuna eklenir.
    users.value.push(user);
  };

  const updateUser = (id: string, updatedUser: any) => {
    //  İŞLEV: Mevcut bir kullanıcıyı listede bularak günceller.
    //  Nereden Çekiliyor: useUserService'in updateUser fonksiyonundan dönen veri.
    //  Nereye Yollanıyor: 'users' dizisindeki ilgili id'ye sahip objenin üzerine yazılır.
    const index = users.value.findIndex(u => u.id === id);
    if (index !== -1) {
      users.value[index] = { ...users.value[index], ...updatedUser };
    }
  };

  const removeUser = (id: string) => {
    //  İŞLEV: Silinen kullanıcıyı state listesinden çıkartır.
    //  Nereden Çekiliyor: useUserService'in deleteUser fonksiyonunda kullanılan silinen id.
    //  Nereye Yollanıyor: 'users' dizisi filtrelenerek yeni liste atanır.
    users.value = users.value.filter(u => u.id !== id);
  };

  const clearUsers = () => {
    //  İŞLEV: Mevcut kullanıcıları temizler (Örn: Logout olduğunda).
    users.value = [];
  };

  return { users, loading, setUsers, addUser, updateUser, removeUser, clearUsers };
});
