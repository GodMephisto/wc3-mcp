---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/using-ai-to-create-custom-voice-lines-campaign-dialogue.347787/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Using AI to Create Custom Voice Lines Campaign Dialogue
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/using-ai-to-create-custom-voice-lines-campaign-dialogue.347787/>
#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Content

Introduction​

Available AI Tools​

Preparing Training Files​

World of Warcraft Voice Lines​

Getting the Best Output​

Speech-to-Speech​

Editing and Importing​

Final Thoughts​

Introduction

Your browser is not able to play this audio.

AI voice generation is a powerful tool that has the potential to revolutionize the way we create our maps. Are you working on a custom campaign focused around Illidan or Arthas? You can now add their voices to your cinematics, have the player lean back, enjoy the scene, and not have to read text after text. You can, of course, also use AI voice generation to add voices to new characters you create. This can greatly increase the immersion and provide an entirely new level of polish to your maps.

However, this new technology also raises many issues, especially regarding copyright. The legal issues surrounding the use of AI voice generation will likely be hotly debated and figured out in the coming years. While the use of AI to generate custom voice lines for existing Warcraft 3 characters is a harmless application of this technology, it gets more problematic when we're dealing with voices from other franchises or those of real people, where the cloning of their voices could raise serious ethical concerns, and is something I do not encourage.

Finally, this technology is rapidly evolving. Many of these tips may no longer be up-to-date at the time you're reading this.

With that out of the way, let's explore the possibilities of using AI voice generation in custom maps!

Available AI Tools

Available AI voice generation tools include:

elevenlabs.io - Upload sound files to train the AI or choose a voice from a library, then have it read a text you enter or change your voice to a target voice.

uberduck.ai - Works similarly to elevenlabs.

voice.ai - Works by changing your voice instead of writing a prompt. You need at least 5 minutes of training data to start building voices, which will be difficult with only Warcraft 3 voice lines, but users can share generated voices.

Courtesy of @The Nightmare Book:

PlayHT - Free.

FakeYou - Has lots of game character voices.

replicastudios.com - Free.

readloud.net - Provides more-or-less robotic voicelines.

GitHub - neonbjb/tortoise-tts:  - A multi-voice text-to-speech system trained with an emphasis on quality. Requires a good GPU.

Elevenlabs is widely regarded as the most advanced AI voice generation tool on the market right now. All voice samples in this tutorial are created with it. You have a limited amount of text that you can generate per month - more if you pay. If you're looking for just a few voice lines or are fine with a not-so-perfect delivery, the character budget of the free account should suffice. However, if you're trying to get a really naturally sounding delivery, you may have to retry the generation many times, which can quickly deplete your allowance.

Update: Voice cloning on elevenlabs is now behind a paywall, but the first month subscription only costs 1$. You can still use the Voice Design to generate voices for your new characters for free.

As an example, here are a custom "What" and a "Pissed" sound I created for my character "Space Jaina" using elevenlabs:

Your browser is not able to play this audio.

Your browser is not able to play this audio.

I don't have any experience with the other tools, so I cannot give any tips on how to achieve the best results using them. However, the tips included in this tutorial, other than those in the elevenlabs specific section, should apply to all voice generation tools equally.

Preparing Training Files

Skip this section if you don't want to clone existing Warcraft 3 characters. If you want to create new voices for new characters, you might simply try out the presets offered within the AI tools.

You can find a repository with training files for many different voices here:

The Great AI Voice Training Samples Repository

The AI needs a good amount of high quality training data to produce satisfying results. To this end, we want to compile all the voice lines of our character from both the unit command responses and the campaign dialogues. The campaign dialogues are located under Sound\Dialogue and can be directly extracted using the Sound Editor. A faster way, however, is to use Casc View and extract the audio files from the Warcraft 3 archives. To do this, click Open Storage, then the Warcraft 3 parent directory. Once you have opened the archive, navigate to war3.w3mod\_locales\enus.w3mod\sound\dialogue\.

If your character has more voice lines in the campaign than the tool you're using allows (25 for elevenlabs), you should either merge samples or use only the longest and best quality samples. Speculation: Merging audio files makes for worse results, because the AI gets confused if it's not an uninterrupted speech.

Listen through your training set again to make sure there are no other sounds appearing, for example the growling of Tyrande's tiger, which would confuse the AI. You might also want to remove voice lines in which the character talks in a very odd way that only makes sense in that specific situation.

You can try to clone voices not appearing in the campaign from just the unit response sounds, but you should not expect any mind-blowing results. Additionally, the cloning of human-like voices will obviously work much better than that of very distorted voices like demons or undead. You should not even attempt to make custom Lich voice lines. It just won't work.

Here are some samples to give you an idea of the quality you can expect from different voices:

Arthas (good quality):

Your browser is not able to play this audio.

Shandris (low amount of training data):

Your browser is not able to play this audio.

Anub'Arak (very distorted):

Your browser is not able to play this audio.

World of Warcraft Voice Lines

You can also train the AI with sound files from World of Warcraft. Note, however, that many of the voice actors have changed from Warcraft 3 to WoW. The easiest way to find the sound files is to go on wowhead.com, then go to Database -> Sound. Here you can search for sound files for your character.

