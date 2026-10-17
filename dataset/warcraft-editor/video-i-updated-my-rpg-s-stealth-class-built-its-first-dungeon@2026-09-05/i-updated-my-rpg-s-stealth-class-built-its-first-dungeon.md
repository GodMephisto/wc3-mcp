---
type: vendor-doc
package: video-rosk
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=R3Qis9wl600
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 372
chapters: 11
timestamped: yes
---

# I Updated My RPG's Stealth Class & Built Its First Dungeon

Source: <https://www.youtube.com/watch?v=R3Qis9wl600>

Uploader: Rosk | Duration: 6m12s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Welcome back to the Rosk devlog! In this first update, I'm adding some depth to the game with a big update of the Scoundrel class, the blockout of the very first dungeon, and new tech that changes how you play. From new stealth mechanics and combo points to smarter combat with interrupts and dedicated servers for dungeons, I've been busy!

Watch as I break down the design behind the new abilities, take a tour of the "Magikite Research Facility BR-33Z," and show off the new monsters that call it home.

--- TIMESTAMPS ---
0:00 - Intro & What's New
0:56 - The Scoundrel Rework: Stealth
1:23 - Scoundrel Rework: Sap & Crowd Control
1:41 - Scoundrel Rework: Combo Points & Finishers
1:58- Designing Our First Dungeon: Magikite Research Facility
2:34 - New Dungeon Enemies: The Gunner & Magikite Mutant
3:20 - A Huge Combat Upgrade: Spell Interrupts!
3:56 - UI Update: New Login Screen & Settings Menu
4:22 - UI Update: The New Character Selection Map
4:35 - The Invisible Magic: How We Deploy Dedicated Dungeon Servers
5:20 - What's Next & Outro

--- ABOUT OUR GAME ---
Rosk is an in-development Coop online RPG inspired by games like World of Warcraft developed with Unreal Engine. Rosk is not an MMO, you and up to 3 friends can run dungeons with ever increasing difficulty! Beat the timer, get the gear, and tame the wild and savage land of Rosk! All game footage is pre-alpha and subject to change. Please like and subscribe to stay updated!

--- FOLLOW THE JOURNEY ---
✅ Follow me on Bluesky! https://bsky.app/profile/Rootcanalentertainment.com
✅ Join our community on Discord to chat with us! https://discord.gg/BfGXyVGE6j

#gamedev #indiegame #devlog #madewithunity #rpg #worldbuilding #gamedesign #indiedev

## Chapters

