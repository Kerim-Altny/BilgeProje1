<template>
  <v-form @submit.prevent="submitRegister">
    <v-text-field
      v-model="registerData.email"
      label="E-posta Adresi"
      variant="outlined"
      density="comfortable"
      :error-messages="registerErrors.email"
      prepend-inner-icon="mdi-email-outline"
      color="primary"
      class="mb-2"
    ></v-text-field>

    <v-text-field
      v-model="registerData.username"
      label="Kullanıcı Adı"
      variant="outlined"
      density="comfortable"
      :error-messages="registerErrors.username"
      prepend-inner-icon="mdi-account-outline"
      color="primary"
      class="mb-2"
    ></v-text-field>

    <v-text-field
      v-model="registerData.password"
      label="Şifre"
      type="password"
      variant="outlined"
      density="comfortable"
      :error-messages="registerErrors.password"
      prepend-inner-icon="mdi-lock-outline"
      color="primary"
      class="mb-2"
    ></v-text-field>

    <v-text-field
      v-model="registerData.passwordConfirm"
      label="Şifre Tekrarı"
      type="password"
      variant="outlined"
      density="comfortable"
      :error-messages="registerErrors.passwordConfirm"
      prepend-inner-icon="mdi-lock-check-outline"
      color="primary"
      class="mb-4"
    ></v-text-field>

    <v-btn type="submit" color="primary" size="large" block class="text-none rounded-md font-weight-medium" elevation="0">
      Hesabı Oluştur
    </v-btn>
  </v-form>
</template>

<script setup>
import { reactive } from 'vue'
import * as yup from 'yup'
import { useAuthStore } from '~/stores/auth'

const authStore = useAuthStore()
const registerData = reactive({ email: '', username: '', password: '', passwordConfirm: '' })
const registerErrors = reactive({ email: '', username: '', password: '', passwordConfirm: '' })

const registerSchema = yup.object({
  email: yup.string().email('Geçerli bir e-posta giriniz.').required('E-posta alanı zorunludur.'),
  username: yup.string().min(4, 'Kullanıcı adı en az 4 karakter olmalıdır.').required('Kullanıcı adı zorunludur.'),
  password: yup.string().min(6, 'Şifre en az 6 karakter olmalıdır.').required('Şifre zorunludur.'),
  passwordConfirm: yup.string()
    .oneOf([yup.ref('password'), null], 'Girdiğiniz şifreler eşleşmiyor.')
    .required('Şifre onayı zorunludur.')
})

const submitRegister = async () => {
  Object.keys(registerErrors).forEach(key => registerErrors[key] = '')

  try {
    await registerSchema.validate(registerData, { abortEarly: false })
    await authStore.register(registerData)
    alert("Kayıt işlemi başarılı");
  } catch (err) {
    if (err.inner) err.inner.forEach(error => { registerErrors[error.path] = error.message })
  }
}
</script>