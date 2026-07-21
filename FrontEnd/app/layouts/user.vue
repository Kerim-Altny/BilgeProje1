<template>
  <div class="adminpage">
    <UserSidebar />
    <div class="mainpage">
      <DashboardHeader :title="title" />
      <main class="content">
        <div v-if="loading" class="skeleton">Yükleniyor…</div>
        <slot v-else />
      </main>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();
const title = computed(() => route.meta.title || 'Panel');

const authStore = useAuthStore();
const authService = useAuthService();
const loading = ref(true);

onMounted(async () => {
  try {
    // Kullanıcı bilgilerini çek (zaten store'da varsa tekrar çekme)
    if (!authStore.currentUser) {
      const currentUser = await authService.getMe();
      authStore.setUser(currentUser);
    }
  } catch (error) {
    authStore.clearAuth();
    await navigateTo('/');
  } finally {
    loading.value = false;
  }
});
</script>
