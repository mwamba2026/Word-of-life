<template>
  <section class="section">
    <div class="container">
      <div class="section-heading center page-heading">
        <p class="eyebrow">Personal study</p>
        <h1 class="section-title">My notes</h1>
        <p v-if="greetingName" class="greeting">Welcome back, {{ greetingName }}.</p>
        <p class="lede">
          Keep a record of what God is showing you: a verse that stayed with you, a thought from a sermon,
          or a prayer you want to return to.
        </p>
        <p class="storage-note">
          Your notes are saved to your account through the .NET API. Sign in to access them on another device.
        </p>
      </div>

      <div class="notes-layout">
        <div ref="formCard" class="panel-card editor-card">
          <h2 class="panel-title">{{ form.id ? 'Edit note' : 'Write a new note' }}</h2>

          <form novalidate @submit.prevent="saveNote">
            <div class="field">
              <label for="note-title">Title</label>
              <input
                id="note-title"
                ref="titleInput"
                v-model="form.title"
                type="text"
                name="title"
                maxlength="120"
                required
                :aria-invalid="errors.title ? 'true' : 'false'"
                :aria-describedby="errors.title ? 'note-title-error' : undefined"
              />
              <p v-if="errors.title" id="note-title-error" class="field-error">{{ errors.title }}</p>
            </div>

            <div class="field">
              <label for="note-reference">Scripture reference <span class="optional">(optional)</span></label>
              <input
                id="note-reference"
                v-model="form.reference"
                type="text"
                name="reference"
                maxlength="60"
                placeholder="For example, Psalm 23:1"
              />
            </div>

            <div class="field">
              <label for="note-body">Your note</label>
              <textarea
                id="note-body"
                ref="bodyInput"
                v-model="form.body"
                name="body"
                rows="8"
                maxlength="4000"
                required
                placeholder="What is this passage teaching you?"
                :aria-invalid="errors.body ? 'true' : 'false'"
                :aria-describedby="errors.body ? 'note-body-error' : undefined"
              ></textarea>
              <p v-if="errors.body" id="note-body-error" class="field-error">{{ errors.body }}</p>
            </div>

            <div class="form-actions">
              <button class="button button-primary" type="submit" :disabled="saving">
                {{ saving ? 'Saving…' : form.id ? 'Update note' : 'Save note' }}
              </button>
              <button v-if="form.id" class="button button-secondary" type="button" :disabled="saving" @click="cancelEdit">
                Cancel editing
              </button>
            </div>

            <p class="status" :class="formStatus.kind ? `is-${formStatus.kind}` : ''" role="status" aria-live="polite">{{ formStatus.text }}</p>
          </form>
        </div>

        <div class="notes-list-area">
          <div class="list-header">
            <h2 id="notes-list-heading" ref="listHeading" class="panel-title" tabindex="-1">
              Your notes <span v-if="allNotes.length" class="count">({{ allNotes.length }})</span>
            </h2>
          </div>

          <div class="field search-field">
            <label for="notes-search">Search your notes</label>
            <input
              id="notes-search"
              v-model="search"
              type="search"
              name="search"
              placeholder="Title, verse or words"
              :disabled="!allNotes.length"
            />
          </div>

          <p class="status" :class="listStatus.kind ? `is-${listStatus.kind}` : ''" role="status" aria-live="polite">{{ listStatus.text }}</p>

          <p v-if="loading" class="empty-state">Loading your notes…</p>

          <p v-else-if="loadError" class="empty-state is-error">{{ loadError }}</p>

          <div v-else-if="!allNotes.length" class="empty-state">
            <p>You have not written any notes yet.</p>
            <p>Start with a verse that spoke to you today, or something you want to remember from a sermon.</p>
          </div>

          <div v-else-if="!filteredNotes.length" class="empty-state">
            <p>No notes match “{{ search.trim() }}”.</p>
            <button class="button button-secondary" type="button" @click="search = ''">Clear search</button>
          </div>

          <template v-else>
            <p v-if="search.trim()" class="result-count">
              Showing {{ filteredNotes.length }} of {{ allNotes.length }} {{ allNotes.length === 1 ? 'note' : 'notes' }}
            </p>

            <ul class="notes-list" aria-labelledby="notes-list-heading">
              <li v-for="note in filteredNotes" :key="note.id">
                <article
                  class="note-card"
                  :class="{ 'is-editing': form.id === note.id }"
                  :aria-labelledby="`note-title-${note.id}`"
                >
                  <p v-if="note.reference" class="reading-reference">{{ note.reference }}</p>
                  <h3 :id="`note-title-${note.id}`">{{ note.title }}</h3>
                  <p class="note-body">{{ note.body }}</p>
                  <p class="note-meta">
                    <time :datetime="note.createdAt">{{ formatDate(note.createdAt) }}</time>
                    <template v-if="note.updatedAt && note.updatedAt !== note.createdAt">
                      · edited <time :datetime="note.updatedAt">{{ formatDate(note.updatedAt) }}</time>
                    </template>
                    <span v-if="form.id === note.id" class="editing-tag">Editing now</span>
                  </p>

                  <div v-if="confirmingId !== note.id" class="note-actions">
                    <button :id="`note-edit-${note.id}`" class="small-button" type="button" @click="startEdit(note)">
                      Edit<span class="visually-hidden"> “{{ note.title }}”</span>
                    </button>
                    <button :id="`note-delete-${note.id}`" class="small-button danger" type="button" @click="askDelete(note)">
                      Delete<span class="visually-hidden"> “{{ note.title }}”</span>
                    </button>
                  </div>

                  <div v-else class="confirm-box" role="group" :aria-labelledby="`note-confirm-${note.id}`">
                    <p :id="`note-confirm-${note.id}`">Delete “{{ note.title }}”? This cannot be undone.</p>
                    <div class="note-actions">
                      <button class="small-button danger-solid" type="button" :disabled="deletingId === note.id" @click="confirmDelete(note)">
                        {{ deletingId === note.id ? 'Deleting…' : 'Yes, delete' }}
                      </button>
                      <button :id="`note-keep-${note.id}`" class="small-button" type="button" :disabled="deletingId === note.id" @click="cancelDelete(note)">
                        Keep it
                      </button>
                    </div>
                  </div>
                </article>
              </li>
            </ul>
          </template>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed, nextTick, onMounted, reactive, ref } from 'vue';
