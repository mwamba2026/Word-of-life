<template>
  <div class="page-shell">
    <header class="site-header">
      <div class="container nav-wrap">
        <RouterLink class="brand" to="/">
          <span class="brand-mark" aria-hidden="true">✦</span>
          <div>
            <p class="brand-name">{{ siteName }}</p>
            <p class="brand-tag">{{ tagline }}</p>
          </div>
        </RouterLink>

        <nav class="main-nav" aria-label="Main navigation">
          <RouterLink v-for="link in mainLinks" :key="link.to" :to="link.to">{{ link.label }}</RouterLink>
        </nav>

        <div class="header-actions">
          <RouterLink v-if="!user" class="text-link" to="/login">Sign in</RouterLink>
          <button v-else class="text-link sign-out" type="button" @click="signOut">Sign out</button>
          <RouterLink class="button button-primary" to="/donate">Donate</RouterLink>
        </div>
      </div>
    </header>

    <main>
      <RouterView />
      <nav v-if="sectionIndex >= 0" class="section-pager container" aria-label="Continue exploring">
        <RouterLink class="pager-arrow" :to="previousSection.to" :aria-label="`Previous section: ${previousSection.label}`">
          <span aria-hidden="true">←</span><span>{{ previousSection.label }}</span>
        </RouterLink>
        <span class="pager-position">{{ sectionIndex + 1 }} / {{ sections.length }}</span>
        <RouterLink class="pager-arrow next" :to="nextSection.to" :aria-label="`Next section: ${nextSection.label}`">
          <span>{{ nextSection.label }}</span><span aria-hidden="true">→</span>
        </RouterLink>
      </nav>
    </main>

    <footer class="site-footer">
      <div class="container footer-wrap">
        <p>© {{ year }} {{ siteName }}</p>
        <div class="footer-links">
          <RouterLink v-if="user?.isAdmin" to="/staff/content">Manage content</RouterLink>
          <RouterLink v-for="link in footerLinks" :key="link.to" :to="link.to">{{ link.label }}</RouterLink>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue';
import { useRoute } from 'vue-router';
import { siteName, tagline } from './config.js';
import { auth } from './api.js';
import { loadResources } from './resourceState.js';

const year = new Date().getFullYear();
const route = useRoute();
const user = ref(auth.currentUser());
onMounted(() => {
  loadResources();
  auth.me().then(current => { user.value = current; }).catch(() => { user.value = null; });
});
const sections = [
  { to: '/bible', label: 'Study' },
  { to: '/sermons', label: 'Watch' },
  { to: '/listen', label: 'Listen' },
  { to: '/podcasts', label: 'Podcasts' },
  { to: '/songs', label: 'Sing' },
  { to: '/write-ups', label: 'Reflect' },
  { to: '/support', label: 'Pray' }
];
const sectionIndex = computed(() => sections.findIndex((item) => item.to === route.path));
const previousSection = computed(() => sections[(sectionIndex.value - 1 + sections.length) % sections.length]);
const nextSection = computed(() => sections[(sectionIndex.value + 1) % sections.length]);

// Re-read the signed-in user after each navigation (login and register pages update it).
watch(() => route.fullPath, () => {
  user.value = auth.currentUser();
});

async function signOut() {
  await auth.logout();
  user.value = null;
}

const mainLinks = [
  { to: '/bible', label: 'Read' },
  { to: '/sermons', label: 'Watch' },
  { to: '/listen', label: 'Listen' },
  { to: '/podcasts', label: 'Podcasts' },
  { to: '/songs', label: 'Sing' },
  { to: '/write-ups', label: 'Reflect' },
  { to: '/support', label: 'Pray' },
  { to: '/about', label: 'About' }
];

const footerLinks = [
  { to: '/notes', label: 'My notes' },
  { to: '/feedback', label: 'Feedback' },
  { to: '/donate', label: 'Donate' },
  { to: '/register', label: 'Create account' }
];
</script>

<style scoped>
.main-nav {
  flex-wrap: wrap;
}

.main-nav .router-link-active {
  color: var(--primary);
  font-weight: 600;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 16px;
}

/* The global .text-link adds a top margin meant for cards; keep Sign in level with Donate. */
.header-actions .text-link {
  margin-top: 0;
}

.sign-out {
  background: none;
  padding: 0;
  color: var(--primary-strong);
}

@media (max-width: 820px) {
  .header-actions {
    gap: 12px;
  }

  .header-actions .button {
    min-height: 40px;
    padding: 0 16px;
  }
}

@media (max-width: 400px) {
  .header-actions {
    gap: 10px;
    font-size: 0.92rem;
  }

  .header-actions .button {
    padding: 0 14px;
  }
}
</style>

<style>
.section-pager { display: flex; align-items: center; justify-content: space-between; gap: 16px; padding: 22px 0 48px; border-top: 1px solid var(--line); }
.pager-arrow { display: inline-flex; align-items: center; gap: 10px; min-height: 48px; padding: 10px 16px; border: 1px solid rgba(49,95,125,.3); border-radius: 999px; color: var(--primary-strong); font-weight: 700; }
.pager-arrow:hover { background: #e7f1f6; }
.pager-arrow span[aria-hidden] { font-size: 1.35rem; line-height: 1; }
.pager-position { color: var(--gold-strong); font-weight: 800; white-space: nowrap; }
@media (max-width: 480px) { .pager-arrow { padding: 8px 12px; gap: 6px; font-size: .9rem; } }
</style>
