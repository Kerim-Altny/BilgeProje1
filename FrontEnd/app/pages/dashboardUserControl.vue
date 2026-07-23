<template>
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
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import Swal from 'sweetalert2';

definePageMeta({ layout: 'dashboard', title: 'Kullanıcı Düzenle' });

import { useRoute } from "vue-router";


const authStore = useAuthStore();
const roleService = useRoleService();
const userService = useUserService();

const route = useRoute();
const userId = route.query.id;

const form = ref({ name: "", email: "", roleId: null, password: "" });
const loading = ref(true);
const saving = ref(false);
const error = ref("");
const loadingUser = ref(true);
const roles = ref([]);
const showPass = ref(false);


onMounted(async () => {
  try {
    const currentUser = authStore.currentUser;
    if (!currentUser?.permissions?.includes("Users.Edit")) {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Yetkisiz İşlem', text: 'Bu işlemi yapmak için yetkiniz yok!' });
      await navigateTo("/dashboardUserList");
      return;
    }
    authStore.currentUser = currentUser;
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
    await authStore.clearAuth();
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