import { auth, notes as notesApi } from '../api.js';

const user = auth.currentUser();
const greetingName = (user?.fullName?.trim().split(/\s+/)[0]) || user?.email || '';

const allNotes = ref([]);
const loading = ref(true);
const loadError = ref('');
const search = ref('');

const form = reactive({ id: null, title: '', reference: '', body: '' });
const errors = reactive({ title: '', body: '' });
const formStatus = reactive({ kind: '', text: '' });
const listStatus = reactive({ kind: '', text: '' });
const saving = ref(false);
const confirmingId = ref(null);
const deletingId = ref(null);

const formCard = ref(null);
const titleInput = ref(null);
const bodyInput = ref(null);
const listHeading = ref(null);

// Snapshot of the form when it was last loaded or cleared, used to warn before discarding unsaved text.
let formSnapshot = JSON.stringify({ title: '', reference: '', body: '' });

const dateFormat = new Intl.DateTimeFormat(undefined, { dateStyle: 'medium', timeStyle: 'short' });

const sortedNotes = computed(() =>
  [...allNotes.value].sort((a, b) => String(b.createdAt || '').localeCompare(String(a.createdAt || '')))
);

const filteredNotes = computed(() => {
  const query = search.value.trim().toLowerCase();
  if (!query) return sortedNotes.value;
  return sortedNotes.value.filter((note) =>
    [note.title, note.reference, note.body].some((value) => String(value || '').toLowerCase().includes(query))
  );
});

function formatDate(iso) {
  const date = new Date(iso);
  return Number.isNaN(date.getTime()) ? '' : dateFormat.format(date);
}

function setStatus(target, kind, text) {
  target.kind = kind;
  target.text = text;
}

function currentFormText() {
  return JSON.stringify({ title: form.title, reference: form.reference, body: form.body });
}

function hasUnsavedText() {
  return currentFormText() !== formSnapshot && Boolean(form.title.trim() || form.body.trim() || form.reference.trim());
}

function fillForm(note) {
  form.id = note?.id ?? null;
  form.title = note?.title ?? '';
  form.reference = note?.reference ?? '';
  form.body = note?.body ?? '';
  errors.title = '';
  errors.body = '';
  formSnapshot = currentFormText();
}

async function fetchNotes() {
  const result = await notesApi.list();
  return Array.isArray(result) ? result : [];
}

async function loadNotes() {
  loading.value = true;
  loadError.value = '';
  try {
    allNotes.value = await fetchNotes();
  } catch (error) {
    loadError.value = error?.message || 'We could not load your notes. Please refresh the page.';
  } finally {
    loading.value = false;
  }
}

