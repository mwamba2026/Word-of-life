<template>
  <section class="section page-section"><div class="container">
    <header class="section-heading center page-heading">
      <p class="eyebrow">Watch and study</p>
      <h1 class="section-title">Teaching to explore with an open Bible.</h1>
      <p class="lede">Explore credited teaching from original publishers and recordings shared by this ministry.</p>
    </header>
    <div class="card-grid teaching-grid">
      <article v-for="item in teachings" :key="item.mediaUrl || item.url || item.title" class="info-card teaching-card">
        <p class="label">{{ item.series }}</p><h2>{{ item.title }}</h2>
        <p class="teaching-meta">{{ item.credit }} · {{ item.topic }}</p><p>{{ item.summary }}</p>
        <video v-if="item.mediaType === 'video' && item.mediaUrl" controls preload="metadata" :src="item.mediaUrl" class="player">Your browser cannot play this video.</video>
        <audio v-else-if="item.mediaUrl" controls preload="metadata" :src="item.mediaUrl" class="player">Your browser cannot play this audio.</audio>
        <a v-if="item.url" class="button button-secondary teaching-link" :href="item.url" target="_blank" rel="noopener noreferrer">Watch at publisher <span aria-hidden="true">↗</span></a>
      </article>
    </div>
    <p class="source-note">External programs open at their publisher. Uploaded recordings play here.</p>
  </div></section>
  <section class="section section-alt"><div class="container center">
    <p class="eyebrow">Read alongside teaching</p><h2 class="section-title">Keep Scripture close.</h2>
    <p class="lede centered">Return to the reading plan and note what you want to remember.</p>
    <RouterLink class="button button-primary" to="/bible">Open the Bible plan</RouterLink>
  </div></section>
</template>
<script setup>
import { siteName } from '../config.js';
import { reactive, watch } from 'vue';
import { resourceState } from '../resourceState.js';
const teachings = reactive([
  { title: 'The Day of the Lord Pt. 2', credit: 'Doug Batchelor · Amazing Facts', topic: 'The Sabbath', series: 'Amazing Facts with Doug Batchelor · 2026', summary: 'A Bible teaching on the seventh-day Sabbath and its place in Christian life.', url: 'https://www.amazingfacts.org/watch/broadcasts/amazing-facts-with-doug-batchelor/amazing-facts-with-doug-batchelor-2026/the-day-of-the-lord-pt-2/' },
  { title: 'Revelation and Inspiration Lesson 5', credit: 'Amazing Facts', topic: 'The gift of prophecy', series: 'The Gift of Prophecy · 2026', summary: 'A study of how God communicated through the prophets.', url: 'https://www.amazingfacts.org/watch/broadcasts/studying-the-word/the-gift-of-prophecy-2026/revelation-and-inspiration-lesson-5/' }
]);
watch(() => resourceState.data?.teachings, items => {
  if (Array.isArray(items)) teachings.splice(0, teachings.length, ...items);
}, { immediate: true });
</script>
<style scoped>
.page-heading .lede, .centered { margin-inline: auto; }
.teaching-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); }
.teaching-card h2 { font-family: 'Fraunces', Georgia, serif; font-size: 1.5rem; line-height: 1.25; margin: 0 0 10px; }
.teaching-meta { color: var(--gold-strong) !important; font-weight: 700; margin-bottom: 14px !important; }
.teaching-link { align-self: flex-start; margin-top: 24px; }
.player { display: block; width: 100%; margin-top: 18px; border-radius: 12px; background: var(--primary-strong); }
.source-note { color: var(--muted); text-align: center; margin-top: 24px; }
@media (max-width: 760px) { .teaching-grid { grid-template-columns: 1fr; } }
</style>
