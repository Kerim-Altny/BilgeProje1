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

// sahteveri --> back end gelince silinecek
const mockLogin = async (data) => {
  await new Promise(resolve => setTimeout(resolve, 800));
  
  // Test // Burdada e mail varmış gibi checkliyor
  if (data.email === 'test@test.com' && data.password === '123456') {
    return { success: true, token: 'mock-token-abc123' };
  }
  
  return { success: false, errorMessage: 'E-posta veya şifre hatalı.' };
};

const handleLogin = async () => {
  isLoading.value = true;
  errorMessage.value = '';

  try {
    //back end gelince açılacak ve mocklogin silinecek

    // const response = await $fetch('http://localhost:5000/api/login', {
    //   method: 'POST',
    //   body: { email: email.value, password: password.value }
    // });

    const response = await mockLogin({
      email: email.value,
      password: password.value
    });

    if (response.success && response.token) {
      localStorage.setItem('token', response.token);
      await navigateTo('/');
    } else {
      errorMessage.value = response.errorMessage || 'Giriş başarısız.';
    }

  } catch (error) {
    errorMessage.value = 'Bağlantı hatası. İnternet bağlantınızı kontrol edin.';
  } finally {
    isLoading.value = false;
  }
}
</script>