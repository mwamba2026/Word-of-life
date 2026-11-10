<template>
  <section class="section"><div class="container staff">
    <div class="section-heading"><p class="eyebrow">Staff tools</p><h1 class="section-title">Manage content</h1>
      <p class="lede">Publish readings, teaching, audiobooks, podcasts, hymns, and reflections. Upload your own audio or video recordings, or link to an original publisher.</p></div>

    <div v-if="!isAdmin" class="panel-card">
      <h2>Admin access required</h2><p>Sign in with the designated staff account. The first administrator also needs the one-time bootstrap key configured on the server.</p>
      <div class="bootstrap-row"><input v-model="bootstrapKey" type="password" autocomplete="off" aria-label="Admin bootstrap key" placeholder="One-time bootstrap key" />
        <button class="button button-primary" type="button" @click="bootstrap">Activate admin</button></div>
      <p v-if="status" role="status">{{ status }}</p>
      <RouterLink class="text-link" to="/login">Sign in <span aria-hidden="true">→</span></RouterLink>
    </div>

    <template v-else>
      <form class="panel-card editor" @submit.prevent="save">
        <div class="form-top"><h2>{{ editingId ? 'Edit item' : 'Add an item' }}</h2><button v-if="editingId" type="button" class="button button-secondary" @click="reset">Cancel edit</button></div>
        <div class="fields">
          <label>Section <select v-model="form.category" required><option v-for="category in categories" :key="category" :value="category">{{ category }}</option></select></label>
          <label>Title <input v-model.trim="form.title" required maxlength="200" /></label>
          <label v-if="usesUrl">Publisher URL {{ form.category === 'guides' ? '' : '(optional when uploading)' }} <input v-model.trim="form.url" type="url" placeholder="https://..." :required="form.category === 'guides'" /></label>
          <label v-if="usesMedia" class="wide">Upload your own recording (MP3/M4A up to 50 MB; MP4/WebM up to 250 MB)
            <input ref="fileInput" type="file" :accept="form.category === 'audiobooks' ? '.mp3,.m4a' : '.mp3,.m4a,.mp4,.webm'" @change="selectFile" />
            <small v-if="form.mediaUrl">Current file: {{ form.mediaUrl }}</small>
          </label>
          <label v-if="form.category === 'teachings'">Presenter or publisher <input v-model.trim="form.credit" required /></label>
          <label v-if="form.category === 'teachings'">Topic <input v-model.trim="form.topic" /></label>
          <label v-if="form.category === 'teachings' || form.category === 'podcasts'">Series <input v-model.trim="form.series" /></label>
          <label v-if="form.category === 'podcasts'">Host or speaker <input v-model.trim="form.host" /></label>
          <label v-if="form.category === 'guides'">Lesson label <input v-model.trim="form.meta" placeholder="Lesson 4" /></label>
          <label v-if="form.category === 'hymns'">Theme <input v-model.trim="form.theme" placeholder="Hope" /></label>
          <label v-if="form.category === 'hymns'">Words by <input v-model.trim="form.author" /></label>
          <label v-if="form.category === 'readings' || form.category === 'reflections'">Scripture reference <input v-model.trim="form.reference" required /></label>
          <label v-if="form.category === 'readings'">Reading theme <input v-model.trim="form.theme" required /></label>
          <label v-if="form.category !== 'readings'">Short description <textarea v-model.trim="form.description" rows="3" required></textarea></label>
          <label v-if="form.category === 'readings'">Passage, one verse per line <textarea v-model.trim="form.body" rows="7" placeholder="1 In the beginning..." required></textarea></label>
          <label v-if="form.category === 'hymns'">Public-domain verse <textarea v-model.trim="form.body" rows="6" required></textarea></label>
          <label v-if="form.category === 'reflections'">Reflection paragraphs <textarea v-model.trim="form.body" rows="8" required></textarea></label>
          <label v-if="form.category === 'readings'">Reflection question <input v-model.trim="form.question" required /></label>
          <label v-if="form.category === 'reflections'">Try this today <input v-model.trim="form.question" required /></label>
          <label class="check"><input v-model="form.isPublished" type="checkbox" /> Publish this item</label>
        </div>
        <div class="form-actions"><button class="button button-primary" type="submit" :disabled="saving">{{ saving ? 'Saving…' : 'Save content' }}</button><p role="status">{{ status }}</p></div>
      </form>
      <div class="panel-card items"><h2>Saved items</h2><p v-if="loading">Loading…</p>
        <p v-else-if="!items.length">No staff-added content yet.</p>
        <ul v-else><li v-for="item in items" :key="item.id"><span><strong>{{ item.title }}</strong><small>{{ item.category }} · {{ item.isPublished ? 'Published' : 'Draft' }}</small></span>
          <span class="item-actions"><button type="button" @click="edit(item)">Edit</button><button type="button" @click="remove(item)">Delete</button></span>
        </li></ul>
      </div>
    </template>
  </div></section>
