<template>
  <div class="adminpage">
    <aside class="leftmenu">
      <div class="brand">
        <i class="brand-mark fa-solid fa-chart-pie"></i>
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
        <NuxtLink to="/dashboardRoleList" class="nav-item">
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

          <div class="page-header">
            <div>
              <h1 class="page-title">Yeni Kullanıcı Ekle</h1>
              <p class="page-subtitle">Kullanıcı bilgilerini girip kaydet.</p>
            </div>
            <NuxtLink to="/dashboardUserList" class="btn-secondary">← Listeye Dön</NuxtLink>
          </div>

          <form @submit.prevent="handleSubmit">
            <!-- Tek kart, ikiye bölünmüş -->
            <div class="split-card">

              <!-- SOL: Bilgi alanları -->
              <div class="split-left">
                <div class="split-section-title">
                  <i class="fa-solid fa-user-pen"></i>
                  Kullanıcı Bilgileri
                </div>

                <div class="field">
                  <label class="field-label">Kullanıcı Adı</label>
                  <div class="input-wrap">
                    <i class="fa-solid fa-user input-icon"></i>
                    <input v-model="form.username" type="text" required placeholder="Örn. ahmetyilmaz" class="field-input padl" />
                  </div>
                </div>

                <div class="field">
                  <label class="field-label">E-posta</label>
                  <div class="input-wrap">
                    <i class="fa-solid fa-envelope input-icon"></i>
                    <input v-model="form.email" type="email" required placeholder="ornek@bilge.com" class="field-input padl" />
                  </div>
                </div>

                <div class="field">
                  <label class="field-label">Şifre</label>
                  <div class="input-wrap">
                    <i class="fa-solid fa-lock input-icon"></i>
                    <input v-model="form.password" :type="showPass ? 'text' : 'password'" required placeholder="En az 6 karakter" class="field-input padl padr" />
                    <button type="button" class="eye-btn" @click="showPass = !showPass" tabindex="-1">
                      <i :class="showPass ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye'"></i>
                    </button>
                  </div>
                </div>

                <p v-if="error" class="form-error" style="margin-top:8px">{{ error }}</p>
              </div>

              <!-- Dikey ayraç -->
              <div class="split-divider"></div>

              <!-- SAĞ: Rol seçimi -->
              <div class="split-right">
                <div class="split-section-title">
                  <i class="fa-solid fa-shield-halved"></i>
                  Rol Seçimi
                </div>
                <p class="split-section-sub">Kullanıcıya atanacak rolü seçin.</p>

                <div class="roles-list">
                  <div
                    v-for="role in roles"
                    :key="role.id"
                    class="role-row"
                    :class="{ selected: form.roleId === role.id }"
                    @click="form.roleId = role.id"
                  >
                    <div class="role-row-icon" :class="{ active: form.roleId === role.id }">
                      <i class="fa-solid fa-shield-halved"></i>
                    </div>
                    <span class="role-row-name">{{ role.name }}</span>
                    <div class="role-row-tick" :class="{ show: form.roleId === role.id }">
                      <i class="fa-solid fa-check"></i>
                    </div>
                  </div>
                  <p v-if="!roles.length" class="no-roles">Henüz rol tanımlanmamış.</p>
                </div>
              </div>

            </div>

            <!-- Butonlar -->
            <div class="form-actions" style="margin-top:20px">
              <NuxtLink to="/dashboardUserList" class="btn-secondary">Vazgeç</NuxtLink>
              <button type="submit" :disabled="saving" class="btn-primary">
                {{ saving ? "Kaydediliyor…" : "Kaydet" }}
              </button>
            </div>
          </form>

        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import Swal from 'sweetalert2';

const authStore = useAuthStore();
const authService = useAuthService();
const roleService = useRoleService();
const userService = useUserService();

const loading = ref(true);
const user = ref(null);
const roles = ref([]);
const showPass = ref(false);

const form = ref({ username: "", email: "", roleId: null, password: "" });
const saving = ref(false);
const error = ref("");

const initials = computed(() => {
  const name = user.value?.username ?? "";
  return name.slice(0, 2).toUpperCase();
});

onMounted(async () => {
  try {
    const currentUser = await authService.getMe();
    if (!currentUser?.permissions?.includes("Dashboard.Access") || !currentUser?.permissions?.includes("Users.Create")) {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Yetkisiz İşlem', text: 'Bu işlemi yapmak için yetkiniz yok!' });
      await navigateTo("/dashboardUserList");
      return;
    }
    user.value = currentUser;
    roles.value = await roleService.getRoles();
    const def = roles.value.find(r => r.name.toLowerCase() === "user") ?? roles.value[0];
    if (def) form.value.roleId = def.id;
  } catch {
    authStore.clearAuth();
    await navigateTo("/");
  } finally {
    loading.value = false;
  }
});

const handleLogout = async () => {
  const r = await Swal.fire({ scrollbarPadding: false, heightAuto: false, title: 'Çıkış yapmak istiyor musunuz?', icon: 'question', showCancelButton: true, confirmButtonColor: '#3085d6', cancelButtonColor: '#d33', confirmButtonText: 'Evet, çıkış yap', cancelButtonText: 'İptal' });
  if (r.isConfirmed) { authStore.clearAuth(); await navigateTo("/"); }
};

const handleSubmit = async () => {
  saving.value = true; error.value = "";
  try {
    await userService.createUser({ Username: form.value.username, Email: form.value.email, Password: form.value.password, RoleId: form.value.roleId });
    await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'success', title: 'Başarılı!', text: 'Kullanıcı başarıyla eklendi.', timer: 1500, showConfirmButton: false });
    await navigateTo("/dashboardUserList");
  } catch (e) {
    if (e.response?.status === 409) await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Hata', text: e.response._data?.message || "Bu kullanıcı zaten mevcut." });
    else if (e.response?.status === 400) await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Hatalı Giriş', text: 'Girdiğin bilgiler eksik veya hatalı (Şifre en az 6 karakter olmalı).' });
    else if (e.response?.status === 401) await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Oturum Süresi Doldu', text: 'Oturum süren dolmuş, lütfen tekrar giriş yap.' });
    else await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Oops...', text: 'Kullanıcı oluşturulamadı.' });
  } finally { saving.value = false; }
};
</script>

