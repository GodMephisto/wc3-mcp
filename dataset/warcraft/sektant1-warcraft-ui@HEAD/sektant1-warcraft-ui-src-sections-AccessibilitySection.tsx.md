---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/AccessibilitySection.tsx"
ref: HEAD
---

const checks = [
  'Keyboard navigable',
  'ARIA labels',
  'Reduced motion',
  'Colour contrast AA',
  'Focus-visible rings (faction-styled, of course)',
] as const;

export default function AccessibilitySection() {
  return (
    <section className="section-card">
      <h3>Forged for everyone.</h3>
      <p className="section-desc">
        All components meet WCAG 2.1 AA contrast ratios — even the Undead theme. Full keyboard navigation,
        screen reader labels, and reduced-motion support are built in, not bolted on.
      </p>
      <div className="a11y-checklist">
        {checks.map((item) => (
          <span key={item} className="a11y-check-item">
            <span className="a11y-check-icon">&#10003;</span> {item}
          </span>
        ))}
      </div>
    </section>
  );
}
