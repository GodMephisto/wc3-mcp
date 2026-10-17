---
type: vendor-doc
package: video-archiblad-studios
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=6phxEH9DKBQ
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 358
timestamped: yes
---

# Importing Tilesets for Dummies

Source: <https://www.youtube.com/watch?v=6phxEH9DKBQ>

Uploader: Archiblad Studios | Duration: 5m58s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Here's the link to the "Ripped Tileset Compilation:"
http://www.bwdyeti.com/cafe/forum/index.php?topic=345.0

If you're a more experienced hacker and already have ripped a few tilesets, don't be stingy! Post them up to add to the listings for the rest of the community to benefit from.

## Transcript

**[0:00](https://www.youtube.com/watch?v=6phxEH9DKBQ&t=0s)** hello YouTube viewers it's time for another General archal tutorial and uh today I am going to be making use of this ripped tile set compilation which is available on Fire Emblem Cafe and I will be showing you how to insert these ripped tile sets so hold on let's get started so I'm going to first show off I've already got the map inserted and what we're going to be looking for is The Western Isles tile

**[0:28](https://www.youtube.com/watch?v=6phxEH9DKBQ&t=28s)** set so let's open up this ROM and uh check out this map the the tile set's not in the game so it's let's see skip Lin stuff yeah that's kind of a mess if you can't tell that looks nothing like the map it's supposed to be so um hold on that's not used till later I did a first take of this video but it didn't really work out too well so I have to uh sort of restart everything but uh let's go here and uh so I need The Western Isles

**[1:03](https://www.youtube.com/watch?v=6phxEH9DKBQ&t=63s)** Village tile set so that is conveniently available on the rip tile set compilation so I'm going to download that open that and extract its contents onto my desktop using the wind Roar that I have not paid for okay so here we are here are the tiles here's the configuration and here's the pallette and those are the three things that make up a tile set so in order for us to uh start making these edits we're going to need to open up a

**[1:35](https://www.youtube.com/watch?v=6phxEH9DKBQ&t=95s)** few nightmare modules so we're going to need the chapter data editor and we're going to need the event references module which is right in here and but first of all first off so what we're going to do is we have these DMP files and we're going to need to uh put those somewhere in the ROM all you really have to do is copy paste paste so and you just copy paste it into wherever you have free space so I'm

**[2:05](https://www.youtube.com/watch?v=6phxEH9DKBQ&t=125s)** going to open up my ROM in the hex editor and I'm going to open up the pallet the tile config and the tiles themselves oh that's slightly annoying it only displays three things at once okay so I have some uh free space at uh d D9 I have some free space right there at offset d900 and so let's start with the uh tiles themselves and so what you're going to do is you're just going to take that and uh copy paste and then with hxd

**[2:47](https://www.youtube.com/watch?v=6phxEH9DKBQ&t=167s)** you use contrl B to paste into the ROM because you don't want to expand the ROM so this wres over it so let's see so our tiles went at D9 1 2 3 4 0 Zer and it ends around there so we're going to start excuse me we're going to start uh our pallet at this offset which is so we're going to write that down just make sure we don't lose anything D9 54 do and so let's go grab the pallette and that's kind of small so let's just paste

**[3:25](https://www.youtube.com/watch?v=6phxEH9DKBQ&t=205s)** that in there and then what's left is our config that will be D9 5610 is where we're going to paste that at so let's go grab our config and paste that in there and there we have it so that is all three parts of The Western Isles tile set are now in the ROM and so now I'm going to open up the ROM in Nightmare which is going to be neat on my desktop as tile set tutorial and the prologue map is using ples and Castle

**[3:58](https://www.youtube.com/watch?v=6phxEH9DKBQ&t=238s)** tile set so we're going to replace ples and Castle I wouldn't recommend doing that for your actual hack but we're going to replace that today with Western Isles over uh the Western is Village tile set so here we have the prologue map pointing to where that's written Plains and Castle is right here so let's open this up so this is the tiles so the tiles are at D9 1 2 3 4 then the pallet is right there and the

**[4:26](https://www.youtube.com/watch?v=6phxEH9DKBQ&t=266s)** pallet is at D9 54 do so we're going to write that off offset in there and then the config is at D9 5610 so we're going to write that in there and then let's take our ROM with this broken map and let's refresh it now that we've installed the tile set and there that's a bit weird there are a tile animations where there shouldn't be that's quite strange but yeah there are no tile animations and here we are here is

**[5:07](https://www.youtube.com/watch?v=6phxEH9DKBQ&t=307s)** Western Isles tile set functioning in fe7 and here's B of the Beast and his little buddy it's kind of trapped there by the map but yeah you can uh freely Traverse when you don't have shitty tutorials on but yeah it functions 100% and now you have a tile set in your game and you can do this with any of the tile sets on this uh listing right here and you can paste them in and you can use any tile set in uh your ROMs so I hope you guys

**[5:45](https://www.youtube.com/watch?v=6phxEH9DKBQ&t=345s)** enjoyed this tutorial I hope you learned something about tile set insertion if not uh you just wasted 6 minutes of your life but uh yeah I'm glad you enjoyed and uh Happy ROM hacking guys