Notably, the voice actress for Tyrande is Elisa Gabrielli in both Warcraft 3 and WoW. Because there is an ambient sound in all of Tyrande's voice files in Warcraft 3, you will get a much better result by training the AI with WoW sound files instead.

Starcraft 2 Voice Lines

Courtesy of @Daratrix

To extract sound files from SC2 (applies to all files)

Open CASC View (Game storage: Starcraft II)

Click the Tools tab => Search File(s)

In the "File Mask" field, you put your filter/search term. You need to use wildcards (*) around the keywords
 For instance: zagara will fetch you every single files that have "zagara" in the name. zagara.ogg will fetch you all files with zagara in the name that also have the .ogg extension (most voicelines are .ogg it seems)

Click search and wait for it to finish.

Despair and try to fix your search terms for 10 minutes.

Highlight all the files that you want to extract in the results (you can Ctrl+A to select everything, Shift+Click to select a range, or Ctrl+Click to select specific files).

Right click > Extract (F5).

I recommend ticking "Extract plain names, ignore storage directory structure" in the extraction options.

Heroes of the Storm Voice Lines

You can find all voice lines for heroes in Heroes of the Storm on the Heroes of the Storm Wiki. Simply go to the article of the hero of your choice and find the voice lines under Quotes. Download the audio files by right-clicking on the play-icon and then "Save Audio As..."

Getting the Best Output (elevenlabs)

This section is specific to the elevenlabs tool. It is based solely on my personal experience and involves a lot of speculation.

In general, the AI is much better at generating convincing speech for longer texts than for shorter texts. However, if you're entering texts that are too long, a single "hiccup" can spoil the entire output, so a good middle ground should be aimed for when you're trying to synthesize campaign dialogues.

As an example for this tutorial, I generated a speech for Jaina that could appear in a campaign cutscene. This was my input:

"I have deployed all the troops from Theramore I can spare for our assault on the gates of Ahn'Qiraj. I am confident that the tauren will join our expedition. However, I'm not so sure about the orcs."

Your browser is not able to play this audio.

The delivery is a bit sleepy, but still quite good, considering this was my very first attempt. The added sigh at the end is also a nice touch. Now let's try to generate the last sentence by itself:

Your browser is not able to play this audio.

In this example, the AI lacks the context and cannot infer the right speed and sentence melody that is appropriate for this sentence. The delivery is far too fast, which is often the case for short snippets. You will not get a satisfying result, even if you try countless times. However, we have a bit of control over the delivery by using punctuation. Here is the same line, but this time with the prompt: "However... I'm not so sure about the orcs."

Your browser is not able to play this audio.

The same line spoken by Tyrande:

Your browser is not able to play this audio.

Ways to change the delivery:

"...", "⸺", or line breaks to add a pause.

Exclamation marks make the delivery more forceful.

"?!?" or similar combos can be used for good effect, but are not always reliable.

Putting words in ALL CAPS seems to nudge the AI into putting the emphasis on a specific word, but it is not reliable.

You can also put the sentence into a longer text to give the AI hints about the context of the text it's supposed to voice, then cut the surrounding text from the voice file using an audio editing software (but the surrounding text should be delimited by pauses). You can also add words like "However" or expressions like "Oh my god" to the beginning of sentences.

Here is a recreation of "O07Jaina46" from the Orc campaign using the prompt:

"Wait!... this is INSANE!... You can't possibly expect me to ally with THEM?!?"(cutting the audio after "to", I should have put a pause there)

Your browser is not able to play this audio.

Your browser is not able to play this audio.

There is now also an elevenlabs discord server, where you can find additional tips and tricks about getting the best output.

Speech-to-Speech (elevenlabs)

Elevenlabs now also offers a speech-to-speech feature, where, instead of using a text prompt, you provide an audio file and the AI changes the voice into the target voice, while keeping the rhythm and melody exactly the same. The audio file can be one you record yourself or an already existing audio file.

This feature allows you to exert much more control over the dialogue's delivery. However, using your own recordings might not produce good results for everyone, because there are several caveats. The AI will pick up your accent as well as any audio problems with the recording, such as background noise, pops and clicks, muffled speech and so on. Unless you have a good recording setup and are able to speak accent-free, the result of the voice transfer will probably sound off.

There are, however, different ways to make use of the speech-to-speech feature, by using already existing audio files or by feeding the audio files created with elevenlabs back into the AI. The utility of the first approach is easy to see. Maybe you want to make a morphed demon hunter voice for a different character or you're working on an alternate reality campaign in which Jaina is a dreadlord?

Your browser is not able to play this audio.

(Tichondrius voice with bass enhanced and slight reverb added in Audacity)

Feeding an AI generated audio file back into the AI is a useful method when you want to make a character speak in a certain way that's not accessible with the default text-to-speech method. For example, it will be very hard to make Tyrande speak in a casual way, because there is always a lot of pathos in her deliveries. However, by generating the voice lines for a different character first, you can then use those audio files to change the delivery for the Tyrande voice.

Here is Uther speaking with the melody of GLaDOS:

Your browser is not able to play this audio.

Your browser is not able to play this audio.

Make sure to slide the "Exaggerate Style" option to full when using this method.

Editing and Importing

