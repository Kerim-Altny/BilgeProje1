<template>
  <div class="adminpage">
    <aside class="leftmenu">
      <div class="brand">
        <span class="brand-mark">●</span>
        <span class="brand-name">Dashboard</span>
      </div>

      <nav class="nav">
        <span class="nav-label">Genel</span>
        <NuxtLink to="/dashboard" class="nav-item">
          <i class="fa-solid fa-house nav-icon"></i>
          <span>Anasayfa</span>
        </NuxtLink>
        <NuxtLink to="/dashboardUserList" class="nav-item active">
          <i class="fa-solid fa-users nav-icon"></i>
          <span>Kullanıcılar</span>
        </NuxtLink>
        <NuxtLink to="/dashboardRoleList" class="nav-item ">
          <i class="fa-solid fa-shield-halved nav-icon"></i>
          <span>Roller</span>
        </NuxtLink>
      </nav>
    </aside>

    <div class="mainpage">
      <header class="mainnav">
        <div class="nav-left">
          <h1 class="page-title">Kullanıcı Düzenle</h1>
        </div>

        <div class="nav-right" v-if="!loading">
          <div class="user-chip">
            <span class="avatar">{{ initials }}</span>
            <span class="greeting">Hoş geldin, <strong>{{ user?.username }}</strong></span>
          </div>
          <button class="logout-btn" @click="handleLogout">
            Çıkış Yap
            <i class="fa-solid fa-right-from-bracket"></i>
          </button>
        </div>
      </header>

      <main class="content">
        <div class="content-inner">
          <div class="page-wrap">
            <div class="page-header">
              <div>
                <h1 class="page-title">Kullanıcı Düzenle</h1>
                <p class="page-subtitle">Kullanıcı bilgilerini güncelle.</p>
              </div>

              <NuxtLink to="/dashboardUserList" class="btn-secondary">
                ← Listeye Dön
              </NuxtLink>
            </div>

            <div class="form-card">
              <div v-if="loading" class="hint">Yükleniyor…</div>

              <form v-else @submit.prevent="handleSubmit" class="form">
                <div class="field">
                  <label class="field-label">Ad Soyad</label>
                  <input v-model="form.name" type="text" required class="field-input" />
                </div>

                <div class="field">
                  <label class="field-label">E-posta</label>
                  <input v-model="form.email" type="email" required class="field-input" />
                </div>

                <div class="field">
                  <label class="field-label">Rol</label>
                  <select v-model="form.roleId" class="field-input">
                    <option v-for="role in roles" :key="role.id" :value="role.id">
                      {{ role.name }}
                    </option>
                  </select>
                </div>

                <div class="field">
                  <label class="field-label">Yeni Şifre (boş bırakılırsa değişmez)</label>
                  <input v-model="form.password" type="password" class="field-input" />
                </div>

                <p v-if="error" class="form-error">{{ error }}</p>

                <div class="form-actions">
                  <NuxtLink to="/dashboardUserList" class="btn-secondary">Vazgeç</NuxtLink>
                  <button type="submit" :disabled="saving" class="btn-primary">
                    {{ saving ? "Kaydediliyor…" : "Güncelle" }}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useRoute } from "vue-router";

definePageMeta({
  layout: "admin",
  title: "Kullanıcı Düzenle",
});

const route = useRoute();
const userId = route.query.id;

const form = ref({ name: "", email: "", roleId: null, password: "" });
const loading = ref(true);
const saving = ref(false);
const error = ref("");
const user = ref(null);
const roles = ref([]);

const initials = computed(() => {
  const name = user.value?.username ?? "";
  return name.slice(0, 2).toUpperCase();
});

const handleLogout = async () => {
  localStorage.removeItem("token");
  await navigateTo("/");
};

onMounted(async () => {
  const token = localStorage.getItem("token");

  try {
    const currentUser = await $fetch("http://localhost:5163/api/auth/me", {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (!currentUser?.canAccessDashboard || !currentUser?.canEdit) {
      alert("Bu işlemi yapmak için yetkiniz yok!");
      await navigateTo("/dashboardUserList");
      return;
    }

    user.value = currentUser;

    roles.value = await $fetch("http://localhost:5163/api/roles", {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (userId) {
      const targetUser = await $fetch(`http://localhost:5163/api/users/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (targetUser) {
        form.value.name = targetUser.username || targetUser.name || "";
        form.value.email = targetUser.email || "";
        form.value.roleId = targetUser.roleId ?? roles.value[0]?.id ?? null;
      }
    }

  } catch (error) {
    console.error("DashboardUserControl Hata:", error);
    localStorage.removeItem("token");
    await navigateTo("/");
  } finally {
    loading.value = false;
  }
});

const handleSubmit = async () => {
  saving.value = true;
  error.value = "";
  const token = localStorage.getItem("token");

  const payload = {
    username: form.value.name,
    email: form.value.email,
    roleId: form.value.roleId,
  };
  if (form.value.password) payload.password = form.value.password;

  try {
    await $fetch(`http://localhost:5163/api/users/${userId}`, {
      method: "PUT",
      headers: { Authorization: `Bearer ${token}` },
      body: payload,
    });
    await navigateTo("/dashboardUserList");
  } catch (e) {
    error.value = "Güncelleme başarısız oldu. Bilgileri kontrol et.";
  } finally {
    saving.value = false;
  }
};
</script>

<style scoped>
.page-wrap {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
}

.page-header {
  width: 100%;
}

.form-card {
  max-width: 100% !important;
  width: 100%;
  margin: 24px auto 0 auto;
}

.form {
  max-width: 100%;
  margin: 0 auto;
  width: 100%;
}
</style>
