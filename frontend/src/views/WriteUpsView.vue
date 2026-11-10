<template>
  <section class="section">
    <div class="container">
      <div class="section-heading center">
        <p class="eyebrow">Write-ups</p>
        <h1 class="section-title">Short reflections for deeper prayer, growth, and obedience.</h1>
        <p class="lede intro">
          Gentle, practical thoughts you can read in a few minutes. Open one, read it
          slowly, and take one idea with you into your day.
        </p>
      </div>

      <div class="card-grid writeup-grid">
        <article
          v-for="item in writeUps"
          :id="`writeup-${item.id}`"
          :key="item.id"
          class="info-card writeup"
          :class="{ 'is-open': isOpen(item.id) }"
          :aria-labelledby="`writeup-${item.id}-title`"
        >
          <p class="meta">
            <span>{{ item.minutes }} min read</span>
            <span aria-hidden="true">·</span>
            <span class="meta-ref">{{ item.reference }}</span>
          </p>
          <h2 :id="`writeup-${item.id}-title`" class="writeup-title">{{ item.title }}</h2>
          <p class="excerpt">{{ item.excerpt }}</p>

          <div v-show="isOpen(item.id)" :id="`writeup-${item.id}-body`" class="writeup-body">
            <template v-for="(block, index) in item.body" :key="index">
              <figure v-if="typeof block === 'object'" class="scripture">
                <blockquote>
                  <p>{{ block.verse }}</p>
                </blockquote>
                <figcaption>{{ block.reference }} (KJV)</figcaption>
              </figure>
              <p v-else>{{ block }}</p>
            </template>

            <div class="try-this">
              <p class="label">Try this today</p>
              <p>{{ item.tryThis }}</p>
            </div>

            <div class="body-links">
              <RouterLink class="text-link" to="/bible">Open the reading plan <span aria-hidden="true">→</span></RouterLink>
              <RouterLink class="text-link" to="/notes">Keep a note <span aria-hidden="true">→</span></RouterLink>
            </div>
          </div>

          <button
            type="button"
            class="toggle"
            :aria-expanded="isOpen(item.id) ? 'true' : 'false'"
            :aria-controls="`writeup-${item.id}-body`"
            @click="toggle(item.id)"
          >
            {{ isOpen(item.id) ? 'Show less' : 'Read the reflection' }}
            <span aria-hidden="true">{{ isOpen(item.id) ? '↑' : '↓' }}</span>
          </button>
        </article>
      </div>

      <p class="kjv-note">Scripture quotations from the King James Version (public domain).</p>
    </div>
  </section>
</template>

<script setup>
import { nextTick, reactive, ref, watch } from 'vue';
import { resourceState } from '../resourceState.js';

const WORDS_PER_MINUTE = 200;

