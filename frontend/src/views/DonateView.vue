<template>
  <section class="section">
    <div class="container">
      <div class="section-heading center page-heading">
        <p class="eyebrow">Give</p>
        <h1 class="section-title">Partner with the work of {{ siteName }}.</h1>
        <p class="lede">
          Every gift helps this outreach keep sharing God's word freely with anyone who
          visits. Whatever you give, thank you for standing with us in prayer and in giving.
        </p>
      </div>

      <div class="card-grid three-up">
        <article v-for="item in purposes" :key="item.title" class="info-card">
          <div class="icon" aria-hidden="true">{{ item.icon }}</div>
          <h2>{{ item.title }}</h2>
          <p>{{ item.text }}</p>
        </article>
      </div>

      <figure class="verse center">
        <blockquote>
          <p>
            “Every man according as he purposeth in his heart, so let him give; not grudgingly,
            or of necessity: for God loveth a cheerful giver.”
          </p>
        </blockquote>
        <figcaption class="reading-reference">2 Corinthians 9:7 (KJV)</figcaption>
      </figure>
    </div>
  </section>

  <section class="section section-alt" aria-labelledby="how-to-give">
    <div class="container">
      <div class="section-heading center">
        <p class="eyebrow">How to give</p>
        <h2 id="how-to-give" class="section-title">Giving details</h2>
      </div>

      <template v-if="hasDetails">
        <div class="details-grid">
          <article v-if="mpesaRows.length" class="panel-card detail-card">
            <p class="label">Mobile money</p>
            <h3>M-Pesa</h3>
            <dl>
              <div v-for="row in mpesaRows" :key="row.label" class="detail-row">
                <dt>{{ row.label }}</dt>
                <dd>{{ row.value }}</dd>
              </div>
            </dl>
            <p v-if="paybill" class="detail-hint">
              On your phone, open M-Pesa, choose Lipa na M-Pesa, then Pay Bill, and enter the
              details above.
            </p>
          </article>

          <article v-if="bankRows.length" class="panel-card detail-card">
            <p class="label">Bank</p>
            <h3>Bank transfer</h3>
            <dl>
              <div v-for="row in bankRows" :key="row.label" class="detail-row">
                <dt>{{ row.label }}</dt>
                <dd>{{ row.value }}</dd>
              </div>
            </dl>
          </article>

          <article v-if="contactEmail" class="panel-card detail-card">
            <p class="label">Questions</p>
            <h3>Ask about giving</h3>
            <p class="detail-text">If you have any question about giving, please email us.</p>
            <a class="text-link" :href="`mailto:${contactEmail}`">{{ contactEmail }}</a>
          </article>
        </div>

        <p class="safety-note center">
          Please give only through the details shown on this page. If anything looks different,
          <RouterLink class="inline-link" to="/support">ask us first</RouterLink>.
        </p>
      </template>

      <div v-else class="panel-card card-highlight coming-soon center">
        <p class="label">Coming soon</p>
        <h3>Donation details will be published here soon.</h3>
        <p>
          Thank you for wanting to give. If you have a question about giving in the meantime,
          please reach us through the Support page.
        </p>
        <RouterLink class="text-link" to="/support">Ask a question <span aria-hidden="true">→</span></RouterLink>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed } from 'vue';
import { siteName, donation } from '../config.js';
import { resourceState } from '../resourceState.js';

// Descriptions of what the site already offers; no figures or projects are claimed.
const purposes = [
  {
    icon: '✦',
    title: 'Scripture for everyone',
    text: 'Keeping Bible readings and short reflections open and free for anyone who visits.'
  },
  {
    icon: '♫',
    title: 'Teaching and worship',
    text: 'Sharing sermons and songs that help people grow in faith wherever they are.'
  },
  {
    icon: '♡',
    title: 'Prayer and care',
    text: 'Making time to pray with and respond to people who reach out for support.'
  }
];

// Payment details come only from config.js. Empty values are never shown.
function clean(value) {
  return String(value ?? '').trim();
}

const details = computed(() => resourceState.data?.site?.donation || donation || {});
const paybill = computed(() => clean(details.value.mpesaPaybill));
const contactEmail = computed(() => clean(details.value.contactEmail));
const mpesaRows = computed(() => [
  { label: 'Paybill number', value: paybill.value },
  { label: 'Account number', value: clean(details.value.mpesaAccount) }
].filter((row) => row.value));
const bankRows = computed(() => [
  { label: 'Bank', value: clean(details.value.bankName) },
  { label: 'Account name', value: clean(details.value.bankAccountName) },
  { label: 'Account number', value: clean(details.value.bankAccountNumber) }
].filter((row) => row.value));
const hasDetails = computed(() => mpesaRows.value.length > 0 || bankRows.value.length > 0 || contactEmail.value !== '');
</script>

<style scoped>
.page-heading .lede {
  margin: 18px auto 0;
}

.info-card h2 {
  margin: 0 0 10px;
  font-size: 1.45rem;
}

.verse {
  max-width: 680px;
  margin: 48px auto 0;
}

.verse blockquote {
  margin: 0 0 12px;
}

.verse blockquote p {
  margin: 0;
  font-size: 1.2rem;
  font-style: italic;
  color: var(--text);
}

.verse .reading-reference {
  margin: 0;
}

.details-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(min(100%, 280px), 1fr));
  gap: 24px;
}

.detail-card h3 {
  margin: 0 0 16px;
  font-size: 1.4rem;
}

.detail-card dl {
  margin: 0;
  display: grid;
  gap: 12px;
}

.detail-row {
  display: grid;
  gap: 2px;
  padding-bottom: 12px;
  border-bottom: 1px solid var(--line);
}

.detail-row:last-child {
  padding-bottom: 0;
  border-bottom: none;
}

.detail-row dt {
  color: var(--muted);
  font-size: 0.9rem;
}

.detail-row dd {
  margin: 0;
  font-size: 1.2rem;
  font-weight: 800;
  overflow-wrap: anywhere;
}

.detail-hint,
.detail-text {
  margin: 16px 0 0;
  color: var(--muted);
  font-size: 0.95rem;
}

.detail-card .text-link {
  margin-top: 12px;
  overflow-wrap: anywhere;
}

.safety-note {
  max-width: 620px;
  margin: 28px auto 0;
  color: var(--muted);
}

.inline-link {
  color: var(--primary-strong);
  font-weight: 700;
  text-decoration: underline;
  text-underline-offset: 3px;
}

.coming-soon {
  max-width: 640px;
  margin: 0 auto;
}

.coming-soon h3 {
  margin: 0 0 10px;
  font-size: 1.5rem;
  line-height: 1.25;
}

.coming-soon p:not(.label) {
  margin: 0;
  color: var(--muted);
}

@media (max-width: 480px) {
  .panel-card {
    padding: 20px;
  }
}
</style>
