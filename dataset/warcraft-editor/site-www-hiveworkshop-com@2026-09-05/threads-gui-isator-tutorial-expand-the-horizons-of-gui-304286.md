---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/gui-isator-tutorial-expand-the-horizons-of-gui.304286/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# GUI Isator Tutorial Expand the Horizons of GUI
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/gui-isator-tutorial-expand-the-horizons-of-gui.304286/>
#### _Guhun_

#### _Guhun_

 Level 12

JoinedJun 12, 2010Messages413

Resources: 17

#### Resources
Models 1Maps 1Spells and Systems 9JASS 4Tutorials 2

Introduction

Credits to the creator of GUI-sator: rgf

Files & Images: Tutorial attachments

Many people have used hacked World Editors with custom GUI actions that expand the GUI API and allow GUI coders to have more control over WC3. However, few people know of GUI-isator, a handy tool that allows anyone to create new functions for GUI in the World Editor. It even supports adding vJASS methods as functions.

This is a simple tutorial that will guide you in the usage of this tool to expand the functionality of the World Editor's GUI triggers and allow you to use advanced JASS systems (that provide the config files for their functions) seamlessly.

 What is this tutorial for?


This tutorial's main objective is to guide GUI users through the process of adding custom functions to their GUI API using GUI-isator. This, it will not teach you how to use the tool to create your own custom functions. If you know JASS, it should be pretty easy to figure it out from the program's description.

 Who is this tutorial for?


I made this tutorial because I wanted to provide a better API for my GUI systems, so I decided to create GUI-isator config files for them. However, I think it might be somewhat confusing to use the program for this purpose from the description alone, so I opted to make a tutorial.

Installation

First, follow this link to download GUI-isator from THW. After you have done so, open the folder you have downloaded. It should contain two files, a readme file and a .rar file. To open a .rar file, you should use a tool to uncompress it and see its contents. I recommend 7-zip it is a free and widely-used tool.

After you have opened the .rar file, you will need to extract all of its contents into your Warcraft III main (not documents) directory. You will probably find it in the path C:\Program Files (x86). If you want a more convenient way to access the exectuable file, you can create a shortcut by right-clicking it, then move the shortcut wherever you wish.

Now, you can run the .exe file, and GUI-isator should open. Click on "Reset the files". You will then be promted to select a file name. This file name will be used if you ever want to save your current configuration. When it prompts you to delete the conjugued configuration file, click on "Yes". This will delete the default configuration file created by the program.

 Step-by-Step instructions


Download GUI-isator from The Hive Workshop.

Extract the files inside the .rar folder into your Warcraft III directoy. C:\Program Files (x86)

Run the exectuable file to open GUI-isator.

Once GUI-isator is open, click on "Reset the files".

Type in the name of your new configuration file, it can be any name you want.

When you are asked whetever the conjugued configuration file should be deleted, click on "Yes".

Usage

 Load Configuration File


To load a configuration file from the program interface, you must move it to your Warcraft III directory, where the program has been installed. After that, all you need to do to load the new functions from the file is to open the program and click on "Add Configuration File", then find your file and click on "Open File".

 Step-by-step Instructions


Move the configuration file (.ini) to your Warcraft III directory.

Run GUI-isator and click on "Add a configuration file"

Choose your file in the list of files and click on "Open File"

 Examples


Try adding the .ini configuration file inside this zip folder to your World Editor. After you have added the configuration file in the program, open this map on the World Editor. If you check the triggers, you should notice some strange actions with a new category: LinkedLists. Now you will be able to use these functions in your map and open maps that include these functions. However, your map will no longer be compatible with the default World Editor. If you'd like someone else to open your map, tell them to download GUI-isator and send them your configuration file.

 Create Your Own Configuration File


After you have added all the configuration files you want, you can create your own configuration file by clicking on "Reset the files" again. This time, after you choose a new file name, you will want to click on "No" when it prompts you to delete the conjugued configuration file, so your old configuration file is not deleted.

