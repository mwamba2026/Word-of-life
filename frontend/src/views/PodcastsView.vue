<template>
  <section class="section"><div class="container">
    <header class="section-heading center"><p class="eyebrow">Podcasts</p>
      <h1 class="section-title">Listen, watch, and reflect.</h1>
      <p class="lede centered">Episodes published by this ministry appear here. More will be added as recordings are ready.</p>
    </header>
    <div v-if="episodes.length" class="card-grid episode-grid">
      <article v-for="episode in episodes" :key="episode.mediaUrl || episode.url || episode.title" class="info-card">
        <p class="label">{{ episode.series || 'Podcast episode' }}</p><h2>{{ episode.title }}</h2>
        <p v-if="episode.host" class="episode-meta">{{ episode.host }}</p>
        <p>{{ episode.description }}</p>
        <video v-if="episode.mediaType === 'video' && episode.mediaUrl" controls preload="metadata" :src="episode.mediaUrl" class="player">Your browser cannot play this video.</video>
        <audio v-else-if="episode.mediaUrl" controls preload="metadata" :src="episode.mediaUrl" class="player">Your browser cannot play this audio.</audio>
        <a v-if="episode.url" class="text-link" :href="episode.url" target="_blank" rel="noopener noreferrer">Open at publisher <span aria-hidden="true">↗</span></a>
      </article>
    </div>
    <div v-else class="panel-card empty center"><h2>Episodes are on the way.</h2><p>Check back when the first recording is published.</p></div>
  </div></section>
</template>
<script setup>
import { computed } from 'vue';
import { resourceState } from '../resourceState.js';
const episodes = computed(() => resourceState.data?.podcasts || []);
</script>
<style scoped>
.centered { margin-inline: auto; }.episode-grid { grid-template-columns: repeat(2,minmax(0,1fr)); }
.episode-grid h2 { font-family: 'Fraunces', Georgia, serif; line-height: 1.25; }
.episode-meta { color: var(--gold-strong); font-weight: 700; }
.player { display: block; width: 100%; margin-top: 16px; border-radius: 12px; background: var(--primary-strong); }
.empty { max-width: 660px; margin: 0 auto; background: white; border-top: 4px solid var(--accent); }
@media (max-width: 760px) { .episode-grid { grid-template-columns: 1fr; } }
</style>
