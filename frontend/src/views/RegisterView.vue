<template>
  <section class="section">
    <div class="container">
      <div class="section-heading center page-heading">
        <p class="eyebrow">Join us</p>
        <h1 class="section-title">Create your account</h1>
        <p class="lede">
          An account gives you a place of your own on {{ siteName }}. Once accounts are fully live,
          your study notes will be kept with your account.
        </p>
      </div>

      <div class="panel-card auth-card">
        <div v-if="registered" class="success-panel">
          <h2 ref="successHeading" tabindex="-1">Your account is ready</h2>
          <p>
            Welcome, {{ registeredName }}. You can now sign in with <strong>{{ registeredEmail }}</strong>
            and the password you just chose.
          </p>
          <RouterLink class="button button-primary submit" to="/login">Sign in</RouterLink>
        </div>

        <form v-else novalidate @submit.prevent="submit">
          <div class="field">
            <label for="register-invite">Review invitation code</label>
            <input id="register-invite" v-model.trim="form.inviteCode" type="password" autocomplete="off" required />
            <p class="hint">Registration is available only to invited reviewers.</p>
          </div>

          <div class="field">
            <label for="register-name">Full name</label>
            <input
              id="register-name"
              ref="nameInput"
              v-model.trim="form.fullName"
              type="text"
              name="fullName"
              autocomplete="name"
              maxlength="100"
              required
              :aria-invalid="errors.fullName ? 'true' : 'false'"
              :aria-describedby="errors.fullName ? 'register-name-error' : undefined"
            />
            <p v-if="errors.fullName" id="register-name-error" class="field-error">{{ errors.fullName }}</p>
          </div>

          <div class="field">
            <label for="register-email">Email address</label>
            <input
              id="register-email"
              ref="emailInput"
              v-model.trim="form.email"
              type="email"
              name="email"
              autocomplete="email"
              inputmode="email"
              required
              :aria-invalid="errors.email ? 'true' : 'false'"
              :aria-describedby="errors.email ? 'register-email-error' : undefined"
            />
            <p v-if="errors.email" id="register-email-error" class="field-error">{{ errors.email }}</p>
          </div>

          <div class="field">
            <label for="register-password">Password</label>
            <input
              id="register-password"
              ref="passwordInput"
              v-model="form.password"
              :type="showPassword ? 'text' : 'password'"
              name="password"
              autocomplete="new-password"
              required
              :aria-invalid="errors.password ? 'true' : 'false'"
              :aria-describedby="passwordDescribedBy"
            />
            <p id="register-password-help" class="hint">
              To keep your account safe, your password needs:
            </p>
            <ul id="register-password-rules" class="rules">
              <li v-for="rule in ruleStates" :key="rule.id" :class="{ met: rule.met }">
                <span class="rule-mark" aria-hidden="true">{{ rule.met ? '✓' : '○' }}</span>
                <span>{{ rule.text }}</span>
                <span class="visually-hidden">{{ rule.met ? '(done)' : '(not yet)' }}</span>
              </li>
            </ul>
            <p v-if="errors.password" id="register-password-error" class="field-error">{{ errors.password }}</p>
          </div>

          <div class="field">
            <label for="register-confirm">Confirm password</label>
            <input
              id="register-confirm"
              ref="confirmInput"
              v-model="form.confirmPassword"
              :type="showPassword ? 'text' : 'password'"
              name="confirmPassword"
              autocomplete="new-password"
              required
              :aria-invalid="errors.confirmPassword ? 'true' : 'false'"
              :aria-describedby="errors.confirmPassword ? 'register-confirm-error' : undefined"
            />
            <p v-if="errors.confirmPassword" id="register-confirm-error" class="field-error">{{ errors.confirmPassword }}</p>
          </div>

          <div class="check">
            <input id="register-show-password" v-model="showPassword" type="checkbox" />
            <label for="register-show-password">Show passwords</label>
          </div>

          <button class="button button-primary submit" type="submit" :disabled="sending">
            {{ sending ? 'Creating your account…' : 'Create account' }}
          </button>
        </form>

        <p class="status" :class="status.kind ? `is-${status.kind}` : ''" role="status" aria-live="polite">{{ status.text }}</p>

        <div v-if="!registered" class="auth-links">
          <p>Already have an account? <RouterLink class="inline-link" to="/login">Sign in</RouterLink></p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed, nextTick, reactive, ref } from 'vue';
import { siteName } from '../config.js';
import { auth } from '../api.js';

const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