// Sample content until the API serves real items
// Original reflections written for this site. Scripture is quoted word for word from the King James Version.
const writeUpSource = [
  {
    id: 'rest-for-the-weary',
    title: 'Rest for the Weary',
    reference: 'Matthew 11:28-30',
    excerpt: 'Jesus does not wait for us to have it all together. He invites the tired and the heavy-hearted to come to Him just as they are.',
    body: [
      'Tiredness is not only in the body. We can carry weariness in our minds and hearts too: worry about family, pressure at work, grief, or the feeling that we are never quite enough.',
      {
        reference: 'Matthew 11:28-29',
        verse: 'Come unto me, all ye that labour and are heavy laden, and I will give you rest. Take my yoke upon you, and learn of me; for I am meek and lowly in heart: and ye shall find rest unto your souls.'
      },
      'A yoke was a wooden frame that joined two animals so they could pull a load together. Jesus does not promise a life without work or burdens. He offers to walk beside us, share the weight, and set a pace we can keep.',
      'He describes Himself as “meek and lowly in heart.” He is gentle with us. We do not have to impress Him or earn His welcome. Being tired is reason enough to come.',
      'Rest might look like a short prayer at your desk, an early night, a walk without your phone, or an honest conversation with God about what you are carrying. Small moments of coming to Him add up to a rested soul.'
    ],
    tryThis: 'Take five quiet minutes today. Tell God plainly what feels heavy, then read Matthew 11:28-30 slowly, twice.'
  },
  {
    id: 'strength-of-surrender',
    title: 'The Strength of Surrender',
    reference: 'Proverbs 3:5-6',
    excerpt: 'True spiritual growth begins when we stop trying to control every outcome and trust God with the process.',
    body: [
      'Most of us like to have a plan. We make lists, rehearse conversations in our heads, and try to see around every corner. Planning is good, but it can slowly turn into a quiet belief that everything depends on us.',
      {
        reference: 'Proverbs 3:5-6',
        verse: 'Trust in the LORD with all thine heart; and lean not unto thine own understanding. In all thy ways acknowledge him, and he shall direct thy paths.'
      },
      'Surrender is not giving up, and it is not pretending we do not care. It is choosing to hold our plans with an open hand. We still do the work in front of us, but we stop carrying the weight of the outcome as if it were ours alone.',
      'Notice the words “all thine heart” and “all thy ways.” God is not asking for the leftover corners of our lives. He invites us to bring the whole of it: our work, our families, our money, our fears. That can feel risky, yet it is where real peace begins.',
      'Surrender is rarely one big moment. It is a small choice made again and again: this morning, this conversation, this decision. Each time we say, “Lord, I trust you with this,” our grip loosens a little and our hearts grow a little stronger.'
    ],
    tryThis: 'Name one situation you have been trying to control. Pray one honest sentence handing it to God, then take the next faithful step and leave the rest with Him.'
  },
  {
    id: 'prayer-that-changes-us',
    title: 'Prayer That Changes Us',
    reference: 'Philippians 4:6-7',
    excerpt: 'Prayer is not only asking for help. It is repositioning our hearts to hear God and align with His will.',
    body: [
      'We often come to prayer with a list, and that is welcome. God invites our requests. Yet prayer is more than a way to get things done. It is time spent with Someone who loves us, and time with Him slowly changes who we are.',
      {
        reference: 'Philippians 4:6-7',
        verse: 'Be careful for nothing; but in every thing by prayer and supplication with thanksgiving let your requests be made known unto God. And the peace of God, which passeth all understanding, shall keep your hearts and minds through Christ Jesus.'
      },
      '“Be careful for nothing” is older English for “don’t be full of worry about anything.” Paul does not promise that every request will be answered the way we expect. He promises something deeper: a peace that guards the heart and mind. Often the situation stays the same for a while, but we do not. We become calmer, kinder, and more ready to listen.',
      'Notice the small phrase “with thanksgiving.” Gratitude turns our attention from what is missing to what God has already given. It is hard to stay anxious and thankful at the same time.',
      'So when you pray, leave room for quiet. Say what is on your heart, then pause and listen. Prayer is a conversation, and some of the most important changes happen in the silence.'
    ],
    tryThis: 'Before you ask for anything today, thank God for three specific things. Then bring your request, and sit quietly with Him for one minute afterwards.'
  },
  {
    id: 'hope-during-delay',
    title: 'Hope During Delay',
    reference: 'Isaiah 40:31',
    excerpt: 'God’s timing is often different from ours, but it is never void of purpose, protection, or provision.',
    body: [
      'Waiting is hard. Whether you are waiting for work, for healing, for a relationship to be mended, or for a prayer to be answered, the days can feel long and the silence heavy. Scripture never pretends that waiting is easy, but it does tell us that waiting is not wasted.',
      {
        reference: 'Isaiah 40:31',
        verse: 'But they that wait upon the LORD shall renew their strength; they shall mount up with wings as eagles; they shall run, and not be weary; and they shall walk, and not faint.'
      },
      'To wait upon the Lord is not to sit still and do nothing. It is to keep looking to Him, keep trusting Him, and stay faithful in small things while the big thing has not yet arrived. Waiting like this is an active kind of hope.',
      'Delay can also be a form of care. Sometimes God is preparing us for what we are asking for, or preparing it for us. We rarely see the whole picture while we are in the middle of it, but we can know that He has not forgotten us.',
      'Notice the order in the verse: mount up, run, walk. Some days you will feel like soaring. On other days, simply walking without fainting is the victory. Both are gifts of strength from God.'
    ],
    tryThis: 'Write down what you are waiting for. Beside it, write one small, faithful thing you can do today while you wait.'
  },
  {
    id: 'lamp-for-the-next-step',
    title: 'A Lamp for the Next Step',
    reference: 'Psalm 119:105',
    excerpt: 'God may not show us the whole road at once, but His word gives enough light for the step in front of us.',
    body: [
      'Many of us want to see the whole map before we begin. We want to know how the story ends, how every need will be met, and where the path will lead years from now.',
      {
        reference: 'Psalm 119:105',
        verse: 'Thy word is a lamp unto my feet, and a light unto my path.'
      },
      'An oil lamp in Bible times gave a small circle of light, enough to see where to place your feet. It did not light up the whole valley. God often guides us the same way: not with every answer at once, but with enough light for today.',
      'This is why a daily habit of reading Scripture matters. A few verses read slowly and honestly can steady a worried mind, correct a wrong turn, or give courage for a hard conversation. Over time, those small lights become a path you can look back on, and you can see how faithfully God has led you.',
      'You do not need to read a lot to begin. Choose a short passage, read it twice, and ask what it shows you about God and about your next step.'
    ],
    tryThis: 'Choose one passage from the reading plan on the Bible page and write down one sentence it says to you today.'
  },
  {
    id: 'serving-with-joy',
    title: 'Serving with Joy',
    reference: 'Psalm 100:2',
    excerpt: 'The most meaningful service is done with humility, gratitude, and a willingness to love people as Christ does.',
    body: [
      'Serving others often starts with excitement and can slowly become a heavy duty. We keep showing up, but the joy fades and we begin to feel tired or unseen.',
      {
        reference: 'Psalm 100:2',
        verse: 'Serve the LORD with gladness: come before his presence with singing.'
      },
      'Gladness in service does not usually come from the task itself. Setting out chairs, cooking a meal, or visiting someone who is lonely can be very ordinary work. The joy comes from remembering who we are really serving. When we serve people, we serve the Lord, and He sees what no one else notices.',
      'Humility helps too. When we serve to be noticed, we are easily hurt. When we serve out of gratitude for what God has done for us, we are free to give without keeping score.',
      'Joyful service also makes room for rest. The Gospels show Jesus often going away to quiet places to pray. If serving has lost its joy, it may be time to step back, receive from God, and return refreshed rather than running on empty.'
    ],
    tryThis: 'Do one small, quiet act of kindness today that no one will thank you for, and offer it to God as worship.'
  }
];

