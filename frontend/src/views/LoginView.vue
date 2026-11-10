<template>
  <section class="section">
    <div class="container">
      <div class="section-heading center page-heading">
        <p class="eyebrow">Welcome back</p>
        <h1 class="section-title">Sign in to {{ siteName }}</h1>
        <p class="lede">Pick up where you left off with your study notes and reflections.</p>
      </div>

      <div class="panel-card auth-card">
        <p v-if="existingUser" class="signed-in-note">
          You are already signed in as <strong>{{ displayName(existingUser) }}</strong>.
          <RouterLink class="inline-link" to="/notes">Go to your notes</RouterLink>
        </p>

        <form novalidate @submit.prevent="submit">
          <div class="field">
            <label for="login-email">Email address</label>
            <input
              id="login-email"
              ref="emailInput"
              v-model.trim="form.email"
              type="email"
              name="email"
              autocomplete="email"
              inputmode="email"
              required
              :aria-invalid="errors.email ? 'true' : 'false'"
              :aria-describedby="errors.email ? 'login-email-error' : undefined"
            />
            <p v-if="errors.email" id="login-email-error" class="field-error">{{ errors.email }}</p>
          </div>

          <div class="field">
            <label for="login-password">Password</label>
            <input
              id="login-password"
              ref="passwordInput"
              v-model="form.password"
              :type="showPassword ? 'text' : 'password'"
              name="password"
              autocomplete="current-password"
              required
              :aria-invalid="errors.password ? 'true' : 'false'"
              :aria-describedby="errors.password ? 'login-password-error' : undefined"
            />
            <p v-if="errors.password" id="login-password-error" class="field-error">{{ errors.password }}</p>
          </div>

          <div class="check">
            <input id="login-show-password" v-model="showPassword" type="checkbox" />
            <label for="login-show-password">Show password</label>
          </div>

          <button class="button button-primary submit" type="submit" :disabled="sending">
            {{ sending ? 'Signing in…' : 'Sign in' }}
          </button>

          <p class="status" :class="status.kind ? `is-${status.kind}` : ''" role="status" aria-live="polite">{{ status.text }}</p>
        </form>

        <div class="auth-links">
          <p>New to {{ siteName }}? <RouterLink class="inline-link" to="/register">Create an account</RouterLink></p>
          <p>Forgotten your password? <RouterLink class="inline-link" to="/support">Ask us for help</RouterLink></p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import { siteName } from '../config.js';
import { auth } from '../api.js';

const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const router = useRouter();
const existingUser = auth.currentUser();

const form = reactive({ email: '', password: '' });
const errors = reactive({ email: '', password: '' });
const status = reactive({ kind: '', text: '' });
const showPassword = ref(false);
const sending = ref(false);
const emailInput = ref(null);
const passwordInput = ref(null);

function displayName(user) {
  return user?.fullName?.trim() || user?.email || 'friend';
}

function validate() {
  if (!form.email) {
    errors.email = 'Please enter your email address.';
  } else if (!EMAIL_PATTERN.test(form.email)) {
    errors.email = 'Please enter an email address like name@example.com.';
  } else {
    errors.email = '';
  }
  errors.password = form.password ? '' : 'Please enter your password.';
  return !errors.email && !errors.password;
}

async function submit() {
  if (sending.value) return;
  status.kind = '';
  status.text = '';

  if (!validate()) {
    status.kind = 'error';
    status.text = 'Please check the highlighted fields.';
    (errors.email ? emailInput : passwordInput).value?.focus();
    return;
  }

  sending.value = true;
  try {
    await auth.login({ email: form.email, password: form.password });
  } catch (error) {
    status.kind = 'error';
    status.text = error?.message || 'Something went wrong. Please try again.';
    sending.value = false;
    return;
  }

  form.password = '';
  status.kind = 'success';
  status.text = 'You are signed in. Taking you to your notes…';
  router.push('/notes').finally(() => {
    sending.value = false;
  });
}
</script>

<style scoped>
.page-heading .lede {
  margin: 16px auto 0;
}

.auth-card {
  --danger: #a3342a;
  max-width: 480px;
  margin: 0 auto;
  padding: 32px 28px;
}

.signed-in-note {
  margin: 0 0 22px;
  padding: 14px 16px;
  border-radius: 14px;
  background: rgba(49, 95, 125, 0.08);
  color: var(--text);
}

.field {
  display: grid;
  gap: 6px;
  margin-bottom: 18px;
}

.field label {
  font-weight: 600;
}

.field input {
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

.field input:focus-visible,
.check input:focus-visible {
  outline: 3px solid rgba(49, 95, 125, 0.35);
  outline-offset: 1px;
  border-color: var(--primary);
}

.field input[aria-invalid='true'] {
  border-color: var(--danger);
}

.field-error {
  margin: 0;
  color: var(--danger);
  font-size: 0.92rem;
}

.check {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 22px;
  color: var(--muted);
}

.check input {
  width: 18px;
  height: 18px;
  accent-color: var(--primary);
}

.submit {
  width: 100%;
}

.submit:disabled {
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

.status.is-success {
  color: var(--primary-strong);
}

.auth-links {
  margin-top: 26px;
  padding-top: 20px;
  border-top: 1px solid var(--line);
  color: var(--muted);
}

.auth-links p {
  margin: 0 0 8px;
}

.inline-link {
  color: var(--primary-strong);
  font-weight: 700;
  text-decoration: underline;
  text-underline-offset: 3px;
}

@media (max-width: 480px) {
  .auth-card {
    padding: 24px 18px;
  }
}
</style>
