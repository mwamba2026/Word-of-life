<template>
  <section class="section">
    <div class="container">
      <div class="section-heading center page-heading">
        <p class="eyebrow">Prayer and support</p>
        <h1 class="section-title">Share a prayer request.</h1>
        <p class="lede">
          You do not have to carry it alone. Send a request through this form.
          Messages are saved to the site database. This is not live chat; replies, if available, can take time.
        </p>
      </div>

      <aside class="panel-card card-highlight care-note" aria-labelledby="care-note-title">
        <span class="care-icon" aria-hidden="true">♡</span>
        <div>
          <h2 id="care-note-title" class="care-title">If you are struggling right now</h2>
          <p>
            Please know that you matter, to God and to us. A message here can take a little
            time to reach someone, so please also talk to a person you trust nearby: a friend,
            a family member, a church leader or a doctor. If you or someone else is in
            immediate danger, contact your local emergency services straight away.
          </p>
        </div>
      </aside>

      <div class="support-grid">
        <form class="panel-card support-form" novalidate @submit.prevent="submit">
          <div>
            <h2 class="form-title">Send a request</h2>
            <p class="form-hint">All fields are required unless marked optional.</p>
          </div>

          <div class="field">
            <label for="support-name">Your name</label>
            <input
              id="support-name"
              v-model="form.name"
              type="text"
              name="name"
              autocomplete="name"
              required
              :aria-invalid="errors.name ? 'true' : 'false'"
              :aria-describedby="errors.name ? 'support-name-error' : undefined"
            />
            <p v-if="errors.name" id="support-name-error" class="field-error">{{ errors.name }}</p>
          </div>

          <div class="field">
            <label for="support-email">Email address</label>
            <input
              id="support-email"
              v-model="form.email"
              type="email"
              name="email"
              autocomplete="email"
              inputmode="email"
              required
              :aria-invalid="errors.email ? 'true' : 'false'"
              :aria-describedby="errors.email ? 'support-email-error' : undefined"
            />
            <p v-if="errors.email" id="support-email-error" class="field-error">{{ errors.email }}</p>
          </div>

          <div class="field">
            <label for="support-phone">Phone number <span class="optional">(optional)</span></label>
            <input
              id="support-phone"
              v-model="form.phone"
              type="tel"
              name="phone"
              autocomplete="tel"
              :aria-invalid="errors.phone ? 'true' : 'false'"
              :aria-describedby="errors.phone ? 'support-phone-error support-phone-hint' : 'support-phone-hint'"
            />
            <p id="support-phone-hint" class="field-hint">Add this only if you would like us to call you.</p>
            <p v-if="errors.phone" id="support-phone-error" class="field-error">{{ errors.phone }}</p>
          </div>

          <div class="field">
            <label for="support-topic">How can we help?</label>
            <select id="support-topic" v-model="form.topic" name="topic" required>
              <option v-for="topic in topics" :key="topic" :value="topic">{{ topic }}</option>
            </select>
          </div>

          <div class="field">
            <label for="support-message">Your message</label>
            <textarea
              id="support-message"
              v-model="form.message"
              name="message"
              rows="6"
              required
              :aria-invalid="errors.message ? 'true' : 'false'"
              :aria-describedby="errors.message ? 'support-message-error' : undefined"
            ></textarea>
            <p v-if="errors.message" id="support-message-error" class="field-error">{{ errors.message }}</p>
          </div>

          <div class="form-actions">
            <button class="button button-primary" type="submit" :disabled="sending">
              {{ sending ? 'Sending…' : 'Send request' }}
            </button>
          </div>

          <p class="status" :class="status.type" aria-live="polite">{{ status.text }}</p>
        </form>

        <div class="support-aside">
          <article class="info-card">
            <div class="icon" aria-hidden="true">✦</div>
            <h2>How your request is handled</h2>
            <p>
              Your request and contact details are stored in the site database and are not
              displayed publicly. There is no automatic email alert or guaranteed reply yet.
              Please avoid sharing information you would not want stored.
            </p>
          </article>

          <figure class="info-card verse-card">
            <p class="label">A word for today</p>
            <blockquote>
              <p>“Casting all your care upon him; for he careth for you.”</p>
            </blockquote>
            <figcaption class="reading-reference">1 Peter 5:7 (KJV)</figcaption>
          </figure>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { reactive, ref, nextTick } from 'vue';
import { siteName } from '../config.js';
import { auth, messages } from '../api.js';

const topics = ['Prayer request', 'Question', 'Other'];
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const PHONE_PATTERN = /^\+?[\d\s()-]{7,20}$/;
const FIELD_ORDER = ['name', 'email', 'phone', 'message'];

// If the visitor is signed in, start with their name and email filled in.
const user = auth.currentUser();

const form = reactive({
  name: user?.fullName ?? '',
  email: user?.email ?? '',
  phone: '',
  topic: topics[0],
  message: ''
});

const errors = reactive({ name: '', email: '', phone: '', message: '' });
const status = reactive({ type: '', text: '' });
const sending = ref(false);

