---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-warcraft-3-reforged-animated-portraits.325536/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Warcraft 3 Reforged Animated Portraits
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-warcraft-3-reforged-animated-portraits.325536/>
#### InsaneMonster

#### InsaneMonster

Patron

 Level 39

JoinedJul 20, 2011Messages672

Resources: 23

#### Resources
Tools 1Maps 3Reforged HD Models 10Sounds 4JASS 1Lua 1Tutorials 3

##
 Warcraft 3 Re-Reforged


WARNING: This tutorial can be applied only to Warcraft 3 Reforged!

GOAL

To animate the portraits of any character, in any map, for any dialogue (consisting of a certain sound, a certain "spoken" text and a certain "speaker" name). Note that making a custom dialogue means to link togheter all that pieces in a custom way.

KNOWLEDGE REQUIRED

Intermediate knowledge of the trigger editor

Basic knowledge of JASS or Lua and GUI Custom Script action

How to import external files into a map
TOOLS REQUIRED

Warcraft 3 Reforged World Editor

Ladik's Casc Viewer

(optional) FaceFX (no save free edition)

If you prefer a video-tutorial instead, you can check the following video:

TABLE OF CONTENT

BACKGROUND (OPTIONAL)

EXPORTING ANIMATIONS USING CASC VIEWER

IMPORTING ANIMATIONS INTO FACEFX (OPTIONAL)

BLIZZARD FACIAL ANIMATIONS NAMING SCHEME

CONVERSATION.JSON FILES AND RESTORING LIP-SYNC

SETTING DIALOGUES PROPERTIES IN-EDITOR

BACKGROUND (OPTIONAL)

This section provides insights into the process of discovering the method explained in this tutorial and the technology used by Blizzard in Reforged. You can skip it if you are not interested.

From the Reforged beta itself, it was clear there was no easy way to customize or even keep animated potraits (with lip-sync) in unofficial built maps. Even re-saving a Blizzard's campaign map disabled the lip-sync of all characters (with some weirds exceptions) and there seemed to be no way to have them back. Not to mention the impossibility of adding custom dialogues with "some" lip-sync behind it. Even the texts of all dialogues seems to be somewhat hardcoded in the campaign.

In a post here, on Hive Workshop, Blizzard actually told lip-sync was a functionality not available to modders yet. This post is just some months old. Well, this is completely wrong, and the fact they hidden something inside their campaign files does not mean this something is not available to us, somehow. I find actually amazing how much misinformation they are spreading about their own game. This is very deterimental for it. For a game which is not going great... to say the least!

First of all, we need to understand why Reforged animated portraits are so much more complex than their Classic counterparts. Many think Blizzard should have added default "talking" animations, like in Classic.

Now, I think there is a reason they didn't. And that is they look bad. Reforged models are less cartoony and higher quality than Classic ones. Because of that, a "faked" lip-movement I feel it would look terrible.

I indeed think they did the right choice going with true lip-sync. And of course, they used FaceFX, which is the same tool they used for Starcraft 2, for the exact same purpose. With this tool, the process of lip-syncing is very easy, and it is a tool commonly used in the videogame industry. They have built some kind of in-engine integration for FaceFX. I believe it's like an "engine inside the engine", which is activated when some appropriate functions are called.

You may have noticed FaceFX to be very expensive. There is however a free version on their website, allowing only to inspect files. This version prevents you from saving anything. If you want to use FaceFX to inspect the facial animations of Reforged, you may be interested in downloading it. I'll show you how to use FaceFX to check the content of an animation file, but consider that in most cases this tutorial should contain all the information you need to skip that specific section, if you so wish.

The engine integration of FaceFX in Reforged seems to output a certain type of "log-files", the conversation.json files.

Note: actually, by talking directly with a Blizzard employee, it seems that conversation.json files are used by a Dialogue Editor which is not yet released inside the World Editor.

You can find conversation.json files inside almost any campaign map if you open the Asset Manager. Each one of these files is regenerated when the map is saved.

 While Re-Reforging the Prologue campaign, I noticed that when exporting the same conversation.json file from an edited campaign map there were some differences with respect to the original one. The interesting fact is that the edited one uses a "relative" animation set file-path, while the original does not.

This is probably due to some kind of relation set by Blizzard in their campaign file which, sadly, is not currently accessible. By investingating this behaviour I found that re-importing all the animations in the map actually solved the problem everyone was having of lip-sync not working on edited campaign maps!

All the references for the dialogue, I mean the links to animations, sounds, texts, etc. were still there, but for some reason the engine was searching the files in a different place.

What about making custom dialogues? Many thinks there is no function in the editor to link togheter all components of a dialogue, and so there is no way to animate a portrait. Actually, this is what Blizzard itself said. And they all are wrong.

