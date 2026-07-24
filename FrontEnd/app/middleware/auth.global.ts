export default defineNuxtRouteMiddleware(async (to) => {
  if (to.meta.public) return;

  const authStore = useAuthStore();

  if (import.meta.client && !authStore.currentUser) {
    const wantsRemember = localStorage.getItem('remember_me') === '1';
    const isSessionActive = sessionStorage.getItem('session_active') === '1';
    const authActive = localStorage.getItem('auth_active') === '1';

    if (authActive && !wantsRemember && !isSessionActive) {
      await authStore.clearAuth();
    }
  }

  // Store'da kullanıcı yoksa (ilk yükleme/sayfa yenileme) cookie'nin hâlâ geçerli olup
  // olmadığını sunucuya sorarak öğreniyoruz.
  if (!authStore.currentUser) {
    try {
      await useAuthService().getMe();
      if (import.meta.client && !localStorage.getItem('remember_me')) {
        sessionStorage.setItem('session_active', '1');
      }
    } catch {
      authStore.setUser(null);
    }
  }

  const isLoggedIn = !!authStore.currentUser;

  if (to.meta.guestOnly) {
    if (isLoggedIn) {
      // Rolüne göre doğru panele yönlendir
      const perms: string[] = authStore.currentUser?.permissions ?? [];
      const isAdmin = perms.includes('Dashboard.Access') || perms.includes('Users.View') || perms.includes('Roles.View');
      return navigateTo(isAdmin ? '/dashboard' : '/user');
    }
    return;
  }

  if (!isLoggedIn) return navigateTo('/');
});
