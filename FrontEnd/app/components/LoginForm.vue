<template>
  <div class="form-kutusu">
    <h2>Giriş Yap</h2>
    
    <form @submit.prevent="handleLogin">
      
      <input v-model="email" type="email" placeholder="E-posta" required />
      <input v-model="password" type="password" placeholder="Şifre" required />
      
      <button type="submit">Giriş Yap</button>
    </form>
    
    <p class="alt-yazi">
      Hesabın Yok mu?
      <NuxtLink to="/register">Kayıt Ol</NuxtLink>
    </p>
  </div>
</template>

<script setup>
const handleLogin = () => {
  console.log("E-posta:", email.value);
  console.log("Şifre:", password.value);
}
</script>