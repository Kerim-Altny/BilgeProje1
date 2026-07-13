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
          <h1 class="page-title">Rol Düzenle</h1>
        </div>

        <div class="nav-right" v-if="!loadingUser">
          <div class="user-chip">
            <span class="avatar">{{ initials }}</span>
            <span class="greeting">Hoş geldin, <strong>{{ currentUser?.username }}</strong></span>
          </div>
          <button class="logout-btn" @click="handleLogout">
            Çıkış Yap
            <i class="fa-solid fa-right-from-bracket"></i>
          </button>
        </div>
      </header>

      <main class="content">
        <div v-if="loadingUser || loadingRole" class="skeleton">
          Yükleniyor…
        </div>

        <div v-else-if="!roleData" class="error-state">
          Rol bilgisi bulunamadı.
          <NuxtLink to="/dashboardRoleList" class="btn-secondary mt-2">
            Listeye Dön
          </NuxtLink>
        </div>

        <div v-else class="content-inner">
          <div class="page-wrap">
            <div class="page-header">
              <div>
                <h1 class="page-title">Rolü Düzenle</h1>
                <p class="page-subtitle">
                  Rol bilgilerini ve izinleri güncelleyebilirsin.
                </p>
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
                  
                  <div v-for="(perms, groupName) in groupedPermissions" :key="groupName" class="permission-group">
                    <h3 class="group-title">{{ groupName }}</h3>
                    <div class="permissions-grid">
                      <label v-for="p in perms" :key="p.name" class="toggle-switch">
                        <input type="checkbox" :value="p.name" v-model="form.permissions" />
                        <div class="toggle-slider"></div>
                        <span>{{ p.description }} <small class="text-gray-400">({{ p.name }})</small></span>
                      </label>
                    </div>
                  </div>

                  <div v-if="Object.keys(groupedPermissions).length === 0" class="text-sm text-gray-500">
                    Sistemde tanımlı yetki bulunamadı.
                  </div>
                </div>

                <p v-if="error" class="form-error">{{ error }}</p>

                <div class="form-actions">
                  <NuxtLink to="/dashboardRoleList" class="btn-secondary">
                    Vazgeç
                  </NuxtLink>
                  <button type="submit" :disabled="saving" class="btn-primary">
                    {{ saving ? "Güncelleniyor…" : "Güncelle" }}
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
import Swal from 'sweetalert2';

const route = useRoute();
const roleId = route.query.id;

const loadingUser = ref(true);
const loadingRole = ref(true);

const currentUser = ref(null);
const roleData = ref(null);

const initials = computed(() => {
  const name = currentUser.value?.username ?? "";
  return name.slice(0, 2).toUpperCase();
});

const form = ref({
  name: "",
  permissions: [],
});

const groupedPermissions = ref({});

const saving = ref(false);
const error = ref("");

onMounted(async () => {
  const token = localStorage.getItem("token");

  try {
    const u = await $fetch("http://localhost:5163/api/auth/me", {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!u?.permissions?.includes("Dashboard.Access") || !u?.permissions?.includes("Roles.Edit")) {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Yetkisiz İşlem', text: 'Bu işlemi yapmak için yetkiniz yok!' });
      await navigateTo("/dashboardRoleList");
      return;
    }
    currentUser.value = u;
  } catch (err) {
    localStorage.removeItem("token");
    await navigateTo("/");
    return;
  } finally {
    loadingUser.value = false;
  }

  if (roleId) {
    try {
      const data = await $fetch(`http://localhost:5163/api/roles/${roleId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      roleData.value = data;
      form.value.name = data.name;

      // Fetch role permissions
      const rolePerms = await $fetch(`http://localhost:5163/api/roles/${roleId}/permissions`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      form.value.permissions = rolePerms.map(p => p.name);

      // Fetch all permissions to display checkboxes
      const allPerms = await $fetch("http://localhost:5163/api/permissions", {
        headers: { Authorization: `Bearer ${token}` },
      });
      
      // Group permissions by group property
      const grouped = {};
      allPerms.forEach(p => {
          if(!grouped[p.group]) grouped[p.group] = [];
          grouped[p.group].push(p);
      });
      groupedPermissions.value = grouped;
    } catch (err) {
      console.error("Rol detayları çekilemedi:", err);
    } finally {
      loadingRole.value = false;
    }
  } else {
    loadingRole.value = false;
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
    cancelButtonText: 'İptal',
    scrollbarPadding: false,
    heightAuto: false,
  });
  if (result.isConfirmed) {
    localStorage.removeItem("token");
    await navigateTo("/");
  }
};

const handleSubmit = async () => {
  saving.value = true;
  error.value = "";
  const token = localStorage.getItem("token");

  try {
    await $fetch(`http://localhost:5163/api/roles/${roleId}`, {
      method: "PUT",
      headers: { Authorization: `Bearer ${token}` },
      body: {
        Name: form.value.name,
      },
    });

    await $fetch(`http://localhost:5163/api/roles/${roleId}/permissions`, {
      method: "PUT",
      headers: { Authorization: `Bearer ${token}` },
      body: {
        Permissions: form.value.permissions,
      },
    });
    saving.value = false;
    await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'success', title: 'Başarılı!', text: 'Rol başarıyla güncellendi.', timer: 1500, showConfirmButton: false });
    await navigateTo("/dashboardRoleList");
  } catch (e) {
    console.error("TAM HATA DETAYI:", e);

    if (e.response?.status === 409) {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Hata', text: e.response._data?.message || "Bu rol adı zaten mevcut." });
    } else if (e.response?.status === 400) {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Hatalı Giriş', text: 'Girdiğiniz bilgiler hatalı veya eksik.' });
    } else if (e.response?.status === 401) {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Oturum Süresi Doldu', text: 'Oturum süreniz dolmuş, lütfen tekrar giriş yapın.' });
    } else {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Oops...', text: 'Rol güncellenemedi. Bilgileri kontrol edip tekrar deneyin.' });
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
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.permission-group {
  margin-top: 16px;
  background: #f8fafc;
  padding: 16px;
  border-radius: 12px;
  border: 1px solid #e2e8f0;
}
.group-title {
  font-size: 15px;
  font-weight: 600;
  color: #0f172a;
  margin-bottom: 12px;
  padding-bottom: 8px;
  border-bottom: 1px solid #e2e8f0;
}
.permissions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 12px;
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

.form-actions {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 16px;
  grid-column: 1 / -1;
  margin-top: 24px;
  padding-top: 20px;
  border-top: 1px solid #e5e7eb;
  position: relative;
  z-index: 999;
}

.form-error {
  grid-column: 1 / -1;
  margin-top: 10px;
}

.btn-primary {
  min-width: 150px;
  display: inline-flex;
  justify-content: center;
  position: relative !important;
  z-index: 10000 !important;
  pointer-events: auto !important;
}

.btn-secondary {
  position: relative !important;
  z-index: 10000 !important;
  pointer-events: auto !important;
}

.error-state {
  text-align: center;
  margin-top: 40px;
  font-size: 1.1rem;
  color: #666;
}
.mt-2 {
  margin-top: 10px;
  display: inline-block;
}
</style>
