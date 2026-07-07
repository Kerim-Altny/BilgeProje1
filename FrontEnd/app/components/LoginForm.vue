<template>
  <div class="form-kutusu">
    <h2>Giriş Yap</h2>
    
    <form @submit.prevent="handleLogin">
      <input v-model="email" type="email" placeholder="E-posta" required />
      <input v-model="password" type="password" placeholder="Şifre" required />
      
      <button type="submit" :disabled="isLoading">
        {{ isLoading ? 'Bekleniyor...' : 'Giriş Yap' }}
      </button>
    </form>
    
    <p v-if="errorMessage" class="hata-mesaji" style="color: red; margin-top: 10px;">
      {{ errorMessage }}
    </p>

    <p class="alt-yazi">
      Hesabın Yok mu?
      <NuxtLink to="/register">Kayıt Ol</NuxtLink>
    </p>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const email = ref('');
const password = ref('');

const isLoading = ref(false);
const errorMessage = ref(''); 

const handleLogin = async () => {
  isLoading.value = true;
  errorMessage.value = '';

  try {
    const response = await $fetch('http://localhost:5000/api/login', {
      method: 'POST',
      body: {
        email: email.value,
        password: password.value
      }
    });

    console.log("Giriş Başarılı, Backend'den gelen yanıt:", response);

  } catch (error) {
    console.error("Giriş başarısız:", error);
    errorMessage.value = 'E-posta veya şifre hatalı. Lütfen tekrar deneyin.';
  } finally {
    isLoading.value = false;
  }
}
</script>