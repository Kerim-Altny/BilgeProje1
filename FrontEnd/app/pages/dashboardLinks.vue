<template>
  <div class="dashboard-layout">
    <!-- İstatistik Kartları -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon">
          <i class="fa-solid fa-link"></i>
        </div>
        <div class="stat-info">
          <p class="stat-label">Toplam Link</p>
          <p class="stat-value">{{ stats.totalLinks }}</p>
        </div>
      </div>

      <div class="stat-card">
        <div
          class="stat-icon"
          style="background: rgba(59, 130, 246, 0.12); color: #3b82f6"
        >
          <i class="fa-solid fa-arrow-pointer"></i>
        </div>
        <div class="stat-info">
          <p class="stat-label">Toplam Tıklanma</p>
          <p class="stat-value">{{ stats.totalClicks }}</p>
        </div>
      </div>

      <div class="stat-card">
        <div
          class="stat-icon"
          style="background: rgba(168, 85, 247, 0.12); color: #a855f7"
        >
          <i class="fa-solid fa-users"></i>
        </div>
        <div class="stat-info">
          <p class="stat-label">Link Oluşturan Kullanıcı</p>
          <p class="stat-value">{{ stats.totalUsers }}</p>
        </div>
      </div>
    </div>

    <!-- Tablo -->
    <div class="table-card">
      <div
        class="table-card-padded"
        style="border-bottom: 1px solid var(--line)"
      >
        <div
          style="
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 12px;
          "
        >
          <div>
            <p class="page-title" style="font-size: 18px; margin-bottom: 4px">
              Tüm Kullanıcı Linkleri
            </p>
            <p class="page-subtitle">
              Sistemdeki tüm kısa linkler ve istatistikleri
            </p>
          </div>
          <div class="search-wrap">
            <i class="fa-solid fa-magnifying-glass search-icon"></i>
            <input
              v-model="search"
              @input="onSearch"
              type="text"
              class="field-input search-input"
              placeholder="URL veya kullanıcı ara..."
            />
          </div>
        </div>
      </div>

      <div v-if="loading" class="skeleton" style="margin: 24px">
        Yükleniyor…
      </div>

      <div
        v-else-if="links.length === 0"
        class="table-card-padded"
        style="text-align: center; padding: 48px; color: var(--muted)"
      >
        <i
          class="fa-solid fa-link"
          style="font-size: 32px; margin-bottom: 12px; display: block"
        ></i>
        Hiç link bulunamadı.
      </div>

      <div v-else style="overflow-x: auto">
        <table class="data-table">
          <thead>
            <tr>
              <th>Kullanıcı</th>
              <th>Hedef URL</th>
              <th>Kısa Kod</th>
              <th>Tıklanma</th>
              <th>Oluşturma Tarihi</th>
              <th>Son Tarih</th>
              <th>İşlem</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="link in links" :key="link.id">
              <td>
                <div style="display: flex; align-items: center; gap: 8px">
                  <span
                    class="avatar-sm"
                    style="
                      background: linear-gradient(135deg, #3b82f6, #2563eb);
                      color: #fff;
                      flex-shrink: 0;
                    "
                  >
                    {{ link.username.slice(0, 2).toUpperCase() }}
                  </span>
                  <span>{{ link.username }}</span>
                </div>
              </td>
              <td>
                <span class="long-url-cell" :title="link.originalUrl">{{
                  truncate(link.originalUrl, 45)
                }}</span>
              </td>
              <td>
                <span class="short-code">{{ link.shortCode }}</span>
              </td>
              <td>
                <span class="click-badge">{{ link.clickCount }}</span>
              </td>
              <td>{{ formatDate(link.createdAt) }}</td>
              <td>
                <span
                  v-if="link.expirationDate"
                  :class="
                    isExpired(link.expirationDate)
                      ? 'badge-danger'
                      : 'badge-success'
                  "
                >
                  {{ formatDate(link.expirationDate) }}
                </span>
                <span v-else style="color: var(--muted); font-size: 13px"
                  >—</span
                >
              </td>
              <td>
                <button
                  v-if="canDelete"
                  class="btn-delete-sm"
                  @click="deleteLink(link.id)"
                  title="Sil"
                >
                  <i class="fa-solid fa-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Sayfalama -->
      <div
        v-if="totalPages > 1"
        class="table-card-padded"
        style="
          display: flex;
          justify-content: center;
          gap: 8px;
          padding-top: 16px;
          padding-bottom: 16px;
        "
      >
        <button
          class="chart-filter-btn"
          :disabled="page === 1"
          @click="changePage(page - 1)"
        >
          ←
        </button>
        <button
          v-for="p in totalPages"
          :key="p"
          class="chart-filter-btn"
          :class="{ active: p === page }"
          @click="changePage(p)"
        >
          {{ p }}
        </button>
        <button
          class="chart-filter-btn"
          :disabled="page === totalPages"
          @click="changePage(page + 1)"
        >
          →
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import Swal from "sweetalert2";

definePageMeta({ layout: "dashboard", title: "Link Yönetimi" });

const api = useApi();
const authStore = useAuthStore();

const loading = ref(true);
const links = ref<any[]>([]);
const search = ref("");
const page = ref(1);
const pageSize = 20;

const stats = ref({ totalLinks: 0, totalClicks: 0, totalUsers: 0 });

const canDelete = computed(
  () => !!authStore.currentUser?.permissions?.includes("Links.Delete"),
);

const totalPages = computed(() => Math.ceil(stats.value.totalLinks / pageSize));

let searchTimer: any = null;
const onSearch = () => {
  clearTimeout(searchTimer);
  searchTimer = setTimeout(() => {
    page.value = 1;
    fetchLinks();
  }, 400);
};

const fetchLinks = async () => {
  loading.value = true;
  try {
    const res = await api<any>(
      `/api/links/admin?search=${search.value}&page=${page.value}&pageSize=${pageSize}`,
      {
        method: "GET",
      },
    );
    stats.value = {
      totalLinks: res.totalLinks,
      totalClicks: res.totalClicks,
      totalUsers: res.totalUsers,
    };
    links.value = res.links;
  } catch (e: any) {
    console.error("Linkler yüklenemedi:", e);
  } finally {
    loading.value = false;
  }
};

const changePage = (p: number) => {
  page.value = p;
  fetchLinks();
};

const deleteLink = async (id: number) => {
  const result = await Swal.fire({
    scrollbarPadding: false,
    heightAuto: false,
    icon: "warning",
    title: "Emin misin?",
    text: "Bu link kalıcı olarak silinecek.",
    showCancelButton: true,
    confirmButtonText: "Evet, sil",
    cancelButtonText: "Vazgeç",
    confirmButtonColor: "#ef4444",
  });
  if (!result.isConfirmed) return;
  try {
    await api(`/api/links/admin/${id}`, { method: "DELETE" });
    await fetchLinks();
    Swal.fire({
      scrollbarPadding: false,
      heightAuto: false,
      icon: "success",
      title: "Silindi",
      timer: 1500,
      showConfirmButton: false,
    });
  } catch (e: any) {
    Swal.fire({
      scrollbarPadding: false,
      heightAuto: false,
      icon: "error",
      title: "Hata",
      text: "Link silinemedi.",
    });
  }
};

const truncate = (str: string, len: number) =>
  str?.length > len ? str.slice(0, len) + "…" : str;
const formatDate = (d: string | Date | null) =>
  d ? new Date(d).toLocaleDateString("tr-TR") : "—";
const isExpired = (d: string | Date) => new Date(d) < new Date();

onMounted(fetchLinks);
</script>

<style scoped>
.search-wrap {
  position: relative;
}

.search-icon {
  position: absolute;
  left: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--muted);
  font-size: 13px;
  pointer-events: none;
}

.search-input {
  padding-left: 34px !important;
  min-width: 260px;
}

.btn-delete-sm {
  background: rgba(239, 68, 68, 0.12);
  color: #ef4444;
  border: none;
  border-radius: 8px;
  padding: 6px 10px;
  cursor: pointer;
  font-size: 13px;
  transition: background 0.2s;
}

.btn-delete-sm:hover {
  background: rgba(239, 68, 68, 0.25);
}

.badge-danger {
  background: rgba(239, 68, 68, 0.12);
  color: #ef4444;
  padding: 3px 8px;
  border-radius: 6px;
  font-size: 12px;
}

.badge-success {
  background: rgba(16, 185, 129, 0.12);
  color: #10b981;
  padding: 3px 8px;
  border-radius: 6px;
  font-size: 12px;
}
</style>
