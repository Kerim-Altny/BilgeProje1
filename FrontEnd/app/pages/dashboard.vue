<template>
  <div class="adminpage">
    <!-- SOL MENÜ -->
    <aside class="leftmenu">
      <div class="brand">
        <span class="brand-mark">●</span>
        <span class="brand-name">Dashboard</span>
      </div>

      <nav class="nav">
        <span class="nav-label">Genel</span>
        <a
          v-for="item in navItems"
          :key="item.key"
          href="#"
          class="nav-item"
          :class="{ active: activeItem === item.key }"
          @click.prevent="activeItem = item.key"
        >
          <span class="nav-icon">{{ item.icon }}</span>
          <span>{{ item.label }}</span>
        </a>
      </nav>
    </aside>

    <!-- SAĞ TARAF -->
    <div class="mainpage">
      <!-- ÜST NAVBAR -->
      <header class="mainnav">
        <div class="nav-left">
          <h1 class="page-title">{{ activeLabel }}</h1>
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

      <!-- İÇERİK -->
      <main class="content">
        <div v-if="loading" class="skeleton">Yükleniyor…</div>
        <div v-else class="content-inner">
          <slot>
            <div class="empty-state">
              <p>Bu sayfa için henüz içerik eklenmedi.</p>
            </div>
          </slot>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";

const loading = ref(true);
const user = ref(null);
const activeItem = ref("dashboard");

const navItems = [
  { key: "dashboard", label: "Anasayfa", icon: "◆" },
  { key: "users", label: "Kullanıcılar", icon: "◆" },
];

const activeLabel = computed(
  () => navItems.find((i) => i.key === activeItem.value)?.label ?? "",
);

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
