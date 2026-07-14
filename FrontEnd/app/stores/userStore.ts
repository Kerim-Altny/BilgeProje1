import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useUserStore = defineStore('user', () => {
  const users = ref<any[]>([]);
  const loading = ref(false);

  const setUsers = (newUsers: any[]) => {
    users.value = newUsers;
  };

  const addUser = (user: any) => {
    users.value.push(user);
  };

  const updateUser = (id: string, updatedUser: any) => {
    const index = users.value.findIndex(u => u.id === id);
    if (index !== -1) {
      users.value[index] = { ...users.value[index], ...updatedUser };
    }
  };

  const removeUser = (id: string) => {
    users.value = users.value.filter(u => u.id !== id);
  };

  const clearUsers = () => {
    users.value = [];
  };

  return { users, loading, setUsers, addUser, updateUser, removeUser, clearUsers };
});
