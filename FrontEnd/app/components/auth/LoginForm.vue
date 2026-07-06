<template>
  <v-form @submit.prevent="submitLogin">
    <v-text-field
      v-model="loginData.username"
      label="Kullanıcı Adı"
      variant="outlined"
      density="comfortable"
      :error-messages="loginErrors.username"
      prepend-inner-icon="mdi-account-outline"
      color="primary"
      class="mb-2"
    ></v-text-field>

    <v-text-field
      v-model="loginData.password"
      label="Şifre"
      type="password"
      variant="outlined"
      density="comfortable"
      :error-messages="loginErrors.password"
      prepend-inner-icon="mdi-lock-outline"
      color="primary"
      class="mb-4"
    ></v-text-field>

    <v-btn type="submit" color="primary" size="large" block class="text-none rounded-md font-weight-medium" elevation="0">
      Oturum Aç
    </v-btn>
  </v-form>
</template>

<script setup>
import { reactive } from 'vue'
import * as yup from 'yup'
import { useAuthStore } from '~/stores/auth'

const authStore = useAuthStore()
const loginData = reactive({ username: '', password: '' })
const loginErrors = reactive({ username: '', password: '' })

const loginSchema = yup.object({
  username: yup.string().required('Kullanıcı adı girilmesi zorunludur.'),
  password: yup.string().required('Şifre girilmesi zorunludur.')
})

const submitLogin = async () => {
  loginErrors.username = ''
  loginErrors.password = ''

  try {
    await loginSchema.validate(loginData, { abortEarly: false })
    await authStore.login(loginData)
    alert("Giriş işlemi başarılı");
  } catch (err) {
    if (err.inner) err.inner.forEach(error => { loginErrors[error.path] = error.message })
  }
}
</script>