Now it's time to import the voice files into your map. If you're working on custom unit response sounds, you can replace any of the existing soundsets. Find one that has the right number of What, Yes, YesAttack, and Pissed sounds. For example, I wanted a sixth Pissed sound for Space Jaina, so I replaced the Knight sounds instead of those from the original Jaina.

To get the correct import paths, go into the Sound Editor (F5) and search for the name of the unit you want to replace. The import path for Knight for example is:

"Units\Human\Knight\SoundFileName". The "Sounds" and "Internal" folders are not part of the import path. Then, go to the Asset Manager (F12), import the sound files, then change the subfolder from "war3mapImported" to the correct path.

Listen to the voice lines next to each other and make sure the audio levels are consistent. You might have to increase the volume on most of them. You might also find that a voice line that sounded great when you listened to it by itself sounds unnatural when put into the ensemble. You might have to then recreate it.

As convincing as these AI generated voices can be, if you put them next to those spoken by the original voice actor, you will notice a difference. Therefore, if you're working on custom unit responses, I recommend recreating them completely, and not using a mix of old and new sounds.

Final Thoughts

I hope this tutorial has been of interest and I hope you have success with bringing our beloved Warcraft characters to life in your maps. Thank you to @Wareditor for helping me get started with the AI tool.


 Last edited: Feb 29, 2024

#### The Nightmare Book

#### The Nightmare Book

 Level 36

JoinedJun 11, 2017Messages1,277

Resources: 7

#### Resources
Maps 7

Cool stuff! It's fascinating to see that AI became useful in case of map developing.

Along with custom Stable Diffusion or Mildjourney models to create your ingame paintings/icons/etc, now we can creature voicelines for our dialogs. Cannot wait start see campaigns will use them more and more frequently.

What a time to be alive.

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

The Nightmare Book said:

 What a time to be alive.


Click to expand...

I don't know about that, but that we get to have a bit of fun with the AI before it destroys us all is a nice consolation .

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

I've created a pastebin, where I upload the training sets for the voices that I made so far:

AI voice training samples

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Will give it a proper read later today.. if I remember, but just going by title and a short glance I think this is way more useful than the community gives it credit for.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Quick thoughts:

In terms of showcase, might be useful to show examples of other voices. Orc voice perhaps. OR if I want to see the difference between say Tyrande and Jaina.

I would also say that uploading the files used for teaching the examples is very nice for those wanting to give it a quick try.

Superb in general.

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Chaosy said:

 Quick thoughts:

In terms of showcase, might be useful to show examples of other voices. Orc voice perhaps. OR if I want to see the difference between say Tyrande and Jaina.

I would also say that uploading the files used for teaching the examples is very nice for those wanting to give it a quick try.

Superb in general.


Click to expand...

Thank you very much! I linked my pastebin with the different voice training sets in the original post and I updated the training data section to include examples of the range of quality in the voices: Arthas (good), Shandris (ok), Anub'Arak (poor). Also, I had Tyrande recite the same line about Ahn'Qiraj.


 Last edited: Mar 14, 2023

#### Kaizer123

#### Kaizer123

 Level 12

JoinedFeb 15, 2016Messages187

Resources: 4

#### Resources
Reforged HD Models 3Ported Skins 1

Antares A said:

 Thank you very much! I linked my pastebin with the different voice training sets in the original post and I updated the training data section to include examples of the range of quality in the voices: Arthas (good), Shandris (ok), Anub'Arak (poor). Also, I had Tyrande recite the same line about Ahn'Qiraj.

Click to expand...

Could you tell me in which AI you create your versions of lines with the voices of the characters?

I tried the one that you can download, but for some reason it didn’t open and showed me some kind of error that I don’t remember why, but it just didn’t open, the program panel itself opened in its design, but later it didn’t load and showed only an inscription with an error, and server time and my own. Your first site, as you yourself mentioned, is paid, and I opened the second one and did not understand how to use it. I rip Alexstrasza's voice from HOTS to make my own versions of her lines. But I couldn't do it. Instead, on the site Uberduck | Text-to-speech, voice automation, synthetic media I could only choose some ready-made human voice, and add my Alexstrasza file only as an add-on, instead of being the basis

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

All the voice lines above are created with elevenlabs. The results I got from the other tools were absolutely terrible in comparison. Someone also mentioned Vasynth to me, but I haven't tried it out yet.

#### raypack

#### raypack

 Level 31

JoinedDec 3, 2020Messages1,261

Resources: 10

#### Resources
Models 1Maps 8Tutorials 1

Wow. God bless you

#### ZethHolyblade

#### ZethHolyblade

 Level 11

JoinedMay 16, 2013Messages193

Resources: 9

#### Resources
Icons 3Skins 6

All I have to say, many many thanks to you (and @deepstrasz).... hopefully, we may hear this in a map one day:

 https://www.hiveworkshop.com/data/audio/399/399098-0abf5015f26e0a3e2a3907af8b238ed1.mp3



 Last edited: Jun 13, 2023

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Starcraft/Warcraft crossover?

#### Footman16

#### Footman16

Community Manager

 Level 58

JoinedJul 14, 2012Messages4,279

Resources: 37

#### Resources
Models 6Packs 1Skins 11Maps 12Reforged HD Models 3Reforged HD Skins 2Reforged HD Icons 2

I think a more in depth showing of using the sound editor to replace the soundsets and/or importing the sounds and using them for cinematic transmissions might be useful for folks.

