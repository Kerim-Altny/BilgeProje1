<template>
  <header class="mainnav">
    <div class="nav-left">
      <h1 class="page-title">{{ title }}</h1>
    </div>

    <div class="nav-right">
      <div class="user-chip">
        <span class="avatar">{{ initials }}</span>
        <span class="greeting">Hoş geldin, <strong>{{ authStore.currentUser?.username }}</strong></span>
      </div>
      <button class="logout-btn" @click="handleLogout">
        Çıkış Yap
        <i class="fa-solid fa-right-from-bracket"></i>
      </button>
    </div>
  </header>
</template>

<script setup>
import { computed } from 'vue';
import Swal from 'sweetalert2';

defineProps({
  title: {
    type: String,
    required: true
  }
});

const authStore = useAuthStore();

const initials = computed(() => {
  const name = authStore.currentUser?.username ?? "";
  return name.slice(0, 2).toUpperCase();
});

const handleLogout = async () => {
  const result = await Swal.fire({
    title: 'Çıkış yapmak istiyor musunuz?',
    icon: 'question',
    showCancelButton: true,
    confirmButtonColor: '#3085d6',
    cancelButtonColor: '#d33',
    confirmButtonText: 'Evet, çıkış yap',
    cancelButtonText: 'İptal',
    scrollbarPadding: false,
    heightAuto: false,
  });
  if (result.isConfirmed) {
    authStore.clearAuth();
    await navigateTo("/");
  }
};
</script>
