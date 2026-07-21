export default defineNuxtRouteMiddleware((to) => {
  const token = useCookie('token').value;

  if (to.meta.guestOnly) {
    if (token) {
      // Rolüne göre doğru panele yönlendir
      const role = import.meta.client ? localStorage.getItem('role') : null;
      if (role === 'Admin') return navigateTo('/dashboard');
      return navigateTo('/user');
    }
    return;
  }

  if (to.meta.public) return;

  if (!token) return navigateTo('/');

  // Token var ama client tarafındayız → sessionStorage kontrolü yap.
  // "Beni Hatırla" seçilmeden giriş yapıldıysa sessionStorage'da bayrak vardır.
  // Yeni pencerede / oturumda sessionStorage boş gelir → cookie'yi temizle, çıkış yaptır.
  if (import.meta.client) {
    const sessionActive = sessionStorage.getItem('session_active');
    const tokenCookie = useCookie('token');

    // cookie'nin kendisi kalıcı mı (max-age var mı) yoksa session cookie mi?
    // Bunu anlamamızın yolu: eğer cookie var ama sessionStorage bayrağı YOK ve
    // cookie manuel max-age ile set edilmemişse → bu "oturumsuz" eski bir cookie.
    // Basit yaklaşım: kalıcı cookie'ler session_active bayrağını KALDIRARAK set edilir.
    // Eğer session_active YOK ve token VAR → ya "Beni Hatırla" ile girilmiş (kalıcı, geçerli)
    //                                          ya da tarayıcı eski oturumu geri yüklemiş (geçersiz).
    // Bunu ayırt etmek için localStorage'da 'remember_me' bayrağını kullanıyoruz.
    const rememberMe = localStorage.getItem('remember_me');

    if (!rememberMe && !sessionActive) {
      // Beni Hatırla seçilmemiş + sessionStorage bayrağı yok = yeni pencere/oturum
      // Cookie'yi temizle ve login'e yönlendir
      document.cookie = `token=; path=/; max-age=0`;
      document.cookie = `refreshToken=; path=/; max-age=0`;
      tokenCookie.value = null;
      return navigateTo('/');
    }
  }
});