</template>
<script setup>
import { computed, onMounted, reactive, ref } from 'vue';
import { auth, content, media } from '../api.js';
import { loadResources } from '../resourceState.js';

const categories = ['readings', 'teachings', 'audiobooks', 'podcasts', 'guides', 'hymns', 'reflections'];
const isAdmin = ref(Boolean(auth.currentUser()?.isAdmin));
const bootstrapKey = ref('');
const status = ref('');
const loading = ref(false);
const saving = ref(false);
const items = ref([]);
const editingId = ref(null);
const empty = () => ({ category: 'teachings', title: '', url: '', mediaUrl: '', mediaType: '', host: '', credit: '', topic: '', series: '', meta: '', theme: '', author: '', reference: '', description: '', body: '', question: '', isPublished: false });
const fileInput = ref(null);
const selectedFile = ref(null);
function selectFile(event) { selectedFile.value = event.target.files?.[0] || null; }
const form = reactive(empty());
const usesUrl = computed(() => ['teachings', 'audiobooks', 'podcasts', 'guides'].includes(form.category));
const usesMedia = computed(() => ['teachings', 'audiobooks', 'podcasts'].includes(form.category));
const slug = text => String(text).toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '') || 'item';

async function list() {
  loading.value = true;
  try { items.value = await content.listAdmin(); }
  catch (error) { status.value = error?.message || 'Could not load staff items.'; }
  finally { loading.value = false; }
}
onMounted(async () => {
  try { isAdmin.value = Boolean((await auth.me())?.isAdmin); }
  catch { isAdmin.value = false; }
  if (isAdmin.value) await list();
});
async function bootstrap() {
  try {
    await auth.bootstrapAdmin(bootstrapKey.value);
    bootstrapKey.value = '';
    isAdmin.value = Boolean((await auth.me())?.isAdmin);
    status.value = isAdmin.value ? 'Admin access activated.' : 'Please sign in again.';
    if (isAdmin.value) await list();
  } catch (error) { status.value = error?.message || 'Admin setup failed.'; }
}
function resource() {
  const common = { title: form.title };
  switch (form.category) {
    case 'readings': return { id: slug(form.title), reference: form.reference, theme: form.theme, verses: form.body.split(/\n+/).filter(Boolean).map((line, index) => { const match = line.match(/^(\d+)\s+(.+)$/); return { n: match ? Number(match[1]) : index + 1, text: match ? match[2] : line }; }), question: form.question };
    case 'teachings': return { ...common, credit: form.credit, topic: form.topic, series: form.series, summary: form.description, url: form.url, mediaUrl: form.mediaUrl, mediaType: form.mediaType };
    case 'audiobooks': return { ...common, description: form.description, url: form.url, mediaUrl: form.mediaUrl, mediaType: form.mediaType };
    case 'podcasts': return { ...common, series: form.series, host: form.host, description: form.description, url: form.url, mediaUrl: form.mediaUrl, mediaType: form.mediaType };
    case 'guides': return { ...common, lesson: form.meta, description: form.description, url: form.url };
    case 'hymns': return { ...common, id: slug(form.title), theme: form.theme, author: form.author, description: form.description, verse: form.body };
    case 'reflections': return { ...common, id: slug(form.title), reference: form.reference, excerpt: form.description, body: form.body.split(/\n\s*\n/).filter(Boolean), tryThis: form.question };
  }
}
async function save() {
  saving.value = true; status.value = '';
  try {
    if (form.url && !/^https:\/\//i.test(form.url)) throw new Error('Use an HTTPS URL from the original publisher.');
    if (form.category === 'guides' && !form.url) throw new Error('Study guides need a publisher URL.');
    if (selectedFile.value) {
      const extension = selectedFile.value.name.toLowerCase().split('.').pop();
      if (!['mp3', 'm4a', 'mp4', 'webm'].includes(extension)) throw new Error('Use MP3, M4A, MP4, or WebM.');
      if (form.category === 'audiobooks' && !['mp3', 'm4a'].includes(extension)) throw new Error('Audiobooks need an audio file.');
      status.value = 'Uploading recording...';
      const uploaded = await media.upload(selectedFile.value);
      form.mediaUrl = uploaded.url; form.mediaType = uploaded.type;
    }
    if (usesMedia.value && !form.url && !form.mediaUrl) throw new Error('Add a publisher link or upload a recording.');
    const item = { title: form.title, category: form.category, body: JSON.stringify(resource()), isPublished: form.isPublished };
    if (editingId.value) await content.update(editingId.value, item); else await content.create(item);
    status.value = 'Content saved.'; reset(); await list(); await loadResources();
  } catch (error) { status.value = error?.message || 'Could not save content.'; }
  finally { saving.value = false; }
}
function edit(item) {
  let data; try { data = JSON.parse(item.body); } catch { status.value = 'This item cannot be edited here.'; return; }
  Object.assign(form, empty(), { category: item.category, title: item.title, url: data.url || '', mediaUrl: data.mediaUrl || '', mediaType: data.mediaType || '', host: data.host || '', credit: data.credit || '', topic: data.topic || '', series: data.series || '', meta: data.lesson || '', theme: data.theme || '', author: data.author || '', reference: data.reference || '', description: data.summary || data.description || data.excerpt || '', body: item.category === 'readings' ? (data.verses || []).map(v => `${v.n} ${v.text}`).join('\n') : item.category === 'reflections' ? (data.body || []).join('\n\n') : data.verse || '', question: data.question || data.tryThis || '', isPublished: item.isPublished });
  editingId.value = item.id; window.scrollTo({ top: 0, behavior: 'smooth' });
}
function reset() { Object.assign(form, empty()); editingId.value = null; selectedFile.value = null; if (fileInput.value) fileInput.value.value = ''; }
async function remove(item) {
  if (!window.confirm(`Delete "${item.title}"?`)) return;
  try { await content.remove(item.id); status.value = 'Content deleted.'; await list(); await loadResources(); }
  catch (error) { status.value = error?.message || 'Could not delete content.'; }
}
</script>
<style scoped>
.staff { max-width: 900px; }.section-title { margin-inline: 0; }
.editor, .items { margin-top: 24px; }.form-top, .form-actions, .bootstrap-row { display: flex; align-items: center; gap: 16px; flex-wrap: wrap; }
.fields { display: grid; grid-template-columns: repeat(2,minmax(0,1fr)); gap: 16px; }
.fields label { display: grid; gap: 6px; font-weight: 700; }.fields label:has(textarea), .fields .check, .fields .wide { grid-column: 1 / -1; }
input, textarea, select { width: 100%; padding: 12px; border: 1px solid #a9beca; border-radius: 10px; font: inherit; }
.check { display: flex !important; align-items: center; }.check input { width: auto; }
.form-actions { margin-top: 20px; }.items ul { list-style: none; padding: 0; }
.items li { display: flex; justify-content: space-between; gap: 16px; padding: 12px 0; border-bottom: 1px solid var(--line); }
.items small { display: block; color: var(--muted); }.item-actions { display: flex; gap: 10px; }
.item-actions button { background: #e7f1f6; color: var(--primary-strong); padding: 8px 12px; border-radius: 8px; }
@media (max-width: 650px) { .fields { grid-template-columns: 1fr; } .items li { display: block; } .item-actions { margin-top: 10px; } }
</style>
