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

                <div class="field">
                  <label class="field-label">İzinler</label>

                  <div v-if="Object.keys(groupedPermissions).length === 0" class="text-sm text-gray-500">
                    Sistemde tanımlı yetki bulunamadı.
                  </div>

                  <div class="perm-cards-grid">
                    <div v-for="(perms, groupName) in groupedPermissions" :key="groupName" class="perm-card">
                      <div class="perm-card-header" @click="toggleGroup(groupName)">
                        <div class="perm-card-icon">
                          <i :class="groupIcon(groupName)"></i>
                        </div>
                        <span class="perm-card-title">{{ groupLabel(groupName) }}</span>
                        <i class="fa-solid perm-card-chevron" :class="openGroups[groupName] ? 'fa-chevron-up' : 'fa-chevron-down'"></i>
                      </div>

                      <div class="perm-card-body" v-show="openGroups[groupName]">
                        <label v-for="p in perms" :key="p.name" class="perm-toggle-row">
                          <div class="perm-toggle-info">
                            <i :class="permIcon(p.name)" class="perm-toggle-icon"></i>
                            <span>{{ p.description }}</span>
                          </div>
                          <div class="toggle-wrap">
                            <input type="checkbox" :value="p.name" v-model="form.permissions" />
                            <div class="toggle-slider"></div>
                          </div>
                        </label>
                      </div>
                    </div>
                  </div>
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
import { ref, reactive, computed, onMounted } from "vue";
import Swal from 'sweetalert2';

const loading = ref(true);
const user = ref(null);

const initials = computed(() => {
  const name = user.value?.username ?? "";
  return name.slice(0, 2).toUpperCase();
});

const form = ref({
  name: "",
  permissions: [],
});

const groupedPermissions = ref({});
const openGroups = reactive({});

const groupIcon = (group) => {
  const icons = {
    Dashboard: 'fa-solid fa-gauge-high',
    Users: 'fa-solid fa-users',
    Roles: 'fa-solid fa-shield-halved',
    Permissions: 'fa-solid fa-key',
  };
  return icons[group] || 'fa-solid fa-lock';
};

const groupLabel = (group) => {
  const labels = {
    Dashboard: 'Dashboard',
    Users: 'Kullanıcılar',
    Roles: 'Roller',
    Permissions: 'İzin Yönetimi',
  };
  return labels[group] || group;
};

const permIcon = (permName) => {
  if (permName.includes('.View')) return 'fa-solid fa-eye';
  if (permName.includes('.Create')) return 'fa-solid fa-plus';
  if (permName.includes('.Edit')) return 'fa-solid fa-pen';
  if (permName.includes('.Delete')) return 'fa-solid fa-trash';
  if (permName.includes('.Access')) return 'fa-solid fa-door-open';
  if (permName.includes('.Assign')) return 'fa-solid fa-user-tag';
  return 'fa-solid fa-check';
};

const toggleGroup = (group) => {
  openGroups[group] = !openGroups[group];
};

