<template>
  <div class="redirect-page">
    <div class="redirect-card">
      <div v-if="status === 'loading'">
        <i class="fa-solid fa-spinner fa-spin redirect-icon" style="color: var(--accent);"></i>
        <p class="redirect-title">Yönlendiriliyorsunuz...</p>
        <p class="redirect-sub">Lütfen bekleyin</p>
      </div>
      <div v-else-if="status === 'notfound'">
        <i class="fa-solid fa-circle-xmark redirect-icon" style="color: #ef4444;"></i>
        <p class="redirect-title">Link Bulunamadı</p>
        <p class="redirect-sub">Bu kısa link geçersiz veya silinmiş olabilir.</p>
        <NuxtLink to="/" class="btn-primary" style="text-decoration: none; display: inline-block; margin-top: 20px; padding: 12px 28px; border-radius: 12px;">
          Ana Sayfaya Dön
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';

definePageMeta({ layout: false });

const route = useRoute();
const status = ref('loading');

onMounted(async () => {
  const code = route.params.code;
  try {
    // Backend'e istek at, originalUrl'i al, yönlendir
    // const data = await $fetch(`/api/urls/resolve/${code}`, { method: 'GET' });
    // window.location.href = data.originalUrl;

    // MOCK — backend hazır olunca yukarıdaki satırları aç
    console.log('Redirect kodu:', code);
    status.value = 'notfound'; // mock'ta bulunamadı göster
  } catch (e) {
    status.value = 'notfound';
  }
});
</script>
