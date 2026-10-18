---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/App.tsx"
ref: HEAD
---

import { useState, useEffect } from "react";

import {
  GlueScreenButton,
  GlueBorderedButton,
  GlueMenuButton,
  GlueCampaignButton,
  GlueDropdown,
  GlueListBox,
  CommandCard,
  useCurrentRace,
  setCurrentRace,
  CursorOverlay,
  HeroPortraitModel,
  ResourceCounter,
  WorkerUnitModel,
  EscOptionButton,
  EscRadioButton,
  EscCheckbox,
  EscSlider,
  InputBox,
  StatBar,
  LoadingBar,
  MenuPanel,
  Tooltip,
  SectionTitle,
  Heading,
  BlpIcon,
  Table,
  RACES,
  type Race,
  type TableColumn,
  type TableRowDef,
} from "../lib/main";
import type { CommandSlot } from "./components/CommandCard/CommandCard";

import "./index.css";

const MOVE: CommandSlot = {
  hotkey: "Q",
  label: "Move",
  iconPath: "buttons/command/BTNMove.blp",
  state: "ready",
  tooltip: {
    name: "Move",
    description: "Gives the order to move to a location.",
  },
};
const STOP: CommandSlot = {
  hotkey: "W",
  label: "Stop",
  iconPath: "buttons/command/BTNStop.blp",
  state: "ready",
  tooltip: { name: "Stop", description: "Stops the current action." },
};
const HOLD: CommandSlot = {
  hotkey: "E",
  label: "Hold Position",
  iconPath: "buttons/command/BTNHoldPosition.blp",
  state: "ready",
  tooltip: {
    name: "Hold Position",
    description: "The unit will hold position and not chase after enemies.",
  },
};
const ATTACK: CommandSlot = {
  hotkey: "R",
  label: "Attack",
  iconPath: "buttons/command/BTNAttack.blp",
  state: "ready",
  tooltip: {
    name: "Attack",
    description: "Attack a target unit or attack-move to a location.",
  },
};
const PATROL: CommandSlot = {
  hotkey: "A",
  label: "Patrol",
  iconPath: "buttons/command/BTNPatrol.blp",
  state: "ready",
  tooltip: {
    name: "Patrol",
    description: "Patrols between two points, engaging enemies along the way.",
  },
};
const EMPTY: CommandSlot = { state: "empty" };
const LEARN: CommandSlot = {
  hotkey: "F",
  label: "Learn Skill",
  iconPath: "buttons/command/BTNSkillz.blp",
  state: "levelup",
  tooltip: {
    name: "Hero Abilities",
    description: "Spend a skill point to learn or upgrade an ability.",
  },
};