// These mirror the ASP.NET Core Identity default password rules the server enforces.
// Identity counts only A–Z, a–z and 0–9 as letters and digits; anything else is a symbol.
const passwordRules = [
  { id: 'length', text: 'At least 12 characters', short: 'at least 12 characters', test: (p) => p.length >= 12 },
  { id: 'upper', text: 'An uppercase letter (A–Z)', short: 'an uppercase letter', test: (p) => /[A-Z]/.test(p) },
  { id: 'lower', text: 'A lowercase letter (a–z)', short: 'a lowercase letter', test: (p) => /[a-z]/.test(p) },
  { id: 'digit', text: 'A number (0–9)', short: 'a number', test: (p) => /[0-9]/.test(p) },
  { id: 'symbol', text: 'A symbol, such as ! ? # or @', short: 'a symbol', test: (p) => /[^A-Za-z0-9]/.test(p) }
];

const form = reactive({ inviteCode: '', fullName: '', email: '', password: '', confirmPassword: '' });
const errors = reactive({ fullName: '', email: '', password: '', confirmPassword: '' });
const status = reactive({ kind: '', text: '' });
const showPassword = ref(false);
const sending = ref(false);
const registered = ref(false);
const registeredName = ref('');
const registeredEmail = ref('');

const nameInput = ref(null);
const emailInput = ref(null);
const passwordInput = ref(null);
const confirmInput = ref(null);
const successHeading = ref(null);

const ruleStates = computed(() => passwordRules.map((rule) => ({ ...rule, met: rule.test(form.password) })));

const passwordDescribedBy = computed(() =>
  ['register-password-help', 'register-password-rules', errors.password ? 'register-password-error' : '']
    .filter(Boolean)
    .join(' ')
);

function validate() {
  errors.fullName = form.fullName ? '' : 'Please enter your name.';

  if (!form.email) {
    errors.email = 'Please enter your email address.';
  } else if (!EMAIL_PATTERN.test(form.email)) {
    errors.email = 'Please enter an email address like name@example.com.';
  } else {
    errors.email = '';
  }

  const missing = ruleStates.value.filter((rule) => !rule.met).map((rule) => rule.short);
  if (!form.password) {
    errors.password = 'Please choose a password.';
  } else if (missing.length) {
    errors.password = `Your password still needs ${missing.join(', ')}.`;
  } else {
    errors.password = '';
  }

  if (!form.confirmPassword) {
    errors.confirmPassword = 'Please type your password again.';
  } else if (form.confirmPassword !== form.password) {
    errors.confirmPassword = 'The two passwords do not match.';
  } else {
    errors.confirmPassword = '';
  }

  const order = [
    [errors.fullName, nameInput],
    [errors.email, emailInput],
    [errors.password, passwordInput],
    [errors.confirmPassword, confirmInput]
  ];
  const firstInvalid = order.find(([message]) => message);
  firstInvalid?.[1].value?.focus();
  return !firstInvalid;
}

async function submit() {
  if (sending.value) return;
  status.kind = '';
  status.text = '';

  if (!validate()) {
    status.kind = 'error';
    status.text = 'Please check the highlighted fields.';
    return;
  }

  sending.value = true;
  try {
    await auth.register({ fullName: form.fullName, email: form.email, password: form.password, inviteCode: form.inviteCode });
  } catch (error) {
    status.kind = 'error';
    status.text = error?.message || 'Something went wrong. Please try again.';
    sending.value = false;
    return;
  }

  registeredName.value = form.fullName.split(/\s+/)[0] || form.fullName;
  registeredEmail.value = form.email;
  form.password = '';
  form.confirmPassword = '';
  form.inviteCode = '';
  sending.value = false;
  registered.value = true;
  status.kind = 'success';
  status.text = 'Your account has been created. You can sign in now.';
  await nextTick();
  successHeading.value?.focus();
}
</script>

<style scoped>
.page-heading .lede {
  margin: 16px auto 0;
}

.auth-card {
  --danger: #a3342a;
  max-width: 520px;
  margin: 0 auto;
  padding: 32px 28px;
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

.hint {
  margin: 4px 0 0;
  color: var(--muted);
  font-size: 0.92rem;
}

.rules {
  list-style: none;
  margin: 0;
  padding: 12px 14px;
  display: grid;
  gap: 4px;
  border-radius: 14px;
  background: rgba(49, 95, 125, 0.06);
  color: var(--muted);
  font-size: 0.92rem;
}

.rules li {
  display: flex;
  align-items: baseline;
  gap: 8px;
}

.rules li.met {
  color: var(--primary-strong);
  font-weight: 600;
}

.rule-mark {
  width: 1.1em;
  flex: none;
  text-align: center;
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

.success-panel h2 {
  margin: 0 0 10px;
  font-size: 1.6rem;
  letter-spacing: -0.03em;
}

.success-panel h2:focus {
  outline: none;
}

.success-panel p {
  margin: 0 0 22px;
  color: var(--muted);
  overflow-wrap: anywhere;
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
  margin: 0;
}

.inline-link {
  color: var(--primary-strong);
  font-weight: 700;
  text-decoration: underline;
  text-underline-offset: 3px;
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

@media (max-width: 480px) {
  .auth-card {
    padding: 24px 18px;
  }
}
</style>
