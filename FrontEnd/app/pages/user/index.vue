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
        <div class="stat-icon" style="background: rgba(59,130,246,0.12); color: #3b82f6;">
          <i class="fa-solid fa-arrow-pointer"></i>
        </div>
        <div class="stat-info">
          <p class="stat-label">Toplam Tıklanma</p>
          <p class="stat-value">{{ stats.totalClicks }}</p>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon" style="background: rgba(168,85,247,0.12); color: #a855f7;">
          <i class="fa-solid fa-fire"></i>
        </div>
        <div class="stat-info">
          <p class="stat-label">En Popüler Link</p>
          <p class="stat-value" style="font-size: 16px; word-break: break-all;">{{ stats.topLink || '—' }}</p>
        </div>
      </div>
    </div>

    <!-- Tıklama Grafiği -->
    <div class="table-card chart-card dark-chart-card">
      <div class="chart-header">
        <p class="page-title " style="font-size: 18px; margin-bottom: 4px;">Tıklama Grafiği</p>
        <p class="page-subtitle dark-text-sub" style="margin-bottom: 0;">Son 7 günde linklerinize gelen toplam tıklamalar</p>
      </div>
      <div class="chart-container">
        <LineChart v-if="chartData.labels.length" :data="chartData" :options="chartOptions" />
      </div>
    </div>

    <!-- Son Linkler -->
    <div class="table-card">
      <div class="table-card-padded" style="border-bottom: 1px solid var(--line);">
        <div style="display: flex; justify-content: space-between; align-items: center;">
          <div>
            <p class="page-title" style="font-size: 18px; margin-bottom: 4px;">Son Oluşturulan Linkler</p>
            <p class="page-subtitle">En son eklenen 5 kısa linkiniz</p>
          </div>
          <NuxtLink to="/user/create" class="btn-primary" style="text-decoration: none; padding: 10px 20px; border-radius: 10px; font-size: 14px;">
            <i class="fa-solid fa-plus" style="margin-right: 6px;"></i> Yeni Link
          </NuxtLink>
        </div>
      </div>

      <div v-if="recentLinks.length === 0" class="table-card-padded user-empty-state">
        <i class="fa-solid fa-link" style="font-size: 32px; color: var(--accent); margin-bottom: 12px;"></i>
        <p>Henüz link oluşturmadınız.</p>
        <NuxtLink to="/user/create" class="btn-primary" style="text-decoration: none; margin-top: 12px; padding: 10px 20px; border-radius: 10px; font-size: 14px; display: inline-block;">
          İlk linki oluştur
        </NuxtLink>
      </div>

      <div v-else style="overflow-x: auto;">
        <table class="data-table">
        <thead>
          <tr>
            <th>Kısa URL</th>
            <th>Hedef URL</th>
            <th>Tıklanma</th>
            <th>Tarih</th>
            <th>İşlem</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="link in recentLinks" :key="link.id">
            <td>
              <div class="short-url-cell">
                <span class="short-code">{{ shortBase }}/r/{{ link.shortCode }}</span>
                <button class="copy-btn" @click="copyUrl(link.shortCode)" :title="'Kopyala'">
                  <i class="fa-regular fa-copy"></i>
                </button>
              </div>
            </td>
            <td>
              <span class="long-url-cell" :title="link.originalUrl">{{ truncate(link.originalUrl, 40) }}</span>
            </td>
            <td>
              <span class="click-badge">{{ link.clickCount }}</span>
            </td>
            <td>{{ formatDate(link.createdAt) }}</td>
            <td>
              <NuxtLink to="/user/links" class="table-action-btn">Detay</NuxtLink>
            </td>
          </tr>
        </tbody>
      </table>
      </div>
    </div>

    <!-- Hızlı Erişim -->
    <div class="two-col">
      <div class="table-card table-card-padded recent-card">
        <p class="page-title" style="font-size: 18px; margin-bottom: 4px;">Hızlı İşlemler</p>
        <p class="page-subtitle">Sık kullanılan kısayollar</p>
        <div class="quick-actions">
          <NuxtLink to="/user/create" class="btn-secondary quick-btn" style="text-decoration: none;">
            <i class="fa-solid fa-plus" style="margin-right: 8px;"></i> Link Oluştur
          </NuxtLink>
          <NuxtLink to="/user/links" class="btn-secondary quick-btn" style="text-decoration: none;">
            <i class="fa-solid fa-list" style="margin-right: 8px;"></i> Tüm Linkler
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { Line as LineChart } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, LineElement, PointElement, CategoryScale, LinearScale, Filler } from 'chart.js'

