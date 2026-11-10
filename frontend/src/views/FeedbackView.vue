<template>
  <section class="section">
    <div class="container">
      <div class="section-heading center page-heading">
        <p class="eyebrow">Feedback</p>
        <h1 class="section-title">Help us make this a better place to visit.</h1>
        <p class="lede">
          Your thoughts help us improve {{ siteName }}: what to fix, what to make clearer
          and what to add next. Your name and email are optional, so you are welcome to stay anonymous.
        </p>
      </div>

      <div class="feedback-grid">
        <form class="panel-card feedback-form" novalidate @submit.prevent="submit">
          <div>
            <h2 class="form-title">Share your feedback</h2>
            <p class="form-hint">Only the message is required.</p>
          </div>

          <div class="field">
            <label for="feedback-name">Your name <span class="optional">(optional)</span></label>
            <input
              id="feedback-name"
              v-model="form.name"
              type="text"
              name="name"
              autocomplete="name"
            />
          </div>

          <div class="field">
            <label for="feedback-email">Email address <span class="optional">(optional)</span></label>
            <input
              id="feedback-email"
              v-model="form.email"
              type="email"
              name="email"
              autocomplete="email"
              inputmode="email"
              :aria-invalid="errors.email ? 'true' : 'false'"
              :aria-describedby="errors.email ? 'feedback-email-error feedback-email-hint' : 'feedback-email-hint'"
            />
            <p id="feedback-email-hint" class="field-hint">Add this only if you would like a reply.</p>
            <p v-if="errors.email" id="feedback-email-error" class="field-error">{{ errors.email }}</p>
          </div>

          <div class="field">
            <label for="feedback-message">Your message</label>
            <textarea
              id="feedback-message"
              v-model="form.message"
              name="message"
              rows="6"
              required
              :aria-invalid="errors.message ? 'true' : 'false'"
              :aria-describedby="errors.message ? 'feedback-message-error' : undefined"
            ></textarea>
            <p v-if="errors.message" id="feedback-message-error" class="field-error">{{ errors.message }}</p>
          </div>

          <div class="form-actions">
            <button class="button button-primary" type="submit" :disabled="sending">
              {{ sending ? 'Sending…' : 'Send feedback' }}
            </button>
          </div>

          <p class="status" :class="status.type" aria-live="polite">{{ status.text }}</p>
        </form>

        <div class="feedback-aside">
          <article class="info-card">
            <div class="icon" aria-hidden="true">✎</div>
            <h2>What helps most</h2>
            <ul class="tips">
              <li>Something that helped you, so we can do more of it.</li>
              <li>A page that was confusing, slow or not working.</li>
              <li>Topics, readings or songs you would like to find here.</li>
              <li>Anything that was hard to use on your phone.</li>
            </ul>
          </article>

          <article class="info-card">
            <div class="icon" aria-hidden="true">♡</div>
            <h2>Looking for prayer?</h2>
            <p>
              If you would like prayer or someone to talk to, the Support page is the best
              place. Requests there are kept in confidence.
            </p>
            <RouterLink class="text-link" to="/support">Go to Support <span aria-hidden="true">→</span></RouterLink>
          </article>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { reactive, ref, nextTick } from 'vue';
import { siteName } from '../config.js';
import { messages } from '../api.js';

const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const FIELD_ORDER = ['email', 'message'];

const form = reactive({ name: '', email: '', message: '' });
const errors = reactive({ email: '', message: '' });
const status = reactive({ type: '', text: '' });
const sending = ref(false);

function validate() {
  const email = form.email.trim();
  errors.email = !email || EMAIL_PATTERN.test(email)
    ? ''
    : 'Please enter a valid email address, for example name@example.com, or leave it empty.';
  errors.message = form.message.trim() ? '' : 'Please write a short message before sending.';
  return FIELD_ORDER.every((field) => !errors[field]);
}

async function focusFirstError() {
  await nextTick();
  const field = FIELD_ORDER.find((name) => errors[name]);
  if (field) document.getElementById(`feedback-${field}`)?.focus();
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
    await messages.sendFeedback({
      name: form.name.trim() || null,
      email: form.email.trim() || null,
      message: form.message.trim()
    });
    status.type = 'success';
    status.text = `Thank you for your feedback. It helps us make ${siteName} better for everyone who visits.`;
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

.feedback-grid {
  display: grid;
  grid-template-columns: minmax(0, 1.25fr) minmax(0, 0.75fr);
  gap: 28px;
  align-items: start;
}

.feedback-form {
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

.feedback-aside {
  display: grid;
  gap: 20px;
}

.info-card h2 {
  margin: 0 0 10px;
  font-size: 1.3rem;
}

.tips {
  margin: 0;
  padding-left: 1.2em;
  color: var(--muted);
  display: grid;
  gap: 6px;
}

@media (max-width: 820px) {
  .feedback-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 480px) {
  .panel-card {
    padding: 20px;
  }

  .form-actions .button {
    width: 100%;
  }
}
</style>
