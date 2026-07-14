<template>
    <div class="adminpage">
        <aside class="leftmenu">
            <div class="brand">
                <span class="brand-mark">â—</span>
                <span class="brand-name">Dashboard</span>
            </div>

            <nav class="nav">
                <span class="nav-label">Genel</span>
                <NuxtLink to="/dashboard" class="nav-item">
                    <i class="fa-solid fa-house nav-icon"></i>
                    <span>Anasayfa</span>
                </NuxtLink>
                <NuxtLink to="/dashboardUserList" class="nav-item">
                    <i class="fa-solid fa-users nav-icon"></i>
                    <span>Kullanıcılar</span>
                </NuxtLink>
                <NuxtLink to="/dashboardRoleList" class="nav-item active">
                    <i class="fa-solid fa-shield-halved nav-icon"></i>
                    <span>Roller</span>
                </NuxtLink>
            </nav>
        </aside>

        <div class="mainpage">
            <header class="mainnav">
                <div class="nav-left">
                    <h1 class="page-title">Roller</h1>
                </div>

                <div class="nav-right" v-if="!loading">
                    <div class="user-chip">
                        <span class="avatar">{{ initials }}</span>
                        <span class="greeting">Hoş geldin, <strong>{{ user?.username }}</strong></span>
                    </div>
                    <button class="logout-btn" @click="handleLogout">
                        Çıkış Yap
                        <i class="fa-solid fa-right-from-bracket"></i>
                    </button>
                </div>
            </header>

            <main class="content">
                <div v-if="loading" class="skeleton">Yükleniyor…</div>
                <div v-else class="content-inner">
                    <div class="page-wrap">
                        <div class="page-header">
                            <div>
                                <h1 class="page-title">Rol Yönetimi</h1>
                                <p class="page-subtitle">
                                    Toplam {{ totalRoles }} rol listeleniyor.
                                </p>
                            </div>

                            <div class="header-actions" v-if="canAdd || canDelete">
                                <button v-if="canDelete && selectedRoles.length > 0" @click="deleteSelectedRoles"
                                    class="btn-danger">
                                    Seçilenleri Sil ({{ selectedRoles.length }})
                                </button>

                                <NuxtLink v-if="canAdd" to="/dashboardRoleAdd" class="btn-primary">
                                    + Yeni Rol Ekle
                                </NuxtLink>
                            </div>
                        </div>

                        <div class="table-card">
                            <div class="table-scroll">
                                <table class="users-table">
                                    <thead>
                                        <tr>
                                            <th class="col-checkbox" v-if="canDelete">
                                                <input type="checkbox" :checked="isAllPageSelected"
                                                    @change="toggleSelectAllPage" class="checkbox" />
                                            </th>
                                            <th>Rol Adı</th>
                                            <th class="th-perm">Dashboard</th>
                                            <th class="th-perm">Kullanıcılar</th>
                                            <th class="th-perm">Roller</th>
                                            <th class="col-actions" v-if="canEdit || canDelete">İşlemler</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="r in paginatedRoles" :key="r.id"
                                            :class="{ 'row-selected': selectedRoles.includes(r.id) }">
                                            <td v-if="canDelete">
                                                <input type="checkbox" :value="r.id" v-model="selectedRoles"
                                                    class="checkbox" />
                                            </td>
                                            <td class="cell-name">{{ r.name }}</td>

                                            <!-- Dashboard -->
                                            <td class="td-perm">
                                                <div class="perm-icons">
                                                    <span v-for="pi in dashboardPerms" :key="pi.perm"
                                                        class="perm-icon-wrap"
                                                        :class="hasPerm(r, pi.perm) ? 'pi-active' : 'pi-inactive'"
                                                        :title="pi.label">
                                                        <i :class="pi.icon"></i>
                                                    </span>
                                                </div>
                                            </td>

                                            <!-- Kullanıcılar -->
                                            <td class="td-perm">
                                                <div class="perm-icons">
                                                    <span v-for="pi in userPerms" :key="pi.perm" class="perm-icon-wrap"
                                                        :class="hasPerm(r, pi.perm) ? 'pi-active' : 'pi-inactive'"
                                                        :title="pi.label">
                                                        <i :class="pi.icon"></i>
                                                    </span>
                                                </div>
                                            </td>

                                            <!-- Roller -->
                                            <td class="td-perm">
                                                <div class="perm-icons">
                                                    <span v-for="pi in rolePerms" :key="pi.perm" class="perm-icon-wrap"
                                                        :class="hasPerm(r, pi.perm) ? 'pi-active' : 'pi-inactive'"
                                                        :title="pi.label">
                                                        <i :class="pi.icon"></i>
                                                    </span>
                                                </div>
                                            </td>

                                            <td class="col-actions" v-if="canEdit || canDelete">
                                                <div class="row-actions">
                                                    <NuxtLink v-if="canEdit" :to="`/dashboardRoleControl?id=${r.id}`"
                                                        class="link-edit">
                                                        <i class="fa-solid fa-pencil"></i>
                                                    </NuxtLink>
                                                    <button v-if="canDelete" @click="deleteSingleRole(r.id)"
                                                        class="link-delete">
                                                        <i class="fa-solid fa-xmark"></i>
                                                    </button>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr v-if="paginatedRoles.length === 0">
                                            <td :colspan="visibleColumnCount" class="empty-row">
                                                Kayıtlı rol bulunamadı.
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>

                            <div class="table-footer">
                                <span class="range-text">Gösterilen Kayıtlar: {{ textRange }}</span>

                                <div class="pagination">
                                    <button @click="currentPage--" :disabled="currentPage === 1" class="page-btn">
                                        Önceki
                                    </button>

                                    <span class="page-indicator">Sayfa {{ currentPage }} / {{ totalPages }}</span>

                                    <button @click="currentPage++" :disabled="currentPage === totalPages"
                                        class="page-btn">
                                        Sonraki
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    </div>
</template>

