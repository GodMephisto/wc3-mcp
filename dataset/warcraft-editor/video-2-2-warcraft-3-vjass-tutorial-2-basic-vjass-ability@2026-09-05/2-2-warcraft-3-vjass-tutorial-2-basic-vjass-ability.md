---
type: vendor-doc
package: video-jokerjassvids
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=3i6GwCe328U
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 341
timestamped: yes
---

# [2/2]Warcraft 3 vJass Tutorial 2: Basic vJass Ability

Source: <https://www.youtube.com/watch?v=3i6GwCe328U>

Uploader: JokerJassVids | Duration: 5m41s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Hopefully I improved from the first.

Recommended JASS modding sites:
http://www.thehelper.net 
http://www.wc3c.net

NewGen:
http://www.wc3c.net/showthread.php?t=90999

## Transcript

**[0:00](https://www.youtube.com/watch?v=3i6GwCe328U&t=0s)** when a unit casts a spell and the spell ID is equal to this raw code value then it will run these functions where it will wait two seconds it will kill the Target and create an animation so let us compile the code and test it to see what happens sorry about that and look the unit died and there's a respawn code from earlier uh coming into effect now if you look your hero isn't gaining any XP and this is

**[0:57](https://www.youtube.com/watch?v=3i6GwCe328U&t=57s)** because the native we are using right now doesn't take into account the killer of the unit so in order to fix that there is another function you can use called Unit damage targets and this has a lot of parameters it takes the Caster the target the damage amount just use any big value uh Boolean attack this is for checks uh for some abilities like um like Thorns Aura I suppose so we will set this to false

**[1:56](https://www.youtube.com/watch?v=3i6GwCe328U&t=116s)** Boolean ranged this is also for some ability where let's say true shot aura that only gives or no that was a bad example my bad uh some sort of Aura I suppose but we can set this to false also and if we scroll over we should be able to see more parameters uh I guess not here we will just open up this and look at it so it takes the Caster Target the damage takes or after what whatever we have we have attack

**[2:43](https://www.youtube.com/watch?v=3i6GwCe328U&t=163s)** type um this is the same as your normal unit attacks so we will set this to normal uh Dam damage type um there there's many damage types here I will show them to you some of them do weird things with magic immunity and such but here we go all these but usually what you want to use is damage typee normal that's your regular spell damage or you could use Universal for ultimates that you want to you want this

**[3:40](https://www.youtube.com/watch?v=3i6GwCe328U&t=220s)** ability damage to go through magic immunity so we will use normal and then there's a last parameter which asks for weapon type this is for the sound of the damage which for most ability abilities shouldn't have any so some people put null that does the same thing but others use weapon type who knows this is the same thing as no there's no sound for it so this will make sure that the Caster will kill

**[4:26](https://www.youtube.com/watch?v=3i6GwCe328U&t=266s)** the Target or at least damage it which in this case is enough to kill the target so let us save it and let's run it and there you go XP so hopefully this helped you in better understanding how Jazz works I guess I'm not quite into V Jazz yet except for the privates and the Scopes but next tutorial I plan on showing you guys a little bit more of V Jazz's features so thanks for watching
