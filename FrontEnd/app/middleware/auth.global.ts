
export default defineNuxtRouteMiddleware((to) => {
  if (import.meta.server) return;

  const token = localStorage.getItem('token');

  if (to.meta.guestOnly) {
    if (token) return navigateTo('/dashboard');
    return;
  }
  if (to.meta.public) return;
  if (!token) return navigateTo('/');
});
