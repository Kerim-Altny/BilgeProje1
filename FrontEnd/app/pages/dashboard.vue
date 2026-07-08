<template>
  <div class="dashboard-sayfasi">
    <p v-if="loading">Yükleniyor...</p>

    <div v-else-if="user">
      <h1>Dashboard</h1>
      <p>Hoş geldin, {{ user.username }}!</p>
      <button @click="handleLogout">Çıkış Yap</button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';

const loading = ref(true);
const user = ref(null);

onMounted(async () => {
  const token = localStorage.getItem('token');

  try {
    // Token'ın hâlâ geçerli olduğunu backend'e doğrulat.
    user.value = await $fetch('http://localhost:5163/api/auth/me', {
      headers: { Authorization: `Bearer ${token}` }
    });
  } catch (error) {
    // Token geçersiz/süresi dolmuş: temizle ve giriş ekranına dön.
    localStorage.removeItem('token');
    await navigateTo('/');
  } finally {
    loading.value = false;
  }
});

const handleLogout = () => {
  localStorage.removeItem('token');
  navigateTo('/');
};
</script>