User Solcius123, on the same post linked above, found that there are three strange identical functions in the trigger GUI editor. To find them you can do the following:

Click on any trigger on any map of your choice.

Right click inside the trigger and choose "New Condition"

Select "Boolean Comparison"

Select the left operand of the equality check and scroll up until you find three "Sound - Set Facial Animation Label"
These three functions, togheter with two other actions which are displayed correctly in the GUI editor, are the key. Infact I found that the functions are not bugged if the trigger is converted to JASS. From the converted trigger I could extrapolate the JASS functions used to set all the components of the dialogue. The functions were all there from the beginning, hidden in plain sight...

EXPORTING ANIMATIONS USING CASC VIEWER

You can skip this section if you already know how to extract .animset_ingame files from Reforged CASC storage or you plan to link directly the files in the game folder. Note that CASC Viewer could still be necessary to see the exact name and path of each one of these files.

Reforged uses for lip-sync .animset_ingame files, which are stripped down versions of the .animset files, the ones FaceFX actually process. These files contains the facial animations of a given character, each animation identified by a specific "name" and usually grouped together under a "group name".

To extract the .animset_ingame files you can use Ladik's Casc Viewer or any other tool able to open Reforged CASC storage. This is just like extracting any other file from the game CASC storage. Anyway, if you don't know how to do so, you can do the following:

Open Ladik's Casc Viewer.

Click on Game Storage in the upper left portion of the screen.

Click on Warcraft 3 Reforged in the dialog.

Open war3.war3mod.

Open _hd.war3mod.

Open _locales.

Open the locale you need, usually enus.war3mod, which is the english locale.

Now, if you want a face-animation used by Blizzard in the campaign open the folder sound.

Now open the folder dialogue.

Now open the folder faceanimation.

Select the mission of your choice and export the .animset_ingame file of the character you need.

I'll be exporting orcx03b/facialanimation/proudmoore.animset_ingame.

If you want instead to use a face-animation employed by Blizzard in the unit responses, open the folder units, then open the race and the unit of your choice. I'll be exporting human/heroarchmage/heroarchmage_portrait.sd.animset_ingame.

Note that the Archmage animset is also compatible with the High Elf Archmage model.

Once the files are extracted, you can import them into your map using the Asset Manager.

IMPORTING ANIMATIONS INTO FACEFX (OPTIONAL)

This section is optional. Use it only if the .animset_ingame files don't follow for some reason the naming scheme defined in the following section or, of course, if you are interested!

When the .animset_ingame files are extracted, you can import them into FaceFX to check their content. This is useful to understand animations names and group names, which are necessary to properly link them inside Reforged editor. I will show how to link all data inside the editor in the following sections. To import an .animset_ingame file inside FaceFX do the following:

Open FaceFX.

Click on Actor in the toolbar.

Click on Mount Animation Set.

Click yes in the dialog.

Use explorer to find the .animset_ingame file you want to open. In my case proudmoore.animset_ingame. Remember to set the filter to "All files" otherwise the .animset_ingame files will not be visible.

Now a warning appears. The warning is due to the fact .animset_ingame files are "stripped down" versions. Ignore the warning and press ok.

Another warning appears, detecting some inconsistencies. I don't know what this is all about, probably the same as the warning before. Just ignore this one too and click ok.

Now the file is opened. You cannot see anything, don't worry, it's normal. Just check the animation group name and you'll see now there is a name in the group name tab: Map-Proudmoore.

Beside it, you can see a list of animations you can choose from the drop down menu. Each one of these animations is connected to a certain sound file you can find in that map of the campaign.

Now we can do the same for the other animation set we've extracted. That is, heroarchmage_portrait.animset_ingame.

Notice that the group name is now BaseSD and each animation has the name of the respective sound file for that unit response during the game.

BLIZZARD FACIAL ANIMATIONS NAMING SCHEME

This section contains the naming scheme followed by Blizzard to define all the facial animations.

There is a quite precise naming scheme followed by Blizzard in naming the animations. From my research it appears to be the following:

For all unit responses in-game:

GROUP NAME: BaseSD

ANIMATION NAME: the same as the respective sound file

For all campaign dialogues:

GROUP NAME: character related in the form "Map-NameOfCharacter" (for example: "Map-Thrall", "Map-Proudmoore", "Map-Grunt", etc)

ANIMATION NAME: the same as the respective sound file

I've not checked them all, of course, but these rules should be quite accurate to let you avoid the entire FaceFX steps of this tutorial in most cases.

CONVERSATION.JSON FILES AND RESTORING LIP-SYNC

