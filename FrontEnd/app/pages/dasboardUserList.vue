<template>
  <div class="page-wrap">
    <div class="page-header">
      <div>
        <h1 class="page-title">Kullanıcı Yönetimi</h1>
        <p class="page-subtitle">
          Toplam {{ totalUsers }} kullanıcı listeleniyor.
        </p>
      </div>

      <div class="header-actions">
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
              <th class="col-checkbox">
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
              <th class="col-actions">İşlemler</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="user in paginatedUsers"
              :key="user.id"
              :class="{ 'row-selected': selectedUsers.includes(user.id) }"
            >
              <td>
                <input
                  type="checkbox"
                  :value="user.id"
                  v-model="selectedUsers"
                  class="checkbox"
                />
              </td>
              <td class="cell-name">{{ user.name }}</td>
              <td class="cell-email">{{ user.email }}</td>
              <td>
                <span class="role-badge">{{ user.role }}</span>
              </td>
              <td class="col-actions">
                <div class="row-actions">
                  <NuxtLink
                    :to="`/dashboardUserControl?id=${user.id}`"
                    class="link-edit"
                  >
                    <i class="fa-solid fa-pencil"></i>
                  </NuxtLink>
                  <button
                    @click="deleteSingleUser(user.id)"
                    class="link-delete"
                  >
                    <i class="fa-solid fa-xmark"></i>
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="paginatedUsers.length === 0">
              <td colspan="5" class="empty-row">
                Kayıtlı kullanıcı bulunamadı.
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="table-footer">
        <span class="range-text">Gösterilen Kayıtlar: {{ textRange }}</span>

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
</template>

<script setup>
import { ref, computed } from "vue";

// Tasarımı ve sayfalamayı görebilmen için 85 adet örnek statik veri ekledim.
// İleride backend API bağladığında burayı veritabanından dolduracaksın.
const users = ref(
  Array.from({ length: 85 }, (_, i) => ({
    id: i + 1,
    name: `Kullanıcı ${i + 1}`,
    email: `kullanici${i + 1}@bilge.com`,
    role: i % 4 === 0 ? "Admin" : "Kullanıcı",
  })),
);

// --- SAYFALAMA AYARLARI (Her sayfada 20 kişi) ---
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

// --- SEÇİM VE KUTUCUK MANTIĞI ---
const selectedUsers = ref([]);

const isAllPageSelected = computed(() => {
  if (paginatedUsers.value.length === 0) return false;
  return paginatedUsers.value.every((user) =>
    selectedUsers.value.includes(user.id),
  );
});

const toggleSelectAllPage = () => {
  const pageUserIds = paginatedUsers.value.map((u) => u.id);

  if (isAllPageSelected.value) {
    selectedUsers.value = selectedUsers.value.filter(
      (id) => !pageUserIds.includes(id),
    );
  } else {
    pageUserIds.forEach((id) => {
      if (!selectedUsers.value.includes(id)) {
        selectedUsers.value.push(id);
      }
    });
  }
};

const textRange = computed(() => {
  if (totalUsers.value === 0) return "0 - 0";
  const start = (currentPage.value - 1) * itemsPerPage + 1;
  const end = Math.min(currentPage.value * itemsPerPage, totalUsers.value);
  return `${start} - ${end}`;
});

// --- AKSİYONLAR / SİLME METOTLARI ---
const deleteSingleUser = (id) => {
  if (confirm("Bu kullanıcıyı silmek istediğinize emin misiniz?")) {
    users.value = users.value.filter((u) => u.id !== id);
    selectedUsers.value = selectedUsers.value.filter((sid) => sid !== id);
  }
};

const deleteSelectedUsers = () => {
  if (
    confirm(
      `${selectedUsers.value.length} kullanıcıyı toplu olarak silmek istediğinize emin misiniz?`,
    )
  ) {
    users.value = users.value.filter(
      (u) => !selectedUsers.value.includes(u.id),
    );
    selectedUsers.value = [];
    currentPage.value = 1;
  }
};
</script>