const HERO_COMMAND_CARDS: Record<Race, CommandSlot[]> = {
  // Archmage
  Human: [
    MOVE,
    STOP,
    HOLD,
    ATTACK,
    PATROL,
    EMPTY,
    EMPTY,
    LEARN,
    {
      hotkey: "Z",
      label: "Blizzard",
      iconPath: "buttons/command/BTNBlizzard.blp",
      state: "ready",
      tooltip: {
        name: "Blizzard",
        type: "Active ability",
        description:
          "Calls down waves of freezing ice shards that damage units in a target area.",
        manaCost: 75,
        cooldown: 6,
      },
    },
    {
      hotkey: "X",
      label: "Summon Water Elemental",
      iconPath: "buttons/command/BTNSummonWaterElemental.blp",
      state: "active",
      tooltip: {
        name: "Summon Water Elemental",
        type: "Active ability",
        description:
          "Summons a Water Elemental to fight for the Archmage. The elemental lasts 60 seconds.",
        manaCost: 125,
        cooldown: 20,
      },
    },
    {
      hotkey: "C",
      label: "Brilliance Aura",
      iconPath: "buttons/command/BTNBrilliance.blp",
      state: "passive",
      tooltip: {
        name: "Brilliance Aura",
        type: "Passive ability",
        description:
          "Increases nearby friendly units' mana regeneration by 0.75 mana per second.",
      },
    },
    {
      hotkey: "V",
      label: "Mass Teleport",
      iconPath: "buttons/command/BTNMassTeleport.blp",
      state: "ready",
      tooltip: {
        name: "Mass Teleport",
        type: "Ultimate ability",
        description:
          "Teleports 24 nearby units to a target friendly ground unit or structure.",
        manaCost: 100,
        cooldown: 20,
      },
    },
  ],
  // Blademaster
  Orc: [
    MOVE,
    STOP,
    HOLD,
    ATTACK,
    PATROL,
    EMPTY,
    EMPTY,
    LEARN,
    {
      hotkey: "Z",
      label: "Wind Walk",
      iconPath: "buttons/command/BTNWindWalkOn.blp",
      state: "active",
      tooltip: {
        name: "Wind Walk",
        type: "Active ability",
        description:
          "Allows the Blademaster to become invisible and move faster for a set amount of time. Deals bonus damage on first attack.",
        manaCost: 75,
        cooldown: 5,
      },
    },
    {
      hotkey: "X",
      label: "Mirror Image",
      iconPath: "buttons/command/BTNMirrorImage.blp",
      state: "ready",
      tooltip: {
        name: "Mirror Image",
        type: "Active ability",
        description:
          "Confuses the enemy by creating illusions of the Blademaster that deal no damage and take extra damage.",
        manaCost: 80,
        cooldown: 10,
      },
    },
    {
      hotkey: "C",
      label: "Critical Strike",
      iconPath: "buttons/command/BTNCriticalStrike.blp",
      state: "passive",
      tooltip: {
        name: "Critical Strike",
        type: "Passive ability",
        description:
          "Gives a 15% chance that the Blademaster will do 2x his normal damage on an attack.",
      },
    },
    {
      hotkey: "V",
      label: "Bladestorm",
      iconPath: "buttons/command/BTNWhirlwind.blp",
      state: "ready",
      tooltip: {
        name: "Bladestorm",
        type: "Ultimate ability",
        description:
          "Causes a whirlwind of destructive force around the Blademaster, rendering him immune to magic and dealing 110 damage per second to nearby enemy units.",
        manaCost: 200,
        cooldown: 180,
      },
    },
  ],
  // Demon Hunter
  NightElf: [
    MOVE,
    STOP,
    HOLD,
    ATTACK,
    PATROL,
    EMPTY,
    EMPTY,
    LEARN,
    {
      hotkey: "Z",
      label: "Mana Burn",
      iconPath: "buttons/command/BTNManaBurn.blp",
      state: "ready",
      tooltip: {
        name: "Mana Burn",
        type: "Active ability",
        description:
          "Sends a bolt of negative energy that burns up to 50 mana from the target unit. Burned mana is dealt as damage.",
        cooldown: 7,
      },
    },
    {
      hotkey: "X",
      label: "Immolation",
      iconPath: "buttons/command/BTNImmolationOn.blp",
      state: "active",
      tooltip: {
        name: "Immolation",
        type: "Toggle ability",
        description:
          "Engulfs the Demon Hunter in flames, dealing 10 damage per second to nearby enemy units. Drains mana while active.",
        manaCost: 35,
      },
    },
    {
      hotkey: "C",
      label: "Evasion",
      iconPath: "buttons/command/BTNEvasion.blp",
      state: "passive",
      tooltip: {
        name: "Evasion",
        type: "Passive ability",
        description: "Gives the Demon Hunter a 10% chance to evade an attack.",
      },
    },
    {
      hotkey: "V",
      label: "Metamorphosis",
      iconPath: "buttons/command/BTNMetamorphosis.blp",
      state: "ready",
      tooltip: {
        name: "Metamorphosis",
        type: "Ultimate ability",
        description:
          "Transforms the Demon Hunter into a powerful ranged demon with 500 bonus hit points and a chaos ranged attack for 60 seconds.",
        manaCost: 150,
        cooldown: 120,
      },
    },
  ],
  // Death Knight
  Undead: [
    MOVE,
    STOP,
    HOLD,
    ATTACK,
    PATROL,
    EMPTY,
    EMPTY,
    LEARN,
    {
      hotkey: "Z",
      label: "Death Coil",
      iconPath: "buttons/command/BTNDeathCoil.blp",
      state: "ready",
      tooltip: {
        name: "Death Coil",
        type: "Active ability",
        description:
          "A coil of death that can damage an enemy living unit for 100 damage or heal a friendly Undead unit for 200 hit points.",
        manaCost: 75,
        cooldown: 6,
      },
    },
    {
      hotkey: "X",
      label: "Death Pact",
      iconPath: "buttons/command/BTNDeathPact.blp",
      state: "ready",
      tooltip: {
        name: "Death Pact",
        type: "Active ability",
        description:
          "Kills a target friendly Undead unit, converting 100% of its hit points into health for the Death Knight.",
        cooldown: 8,
      },
    },
    {
      hotkey: "C",
      label: "Unholy Aura",
      iconPath: "buttons/command/BTNUnholyAura.blp",
      state: "passive",
      tooltip: {
        name: "Unholy Aura",
        type: "Passive ability",
        description:
          "Increases nearby friendly units' movement speed by 10% and life regeneration rate.",
      },
    },
    {
      hotkey: "V",
      label: "Animate Dead",
      iconPath: "buttons/command/BTNAnimateDead.blp",
      state: "active",
      tooltip: {
        name: "Animate Dead",
        type: "Ultimate ability",
        description:
          "Raises 6 dead units in an area to fight for the Death Knight for 120 seconds.",
        manaCost: 175,
        cooldown: 240,
      },
    },
  ],
};

