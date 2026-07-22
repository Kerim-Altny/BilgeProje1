<template>
  <aside class="leftmenu">
    <div class="brand">
      <i class="brand-mark fa-solid fa-chart-pie"></i>
      <span class="brand-name">Dashboard</span>
    </div>

    <nav class="nav">
      <span class="nav-label">Genel</span>

      <!-- Admin için Anasayfa → /dashboard -->
      <NuxtLink v-if="isAdmin" to="/dashboard" class="nav-item" active-class="active"
        :class="{ 'active': $route.path === '/dashboard' }">
        <i class="fa-solid fa-house nav-icon"></i>
        <span>Anasayfa</span>
      </NuxtLink>

      <!-- Normal kullanıcı için Anasayfa → /user (kendi link istatistikleri) -->
      <NuxtLink v-if="!isAdmin" to="/user" class="nav-item" active-class="active"
        :class="{ 'active': $route.path === '/user' }">
        <i class="fa-solid fa-house nav-icon"></i>
        <span>Anasayfa</span>
      </NuxtLink>

      <!-- Admin yönetim menüleri -->
      <NuxtLink v-if="canViewUsers" to="/dashboardUserList" class="nav-item" active-class="active"
        :class="{ 'active': $route.path.startsWith('/dashboardUser') }">
        <i class="fa-solid fa-users nav-icon"></i>
        <span>Kullanıcılar</span>
      </NuxtLink>
      <NuxtLink v-if="canViewRoles" to="/dashboardRoleList" class="nav-item" active-class="active"
        :class="{ 'active': $route.path.startsWith('/dashboardRole') }">
        <i class="fa-solid fa-shield-halved nav-icon"></i>
        <span>Roller</span>
      </NuxtLink>
      <!-- Admin: tüm kullanıcıların link istatistikleri -->
      <NuxtLink v-if="isAdmin" to="/dashboardLinks" class="nav-item" active-class="active"
        :class="{ 'active': $route.path === '/dashboardLinks' }">
        <i class="fa-solid fa-chart-line nav-icon"></i>
        <span>Link İstatistikleri</span>
      </NuxtLink>

      <!-- Admin: kendi link işlemleri (Link oluştur, düzenle, sil) -->
      <span v-if="isAdmin && canCreateLinks" class="nav-label" style="margin-top: 15px;">Link İşlemleri</span>
      <NuxtLink v-if="isAdmin && canCreateLinks" to="/user/create" class="nav-item" active-class="active"
        :class="{ 'active': $route.path === '/user/create' }">
        <i class="fa-solid fa-plus nav-icon"></i>
        <span>Link Oluştur</span>
      </NuxtLink>
      <NuxtLink v-if="isAdmin && canViewLinks" to="/user/links" class="nav-item" active-class="active"
        :class="{ 'active': $route.path === '/user/links' }">
        <i class="fa-solid fa-list nav-icon"></i>
        <span>Linklerim</span>
      </NuxtLink>

      <!-- Normal kullanıcı: kendi linkleri -->
      <span v-if="!isAdmin" class="nav-label" style="margin-top: 15px;">URL Kısaltıcı</span>
      <NuxtLink v-if="!isAdmin" to="/user/links" class="nav-item" active-class="active"
        :class="{ 'active': $route.path === '/user/links' }">
        <i class="fa-solid fa-list nav-icon"></i>
        <span>Linklerim</span>
      </NuxtLink>
      <NuxtLink v-if="!isAdmin" to="/user/create" class="nav-item" active-class="active"
        :class="{ 'active': $route.path === '/user/create' }">
        <i class="fa-solid fa-plus nav-icon"></i>
        <span>Link Oluştur</span>
      </NuxtLink>
    </nav>
  </aside>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRoute } from 'vue-router';

const authStore = useAuthStore();
const route = useRoute();

const canViewUsers  = computed(() => !!authStore.currentUser?.permissions?.includes("Users.View"));
const canViewRoles  = computed(() => !!authStore.currentUser?.permissions?.includes("Roles.View"));
const canViewLinks  = computed(() => !!authStore.currentUser?.permissions?.includes("Links.View"));
const canCreateLinks = computed(() => !!authStore.currentUser?.permissions?.includes("Links.Create"));

// Admin: Dashboard.Access, Users.View veya Roles.View yetkisi olan kişiler
const isAdmin = computed(() => {
  const perms: string[] = authStore.currentUser?.permissions ?? [];
  return perms.includes('Dashboard.Access') || perms.includes('Users.View') || perms.includes('Roles.View');
});
</script>
