<template>
  <section class="section">
    <div class="container">
      <div class="section-heading center">
        <p class="eyebrow">Bible</p>
        <h1 class="section-title">Scripture as light, wisdom, and strength for the journey.</h1>
        <p class="lede intro">
          Read a short passage slowly, notice what stands out to you, and carry one
          thought with you into your day.
        </p>
      </div>

      <article class="panel-card card-highlight today" aria-labelledby="today-heading">
        <p class="label">Today’s passage · seven-day cycle</p>
        <h2 id="today-heading" class="reading-reference today-ref">{{ todaysReading.reference }}</h2>
        <blockquote class="passage" :class="{ poetry: todaysReading.poetry }">
          <p>
            <span v-for="verse in todaysReading.verses" :key="verse.n" class="verse"><sup v-if="todaysReading.verses.length > 1" class="verse-num">{{ verse.n }}</sup>{{ verse.text + ' ' }}</span>
          </p>
        </blockquote>
        <div class="reflect">
          <p class="label">Reflect</p>
          <p class="question">{{ todaysReading.question }}</p>
        </div>
        <RouterLink class="text-link" to="/notes">Write your thoughts in My notes <span aria-hidden="true">→</span></RouterLink>
      </article>
    </div>
  </section>

  <section class="section section-alt" aria-labelledby="plan-heading">
    <div class="container">
      <div class="section-heading center">
        <p class="eyebrow">A seven-day reading plan</p>
        <h2 id="plan-heading" class="section-title">Well-loved passages to read slowly.</h2>
        <p class="lede intro">Choose a day to read the full passage and a question to think over.</p>
      </div>

      <div class="plan-layout">
        <ol class="plan-list" role="list" aria-label="Reading plan">
          <li v-for="(item, index) in readingPlan" :key="item.id">
            <button
              type="button"
              class="plan-item"
              :class="{ 'is-selected': item.id === selectedId }"
              :aria-current="item.id === selectedId ? 'true' : undefined"
              aria-controls="plan-reader"
              @click="select(item.id)"
            >
              <span class="plan-day">Day {{ index + 1 }}</span>
              <span class="plan-ref">{{ item.reference }}</span>
              <span class="plan-theme">{{ item.theme }}</span>
            </button>
          </li>
        </ol>

        <article id="plan-reader" ref="reader" class="panel-card reader" aria-labelledby="reader-heading">
          <p class="label">Day {{ selectedIndex + 1 }} · {{ selected.theme }}</p>
          <h3 id="reader-heading" class="reader-ref">{{ selected.reference }}</h3>
          <blockquote class="passage" :class="{ poetry: selected.poetry }">
            <p>
              <span v-for="verse in selected.verses" :key="verse.n" class="verse"><sup v-if="selected.verses.length > 1" class="verse-num">{{ verse.n }}</sup>{{ verse.text + ' ' }}</span>
            </p>
          </blockquote>

          <div class="reflect">
            <p class="label">Reflect</p>
            <p class="question">{{ selected.question }}</p>
            <p v-if="selected.plainWords" class="plain-words">
              <strong>Older words, plain meaning:</strong> {{ selected.plainWords }}
            </p>
          </div>

          <RouterLink class="text-link" to="/notes">Write your thoughts in My notes <span aria-hidden="true">→</span></RouterLink>

          <div class="reader-nav">
            <button
              ref="prevButton"
              type="button"
              class="button button-secondary"
              :disabled="selectedIndex === 0"
              @click="step(-1)"
            >
              <span aria-hidden="true">←</span>&nbsp;Previous day
            </button>
            <button
              ref="nextButton"
              type="button"
              class="button button-secondary"
              :disabled="selectedIndex === readingPlan.length - 1"
              @click="step(1)"
            >
              Next day&nbsp;<span aria-hidden="true">→</span>
            </button>
          </div>
        </article>
      </div>

      <p class="kjv-note">Scripture quotations from the King James Version (public domain).</p>
      <p class="visually-hidden" aria-live="polite">{{ announcement }}</p>
    </div>
  </section>

  <section class="section section-alt" aria-labelledby="external-guides-heading">
    <div class="container">
      <div class="section-heading center">
        <p class="eyebrow">Continue studying</p>
        <h2 id="external-guides-heading" class="section-title">Bible study guides from Amazing Facts.</h2>
        <p class="lede guide-intro">These guides are published and hosted by Amazing Facts. They open on the original site.</p>
      </div>
      <div class="card-grid guide-grid">
        <article v-for="guide in externalGuides" :key="guide.url" class="info-card guide-card">
          <p class="label">{{ guide.lesson }}</p>
          <h3>{{ guide.title }}</h3>
          <p>{{ guide.description }}</p>
          <a class="text-link" :href="guide.url" target="_blank" rel="noopener noreferrer">Read at publisher <span aria-hidden="true">↗</span></a>
        </article>
      </div>
      <p class="guide-all"><a class="button button-secondary" href="https://www.amazingfacts.org/study/bible-study-guides/" target="_blank" rel="noopener noreferrer">Browse all 27 guides ↗</a></p>
    </div>
  </section>
