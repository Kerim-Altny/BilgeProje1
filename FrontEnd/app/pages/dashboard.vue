<template>
  <div class="adminpage">
    <aside class="leftmenu">
      <div class="brand">
        <span class="brand-mark">●</span>
        <span class="brand-name">Dashboard</span>
      </div>

      <nav class="nav">
        <span class="nav-label">Genel</span>
        <NuxtLink to="/dashboard" class="nav-item active">
          <span class="nav-icon">◆</span>
          <span>Anasayfa</span>
        </NuxtLink>
        <NuxtLink to="/dashboardUserList" class="nav-item">
          <span class="nav-icon">◆</span>
          <span>Kullanıcılar</span>
        </NuxtLink>
      </nav>
    </aside>

    <div class="mainpage">
      <header class="mainnav">
        <div class="nav-left">
          <h1 class="page-title">Anasayfa</h1>
        </div>

        <div class="nav-right" v-if="!loading">
          <div class="user-chip">
            <span class="avatar">{{ initials }}</span>
            <span class="greeting"
              >Hoş geldin, <strong>{{ user?.username }}</strong></span
            >
          </div>
          <button class="logout-btn" @click="handleLogout">
            Çıkış Yap
            <i class="fa-solid fa-right-from-bracket"></i>
          </button>
        </div>
      </header>

      <main class="content">
        <div v-if="loading" class="skeleton">Yükleniyor…</div>
        <div v-else class="content-inner">
          <p class="hint">Genel bakış içeriği buraya gelecek.</p>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";

const loading = ref(true);
const user = ref(null);

const initials = computed(() => {
  const name = user.value?.username ?? "";
  return name.slice(0, 2).toUpperCase();
});

onMounted(async () => {
  const token = localStorage.getItem("token");

  try {
    user.value = await $fetch("http://localhost:5163/api/auth/me", {
      headers: { Authorization: `Bearer ${token}` },
    });
  } catch (error) {
    localStorage.removeItem("token");
    await navigateTo("/");
  } finally {
    loading.value = false;
  }
});

const handleLogout = async () => {
  localStorage.removeItem("token");
  await navigateTo("/");
};
</script>