function countWords(text) {
  return text.trim().split(/\s+/).filter(Boolean).length;
}

// Reading time is worked out from the actual text, so it stays right when a reflection is edited.
const writeUps = reactive(writeUpSource.map((item) => {
  const parts = [item.excerpt, item.tryThis, ...item.body.map((block) => (typeof block === 'object' ? block.verse : block))];
  const words = parts.reduce((total, text) => total + countWords(text), 0);
  return { ...item, minutes: Math.max(1, Math.ceil(words / WORDS_PER_MINUTE)) };
}));
watch(() => resourceState.data?.reflections, items => {
  if (!Array.isArray(items)) return;
  writeUps.splice(0, writeUps.length, ...items.map(item => {
    const parts = [item.excerpt, item.tryThis, ...(item.body || []).map(block => typeof block === 'object' ? block.verse : block)];
    const words = parts.reduce((total, text) => total + countWords(String(text || '')), 0);
    return { ...item, minutes: Math.max(1, Math.ceil(words / WORDS_PER_MINUTE)) };
  }));
}, { immediate: true });

const openIds = ref([]);

function isOpen(id) {
  return openIds.value.includes(id);
}

function prefersReducedMotion() {
  return typeof window.matchMedia === 'function' && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
}

function toggle(id) {
  if (!isOpen(id)) {
    openIds.value = [...openIds.value, id];
    return;
  }

  openIds.value = openIds.value.filter((openId) => openId !== id);

  // Closing a long reflection can leave the reader far below its card, so bring the card back into view.
  nextTick(() => {
    const card = document.getElementById(`writeup-${id}`);
    if (!card) return;
    const header = document.querySelector('.site-header');
    const offset = (header ? header.offsetHeight : 0) + 16;
    const top = card.getBoundingClientRect().top;
    if (top < offset) {
      window.scrollTo({
        top: window.scrollY + top - offset,
        behavior: prefersReducedMotion() ? 'auto' : 'smooth'
      });
    }
  });
}
</script>