</template>

<script setup>
import { computed, nextTick, reactive, ref, watch } from 'vue';
import { resourceState } from '../resourceState.js';

const externalGuides = reactive([
  { lesson: 'Lesson 1', title: 'Is There Anything Left You Can Trust?', description: 'Explore why the Bible can be trusted.', url: 'https://www.amazingfacts.org/study/bible-study-guides/is-there-anything-left-you-can-trust/' },
  { lesson: 'Lesson 2', title: 'Did God Create the Devil?', description: 'Study the Bible account of the origin of evil.', url: 'https://www.amazingfacts.org/study/bible-study-guides/did-god-create-the-devil/' },
  { lesson: 'Lesson 3', title: 'Saved from Certain Death', description: 'Consider the Bible message of salvation.', url: 'https://www.amazingfacts.org/study/bible-study-guides/saved-from-certain-death/' }
]);

import { readingPlan as fallbackReadings, getTodaysReading } from '../readingPlan.js';
const readingPlan = reactive([...fallbackReadings]);



const todaysReading = reactive(getTodaysReading());
const selectedId = ref(todaysReading.id);
const selectedIndex = computed(() => readingPlan.findIndex((item) => item.id === selectedId.value));
const selected = computed(() => readingPlan[selectedIndex.value]);
const announcement = ref('');

const reader = ref(null);
const prevButton = ref(null);
const nextButton = ref(null);
watch(() => resourceState.data, data => {
  if (!data) return;
  if (Array.isArray(data.readings) && data.readings.length) {
    readingPlan.splice(0, readingPlan.length, ...data.readings);
    Object.assign(todaysReading, getTodaysReading(new Date(), readingPlan));
    selectedId.value = todaysReading.id;
  }
  if (Array.isArray(data.guides)) externalGuides.splice(0, externalGuides.length, ...data.guides);
}, { immediate: true });

function prefersReducedMotion() {
  return typeof window.matchMedia === 'function' && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
}

// On phones the reader sits below the list, so bring it into view when a new day is chosen.
// The sticky site header is measured so the passage heading is not hidden behind it.
function revealReader() {
  const el = reader.value;
  if (!el) return;
  const header = document.querySelector('.site-header');
  const offset = (header ? header.offsetHeight : 0) + 16;
  const top = el.getBoundingClientRect().top;
  if (top >= offset && top <= window.innerHeight * 0.6) return;
  window.scrollTo({
    top: window.scrollY + top - offset,
    behavior: prefersReducedMotion() ? 'auto' : 'smooth'
  });
}

function select(id) {
  selectedId.value = id;
  const index = selectedIndex.value;
  announcement.value = `Showing day ${index + 1}: ${readingPlan[index].reference}.`;
  nextTick(revealReader);
}

function step(delta) {
  const target = readingPlan[selectedIndex.value + delta];
  if (!target) return;
  select(target.id);
  // If the button just pressed is now disabled (first or last day), keep keyboard focus nearby.
  nextTick(() => {
    const pressed = delta < 0 ? prevButton.value : nextButton.value;
    const other = delta < 0 ? nextButton.value : prevButton.value;
    if (pressed && pressed.disabled && other) other.focus();
  });
}
</script>

<style scoped>