- [0:00](https://www.youtube.com/watch?v=R3Qis9wl600&t=0s) Intro & What's New
- [0:56](https://www.youtube.com/watch?v=R3Qis9wl600&t=56s) The Scoundrel Rework: Stealth
- [1:23](https://www.youtube.com/watch?v=R3Qis9wl600&t=83s) Scoundrel Rework: Sap & Crowd Control
- [1:41](https://www.youtube.com/watch?v=R3Qis9wl600&t=101s) Scoundrel Rework: Combo Points & Finishers
- [1:58](https://www.youtube.com/watch?v=R3Qis9wl600&t=118s) Designing Our First Dungeon: Magikite Research Facility
- [2:34](https://www.youtube.com/watch?v=R3Qis9wl600&t=154s) New Dungeon Enemies: The Gunner & Magikite Mutant
- [3:20](https://www.youtube.com/watch?v=R3Qis9wl600&t=200s) A Huge Combat Upgrade: Spell Interrupts!
- [3:56](https://www.youtube.com/watch?v=R3Qis9wl600&t=236s) UI Update: New Login Screen & Settings Menu
- [4:22](https://www.youtube.com/watch?v=R3Qis9wl600&t=262s) UI Update: The New Character Selection Map
- [4:35](https://www.youtube.com/watch?v=R3Qis9wl600&t=275s) The Invisible Magic: How We Deploy Dedicated Dungeon Servers
- [5:20](https://www.youtube.com/watch?v=R3Qis9wl600&t=320s) What's Next & Outro

## Transcript

**[0:09](https://www.youtube.com/watch?v=R3Qis9wl600&t=9s)** [Music] Hey everyone and welcome back to the Rosk devlog. For the past few weeks, I've been focused on adding some depth to Rosk. not just new places to explore, but new ways to play, new tactical challenges to overcome, and even a dash of polish to the player login experience. If you didn't watch my last video, let me explain Rosk.

**[0:38](https://www.youtube.com/watch?v=R3Qis9wl600&t=38s)** Rosk is a fourplayer co-op dungeon crawler RPG, similar to dungeons in World of Warcraft or Final Fantasy 14, but not an MMO. There is no leveling, no overworld, no fluff. Now, let's get things started with our favorite class that operates from the shadows.

**[0:59](https://www.youtube.com/watch?v=R3Qis9wl600&t=59s)** Let's talk about the scoundrel. I wanted this class to feel like a classic World of Warcraft rogue, a sneaky opportunist, as well as a damage dealer. To get there, I've introduced three core mechanics.

**[1:13](https://www.youtube.com/watch?v=R3Qis9wl600&t=73s)** The first and most iconic, of course, is stealth. When activated, the scoundrel fades from view, allowing them to bypass enemies or more importantly, set up the perfect opening attack.

**[1:25](https://www.youtube.com/watch?v=R3Qis9wl600&t=85s)** While stealth, the scoundrel gains access to sap, the crowdcontrol ability we all know and love. This lets you temporarily incapacitate an enemy, which is perfect for handling tricky group encounters. You can sap one target and focus on another.

**[1:41](https://www.youtube.com/watch?v=R3Qis9wl600&t=101s)** Finally, I've added the classic combo points. Certain abilities now build points, which you can then spend on powerful finishing moves. This adds some rhythm to combat, making you think about your rotation to maximize damage. It's all about building up for that devastating final blow. Now, a newly updated class needs a new place to shine. I'm excited to give you a first look at the blockout for our first

**[2:05](https://www.youtube.com/watch?v=R3Qis9wl600&t=125s)** dungeon, the Magikite Research Facility, BR33Z.

**[2:10](https://www.youtube.com/watch?v=R3Qis9wl600&t=130s)** Right now, it is in a blockout phase.

**[2:12](https://www.youtube.com/watch?v=R3Qis9wl600&t=132s)** That means I'm focusing entirely on the layout and pacing, the feel of the space before a single final art asset goes in.

**[2:19](https://www.youtube.com/watch?v=R3Qis9wl600&t=139s)** I'm making sure the hallways feel tense, the arenas are the right size for boss fights, and the exploration flows naturally. I've added some mobs and a few decorations to give it a theme for each area. And there are three bosses, but I'm going to leave the third a mystery for now.

**[2:35](https://www.youtube.com/watch?v=R3Qis9wl600&t=155s)** But a dungeon is only as good as the things trying to kill you in it. I've created two new mobs specifically for this dungeon. First, the Montre gunner.

**[2:43](https://www.youtube.com/watch?v=R3Qis9wl600&t=163s)** These guys will try to keep you at a distance with their Magikite blasting rifles.

**[2:48](https://www.youtube.com/watch?v=R3Qis9wl600&t=168s)** Make sure to focus them down so they don't kill your tank from the back.

**[2:54](https://www.youtube.com/watch?v=R3Qis9wl600&t=174s)** And second are the terrifying Magikite mutants. These poor peasants got caught by the Montre Empire and have been twisted into hulking brutes. Steer clear of their Magikite slam ability shown by the indicator on the floor.

**[3:08](https://www.youtube.com/watch?v=R3Qis9wl600&t=188s)** By the way, if you're interested in the lore of Rosk, please leave a comment down below. I'd love to tell you more about the Mantres, Empire, and Magikite.

**[3:15](https://www.youtube.com/watch?v=R3Qis9wl600&t=195s)** But for now, just know they're the bad guys.

**[3:21](https://www.youtube.com/watch?v=R3Qis9wl600&t=201s)** Creating these new mobs with castable abilities immediately highlighted a missing piece in my combat system, interrupts.

**[3:28](https://www.youtube.com/watch?v=R3Qis9wl600&t=208s)** Previously, if an enemy started casting a big scary spell, there was nothing you could do but brace for impact and hope your oracle was nearby for a quick res.

**[3:37](https://www.youtube.com/watch?v=R3Qis9wl600&t=217s)** Not anymore. I've added an interrupt to the spell slinger, allowing you to counter enemy spell casting. To make this a little easier, I also added a new cast bar that appears on the enemy's health bar as well as the target UI. Now you have the information and the tools to react, adding a more tactical layer to combat.

**[3:57](https://www.youtube.com/watch?v=R3Qis9wl600&t=237s)** Finally, I needed to zoom out from the moment to moment gameplay to improve the entire journey starting from the second you launch the game. I have a brand new login screen to set the mood and replace the old plain picture. I made it myself using the landscape editing tool. And even though I'm not the best at level design, I'm actually pretty proud of it.

**[4:16](https://www.youtube.com/watch?v=R3Qis9wl600&t=256s)** Once you're logged in, you'll find a fully functional settings menu to tweak graphics and audio to your liking.

**[4:22](https://www.youtube.com/watch?v=R3Qis9wl600&t=262s)** I also updated the character selection map. It was feeling a little bit bare before. And my favorite addition is the one in 100 chance to spawn this little guy.

**[4:34](https://www.youtube.com/watch?v=R3Qis9wl600&t=274s)** [Music] And this is the real big update. After you log in and find yourself in the Scarlet Tavern, you open your dungeon finder and select a dungeon. You and your party are whisked away to a new map. But how do I make that a smooth, private experience for you and your group? This was a big back-end project, and I'm happy to announce I have finally, finally implemented dungeon servers on demand. Basically, when you

**[5:00](https://www.youtube.com/watch?v=R3Qis9wl600&t=300s)** choose to enter a dungeon, the main server hands you off to a brand new private instant server just for your dungeon run. This ensures your experience is lag-free and isolated.

**[5:10](https://www.youtube.com/watch?v=R3Qis9wl600&t=310s)** Once you're finished, it seamlessly returns you back to Breeze. It was a little complicated to get working, but it's crucial for scaling the game and making instance content feel great. So, from combo points and interrupts to the new dungeon and server architecture that runs it all, it's been a big couple of weeks. The scoundrel is feeling deadly.

**[5:28](https://www.youtube.com/watch?v=R3Qis9wl600&t=328s)** The first dungeon is taking shape, and the game's foundation is stronger than ever. My next big step is to start the art pass on the Magikite facility, turning those big blue boxes into a fully realized atmospheric location. You won't want to miss that. The other major things I need to get done is an inventory system, equipment, and character customization.

**[5:48](https://www.youtube.com/watch?v=R3Qis9wl600&t=348s)** After that, I should be ready to start opening up the play test to more people and get some alpha servers going for general use. If you're excited about Rosk, the best way you can help out is by liking the video and subscribing to the channel. It really helps the YouTube algorithm send this video to the right audience. And if you want to hang out with other people excited for Rosk, we do have a Discord you can join with a

**[6:08](https://www.youtube.com/watch?v=R3Qis9wl600&t=368s)** link in the description. Thank you so much for watching and I will see you in the
