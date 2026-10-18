---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/IconsSection.tsx"
ref: HEAD
---

export default function IconsSection() {
  return (
    <div className="tab-content">
      <div className="section-card">
        <h2>Resource Icons</h2>
        <p className="section-desc">Feedback resources and race-specific supply icons.</p>
        <div className="resource-icons-grid">
          <div className="resource-icon-card"><img src="war3/Feedback/Resources/ResourceGold.png" alt="" /><div className="ri-label">Gold</div></div>
          <div className="resource-icon-card"><img src="war3/Feedback/Resources/ResourceLumber.png" alt="" /><div className="ri-label">Lumber</div></div>
          <div className="resource-icon-card"><img src="war3/Feedback/Resources/ResourceSupply.png" alt="" /><div className="ri-label">Supply</div></div>
          <div className="resource-icon-card"><img src="war3/Feedback/Resources/ResourceUpkeep.png" alt="" /><div className="ri-label">Upkeep</div></div>
          <div className="resource-icon-card"><img src="war3/Feedback/Resources/ResourceManaStone.png" alt="" /><div className="ri-label">Mana Stone</div></div>
        </div>
        <h3>Race-Specific Supply Icons</h3>
        <div className="resource-icons-grid" style={{ marginTop: '10px' }}>
          <div className="resource-icon-card"><img src="war3/Feedback/Resources/ResourceHuman.png" alt="" /><div className="ri-label">Human</div></div>
          <div className="resource-icon-card"><img src="war3/Feedback/Resources/ResourceOrc.png" alt="" /><div className="ri-label">Orc</div></div>
          <div className="resource-icon-card"><img src="war3/Feedback/Resources/ResourceNightElf.png" alt="" /><div className="ri-label">Night Elf</div></div>
          <div className="resource-icon-card"><img src="war3/Feedback/Resources/ResourceUndead.png" alt="" /><div className="ri-label">Undead</div></div>
        </div>
      </div>

      <div className="section-card">
        <h2>Infocards</h2>
        <p className="section-desc">Armor types, attack types, hero attributes, and unit infocards from the in-game info panel.</p>
        <div className="infocards-grid">
          <div className="infocard-group-label">Armor Types</div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-armor-small.png" alt="" /><div className="ic-label">Small</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-armor-medium.png" alt="" /><div className="ic-label">Medium</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-armor-large.png" alt="" /><div className="ic-label">Large</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-armor-fortified.png" alt="" /><div className="ic-label">Fortified</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-armor-hero.png" alt="" /><div className="ic-label">Hero</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-armor-divine.png" alt="" /><div className="ic-label">Divine</div></div>
          <div className="infocard"><img src="War3x/Widgets/Console/Human/infocard-armor-unarmored.png" alt="" /><div className="ic-label">Unarmored</div></div>

          <div className="infocard-group-label">Attack Types</div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-attack-melee.png" alt="" /><div className="ic-label">Melee</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-attack-piercing.png" alt="" /><div className="ic-label">Piercing</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-attack-siege.png" alt="" /><div className="ic-label">Siege</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-attack-magic.png" alt="" /><div className="ic-label">Magic</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-attack-chaos.png" alt="" /><div className="ic-label">Chaos</div></div>
          <div className="infocard"><img src="War3x/Widgets/Console/Human/infocard-attack-hero.png" alt="" /><div className="ic-label">Hero</div></div>

          <div className="infocard-group-label">Hero Attributes</div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-heroattributes-str.png" alt="" /><div className="ic-label">Strength</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-heroattributes-agi.png" alt="" /><div className="ic-label">Agility</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-heroattributes-int.png" alt="" /><div className="ic-label">Intelligence</div></div>

          <div className="infocard-group-label">Unit Infocards</div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-priest.png" alt="" /><div className="ic-label">Priest</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-sorceress.png" alt="" /><div className="ic-label">Sorceress</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-shaman.png" alt="" /><div className="ic-label">Shaman</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-witchdoctor.png" alt="" /><div className="ic-label">Witch Doctor</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-necromancer.png" alt="" /><div className="ic-label">Necromancer</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-banshee.png" alt="" /><div className="ic-label">Banshee</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-druidofclaw.png" alt="" /><div className="ic-label">Druid of Claw</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-druidoftalon.png" alt="" /><div className="ic-label">Druid of Talon</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-gold.png" alt="" /><div className="ic-label">Gold Mine</div></div>
          <div className="infocard"><img src="war3/Widgets/Console/Human/infocard-supply.png" alt="" /><div className="ic-label">Supply</div></div>
          <div className="infocard"><img src="War3x/Widgets/Console/Human/infocard-siren.png" alt="" /><div className="ic-label">Siren</div></div>
          <div className="infocard"><img src="War3x/Widgets/Console/Human/infocard-spiritwalker.png" alt="" /><div className="ic-label">Spirit Walker</div></div>
        </div>
      </div>

      <div className="section-card">
        <h2>Dialog Icons</h2>
        <p className="section-desc">Error, message, and question dialog box icons.</p>
        <div className="dialog-icons">
          <div className="dialog-icon-card"><img src="war3/Widgets/Glues/dialogbox-error.png" alt="Error" /><div className="icon-label">Error</div></div>
          <div className="dialog-icon-card"><img src="war3/Widgets/Glues/dialogbox-message.png" alt="Message" /><div className="icon-label">Message</div></div>
          <div className="dialog-icon-card"><img src="war3/Widgets/Glues/dialogbox-question.png" alt="Question" /><div className="icon-label">Question</div></div>
        </div>
      </div>

      <div className="section-card">
        <h2>Cursors</h2>
        <p className="section-desc">Race-specific mouse cursors (256x128 sprite sheets — first frame shown). The active race cursor follows your mouse.</p>
        <div className="cursors-row">
          <div className="cursor-card"><img src="war3/Cursor/HumanCursor.png" alt="" /><div className="cursor-label">Human</div></div>
          <div className="cursor-card"><img src="war3/Cursor/OrcCursor.png" alt="" /><div className="cursor-label">Orc</div></div>
          <div className="cursor-card"><img src="war3/Cursor/NightElfCursor.png" alt="" /><div className="cursor-label">Night Elf</div></div>
          <div className="cursor-card"><img src="war3/Cursor/UndeadCursor.png" alt="" /><div className="cursor-label">Undead</div></div>
        </div>
      </div>
    </div>
  );
}
