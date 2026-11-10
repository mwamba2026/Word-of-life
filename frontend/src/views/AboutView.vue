<template>
  <section class="section"><div class="container about-layout">
    <div><p class="eyebrow">About this space</p><h1 class="section-title">{{ mission }}</h1>
      <p class="lede">{{ siteName }} brings together a Bible reading plan, short reflections, public-domain hymns, and links to teaching and audiobooks from their original publishers.</p>
      <p>You can use the reading and reflection pages without creating an account.</p>
    </div>
    <aside class="panel-card peace-card"><p class="label">What you will find here</p>
      <ul><li>Seven KJV passages in a repeating daily cycle</li><li>Short reflections and a private notes area</li><li>Public-domain hymns for reading and singing</li><li>Clearly attributed links to external teaching</li></ul>
    </aside>
  </div></section>
  <section class="section section-alt"><div class="container center"><p class="eyebrow">A faithful next step</p><h2 class="section-title">Begin where you are.</h2>
    <p class="lede centered">Read, pray, and return whenever you need a quiet moment.</p><RouterLink class="button button-primary" to="/bible">Start reading</RouterLink>
  </div></section>
</template>
<script setup>
import { computed } from 'vue';
import { siteName } from '../config.js';
import { resourceState } from '../resourceState.js';
const mission = computed(() => resourceState.data?.site?.mission || 'A quiet place for Scripture, reflection, and prayer.');
</script>
<style scoped>
.about-layout { display: grid; grid-template-columns: 1.2fr .8fr; gap: 48px; align-items: center; }
.peace-card { background: #fff; border-top: 4px solid var(--peace); }
.peace-card ul { padding-left: 20px; line-height: 1.8; }
.centered { margin: 18px auto 26px; }
@media (max-width: 760px) { .about-layout { grid-template-columns: 1fr; gap: 28px; } }
</style>
