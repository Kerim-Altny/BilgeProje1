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
          <h1 class="page-title">Kullanıcılar</h1>
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
                <h1 class="page-title">Kullanıcı Yönetimi</h1>
                <p class="page-subtitle">
                  Toplam {{ totalUsers }} kullanıcı listeleniyor.
                </p>
              </div>

              <div class="header-actions" v-if="canAdd || canDelete">
                <button v-if="canDelete && selectedUsers.length > 0" @click="deleteSelectedUsers" class="btn-danger">
                  Seçilenleri Sil ({{ selectedUsers.length }})
                </button>

                <NuxtLink v-if="canAdd" to="/dashboardUserAdd" class="btn-primary">
                  + Yeni Kullanıcı Ekle
                </NuxtLink>
              </div>
            </div>

            <div class="table-card">
              <div class="table-scroll">
                <table class="users-table">
                  <thead>
                    <tr>
                      <th class="col-checkbox" v-if="canDelete">
                        <input type="checkbox" :checked="isAllPageSelected" @change="toggleSelectAllPage"
                          class="checkbox" />
                      </th>
                      <th>Ad Soyad</th>
                      <th>E-posta</th>
                      <th>Rol</th>
                      <th class="col-actions" v-if="canEdit || canDelete">İşlemler</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="u in paginatedUsers" :key="u.id"
                      :class="{ 'row-selected': selectedUsers.includes(u.id) }">
                      <td v-if="canDelete">
                        <input type="checkbox" :value="u.id" v-model="selectedUsers" class="checkbox" />
                      </td>
                      <td class="cell-name">{{ u.name }}</td>
                      <td class="cell-email">{{ u.email }}</td>
                      <td>
                        <span class="role-badge">{{ u.role }}</span>
                      </td>
                      <td class="col-actions" v-if="canEdit || canDelete">
                        <div class="row-actions">
                          <NuxtLink v-if="canEdit" :to="`/dashboardUserControl?id=${u.id}`" class="link-edit">
                            <i class="fa-solid fa-pencil"></i>
                          </NuxtLink>
                          <button v-if="canDelete" @click="deleteSingleUser(u.id)" class="link-delete">
                            <i class="fa-solid fa-xmark"></i>
                          </button>
                        </div>
                      </td>
                    </tr>
                    <tr v-if="paginatedUsers.length === 0">
                      <td :colspan="visibleColumnCount" class="empty-row">
                        Kayıtlı kullanıcı bulunamadı.
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <div class="table-footer">
                <span class="range-text">Gösterilen Kayıtlar: {{ textRange }}</span>

                <div class="pagination">
                  <button @click="currentPage--" :disabled="currentPage === 1" class="page-btn">
                    Önceki
                  </button>

                  <span class="page-indicator">Sayfa {{ currentPage }} / {{ totalPages }}</span>

                  <button @click="currentPage++" :disabled="currentPage === totalPages" class="page-btn">
                    Sonraki
                  </button>
                </div>
              </div>
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

const authStore = useAuthStore();
const authService = useAuthService();
const userService = useUserService();

const loading = ref(true);
const user = ref(null);

const initials = computed(() => {
  const name = user.value?.username ?? "";
  return name.slice(0, 2).toUpperCase();
});

const users = ref([]);

const fetchUsersList = async () => {
  try {
    const response = await userService.getUsers();

    users.value = response.map((u) => ({
      id: u.id,
      name: u.username,
      email: u.email,
      role: u.roleName || "Kullanıcı",
    }));
  } catch (error) {
    console.error("Kullanıcılar çekilirken hata oluştu:", error);
  }
};

const canAdd = computed(() => !!user.value?.permissions?.includes("Users.Create"));
const canEdit = computed(() => !!user.value?.permissions?.includes("Users.Edit"));
const canDelete = computed(() => !!user.value?.permissions?.includes("Users.Delete"));

