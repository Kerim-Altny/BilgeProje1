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
          <span class="nav-icon">◆</span>
          <span>Anasayfa</span>
        </NuxtLink>
        <NuxtLink to="/dashboardUserList" class="nav-item active">
          <span class="nav-icon">◆</span>
          <span>Kullanıcılar</span>
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
            <span class="greeting"
              >Hoş geldin, <strong>{{ user?.username }}</strong></span
            >
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

              <div class="header-actions" v-if="userRole === 'Admin'">
                <button
                  v-if="selectedUsers.length > 0"
                  @click="deleteSelectedUsers"
                  class="btn-danger"
                >
                  Seçilenleri Sil ({{ selectedUsers.length }})
                </button>

                <NuxtLink to="/dashboardUserAdd" class="btn-primary">
                  + Yeni Kullanıcı Ekle
                </NuxtLink>
              </div>
            </div>

            <div class="table-card">
              <div class="table-scroll">
                <table class="users-table">
                  <thead>
                    <tr>
                      <th class="col-checkbox" v-if="userRole === 'Admin'">
                        <input
                          type="checkbox"
                          :checked="isAllPageSelected"
                          @change="toggleSelectAllPage"
                          class="checkbox"
                        />
                      </th>
                      <th>Ad Soyad</th>
                      <th>E-posta</th>
                      <th>Rol</th>
                      <th class="col-actions" v-if="userRole === 'Admin'">İşlemler</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr
                      v-for="u in paginatedUsers"
                      :key="u.id"
                      :class="{ 'row-selected': selectedUsers.includes(u.id) }"
                    >
                      <td v-if="userRole === 'Admin'">
                        <input
                          type="checkbox"
                          :value="u.id"
                          v-model="selectedUsers"
                          class="checkbox"
                        />
                      </td>
                      <td class="cell-name">{{ u.name }}</td>
                      <td class="cell-email">{{ u.email }}</td>
                      <td>
                        <span class="role-badge">{{ u.role }}</span>
                      </td>
                      <td class="col-actions" v-if="userRole === 'Admin'">
                        <div class="row-actions">
                          <NuxtLink
                            :to="`/dashboardUserControl?id=${u.id}`"
                            class="link-edit"
                          >
                            <i class="fa-solid fa-pencil"></i>
                          </NuxtLink>
                          <button
                            @click="deleteSingleUser(u.id)"
                            class="link-delete"
                          >
                            <i class="fa-solid fa-xmark"></i>
                          </button>
                        </div>
                      </td>
                    </tr>
                    <tr v-if="paginatedUsers.length === 0">
                      <td :colspan="userRole === 'Admin' ? 5 : 3" class="empty-row">
                        Kayıtlı kullanıcı bulunamadı.
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <div class="table-footer">
                <span class="range-text"
                  >Gösterilen Kayıtlar: {{ textRange }}</span
                >

                <div class="pagination">
                  <button
                    @click="currentPage--"
                    :disabled="currentPage === 1"
                    class="page-btn"
                  >
                    Önceki
                  </button>

                  <span class="page-indicator"
                    >Sayfa {{ currentPage }} / {{ totalPages }}</span
                  >

                  <button
                    @click="currentPage++"
                    :disabled="currentPage === totalPages"
                    class="page-btn"
                  >
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

const loading = ref(true);
const user = ref(null);

const initials = computed(() => {
  const name = user.value?.username ?? "";
  return name.slice(0, 2).toUpperCase();
});

// --- API'DEN KULLANICILARI ÇEKME FONKSİYONU ---
const users = ref([]);

const fetchUsersList = async (token) => {
  try {
    const response = await $fetch("http://localhost:5163/api/users", {
      headers: { Authorization: `Bearer ${token}` },
    });

    // HTML tasarımdaki "u.name" kısmını bozmamak için
    // backend'den gelen "username" verisini "name" olarak eşliyoruz.
    users.value = response.map((u) => ({
      id: u.id,
      name: u.username,
      email: u.email,
      role: u.role || "Kullanıcı",
    }));
  } catch (error) {
    console.error("Kullanıcılar çekilirken hata oluştu:", error);
  }
};

const userRole = ref("");

onMounted(async () => {
  const token = localStorage.getItem("token");
  userRole.value = localStorage.getItem("role") || "Kullanıcı";

  try {
    // 1. Giriş yapan adminin bilgilerini çek
    user.value = await $fetch("http://localhost:5163/api/auth/me", {
      headers: { Authorization: `Bearer ${token}` },
    });

    // 2. Tablo için tüm kullanıcıları çek
    await fetchUsersList(token);
  } catch (error) {
    localStorage.removeItem("token");
    await navigateTo("/");
  } finally {
    loading.value = false;
  }
});

const handleLogout = async () => {
  localStorage.removeItem("token");
  await navigateTo("/");
};

// --- SAYFALAMA VE SEÇİM MANTIĞI ---
const currentPage = ref(1);
const itemsPerPage = 20;

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

// --- SİLME İŞLEMLERİ (API ENTEGRELİ) ---

const deleteSingleUser = async (id) => {
  if (confirm("Bu kullanıcıyı silmek istediğinize emin misiniz?")) {
    const token = localStorage.getItem("token");
    try {
      // Backend'e silme isteği at
      await $fetch(`http://localhost:5163/api/users/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      });

      // Arayüzden de kaldır
      users.value = users.value.filter((u) => u.id !== id);
      selectedUsers.value = selectedUsers.value.filter((sid) => sid !== id);
    } catch (e) {
      alert("Silme işlemi başarısız oldu.");
      console.error(e);
    }
  }
};

const deleteSelectedUsers = async () => {
  if (
    confirm(
      `${selectedUsers.value.length} kullanıcıyı toplu olarak silmek istediğinize emin misiniz?`,
    )
  ) {
    const token = localStorage.getItem("token");
    try {
      // Seçilen tüm kullanıcı id'leri için backend'e silme isteği atıyoruz
      // Promise.all ile hepsini paralel ve hızlıca siliyoruz
      await Promise.all(
        selectedUsers.value.map((id) =>
          $fetch(`http://localhost:5163/api/users/${id}`, {
            method: "DELETE",
            headers: { Authorization: `Bearer ${token}` },
          }),
        ),
      );

      // İşlem başarılı olursa arayüzden seçili olanları temizle
      users.value = users.value.filter(
        (u) => !selectedUsers.value.includes(u.id),
      );
      selectedUsers.value = [];

      // Eğer sayfa boşaldıysa bir önceki sayfaya geç
      if (paginatedUsers.value.length === 0 && currentPage.value > 1) {
        currentPage.value--;
      }
    } catch (e) {
      alert("Bazı kullanıcılar silinirken hata oluştu.");
      console.error(e);
      // Hata olsa bile listeyi güncel tutmak için API'den verileri tekrar çekebiliriz
      await fetchUsersList(token);
    }
  }
};
</script>

<style scoped>
/* CSS kısımların aynı kaldığı için buraya eklemedim, mevcut CSS'lerini kullanmaya devam edebilirsin */
</style>