<style scoped>
.intro {
  margin: 18px auto 0;
}

.writeup-grid {
  grid-template-columns: repeat(2, minmax(0, 1fr));
  align-items: start;
}

.writeup {
  transition: border-color 160ms ease, box-shadow 160ms ease;
}

.writeup.is-open {
  border-color: rgba(49, 95, 125, 0.3);
  box-shadow: var(--shadow);
}

.meta {
  display: flex;
  flex-wrap: wrap;
  gap: 4px 10px;
  margin: 0 0 12px;
  font-size: 0.85rem;
  font-weight: 600;
  color: var(--muted);
}

.meta-ref {
  color: var(--primary);
  font-weight: 700;
}

.writeup-title {
  margin: 0 0 10px;
  font-size: 1.45rem;
  line-height: 1.25;
  letter-spacing: -0.02em;
}

.excerpt {
  font-size: 1.02rem;
}

.writeup-body {
  margin-top: 18px;
  padding-top: 18px;
  border-top: 1px solid var(--line);
}

.writeup-body p {
  margin: 0 0 14px;
  color: var(--text);
}

.scripture {
  margin: 20px 0;
  padding: 16px 18px;
  border-left: 4px solid var(--accent);
  border-radius: 0 14px 14px 0;
  background: rgba(178, 138, 72, 0.1);
}

.scripture blockquote {
  margin: 0;
}

.scripture blockquote p {
  margin: 0 0 8px;
  font-family: Georgia, 'Times New Roman', serif;
  font-size: 1.1rem;
  line-height: 1.7;
}

.scripture figcaption {
  color: var(--primary);
  font-size: 0.9rem;
  font-weight: 700;
}

.try-this {
  margin-top: 20px;
  padding: 16px 18px;
  border-radius: 16px;
  background: rgba(49, 95, 125, 0.08);
}

.writeup-body .label {
  margin-bottom: 6px;
  color: var(--primary);
}

.try-this p:last-child {
  margin-bottom: 0;
}

.body-links {
  display: flex;
  flex-wrap: wrap;
  gap: 4px 24px;
}

.body-links .text-link {
  margin-top: 16px;
  padding-top: 0;
}

.toggle {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  align-self: flex-start;
  min-height: 44px;
  margin-top: 14px;
  padding: 0;
  background: none;
  color: var(--primary-strong);
  font-weight: 800;
}

.toggle:hover {
  color: var(--primary);
}

.toggle:focus-visible {
  outline: 3px solid rgba(49, 95, 125, 0.45);
  outline-offset: 4px;
  border-radius: 6px;
}

.kjv-note {
  margin: 28px 0 0;
  text-align: center;
  color: var(--muted);
  font-size: 0.9rem;
}

@media (max-width: 820px) {
  .writeup-grid {
    grid-template-columns: 1fr;
  }
}
</style>
