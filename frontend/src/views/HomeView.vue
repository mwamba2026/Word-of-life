<template>
  <section class="hero"><div class="container hero-grid">
    <div class="hero-copy">
      <p class="eyebrow">Scripture for the next faithful step</p>
      <h1>Make room for the Word today.</h1>
      <p class="lede">Read a passage, reflect on it, and carry one small act of faith into your day.</p>
      <div class="hero-actions"><RouterLink class="button button-primary" to="/bible">Read today’s passage</RouterLink><RouterLink class="button button-secondary" to="/support">Share a prayer request</RouterLink></div>
    </div>
    <div class="hero-panel"><div class="panel-card peace-card">
      <p class="label">Today’s passage · {{ today.theme }}</p>
      <p class="reading-reference">{{ today.reference }}</p>
      <h2>{{ today.verses[0].text }}</h2>
      <p>{{ today.question }}</p>
      <RouterLink class="text-link" to="/bible">Read and reflect <span aria-hidden="true">→</span></RouterLink>
    </div></div>
  </div></section>

  <section class="section section-alt" aria-labelledby="explore-heading"><div class="container">
    <div class="section-heading center"><p class="eyebrow">Choose a path</p><h2 id="explore-heading" class="section-title">Explore one step at a time.</h2>
      <p class="lede centered">Use the arrows or swipe to move between Study, Watch, Listen, Podcasts, Sing, Reflect, and Pray.</p></div>
    <div class="path-carousel" role="region" aria-roledescription="carousel" aria-label="Ways to explore"
         tabindex="0" @keydown.left.prevent="move(-1)" @keydown.right.prevent="move(1)"
         @touchstart.passive="touchStart" @touchend.passive="touchEnd">
      <div class="carousel-controls">
        <button type="button" class="arrow-button" @click="move(-1)" :aria-label="`Previous path: ${previous.label}`">←</button>
        <span class="carousel-count" aria-live="polite">{{ activeIndex + 1 }} / {{ paths.length }}</span>
        <button type="button" class="arrow-button" @click="move(1)" :aria-label="`Next path: ${next.label}`">→</button>
      </div>
      <article class="panel-card carousel-panel" :key="active.to">
        <p class="eyebrow gold-label">{{ active.kicker }}</p>
        <h3>{{ active.label }}</h3><p class="lede">{{ active.text }}</p>
        <p class="carousel-detail">{{ active.detail }}</p>
        <RouterLink class="button button-primary" :to="active.to">{{ active.action }}</RouterLink>
      </article>
      <div class="carousel-dots" role="group" aria-label="Choose a path">
        <button v-for="(path, index) in paths" :key="path.to" type="button" class="carousel-dot"
                :class="{ selected: index === activeIndex }" :aria-label="`Show ${path.label}`"
                :aria-pressed="index === activeIndex" @click="activeIndex = index"></button>
      </div>
    </div>
  </div></section>
</template>
<script setup>
import { computed, reactive, ref, watch } from 'vue';
import { resourceState } from '../resourceState.js';
import { getTodaysReading } from '../readingPlan.js';
const today = reactive(getTodaysReading());
const paths = reactive([
  { label: 'Study', kicker: 'Begin with the Word', text: 'Read the daily passage and move through a seven-day cycle.', detail: today.reference, action: 'Open the reading plan', to: '/bible' },
  { label: 'Watch', kicker: 'Teaching with context', text: 'Explore programs linked to and credited to their original publisher.', detail: 'Bible teaching from Amazing Facts', action: 'Explore teaching', to: '/sermons' },
  { label: 'Listen', kicker: 'Audiobooks on the go', text: 'Explore Christian audio hosted by Amazing Facts.', detail: 'Audiobooks · original publisher', action: 'Browse audiobooks', to: '/listen' },
  { label: 'Podcasts', kicker: 'Episodes to revisit', text: 'Listen to new ministry episodes when they are published.', detail: 'Audio and video episodes', action: 'Browse podcasts', to: '/podcasts' },
  { label: 'Sing', kicker: 'Worship at your pace', text: 'Read and sing public-domain hymns, grouped by theme.', detail: 'Praise · Grace · Comfort · Hope', action: 'Browse hymns', to: '/songs' },
  { label: 'Reflect', kicker: 'Put faith into practice', text: 'Read short reflections and choose a practical next step.', detail: 'Scripture, reflection, action', action: 'Read reflections', to: '/write-ups' },
  { label: 'Pray', kicker: 'Bring what is on your heart', text: 'Use the prayer form or pause with a word of encouragement.', detail: 'Requests are not answered in real time', action: 'Visit prayer', to: '/support' }
]);
watch(() => resourceState.data?.readings, items => {
  if (!Array.isArray(items) || !items.length) return;
  Object.assign(today, getTodaysReading(new Date(), items));
  paths[0].detail = today.reference;
}, { immediate: true });
const activeIndex = ref(0);
const active = computed(() => paths[activeIndex.value]);
const previous = computed(() => paths[(activeIndex.value - 1 + paths.length) % paths.length]);
const next = computed(() => paths[(activeIndex.value + 1) % paths.length]);
function move(delta) { activeIndex.value = (activeIndex.value + delta + paths.length) % paths.length; }
let startX = 0;
function touchStart(event) { startX = event.changedTouches[0].screenX; }
function touchEnd(event) { const delta = event.changedTouches[0].screenX - startX; if (Math.abs(delta) > 45) move(delta < 0 ? 1 : -1); }
</script>
<style scoped>
.peace-card { background: #fff; border-top: 4px solid var(--peace); }
.peace-card h2 { font-size: clamp(1.35rem, 2vw, 1.75rem); line-height: 1.35; }
.centered { margin: 18px auto 0; }
.path-carousel { max-width: 780px; margin: 0 auto; }
.carousel-controls { display: flex; align-items: center; justify-content: center; gap: 28px; margin-bottom: 18px; }
.arrow-button { width: 50px; height: 50px; border-radius: 50%; background: var(--primary); color: #fff; font-size: 1.5rem; }
.arrow-button:hover { background: var(--primary-strong); }
.carousel-count { color: var(--gold-strong); font-weight: 800; }
.carousel-panel { min-height: 285px; text-align: center; background: #fff; border-top: 4px solid var(--accent); }
.carousel-panel h3 { font-family: 'Fraunces', Georgia, serif; font-size: clamp(2rem, 4vw, 3rem); margin: 0; }
.carousel-panel .lede { margin: 12px auto; }
.gold-label { color: var(--gold-strong); }
.carousel-detail { color: var(--primary-strong); font-weight: 700; }
.carousel-dots { display: flex; justify-content: center; gap: 9px; margin-top: 22px; }
.carousel-dot { width: 13px; height: 13px; border-radius: 50%; background: #bfd0da; }
.carousel-dot.selected { background: var(--gold-strong); }
</style>
