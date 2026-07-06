<template>
  <div class="form-kutusu">
    <h2>Kayıt Ol</h2>
    <form @submit.prevent="kayitIsleminiBaslat">
      <input type="text" placeholder="Ad Soyad" required />
      <input type="email" placeholder="E-posta" required />
      <input type="password" placeholder="Şifre" required />
      <button type="submit">Hesap Oluştur</button>
    </form>
    
    <p class="alt-yazi">
      Zaten bir hesabın var mı? 
      <NuxtLink to="/">Giriş Yap</NuxtLink>
    </p>
  </div>
</template>

<script setup>
const kayitIsleminiBaslat = () => {
  console.log("Kayıt ol butonuna basıldı!");
}
</script>

