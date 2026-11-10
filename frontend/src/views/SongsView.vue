<template>
  <section class="section page-section">
    <div class="container">
      <header class="section-heading center page-heading">
        <p class="eyebrow">Songs</p>
        <h1 class="section-title">Worship that draws the heart closer to God.</h1>
        <p class="lede">
          Read and sing public-domain hymns for praise, comfort, and hope. Choose a theme
          to find a hymn for where you are today.
        </p>
      </header>

      <h2 class="list-title">Songs and hymns</h2>

      <div class="theme-filter" role="group" aria-label="Filter songs by theme">
        <button
          v-for="theme in themes"
          :key="theme"
          type="button"
          class="theme-chip"
          :class="{ 'is-active': selectedTheme === theme }"
          :aria-pressed="selectedTheme === theme ? 'true' : 'false'"
          @click="selectedTheme = theme"
        >
          {{ theme }}
        </button>
      </div>

      <p class="filter-status" aria-live="polite">{{ statusText }}</p>

      <div class="card-grid three-up song-grid">
        <article v-for="song in visibleSongs" :key="song.id" class="info-card song-card">
          <div class="icon" aria-hidden="true">{{ themeIcons[song.theme] }}</div>
          <p class="label">{{ song.theme }}</p>
          <h3>{{ song.title }}</h3>
          <p class="song-meta">
            {{ song.author ? `Hymn · words by ${song.author}` : 'Hymn for reflection' }}
          </p>
          <p class="song-description">{{ song.description }}</p>

          <details v-if="song.verse" class="song-verse">
            <summary>Read the first verse</summary>
            <p class="verse-text">{{ song.verse }}</p>
            <p class="verse-note">These words are in the public domain.</p>
          </details>
        </article>
      </div>
    </div>
  </section>

  <section class="section section-alt">
    <div class="container center closing">
      <p class="eyebrow">Sing with gladness</p>
      <figure class="scripture">
        <blockquote>
          <p>
            “Make a joyful noise unto the LORD, all ye lands. Serve the LORD with gladness:
            come before his presence with singing.”
          </p>
        </blockquote>
        <figcaption class="reading-reference">Psalm 100:1–2 (KJV)</figcaption>
      </figure>
      <p class="lede">
        Has a song carried you through a hard season, or is there one you would like to hear here?
        Tell the {{ siteName }} team.
      </p>
      <div class="closing-actions">
        <RouterLink class="button button-primary" to="/feedback">Share a song with us</RouterLink>
        <RouterLink class="button button-secondary" to="/sermons">Browse sermons</RouterLink>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed, reactive, ref, watch } from 'vue';
import { resourceState } from '../resourceState.js';
import { siteName } from '../config.js';

const themeIcons = {
  Praise: '♫',
  Grace: '✦',
  Comfort: '♡',
  Hope: '✧'
};

const themes = ['All', ...Object.keys(themeIcons)];

// Public-domain hymn verses for reading and singing; no audio is claimed.
const songs = reactive([
  {
    id: 'amazing-grace',
    title: 'Amazing Grace',
    theme: 'Grace',
    author: 'John Newton',
    description:
      'A testimony of rescue: grace that finds the lost, opens blind eyes, and leads us safely home.',
    verse: [
      'Amazing grace! How sweet the sound',
      'That saved a wretch like me!',
      'I once was lost, but now am found;',
      'Was blind, but now I see.'
    ].join('\n')
  },
  {
    id: 'holy-holy-holy',
    title: 'Holy, Holy, Holy',
    theme: 'Praise',
    author: 'Reginald Heber',
    description:
      'A hymn of adoration to God in three Persons, lifting our worship to His holiness and mercy.',
    verse: [
      'Holy, holy, holy! Lord God Almighty!',
      'Early in the morning our song shall rise to Thee;',
      'Holy, holy, holy, merciful and mighty!',
      'God in three Persons, blessed Trinity!'
    ].join('\n')
  },
  {
    id: 'it-is-well',
    title: 'It Is Well with My Soul',
    theme: 'Comfort',
    author: 'Horatio Spafford',
    description:
      'Born out of great personal sorrow, a quiet confession that the soul can rest in God through every storm.',
    verse: [
      'When peace, like a river, attendeth my way,',
      'When sorrows like sea billows roll;',
      'Whatever my lot, Thou hast taught me to say,',
      'It is well, it is well with my soul.'
    ].join('\n')
  },
  {
    id: 'blessed-assurance',
    title: 'Blessed Assurance',
    theme: 'Hope',
    author: 'Fanny Crosby',
    description:
      'A joyful song of confidence in Christ that looks ahead to the glory promised to His people.',
    verse: [
      'Blessed assurance, Jesus is mine!',
      'O what a foretaste of glory divine!',
      'Heir of salvation, purchase of God,',
      'Born of His Spirit, washed in His blood.'
    ].join('\n')
  },
  {
    id: 'what-a-friend',
    title: 'What a Friend We Have in Jesus',
    theme: 'Comfort',
    author: 'Joseph Scriven',
    description:
      'An invitation to carry every burden, sin, and grief to God in prayer, because He is a faithful friend.',
    verse: [
      'What a friend we have in Jesus,',
      'All our sins and griefs to bear!',
      'What a privilege to carry',
      'Everything to God in prayer!',
      'O what peace we often forfeit,',
      'O what needless pain we bear,',
      'All because we do not carry',
      'Everything to God in prayer!'
    ].join('\n')
  },
  {
    id: 'abide-with-me',
    title: 'Abide with Me',
    theme: 'Comfort',
    author: 'Henry Francis Lyte',
    description:
      'An evening prayer for God’s nearness when the light fades and other helpers fail.',
    verse: [
      'Abide with me; fast falls the eventide;',
      'The darkness deepens; Lord, with me abide;',
      'When other helpers fail and comforts flee,',
      'Help of the helpless, O abide with me.'
    ].join('\n')
  }
]);

