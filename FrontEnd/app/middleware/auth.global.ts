export default defineNuxtRouteMiddleware((to) => {
  // Artık localStorage yerine token'ı Cookie'den alıyoruz (SSR uyumlu)
  const token = useCookie('token').value;

  if (to.meta.guestOnly) {
    if (token) return navigateTo('/dashboard');
    return;
  }
  if (to.meta.public) return;
  if (!token) return navigateTo('/');
});