Also elevenlabs will not recreate the audio effects on a unit such as the reverb/echo effect on some warcraft characters voices and another program like Audacity (I mention audacity because it's free and what I use) will need to be used to recreate that.

#### ZethHolyblade

#### ZethHolyblade

 Level 11

JoinedMay 16, 2013Messages193

Resources: 9

#### Resources
Icons 3Skins 6

Antares A said:

 Starcraft/Warcraft crossover?

Click to expand...

Mayhaps

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

I'm very excited by the modding prospects of AI Voice Generation, and glad you are jumping on this nascent niche to try & organize the chaos with such a well-thought-out & detailed tutorial. I have been looking forward to getting into this myself but am not quite ready for it; when I am, I'm sure this tutorial will be invaluable. : )

@Wa666r has done some extensive work in this direction as well, with his Nerubian faction & campaign; maybe he could supplement some of the advice with his own experiences?

#### raypack

#### raypack

 Level 31

JoinedDec 3, 2020Messages1,261

Resources: 10

#### Resources
Models 1Maps 8Tutorials 1

#### TheDarkGiganotosaur

#### TheDarkGiganotosaur

 Level 15

JoinedSep 26, 2007Messages370

Hello, @Antares A! I've been meaning to say this much, much earlier, and now I guess it's time and I'd like ask you something.

First of all, I'm really impressed with what the AI voice generation tools can do (especially after viewing/listening the videos/sounds in Hive) and am interested in creating my own AI-voice-generated dialogues/lines for my custom campaign (which is waaaaay far from being finished due to real-life issues getting in the way) and to also create my own and/or improve VoicePacks for UT99 (Unreal Tournament). However, there's just one problem... I have no means to access/use the said tools because of many reasons, therefore I'll list them--but it may sound a little bit like a harmless rant. Sorry...!

In my opinion, all applications should be user-friendly (and fully downloadable if it's Freeware). They should NEVER have compulsive launch requirements such as Internet connection, account creations, et cetera. Also, they should NEVER have compulsive dependency requirements such as .NET Framework, Java, et cetera. And finally, they should ALWAYS be portable (no installation needed, just extract them to any folder and run the executable file) and stealth (no traces left in both user's AppData/Application Data folder and in Windows registry), and FULLY compatible with older Windows OS'es (no way I'm using Spyware Windows 10 and beyond) and WINE (for Linux/Mac users).

Nevertheless, I understand that there's a risk of these tools being misused so putting up these intentional "roadblocks" seem to be the only way to prevent people from causing trouble/mischief worldwide, so I guess I'll have to live with that.

Anyway... and after getting that off my chest, here's my question. May I request you and/or any Hive member (who has full access to the AI voice generation tools, that is) to create needed AI-voice-generated dialogues/lines and share them (as MP3 and/or WAV files) with me? I, of course, would gladly credit to whoever does this for me, so I can add proper cinematic/ingame dialogue speeches into my custom campaign and then release it for the public (once the progress of my custom campaign reaches its completion) and also create stunning (with unique lines) VoicePacks for UT99.

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Hi there,

I don't have an active elevenlabs subscription right now, but I can definitely create some lines for you the next time I subscribe to create some for myself. However, doing an entire campaign is a massive undertaking and a bit too much of an ask. I don't think there's a way around you trying to fix the issues you have with accessing their tool or you team up with someone else who also wants to get into AI voice generation. The first month is only 1$, the only issue being that you need a credit card. This is often the case with American companies. I don't know if they don't realize that, outside of the US, the people who own a credit card are in the minority or they just don't care.

While what you're describing sounds good on paper, the reality is that if it were a downloadable software, it would probably be unbearaby slow on many people's pcs, severely restricting their customer base. I'm getting the impression that your pc is also not the most cutting edge.

Big fan of the early Unreal games (not a surprise looking at the music in my map)! Interested to hear what the VoicePacks will sound like!

#### TheDarkGiganotosaur

#### TheDarkGiganotosaur

 Level 15

JoinedSep 26, 2007Messages370

Antares A said:

 I don't have an active elevenlabs subscription right now, but I can definitely create some lines for you the next time I subscribe to create some for myself. However, doing an entire campaign is a massive undertaking and a bit too much of an ask. I don't think there's a way around you trying to fix the issues you have with accessing their tool or you team up with someone else who also wants to get into AI voice generation.


Click to expand...

Thanks for replying, and thank you so much for your kindness, though I regret to inform that I have nothing to request at the moment. As for you not having an active subscription, it's not a problem if I have to wait a very long time... patience is a virtue after all.

Besides, like I said, my Campaign is far from complete, and I have no idea how long it'll take for me to finish... I need to revamp the whole thing since I've given up on the project 10 years ago, but now I decided to return since I was inspired with many new, interesting, and promising models being uploaded to Hive. As for the UT's VoicePacks, check the last quote for more details.

Hopefully, when I feel ready to request, I'll contact you via "Start conversation" thingy.

EDIT: Whoops! I forgot to leave a "Like" in your post. Sorry about that.

Antares A said:

 The first month is only 1$, the only issue being that you need a credit card. This is often the case with American companies. I don't know if they don't realize that, outside of the US, the people who own a credit card are in the minority or they just don't care.


