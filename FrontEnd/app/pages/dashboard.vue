<template>
  <div v-if="loading" class="skeleton">Yükleniyor…</div>
  <div v-else class="dashboard-layout">
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon"><i class="fa-solid fa-users"></i></div>
        <div class="stat-info">
          <p class="stat-label">Toplam kullanıcı</p>
          <p class="stat-value">{{ dashboardData.totalUsers }}</p>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon" style="background: rgba(59, 130, 246, 0.15); color: #3b82f6;"><i class="fa-solid fa-shield-halved"></i></div>
        <div class="stat-info">
          <p class="stat-label">Toplam rol</p>
          <p class="stat-value">{{ dashboardData.totalRoles }}</p>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon" style="background: rgba(168, 85, 247, 0.15); color: #a855f7;"><i class="fa-solid fa-user-plus"></i></div>
        <div class="stat-info">
          <p class="stat-label">Son 30 günde kayıt olan</p>
          <p class="stat-value">{{ dashboardData.usersLast30Days }}</p>
        </div>
      </div>
    </div>

    <div class="table-card chart-card dark-chart-card">
      <p class="page-title " style="font-size: 18px; margin-bottom: 4px;">Kullanıcı artış / azalış (son 6 ay)</p>
      <p class="page-subtitle dark-text-sub" style="margin-bottom: 20px;">Aylık bazda sisteme kayıt olan yeni kullanıcı net sayısı</p>
      <div class="chart-container">
        <LineChart v-if="chartData.labels.length" :data="chartData" :options="chartOptions" />
      </div>
    </div>

    <div class="two-col">
      <div class="table-card recent-card">
        <p class="page-title" style="font-size: 18px; margin-bottom: 4px;">Son kayıt olan kullanıcılar</p>
        <p class="page-subtitle">Sisteme en son katılan 5 kullanıcı</p>
        <ul class="recent-list">
          <li v-for="(u, index) in dashboardData.recentUsers" :key="index" class="recent-item">
            <span class="avatar-sm" :style="{ background: getRandomGradient(index), color: '#fff' }">
              {{ (u.username || 'U').slice(0, 2).toUpperCase() }}
            </span>
            <div>
              <p class="recent-name">{{ u.username }}</p>
              <p class="recent-sub">{{ new Date(u.createdAt).toLocaleDateString('tr-TR') }}</p>
            </div>
          </li>
        </ul>
      </div>

      <div class="table-card recent-card" v-if="canManage">
        <p class="page-title" style="font-size: 18px; margin-bottom: 4px;">Hızlı işlemler</p>
        <p class="page-subtitle">Sık kullanılan kısayollar</p>
        <div class="quick-actions">
          <NuxtLink to="/dashboardUserList" class="btn-secondary quick-btn">
            <i class="fa-solid fa-user-plus" style="margin-right: 8px;"></i> Kullanıcı ekle
          </NuxtLink>
          <NuxtLink to="/dashboardRoleList" class="btn-secondary quick-btn">
            <i class="fa-solid fa-shield-halved" style="margin-right: 8px;"></i> Rol oluştur
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { Line as LineChart } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, LineElement, PointElement, CategoryScale, LinearScale, Filler } from 'chart.js'

definePageMeta({ layout: 'dashboard', title: 'Anasayfa' });
ChartJS.register(Title, Tooltip, Legend, LineElement, PointElement, CategoryScale, LinearScale, Filler)

const authStore = useAuthStore();
const api = useApi();
const loading = ref(true);

const canManage = computed(
  () => !!authStore.currentUser?.permissions?.some(p => p.startsWith("Users.") || p.startsWith("Roles."))
);

const dashboardData = ref({
  totalUsers: 0,
  totalRoles: 0,
  usersLast30Days: 0,
  recentUsers: []
});

const chartData = ref({
  labels: [],
  datasets: [
    {
      label: 'Yeni Kullanıcılar',
      borderColor: '#3b82f6',
      backgroundColor: 'rgba(59, 130, 246, 0.15)',
      pointBackgroundColor: '#3b82f6',
      pointBorderColor: '#fff',
      pointBorderWidth: 2,
      pointRadius: 4,
      pointHoverRadius: 6,
      fill: true,
      tension: 0.4,
      data: []
    }
  ]
});

const fetchDashboardStats = async () => {
   try {
    const response = await api('/api/Dashboard/stats', {
      method: 'GET',
      headers: { Authorization: `Bearer ${authStore.token}` }
    });
    if (response) {
      dashboardData.value = {
        totalUsers: response.totalUsers,
        totalRoles: response.totalRoles,
        usersLast30Days: response.usersLast30Days,
        recentUsers: response.recentUsers
      };
      chartData.value.labels = response.chartLabels;
      chartData.value.datasets[0].data = response.chartValues;
    }
  } catch (error) {
    console.error("Dashboard verileri çekilirken hata oluştu:", error);
  }
};

const chartOptions = ref({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: {
      backgroundColor: '#1e293b',
      titleFont: { size: 14, family: 'Inter' },
      bodyFont: { size: 14, family: 'Inter' },
      padding: 12,
      cornerRadius: 8,
      displayColors: false
    }
  },
  scales: {
    y: {
      beginAtZero: false,
      grid: { color: 'rgba(255, 255, 255, 0.05)', drawBorder: false },
      border: { display: false },
      ticks: { font: { family: 'Inter', size: 12 }, color: '#94a3b8', padding: 10 }
    },
    x: {
      grid: { display: false, drawBorder: false },
      border: { display: false },
      ticks: { font: { family: 'Inter', size: 12 }, color: '#94a3b8', padding: 10 }
    }
  }
});

const getRandomGradient = (index) => {
  const gradients = [
    'linear-gradient(135deg, #f59e0b 0%, #d97706 100%)',
    'linear-gradient(135deg, #3b82f6 0%, #2563eb 100%)',
    'linear-gradient(135deg, #ec4899 0%, #db2777 100%)',
    'linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%)',
    'linear-gradient(135deg, #10b981 0%, #059669 100%)'
  ];
  return gradients[index % gradients.length];
};

onMounted(async () => {
  await fetchDashboardStats();
  loading.value = false;
});
</script>