onMounted(loadNotes);

async function saveNote() {
  if (saving.value) return;
  setStatus(formStatus, '', '');

  errors.title = form.title.trim() ? '' : 'Please give your note a title.';
  errors.body = form.body.trim() ? '' : 'Please write something in your note.';
  if (errors.title || errors.body) {
    setStatus(formStatus, 'error', 'Please fill in the highlighted fields.');
    (errors.title ? titleInput : bodyInput).value?.focus();
    return;
  }

  const editing = Boolean(form.id);
  const payload = { title: form.title.trim(), reference: form.reference.trim(), body: form.body.trim() };
  if (editing) payload.id = form.id;

  saving.value = true;
  try {
    const saved = await notesApi.save(payload);
    const latest = await fetchNotes();
    allNotes.value = latest;

    fillForm(null);
    setStatus(formStatus, 'success', editing ? 'Your note has been updated.' : 'Your note has been saved.');
  } catch (error) {
    setStatus(formStatus, 'error', error?.message || 'We could not save your note. Please try again.');
  } finally {
    saving.value = false;
  }
}

async function startEdit(note) {
  if (form.id === note.id) {
    titleInput.value?.focus();
    return;
  }
  if (hasUnsavedText() && !window.confirm('You have unsaved text in the note form. Replace it with this note?')) {
    return;
  }
  confirmingId.value = null;
  fillForm(note);
  setStatus(formStatus, 'info', `Editing “${note.title}”.`);
  await nextTick();
  scrollToForm();
  titleInput.value?.focus({ preventScroll: true });
}

// Scroll the editor into view below the sticky site header (which is taller on phones).
function scrollToForm() {
  if (!formCard.value) return;
  const header = document.querySelector('.site-header');
  const offset = (header ? header.getBoundingClientRect().height : 0) + 16;
  const top = formCard.value.getBoundingClientRect().top + window.scrollY - offset;
  window.scrollTo({ top: Math.max(top, 0) });
}

async function cancelEdit() {
  const editedId = form.id;
  fillForm(null);
  setStatus(formStatus, 'info', 'Editing cancelled. Your note was not changed.');
  await nextTick();
  const editButton = editedId ? document.getElementById(`note-edit-${editedId}`) : null;
  (editButton || titleInput.value)?.focus();
}

async function askDelete(note) {
  confirmingId.value = note.id;
  setStatus(listStatus, '', '');
  await nextTick();
  document.getElementById(`note-keep-${note.id}`)?.focus();
}

async function cancelDelete(note) {
  confirmingId.value = null;
  await nextTick();
  document.getElementById(`note-delete-${note.id}`)?.focus();
}

async function confirmDelete(note) {
  if (deletingId.value) return;
  deletingId.value = note.id;
  try {
    await notesApi.remove(note.id);
    allNotes.value = await fetchNotes();
    if (form.id === note.id) fillForm(null);
    confirmingId.value = null;
    setStatus(listStatus, 'success', `“${note.title}” was deleted.`);
    await nextTick();
    listHeading.value?.focus();
  } catch (error) {
    setStatus(listStatus, 'error', error?.message || 'We could not delete that note. Please try again.');
  } finally {
    deletingId.value = null;
  }
}
</script>

<style scoped>
.page-heading .lede {
  margin: 16px auto 0;
}

.greeting {
  margin: 18px 0 0;
  font-size: 1.2rem;
  font-weight: 700;
  color: var(--primary-strong);
}

.storage-note {
  max-width: 620px;
  margin: 20px auto 0;
  padding: 12px 18px;
  border-radius: 16px;
  background: rgba(49, 95, 125, 0.08);
  border: 1px solid rgba(49, 95, 125, 0.16);
  color: var(--text);
  font-size: 0.95rem;
}

.notes-layout {
  --danger: #a3342a;
  display: grid;
  grid-template-columns: minmax(0, 5fr) minmax(0, 7fr);
  gap: 32px;
  align-items: start;
}

.panel-title {
  margin: 0 0 18px;
  font-size: 1.45rem;
  letter-spacing: -0.03em;
}

.panel-title:focus {
  outline: none;
}

.count {
  color: var(--muted);
  font-weight: 600;
}

.field {
  display: grid;
  gap: 6px;
  margin-bottom: 18px;
}

.field label {
  font-weight: 600;
}

