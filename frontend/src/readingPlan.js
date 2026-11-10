export const readingPlan = [
  {
    id: 'psalm-23',
    reference: 'Psalm 23',
    theme: 'When you need a shepherd',
    poetry: true,
    verses: [
      { n: 1, text: 'The LORD is my shepherd; I shall not want.' },
      { n: 2, text: 'He maketh me to lie down in green pastures: he leadeth me beside the still waters.' },
      { n: 3, text: 'He restoreth my soul: he leadeth me in the paths of righteousness for his name’s sake.' },
      { n: 4, text: 'Yea, though I walk through the valley of the shadow of death, I will fear no evil: for thou art with me; thy rod and thy staff they comfort me.' },
      { n: 5, text: 'Thou preparest a table before me in the presence of mine enemies: thou anointest my head with oil; my cup runneth over.' },
      { n: 6, text: 'Surely goodness and mercy shall follow me all the days of my life: and I will dwell in the house of the LORD for ever.' }
    ],
    question: 'Which part of your life most needs the care of a good shepherd right now: rest, direction, comfort, or protection?',
    plainWords: '“I shall not want” means “I will not be without what I truly need.”'
  },
  {
    id: 'john-3-16-17',
    reference: 'John 3:16-17',
    theme: 'God’s love for the world',
    verses: [
      { n: 16, text: 'For God so loved the world, that he gave his only begotten Son, that whosoever believeth in him should not perish, but have everlasting life.' },
      { n: 17, text: 'For God sent not his Son into the world to condemn the world; but that the world through him might be saved.' }
    ],
    question: 'How would today feel different if you truly believed that God looks at you with love, not condemnation?',
    plainWords: '“His only begotten Son” means God’s one and only Son, Jesus.'
  },
  {
    id: 'romans-8-28',
    reference: 'Romans 8:28',
    theme: 'Trusting God in hard seasons',
    verses: [
      { n: 28, text: 'And we know that all things work together for good to them that love God, to them who are the called according to his purpose.' }
    ],
    question: 'Think of a hard season you have already come through. What good, even something small, did God bring out of it?'
  },
  {
    id: 'philippians-4-6-7',
    reference: 'Philippians 4:6-7',
    theme: 'Peace in place of worry',
    verses: [
      { n: 6, text: 'Be careful for nothing; but in every thing by prayer and supplication with thanksgiving let your requests be made known unto God.' },
      { n: 7, text: 'And the peace of God, which passeth all understanding, shall keep your hearts and minds through Christ Jesus.' }
    ],
    question: 'What is one worry you could turn into a short, honest prayer right now, together with one thing you are thankful for?',
    plainWords: '“Be careful for nothing” is older English for “don’t be full of worry about anything.” “Supplication” means asking humbly.'
  },
  {
    id: 'isaiah-40-31',
    reference: 'Isaiah 40:31',
    theme: 'Strength for the tired',
    verses: [
      { n: 31, text: 'But they that wait upon the LORD shall renew their strength; they shall mount up with wings as eagles; they shall run, and not be weary; and they shall walk, and not faint.' }
    ],
    question: 'Where are you running on empty? What would it look like to wait on God there instead of pushing on alone?',
    plainWords: 'To “wait upon the LORD” means to keep trusting and looking to Him, not simply to pass the time.'
  },
  {
    id: 'proverbs-3-5-6',
    reference: 'Proverbs 3:5-6',
    theme: 'Guidance for decisions',
    poetry: true,
    verses: [
      { n: 5, text: 'Trust in the LORD with all thine heart; and lean not unto thine own understanding.' },
      { n: 6, text: 'In all thy ways acknowledge him, and he shall direct thy paths.' }
    ],
    question: 'What decision are you trying to work out on your own? How could you invite God into it today?',
    plainWords: '“Acknowledge him” means to recognise God and include Him in every part of life.'
  },
  {
    id: 'joshua-1-9',
    reference: 'Joshua 1:9',
    theme: 'Courage for the next step',
    verses: [
      { n: 9, text: 'Have not I commanded thee? Be strong and of a good courage; be not afraid, neither be thou dismayed: for the LORD thy God is with thee whithersoever thou goest.' }
    ],
    question: 'What step have you been afraid to take? Does knowing that God goes with you change how it looks?',
    plainWords: '“Dismayed” means discouraged or alarmed. “Whithersoever thou goest” means “wherever you go.”'
  }
];

export function getTodaysReading(now = new Date(), items = readingPlan) {
  const day = Math.floor(Date.UTC(now.getFullYear(), now.getMonth(), now.getDate()) / 86400000);
  return items[((day % items.length) + items.length) % items.length];
}