<script setup>
const api = useApi();
import { ref, computed, onMounted } from "vue";
import Swal from 'sweetalert2';

const loading = ref(true);
const user = ref(null);

const initials = computed(() => {
    const name = user.value?.username ?? "";
    return name.slice(0, 2).toUpperCase();
});

const roles = ref([]);

const fetchRolesList = async (token) => {
    try {
        const response = await api("/api/roles", {
            headers: { Authorization: `Bearer ${token}` },
        });

        roles.value = response.map((r) => ({
            id: r.id,
            name: r.name,
            permissions: r.permissions || []
        }));
    } catch (error) {
        console.error("Roller çekilirken hata oluştu:", error);
    }
};

// Permission icon definitions
const dashboardPerms = [
    { perm: 'Dashboard.Access', label: 'Giriş', icon: 'fa-solid fa-door-open' },
];

const userPerms = [
    { perm: 'Users.View', label: 'Görme', icon: 'fa-solid fa-eye' },
    { perm: 'Users.Create', label: 'Oluşturma', icon: 'fa-solid fa-plus' },
    { perm: 'Users.Edit', label: 'Düzenle', icon: 'fa-solid fa-pen' },
    { perm: 'Users.Delete', label: 'Silme', icon: 'fa-solid fa-trash' },
];

const rolePerms = [
    { perm: 'Roles.View', label: 'Görme', icon: 'fa-solid fa-eye' },
    { perm: 'Roles.Create', label: 'Oluşturma', icon: 'fa-solid fa-plus' },
    { perm: 'Roles.Edit', label: 'Düzenle', icon: 'fa-solid fa-pen' },
    { perm: 'Roles.Delete', label: 'Silme', icon: 'fa-solid fa-trash' },
];

const hasPerm = (role, permName) => {
    return (role.permissions || []).includes(permName);
};

const permGroups = [
    { key: 'Dashboard', label: 'Dashboard' },
    { key: 'Users', label: 'Kullanıcılar' },
    { key: 'Roles', label: 'Roller' },
];

const hasGroupPerm = (role, groupKey) => {
    return (role.permissions || []).some(p => p.startsWith(groupKey + '.'));
};

const canAdd = computed(() => !!user.value?.permissions?.includes("Roles.Create"));
const canEdit = computed(() => !!user.value?.permissions?.includes("Roles.Edit"));
const canDelete = computed(() => !!user.value?.permissions?.includes("Roles.Delete"));


