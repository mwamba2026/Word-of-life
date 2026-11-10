import { createRouter, createWebHistory } from 'vue-router';
import HomeView from './views/HomeView.vue';

const routes = [
  { path: '/', name: 'home', component: HomeView, meta: { title: 'Home' } },
  { path: '/listen', name: 'listen', component: () => import('./views/ListenView.vue'), meta: { title: 'Listen' } },
  { path: '/podcasts', name: 'podcasts', component: () => import('./views/PodcastsView.vue'), meta: { title: 'Podcasts' } },
  { path: '/songs', name: 'songs', component: () => import('./views/SongsView.vue'), meta: { title: 'Songs' } },
  { path: '/sermons', name: 'sermons', component: () => import('./views/SermonsView.vue'), meta: { title: 'Sermons' } },
  { path: '/bible', name: 'bible', component: () => import('./views/BibleView.vue'), meta: { title: 'Bible' } },
  { path: '/write-ups', name: 'write-ups', component: () => import('./views/WriteUpsView.vue'), meta: { title: 'Write-ups' } },
  { path: '/notes', name: 'notes', component: () => import('./views/NotesView.vue'), meta: { title: 'My notes' } },
  { path: '/staff/content', name: 'staff-content', component: () => import('./views/StaffContentView.vue'), meta: { title: 'Manage content' } },
  { path: '/about', name: 'about', component: () => import('./views/AboutView.vue'), meta: { title: 'About' } },
  { path: '/support', name: 'support', component: () => import('./views/SupportView.vue'), meta: { title: 'Support' } },
  { path: '/feedback', name: 'feedback', component: () => import('./views/FeedbackView.vue'), meta: { title: 'Feedback' } },
  { path: '/donate', name: 'donate', component: () => import('./views/DonateView.vue'), meta: { title: 'Donate' } },
  { path: '/login', name: 'login', component: () => import('./views/LoginView.vue'), meta: { title: 'Sign in' } },
  { path: '/register', name: 'register', component: () => import('./views/RegisterView.vue'), meta: { title: 'Create account' } },
  { path: '/:pathMatch(.*)*', redirect: '/' }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior() {
    return { top: 0 };
  }
});

export default router;
