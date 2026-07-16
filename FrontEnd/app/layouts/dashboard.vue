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

<script setup>
import { computed, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import Swal from 'sweetalert2';

const route = useRoute();
const title = computed(() => route.meta.title || 'Dashboard');

const authStore = useAuthStore();
const authService = useAuthService();
const loading = ref(true);

onMounted(async () => {
  try {
    const currentUser = await authService.getMe();
    if (!currentUser?.permissions?.includes("Dashboard.Access")) {
      await Swal.fire({
        scrollbarPadding: false, heightAuto: false,
        icon: 'error',
        title: 'Erişim Engellendi',
        text: 'Bu panele erişim yetkiniz yok!',
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
