<template>
  <div class="redirect-page">
    <div class="redirect-card">
      <div v-if="status === 'loading'">
        <i class="fa-solid fa-spinner fa-spin redirect-icon" style="color: var(--accent);"></i>
        <p class="redirect-title">Yönlendiriliyorsunuz...</p>
        <p class="redirect-sub">Lütfen bekleyin</p>
      </div>
      <div v-else-if="status === 'expired'">
        <i class="fa-solid fa-clock redirect-icon" style="color: #f59e0b;"></i>
        <p class="redirect-title">Linkin Süresi Doldu</p>
        <p class="redirect-sub">Bu kısa linkin geçerlilik süresi sona ermiş.</p>
        <NuxtLink to="/" class="btn-primary" style="text-decoration: none; display: inline-block; margin-top: 20px; padding: 12px 28px; border-radius: 12px;">
          Ana Sayfaya Dön
        </NuxtLink>
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

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';

definePageMeta({ layout: false, public: true });

const route = useRoute();
const status = ref('loading');
const { apiBase } = useRuntimeConfig().public;

onMounted(async () => {
  const code = route.params.code as string;
  try {
    const data = await $fetch<{ originalUrl: string }>(`${apiBase}/api/links/resolve/${code}`, {
      method: 'GET'
    });
    // Tıklama sayıldı, yönlendir
    window.location.href = data.originalUrl;
  } catch (e: any) {
    const msg: string = e?.data?.message ?? '';
    if (msg.includes('süresi')) {
      status.value = 'expired';
    } else {
      status.value = 'notfound';
    }
  }
});
</script>
