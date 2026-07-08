<template>
  <div class="form-kutusu">
    <h2>Kayıt Ol</h2>
    
    <form @submit.prevent="handleRegister">
      <input v-model="username" type="text" placeholder="Kullanıcı Adı" required />
      <input v-model="email" type="email" placeholder="E-posta" required />
      <input v-model="password" type="password" placeholder="Şifre" required />
      
      <button type="submit" :disabled="isLoading">
        {{ isLoading ? 'Bekleniyor...' : 'Hesap Oluştur' }}
      </button>
    </form>

    <p v-if="errorMessage" style="color: red; margin-top: 10px;">
      {{ errorMessage }}
    </p>

    <p v-if="successMessage" style="color: green; margin-top: 10px;">
      {{ successMessage }}
    </p>
    
    <p class="alt-yazi">
      Zaten bir hesabın var mı? 
      <NuxtLink to="/">Giriş Yap</NuxtLink>
    </p>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const username = ref('');
const email = ref('');
const password = ref('');
const isLoading = ref(false);
const errorMessage = ref('');
const successMessage = ref('');


const handleRegister = async () => {
  isLoading.value = true;
  errorMessage.value = '';
  successMessage.value = '';

  try {

     const response = await $fetch('http://localhost:5163/api/auth/register', {
       method: 'POST',
       body: { username: username.value, email: email.value, password: password.value }
     });


    if (response.success) {
      successMessage.value = 'Hesabın oluşturuldu! Giriş sayfasına yönlendiriliyorsun...';
      setTimeout(async () => {
        await navigateTo('/');
      }, 1500);
    } else {
      errorMessage.value = response.errorMessage || 'Kayıt başarısız.';
    }

  } catch (error) {
    const data = error.data;
    if (data?.errorMessage) {
      errorMessage.value = data.errorMessage;
    } else if (data?.errors) {
      errorMessage.value = Object.values(data.errors).flat()[0];
    } else {
      errorMessage.value = 'Bağlantı hatası. İnternet bağlantınızı kontrol edin.';
    }
  } finally {
    isLoading.value = false;
  }
}
</script>