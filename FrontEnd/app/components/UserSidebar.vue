<template>
  <aside class="leftmenu">
    <div class="brand">
      <i class="brand-mark fa-solid fa-link"></i>
      <span class="brand-name">URL Kısaltıcı</span>
    </div>

    <nav class="nav">
      <span class="nav-label">Genel</span>
      <NuxtLink to="/user" class="nav-item" active-class="active" :class="{ active: $route.path === '/user' }">
        <i class="fa-solid fa-house nav-icon"></i>
        <span>Ana Sayfa</span>
      </NuxtLink>

      <span class="nav-label" style="margin-top: 16px;">Linkler</span>
      <NuxtLink to="/user/links" class="nav-item" active-class="active">
        <i class="fa-solid fa-list nav-icon"></i>
        <span>Linklerim</span>
      </NuxtLink>
      <NuxtLink to="/user/create" class="nav-item" active-class="active">
        <i class="fa-solid fa-plus nav-icon"></i>
        <span>Link Oluştur</span>
      </NuxtLink>
    </nav>

    <!-- Kullanıcı bilgisi alttan -->
    <div class="sidebar-user-info">
      <div class="sidebar-avatar">{{ initials }}</div>
      <div class="sidebar-user-text">
        <p class="sidebar-username">{{ authStore.currentUser?.username }}</p>
        <p class="sidebar-role">Kullanıcı</p>
      </div>
    </div>
  </aside>
</template>

<script setup>
import { computed } from 'vue';

const authStore = useAuthStore();

const initials = computed(() => {
  const name = authStore.currentUser?.username ?? '';
  return name.slice(0, 2).toUpperCase();
});
</script>