const HERO_TABLE_COLUMNS: TableColumn[] = [
  {
    key: "hero",
    header: "Hero",
    width: "28%",
    headerTooltip: {
      content: <span style={{ color: "#d8d0b8" }}>Race hero unit</span>,
    },
  },
  {
    key: "gold",
    header: "Gold",
    align: "center",
    headerTooltip: {
      content: <span style={{ color: "#fcd312" }}>Gold cost to summon</span>,
    },
  },
  {
    key: "lumber",
    header: "Lumber",
    align: "center",
    headerTooltip: {
      content: <span style={{ color: "#00c000" }}>Lumber cost to summon</span>,
    },
  },
  { key: "hp", header: "HP", align: "right" },
  { key: "mp", header: "MP", align: "right" },
  { key: "attack", header: "Attack", align: "right" },
];

const HERO_TABLE_ROWS: TableRowDef[] = [
  {
    id: "archmage",
    cells: {
      hero: {
        iconPath: "buttons/command/BTNHeroPaladin.blp",
        iconSize: 22,
        value: "Archmage",
        tooltip: {
          icon: <BlpIcon path="buttons/command/BTNHeroPaladin.blp" size={38} />,
          content: (
            <>
              <div>
                <strong style={{ color: "#fcd312" }}>Archmage</strong>
              </div>
              <div style={{ color: "#809fff", fontSize: 11 }}>Human Hero</div>
              <div style={{ marginTop: 4, color: "#ccc", fontSize: 12 }}>
                Master of the arcane arts. Excels at area denial and mana
                support.
              </div>
            </>
          ),
        },
      },
      gold: { resourceIcon: "gold", value: 425 },
      lumber: { resourceIcon: "lumber", value: 100 },
      hp: { value: 625 },
      mp: { value: 300 },
      attack: { value: "23–29" },
    },
  },
  {
    id: "blademaster",
    highlighted: true,
    cells: {
      hero: {
        iconPath: "buttons/command/BTNHeroBlademaster.blp",
        iconSize: 22,
        value: "Blademaster",
        tooltip: {
          icon: (
            <BlpIcon path="buttons/command/BTNHeroBlademaster.blp" size={38} />
          ),
          content: (
            <>
              <div>
                <strong style={{ color: "#fcd312" }}>Blademaster</strong>
              </div>
              <div style={{ color: "#809fff", fontSize: 11 }}>Orc Hero</div>
              <div style={{ marginTop: 4, color: "#ccc", fontSize: 12 }}>
                Swift melee duelist with high burst damage and Wind Walk.
              </div>
            </>
          ),
        },
      },
      gold: { resourceIcon: "gold", value: 425 },
      lumber: { resourceIcon: "lumber", value: 100 },
      hp: { value: 750 },
      mp: { value: 225 },
      attack: { value: "29–39" },
    },
  },
  {
    id: "demonhunter",
    cells: {
      hero: {
        iconPath: "buttons/command/BTNHeroDemonHunter.blp",
        iconSize: 22,
        value: "Demon Hunter",
        tooltip: {
          icon: (
            <BlpIcon path="buttons/command/BTNHeroDemonHunter.blp" size={38} />
          ),
          content: (
            <>
              <div>
                <strong style={{ color: "#fcd312" }}>Demon Hunter</strong>
              </div>
              <div style={{ color: "#809fff", fontSize: 11 }}>
                Night Elf Hero
              </div>
              <div style={{ marginTop: 4, color: "#ccc", fontSize: 12 }}>
                Agile anti-caster with Mana Burn and Metamorphosis ultimate.
              </div>
            </>
          ),
        },
      },
      gold: { resourceIcon: "gold", value: 425 },
      lumber: { resourceIcon: "lumber", value: 100 },
      hp: { value: 700 },
      mp: { value: 270 },
      attack: { value: "26–36" },
    },
  },
  {
    id: "deathknight",
    cells: {
      hero: {
        iconPath: "buttons/command/BTNHeroDeathKnight.blp",
        iconSize: 22,
        value: "Death Knight",
        tooltip: {
          icon: (
            <BlpIcon path="buttons/command/BTNHeroDeathKnight.blp" size={38} />
          ),
          content: (
            <>
              <div>
                <strong style={{ color: "#fcd312" }}>Death Knight</strong>
              </div>
              <div style={{ color: "#809fff", fontSize: 11 }}>Undead Hero</div>
              <div style={{ marginTop: 4, color: "#ccc", fontSize: 12 }}>
                Durable sustain hero with Unholy Aura and Animate Dead ultimate.
              </div>
            </>
          ),
        },
      },
      gold: { resourceIcon: "gold", value: 425 },
      lumber: { resourceIcon: "lumber", value: 100 },
      hp: { value: 775 },
      mp: { value: 225 },
      attack: { value: "32–38" },
    },
  },
];