This section allows you to understand what the conversation.json files contain and how to restore already linked lip-sync in official campaign maps when they are edited. Note that understanding conversation.json files is required to properly link all components inside the editor in the last section!

 To see the content of a conversation.json file, open the Warcraft 3 Reforged editor on any Blizzard campaign map. Once it is loaded, open the Asset Manager and export the conversation.json file.

Once exported, open it with any text editor. You can see that the file uses a list of string (war3map.wts) and consists in a list of triggers in which dialogues appear in a certain order.

We need to inspect carefully the file to understand each component of the FaceFX integration. To my knowledge, the properties you can find inside each one of these files are the following:

"conversationOrder" is the order of the dialogue in the trigger. This is altered when you generate/reorder new dialogues in new or existing triggers.

"speakerModel" is just the model of the speaker.

"speakerNameId" is the id of the string contaning the name of the speaker (for example, Thrall). This is set with a specific function in the editor and not when calling the dialogue itself.

"speakerUnitID" is the id in the object editor of the unit which is speaking, if any.

"soundFile" is just the sound file used.

"dialogueId" is the id of the string containing the dialogue, the text displayed in the subtitles.

This is also set using a specific function in the editor and not when calling the dialogue itself.

"animationLabel" is the name of the animation as you can check in FaceFX or using the naming scheme I've provided. This is set with an "hidden" function in the editor and not when calling the dialogue itself.

"animationGroupLabel" is the group name of the animation as you can check in FaceFX or using the naming scheme I've provided. This is set with an "hidden" function in the editor too and not when calling the dialogue itself.

"animationSetFilepath" is the path of the .animset_ingame file for this dialogue. If relative, it uses the map Asset Manager folder as the root directory.

"animationSetFilepathMapRelative" is just a boolean (true, false) flag defining if the previous property uses a relative file-path or not.
It's easy to see that all the properties displayed above are what define a dialogue, so to speak. So now we know what to connect together to generate a new dialogue of our choice. We need only to understand how to connect all pieces, and we will do so in the next and last section.

Beside that, known that when an official campaign map is edited, more often than not the file-path is for some reason set to relative. Since there is no .animset_ingame file at the map relative path, the animations are broken even if all the links are still there. To restore them, just import in the edited map (using the Asset Manager) all the required .animset_ingame files at the path specified in the conversation.json. Remeber to export the conversation.json file once the map is already edited to get the proper file-path.

SETTING DIALOGUES PROPERTIES IN-EDITOR

This section explains the function used to properly link togheter all components of a dialogue in order to make custom dialogues. Note that making a custom dialogue is the necessary step to take in order to animate the portraits.

Now, we know what to link togheter to generate our custom dialogues but not how to do so.

The functions you need actually are just the following, written in JASS:

 JASS:

 call SetSoundFacialAnimationLabel(sound_variable, "animation_name (the animationLabel)")
 call SetSoundFacialAnimationGroupLabel(sound_variable, "animation_group_name (the animationGroupLabel)")
 call SetSoundFacialAnimationSetFilepath(sound_variable, "relative_path_to_animset OR game_asset_path")
 call SetDialogueTextKey(sound_variable, "spoken_text")
 call SetDialogueSpeakerNameKey(sound_variable, "speaker_name")

As you can see their names are quite intuitive. Also, all of them have a corrispective in the GUI trigger editor:

SetSoundFacialAnimationLabel is displayed as Sound - Set Facial Animation Label under boolean function when creating conditions.

SetSoundFacialAnimationGroupLabel is displayed as Sound - Set Facial Animation Label under boolean function when creating conditions.

SetSoundFacialAnimationSetFilepath is displayed as Sound - Set Facial Animation Label under boolean function when creating conditions.

SetDialogueTextKey is displayed as Cinematic - Set Dialogue Text ID under actions.

SetDialogueSpeakerNameKey is displayed as Cinematic - Set Dialogue Text ID under actions.
Since the first 3/5 of them the are somewhat bugged in the GUI implementation, you can use for all of them the JASS version. For example, you can write down custom JASS in the custom script code section in the trigger editor of your map:

This is an example of how I've organized all the functions in one of my maps, which is completely built from scratch and in which portraits are all fully animated. You can see I've made some additional helper functions to load what I need for each animation set:

 JASS:

function LoadD20Proudmore08FacialAnimations takes nothing returns nothing
 call SetSoundFacialAnimationLabel(gg_snd_D20Proudmoore08, "D20Proudmoore08")
 call SetSoundFacialAnimationGroupLabel(gg_snd_D20Proudmoore08, "Map-Proudmoore")
 call SetSoundFacialAnimationSetFilepath(gg_snd_D20Proudmoore08, "FacialAnimation/Proudmoore.animset")
 call SetDialogueTextKey( gg_snd_D20Proudmoore08, "For Lordaeron!" )
 call SetDialogueSpeakerNameKey( gg_snd_D20Proudmoore08, "Admiral Proudmoore" )