Click to expand...

Sadly, I have no means (yet) to perform online payments, let alone earn. Need a PayPal and/or similar account, though.

Antares A said:

 While what you're describing sounds good on paper, the reality is that if it were a downloadable software, it would probably be unbearaby slow on many people's pcs, severely restricting their customer base. I'm getting the impression that your pc is also not the most cutting edge.


Click to expand...

Actually, my current PC (a gaming laptop) is more than meets the eye...! This "baby" is a Lenovo Legion with these specs (except the 1 TB HDD part, since mine doesn't have it).

Anyway... and without going too off-topic, I only stated what I said earlier because few years ago I used to have an old laptop which had Windows XP installed in it (and I often had to use USB stick to boot Knoppix, a neat Linux distro with a plethora of applications). And now, like I said in one of my profile posts, I purchased a new gaming laptop which is far more powerful and faster. Additionaly, I have an external 1 TB SSD so I'm not worried about the space. The laptop already comes with Windows 10 installed, but there's absolutely no way I'm gonna use it, so I'm using Linux Mint (installed in the said external SSD) instead... and along with it I have Windows 7 installed inside VirtualBox where Warcraft 3 (with v1.26a patch installed) resides.

As for the reason of my harmless rant... well, the only problem that I have with people is when they treat others (who aren't rich and are stuck with old PCs, OS'es, bitnesses, et cetera of their choice) with contempt. As for application developers, I wish they'd stop, cease, and desist this crazed mentality of "killing off" support of what they don't approve of or consider to be beneath them--obsolescence of OSes and bitnesses. Applications SHOULD be made/available for/to EVERYONE, NOT just for/to some... and the people have all the right to use whatever application and whatever PC (regardless of the OS and bitness) they want and should never be oppressed.

Here's a post by LoneCrusader (a MSFN Moderator), and his following and specific words were pretty epic:

 This attitude (prevalent most everywhere I look online these days) that it's OK to force, browbeat, insult the intelligence of, shame, chide, or in any other manner "push" users of any product to give it up for something newer just because someone else says it's better is a cancer, pure and simple. I'll bet that most people pushing this drivel would be deeply offended if say, someone told them their political views were wrong and that they should change because someone else says so. Too many people today are willing to sing the praises of freedom until that freedom applies to something they don't like.

Yes, Microsoft has a right to change their products and/or change their business philosophy or whatever. But as far as I am concerned they have a moral obligation to NOT WILLFULLY make it more difficult for users of previous products to keep those products operating. Said users paid for those products, and the fact that some time has passed does not make those users somehow obligated to give up something that works perfectly well and buy something else because Microsoft says so. Don't want to support it any longer? Fine. But don't start putting roadblocks out for those older systems intentionally.

This is the main root of the problem that many of us have with Microsoft and many other software/tech companies. It's not what they choose to do, but how they go about doing it. So they want to release a new operating system? Fine. Release it, and if it's really better than what users already have then people will begin to move to it of their own accord. Where the problem arises is 1)when they start pushing FUD garbage about how suddenly you will become vulnerable to this or that catastrophe simply because you haven't chosen to move on; 2)when they use their "influence" to push other companies and products to no longer support an older system; and 3)when they start "manipulating history" to remove information or files relevant to older products, thereby making it more difficult to keep the older systems running (which once again were paid for, and the fact that time has passed does not invalidate this). Very few things make me angrier than when I follow a link to Microsoft's site that is supposed to lead me to some documentation or update for an older system and instead I get some garbage like "We're sorry. This page/update is no longer available. Update to Windows 10 today! It's up to date, fast and secure!" My response? Sure. When .... freezes over.


Click to expand...

Antares A said:

 Big fan of the early Unreal games (not a surprise looking at the music in my map)! Interested to hear what the VoicePacks will sound like!


Click to expand...

I'm more or less good with UnrealScript, but it's a very daunting thing to learn, but I already created four neat mutators--but they're currently unreleased and not bug-free. Creating a VoicePack, however, is pretty easy... all it needs are WAV files (with acknowledgements, orders, taunts, and others lines) and a little bit of "UnrealScripting" and presto!

I like using many different kinds of voices for computer-controlled players (A.K.A. Bots), both original and non-original (like VoicePacks from UT2k4, UC2, UT3 or other different games converted to UT). And the use of AI voice generation tools may be the ultimate solution to fill the missing lines or even add new ones! Speaking of which, I need some/a long time to think about which VoicePack I'd like to work on. Also, I collect different voices for UT but it's hard because most important lines like "The enemy flag carrier is here." or "Control point is secure." or "I'm under heavy attack!", et cetera are missing... and mixing and splicing the voices with same tones and forms is very difficult, hence the reason for requiring the usage of the AI voice generation tools.


 Last edited: Jul 21, 2023

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Yea, it sounds like your pc would be powerful enough to run an AI program. I'm sure in the future there will be competitors of elevenlabs that are on par with its quality and use a downloadable software. It may already exist. I haven't kept up with it that much and things are moving fast right now.

Regarding the killing off support, yea that sucks, but that's what's to be expected if the company can't really make any more money with it. Although, intentionally hampering use of an old software so that people have to buy the newer one is really scummy. I wish there would be laws that if a company abandons support for a program, it becomes open source. But I'm sure there are problems with such a proposal, and it's kinda off-topic here to discuss that.

