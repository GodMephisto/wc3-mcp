---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-import-an-icon.7287/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Import an Icon
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-import-an-icon.7287/>
#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

How to Import an Icon

By Archian

Table of Content:

Introduction

Downloadable Icons

Icon Types

Import the Icon

Introduction

This tutorial will explain how to import an icon into the World Editor, it will also include links to downloadable icons and other tutorials relating to icons. Click the images to enlarge them.

Icons are small images used in warcraft's game interface.

They're used for items, abilities and units.

Notice, that all icons in Warcraft 3 are 64 x 64 or 32 x 32 pixel 24-bit pictures in .TGA or .BLP format.

Downloadable Icons

In order to import an icon, you must have an icon to import.

You can download icons from sites such as:

Wc3campaigns.net

Warcraft 3 Campaigns is a vast web forum community; and the home of top-quality resources.

Wc3sear.ch

Wc3sear.ch have maps, models, skins, icons, discussion forums and mapping tutorials.

However, you can make them yourself aswell.

There are many tutorials which can help you, such as Tiki's How to make an icon tutorial.

If you make your own icons, you might also want to have a tool to convert the icon files into .BLP format, such as Warcraft 3 Viewer.

Icon Types

There are four icon types in Warcraft 3.

From left to right: Normal, Passive, Autocast and Disabled.

Icon originally created by Daelin. Modified for tutorial by Archian.

Normal

The custom path for "normal" icons can be basically anything you wish it to be.

For instance:whatever\BTNmyIcon.blp

Passive

The passive icon is the same as the normal, except it have no borders.

This type of icon is just a sub-type of normal icons.

Passive icons in warcraft usually have the following path: ReplaceableTextures\PassiveButtons\PASBTN

Autocast

The autocast icon is the same as the passive, except that it has yellow borders.

Like the passive, this type of icon is just a sub-type of normal icons.

Autocast icons typically end with the following path: ReplaceableTextures\CommandButtons\BTNHealOn.blp

or ReplaceableTextures\CommandButtons\BTNHealOff.blp depending whether the ability is turned on or off.

Disabled

The disabled icon is basically the same as the normal, except that it has no borders and is more dark.

However the disabled icon must always have the following path: ReplaceableTextures\CommandButtonsDisabled\DISBTN

Note: The "BTN" prefix isn't necessarily needed, but is recommended to keep icon images better organized.

Technically an icon would work without the "BTN" prefix as well, as long as the disabled version has the "DIS" prefix:

whatever\myIcon.blp

ReplaceableTextures\CommandButtonsDisabled\DISmyIcon.blp

Import the Icon

Step by step.

Open up the import manager (F12)

Then choose to import file (Ctrl+I) and browse through your files and find the icon you wish to import.

Once the icon is imported, you need to change it's custom path.

To do so, right click on the icon file on the list and select the option "Modify File Properties..." from the drop down menu that appears. Or you can simply double click on the icon file on the list.

Here, you can adjust the path once you've checked the checkbox "Use Custom Path".

Change the path for the icon file and click on "OK".


 Last edited: Nov 16, 2006

#### Lord Zodiac

#### Lord Zodiac

 Level 12

JoinedFeb 10, 2022Messages254

Resources: 38

#### Resources
Icons 38

I have the icons and they are imported I am sure this needs to go war3mapImported\ and maybe changed to something else and insurted where

#### Attachments

line.PNG

 20 KB

 · Views: 219



#### Shawwy

#### Shawwy

 Level 3

JoinedMar 6, 2022Messages20

Is there a way to edit multiple BTNs at once?

I have over 100 icon files in my import manager (without counting the DISBTN and DISPAS).

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Shawwy said:

 Is there a way to edit multiple BTNs at once?

I have over 100 icon files in my import manager (without counting the DISBTN and DISPAS).


Click to expand...

Maybe this tool helps: FaMPQ Import Fixer

#### Shawwy

#### Shawwy

 Level 3

JoinedMar 6, 2022Messages20

Warseeker said:

 Maybe this tool helps: FaMPQ Import Fixer

Click to expand...

Can this work with campaign files?

 You must log in or register to reply here.


### Similar threads

 Importing Models, Skins and Icons.


Traxamillion

Mar 7, 2010

Misc/General Mapping Tutorials

Replies28Views89K

Jan 23, 2022
Shadowsky

Locked

 How to import a Icon


Archian

Apr 15, 2006

Tutorial Archive

Replies25Views7K

Sep 11, 2006
Ralle

 Passive ability with a cooldown (attack-based)


Arisato

Feb 9, 2012

Misc/General Mapping Tutorials

Replies42Views42K

Mar 25, 2025
Alyssa-Quinxzel

Locked

 Icon Trouble


tonic

Apr 23, 2004

Texturing & Surfacing

Replies0Views2K

Apr 23, 2004
tonic

 Universal SD/SD+/HD Icons


Mayday

Oct 16, 2020

Misc/General Mapping Tutorials

Replies10Views6K

Nov 19, 2024
Mayday

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