endfunction

function LoadD20Proudmore09FacialAnimations takes nothing returns nothing
 call SetSoundFacialAnimationLabel(gg_snd_D20Proudmoore09, "D20Proudmoore09")
 call SetSoundFacialAnimationGroupLabel(gg_snd_D20Proudmoore09, "Map-Proudmoore")
 call SetSoundFacialAnimationSetFilepath(gg_snd_D20Proudmoore09, "FacialAnimation/Proudmoore.animset")
 call SetDialogueTextKey( gg_snd_D20Proudmoore09, "For Sir Lothar!" )
 call SetDialogueSpeakerNameKey( gg_snd_D20Proudmoore09, "Admiral Proudmoore" )
endfunction

function LoadD20Proudmore10FacialAnimations takes nothing returns nothing
 call SetSoundFacialAnimationLabel(gg_snd_D20Proudmoore10, "D20Proudmoore10")
 call SetSoundFacialAnimationGroupLabel(gg_snd_D20Proudmoore10, "Map-Proudmoore")
 call SetSoundFacialAnimationSetFilepath(gg_snd_D20Proudmoore10, "FacialAnimation/Proudmoore.animset")
 call SetDialogueTextKey( gg_snd_D20Proudmoore10, "Death to the Blackbloods!" )
 call SetDialogueSpeakerNameKey( gg_snd_D20Proudmoore10, "Admiral Proudmoore" )
endfunction

function LoadO01Thrall22FacialAnimations takes nothing returns nothing
 call SetSoundFacialAnimationLabel(gg_snd_O01Thrall22, "O01Thrall22")
 call SetSoundFacialAnimationGroupLabel(gg_snd_O01Thrall22, "Map-Thrall")
 call SetSoundFacialAnimationSetFilepath(gg_snd_O01Thrall22, "FacialAnimation/ThrallO01.animset")
 call SetDialogueTextKey( gg_snd_O01Thrall22, "Lok'Tar! Lok'Tar!" )
 call SetDialogueSpeakerNameKey( gg_snd_O01Thrall22, "Thrall" )
endfunction

function LoadO07Thrall38FacialAnimations takes nothing returns nothing
 call SetSoundFacialAnimationLabel(gg_snd_O07Thrall38, "O07Thrall38")
 call SetSoundFacialAnimationGroupLabel(gg_snd_O07Thrall38, "Map-ThrallMountless")
 call SetSoundFacialAnimationSetFilepath(gg_snd_O07Thrall38, "FacialAnimation/ThrallO07.animset")
 call SetDialogueTextKey( gg_snd_O07Thrall38, "What the hell is going on here?!" )
 call SetDialogueSpeakerNameKey( gg_snd_O07Thrall38, "Thrall" )
endfunction

function LoadThrallPissed3FacialAnimations takes nothing returns nothing
 call SetSoundFacialAnimationLabel(gg_snd_ThrallPissed3, "ThrallPissed3")
 call SetSoundFacialAnimationGroupLabel(gg_snd_ThrallPissed3, "BaseSD")
 call SetSoundFacialAnimationSetFilepath(gg_snd_ThrallPissed3, "FacialAnimation/Thrall_Portrait.animset")
 call SetDialogueTextKey( gg_snd_ThrallPissed3, "The spirits will guide me..." )
 call SetDialogueSpeakerNameKey( gg_snd_ThrallPissed3, "Thrall" )
endfunction

function LoadThrallYesAttack1FacialAnimations takes nothing returns nothing
 call SetSoundFacialAnimationLabel(gg_snd_ThrallYesAttack1, "ThrallYesAttack1")
 call SetSoundFacialAnimationGroupLabel(gg_snd_ThrallYesAttack1, "BaseSD")
 call SetSoundFacialAnimationSetFilepath(gg_snd_ThrallYesAttack1, "FacialAnimation/Thrall_Portrait.animset")
 call SetDialogueTextKey( gg_snd_ThrallYesAttack1, "... Lok'Narosh!" )
 call SetDialogueSpeakerNameKey( gg_snd_ThrallYesAttack1, "Thrall" )
endfunction

