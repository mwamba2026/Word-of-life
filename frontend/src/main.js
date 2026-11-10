import { createApp } from 'vue';
import App from './App.vue';
import router from './router.js';
import { siteName } from './config.js';
import './style.css';

router.afterEach((to) => {
  document.title = to.meta.title && to.name !== 'home' ? `${to.meta.title} | ${siteName}` : siteName;
});

createApp(App).use(router).mount('#app');

if (import.meta.env.PROD && 'serviceWorker' in navigator) {
  window.addEventListener('load', () => {
    navigator.serviceWorker.register('/service-worker.js').catch(() => {});
  });
}
