<template>
  <div class="form-kutusu">
    <h2>Giriş Yap</h2>

    <form @submit.prevent="handleLogin">
      <input v-model="email" type="email" placeholder="E-posta" required />
      <input v-model="password" type="password" placeholder="Şifre" required />

      <button type="submit" :disabled="isLoading">
        {{ isLoading ? "Bekleniyor..." : "Giriş Yap" }}
      </button>
    </form>

    <p
      v-if="errorMessage"
      class="hata-mesaji"
      style="color: red; margin-top: 10px"
    >
      {{ errorMessage }}
    </p>

    <p v-if="successMessage" style="color: green; margin-top: 10px">
      {{ successMessage }}
    </p>

    <p class="alt-yazi">
      Hesabın Yok mu?
      <NuxtLink to="/register">Kayıt Ol</NuxtLink>
    </p>
  </div>
</template>

<script setup>
import { ref } from "vue";

const email = ref("");
const password = ref("");
const isLoading = ref(false);
const errorMessage = ref("");
const successMessage = ref("");

const handleLogin = async () => {
  isLoading.value = true;
  errorMessage.value = "";
  successMessage.value = "";

  try {
    //back end gelince açılacak ve mocklogin silinecek

    const response = await $fetch("http://localhost:5163/api/auth/login", {
      method: "POST",
      body: { email: email.value, password: password.value },
    });

    if (response.success && response.token) {
      localStorage.setItem("token", response.token);
      localStorage.setItem("role", response.role);
      successMessage.value = "Giriş başarılı! Yönlendiriliyorsunuz...";
      setTimeout(async () => {
        await navigateTo("/dashboard");
      }, 1500);
    } else {
      errorMessage.value = response.errorMessage || "Giriş başarısız.";
    }
  } catch (error) {
    const data = error.data;
    if (data?.errorMessage) {
      errorMessage.value = data.errorMessage;
    } else if (data?.errors) {
      errorMessage.value = Object.values(data.errors).flat()[0];
    } else {
      errorMessage.value =
        "Bağlantı hatası. İnternet bağlantınızı kontrol edin.";
    }
  } finally {
    isLoading.value = false;
  }
};
</script>
