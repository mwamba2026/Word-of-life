<template>
  <section class="section"><div class="container">
    <header class="section-heading center">
      <p class="eyebrow">Listen</p>
      <h1 class="section-title">Christian audiobooks for the journey.</h1>
      <p class="lede centered">Explore Christian audiobooks from their original publishers and recordings added by this ministry.</p>
    </header>
    <div class="card-grid audio-grid">
      <article v-for="book in books" :key="book.mediaUrl || book.url || book.title" class="info-card">
        <p class="label">Christian audiobooks</p>
        <h2>{{ book.title }}</h2><p>{{ book.description }}</p>
        <audio v-if="book.mediaUrl" controls preload="metadata" :src="book.mediaUrl" class="player">Your browser cannot play this audio.</audio>
        <a v-if="book.url" class="text-link" :href="book.url" target="_blank" rel="noopener noreferrer">Open at publisher ↗</a>
      </article>
    </div>
    <div class="panel-card catalog-card">
      <div><p class="eyebrow gold-label">More to explore</p><h2>Browse the full audiobook catalog.</h2>
        <p>Amazing Facts adds and manages its own recordings and playback.</p></div>
      <a class="button button-primary" href="https://www.amazingfacts.org/listen/christian-audiobooks/" target="_blank" rel="noopener noreferrer">Open Amazing Facts audiobooks â†—</a>
    </div>
  </div></section>
</template>
<script setup>
import { siteName } from '../config.js';
import { reactive, watch } from 'vue';
import { resourceState } from '../resourceState.js';
const books = reactive([
  { title: 'The High Cost of the Cross', description: 'A Christian audiobook listed in the Amazing Facts catalog.', url: 'https://www.amazingfacts.org/listen/christian-audiobooks/aim_the_high_cost_of_the_cross-mp3/' },
  { title: 'Anything but Secret', description: 'A Christian audiobook listed in the Amazing Facts catalog.', url: 'https://www.amazingfacts.org/listen/christian-audiobooks/anything-but-secret/' }
]);
watch(() => resourceState.data?.audiobooks, items => {
  if (Array.isArray(items)) books.splice(0, books.length, ...items);
}, { immediate: true });
</script>
<style scoped>
.player { display: block; width: 100%; margin-top: 16px; }
.centered { margin: 18px auto 0; }
.audio-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); }
.audio-grid h2, .catalog-card h2 { font-family: 'Fraunces', Georgia, serif; line-height: 1.2; }
.audio-grid h2 { font-size: 1.5rem; margin: 0 0 12px; }
.catalog-card { display: flex; justify-content: space-between; align-items: center; gap: 22px; margin-top: 30px; background: #fff; }
.catalog-card h2 { margin: 0 0 8px; }
.catalog-card p:last-child { margin: 0; color: var(--muted); }
.gold-label { color: var(--gold-strong); }
@media (max-width: 760px) { .audio-grid { grid-template-columns: 1fr; } .catalog-card { display: block; } .catalog-card .button { margin-top: 18px; } }
</style>