Mixing AI generated voice lines with the original ones from Unreal Tournament may not work that well. You will probably still hear a significant difference between the original and the AI generated voice lines. I suspect that you would want to replace them entirely if you go that route.


 Last edited: Jul 22, 2023

#### TheDarkGiganotosaur

#### TheDarkGiganotosaur

 Level 15

JoinedSep 26, 2007Messages370

Antares A said:

 Yea, it sounds like your pc would be powerful enough to run an AI program. I'm sure in the future there will be competitors of elevenlabs that are on par with its quality and use a downloadable software. It may already exist. I haven't kept up with it that much and things are moving fast right now.


Click to expand...

Well, the way things have been evolving nowadays and depending how powerful (and seemingly dangerous) the AI tools may be, I don't think they'd give them away like candy, per se... but I digress.

Antares A said:

 Regarding the killing off support, yea that sucks, but that's what's to be expected if the company can't really make any more money with it. Although, intentionally hampering use of an old software so that people have to buy the newer one is really scummy. I wish there would be laws that if a company abandons support for a program, it becomes open source. But I'm sure there are problems with such a proposal, and it's kinda off-topic here to discuss that.


Click to expand...

You're right, I shouldn't derail the thread with "off-topicness"... man, Hive really needs an "off-topic" tag! Still, it's a pretty sad reality we're all living in...

Antares A said:

 Mixing AI generated voice lines with the original ones from Unreal Tournament may not work that well. You will probably still hear a significant difference between the original and the AI generated voice lines. I suspect that you would want to replace them entirely if you go that route.


Click to expand...

Umm... no, I meant ripping voices from other games first and then use them to create AI-generated voice lines for... well, Warcraft 3, Unreal Tournament, et cetera. And there's no better place to download ripped voices from several games than this site!

Long ago, while I was still creating my Campaign, I used Handmaiden's voice (from Star Wars KOTOR 2) as a soundset for a female Impaler Hero, and the voice had pretty useful lines... of course, it wasn't easy figuring out what to put in the "Pissed" lines so I browsed through her lines and found that her "Battle is a pure means of expression blah blah" words would fit perfectly. Unfortunately, the only thing lacking was cinematic dialogues and there's no way I could ever imitate her voice with my slightly limited vocal cords, but that won't be a problem anymore... not now that we have these neat AI tools!

Anyway, from my perspective (and I could be wrong), I think that an AI tool needs to "learn" the voice of a character of our choice with a series of dialogue sound files (containing that type of voice) obviously extracted from any game... like those from Jaina. Which is why I was seriously impressed when you created your own lines using Jaina's real voice... how envious I feel! I even enjoyed hearing Uther and Garithos exchanging words from the "Azeroth Directorate" video above, which was pretty cool, by the way. Again, how envious I feel!

Unreal Tournament series has several voice lines an AI tool can learn and hopefully reproduce them with our own lines, so I don't think it's difficult... unless it has some echo, radio effects, or whatever in it, but it's of little consequence. Because what truly matters is that it really works, right?

#### emperor_d3st

#### emperor_d3st

 Level 18

JoinedApr 13, 2008Messages1,629

Resources: 2

#### Resources
Ported Models 2

Antares A said:

 I don't know about that, but that we get to have a bit of fun with the AI before it destroys us all is a nice consolation .


Click to expand...

People might think you are joking, but it's clear that is exactly what is going to happen, lol.

Thank you for the tutorial!

#### Barade

#### Barade

 Level 29

JoinedFeb 2, 2006Messages1,692

Resources: 5

#### Resources
Tools 1Maps 2Spells and Systems 2

Instant voice cloning for ElvenLabs costs money, right?

What about GitHub - jaywalnut310/vits: VITS: Conditional Variational Autoencoder with Adversarial Learning for End-to-End Text-to-Speech which is used for Gothic mods: GitHub - lexkoro/GameTTS

It seems to be free without any charges.

However, I have no idea how it works/how the models are generated.

I need this for announcer voices for all 4 standard races and my custom races but I do not want to subscribe the tool for money even if it is not that expensive.

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

I haven't tried out other AI voice generators since writing the tutorial, but back then their quality was abysmal and elevenlabs was just miles ahead. They might have gotten better since, but, if you're looking for freeware, the saying "You get what you pay for" probably still applies.

If you want to try out these tools, I'd be interested in your results. I can also add them as alternatives to the tutorial if you deem them viable.

I'll be sticking with elevenlabs though. They just added speech-to-speech feature and I'm excited to try that out and add it to the tutorial.

#### Pandaprewmaster325

#### Pandaprewmaster325

 Level 14

JoinedMay 19, 2022Messages300

Resources: 1

#### Resources
Tools 1

oh man wanted the orc warlock to say "the legion has sent the dead to best us! victory to the blackc*ock clan"

and the orc blademaster to say "we orcs are the true d*ck suckers of the burning legion these mindless undead are virgins! impure!" too bad cant do that now guess this joke will never be true thanks to their paywall

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

I added a section on the elevenlabs speech-to-speech feature. Also, I think there was a tutorial on how to create custom voice lines without overriding existing ones somewhere on hive, but I can't seem to find it. If anyone can link it here, I will include it in the final section of the tutorial.