onMounted(async () => {
  try {
    const currentUser = await authService.getMe();

    if (!currentUser?.permissions?.includes("Dashboard.Access")) {
      await Swal.fire({ icon: 'error', title: 'Erişim Engellendi', text: 'Bu panele erişim yetkiniz yok!' });
      authStore.clearAuth();
      await navigateTo("/");
      return;
    }

    if (!currentUser?.permissions?.includes("Users.View") && !currentUser?.permissions?.includes("Users.Create") && !currentUser?.permissions?.includes("Users.Edit") && !currentUser?.permissions?.includes("Users.Delete")) {
      await Swal.fire({ icon: 'error', title: 'Yetkisiz İşlem', text: 'Bu sayfaya erişim yetkiniz yok!' });
      await navigateTo("/dashboard");
      return;
    }

    user.value = currentUser;

    await fetchUsersList();
  } catch (error) {
    authStore.clearAuth();
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
    authStore.clearAuth();
    await navigateTo("/");
  }
};

const currentPage = ref(1);
const itemsPerPage = 15;

const totalUsers = computed(() => users.value.length);
const totalPages = computed(
  () => Math.ceil(totalUsers.value / itemsPerPage) || 1,
);

const paginatedUsers = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  return users.value.slice(start, end);
});

const selectedUsers = ref([]);

const isAllPageSelected = computed(() => {
  if (paginatedUsers.value.length === 0) return false;
  return paginatedUsers.value.every((u) => selectedUsers.value.includes(u.id));
});

const toggleSelectAllPage = () => {
  const pageUserIds = paginatedUsers.value.map((u) => u.id);
  if (isAllPageSelected.value) {
    selectedUsers.value = selectedUsers.value.filter(
      (id) => !pageUserIds.includes(id),
    );
  } else {
    pageUserIds.forEach((id) => {
      if (!selectedUsers.value.includes(id)) selectedUsers.value.push(id);
    });
  }
};

const textRange = computed(() => {
  if (totalUsers.value === 0) return "0 - 0";
  const start = (currentPage.value - 1) * itemsPerPage + 1;
  const end = Math.min(currentPage.value * itemsPerPage, totalUsers.value);
  return `${start} - ${end}`;
});

const deleteSingleUser = async (id) => {
  const result = await Swal.fire({
    title: 'Emin misiniz?',
    text: "Bu kullanıcıyı silmek istediğinize emin misiniz?",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#3085d6',
    cancelButtonColor: '#d33',
    confirmButtonText: 'Evet, sil!',
    cancelButtonText: 'İptal'
  });

  if (result.isConfirmed) {
    try {
      await userService.deleteUser(id);

      users.value = users.value.filter((u) => u.id !== id);
      selectedUsers.value = selectedUsers.value.filter((sid) => sid !== id);

      await Swal.fire({ icon: 'success', title: 'Silindi!', text: 'Kullanıcı başarıyla silindi.', timer: 1500, showConfirmButton: false });
    } catch (e) {
      await Swal.fire({ icon: 'error', title: 'Hata', text: 'Silme işlemi başarısız oldu.' });
      console.error(e);
    }
  }
};

const deleteSelectedUsers = async () => {
  const result = await Swal.fire({
    title: 'Emin misiniz?',
    text: `${selectedUsers.value.length} kullanıcıyı toplu olarak silmek istediğinize emin misiniz?`,
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#3085d6',
    cancelButtonColor: '#d33',
    confirmButtonText: 'Evet, sil!',
    cancelButtonText: 'İptal'
  });

  if (result.isConfirmed) {
    try {

      await Promise.all(
        selectedUsers.value.map((id) =>
          userService.deleteUser(id)
        ),
      );

      users.value = users.value.filter(
        (u) => !selectedUsers.value.includes(u.id),
      );
      selectedUsers.value = [];

      if (paginatedUsers.value.length === 0 && currentPage.value > 1) {
        currentPage.value--;
      }

      await Swal.fire({ icon: 'success', title: 'Silindi!', text: 'Seçili kullanıcılar başarıyla silindi.', timer: 1500, showConfirmButton: false });
    } catch (e) {
      await Swal.fire({ icon: 'error', title: 'Hata', text: 'Bazı kullanıcılar silinirken hata oluştu.' });
      console.error(e);
      await fetchUsersList();
    }
  }
};
</script>

<style scoped></style>
