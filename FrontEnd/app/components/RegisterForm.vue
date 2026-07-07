<template>
  <div class="form-kutusu">
    <h2>Kayıt Ol</h2>
    
    <form @submit.prevent="handleRegister">
      <input v-model="fullName" type="text" placeholder="Ad Soyad" required />
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

const fullName = ref('');
const email = ref('');
const password = ref('');
const isLoading = ref(false);
const errorMessage = ref('');
const successMessage = ref('');

// sahteveri --> back end gelince silinecek
const mockRegister = async (data) => {
  await new Promise(resolve => setTimeout(resolve, 800)); 
  
  // Test // Burdada e mail varmış gibi checkliyor
  if (data.email === 'test@test.com') {
    return { success: false, errorMessage: 'Bu e-posta adresi zaten kullanımda.' };
  }
  
  return { success: true };
};

const handleRegister = async () => {
  isLoading.value = true;
  errorMessage.value = '';
  successMessage.value = '';

  try {
    //back end gelince açılacak ve mockregister silinecek

    // const response = await $fetch('http://localhost:5000/api/register', {
    //   method: 'POST',
    //   body: { fullName: fullName.value, email: email.value, password: password.value }
    // });
    
    const response = await mockRegister({
      fullName: fullName.value,
      email: email.value,
      password: password.value
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
    errorMessage.value = 'Bağlantı hatası. İnternet bağlantınızı kontrol edin.';
  } finally {
    isLoading.value = false;
  }
}
</script>