#### deepstrasz

#### deepstrasz

Map Reviewer

 Level 84

JoinedJun 4, 2009Messages22,555

Resources: 7

#### Resources
Maps 4Spells and Systems 2Sounds 1

Antares A said:

 I added a section on the elevenlabs speech-to-speech feature. Also, I think there was a tutorial on how to create custom voice lines without overriding existing ones somewhere on hive, but I can't seem to find it. If anyone can link it here, I will include it in the final section of the tutorial.


Click to expand...

Is this it?

###
 Baradé's Unit Sound Sets 1.0


Motivation This system helps you using custom unit sound sets without replacing existing ones. Usually, you would import the sound set files and use paths of an existing sound set, making the existing one unavailable for your map. This system allows you registering unit sound sets with different...

 www.hiveworkshop.com


#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Oh, that might be it. I thought it was possible just with some world editor trick or something like that.

#### Mint Blancmanche

#### Mint Blancmanche

 Level 16

JoinedMar 17, 2019Messages424

Resources: 8

#### Resources
Reforged HD Icons 2User Interface 2Sounds 4

I wanted to make a female announcer for the "build more farm/ally town is under siege" stuff but when I went to save/download it asked for paid account.

anyone knows of some free ai shit?

#### The Nightmare Book

#### The Nightmare Book

 Level 36

JoinedJun 11, 2017Messages1,277

Resources: 7

#### Resources
Maps 7

Mint Blancmanche said:

 I wanted to make a female announcer for the "build more farm/ally town is under siege" stuff but when I went to save/download it asked for paid account.

anyone knows of some free ai shit?


Click to expand...

The Nightmare Book said:

 Actually, there are or were free options. Check them here:

PlayHT - for professional usage, dunno it is free for now;

FakeYou, Your Deep Fake Text to Speech Website. - a lot of game character voices, you can check them;

Home - also provides free (?);

Free service - text to speech and save to mp3! and so-on reader websites, now provide more-or-less robotic voicelines.

GitHub - neonbjb/tortoise-tts: A multi-voice TTS system trained with an emphasis on quality - use this repo to install tts-model, which is nice in quality, but requires a good GPU.

In terms of quality, ElvenLabs is are most proficient one. But I don't recommend that you use multiple accounts for their services - you can get an IP Ban for 100 years, and you'll be unbanned if you buy a subscription.


Click to expand...

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

I haven't looked into what happened on the market since writing this tutorial. I can add those as alternatives to the OP.

#### The Nightmare Book

#### The Nightmare Book

 Level 36

JoinedJun 11, 2017Messages1,277

Resources: 7

#### Resources
Maps 7

Antares A said:

 I haven't looked into what happened on the market since writing this tutorial. I can add those as alternatives to the OP.


Click to expand...

For music-specific tasks, you can also include Suno.AI which is a discord-based community, where you can freely create simple music tracks with any music genre you want.

###
 Join the Suno Discord Server!


Make any song you can imagine | 279341 members

 discord.gg


#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

That's an interesting tool as well, but not really the topic of this tutorial.

I've added the sites you listed. I just edited your descriptions slightly. See if they're good?

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Mint Blancmanche said:

 I wanted to make a female announcer for the "build more farm/ally town is under siege" stuff but when I went to save/download it asked for paid account.

anyone knows of some free ai shit?


Click to expand...

I have a subscription right now. PM me the sound files and tell me which voice you want it changed to and I'll do it real quick.

#### Mint Blancmanche

#### Mint Blancmanche

 Level 16

JoinedMar 17, 2019Messages424

Resources: 8

#### Resources
Reforged HD Icons 2User Interface 2Sounds 4

forgot about this subject lol.

found a "free" text to speach, made my female announcer and replaced (human/generic guy replay) a couple of days ago.

not gonna post it cuz I don't want people to look at me funny lol.

on another note, replaced the horrible "ArtilleryCorpseExplodeDeath1" for the sound of another game that has proper gory body explosions.

little sample of my announcer or whatever the warning voices are called xD

#### Attachments

KnightUnitAttack1.mp3

 57.2 KB



PeasantCannotBuildThere1.mp3

 44.5 KB



GenericWarningHeroFallen1.mp3

 49.7 KB



GenericWarningUnitAttack1.mp3

 65.6 KB



KnightResearchComplete1.mp3

 43.6 KB



KnightUpgradeComplete1.mp3

 45 KB



PeasantBuildingComplete1.mp3

 37.5 KB




 Last edited: Jul 2, 2024

#### Cheshire

#### Cheshire

 Level 26

JoinedMar 29, 2020Messages1,482

Resources: 4

#### Resources
Icons 3Maps 1

this tutorial is great, thanks!

#### ZethHolyblade

#### ZethHolyblade

 Level 11

JoinedMay 16, 2013Messages193

Resources: 9

#### Resources
Icons 3Skins 6

Has anybody tried cloning the Gyrocopter / Flying machine's voice in elevenlabs or other AI sources? I noticed they added a new feature that removes background noises... but it's been a couple of hours and I can't get the voice i want.. Sounds like a completely regular / rejuvenated dude.

I want it for my project, so that he can say some funny unit quotes

#### Attachments

Alliance Gyrocraft - Base Cleaned.rar

 5.8 MB

 · Views: 134