const MAP_LIST = [
  "Lost Temple",
  "Turtle Rock",
  "Gnoll Wood",
  "Plunder Isle",
  "Twisted Meadows",
  "Echo Isles",
  "Terenas Stand",
  "Centaur Grove",
] as const;

function App() {
  const race = useCurrentRace();

  const [tipsEnabled, setTipsEnabled] = useState(false);
  const [musicVolume, setMusicVolume] = useState(0.7);
  const [sfxVolume, setSfxVolume] = useState(0.5);
  const [playerName, setPlayerName] = useState("");
  const [selectedMap, setSelectedMap] = useState<string>("Lost Temple");
  const [hpPct, setHpPct] = useState(0);
  const [mpPct, setMpPct] = useState(0);
  const [xpPct, setXpPct] = useState(0);
  const [buildPct, setBuildPct] = useState(0);
  const [loadPct, setLoadPct] = useState(0);

  useEffect(() => {
    const bars = [
      { set: setHpPct, step: 0.8, interval: 120 },
      { set: setMpPct, step: 1.2, interval: 90 },
      { set: setXpPct, step: 0.5, interval: 150 },
      { set: setBuildPct, step: 1.5, interval: 100 },
      { set: setLoadPct, step: 0.6, interval: 130 },
    ];
    const timers = bars.map(({ set, step, interval }) =>
      setInterval(() => {
        set((prev) => (prev >= 100 ? 0 : Math.min(100, prev + step)));
      }, interval),
    );
    return () => timers.forEach(clearInterval);
  }, []);

  return (
    <div className="wc3-scene">
      <CursorOverlay />

      {/* Site logo */}
      <div className="demo-site-logo">
        <img src="./site-logo.png" alt="Warcraft III UI" />
      </div>

      {/* Race selector */}
      <header className="demo-race-bar">
        {RACES.map((r: Race) => (
          <EscOptionButton key={r} onClick={() => setCurrentRace(r)}>
            {r === "NightElf" ? "Night Elf" : r}
          </EscOptionButton>
        ))}
      </header>

      {/* Main content */}
      <main className="demo-grid">
        {/* ── Column 1: Hero + ESC Menu Controls ── */}
        <div className="demo-col">
          <SectionTitle title="Hero Portrait">
            <div className="demo-hero-wrap">
              <HeroPortraitModel race={race} />
            </div>
          </SectionTitle>

          <SectionTitle title="Resources">
            <ResourceCounter />
          </SectionTitle>
          <SectionTitle title="Command Card">
            <CommandCard slots={HERO_COMMAND_CARDS[race]} />
          </SectionTitle>
          <SectionTitle title="Worker Unit">
            <div className="demo-worker-wrap">
              <WorkerUnitModel race={race} side="left" />
            </div>
          </SectionTitle>
        </div>

        {/* ── Column 3: Glue Buttons ── */}
        <div className="demo-col">
          <SectionTitle title="Screen Button">
            <GlueScreenButton onClick={() => alert("Click!")}>
              Single Player
            </GlueScreenButton>
            <GlueScreenButton disabled>Disabled</GlueScreenButton>
          </SectionTitle>

          <SectionTitle title="Bordered Button">
            <GlueBorderedButton onClick={() => alert("Click!")}>
              Create Game
            </GlueBorderedButton>
          </SectionTitle>

          <SectionTitle title="Option Buttons" className="demo-center">
            <EscOptionButton
              onClick={() =>
                setCurrentRace(RACES[Math.floor(Math.random() * RACES.length)])
              }
            >
              Random Race
            </EscOptionButton>
            <EscOptionButton disabled>Disabled</EscOptionButton>
          </SectionTitle>
          <SectionTitle title="Menu Button" className="demo-center">
            <div className="demo-btn-stack">
              <GlueMenuButton onClick={() => alert("OK")}>OK</GlueMenuButton>
              <GlueMenuButton variant="single" onClick={() => alert("Cancel")}>
                Cancel
              </GlueMenuButton>
            </div>
          </SectionTitle>

          <SectionTitle title="Campaign Button">
            <GlueCampaignButton onClick={() => alert("Campaign!")}>
              Human Campaign
            </GlueCampaignButton>
          </SectionTitle>

          <SectionTitle title="Sliders">
            <EscSlider
              label="Music"
              value={musicVolume}
              onChange={setMusicVolume}
            />
            <EscSlider label="SFX" value={sfxVolume} onChange={setSfxVolume} />
          </SectionTitle>

          <SectionTitle title="Input Box">
            <InputBox
              value={playerName}
              placeholder="Player name..."
              onChange={setPlayerName}
            />
          </SectionTitle>
          <SectionTitle title="Dropdown">
            <GlueDropdown
              options={MAP_LIST}
              value={selectedMap as (typeof MAP_LIST)[number]}
              onChange={setSelectedMap}
            />
          </SectionTitle>

          <SectionTitle title="List Box + Scrollbar">
            <GlueListBox
              items={MAP_LIST}
              value={selectedMap as (typeof MAP_LIST)[number]}
              onChange={setSelectedMap}
              height={140}
            />
          </SectionTitle>
        </div>

        {/* ── Column 4: CommandCard, Bars, Panels ── */}
        <div className="demo-col">
          <SectionTitle title="Headings">
            <Heading level={1}>Heading 1</Heading>
            <Heading
              level={2}
              icon={<BlpIcon path="resources/ResourceGold.blp" size={20} />}
            >
              Heading 2
            </Heading>
            <Heading
              level={3}
              icon={<BlpIcon path="resources/ResourceLumber.blp" size={16} />}
            >
              Heading 3
            </Heading>
            <Heading level={4}>Heading 4</Heading>
          </SectionTitle>

          <SectionTitle title="Stat Bars">
            <StatBar
              label="HP"
              type="health"
              fillPercent={hpPct}
              maxValue={1200}
            />
            <StatBar
              label="MP"
              type="mana"
              fillPercent={mpPct}
              maxValue={400}
            />
            <StatBar type="xp" fillPercent={xpPct} hasBorder />
            <StatBar type="build" fillPercent={buildPct} hasBorder />
          </SectionTitle>

          <SectionTitle title="Loading Bar">
            <LoadingBar progress={loadPct} />
          </SectionTitle>

          <SectionTitle title="Menu Panel">
            <MenuPanel style={{ minHeight: 60, padding: 12 }}>
              <span className="demo-panel-text">Panel content goes here</span>
            </MenuPanel>
          </SectionTitle>

          <SectionTitle title="Checkboxes">
            <EscCheckbox
              label="Show tips"
              checked={tipsEnabled}
              onChange={setTipsEnabled}
            />
            <EscCheckbox label="Disabled" checked disabled />
          </SectionTitle>

          <SectionTitle title="Radio Buttons">
            <div className="demo-radio-group">
              {RACES.map((r: Race) => (
                <EscRadioButton
                  key={r}
                  label={r}
                  selected={race === r}
                  onSelect={() => setCurrentRace(r)}
                />
              ))}
            </div>
          </SectionTitle>
          <SectionTitle title="Tooltip">
            <Tooltip
              icon={
                <BlpIcon
                  path="./buttons/command/BTNBrillianceAura.blp"
                  size={38}
                />
              }
            >
              <div>
                <strong style={{ color: "#fcd312" }}>Brilliance Aura</strong>
              </div>
              <div style={{ color: "#00c000", fontSize: 11 }}>
                Passive ability
              </div>
              <div style={{ marginTop: 4, color: "#ccc", fontSize: 12 }}>
                Increases nearby allied units&apos; mana regeneration by 0.75
                per second.
              </div>
            </Tooltip>
            <Tooltip>
              <div>
                <strong style={{ color: "#fcd312" }}>Without icon</strong>
              </div>
              <div style={{ marginTop: 4, color: "#ccc", fontSize: 12 }}>
                Tooltips still work without an icon.
              </div>
            </Tooltip>
          </SectionTitle>

          <SectionTitle title="Table">
            <div style={{ width: 520 }}>
              <Table
                columns={HERO_TABLE_COLUMNS}
                rows={HERO_TABLE_ROWS}
                showRowNumbers
              />
            </div>
          </SectionTitle>
        </div>
      </main>
    </div>
  );
}

export default App;
