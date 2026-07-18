<template>
  <div class="form-kutusu">
    <h2>Giriş Yap</h2>

    <form @submit.prevent="handleLogin">
      <input v-model="email" type="email" placeholder="E-posta" required />
      <input v-model="password" type="password" placeholder="Şifre" required />

      <div class="remember-me-container">
        <label class="remember-me-label">
          <input
            type="checkbox"
            v-model="rememberMe"
            class="remember-me-checkbox"
          />
          Beni Hatırla
        </label>
      </div>

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
import Swal from "sweetalert2";

const authService = useAuthService();
const authStore = useAuthStore();

const email = ref("");
const password = ref("");
const rememberMe = ref(false);
const isLoading = ref(false);
const errorMessage = ref("");
const successMessage = ref("");

const handleLogin = async () => {
  isLoading.value = true;
  errorMessage.value = "";
  successMessage.value = "";

  try {
    const response = await authService.login(
      { email: email.value, password: password.value },
      rememberMe.value,
    );

    if (response.success && response.token) {
      localStorage.setItem("role", response.role);

      try {
        const currentUser = await authService.getMe();

        if (!currentUser?.permissions?.includes("Dashboard.Access")) {
          authStore.clearAuth();
          localStorage.removeItem("role");
          await Swal.fire({
            icon: "error",
            title: "Erişim Engellendi",
            text: "Bu panele erişim yetkiniz yok!",
            confirmButtonText: "Tamam",
            confirmButtonColor: "#3085d6",
          });
          isLoading.value = false;
          return;
        }
      } catch (err) {
        authStore.clearAuth();
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
    const data = error.data || error.response?._data;
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