onMounted(async () => {
    const token = localStorage.getItem("token");

    try {
        const currentUser = await api("/api/auth/me", {
            headers: { Authorization: `Bearer ${token}` },
        });

        if (!currentUser?.permissions?.includes("Dashboard.Access")) {
            await Swal.fire({ icon: 'error', title: 'Erişim Engellendi', text: 'Bu panele erişim yetkiniz yok!' });
            localStorage.removeItem("token");
            await navigateTo("/");
            return;
        }

        if (!currentUser?.permissions?.includes("Roles.View") && !currentUser?.permissions?.includes("Roles.Create") && !currentUser?.permissions?.includes("Roles.Edit") && !currentUser?.permissions?.includes("Roles.Delete")) {
            await Swal.fire({ icon: 'error', title: 'Yetkisiz İşlem', text: 'Bu sayfaya erişim yetkiniz yok!' });
            await navigateTo("/dashboard");
            return;
        }

        user.value = currentUser;

        await fetchRolesList(token);
    } catch (error) {
        localStorage.removeItem("token");
        await navigateTo("/");
    } finally {
        loading.value = false;
    }
});

const handleLogout = async () => {
    const result = await Swal.fire({
        title: 'Çıkış yapmak istiyor musunuz?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Evet, çıkış yap',
        cancelButtonText: 'İptal'
    });
    if (result.isConfirmed) {
        localStorage.removeItem("token");
        await navigateTo("/");
    }
};

const currentPage = ref(1);
const itemsPerPage = 15;

const totalRoles = computed(() => roles.value.length);
const totalPages = computed(
    () => Math.ceil(totalRoles.value / itemsPerPage) || 1,
);

const paginatedRoles = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage;
    const end = start + itemsPerPage;
    return roles.value.slice(start, end);
});

const selectedRoles = ref([]);

const isAllPageSelected = computed(() => {
    if (paginatedRoles.value.length === 0) return false;
    return paginatedRoles.value.every((r) => selectedRoles.value.includes(r.id));
});

const toggleSelectAllPage = () => {
    const pageRoleIds = paginatedRoles.value.map((r) => r.id);
    if (isAllPageSelected.value) {
        selectedRoles.value = selectedRoles.value.filter(
            (id) => !pageRoleIds.includes(id),
        );
    } else {
        pageRoleIds.forEach((id) => {
            if (!selectedRoles.value.includes(id)) selectedRoles.value.push(id);
        });
    }
};

const textRange = computed(() => {
    if (totalRoles.value === 0) return "0 - 0";
    const start = (currentPage.value - 1) * itemsPerPage + 1;
    const end = Math.min(currentPage.value * itemsPerPage, totalRoles.value);
    return `${start} - ${end}`;
});

const deleteSingleRole = async (id) => {
    const result = await Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu rolü silmek istediğinize emin misiniz?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Evet, sil!',
        cancelButtonText: 'İptal'
    });

    if (result.isConfirmed) {
        const token = localStorage.getItem("token");
        try {
            await api(`/api/roles/${id}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` },
            });

            roles.value = roles.value.filter((r) => r.id !== id);
            selectedRoles.value = selectedRoles.value.filter((sid) => sid !== id);

            await Swal.fire({ icon: 'success', title: 'Silindi!', text: 'Rol başarıyla silindi.', timer: 1500, showConfirmButton: false });
        } catch (e) {
            await Swal.fire({ icon: 'error', title: 'Hata', text: 'Silme işlemi başarısız oldu.' });
            console.error(e);
        }
    }
};

const deleteSelectedRoles = async () => {
    const result = await Swal.fire({
        title: 'Emin misiniz?',
        text: `${selectedRoles.value.length} rolü toplu olarak silmek istediğinize emin misiniz?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Evet, sil!',
        cancelButtonText: 'İptal'
    });

    if (result.isConfirmed) {
        const token = localStorage.getItem("token");
        try {
            await Promise.all(
                selectedRoles.value.map((id) =>
                    api(`/api/roles/${id}`, {
                        method: "DELETE",
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                ),
            );

            roles.value = roles.value.filter(
                (r) => !selectedRoles.value.includes(r.id),
            );
            selectedRoles.value = [];

            if (paginatedRoles.value.length === 0 && currentPage.value > 1) {
                currentPage.value--;
            }

            await Swal.fire({ icon: 'success', title: 'Silindi!', text: 'Seçili roller başarıyla silindi.', timer: 1500, showConfirmButton: false });
        } catch (e) {
            await Swal.fire({ icon: 'error', title: 'Hata', text: 'Bazı roller silinirken hata oluştu.' });
            console.error(e);
            await fetchRolesList(token);
        }
    }
};
</script>