function LoadHeroArchMageYesAttack3FacialAnimations takes nothing returns nothing
 call SetSoundFacialAnimationLabel(gg_snd_HeroArchMageYesAttack3, "HeroArchMageYesAttack3")
 call SetSoundFacialAnimationGroupLabel(gg_snd_HeroArchMageYesAttack3, "BaseSD")
 call SetSoundFacialAnimationSetFilepath(gg_snd_HeroArchMageYesAttack3, "FacialAnimation/HeroArchmage_Portrait.animset")
 call SetDialogueTextKey( gg_snd_HeroArchMageYesAttack3, "Infury frostaris sedaa!" )
 call SetDialogueSpeakerNameKey( gg_snd_HeroArchMageYesAttack3, "Kelen the Seeker" )
endfunction

For completion, here there is a Lua version:

 Lua:

-- Admiral Proudmoore

function LoadD20Proudmore08FacialAnimations()
 SetSoundFacialAnimationLabel(gg_snd_D20Proudmoore08, "D20Proudmoore08")
 SetSoundFacialAnimationGroupLabel(gg_snd_D20Proudmoore08, "Map-Proudmoore")
 SetSoundFacialAnimationSetFilepath(gg_snd_D20Proudmoore08, "Sound/Dialogue/FaceAnimation/Orcx03b/FacialAnimation/Proudmoore.animset")
end

function LoadD20Proudmore09FacialAnimations()
 SetSoundFacialAnimationLabel(gg_snd_D20Proudmoore09, "D20Proudmoore09")
 SetSoundFacialAnimationGroupLabel(gg_snd_D20Proudmoore09, "Map-Proudmoore")
 SetSoundFacialAnimationSetFilepath(gg_snd_D20Proudmoore09, "Sound/Dialogue/FaceAnimation/Orcx03b/FacialAnimation/Proudmoore.animset")
end

function LoadD20Proudmore10FacialAnimations()
 SetSoundFacialAnimationLabel(gg_snd_D20Proudmoore10, "D20Proudmoore10")
 SetSoundFacialAnimationGroupLabel(gg_snd_D20Proudmoore10, "Map-Proudmoore")
 SetSoundFacialAnimationSetFilepath(gg_snd_D20Proudmoore10, "Sound/Dialogue/FaceAnimation/Orcx03b/FacialAnimation/Proudmoore.animset")
end

-- Thrall

function LoadO01Thrall22FacialAnimations()
 SetSoundFacialAnimationLabel(gg_snd_O01Thrall22, "O01Thrall22")
 SetSoundFacialAnimationGroupLabel(gg_snd_O01Thrall22, "Map-Thrall")
 SetSoundFacialAnimationSetFilepath(gg_snd_O01Thrall22, "Sound/Dialogue/FaceAnimation/Orc01/FacialAnimation/Thrall.animset")
end

function LoadO07Thrall38FacialAnimations()
 SetSoundFacialAnimationLabel(gg_snd_O07Thrall38, "O07Thrall38")
 SetSoundFacialAnimationGroupLabel(gg_snd_O07Thrall38, "Map-ThrallMountless")
 SetSoundFacialAnimationSetFilepath(gg_snd_O07Thrall38, "Sound/Dialogue/FaceAnimation/Orc07/FacialAnimation/ThrallMountless.animset")
end

function LoadThrallPissed3FacialAnimations()
 SetSoundFacialAnimationLabel(gg_snd_ThrallPissed3, "ThrallPissed3")
 SetSoundFacialAnimationGroupLabel(gg_snd_ThrallPissed3, "BaseSD")
 SetSoundFacialAnimationSetFilepath(gg_snd_ThrallPissed3, "Units/Orc/Thrall/Thrall_Portrait.sd.animset")
end

function LoadThrallYesAttack1FacialAnimations()
 SetSoundFacialAnimationLabel(gg_snd_ThrallYesAttack1, "ThrallYesAttack1")
 SetSoundFacialAnimationGroupLabel(gg_snd_ThrallYesAttack1, "BaseSD")
 SetSoundFacialAnimationSetFilepath(gg_snd_ThrallYesAttack1, "Units/Orc/Thrall/Thrall_Portrait.sd.animset")
end

-- Kelen the Seeker

function LoadHeroArchMageYesAttack3FacialAnimations()
 SetSoundFacialAnimationLabel(gg_snd_HeroArchMageYesAttack3, "HeroArchMageYesAttack3")
 SetSoundFacialAnimationGroupLabel(gg_snd_HeroArchMageYesAttack3, "BaseSD")
 SetSoundFacialAnimationSetFilepath(gg_snd_HeroArchMageYesAttack3, "Units/Human/HeroArchmage/HeroArchmage_Portrait.sd.animset")
end

