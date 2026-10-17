---
type: vendor-doc
package: video-ogre-magi
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=fkaPrW05taY
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 137
chapters: 11
timestamped: yes
---

# How to make Dummy Unit (JASS Code) #worldeditor #warcraft3 #ogrewe

Source: <https://www.youtube.com/watch?v=fkaPrW05taY>

Uploader: Ogre Magi | Duration: 2m17s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

In this video, I'm going to show you how I set up a dummy unit that can auto-cast a spell on any target at any time—without needing player control. Make sure to check out my previous video on folder structure so you’ll understand my code better.

Timeline:
- Intro 0:00 - 0:06
- What is Dummy Unit - 0:06 - 0:15
- Example skill - 0:16 - 0:24
- Create Dummy Unit - 0:25 - 0:39
- Create skill for Dummy - 0:40 - 0:45
- Code Dummy.j - 0:46 - 1:25
- Make channel ability for hero - 1:26 - 1:35
- Code Event Spell.j - 1:36 - 1:43
- Test game - 1:44 - 1:54
- About Dummy SFX - 1:55 - 2:12
- Outro - 2:13 - 2:16

## Chapters

- [0:00](https://www.youtube.com/watch?v=fkaPrW05taY&t=0s) Intro.
- [0:06](https://www.youtube.com/watch?v=fkaPrW05taY&t=6s) What is Dummy Unit.
- [0:16](https://www.youtube.com/watch?v=fkaPrW05taY&t=16s) Example skill.
- [0:25](https://www.youtube.com/watch?v=fkaPrW05taY&t=25s) Create Dummy Unit.
- [0:40](https://www.youtube.com/watch?v=fkaPrW05taY&t=40s) Create skill for Dummy.
- [0:46](https://www.youtube.com/watch?v=fkaPrW05taY&t=46s) Code Dummy.j.
- [1:26](https://www.youtube.com/watch?v=fkaPrW05taY&t=86s) Make channel ability for hero.
- [1:36](https://www.youtube.com/watch?v=fkaPrW05taY&t=96s) Code Event Spell.j.
- [1:44](https://www.youtube.com/watch?v=fkaPrW05taY&t=104s) Test game.
- [1:55](https://www.youtube.com/watch?v=fkaPrW05taY&t=115s) About Dummy SFX.
- [2:13](https://www.youtube.com/watch?v=fkaPrW05taY&t=133s) Outro.

## Transcript

**[0:00](https://www.youtube.com/watch?v=fkaPrW05taY&t=0s)** Hey buddy, it's me, Oc Ghi, again .  Today we're going to talk about Dummy Units.  So what is a dummy unit?  A dummy unit is an invisible unit used to cast secondary skills, and it will cast automatically without requiring player control.

**[0:16](https://www.youtube.com/watch?v=fkaPrW05taY&t=16s)** Now we will have an illustrative example.  I'm going to create a skill where, when used, all nearby enemies are cursed, and the user's attacks have a chance to miss.   Let 's get started!  The first step is to create a dummy unit.  You need to import the Dummy MDX model.  Of course, this model is invisible .  Assign a model to the Dummy Unit and modify the Dummy Unit settings to match my Dummy Unit

**[0:37](https://www.youtube.com/watch?v=fkaPrW05taY&t=37s)** in Object Manager (F6).  Next, you need a dummy skill customer.  A mana-free, cooldown-free custom skill with a range of 999,999.

**[0:47](https://www.youtube.com/watch?v=fkaPrW05taY&t=47s)** And now we need the code to initialize the dummy.

**[0:49](https://www.youtube.com/watch?v=fkaPrW05taY&t=49s)** This is my objective dummy code within the setup function.  I created a single dummy .  This function will be called in the game's settings file.

**[0:58](https://www.youtube.com/watch?v=fkaPrW05taY&t=58s)** Note that with just one dummy, it can cast multiple spells simultaneously.

**[1:04](https://www.youtube.com/watch?v=fkaPrW05taY&t=64s)** The prelot AB function will initiate the skill test .  You will need to do this before the game starts.  Here I need to activate the Dummy Cers skill I just created.  The ` Tit` function allows me to use Dummy Units to order skills to a specific target.

**[1:18](https://www.youtube.com/watch?v=fkaPrW05taY&t=78s)** You need to look up the correct string order for Dummy Scast's skill.  You can view it in Object Manager (F6).  And now we create skills for the Unit to use.  I use channeling skills to create skills for use in games.  Let's name it C on.  Pay attention to the settings in the skill.  Now we need to go into the event spell file and check if the skill ID used is the ID of C on.  So I'll use Dummy Order Skill C on all enemies within a

**[1:43](https://www.youtube.com/watch?v=fkaPrW05taY&t=103s)** 350 range.

**[1:46](https://www.youtube.com/watch?v=fkaPrW05taY&t=106s)** Let's test the game!  Oh yeah, the skill is working now.  There's another type of dummy, the one that uses targeting and channeling techniques.  I'll talk about it in another video.  Note that maps with versions lower than 1.28 will have an additional type of dummy .  This type of dummy will replace the skill animation to showcase the skill to the player, and is called Dummy FFX.  This changes when using maps in

**[2:09](https://www.youtube.com/watch?v=fkaPrW05taY&t=129s)** higher versions, as they use effects instead of dummy FFX.  I'm Map MakerG Magi, see you in the next video.
