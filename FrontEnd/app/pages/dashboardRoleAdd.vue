<template>
  <div v-if="loading" class="skeleton">Yükleniyor…</div>
        <div v-else class="content-inner">
          <div class="page-wrap">
            <div class="page-header">
              <div>
                <h1 class="page-title">Yeni Rol Ekle</h1>
                <p class="page-subtitle">Rol ismini belirle ve izinleri gruplara tıklayarak yönet.</p>
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
                <div v-for="group in permGroups" :key="group.key" class="perm-card"
                  :class="{ 'card-open': openGroups.includes(group.key) }">
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
                  {{ saving ? "Kaydediliyor…" : "Kaydet" }}
                </button>
              </div>
            </form>
          </div>
        </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import Swal from 'sweetalert2';

definePageMeta({ layout: 'dashboard', title: 'Rol Ekle' });

const authStore = useAuthStore();
const roleService = useRoleService();
const permissionService = usePermissionService();

const form = ref({
  name: "",
  permissions: [],
});

const permGroups = ref([]);
const openGroups = ref([]);

const saving = ref(false);
const error = ref("");
const loading = ref(true);

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
  try {
    const u = authStore.currentUser;
    if (!u?.permissions?.includes("Roles.Create")) {
      await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Yetkisiz İşlem', text: 'Bu işlemi yapmak için yetkiniz yok!' });
      await navigateTo("/dashboardRoleList");
      return;
    }

    const allPerms = await permissionService.getPermissions();

    const grouped = {};
    allPerms.forEach(p => {
        if (p.group === 'Permissions') return; 
        if(!grouped[p.group]) grouped[p.group] = [];
        grouped[p.group].push(p);
    });

    const iconMap = {
      'Dashboard': 'fa-solid fa-gauge-high',
      'Users': 'fa-solid fa-users',
      'Roles': 'fa-solid fa-shield-halved',
      'Urls': 'fa-solid fa-link',
    };
    
    const labelMap = {
      'Dashboard': 'Dashboard',
      'Users': 'Kullanıcılar',
      'Roles': 'Roller',
      'Urls': 'URL Kısaltıcı',
    };

    permGroups.value = Object.keys(grouped).map(key => ({
      key,
      label: labelMap[key] || key,
      icon: iconMap[key] || 'fa-solid fa-key',
      perms: grouped[key]
    }));

  } catch (err) {
    console.error(err);
    await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'error', title: 'Hata', text: 'İzinler yüklenirken bir sorun oluştu.' });
    await navigateTo("/dashboardRoleList");
  } finally {
    loading.value = false;
  }
});

const handleSubmit = async () => {
  saving.value = true;
  error.value = "";
  try {
    await roleService.createRole({
      name: form.value.name,
      permissions: form.value.permissions
    });
    
    await Swal.fire({ scrollbarPadding: false, heightAuto: false, icon: 'success', title: 'Başarılı', text: 'Rol başarıyla eklendi.', timer: 2000, showConfirmButton: false });
    await navigateTo("/dashboardRoleList");
  } catch (err) {
    error.value = err.response?._data?.message || "Rol eklenemedi. Lütfen bilgileri kontrol edin.";
  } finally {
    saving.value = false;
  }
};
</script>