Please note that in this Lua version the two not bugged functions are called as GUI actions, because that allowed me to better use the localized facial animations in the game (for the Re-Reforged project). For the same reason, and to save memory space, I'm linking to path to the files contained in the game assets, instead to their relative path when such files are imported into the map. If you don't need localization, you can just stick to the custom script implementation provided above and easily converting the two missing functions to Lua by removing the call keyword. I would always suggest, though, to use the game file path instead of the relative ones, to avoid importing unnecessary assets.

To actually load the functions defined here, I've used a simple trigger during initalization in which, using the action Custom Script, I call each one of these functions to generate all the references in-game when I'm initializing the map:

CONCLUSIONS
And... this is it people!

You can now have animated portraits for all characters in any map. Now, I've used them with the respective sound files, but you can do whatever you want. It's a bit more complex than classic, I know, but the result is worth the effort in my opinion.

Hope you liked the tutorial, let me know if it's clear or not.

Have fun animating those characters!


 Last edited: Jun 6, 2021

#### Tamplier

#### Tamplier

 Level 48

JoinedSep 7, 2018Messages356

Resources: 94

#### Resources
Models 13Packs 3Skins 2Tools 2Maps 3Reforged HD Models 69Ported Models 1Reforged HD Skins 1

##
 Chronicles of the Second War


Is the conversation.json mandatory for dialogues now? Can we do the classic send transmission function?

#### InsaneMonster

#### InsaneMonster

Patron

 Level 39

JoinedJul 20, 2011Messages672

Resources: 23

#### Resources
Tools 1Maps 3Reforged HD Models 10Sounds 4JASS 1Lua 1Tutorials 3

##
 Warcraft 3 Re-Reforged


Tamplier777 said:

 Is the conversation.json mandatory for dialogues now? Can we do the classic send transmission function?


Click to expand...

If you want animated portraits you have to use dialogues. Dialogues are set-up linking togheter some elements as explained in the tutorial. If don't do this the FaceFX "engine" does not start and you cannot make the portraits move. The old way of doing that is gone by a decade. Starcraft 2 also it's the same, it's just less bugged in the way to actually run the portrait animations, but the principles are the same.

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Just WOW (no, not the mmorpg ^^).

I didn't even consider this kind of feature before, and I have to admit your research and the information you are providing is outstanding.

I seem to understand your explanations on why FaceFX was an obvious choice for Blizzard, and the fact that your proved it was indeed accessible to map makers opens a lot of potential for future custom campaigns (hoping Blizzard does not let us down on that one), but I believe it could also bring some spice to custom games.

I only went back to wc3 modding in December 2019, so at the moment, I am focusing on catching up with the community as it has evolved so much since my departure i,n 2004, but one day, just for the challenge and also for fun, I can't see how I could ignore all this.

When the time comes, your research will definitely become a precious help.

And for this, I am thankful already.

#### tulee

#### tulee

 Level 31

JoinedJul 26, 2008Messages1,575

Resources: 26

#### Resources
Maps 26

InsaneMonster said:

 The old way of doing that is gone by a decade.


Click to expand...

So wait, what you are saying is, even for normal transmissions on reforged (not talking about perfect lip sync, just the portrait moving its mouth) we need to go through this entire route you described above?

#### InsaneMonster

#### InsaneMonster

Patron

 Level 39

JoinedJul 20, 2011Messages672

Resources: 23

#### Resources
Tools 1Maps 3Reforged HD Models 10Sounds 4JASS 1Lua 1Tutorials 3

##
 Warcraft 3 Re-Reforged


Macadamia said:

 Just WOW (no, not the mmorpg ^^).

I didn't even consider this kind of feature before, and I have to admit your research and the information you are providing is outstanding.

I seem to understand your explanations on why FaceFX was an obvious choice for Blizzard, and the fact that your proved it was indeed accessible to map makers opens a lot of potential for future custom campaigns (hoping Blizzard does not let us down on that one), but I believe it could also bring some spice to custom games.

I only went back to wc3 modding in December 2019, so at the moment, I am focusing on catching up with the community as it has evolved so much since my departure i,n 2004, but one day, just for the challenge and also for fun, I can't see how I could ignore all this.

When the time comes, your research will definitely become a precious help.

And for this, I am thankful already.


Click to expand...

Thank you!

tulee said:

 So wait, what you are saying is, even for normal transmissions on reforged (not talking about perfect lip sync, just the portrait moving its mouth) we need to go through this entire route you described above?


Click to expand...

In most cases just the not-optional sections. I took 10 minutes to set-up all the portraits once discovered how to do it. It's quite fast if you know what to search. I explained the entire process very in-depth for those interested about why some things are done the way they are. Also I believe that understand something makes you more efficient in doing that very something.

#### Tamplier

#### Tamplier

 Level 48

JoinedSep 7, 2018Messages356

Resources: 94

