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
        <NuxtLink to="/dashboardUserList" class="nav-item">
          <i class="fa-solid fa-users nav-icon"></i>
          <span>Kullanıcılar</span>
        </NuxtLink>
        <NuxtLink to="/dashboardRoleList" class="nav-item active">
          <i class="fa-solid fa-shield-halved nav-icon"></i>
          <span>Roller</span>
        </NuxtLink>
      </nav>
    </aside>

    <div class="mainpage">
      <header class="mainnav">
        <div class="nav-left">
          <h1 class="page-title">Rol Ekle</h1>
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
                <h1 class="page-title">Yeni Rol Ekle</h1>
                <p class="page-subtitle">Rol ismini ve izinleri belirleyip kaydet.</p>
              </div>

              <NuxtLink to="/dashboardRoleList" class="btn-secondary">
                ← Listeye Dön
              </NuxtLink>
            </div>

            <div class="form-card">
              <form @submit.prevent="handleSubmit" class="form">
                <div class="field">
                  <label class="field-label">Rol İsmi</label>
                  <input v-model="form.name" type="text" required placeholder="Örn. Editör"
                    class="field-input" />
                </div>

                <div class="field field-checkboxes">
                  <label class="field-label">İzinler</label>
                  
                  <label class="toggle-switch">
                    <input type="checkbox" v-model="form.canAdd" />
                    <div class="toggle-slider"></div>
                    <span><i class="fa-solid fa-plus" style="color: #10b981; margin-right: 6px;"></i> Ekleme İzni (Yeni veri ekleyebilir)</span>
                  </label>

                  <label class="toggle-switch">
                    <input type="checkbox" v-model="form.canEdit" />
                    <div class="toggle-slider"></div>
                    <span><i class="fa-solid fa-pen-to-square" style="color: #6366f1; margin-right: 6px;"></i> Düzenleme İzni (Verileri güncelleyebilir)</span>
                  </label>
                  
                  <label class="toggle-switch">
                    <input type="checkbox" v-model="form.canDelete" />
                    <div class="toggle-slider"></div>
                    <span><i class="fa-solid fa-trash-can" style="color: #ef4444; margin-right: 6px;"></i> Silme İzni (Verileri silebilir)</span>
                  </label>
                  
                  <label class="toggle-switch">
                    <input type="checkbox" v-model="form.canAccessDashboard" />
                    <div class="toggle-slider"></div>
                    <span><i class="fa-solid fa-chart-pie" style="color: #10b981; margin-right: 6px;"></i> Dashboard Giriş İzni (Panele erişebilir)</span>
                  </label>
                </div>

                <p v-if="error" class="form-error">{{ error }}</p>

                <div class="form-actions">
                  <NuxtLink to="/dashboardRoleList" class="btn-secondary">
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
import Swal from 'sweetalert2';

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

    if (!currentUser?.canAccessDashboard || !currentUser?.canAdd) {
      await Swal.fire({ icon: 'error', title: 'Yetkisiz İşlem', text: 'Bu işlemi yapmak için yetkiniz yok!' });
      await navigateTo("/dashboardRoleList");
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
  const result = await Swal.fire({
    title: 'Çıkış yapmak istiyor musunuz?',
    icon: 'question',
    showCancelButton: true,
    confirmButtonColor: '#3085d6',
    cancelButtonColor: '#d33',
    confirmButtonText: 'Evet, çıkış yap',
    cancelButtonText: 'İptal'
  });
  if (result.isConfirmed) {
    localStorage.removeItem("token");
    await navigateTo("/");
  }
};


const form = ref({
  name: "",
  canAdd: false,
  canEdit: false,
  canDelete: false,
  canAccessDashboard: false,
});

const saving = ref(false);
const error = ref("");

const handleSubmit = async () => {
  saving.value = true;
  error.value = "";
  const token = localStorage.getItem("token");

  try {
    await $fetch("http://localhost:5163/api/roles", {
      method: "POST",
      headers: { Authorization: `Bearer ${token}` },
      body: {
        Name: form.value.name,
        CanAdd: form.value.canAdd,
        CanEdit: form.value.canEdit,
        CanDelete: form.value.canDelete,
        CanAccessDashboard: form.value.canAccessDashboard,
      },
    });
    
    await Swal.fire({ icon: 'success', title: 'Başarılı!', text: 'Rol başarıyla eklendi.', timer: 1500, showConfirmButton: false });
    await navigateTo("/dashboardRoleList");
  } catch (e) {
    console.error("TAM HATA DETAYI:", e);

    if (e.response?.status === 409) {
      await Swal.fire({ icon: 'error', title: 'Hata', text: e.response._data?.message || "Bu rol zaten mevcut." });
    } else if (e.response?.status === 400) {
      await Swal.fire({ icon: 'error', title: 'Hatalı Giriş', text: 'Girdiğin bilgiler eksik veya hatalı.' });
    } else if (e.response?.status === 401) {
      await Swal.fire({ icon: 'error', title: 'Oturum Süresi Doldu', text: 'Oturum süren dolmuş, lütfen tekrar giriş yap.' });
    } else {
      await Swal.fire({ icon: 'error', title: 'Oops...', text: 'Rol oluşturulamadı. Bilgileri kontrol edip tekrar deneyin.' });
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
  grid-template-columns: 1fr;
  gap: 20px 24px;
}

.field-checkboxes {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.toggle-switch {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  font-size: 15px;
  color: #374151;
  font-weight: 500;
  user-select: none;
  padding: 12px 16px;
  border-radius: 12px;
  transition: all 0.2s ease;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
}

.toggle-switch:hover {
  background: #f1f5f9;
  border-color: #cbd5e1;
  transform: translateY(-1px);
}

.toggle-switch input {
  display: none;
}

.toggle-slider {
  position: relative;
  width: 44px;
  height: 24px;
  background-color: #cbd5e1;
  border-radius: 24px;
  transition: 0.3s ease;
  box-shadow: inset 0 2px 4px rgba(0,0,0,0.1);
  flex-shrink: 0;
}

.toggle-slider::before {
  content: "";
  position: absolute;
  height: 18px;
  width: 18px;
  left: 3px;
  bottom: 3px;
  background-color: white;
  border-radius: 50%;
  transition: 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 2px 4px rgba(0,0,0,0.2);
}

.toggle-switch input:checked + .toggle-slider {
  background-color: #10b981;
}

.toggle-switch input:checked + .toggle-slider::before {
  transform: translateX(20px);
}

.form-actions,
.form-error {
  grid-column: 1 / -1;
  margin-top: 10px;
}
</style>