.guide-intro { margin: 18px auto 0; }
.guide-card h3 { font-family: 'Fraunces', Georgia, serif; font-weight: 600; }
.guide-all { text-align: center; margin: 28px 0 0; }

.intro {
  margin: 18px auto 0;
}

/* Today's reading */
.today {
  max-width: 820px;
  margin: 0 auto;
  padding: clamp(22px, 4vw, 40px);
}

.today-ref,
.reader-ref {
  margin: 0 0 16px;
  font-size: clamp(1.6rem, 3vw, 2.2rem);
  letter-spacing: -0.03em;
  line-height: 1.15;
}

.today-ref {
  color: var(--primary-strong);
}

/* Scripture text */
.passage {
  margin: 0 0 22px;
  font-family: Georgia, 'Times New Roman', serif;
  font-size: 1.15rem;
  line-height: 1.8;
  color: var(--text);
}

.passage p {
  margin: 0;
}

.poetry .verse {
  display: block;
  padding-left: 1.4em;
  text-indent: -1.4em;
}

.poetry .verse + .verse {
  margin-top: 6px;
}

.verse-num {
  font-family: 'Source Sans 3', 'Segoe UI', sans-serif;
  font-size: 0.62em;
  font-weight: 700;
  line-height: 0;
  color: var(--primary);
  margin-right: 0.25em;
}

.reflect {
  border-top: 1px solid var(--line);
  padding-top: 18px;
}

.question {
  margin: 0;
  font-size: 1.05rem;
  font-weight: 600;
  color: var(--text);
}

.plain-words {
  margin: 14px 0 0;
  color: var(--muted);
  font-size: 0.95rem;
}

.plain-words strong {
  color: var(--text);
  font-weight: 700;
}

/* Reading plan */
.plan-layout {
  display: grid;
  grid-template-columns: minmax(240px, 340px) minmax(0, 1fr);
  gap: 24px;
  align-items: start;
}

.plan-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: grid;
  gap: 10px;
}

.plan-item {
  width: 100%;
  display: grid;
  grid-template-columns: auto 1fr;
  column-gap: 14px;
  align-items: baseline;
  text-align: left;
  padding: 14px 16px;
  border: 1px solid var(--line);
  border-radius: 16px;
  background: var(--surface);
  color: var(--text);
  transition: border-color 160ms ease, background-color 160ms ease, box-shadow 160ms ease;
}

.plan-item:hover {
  border-color: rgba(49, 95, 125, 0.35);
}

.plan-item:focus-visible,
.reader-nav .button:focus-visible {
  outline: 3px solid rgba(49, 95, 125, 0.45);
  outline-offset: 2px;
}

.plan-item.is-selected {
  border-color: var(--primary);
  background: linear-gradient(145deg, rgba(49, 95, 125, 0.1), rgba(178, 138, 72, 0.16));
  box-shadow: inset 4px 0 0 var(--accent), 0 10px 24px rgba(14, 22, 24, 0.06);
}

.plan-day {
  grid-row: span 2;
  min-width: 3.8em;
  font-size: 0.72rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.12em;
  color: var(--primary);
}

.plan-ref {
  font-weight: 800;
}

.plan-theme {
  grid-column: 2;
  color: var(--muted);
  font-size: 0.9rem;
}

.reader {
  padding: clamp(22px, 4vw, 36px);
}

.reader-ref {
  color: var(--primary-strong);
}

.reader-nav {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  gap: 12px;
  margin-top: 24px;
  padding-top: 18px;
  border-top: 1px solid var(--line);
}

.reader-nav .button {
  flex: 1 1 auto;
  padding: 0 18px;
}

.reader-nav .button:disabled {
  opacity: 0.45;
  cursor: not-allowed;
}

.kjv-note {
  margin: 28px 0 0;
  text-align: center;
  color: var(--muted);
  font-size: 0.9rem;
}

.visually-hidden {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0 0 0 0);
  white-space: nowrap;
  border: 0;
}

@media (max-width: 820px) {
  .plan-layout {
    grid-template-columns: 1fr;
  }
}

@media (min-width: 821px) {
  .reader-nav .button {
    flex: 0 1 auto;
  }
}
</style>