definePageMeta({ layout: 'dashboard', title: 'Ana Sayfa' });
ChartJS.register(Title, Tooltip, Legend, LineElement, PointElement, CategoryScale, LinearScale, Filler)

const api = useApi();

const stats = ref({ totalLinks: 0, totalClicks: 0, topLink: '' });
const recentLinks = ref<any[]>([]);
const shortBase = ref(window?.location?.origin ?? '');

const loadData = async () => {
  try {
    const [data, chartRaw] = await Promise.all([
      api<any>('/api/links/my', { method: 'GET' }),
      api<any[]>('/api/links/my/chart', { method: 'GET' }),
    ]);

    stats.value = {
      totalLinks: data.totalLinks,
      totalClicks: data.totalClicks,
      topLink: data.topLink || ''
    };
    recentLinks.value = (data.links ?? []).slice(0, 5);

    // Grafiği gerçek veriyle güncelle
    chartData.value = {
      labels: chartRaw.map((d: any) => `${d.dayName}\n${d.date}`),
      datasets: [{
        label: 'Tıklamalar',
        data: chartRaw.map((d: any) => d.clicks),
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59, 130, 246, 0.2)',
        fill: true,
        tension: 0.4,
        borderWidth: 2,
        pointBackgroundColor: '#3b82f6',
        pointBorderColor: '#fff',
        pointRadius: 4,
        pointHoverRadius: 6,
      }]
    };
  } catch (e: any) {
    console.error('User dashboard yüklenemedi:', e);
  }
};

const truncate = (str: string, len: number) => str?.length > len ? str.slice(0, len) + '…' : str;
const formatDate = (d: string | Date) => new Date(d).toLocaleDateString('tr-TR');

const copyUrl = async (code: string) => {
  const url = `${shortBase.value}/r/${code}`;
  await navigator.clipboard.writeText(url);
};

const chartData = ref({
  labels: ['Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt', 'Paz'],
  datasets: [{
    label: 'Tıklamalar',
    data: [0, 0, 0, 0, 0, 0, 0],
    borderColor: '#3b82f6',
    backgroundColor: 'rgba(59, 130, 246, 0.2)',
    fill: true,
    tension: 0.4,
    borderWidth: 2,
    pointBackgroundColor: '#3b82f6',
    pointBorderColor: '#fff',
    pointRadius: 4,
    pointHoverRadius: 6,
  }]
});

const chartOptions = ref<any>({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: {
      backgroundColor: 'rgba(15, 23, 42, 0.9)',
      titleColor: '#f1f5f9',
      bodyColor: '#cbd5e1',
      borderColor: 'rgba(255,255,255,0.1)',
      borderWidth: 1,
      padding: 10,
      displayColors: false,
      callbacks: {
        label: (ctx: any) => `${ctx.raw} tıklama`
      }
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      grid: { color: 'rgba(255, 255, 255, 0.05)', drawBorder: false },
      ticks: { color: '#94a3b8', font: { size: 11, family: 'Inter' } }
    },
    x: {
      grid: { display: false, drawBorder: false },
      ticks: { color: '#94a3b8', font: { size: 11, family: 'Inter' } }
    }
  },
  interaction: {
    intersect: false,
    mode: 'index',
  },
});

onMounted(loadData);
</script>