#### Cheshire

#### Cheshire

 Level 26

JoinedMar 29, 2020Messages1,482

Resources: 4

#### Resources
Icons 3Maps 1

ZethHolyblade said:

 Has anybody tried cloning the Gyrocopter / Flying machine's voice in elevenlabs or other AI sources? I noticed they added a new feature that removes background noises... but it's been a couple of hours and I can't get the voice i want.. Sounds like a completely regular / rejuvenated dude.

I want it for my project, so that he can say some funny unit quotes

Click to expand...

this isn't exactly the same voice, but Dota 2's gyrocopter is pretty similar, the voicelines are completely clean and there are a lot of them.

###
 Gyrocopter/Responses


▶️ Gyrocopter! ▶️ Let's go for a spin! ▶️ Got 'er oiled up and loaded! ▶️ I'll start the checklist. ▶️ Spin 'em up! ▶️ Get yer rotors running! ▶️ All systems nominal. ▶️ C'mon you whippersnapper! ▶️ Expect some turbulence. ▶️ Locked and loaded! ▶️ Gonna break your beak! ▶️ Flying the friendly...

 dota2.fandom.com


#### ZethHolyblade

#### ZethHolyblade

 Level 11

JoinedMay 16, 2013Messages193

Resources: 9

#### Resources
Icons 3Skins 6

Cheshire said:

 this isn't exactly the same voice, but Dota 2's gyrocopter is pretty similar, the voicelines are completely clean and there are a lot of them.

###
 Gyrocopter/Responses


▶️ Gyrocopter! ▶️ Let's go for a spin! ▶️ Got 'er oiled up and loaded! ▶️ I'll start the checklist. ▶️ Spin 'em up! ▶️ Get yer rotors running! ▶️ All systems nominal. ▶️ C'mon you whippersnapper! ▶️ Expect some turbulence. ▶️ Locked and loaded! ▶️ Gonna break your beak! ▶️ Flying the friendly...

 dota2.fandom.com


Click to expand...

Thank you, although I was able to get the voice closer to the original after many tries. The problem is now getting the prompts right, for the AI to pronounce the lines as intended and, more importantly, without screaming.

I'm trying to make the AI voice speak like the starcraft 1 wraith, and say these lines:

#### Attachments

ElevenLabs_2024-08-13T20_58_49_New Gyro_ivc_s55_sb85_se15_b_m2.mp3

 53.5 KB



ElevenLabs_2024-08-13T20_57_50_New Gyro_ivc_s55_sb85_se15_b_m2.mp3

 86.1 KB



ElevenLabs_2024-08-13T20_57_19_New Gyro_ivc_s55_sb85_se15_b_m2.mp3

 79.6 KB



#### Retria

#### Retria

 Level 9

JoinedMar 2, 2014Messages170

I started playing with it, still have 9,567 credits remaining but this apperead. What can I imagine under temporarily?

#### The Nightmare Book

#### The Nightmare Book

 Level 36

JoinedJun 11, 2017Messages1,277

Resources: 7

#### Resources
Maps 7

Retria said:

 I started playing with it, still have 9,567 credits remaining but this apperead. What can I imagine under temporarily?

View attachment 540428

Click to expand...

Did you try swapping accounts for free usage? Congrats - you've been banned.

The proper way to get rid of the ban is to pay $1 they want and get first plan, then cancel subscription if you don't want to continue.

#### Retria

#### Retria

 Level 9

JoinedMar 2, 2014Messages170

That was my first account and I never used vpn and I have cable internet so nobody should be on my IP.

My conspiracy brain tell me they need extra revenue now so they lure people for free 10k points and ban them after 500 or something.

Oh well basic plan is not expensive.

#### KhalJyanoz

#### KhalJyanoz

 Level 7

JoinedSep 15, 2020Messages88

Resources: 1

#### Resources
Models 1

This will all be interesting to keep in mind; especially since the mega campaign I have in mind with like 20+ factions is going to be more Age of Empires/Mythology coded. Even if one's not intending on a fully voiced campaign, it's still doable to override a lot of the UI voices (More Gold is Required / Wurk Kompleet / Our Hero has been killed! / Our sacred grove is being desecrated! / We Need More Coral Beds) to keep things fresh.

 You must log in or register to reply here.


### Similar threads

 How to Voice Act


Kwaliti

Oct 30, 2007

Misc/General Mapping Tutorials

Replies9Views11K

Dec 19, 2007
nathanman

Locked

[Solved]
 Voice files extracted from the game for Voice Cloning (AI-generated new dialogues)


Vel (Tien-Yun) Wu

Mar 1, 2021

Requests

Replies5Views4K

Mar 20, 2021
Vel (Tien-Yun) Wu

Locked

 My AI jainarated voice + offering to create voice-overs for your dialogues


Antares

Mar 1, 2023

Map Development

Replies24Views4K

May 18, 2023
Antares

Locked

 The Great AI Voice Training Samples Repository


Antares

Apr 17, 2023

Sound / Music Art

Replies9Views7K

Jun 23, 2023
ZethHolyblade

Locked

 Does using AI-generated voice for cinematics ruin the game?


TheIronRaven 2.0

Feb 4, 2021

Sound / Music Art

Replies8Views2K

Apr 27, 2021
pyf

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