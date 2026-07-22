<template>
  <div class="dashboard-layout">
    <div class="create-link-wrapper">
      <!-- Sol: Form -->
      <div class="table-card table-card-padded create-form-card">
        <p class="page-title" style="font-size: 22px; margin-bottom: 6px;">Yeni Kısa Link Oluştur</p>
        <p class="page-subtitle" style="margin-bottom: 28px;">Uzun URL'inizi yapıştırın, anında kısa link alın.</p>

        <form @submit.prevent="handleCreate">
          <div class="create-field">
            <label class="create-label">Uzun URL <span style="color: var(--danger);">*</span></label>
            <div class="create-input-wrapper">
              <i class="fa-solid fa-globe create-input-icon"></i>
              <input
                v-model="form.originalUrl"
                type="url"
                class="create-input"
                placeholder="https://ornek.com/cok/uzun/bir/adres"
                required
              />
            </div>
          </div>

          <div class="create-field">
            <label class="create-label">
              Özel Kısa Kod
              <span class="create-label-badge">Opsiyonel</span>
            </label>
            <div class="create-input-wrapper">
              <span class="create-input-prefix">/r/</span>
              <input
                v-model="form.customCode"
                type="text"
                class="create-input create-input-prefixed"
                placeholder="ornekkod"
                maxlength="20"
                pattern="[a-zA-Z0-9\-_]+"
                title="Sadece harf, rakam, tire ve alt çizgi kullanabilirsiniz"
              />
            </div>
            <p class="create-hint">Boş bırakırsanız otomatik oluşturulur (Snowflake ID + Base62)</p>
          </div>

          <button type="submit" class="create-submit-btn" :disabled="isLoading">
            <span v-if="isLoading">
              <i class="fa-solid fa-spinner fa-spin" style="margin-right: 8px;"></i> Oluşturuluyor...
            </span>
            <span v-else>
              <i class="fa-solid fa-wand-magic-sparkles" style="margin-right: 8px;"></i> Kısa Link Oluştur
            </span>
          </button>

          <p v-if="errorMsg" class="create-error">
            <i class="fa-solid fa-circle-exclamation" style="margin-right: 6px;"></i>{{ errorMsg }}
          </p>
        </form>
      </div>

      <!-- Sağ: Sonuç -->
      <div class="table-card table-card-padded create-result-card">
        <!-- Boş hali -->
        <div v-if="!result" class="create-result-empty">
          <div class="create-result-icon-wrap">
            <i class="fa-solid fa-link"></i>
          </div>
          <p style="font-size: 16px; font-weight: 600; color: var(--ink); margin-bottom: 8px;">Kısa linkiniz burada görünecek</p>
          <p style="font-size: 14px; color: var(--ink-soft);">Formu doldurun ve "Oluştur" butonuna tıklayın.</p>
        </div>

        <!-- Sonuç var -->
        <div v-else class="create-result-success">
          <div class="result-checkmark">
            <i class="fa-solid fa-check"></i>
          </div>
          <p style="font-size: 16px; font-weight: 700; color: var(--ink); margin-bottom: 6px;">Link Oluşturuldu!</p>
          <p style="font-size: 13px; color: var(--ink-soft); margin-bottom: 24px;">Aşağıdaki linki paylaşmaya başlayabilirsiniz.</p>

          <div class="result-url-box">
            <span class="result-url-text">{{ shortBase }}/r/{{ result.shortCode }}</span>
            <button class="copy-btn result-copy-btn" @click="copyResult">
              <i class="fa-regular fa-copy"></i>
            </button>
          </div>

          <p v-if="copied" class="copy-success-text">
            <i class="fa-solid fa-check" style="margin-right: 4px;"></i> Kopyalandı!
          </p>

          <div class="result-meta">
            <div class="result-meta-item">
              <span class="result-meta-label">Hedef URL</span>
              <a :href="result.originalUrl" target="_blank" class="result-meta-value long-url-link">
                {{ truncate(result.originalUrl, 50) }}
              </a>
            </div>
            <div class="result-meta-item">
              <span class="result-meta-label">Kısa Kod</span>
              <span class="result-meta-value" style="font-family: monospace; font-size: 15px;">{{ result.shortCode }}</span>
            </div>
          </div>

          <div style="display: flex; gap: 10px; margin-top: 20px;">
            <button class="btn-secondary" style="flex:1; padding: 12px;" @click="resetForm">
              <i class="fa-solid fa-plus" style="margin-right: 8px;"></i> Yeni Link Oluştur
            </button>
            <button class="btn-primary" style="flex:1; padding: 12px;" @click="goToLinks">
              <i class="fa-solid fa-list" style="margin-right: 8px;"></i> Linklerimi Gör
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Açıklama kutusu -->
    <div class="table-card create-info-box">
      <div class="info-box-grid">
        <div class="info-item">
          <div class="info-icon" style="background: rgba(0,220,130,0.12); color: var(--accent);">
            <i class="fa-solid fa-snowflake"></i>
          </div>
          <div>
            <p class="info-title">Snowflake ID</p>
            <p class="info-desc">Her link için zamana dayalı, benzersiz bir ID üretilir. Milyonlarca link olsa bile asla çakışmaz.</p>
          </div>
        </div>
        <div class="info-item">
          <div class="info-icon" style="background: rgba(59,130,246,0.12); color: #3b82f6;">
            <i class="fa-solid fa-code"></i>
          </div>
          <div>
            <p class="info-title">Base62 Encoding</p>
            <p class="info-desc">Büyük ID sayısı, harf+rakamdan oluşan 6 karakterlik bir koda dönüştürülür. Örn: <code>928374...</code> → <code>aB3xK2</code></p>
          </div>
        </div>
        <div class="info-item">
          <div class="info-icon" style="background: rgba(168,85,247,0.12); color: #a855f7;">
            <i class="fa-solid fa-bolt"></i>
          </div>
          <div>
            <p class="info-title">Anında Yönlendirme</p>
            <p class="info-desc">Kısa link açıldığında sunucu kodu çözer ve kullanıcıyı anında hedef adrese yönlendirir.</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

definePageMeta({ layout: 'dashboard', title: 'Link Oluştur' });

const api = useApi();

const form = ref({ originalUrl: '', customCode: '' });
const result = ref<any>(null);
const isLoading = ref(false);
const errorMsg = ref('');
const copied = ref(false);
const shortBase = ref(typeof window !== 'undefined' ? window.location.origin : '');

const handleCreate = async () => {
  isLoading.value = true;
  errorMsg.value = '';
  result.value = null;

  try {
    const data = await api<any>('/api/links/my', {
      method: 'POST',
      body: {
        originalUrl: form.value.originalUrl,
        ...(form.value.customCode ? { customCode: form.value.customCode } : {})
      }
    });
    result.value = {
      shortCode: data.shortCode,
      originalUrl: data.originalUrl,
    };
  } catch (e: any) {
    const data = e?.data ?? e?.response?._data;
    errorMsg.value = data?.message ?? 'Bir hata oluştu. Lütfen tekrar deneyin.';
  } finally {
    isLoading.value = false;
  }
};

const copyResult = async () => {
  const url = `${shortBase.value}/r/${result.value.shortCode}`;
  await navigator.clipboard.writeText(url);
  copied.value = true;
  setTimeout(() => { copied.value = false; }, 2000);
};

const resetForm = () => {
  form.value = { originalUrl: '', customCode: '' };
  result.value = null;
  errorMsg.value = '';
};

const goToLinks = () => navigateTo('/user/links');

const truncate = (str: string, len: number) => str?.length > len ? str.slice(0, len) + '…' : str;
</script>
