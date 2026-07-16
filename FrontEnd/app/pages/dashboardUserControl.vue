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
        <div v-if="loading" class="skeleton">Yükleniyor…</div>
        <div v-else class="content-inner">

          <div class="page-header">
            <div>
              <h1 class="page-title">Kullanıcı Düzenle</h1>
              <p class="page-subtitle">Kullanıcı bilgilerini güncelle.</p>
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
                    <input v-model="form.name" type="text" required placeholder="Kullanıcı adı" class="field-input padl" />
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
                  <label class="field-label">
                    Yeni Şifre
                    <span class="opt-badge">İsteğe Bağlı</span>
                  </label>
                  <div class="input-wrap">
                    <i class="fa-solid fa-lock input-icon"></i>
                    <input v-model="form.password" :type="showPass ? 'text' : 'password'" placeholder="Boş bırakılırsa değişmez" class="field-input padl padr" />
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
                {{ saving ? "Kaydediliyor…" : "Güncelle" }}
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
import { useRoute } from "vue-router";
import Swal from 'sweetalert2';

definePageMeta({ layout: "admin", title: "Kullanıcı Düzenle" });

const authStore = useAuthStore();
const authService = useAuthService();
const roleService = useRoleService();
const userService = useUserService();

const route = useRoute();
const userId = route.query.id;

const form = ref({ name: "", email: "", roleId: null, password: "" });
const loading = ref(true);
const saving = ref(false);
const error = ref("");
const user = ref(null);
const roles = ref([]);
const showPass = ref(false);

const initials = computed(() => {
  const name = user.value?.username ?? "";
  return name.slice(0, 2).toUpperCase();
});

const handleLogout = async () => {
  const r = await Swal.fire({ scrollbarPadding: false, heightAuto: false, title: 'Çıkış yapmak istiyor musunuz?', icon: 'question', showCancelButton: true, confirmButtonColor: '#3085d6', cancelButtonColor: '#d33', confirmButtonText: 'Evet, çıkış yap', cancelButtonText: 'İptal' });
  if (r.isConfirmed) { authStore.clearAuth(); await navigateTo("/"); }
};

onMounted(async () => {
  try {
    const currentUser = await authService.getMe();
    if (!currentUser?.permissions?.includes("Dashboard.Access") || !currentUser?.permissions?.includes("Users.Edit")) {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Yetkisiz İşlem', text: 'Bu işlemi yapmak için yetkiniz yok!' });
      await navigateTo("/dashboardUserList");
      return;
    }
    user.value = currentUser;
    roles.value = await roleService.getRoles();
    if (userId) {
      const t = await userService.getUserById(userId);
      if (t) {
        form.value.name = t.username || t.name || "";
        form.value.email = t.email || "";
        form.value.roleId = t.roleId ?? roles.value[0]?.id ?? null;
      }
    }
  } catch (err) {
    console.error("DashboardUserControl Hata:", err);
    authStore.clearAuth();
    await navigateTo("/");
  } finally {
    loading.value = false;
  }
});

const handleSubmit = async () => {
  saving.value = true; error.value = "";
  const payload = { username: form.value.name, email: form.value.email, roleId: form.value.roleId };
  if (form.value.password) payload.password = form.value.password;
  try {
    await userService.updateUser(userId, payload);
    await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'success', title: 'Başarılı!', text: 'Kullanıcı başarıyla güncellendi.', timer: 1500, showConfirmButton: false });
    await navigateTo("/dashboardUserList");
  } catch (e) {
    if (e.response?.status === 409) await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Hata', text: e.response._data?.message || "Bu kullanıcı adı veya e-posta zaten kullanılıyor." });
    else if (e.response?.status === 400) await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Hatalı Giriş', text: 'Girdiğin bilgiler eksik veya hatalı.' });
    else if (e.response?.status === 401) await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Oturum Süresi Doldu', text: 'Oturum süren dolmuş, lütfen tekrar giriş yap.' });
    else await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Oops...', text: 'Güncelleme başarısız oldu.' });
  } finally { saving.value = false; }
};
</script>

