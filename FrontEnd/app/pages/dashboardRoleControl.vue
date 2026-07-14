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
                <p class="page-subtitle">Rol ismini ve izin gruplarını güncelleyebilirsin.</p>
              </div>

              <NuxtLink to="/dashboardRoleList" class="btn-secondary">
                ← Listeye Dön
              </NuxtLink>
            </div>

            <form @submit.prevent="handleSubmit">
              <div class="form-card name-card">
                <label class="field-label">Rol İsmi</label>
                <input v-model="form.name" type="text" required placeholder="Örn. Editör" class="field-input" />
              </div>

              <h2 class="perms-section-title">İzin Grupları</h2>

              <div class="cards-grid">
                <div v-for="group in permGroups" :key="group.key" class="perm-card" :class="{ 'card-open': openGroups.includes(group.key) }">
                  <div class="card-header" @click="toggleGroup(group.key)">
                    <div class="card-title-wrap">
                      <div class="card-icon-bg" :class="openGroups.includes(group.key) ? 'icon-green' : 'icon-default'">
                        <i :class="group.icon"></i>
                      </div>
                      <div>
                        <h2 class="card-title">{{ group.label }}</h2>
                        <p class="card-desc">{{ selectedCount(group.perms) }} / {{ group.perms.filter(p => !p.name.endsWith('.View') && !p.name.endsWith('.Access')).length }} seçili</p>
                      </div>
                    </div>
                    <div class="card-eye" :class="{ 'eye-active': openGroups.includes(group.key) }">
                      <i :class="openGroups.includes(group.key) ? 'fa-solid fa-eye' : 'fa-solid fa-eye-slash'"></i>
                    </div>
                  </div>

                  <transition name="expand">
                    <div v-if="openGroups.includes(group.key)" class="card-body">
                      <div class="divider"></div>
                      <div class="permissions-list">
                        <label v-for="p in group.perms.filter(p => !p.name.endsWith('.View') && !p.name.endsWith('.Access'))" :key="p.name" class="toggle-switch">
                          <input type="checkbox" :value="p.name" v-model="form.permissions" />
                          <div class="toggle-slider"></div>
                          <div class="perm-info">
                            <span class="perm-name">{{ p.description }}</span>
                          </div>
                        </label>
                      </div>
                    </div>
                  </transition>
                </div>
              </div>

              <p v-if="error" class="form-error">{{ error }}</p>

              <div class="form-actions">
                <NuxtLink to="/dashboardRoleList" class="btn-secondary">Vazgeç</NuxtLink>
                <button type="submit" :disabled="saving" class="btn-primary">
                  {{ saving ? "Güncelleniyor…" : "Güncelle" }}
                </button>
              </div>
            </form>
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

const api = useApi();
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

const permGroups = ref([]);
const openGroups = ref([]);

const saving = ref(false);
const error = ref("");

const toggleGroup = (key) => {
  const isCurrentlyOpen = openGroups.value.includes(key);
  const basePermName = key === 'Dashboard' ? 'Dashboard.Access' : `${key}.View`;

  if (isCurrentlyOpen) {
    openGroups.value = openGroups.value.filter(k => k !== key);
    const groupPermNames = permGroups.value.find(g => g.key === key)?.perms.map(p => p.name) || [];
    form.value.permissions = form.value.permissions.filter(p => !groupPermNames.includes(p));
  } else {
    openGroups.value.push(key);
    if (!form.value.permissions.includes(basePermName)) {
      form.value.permissions.push(basePermName);
    }
  }
};

const selectedCount = (perms) => {
  const innerPerms = perms.filter(p => !p.name.endsWith('.View') && !p.name.endsWith('.Access'));
  return innerPerms.filter(p => form.value.permissions.includes(p.name)).length;
};

onMounted(async () => {
  const token = localStorage.getItem("token");

  try {
    const u = await api("/api/auth/me", {
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
      const data = await api(`/api/roles/${roleId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      roleData.value = data;
      form.value.name = data.name;
      form.value.permissions = data.permissions || [];

      const allPerms = await api("/api/permissions", {
        headers: { Authorization: `Bearer ${token}` },
      });

      const grouped = {};
      allPerms.forEach(p => {
        if (p.group === 'Permissions') return; // Hide Permissions group
        if (!grouped[p.group]) grouped[p.group] = [];
        grouped[p.group].push(p);
      });

      const iconMap = {
        'Dashboard': 'fa-solid fa-gauge-high',
        'Users': 'fa-solid fa-users',
        'Roles': 'fa-solid fa-shield-halved',
      };
      
      const labelMap = {
        'Dashboard': 'Dashboard',
        'Users': 'Kullanıcılar',
        'Roles': 'Roller',
      };

      permGroups.value = Object.keys(grouped).map(key => ({
        key,
        label: labelMap[key] || key,
        icon: iconMap[key] || 'fa-solid fa-key',
        perms: grouped[key]
      }));

      openGroups.value = permGroups.value.filter(g => {
         const base = g.key === 'Dashboard' ? 'Dashboard.Access' : `${g.key}.View`;
         return form.value.permissions.includes(base);
      }).map(g => g.key);

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
    await api(`/api/roles/${roleId}`, {
      method: "PUT",
      headers: { Authorization: `Bearer ${token}` },
      body: {
        Name: form.value.name,
      },
    });

    await api(`/api/roles/${roleId}/permissions`, {
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
