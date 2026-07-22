<template>
  <div class="adminpage">
    <DashboardSidebar />
    <div class="mainpage">
      <DashboardHeader :title="title" />
      <main class="content">
        <div v-if="loading" class="skeleton">Yükleniyor…</div>
        <slot v-else />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import Swal from 'sweetalert2';

const route = useRoute();
const title = computed(() => (route.meta.title as string) || 'Dashboard');

const authStore = useAuthStore();
const authService = useAuthService();
const loading = ref(true);

onMounted(async () => {
  try {
    const currentUser = await authService.getMe();
    if (!currentUser) {
      await Swal.fire({
        scrollbarPadding: false, heightAuto: false,
        icon: 'error',
        title: 'Erişim Engellendi',
        text: 'Oturumunuz geçersiz. Lütfen tekrar giriş yapın.',
        confirmButtonText: 'Tamam',
        confirmButtonColor: '#3085d6'
      });
      authStore.clearAuth();
      await navigateTo("/");
      return;
    }
    authStore.setUser(currentUser);
  } catch (error) {
    authStore.clearAuth();
    await navigateTo("/");
  } finally {
    loading.value = false;
  }
});
</script>