.optional {
  color: var(--muted);
  font-weight: 400;
}

.field input,
.field textarea {
  width: 100%;
  min-height: 48px;
  padding: 10px 14px;
  border: 1px solid var(--muted);
  border-color: color-mix(in srgb, var(--muted) 75%, var(--surface));
  border-radius: 14px;
  background: var(--surface);
  color: var(--text);
  font: inherit;
}

.field textarea {
  min-height: 180px;
  resize: vertical;
  line-height: 1.6;
}

.field input:focus-visible,
.field textarea:focus-visible {
  outline: 3px solid rgba(49, 95, 125, 0.35);
  outline-offset: 1px;
  border-color: var(--primary);
}

.field input:disabled {
  opacity: 0.6;
}

.field [aria-invalid='true'] {
  border-color: var(--danger);
}

.field-error {
  margin: 0;
  color: var(--danger);
  font-size: 0.92rem;
}

.form-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.button:disabled {
  opacity: 0.65;
  cursor: not-allowed;
  box-shadow: none;
}

.status {
  margin: 16px 0 0;
  font-weight: 600;
}

.status:empty {
  margin: 0;
}

.status.is-error {
  color: var(--danger);
}

.status.is-success,
.status.is-info {
  color: var(--primary-strong);
}

.notes-list-area .status {
  margin: 0 0 16px;
}

.notes-list-area .status:empty {
  margin: 0;
}

.search-field {
  margin-bottom: 16px;
}

.empty-state {
  margin: 0;
  padding: 28px 24px;
  border: 1px dashed color-mix(in srgb, var(--muted) 45%, transparent);
  border-radius: 22px;
  color: var(--muted);
  text-align: center;
}

.empty-state p {
  margin: 0 0 8px;
}

.empty-state .button {
  margin-top: 8px;
}

.empty-state.is-error {
  color: var(--danger);
}

.result-count {
  margin: 0 0 12px;
  color: var(--muted);
  font-size: 0.92rem;
}

.notes-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: grid;
  gap: 18px;
}

.note-card {
  background: var(--surface);
  border: 1px solid var(--line);
  border-radius: 22px;
  box-shadow: 0 14px 30px rgba(14, 22, 24, 0.05);
  padding: 22px 22px 18px;
}

.note-card.is-editing {
  border-color: var(--primary);
  box-shadow: 0 0 0 2px rgba(49, 95, 125, 0.18);
}

.note-card h3 {
  margin: 0 0 8px;
  font-size: 1.25rem;
  overflow-wrap: anywhere;
}

.note-card .reading-reference {
  overflow-wrap: anywhere;
}

.note-body {
  margin: 0 0 12px;
  color: var(--text);
  white-space: pre-wrap;
  overflow-wrap: anywhere;
}

.note-meta {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 4px 6px;
  margin: 0 0 14px;
  color: var(--muted);
  font-size: 0.88rem;
}

.editing-tag {
  margin-left: 6px;
  padding: 2px 10px;
  border-radius: 999px;
  background: rgba(178, 138, 72, 0.22);
  color: var(--text);
  font-weight: 600;
}

.note-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.small-button {
  min-height: 40px;
  padding: 0 16px;
  border-radius: 999px;
  border: 1px solid rgba(49, 95, 125, 0.3);
  background: transparent;
  color: var(--primary-strong);
  font-weight: 700;
}

.small-button:hover {
  background: rgba(49, 95, 125, 0.08);
}

.small-button:focus-visible {
  outline: 3px solid rgba(49, 95, 125, 0.35);
  outline-offset: 2px;
}

.small-button.danger {
  color: var(--danger);
  border-color: rgba(163, 52, 42, 0.35);
}

.small-button.danger:hover {
  background: rgba(163, 52, 42, 0.08);
}

.small-button.danger-solid {
  background: var(--danger);
  border-color: var(--danger);
  color: white;
}

.small-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.confirm-box {
  padding: 14px 16px;
  border-radius: 16px;
  background: rgba(163, 52, 42, 0.07);
}

.confirm-box p {
  margin: 0 0 10px;
  font-weight: 600;
  overflow-wrap: anywhere;
}

.visually-hidden {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}

@media (max-width: 820px) {
  .notes-layout {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 480px) {
  .editor-card {
    padding: 20px 16px;
  }

  .note-card {
    padding: 18px 16px 16px;
  }

  .form-actions .button {
    flex: 1 1 100%;
  }
}
</style>