onMounted(async () => {
  const token = localStorage.getItem("token");

  try {

    const currentUser = await $fetch("http://localhost:5163/api/auth/me", {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (!currentUser?.permissions?.includes("Dashboard.Access") || !currentUser?.permissions?.includes("Roles.Create")) {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Yetkisiz İşlem', text: 'Bu işlemi yapmak için yetkiniz yok!' });
      await navigateTo("/dashboardRoleList");
      return;
    }

    user.value = currentUser;

    // Fetch all permissions to display checkboxes
    const allPerms = await $fetch("http://localhost:5163/api/permissions", {
      headers: { Authorization: `Bearer ${token}` },
    });
    
    // Group permissions by group property
    const grouped = {};
    allPerms.forEach(p => {
        if (p.group === 'Permissions') return; // İzinler grubunu arayüzde gösterme
        if(!grouped[p.group]) grouped[p.group] = [];
        grouped[p.group].push(p);
    });
    groupedPermissions.value = grouped;

    // Open all groups by default
    Object.keys(grouped).forEach(g => { openGroups[g] = true; });

  } catch (error) {
    console.error("Yetkiler veya kullanıcı bilgisi alınamadı:", error);
    localStorage.removeItem("token");
    await navigateTo("/");
  } finally {
    loading.value = false;
  }
});

const handleLogout = async () => {
  const result = await Swal.fire({ scrollbarPadding: false, heightAuto: false,
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

const saving = ref(false);
const error = ref("");

const handleSubmit = async () => {
  saving.value = true;
  error.value = "";
  const token = localStorage.getItem("token");

  try {
    const roleResponse = await $fetch("http://localhost:5163/api/roles", {
      method: "POST",
      headers: { Authorization: `Bearer ${token}` },
      body: {
        Name: form.value.name,
      },
    });

    await $fetch(`http://localhost:5163/api/roles/${roleResponse.id}/permissions`, {
      method: "PUT",
      headers: { Authorization: `Bearer ${token}` },
      body: {
        Permissions: form.value.permissions,
      },
    });
    saving.value = false;
    await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'success', title: 'Başarılı!', text: 'Rol başarıyla eklendi.', timer: 1500, showConfirmButton: false });
    await navigateTo("/dashboardRoleList");
  } catch (e) {
    console.error("TAM HATA DETAYI:", e);

    if (e.response?.status === 409) {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Hata', text: e.response._data?.message || "Bu rol zaten mevcut." });
    } else if (e.response?.status === 400) {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Hatalı Giriş', text: 'Girdiğin bilgiler eksik veya hatalı.' });
    } else if (e.response?.status === 401) {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Oturum Süresi Doldu', text: 'Oturum süren dolmuş, lütfen tekrar giriş yap.' });
    } else {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Oops...', text: 'Rol oluşturulamadı. Bilgileri kontrol edip tekrar deneyin.' });
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

/* ── Permission Cards Grid ── */
.perm-cards-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 16px;
  margin-top: 8px;
}

.perm-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  overflow: hidden;
  transition: box-shadow 0.2s ease, border-color 0.2s ease;
}

.perm-card:hover {
  border-color: #cbd5e1;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);
}

.perm-card-header {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px 18px;
  cursor: pointer;
  user-select: none;
  background: #f8fafc;
  border-bottom: 1px solid #e2e8f0;
  transition: background 0.15s ease;
}

.perm-card-header:hover {
  background: #f1f5f9;
}

.perm-card-icon {
  width: 38px;
  height: 38px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
  color: #fff;
  background: linear-gradient(135deg, #3b82f6, #2563eb);
  flex-shrink: 0;
}

.perm-card:nth-child(2) .perm-card-icon {
  background: linear-gradient(135deg, #8b5cf6, #7c3aed);
}
.perm-card:nth-child(3) .perm-card-icon {
  background: linear-gradient(135deg, #10b981, #059669);
}
.perm-card:nth-child(4) .perm-card-icon {
  background: linear-gradient(135deg, #f59e0b, #d97706);
}

.perm-card-title {
  font-size: 15px;
  font-weight: 600;
  color: #0f172a;
  flex: 1;
}

.perm-card-chevron {
  font-size: 12px;
  color: #94a3b8;
  transition: transform 0.2s ease;
}

.perm-card-body {
  padding: 8px 0;
}

/* ── Toggle Rows ── */
.perm-toggle-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 18px;
  cursor: pointer;
  transition: background 0.15s ease;
  user-select: none;
}

.perm-toggle-row:hover {
  background: #f8fafc;
}

.perm-toggle-row + .perm-toggle-row {
  border-top: 1px solid #f1f5f9;
}

.perm-toggle-info {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 14px;
  color: #334155;
  font-weight: 500;
}

.perm-toggle-icon {
  width: 28px;
  height: 28px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  background: #f1f5f9;
  color: #64748b;
  flex-shrink: 0;
}

/* ── Toggle Switch ── */
.toggle-wrap {
  position: relative;
  flex-shrink: 0;
}

.toggle-wrap input {
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

.toggle-wrap input:checked + .toggle-slider {
  background-color: #10b981;
}

.toggle-wrap input:checked + .toggle-slider::before {
  transform: translateX(20px);
}

/* ── Form Actions ── */
.form-actions {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 16px;
  margin-top: 24px;
  padding-top: 20px;
  border-top: 1px solid #e5e7eb;
}

.form-error {
  margin-top: 10px;
}

.btn-primary {
  min-width: 150px;
  display: inline-flex;
  justify-content: center;
}
</style>
