<template>
  <div class="page-wrap">
    <div class="page-header">
      <div>
        <h1 class="page-title">Kullanıcı Düzenle</h1>
        <p class="page-subtitle">Kullanıcı bilgilerini güncelle.</p>
      </div>

      <NuxtLink to="/dashboard/userlist" class="btn-secondary">
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
          <input
            v-model="form.email"
            type="email"
            required
            class="field-input"
          />
        </div>

        <div class="field">
          <label class="field-label">Rol</label>
          <select v-model="form.role" class="field-input">
            <option value="Kullanıcı">Kullanıcı</option>
            <option value="Admin">Admin</option>
          </select>
        </div>

        <div class="field">
          <label class="field-label"
            >Yeni Şifre (boş bırakılırsa değişmez)</label
          >
          <input v-model="form.password" type="password" class="field-input" />
        </div>

        <p v-if="error" class="form-error">{{ error }}</p>

        <div class="form-actions">
          <NuxtLink to="/dashboard/userlist" class="btn-secondary"
            >Vazgeç</NuxtLink
          >
          <button type="submit" :disabled="saving" class="btn-primary">
            {{ saving ? "Kaydediliyor…" : "Güncelle" }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";

definePageMeta({
  layout: "admin",
  title: "Kullanıcı Düzenle",
});

const route = useRoute();
const userId = route.query.id;

const form = ref({ name: "", email: "", role: "Kullanıcı", password: "" });
const loading = ref(true);
const saving = ref(false);
const error = ref("");
const user = ref(null);

onMounted(async () => {
  const token = localStorage.getItem("token");

  try {
    // API'den giriş yapan kullanıcının bilgilerini çekiyoruz
    const currentUser = await $fetch("http://localhost:5163/api/auth/me", {
      headers: { Authorization: `Bearer ${token}` },
    });

    // GÜVENLİK KONTROLÜ: Eğer rol Admin değilse anında geri postala!
    if (currentUser?.role?.toLowerCase() !== "admin") {
      alert("Bu işlemi yapmak için yetkiniz yok!");
      await navigateTo("/dashboardUserList");
      return;
    }

    user.value = currentUser;

    // Düzenlenecek kullanıcının bilgilerini çek
    if (userId) {
      const targetUser = await $fetch(`http://localhost:5163/api/users/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (targetUser) {
        form.value.name = targetUser.username || targetUser.name || "";
        form.value.email = targetUser.email || "";
        form.value.role = targetUser.role || "Kullanıcı";
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
    role: form.value.role,
  };
  if (form.value.password) payload.password = form.value.password;

  try {
    await $fetch(`http://localhost:5163/api/users/${userId}`, {
      method: "PUT",
      headers: { Authorization: `Bearer ${token}` },
      body: payload,
    });
    await navigateTo("/dashboardUserlist");
  } catch (e) {
    error.value = "Güncelleme başarısız oldu. Bilgileri kontrol et.";
  } finally {
    saving.value = false;
  }
};
</script>
