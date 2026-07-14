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
const api = useApi();
import { ref } from "vue";
import Swal from 'sweetalert2';

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

    const response = await api("/api/auth/login", {
      method: "POST",
      body: { email: email.value, password: password.value },
    });

    if (response.success && response.token) {
      localStorage.setItem("token", response.token);
      localStorage.setItem("role", response.role);
      
      try {
        const currentUser = await api("/api/auth/me", {
          headers: { Authorization: `Bearer ${response.token}` },
        });

        if (!currentUser?.permissions?.includes("Dashboard.Access")) {
          localStorage.removeItem("token");
          localStorage.removeItem("role");
          await Swal.fire({
            icon: 'error',
            title: 'Erişim Engellendi',
            text: 'Bu panele erişim yetkiniz yok!',
            confirmButtonText: 'Tamam',
            confirmButtonColor: '#3085d6'
          });
          isLoading.value = false;
          return;
        }
      } catch (err) {
        localStorage.removeItem("token");
        localStorage.removeItem("role");
        errorMessage.value = "Yetki kontrolü sırasında bir hata oluştu.";
        isLoading.value = false;
        return;
      }

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