#### Resources
Models 13Packs 3Skins 2Tools 2Maps 3Reforged HD Models 69Ported Models 1Reforged HD Skins 1

##
 Chronicles of the Second War


Ok, to simplify this case I'll describe it shorter.

Case 1: You recorded your voice and want to have animated quote.

Case 2: You need to put some existing soundfile and quote in to the character mouth.

Case1 Instruction:

For instance you want Gul'dan to say "Damn you maggots! You will all tremble before me, after I conquer this world!"

1. Record a sound file. GuldanTest01.flac

2. Import this file in your map.

3. Go to sound editor and place sound variable based on imported file.

4. Open trigger editor. Create Trigger with title "Speak" in a folder "Test" with any event you can cause and with action:

Play Dialogue from a speaker.

You need to pick your sound file and unit that speaks (Gul'Dan).

5. Use Casc Viewer or Retera Studio Reforged Hack to Extract faceanim file from campaign

hd.w3mod\_locales\enus.w3mod\sound\dialogue\faceanimation\nightelfx03\facialanimation\orcwarlockguldan.animset_ingame

6. Import this file in your map with simple path like FacialAnimation\orcwarlockguldan.animset_ingame

7. Go to your map Custom Code in trigger editor. Add a function:

 Code:

function LoadGuldanFacialAnimations takes nothing returns nothing
 call SetSoundFacialAnimationLabel(gg_snd_GuldanTest01.flac, "S03Guldan06")
 call SetSoundFacialAnimationGroupLabel(gg_snd_GuldanTest01.flac, "Map-OrcWarlockGuldan")
 call SetSoundFacialAnimationSetFilepath(gg_snd_GuldanTest01.flac, "FacialAnimation/orcwarlockguldan.animset")
 call SetDialogueTextKey( gg_snd_GuldanTest01.flac, "Damn you maggots! You will all tremble before me, after I conquer this world!" )
 call SetDialogueSpeakerNameKey( gg_snd_GuldanTest01.flac, "Gul'dan" )
endfunction

8. Go to initialization Trigger. Add action using custom script:

call LoadGuldanFacialAnimations( )

9. Download example of Conversation.json file from any campaign map.

10. Edit this file for your needs:

Notice that your trigger fiolder and trigger name (where your play dialogue action is started) is written it this file.

 Code:

{
 "stringTablePath": "war3map.wts",
 "conversation": {
 "Test.Speak": [
 {
 "conversationOrder": 0,
 "speakerModel": "units\\orc\\OrcWarlockGuldan\\OrcWarlockGuldan.mdl",
 "soundFile": "GuldanTest01.flac"
 }
 ],
 }
}

11. Import this file to your map.

12. Run your map and launch speaking event.

So basicly Gul'Dan moves lips as he would say the text from S03Guldan06 but with your custom sound file.

Case2 Instruction:

Steps are the same except you don't need to record sound file, you just use existing.

In custom code you write the original sound file name and text. In this case your lipsinc will be perfect.

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

What does the *.facefx_ingame file that is present in (almost) every unit model folder do?

#### InsaneMonster

#### InsaneMonster

Patron

 Level 39

JoinedJul 20, 2011Messages672

Resources: 23

#### Resources
Tools 1Maps 3Reforged HD Models 10Sounds 4JASS 1Lua 1Tutorials 3

##
 Warcraft 3 Re-Reforged


It is a "File used by FaceFX, an application used to create facial animations for video games; stores the stripped down version of the .facefx model that is suitable for use in the target video game; designed to store only the information required by the actor; used as the publishing format for virtual characters."

I think it stores data regarding the actor, while the .animset_ingame stores data about the animation itself (for that character).

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

Somehow I am no more knowledgeable after reading this tutorial. Or maybe it's not about answering the question that I have. I just want a simplistic lip-sync or whatever rig so that a simple custom model in HD can have unit responses when clicked on. Like very simple, to the extent of just having the jaw move to mimic the motion of talking would be sufficient.

#### InsaneMonster

#### InsaneMonster

Patron

 Level 39

JoinedJul 20, 2011Messages672

Resources: 23

#### Resources
Tools 1Maps 3Reforged HD Models 10Sounds 4JASS 1Lua 1Tutorials 3

##
 Warcraft 3 Re-Reforged


dtnmang said:

 Somehow I am no more knowledgeable after reading this tutorial. Or maybe it's not about answering the question that I have. I just want a simplistic lip-sync or whatever rig so that a simple custom model in HD can have unit responses when clicked on. Like very simple, to the extent of just having the jaw move to mimic the motion of talking would be sufficient.


Click to expand...

You should check out how it works in SC2, here it is the same. The game is built to use lip-sync by FaceFX. So you cannot have a simplistic animation. That kind of animation has been removed. You can make your own though, by moving the nodes of the mouth in the portait talk animation, it should work even without lip-sync.

#### Malmgrev

#### Malmgrev

 Level 21

JoinedMar 29, 2020Messages333

Resources: 36

#### Resources
Models 1Packs 1Maps 4Reforged HD Models 18Reforged HD Skins 11Sounds 1

You guys are amazing for solving this, I can't thank you enough. I made an example map from Tamplier's instructions here in case someone encounter problems:

Lipsync Map Tutorial Gul'Dan | HIVE

The dialogue will start automatically 3 seconds in and you will also experience my very tired and bad Gul'Dan voice .

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Great tutorial!

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Love the color usage and overall structure with a few exceptions.

the TABLE OF CONTENTS is way too bloated. Traditionally it only gives an overview by listing the section names but if you want an explanation I would advise a re-structure.

This is not a must, merely an idea. Feel free to come up with your own.

Table of Contents

Requirements​

Goes through the preparation​

Starting​

Goes through the first few steps​

Something else​

Long sentence to get a point across​

​
Lists should also be indented

Wowe

Awesome

Numberzzz

works

too

And yes I am being picky because you're so close to exceptionalism.


 Last edited: Apr 30, 2021

#### InsaneMonster

#### InsaneMonster

Patron

 Level 39

JoinedJul 20, 2011Messages672

Resources: 23

#### Resources
Tools 1Maps 3Reforged HD Models 10Sounds 4JASS 1Lua 1Tutorials 3

##
 Warcraft 3 Re-Reforged


Chaosy said:

 Love the color usage and overall structure with a few exceptions.

the TABLE OF CONTENTS is way too bloated. Traditionally it only gives an overview by listing the section names but if you want an explanation I would advise a re-structure.

This is not a must, merely an idea. Feel free to come up with your own.

Table of Contents

Requirements​

Goes through the preparation​

Starting​

Goes through the first few steps​

Something else​

Long sentence to get a point across​

​
Lists should also be indented

Wowe

Awesome

Numberzzz

works

too

And yes I am being picky because you're so close to exceptionalism.


Click to expand...

Ok, I've fixed all you requested and added some improvements (LUA version, some more expert suggestions, stuff like that...). Let me know!

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Stunning. Approved.

#### Ardenaso

#### Ardenaso

HD Model Reviewer

 Level 35

JoinedJun 22, 2013Messages2,047

Resources: 23

#### Resources
Tools 1Reforged HD Models 19Reforged HD Skins 3

Just realized about this now and this will be one of my last hopes to get my Goblin Civilians speaking

#### Yevhen Snizhko

#### Yevhen Snizhko

 Level 12

JoinedMay 13, 2023Messages157

Resources: 4

#### Resources
Models 1Maps 2Tutorials 1

A note on call SetDialogueTextKey (and maybe on other functions):

If you are using the custom scripts in GUI as a Custom Script action, there is a limit - 260 characters in the line.

#### InsaneMonster

#### InsaneMonster

Patron

 Level 39

JoinedJul 20, 2011Messages672

Resources: 23

#### Resources
Tools 1Maps 3Reforged HD Models 10Sounds 4JASS 1Lua 1Tutorials 3

##
 Warcraft 3 Re-Reforged


Yevhen said:

 A note on call SetDialogueTextKey (and maybe on other functions):

If you are using the custom scripts in GUI as a Custom Script action, there is a limit - 260 characters in the line.


Click to expand...

You usually want to use an item so that it appears in .wts file, in order to be translated.

 You must log in or register to reply here.


### Similar threads

Locked

 Reforged - Lip sync on characters during cinematic


Solcius123

Feb 4, 2020

World Editor Help Zone

Replies22Views8K

Jun 17, 2020
InsaneMonster

 How to: Warcraft 3 Reforged Localized Dialogues


InsaneMonster

Aug 18, 2021

Misc/General Mapping Tutorials

Replies4Views5K

May 7, 2024
OutsiderXE

 How to: Warcraft 3 Reforged Facial Expressions


InsaneMonster

Aug 17, 2021

Misc/General Mapping Tutorials

Replies12Views4K

Feb 9, 2022
InsaneMonster

Locked

 Portrait animation on classic maps for Reforged


Yevhen Snizhko

Aug 9, 2023

World Editor Help Zone

Replies0Views1K

Aug 9, 2023
Yevhen Snizhko

 How to make Cinematics in Warcraft III - Complete Series


Turnro

Apr 8, 2022

Misc/General Mapping Tutorials

Replies0Views5K

Apr 8, 2022
Turnro

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top