After you have done this, your configuration file, which includes all the functions you have added through other configurations files, will be saved in the same folder where you installed GUI-isator. Remember, the name of this file is the original name you chose when you first opened the program and clicked on "Reset the files", not the name you typed in after you clicked it this time.

You shouldn't really ever need to reset your files, unless you want to restore the World Editor to its default state. If you want to get your custom GUI functions back after reseting your files, you can load your created configuration file like you would any other configuration file.

Overview

The main advantage of this program is that it allows coders of GUI-friendly systems to provide a more convenient API for their users, since they will now be able to execute JASS functions with GUI instead of having to run a trigger after setting some global variables.

Obviously, coders will still need to make their systems compatible with the default GUI API in order to get their submission approved in the Spell Section. However, providing a config file and a link to this tutorial should be enough to get people interested in expanding the capabilities of GUI and bettering their map-making experience.

 TO-DO List


Add more images

Add more examples

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I was really into this idea a few years ago.

It is great for personal use, yes.

But you cannot assist people in the help zone with your fancy functions.

Nor can you enter contests(probably?)

Or submit spells.

I am curious however, what if you import the config into the map?

Skins and such are loaded and visible in the editor, wouldn't this mean everyone can open the map if it is imported?

#### _Guhun_

#### _Guhun_

 Level 12

JoinedJun 12, 2010Messages413

Resources: 17

#### Resources
Models 1Maps 1Spells and Systems 9JASS 4Tutorials 2

Yeah, if you want to submit stuff, you will have to keep your code compatible with the default world editor. However, the point of this tutorial is not to help people make custom functions themselves, but to help them import custom functions made by other people.

Importing the config will do nothing, since the config is only useful for the GUI-isator app. However, importing the text files generated by GUI-isator mgiht work, but I was sure it wouldn't that I didn't event test that. I'm pretty sure it won't work because the editor probably looks for the functions in the text files when it is actually opened, not when it opens a map. Though I guess it's worth testing.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice tutorial. Definitely could be useful for some folks.

But before I approve, I think it should have at least one concrete example that goes through the process of creating an ini file and what the contents actually mean. At the moment, it is mostly just useful for getting people exposed to the program and being able to load other people's config files.

#### _Guhun_

#### _Guhun_

 Level 12

JoinedJun 12, 2010Messages413

Resources: 17

#### Resources
Models 1Maps 1Spells and Systems 9JASS 4Tutorials 2

I think the target audience for the people who want to create a .ini file and the people who just want to add the GUI functions is pretty different. If you want to create a .ini file, you probably already know JASS and the inner workings of the world editor and how it treats variables differently. If you just want to add custom functions into GUI that someone else made, you are likely someone who just knows GUI and how little idea about how it works internally and how JASS is different. This tutorial is targeted more at the second kind of person, who just wants to use JASS systems and functions without the hassle of using custom scripts and GUI wrapper triggers.

If I add another section about what the config actually means, that will likely make the tutorial more confusing to the people I want to help with it. That's why I thought, at least. If you really think it could benefit from explaining how to create new custom functions and ini files, I'm open to adding that as well.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Gotcha, that makes sense. Approved!

 You must log in or register to reply here.


### Similar threads

Locked

 Gui-isator : a program to mod the GUI editor


rgf

Mar 8, 2014

Warcraft Editing Tools

Replies22Views5K

Mar 22, 2014
Troll-Brain

Locked

[JASS]
 JASS to GUI


orcfan32

Oct 16, 2004

Triggers & Scripts

Replies4Views2K

Dec 7, 2004
MasterGT4

 Effortless and Efficient Unit Group functions for GUI Triggers


Spacebuns

Jan 6, 2022

Trigger (GUI) Editor Tutorials

Replies4Views4K

Jan 16, 2022
Spacebuns

 Chaosy's GUI tutorial for beginners


Chaosy

Apr 6, 2012

Trigger (GUI) Editor Tutorials

Replies9Views8K

Dec 25, 2016
Chaosy

Locked

 How to make JNGP GUI see hashtable functions?


mvlabat

Feb 16, 2013

World Editor Help Zone

Replies8Views2K

Feb 17, 2013
TKF

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Trigger (GUI) Editor Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top