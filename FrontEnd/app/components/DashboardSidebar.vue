<template>
  <aside class="leftmenu">
    <div class="brand">
      <i class="brand-mark fa-solid fa-chart-pie"></i>
      <span class="brand-name">Dashboard</span>
    </div>

    <nav class="nav">
      <span class="nav-label">Genel</span>
      <NuxtLink to="/dashboard" class="nav-item" active-class="active">
        <i class="fa-solid fa-house nav-icon"></i>
        <span>Anasayfa</span>
      </NuxtLink>
      <NuxtLink v-if="canViewUsers" to="/dashboardUserList" class="nav-item" active-class="active" :class="{'active': $route.path.startsWith('/dashboardUser')}">
        <i class="fa-solid fa-users nav-icon"></i>
        <span>Kullanıcılar</span>
      </NuxtLink>
      <NuxtLink v-if="canViewRoles" to="/dashboardRoleList" class="nav-item" active-class="active" :class="{'active': $route.path.startsWith('/dashboardRole')}">
        <i class="fa-solid fa-shield-halved nav-icon"></i>
        <span>Roller</span>
      </NuxtLink>
    </nav>
  </aside>
</template>

<script setup>
import { computed } from 'vue';
import { useRoute } from 'vue-router';

const authStore = useAuthStore();
const route = useRoute();
const canViewUsers = computed(() => !!authStore.currentUser?.permissions?.includes("Users.View"));
const canViewRoles = computed(() => !!authStore.currentUser?.permissions?.includes("Roles.View"));
</script>
