import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useRoleStore = defineStore('role', () => {
  const roles = ref<any[]>([]);
  const loading = ref(false);

  const setRoles = (newRoles: any[]) => {
    roles.value = newRoles;
  };

  const addRole = (role: any) => {
    roles.value.push(role);
  };

  const updateRole = (id: string, updatedRole: any) => {
    const index = roles.value.findIndex(r => r.id === id);
    if (index !== -1) {
      roles.value[index] = { ...roles.value[index], ...updatedRole };
    }
  };

  const removeRole = (id: string) => {
    roles.value = roles.value.filter(r => r.id !== id);
  };

  const clearRoles = () => {
    roles.value = [];
  };

  return { roles, loading, setRoles, addRole, updateRole, removeRole, clearRoles };
});