function validate() {
  const name = form.name.trim();
  const email = form.email.trim();
  const phone = form.phone.trim();

  errors.name = name ? '' : 'Please tell us your name.';

  if (!email) {
    errors.email = 'Please enter your email so we can reply.';
  } else if (!EMAIL_PATTERN.test(email)) {
    errors.email = 'Please enter a valid email address, for example name@example.com.';
  } else {
    errors.email = '';
  }

  errors.phone = !phone || PHONE_PATTERN.test(phone)
    ? ''
    : 'Please use digits only (you may start with + and your country code).';

  errors.message = form.message.trim() ? '' : 'Please share a few words about how we can help.';

  return FIELD_ORDER.every((field) => !errors[field]);
}

async function focusFirstError() {
  await nextTick();
  const field = FIELD_ORDER.find((name) => errors[name]);
  if (field) document.getElementById(`support-${field}`)?.focus();
}

async function submit() {
  if (sending.value) return;
  status.type = '';
  status.text = '';

  if (!validate()) {
    status.type = 'error';
    status.text = 'Please check the highlighted fields and try again.';
    focusFirstError();
    return;
  }

  sending.value = true;
  try {
    await messages.sendSupportRequest({
      name: form.name.trim(),
      email: form.email.trim(),
      phone: form.phone.trim() || null,
      topic: form.topic,
      message: form.message.trim()
    });
    status.type = 'success';
    status.text = 'Your request was saved. Thank you for sharing it. A personal reply is not guaranteed.';
    form.message = '';
  } catch (error) {
    status.type = 'error';
    status.text = error?.message || 'Something went wrong while sending. Please try again.';
  } finally {
    sending.value = false;
  }
}
</script>

<style scoped>
.page-heading .lede {
  margin: 18px auto 0;
}

.care-note {
  display: flex;
  align-items: flex-start;
  gap: 16px;
  max-width: 860px;
  margin: 0 auto 32px;
  box-shadow: none;
}

.care-icon {
  flex: none;
  width: 44px;
  height: 44px;
  border-radius: 14px;
  display: grid;
  place-items: center;
  background: var(--surface);
  color: var(--primary);
  font-size: 1.4rem;
}

.care-title {
  margin: 0 0 6px;
  font-size: 1.15rem;
}

.care-note p {
  margin: 0;
  color: var(--text);
}

.support-grid {
  display: grid;
  grid-template-columns: minmax(0, 1.25fr) minmax(0, 0.75fr);
  gap: 28px;
  align-items: start;
}

.support-form {
  --error: #a63d2f;
  display: grid;
  gap: 18px;
}

.form-title {
  margin: 0;
  font-size: 1.5rem;
  letter-spacing: -0.02em;
}

.form-hint,
.field-hint {
  margin: 4px 0 0;
  color: var(--muted);
  font-size: 0.92rem;
}

.field {
  display: grid;
  gap: 6px;
}

.field label {
  font-weight: 700;
}

.optional {
  font-weight: 400;
  color: var(--muted);
}

.field input,
.field select,
.field textarea {
  width: 100%;
  min-height: 48px;
  padding: 12px 14px;
  font: inherit;
  color: var(--text);
  background: var(--surface);
  border: 1px solid var(--muted);
  border-color: color-mix(in srgb, var(--muted) 75%, var(--surface));
  border-radius: 14px;
}

.field textarea {
  min-height: 160px;
  resize: vertical;
}

.field input:focus-visible,
.field select:focus-visible,
.field textarea:focus-visible {
  outline: 2px solid var(--primary);
  outline-offset: 1px;
  border-color: var(--primary);
}

.field [aria-invalid='true'] {
  border-color: var(--error);
}

.field-error {
  margin: 0;
  color: var(--error);
  font-size: 0.92rem;
  font-weight: 600;
}

.form-actions .button:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}

.status {
  margin: 0;
}

.status:not(:empty) {
  padding: 12px 16px;
  border-radius: 14px;
  font-weight: 600;
}

.status.success {
  background: rgba(49, 95, 125, 0.1);
  color: var(--primary-strong);
}

.status.error {
  background: rgba(166, 61, 47, 0.08);
  color: var(--error);
}

.support-aside {
  display: grid;
  gap: 20px;
}

.info-card h2 {
  margin: 0 0 10px;
  font-size: 1.3rem;
}

.verse-card {
  margin: 0;
}

.verse-card .label {
  margin-bottom: 8px;
  color: var(--primary);
}

.verse-card blockquote {
  margin: 0 0 12px;
}

.verse-card blockquote p {
  color: var(--text);
  font-size: 1.15rem;
  font-style: italic;
}

.verse-card .reading-reference {
  margin: 0;
}

@media (max-width: 820px) {
  .support-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 480px) {
  .panel-card {
    padding: 20px;
  }

  .care-note {
    flex-direction: column;
    gap: 12px;
  }

  .form-actions .button {
    width: 100%;
  }
}
</style>
