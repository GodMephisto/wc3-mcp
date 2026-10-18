---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/FAQSection.tsx"
ref: HEAD
---

const items = [
  {
    q: 'Is this an official Blizzard product?',
    a: 'No. This is an independent open-source project. Blizzard Entertainment has no affiliation. All visual assets are recreated from scratch.',
  },
  {
    q: 'Does it work with Tailwind?',
    a: 'Yes. WC3UI uses CSS custom properties internally. Tailwind utility classes compose cleanly alongside faction themes.',
  },
  {
    q: 'Can I use it in production?',
    a: "We've been in beta since the Reign of Chaos launch. So, technically, it's battle-tested.",
  },
  {
    q: 'Why is there no Blood Elf theme?',
    a: "That's a TBC problem. We're tracking it in issue #247.",
  },
] as const;

export default function FAQSection() {
  return (
    <section className="section-card faq-section">
      <h3>FAQ</h3>
      <div className="faq-list">
        {items.map((item) => (
          <div key={item.q} className="faq-item">
            <h4 className="faq-question-heading">{item.q}</h4>
            <p className="faq-answer-text">{item.a}</p>
          </div>
        ))}
      </div>
    </section>
  );
}
