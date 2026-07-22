<template>
  <div class="dashboard-layout">
    <!-- Başlık + Yeni Link Butonu -->
    <div class="table-card table-card-padded" style="margin-top: 0;">
      <div style="display: flex; justify-content: space-between; align-items: center;">
        <div>
          <p class="page-title" style="font-size: 22px; margin-bottom: 4px;">Linklerim</p>
          <p class="page-subtitle">Oluşturduğunuz tüm kısa linkler</p>
        </div>
        <NuxtLink to="/user/create" class="btn-primary" style="text-decoration: none; padding: 12px 24px; border-radius: 12px; font-size: 14px; font-weight: 600;">
          <i class="fa-solid fa-plus" style="margin-right: 8px;"></i> Yeni Link
        </NuxtLink>
      </div>
    </div>

    <!-- Arama -->
    <div class="table-card table-card-padded">
      <input
        v-model="search"
        type="text"
        class="user-search-input"
        placeholder="🔍  Link veya URL'de ara..."
      />
    </div>

    <!-- Tablo -->
    <div class="table-card">
      <div v-if="loading" class="table-card-padded skeleton">Yükleniyor…</div>

      <div v-else-if="filteredLinks.length === 0" class="table-card-padded user-empty-state">
        <i class="fa-solid fa-link" style="font-size: 36px; color: var(--accent); margin-bottom: 14px;"></i>
        <p style="font-size: 16px; font-weight: 600; color: var(--ink); margin-bottom: 6px;">
          {{ search ? 'Sonuç bulunamadı' : 'Henüz link oluşturmadınız' }}
        </p>
        <p style="font-size: 14px; color: var(--ink-soft);">
          {{ search ? 'Farklı bir arama deneyin.' : 'İlk kısa linkinizi oluşturun!' }}
        </p>
        <NuxtLink v-if="!search" to="/user/create" class="btn-primary" style="text-decoration: none; margin-top: 16px; padding: 10px 24px; border-radius: 10px; font-size: 14px; display: inline-block;">
          Link Oluştur
        </NuxtLink>
      </div>

      <table v-else class="data-table">
        <thead>
          <tr>
            <th>Kısa URL</th>
            <th>Hedef URL</th>
            <th>Tıklanma</th>
            <th>Oluşturulma</th>
            <th>İşlemler</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="link in filteredLinks" :key="link.id" class="data-row">
            <td>
              <div class="short-url-cell">
                <a :href="`/r/${link.shortCode}`" target="_blank" class="short-code-link">
                  /r/{{ link.shortCode }}
                </a>
                <button class="copy-btn" @click="copyUrl(link.shortCode)" :title="'Kopyala'">
                  <i class="fa-regular fa-copy"></i>
                </button>
              </div>
            </td>
            <td>
              <a :href="link.originalUrl" target="_blank" class="long-url-link" :title="link.originalUrl">
                {{ truncate(link.originalUrl, 45) }}
              </a>
            </td>
            <td>
              <span class="click-badge">
                <i class="fa-solid fa-arrow-pointer" style="font-size: 11px; margin-right: 4px;"></i>
                {{ link.clickCount }}
              </span>
            </td>
            <td style="color: var(--ink-soft); font-size: 14px;">{{ formatDate(link.createdAt) }}</td>
            <td>
              <div style="display: flex; gap: 8px;">
                <button class="copy-btn" @click="copyUrl(link.shortCode)" title="Kopyala" style="padding: 6px 10px;">
                  <i class="fa-regular fa-copy"></i>
                </button>
                <button class="delete-btn" @click="deleteLink(link.id)" title="Sil" style="padding: 6px 10px;">
                  <i class="fa-solid fa-trash"></i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Kopyalama Bildirimi -->
    <Transition name="toast">
      <div v-if="showToast" class="copy-toast">
        <i class="fa-solid fa-check"></i> Link kopyalandı!
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import Swal from 'sweetalert2';

definePageMeta({ layout: 'dashboard', title: 'Linklerim' });

const api = useApi();
const links = ref<any[]>([]);
const loading = ref(true);
const search = ref('');
const showToast = ref(false);

const filteredLinks = computed(() => {
  if (!search.value.trim()) return links.value;
  const q = search.value.toLowerCase();
  return links.value.filter((l: any) =>
    l.shortCode.toLowerCase().includes(q) ||
    l.originalUrl.toLowerCase().includes(q)
  );
});

const loadLinks = async () => {
  try {
    const data = await api<any>('/api/links/my', { method: 'GET' });
    links.value = data.links;
  } catch (e: any) {
    console.error('Linkler yüklenemedi:', e);
  } finally {
    loading.value = false;
  }
};

const copyUrl = async (code: string) => {
  const url = `${window.location.origin}/r/${code}`;
  await navigator.clipboard.writeText(url);
  showToast.value = true;
  setTimeout(() => { showToast.value = false; }, 2500);
};

const deleteLink = async (id: number) => {
  const result = await Swal.fire({
    title: 'Linki sil?',
    text: 'Bu işlem geri alınamaz.',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#ef4444',
    cancelButtonColor: '#6b7280',
    confirmButtonText: 'Evet, sil',
    cancelButtonText: 'İptal',
    scrollbarPadding: false,
    heightAuto: false,
  });
  if (!result.isConfirmed) return;

  try {
    await api(`/api/links/my/${id}`, { method: 'DELETE' });
    links.value = links.value.filter((l: any) => l.id !== id);
    await Swal.fire({ icon: 'success', title: 'Silindi!', text: 'Link başarıyla silindi.', timer: 1500, showConfirmButton: false, scrollbarPadding: false, heightAuto: false });
  } catch (e: any) {
    await Swal.fire({ icon: 'error', title: 'Hata!', text: 'Silme işlemi başarısız.', scrollbarPadding: false, heightAuto: false });
  }
};

const truncate = (str: string, len: number) => str?.length > len ? str.slice(0, len) + '…' : str;
const formatDate = (d: string | Date) => new Date(d).toLocaleDateString('tr-TR');

onMounted(loadLinks);
</script>
