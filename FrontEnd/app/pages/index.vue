<template>
  <v-container class="d-flex align-center justify-center">
    <v-card width="400" elevation="2" class="pa-6 rounded-lg bg-white">
      
      <!-- PORTAL LOGO / BAŞLIK ALANI -->
      <div class="text-center mb-6">
        <v-avatar color="primary-lighten-4" size="48" class="mb-2">
          <v-icon color="primary" icon="mdi-shield-key-outline" size="28"></v-icon>
        </v-avatar>
        <h2 class="text-h5 font-weight-bold text-grey-darken-3">
          {{ isLogin ? 'Bilge Teknoloji Giriş' : 'Yeni Hesap Oluştur' }}
        </h2>
        <p class="text-caption text-grey-medium-dark mt-1">
          {{ isLogin ? 'Lütfen şirket hesabı bilgilerinizle oturum açın.' : 'Formu eksiksiz doldurun.' }}
        </p>
      </div>

      <!-- SEKMELİ GEÇİŞ PENCERESİ -->
      <v-window v-model="formWindow">
        
        <v-window-item value="login">
          <AuthLoginForm />
        </v-window-item>

        <v-window-item value="register">
          <AuthRegisterForm />
        </v-window-item>

      </v-window>

      <!-- ALT GEÇİŞ LİNKİ -->
      <div class="text-center mt-6">
        <span class="text-body-2 text-grey-darken-1">
          {{ isLogin ? 'Şirketinizde yeni misiniz?' : 'Zaten bir hesabınız var mı?' }}
        </span>
        <a href="#" @click.prevent="toggleForm" class="text-body-2 font-weight-bold text-primary ml-1 text-decoration-none">
          {{ isLogin ? 'Kayıt Olun' : 'Giriş Yapın' }}
        </a>
      </div>

    </v-card>
  </v-container>
</template>

<script setup>
import { ref, computed } from 'vue'

// Bu sayfanın layouts/auth.vue şablonunu kullanmasını sağlar
definePageMeta({
  layout: 'auth'
})

const isLogin = ref(true)
const formWindow = computed(() => isLogin.value ? 'login' : 'register')

const toggleForm = () => {
  isLogin.value = !isLogin.value
}
</script>

<style scoped>
.text-grey-medium-dark {
  color: #616161 !important;
}
</style>