watch(() => resourceState.data?.hymns, items => {
  if (Array.isArray(items)) songs.splice(0, songs.length, ...items);
}, { immediate: true });
const selectedTheme = ref('All');

const visibleSongs = computed(() =>
  selectedTheme.value === 'All' ? songs : songs.filter((song) => song.theme === selectedTheme.value)
);

const statusText = computed(() => {
  const count = visibleSongs.value.length;
  const noun = count === 1 ? 'song' : 'songs';
  return selectedTheme.value === 'All'
    ? `Showing all ${count} ${noun}.`
    : `Showing ${count} ${noun} about ${selectedTheme.value.toLowerCase()}.`;
});
</script>

<style scoped>
.page-heading .lede {
  margin-inline: auto;
}

.list-title {
  margin: 8px 0 16px;
  text-align: center;
  font-size: 1.5rem;
  letter-spacing: -0.02em;
}

.theme-filter {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 10px;
}

.theme-chip {
  min-height: 44px;
  padding: 0 20px;
  border-radius: 999px;
  border: 1px solid var(--line);
  background: var(--surface);
  color: var(--muted);
  font-weight: 600;
  transition: background 160ms ease, color 160ms ease, border-color 160ms ease;
}

.theme-chip:hover {
  border-color: var(--primary);
  color: var(--primary-strong);
}

.theme-chip.is-active {
  background: var(--primary);
  border-color: var(--primary);
  color: #fff;
}

.theme-chip:focus-visible,
.song-verse summary:focus-visible {
  outline: 3px solid var(--accent);
  outline-offset: 2px;
}

.filter-status {
  margin: 14px 0 0;
  text-align: center;
  color: var(--muted);
  font-size: 0.95rem;
}

.song-card .label {
  margin: 0 0 6px;
  color: var(--primary);
}

.song-meta {
  margin: 0 0 12px;
  font-size: 0.88rem;
  font-weight: 600;
}

.song-verse {
  margin-top: auto;
  padding-top: 16px;
}

.song-verse summary {
  padding: 10px 0;
  cursor: pointer;
  color: var(--primary-strong);
  font-weight: 800;
}

.verse-text {
  margin: 6px 0 0;
  padding: 14px 16px;
  border-left: 3px solid var(--accent);
  border-radius: 0 12px 12px 0;
  background: rgba(178, 138, 72, 0.08);
  color: var(--text);
  white-space: pre-line;
}

.verse-note {
  margin: 8px 0 0;
  font-size: 0.8rem;
}

.closing .lede {
  margin-inline: auto;
}

.scripture {
  max-width: 700px;
  margin: 0 auto 20px;
}

.scripture blockquote {
  margin: 0;
}

.scripture blockquote p {
  margin: 0 0 12px;
  font-size: clamp(1.25rem, 2.4vw, 1.6rem);
  line-height: 1.4;
  letter-spacing: -0.02em;
}

.closing-actions {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 16px;
  margin-top: 28px;
}

@media (min-width: 821px) and (max-width: 1024px) {
  .song-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 820px) {
  .page-section {
    padding-top: 48px;
  }
}
</style>
