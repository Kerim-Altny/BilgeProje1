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
          <h1 class="page-title">Kullanıcı Ekle</h1>
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
        <div v-if="loading" class="skeleton">Yükleniyor…</div>
        <div v-else class="content-inner">
          <div class="page-wrap">
            <div class="page-header">
              <div>
                <h1 class="page-title">Yeni Kullanıcı Ekle</h1>
                <p class="page-subtitle">Kullanıcı bilgilerini girip kaydet.</p>
              </div>

              <NuxtLink to="/dashboardUserList" class="btn-secondary">
                ← Listeye Dön
              </NuxtLink>
            </div>

            <div class="form-card">
              <form @submit.prevent="handleSubmit" class="form">
                <div class="field">
                  <label class="field-label">Kullanıcı Adı</label>
                  <input v-model="form.username" type="text" required placeholder="Örn. ahmetyilmaz"
                    class="field-input" />
                </div>

                <div class="field">
                  <label class="field-label">E-posta</label>
                  <input v-model="form.email" type="email" required placeholder="ornek@bilge.com" class="field-input" />
                </div>

                <div class="field">
                  <label class="field-label">Rol</label>
                  <select v-model="form.role" class="field-input">
                    <option value="Kullanıcı">Kullanıcı</option>
                    <option value="Admin">Admin</option>
                  </select>
                </div>

                <div class="field">
                  <label class="field-label">Şifre</label>
                  <input v-model="form.password" type="password" required placeholder="En az 6 karakter"
                    class="field-input" />
                </div>

                <p v-if="error" class="form-error">{{ error }}</p>

                <div class="form-actions">
                  <NuxtLink to="/dashboardUserList" class="btn-secondary">
                    Vazgeç
                  </NuxtLink>
                  <button type="submit" :disabled="saving" class="btn-primary">
                    {{ saving ? "Kaydediliyor…" : "Kaydet" }}
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

const loading = ref(true);
const user = ref(null);

const initials = computed(() => {
  const name = user.value?.username ?? "";
  return name.slice(0, 2).toUpperCase();
});

onMounted(async () => {
  const token = localStorage.getItem("token");

  try {

    const currentUser = await $fetch("http://localhost:5163/api/auth/me", {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (currentUser?.role?.toLowerCase() !== "admin") {
      alert("Bu işlemi yapmak için yetkiniz yok!");
      await navigateTo("/dashboardUserList");
      return;
    }

    user.value = currentUser;
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


const form = ref({
  username: "",
  email: "",
  role: "Kullanıcı",
  password: "",
});

const saving = ref(false);
const error = ref("");

const handleSubmit = async () => {
  saving.value = true;
  error.value = "";
  const token = localStorage.getItem("token");

  try {
    await $fetch("http://localhost:5163/api/users", {
      method: "POST",
      headers: { Authorization: `Bearer ${token}` },
      body: {
        Username: form.value.username,
        Email: form.value.email,
        Password: form.value.password,
        Role: form.value.role,
      },
    });
    await navigateTo("/dashboardUserList");
  } catch (e) {

    console.error("TAM HATA DETAYI:", e);
    console.error("BACKEND'DEN GELEN CEVAP:", e.response?._data);

    if (e.response?.status === 409) {
      error.value = e.response._data?.message || "Bu kullanıcı zaten mevcut.";
    } else if (e.response?.status === 400) {
      error.value =
        "Girdiğin bilgiler eksik veya hatalı (Şifre en az 6 karakter olmalı vs.).";
    } else if (e.response?.status === 401) {
      error.value = "Oturum süren dolmuş, lütfen tekrar giriş yap.";
    } else {
      error.value = "Kullanıcı oluşturulamadı. (Konsolu kontrol et)";
    }
  } finally {
    saving.value = false;
  }
};
</script>

<style scoped>
.form-card {
  max-width: 100%;
}

.form {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px 24px;
}

.form-actions,
.form-error {
  grid-column: 1 / -1;
}
</style>
