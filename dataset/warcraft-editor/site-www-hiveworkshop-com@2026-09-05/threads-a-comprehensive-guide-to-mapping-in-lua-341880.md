---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/a-comprehensive-guide-to-mapping-in-lua.341880/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# A Comprehensive Guide to Mapping in Lua
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/a-comprehensive-guide-to-mapping-in-lua.341880/>
#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

### Getting started

### Lua Fundamentals

### Advanced concepts

### Appendix

### Introduction

### Why Lua?

### Wc3 Setup

### IDE Setup

### The Warcraft API

### Executing Code

The presentation of this guide is optimized for the Nightelf style of Hiveworkshop. I recommend swapping to that style before reading. Use the small style-button on the bottom left of the site (scroll down to the grey bottom bar).

If you are reading on mobile, I suggest using landscape mode for better readability of both code- and table-blocks.

This guide took months to complete. Please leave a like, if you find it helpful.

Introduction

This guide aims to be a comprehensive Lua-tutorial for Warcraft mappers.

It includes the relevant knowledge to create a full map in Lua, i.e. how to setup a Warcraft map in Lua mode, how to program in Lua and what to be aware of specifically in the Warcraft environment. It also points out differences to JASS and features several resource spotlights from the Hive.

To be clear, this guide teaches a lot of programming knowledge, but it is not a programming tutorial. I assume you have programmed before (at least on a beginner level) and are familiar with all basic concepts like variables, loops, conditions and data structures (arrays, hashtables, ...).

If you have worked in JASS before, you are perfectly prepared.

This guide will not be a quick read. If you are experienced with Lua-programming already, you can of course cherry-pick the chapters you are interested in. But if you come here to learn from scratch, definitely count some hours in (it's gonna be worth it, as there is no real shortcut around actually learning at least the core of the language).

If you come from (v)Jass, don't intend to spend much time reading and rather want to focus on how your known concepts can quickly be translated into Lua, I recommend reading @Bribe 's excellent tutorial about switching from vJass to Lua instead.

Motivation

Lua was one of the additions that Warcraft Reforged brought to the game, and the greatest for me personally. The language adds so many possibilities that make mapping more joyful than ever before. It lets you write much simpler and shorter code, improves performance, allows you to overwrite Warcraft natives and just gets rid of all these small annoying JASS nuances we've got used to over time.

At the same time, running a map in Lua-mode adds risks. The language needs more knowledge to get started, is more prone to bugs and has additional causes of desyncs, at least for the unexperienced user. The knowledge to prevent these issues is present on Hive, but (as always) scattered across the site and actually hard to find, if you don't already know about them in advance.

So far, we were missing a central place collecting all this information and presenting it in a way accessible for Lua newbies.

This guide attempts to be exactly this - a summary of the relevant knowledge for Lua mapmaking and the single thing you have to read to get started. As such, it will be subject to updates and further additions, whenever the community develops something substantial.

I personally had the privilege of learning vJASS and Lua in direct succession, just to see which ones fits me most. And I can say without a doubt that Lua was a relieving experience offering so many more options and so many less limitations that I never want to go back.

How to read this guide

You see that there are four tabs on top of this guide, which include several chapters each.

Getting started helps you with your Warcraft and coding environment setup. This preparation is key, if you don't want to get into trouble later. Lua is a lot of fun, if done right, but can be a frustrating experience, if done wrong.

Lua Fundamentals teaches you basic Lua programming knowledge. This knowledge is pretty much required, if you want to build a custom map in Lua. Several sections are hidden behind further-reading-spoilers, which can safely be skipped on first read.

Advanced Concepts as a whole can be considered further reading. You don't necessarily need it for mapping, but understanding it will further enhance your Lua skills. I recommend reading the chapters here on demand, i.e. as soon as hey become relevant to you.

Appendix is an in-progress repository for collecting various Lua-related information that I (or others) find useful. One of the most important reads here is the chapter about desync threats in Lua-mode.

Several sections of the guide will be emphasized using one of the following info boxes:

Usually on top of a section to summarize its key message.

If that key message sounds familiar, you can probably scroll to the next section.

Refers to a later chapter that is going to discuss the current content in further detail.

Includes a warning about something to be aware of. Don't jump over these or you might later be unpleasantly surprised.

Includes Warcraft-specific Lua-knowledge. Read these carefully.

This guide will reference other websites and resources, whenever sensible. You find a summary of all these links in Appendix - Useful resources. If you search for a resource you think you've seen before in this guide, this is your go-to place.

Credits

Special thanks to @Luashine for her extensive and helpful proofread. Much appreciated!

Why Lua?

You are already here reading my tutorial, so I probably don't have to convince you of anything, but its still a good idea to think about why using Lua instead of JASS would be a good idea at all.

Please note that Lua and (v)Jass are not the only options you have. You can basically use any other programming language on earth by compiling it to Lua before importing the code to your map. Sure thing, you need to have a to-Lua-compiler for the desired language to begin with. The most famous example in the Wc3 community is Typescript. Doing so allows you to work in a type-safe-approach whilst also benefitting from most advantages Lua has. This is a different topic, though, and will not be covered over the course of this tutorial.

Advantages of using Lua (over JASS)

Lua is a wide-spread language that you might want to use in other games or tools in the future (for instance, Adobe Photoshop is using Lua as its scripting language).

You can find helpful Lua-resources outside of Wc3, let it be tutorials, snippets, answers to frequently asked questions etc.

Lua offers many features that JASS doesn't offer, but not vice versa, making it the more powerful language, period. Honestly, JASS is just incredibly restricted in comparison to basically every other language on earth. Examples include scoping, data structures, loops, callbacks, I could go on forever.

The fact that Lua provides far more options than Jass makes it a more fun experience (as long as you don't fall for too many Lua traps).

Lua provides some powerful built-in libraries in addition to the Warcraft natives, which can save you a lot of time.

Lua offers much better performance. Arithmetics for example is like 10 times faster than in Jass.

You can overwrite natives in Lua.

Lua has type-freedom (which you also see listed as a disadvantage below, haha). This often comes in handy with generic data structures, which don't require this Jass-like textmacro copy-pasting anymore.

Lua has no op-limit (god bless)

Hidden natives (as well as the few good ones from common.ai) are immediately available without previous declaration.
Disadvantages of using Lua

Lua is not type-safe, i.e. you can't denote function parameter types. This is meant to be a feature of the language, but definitely has its drawbacks (especially regarding syntax check, which can't prevent wrong-parameter-errors anymore).

There are solutions of course, like enabling ingame error messages with Debug Utils.

Lua needs more knowledge to get started.

Lua has some additional risks of producing desyncs in contrast to JASS (but this guide will get you covered, so don't be afraid!).

Preparing the World Editor

To use Lua in a Wc3-map, Lua must be set as the map's scripting language in the world editor preferences.

In the main window top bar, go Scenario -> Map Options. Choose Lua in the Script language dropdown, which is the second from below.

If the dropdown is greyed out, it means that you have active JASS code somewhere in your trigger editor.

In this case, you can do one of the following (but make a safety copy of your old map beforehand):

disable all scripts containing JASS code, switch to Lua, rewrite Jass to Lua, re-enable scripts

export your triggers from WE (File -> Export Triggers), delete triggers in trigger editor, switch to Lua, re-import triggers, rewrite to Lua.
In doubt, creating a new map will always do.

From this point on, entering Lua code is the same story as with JASS code. You open a script in the trigger editor (Ctrl + U), enter your code, save the map, finished.

One thing I noticed after switching to Lua mode was that the syntax-check (Ctrl+F10) was either trying to conduct a JASS-syntax-check (immediately leading to error prompts) or not doing anything at all.

You should avoid using Ctrl+F10, but you still get a proper Lua-syntax-check upon saving the map, so don't worry!

If you struggle rewriting your JASS code to Lua (if there is any) or just want to learn something by looking at generated code, you can use one of the following converters:

GitHub - goshante/cjass2lua: Code converterter from cJass and JASS to Lua for Warcraft III

Downloadable .exe. Supports Jass and cJass, but not vJass. Automatically adds emmy annotations to the code, which you should do anyway.

vjass2lua-webapp

Webapp made by @Bribe that converts Jass, vJass and Zinc to Lua. Converted vJass might require importing extra code (that is linked within the resource). Also adds Emmy Annotation.

Both converters gets most of the conversion right, but no converter is flawless.

Use scripts instead of trigger documents

The ability to create script documents exists in the trigger editor since Warcraft Reforged. Before that time, we were used to text-based trigger documents. In JASS-mode, you can still create those by converting a GUI trigger to custom text. In Lua-maps though, this convert-option is not available. Yes, you can still get a text-based trigger document by creating it in JASS-mode before switching to Lua-mode (and produce further ones by copy-pasting), but there is no point in doing so. Their main selling point was their checkbox for map-init-execution, which also doesn't exist in Lua-mode. Plus, converted trigger documents are compiled strictly before script documents (no matter their order in the trigger editor), which is really annoying for managing dependencies later on.

So essentially, just stick to scripts documents (Ctrl+U) and you are fine

The missing convert-to-custom-text option on GUI-triggers has a downside: Finding the names of Wc3 natives by converting them from GUI (benefitting from the excellent GUI search tool) is not possible anymore. To compensate for that, you can of course open a separate JASS-map for this purpose, which is exactly what I do. It doesn't even require effort, because the World Editor opens an empty JASS map on every startup (which stays open even if you open your own map afterwards). You can always switch between the maps (without loading times) using the Window tab on the WE top menu bar.

Use a real development environment!

Now, where do you want to write your code? You might be tempted to write it directly into the Wc3 trigger editor, but in all honesty, that's a terrible idea. For programming and specifically Lua, features like Syntax highlighting, auto-completion, parameter preview, etc. exist and will save you a lot of time.

Notepad++ partly offers these features, but trust me, it's still not enough. Lua-coding without a proper development environment can be incredibly frustrating and will sooner or later lead to your personal quit moment. This is something I can not stress enough. Lua has more error-potential due to it's type-freedom (we speak about that later), so the Lua-syntax-check on map save will (in contrast to the JASS one) simply not spot errors resulting from passing wrong parameters to functions. You do need a proper development environment to compensate for that. No excuses!

I personally recommend Visual Studio Code (it's free) and its Lua extension made by @sumneko.

You can also choose the MS-branding/telemetry/licensing-free version VSCodium.

If you want to use VSCode + sumneko-extension (do it!), please conduct the following steps:

Download and install Visual Studio Code

Open VSCode, click on the Extensions-icon on the left sidebar (bottommost of the 5 icons for me), enter sumneko.lua in the searchbar, press enter and install the Lua-extension that's popping up.

-> The extension is in active development and frequently pushes new updates, which arent't always bug-free. If you prefer a stable version, I personally recommend version 3.7.4. You can go back to that version using the gear-icon next to the extension name.

The extension might require you to restart VSCode.

Decide for a directory on your harddrive where to store your future code files. Download the files blizzard.lua,common.lua and HiddenNatives.lua attached to this post and copy them to the chosen directory.

In VSCode, go File -> Add folder to workspace and choose the directory from the previous step. Then go File -> Save Workspace as and save the workspace file on your computer.

Subfolders and files created in the chosen directory will automatically be added to the workspace.

The sumneko-extension offers several features to explore. The default settings are not bad, but they will likely not work for large files such as common.lua and enable certain diagnostic checks that I don't consider beneficial for Wc3 mapping. I recommend changing your settings.json (a global settings-file for VSCode where the sumneko-extension stores its preferences) for better user experience.

To do so, open C:\Users\%USERNAME%\AppData\Roaming\Code\User\settings.json (assuming you are on a windows PC) and insert the following settings:

 JSON:

{
 "json.maxItemsComputed": 30000,
 "Lua.runtime.version": "Lua 5.3",
 "Lua.workspace.preloadFileSize": 2000,
 "Lua.workspace.maxPreload": 1000,
 "Lua.hint.enable": true,
 "Lua.type.weakUnionCheck": true,
 "Lua.type.weakNilCheck": true,
 "Lua.diagnostics.disable": [
 "lowercase-global",
 "inject-field"
 ],
 "editor.parameterHints.enabled": true,
 "workbench.editor.enablePreview": false,
 "editor.semanticHighlighting.enabled": true
}

These settings will:
enable large file support

adapt the extension to the Lua version used in Wc3

disable diagnostics that I consider disadvantageous for Wc3 mapping (strong union check, strong nil check, lowercase-global, inject-field)

enable function param type preview boxes (like this:

)

disable preview editors (the ones that automatically close when opening a new file, I hate them)
You can either replace your current json by the one listed above (which will obviously overwrite all settings you had done previously) or add the new ones into the existing json (in which case you should append a comma to the last existing entry and copy-paste the above json without outer brackets after it).

Go back to the "Explorer" tab of VSCode (topmost icon in the left sidebar). You can create new Lua script files by pressing Ctrl+N into Ctrl+S, choose the workspace directory and select "lua" as fileending. The extension will only work on ".lua"-files.

You are ready to write Lua code.

VSCode after above preparation allows you to conveniently write your Lua code, but not to execute it. See chapter Executing Code for more information.

Building your map

To test your code in Wc3, you obviously need to get it into your map. Do so by using one of the following options:

Manual Copy-Pasting

Copy-paste your code manually from VSCode to a script file in the Wc3 trigger editor.

It's a good habit to maintain the same structure in VSCode as you do in the Wc3 Trigger Editor (i.e. the same folders, subfolders and document names).

Also, let VSCode always be your "leading" system, i.e. only conduct code changes in VSCode and copy them to the World Editor, never vice versa. This prevents accidental code loss.

I know, copy-pasting code to the trigger editor can be annoying, but using VSCode is definitely worth it, so don't let this process discourage you!

The Wc3 trigger editor sometimes behaves buggy upon pasting code: when replacing currently selected text by pasting something into it, the result can screw up and look like chinese letters. If that happens, don't worry. Just delete the mess and paste again.

To prevent this bug from happening in the first place, delete the text to replace before pasting the new code.

[Advanced] Ceres

Tool hosted on Hive that completely builds your map file from your code, so you wouldn't ever need to manually copy-paste anymore.

Offers several other advanced features. The tool is no longer being maintained, so use at your own risk.

[Advanced] cheapack

Another external tool. Simpler alternative to Ceres, which doesn't have Ceres advanced features, but is well suited for injecting your new code into the map without breaking map editing in WE itself.

Use at your own risk.

[Advanced] Dencer.warcraft-vscode

VSCode Extension with Wc3 build-map functionality. Integration into VSCode is comfortable.

For first-time setup, these instructions by @Trokkin might be of help.

Again, use at own risk.

Start with GUI

Experiment with GUI in the trigger editor before starting with Lua.

This will make you learn about the Warcraft environment in a fun and secure setting.

Mapping in Lua is not solely about learning Lua. We are also required to understand how to produce something to show up in the game, in other words how to use the Warcraft API.

Just to keep you from running into walls: There is no real point in starting with Lua (or Jass), before you have understood the basic features of the World Editor. That includes placing some terrain, editing unit properties in the Object Editor, but first and foremost experimenting with GUI in the trigger editor.

This preparation is key: Using GUI as first contact with the Warcraft API will give you the required knowledge to attempt doing the same stuff in Lua. Experimenting in the trigger editor will teach you that Warcraft has a trigger-based system, in which you may execute your desired actions only as a response to certain ingame events (like whenever a unit dies). GUI will show you the available events, the actions available for response, the different object types (players, units, triggers, timers, ...) and the properties they have.

Leave yourself time with this. Spending a few days or weeks with creating small little pieces in the World Editor will pay off sooner or later. Mapping is supposed to be fun and struggling with too many things at once may put you off. If you can't solve a problem, ask questions in the World Editor Help Zone or on Discord.

Once you feel comfortable in GUI and are able to get the results you want, you can start coding your triggers in Lua.

The focus of this tutorial will lay on the Lua language and the caveats you must consider upon using Lua in the Warcraft environment. This chapter will still try to get you started with basic knowledge about the Warcraft API.

The Warcraft API

All Warcraft API-functions are listed in the attached files common.lua, Blizzard.lua and HiddenNatives.lua.

There are also internet repositories like Jassbot and Jass documentation database.

First things first, what is the Warcraft API? Well, basically a bunch of global functions (like CreateUnit) available for use within your scripts in the trigger editor. Those functions do generate an effect ingame (creating a unit in the mentioned case) and don't exist outside of Wc3. Writing working code really boils down to knowing the API functions available.

If you closely followed the chapter IDE setup, you will by now have downloaded the files common.lua and Blizzard.lua and included it into your scripting workspace (assumably VSCode + sumneko-extension). These two files are listing 99% of all existing Warcraft API functions!

These definition files are meant to support you within VSCode. You don't need to copy-paste them to the World Editor!

common.lua lists the so called natives, functions that run directly in the Warcraft engine. We don't exactly know what they internally do, but we can see their outcome after using them (and their function name mostly suggests their behaviour). This is why none of the listed natives actually shows a function body.

Blizzard.lua on the other hand is a file of API functions Blizzard has written in JASS (/Lua) on top of the natives from common.lua. As such, the file does show function bodies and you can theoretically read through what every function does (apart from the fact that these functions will be using those from common.lua, which you can still not look into).

Some functions from Blizzard.lua are well-made, others are sub-optimal or redundant. Don't stress yourself too much with getting this knowledge, it will come automatically over time. There's nothing in there you couldn't do with common.lua yourself.

Warcraft has even more sources of API functions:

There is a file called common.ai, which contains functions used by Warcrafts AI-engine. Only some of them are actually useful for modding, others are broken or simply not available for us.

And finally, there are API-functions that are not listed in any of these files, so called Hidden natives.

Attached to this guide, you find a file called HiddenNatives.lua, where I combined a selected few hidden/common.ai natives.

Knowing the API-functions is just the first part of the deal. You also need to know where to execute them to make them work (most of the time: by creating a trigger during loading screen and adding the desired action to it).

Please consult the chapter Executing Code for more information.

There is a lot of knowledge on hive centered around which API functions are good or bad to use, but I honestly suggest that you don't bother too much with this, until you get your stuff working. Caring for too many details too early just kills your fun

If you have specific questions about any API-function, you have many options: searching on hive, searching on Google, looking into the Jass documentation database, consulting Lep's Jassbot, try and error, asking questions in the World Editor Help Zone, asking on Discord, ...

Finding the API-functions you need

You can find API-functions by using the GUI-to-JASS-conversion feature of the World Editor.

This requires switching to a JASS-map once in a while (which WE always opens on startup).

The question remains: If I want to do <insertAnything>, which API-function should I use?

Reading through common.lua and blizzard.lua is not an option - the files are way too big (although you could in theory try your luck with text search).

A better way is to search in GUI. It has all actions structured into categories (Unit, Player, Animation, etc.) and provides an excellent search-tool for finding the right trigger action for your needs.

Fortunately, we can still benefit from that search-tool when using Lua: the World Editor offers the option to convert a GUI-trigger to code. That means, you can find an API-function by inserting the desired action into a GUI-trigger and converting it!

One little problem, though: The convert-feature is only available in maps set to JASS-mode.

The solution is just as simple: Switch between your Lua-map and an empty Jass-map using the Window-tab on the top bar of the Editor. This is particularly easy, because World Editor always shows an empty JASS-map on startup, which stays open even after we opened our Lua-map. The Window-tab should show a list of opened maps, of which the first supposably reads "1 Untitled" (which is the empty Jass map). Clicking here will switch maps and there is near-to-no loading time from doing so. You don't even lose unsaved progress, because all maps stay opened this way (but saving is still a good practice).

To summarize, when you search for an API-function, switch to empty JASS-map, use GUI-conversion, copy API-function, switch back.

Example:

Let's say, we want to create a Footman for Player 1 every 10 seconds on the location X=0, Y=1024.

We switch to the empty JASS-map in World Editor by navigating Window -> 1 Untitled.

We open up the trigger editor and create our trigger in GUI:

ExampleTrigger
Events
Time - Every 10.00 seconds of game time

Conditions

Actions
Unit - Create 1 Footman for Player 1 (Red) at (Point(0.00, 1024.00)) facing 0.00 degrees

We convert that trigger to JASS-code by going Edit -> Convert To Custom Text (in the Trigger Editor top bar).

The result should look like this:

 JASS:

function Trig_ExampleTrigger_Actions takes nothing returns nothing
 call CreateNUnitsAtLoc( 1, 'hfoo', Player(0), Location(0, 1024.00), 0.00 )
endfunction

//===========================================================================
function InitTrig_ExampleTrigger takes nothing returns nothing
 set gg_trg_ExampleTrigger = CreateTrigger( )
 call TriggerRegisterTimerEventPeriodic( gg_trg_ExampleTrigger, 10.00 )
 call TriggerAddAction( gg_trg_ExampleTrigger, function Trig_ExampleTrigger_Actions )
endfunction

The top function of the code is the trigger action, while the bottom function creates the trigger and adds the event and action to it.

By looking at the code, we see the following API-functions:

CreateNUnitsAtLoc must be the function creating the unit. It takes a few values, containing Player(0) (which must be Player 1) and Location(0, 1024.00) (which must be the native creating the location).

CreateTrigger seems to create the trigger in question and TriggerRegisterTimerEventPeriodic seems to add the every-10-seconds-event to it. TriggerAddAction finally must be the API-function that attaches the trigger action to the trigger.

Now we most likely go back to VSCode and write our code there. Copy-paste the function-names you need from the code above. You don't have to remember all the parameters, because VSCode will remind you via tooltip after writing the opening bracket:

In World Editor, go back to your map via Window tab.

This procedure sounds cumbersome, but it's actually really quick. Also remember that you only have to do it once per API-function you search for. Next time, you can basically look on your own code to find it again (or use your memory, lol).

Advanced users can now optimize the API-functions they use by tracking down dependencies in Blizzard.lua.

VSCode offers a neat feature to jump from one function definition to another: Holding Ctrl and left-clicking on a function name will open the file containing it and jump directly to its definition.

Doing so with CreateNUnitsAtLoc will open Blizzard.lua (assuming you have it in the same workspace) and let you study its body. You will see that it does a lot of unnecessary stuff, while the actual unit creation is forwarded to another API-function, CreateUnitAtLocSaveLast.

Conducting the same trick again, you can see that this mainly executes CreateUnitAtLoc. This last one comes from common.lua, so there is no further reference to follow. You now can choose to use CreateUnitAtLoc and save the overhead from the original functions.

Sure thing, doing so requires you to be sure enough that you don't need the overhead actions within the original function.

Changing GUI-Habits

GUI is nice for learning the Warcraft API, but questionable as a programming tool. Its limitations force you to do things that would be considered terrible style in other environments. Staying in GUI for too long can manifest bad habits that you should try to let go.

As a loose list of examples:

Needless to say, but get used to defining your own functions and using local variables. These two missing features are the major flaws of GUI and not utilizing them in real programming will lead to a plethora of bugs in your code.

Don't use the Variable Editor anymore. You are supposed to code in your development environment, not in the World Editor. You can easily define global variables in your Lua-scripts instead.

Stop using Wc3-hashtables. They are a blessing for GUI-users, but Lua-tables are strictly superior.

Try not to use Locations and instead process x- and y-coordinates separately. Lua-functions make this very easy, because they allow for multiple return values. Nearly every Warcraft native dealing with locations also exists in a version that deals with coordinates instead. For instance, you can forget about CreateUnitAtLoc and instead utilize CreateUnit.

Try to not rely on "Get-Last-Created-Whatever"-functions. If you want to access a unit after creation, don't use GetLastCreateUnit(), but save it to a variable during creation via local u = CreateUnit(Player(0), FourCC('hfoo'), 0,0,0) and use u afterwards.

Executing Lua-code in Wc3

This chapter will show how to execute Lua-code in Warcraft 3.

If you are still learning Lua basics, better stick to an external Lua-engine for now (like this quick-to-use online Lua interpreter), until you start with Warcraft-specific stuff. This prevents endless copy-paste-restart-testmap iterations.

Before we start, I strongly recommend importing the following two resources into your map (and your VSCode-workspace) that I consider mandatory for Lua Wc3 mapping:

Total Initialization and Debug Utils.

They both support (safely) executing Lua code and will be discussed in the progress of this tutorial.

Creating and running triggers

You know that Wc3 has an event-based script language. That means, the main way to execute code is to write it into a function and to use a trigger to execute that function. But triggers don't come from nothing, do they? Don't we also need to create those triggers by executing code in the first place?

Yes, we do. In JASS-mode, you maybe remember this little "Run on Map Initialization" checkbox located in the interface directly above the trigger code box (which became visible after converting a GUI-trigger to text)? That checkbox pretty much gave you the option to create triggers during loading screen, which would later run the game.

Well, that's no longer possible to do in Lua-mode, because the "Convert to text"-option no longer exists.

Instead of converted triggers, we do have Scripts now (Ctrl + U), but these don't provide the mentioned checkbox.

So what shall we do to create triggers?

You have two options: Using a GUI-trigger or utilizing the Total Initialization library. The latter is clearly superior, so I'm going to show that first.

Resource Spotlight: Total Initialization

Importing this resource into your map provides the ability to run functions during map loading screen. In particular, you can use Total Initialization to execute functions that create triggers

All functions executed via Total Initialization will be properly error handled, i.e. will print a visible error message on screen after game start, if not successful.

We examplatory show some of the hooks provided. For a full list, you can visit the resource main page.

OnInit.root(f)Executes f in Lua root (that is, immediately). Effectively the same as just writing f(), but with error handling included.

OnInit.global(f)Executes f during loading screen after udg_-variables have been assigned (which probably doesn't matter for you, because you won't create variables in the variable editor and therefore don't have udg-variables). This hook is sensible to use, when you want something to happen prior to the other ones below.

OnInit.trig(f)Executes f during loading screen after GUI-triggers have been created (in other words, after gg_trg-variables have been assigned). Your map will probably not have any GUI-trigger, but this is still the ideal point for your own trigger creation.

Happens after Global Init above.

OnInit.map(f)Executes f after the GUI Map initialization event has fired. This is ideal for most other stuff you do in loading screen.

Happens after Trigger Init above.

OnInit.final(f)Executes f upon game start, i.e. immediately after the game has finished loading. This is ideal for everything you want to have in place right at start, but that can't be done during loading screen (like tracking mouse movement, which would desync during loading screen).

Example: Creating a trigger via OnInit.trig():

The example requires basic Lua-knowledge to read (which you'll get in later chapters), but I assume that you can already grasp the idea.

 Lua:

--We create a trigger that shall print "Hello World", whenever Player 1 types "-a" into the chat.
do
 local function sayHello()
 print("Hello World")
 end

 local function createExampleTrigger()
 local trigger = CreateTrigger() --we don't even need a global trigger variable
 TriggerRegisterPlayerChatEvent(trigger, Player(0), "-a", false)
 TriggerAddAction(trigger, sayHello)
 end

 OnInit.trig(createExampleTrigger) --will execute "createExampleTrigger" during loading screen
end

It's also noteworthy that Total Initialization includes error handling. If any of your executed functions break, the resource will remember that until the game has started and print a visual error message on screen.

Alternative: Rely on GUI-triggers

As an alternative to using the resource above, you can also create a GUI-trigger using the Map Initialization event and add trigger actions via GUI Custom Script line. Within these Custom Scripts, you can execute functions that create further triggers.

This GUI-method works, but has severe downsides (see spoiler below). Honestly, do yourself a favor and just use Total Initialization. If you do, skip the spoiler below (except maybe for your personal education).

Example:

Write a function that creates your desired trigger:

 Lua:

--We create a trigger that shall print "Hello World", whenever Player 1 will type "-a" into the chat.
do
 local function sayHello()
 print("Hello World")
 end

 function CreateExampleTrigger()
 local trigger = CreateTrigger() --we don't even need a global trigger variable
 TriggerRegisterPlayerChatEvent(trigger, Player(0), "-a", false)
 TriggerAddAction(trigger, sayHello)
 end
end

Run that function in a GUI trigger:

CreateTriggers
Events
Map initialization

Conditions

Actions
Custom script: CreateExampleTrigger()

If you stick to this method, you will probably want to add every trigger-creating-function to the same GUI-trigger (because why should you have more than one GUI-trigger...).

Downsides of the GUI-method

You will not have proper error handling. Making any mistake in a GUI custom script line (can be a typo of the function name or something within the function) will cause the GUI trigger to break at that point without visible error notification (because the game can't print stuff during loading screen).

You will sometimes forget to add one of your trigger creating functions to the GUI-trigger, because it requires leaving your development environment and adding further logic inside the World editor. This error can take a long time to find, because there again is no message resulting from it.

Likewise, deleting one of your triggers will also require both deleting your trigger creating function in your code and the GUI custom script line (again taking an action in both IDE and World Editor). Forgetting about the second will make the GUI-trigger break, because it tries to execute a non-existing function. Trust me, you will get mad debugging a mistake like that.

Searching through tons of custom scripts is super annoying anyway.

It's hard to keep track of what you have already added. It will inevitably become a mess after a while.

You can't create triggers listening to the map initialization event this way. That event will already have passed, when the triggers are created. Instead, you can obviously execute your desired trigger actions directly in the GUI-trigger, so it's not really a downside. Still can be confusing and offers potential for errors.

You need to maintain GUI-code that you can't keep in your developing environment. This kind of breaks with the "Only conduct changes in your IDE and only copy in IDE -> World Editor direction" rule.

The Lua Root

Looking at the example from the Total Initialization section, you might have noticed that the line OnInit.trig(f) wasn't part of any function definition, but was just written freely into the script. We call this free space the Lua root.

All code written to the Lua root is executed once during early loading screen. In fact, this includes your whole script, of which most code will probably be your function definitions.

Consider this simple script:

 Lua:

--This code is part of the Lua root (like everything else) and will run during early loading screen
--The function itself will be defined, not executed (because that's what the code does: it defines a function).
function foo()
 a = 0
end

--You can also execute a defined function in the Lua root.
--Be careful, though. Errors you made in the Lua root will cause the map to not even start (and let Warcraft return to the main menu). This can be a pain to debug.
foo() --executes foo in the Lua root.

The Lua root is a good place to prepare data structures (Lua tables!), define global variables, etc.

This style of coding was not possible in JASS, where all code (apart from the globals-block) had to be part of a function definition.

One important restriction, though:

Don't use Warcraft objects in the Lua root!

At the time the Lua root is executed, many Warcraft natives are not yet functioning properly.

Creating warcraft objects in the Lua root can even lead to desyncs.

This is exactly the reason why we use Total Initialization for trigger creation (triggers are warcraft objects): The library delays function execution to a later point, where the mentioned issues are gone.

 Lua:

--BAD IDEA:
--Taking the CreateExampleTrigger example from above, you might be tempted to create the trigger in the Lua root.
--But please don't do that - CreateTrigger() is a Warcraft native and you shall not execute those in the root.
trigger = CreateTrigger()
TriggerRegisterPlayerChatEvent(trigger, Player(0), "-a", false)
TriggerAddAction(trigger, SayHello)
--Instead of doing this, stick to the method that was presented in the Total Initialization example.

But be cautious: If the code written to the root causes an error, it prevents the map from loading and instead lets you return to the Warcraft main menu.

If your map refuses to load and instead returns to the main menu, you either remember what you have done since you last successfully loaded the map or take a look at the last lines of War3Log.txt located at %USERPROFILE%\Documents\Warcraft III\Logs (assuming you are on windows).

If that doesn't help, try with commenting out code from the Lua root, until your map loads again.

Also, a quick word about references in the root:

Script and code order in the Lua root can be important: If you execute a function in the root that calls another function, that other function must have been defined before. Before means, either above in the same script or in another script located above in the trigger editor. The same holds for tables referencing each other.

The warning above only holds for code dependencies in the root, because the Lua root immediately runs upon Warcraft loading the script on startup, not waiting until everything afterwards has been loaded as well.

Functions determined for later use on the other hand (which should be 95+% of your code) will run after everything has been initially loaded and as such can always reference each other no matter their order in the trigger editor.

In particular, calling one function in the body of another function determined for ingame use (including execution in the hooks provided by Total Initialization) does not require any specific definition order in the map script (in contrast to JASS). We will talk about this again in the chapter Lua Fundamentals - Functions and Closures and Scope.

### Overview

### Debugging

### Variables & Data types

### Strings and Numbers

### Conditions

### Logical operators

### Loops

### Tables

### Functions

### Closures and Scope

### Lua incorporated libraries

Overview

This chapter teaches you how basic programming elements like variable declaration, loops, etc. work in Lua.

As mentioned in the introduction, the reader is assumed to have knowledge about the concepts already, so we rather focus on learning the syntax and specialties of Lua.

I strongly suggest that you try to write a bit of code while reading through the following chapters.

You might be tempted to write your example code into the trigger editor and test it by using the test-map-button, but don't do that, it's going to be incredibly annoying (super long startup time of Reforged, requires triggers to see results ingame). Better use a Lua-console or a website for Lua code execution, until you've got a basic feeling for writing Lua code.

To get started, let us quickly collect a few prerequisites:

The Print-Function

Without any doubt, the print-function is the most important tool to learn Lua.

It will print any value on screen (no matter what type, even Warcraft objects) and supports as many arguments as you wish.

For instance print("Hello World", 42, true, Player(0)) will work just fine.

The call-keyword you know from JASS-code is not required.

Comments

Any line of code starting with two hyphens will be considered a comment:

 Lua:

--This is a single-line comment.

Appending two open square brackets after the hyphens will begin a block-comment, which lasts until you close with two closing square brackets:

 Lua:

--[[
All lines up to the next two closing square brackets belong to the block comment.
]]

Annotations

There is also a special type of comment called Annotation.

Annotations start with three hyphens instead of two (like ---@param playernumber integer).

They are still just comments, but they provide additional standardized documentation, which your development environment might depend on to provide certain intellisense features like type checking and parameter preview.

Annotations play a crucial role in Lua programming, because they are the only option for denoting function input and output parameter types, letting your development environment conduct the syntax check for you that WE isn't capable of.

If you want to learn about annotations, please refer to this thread (but maybe wait with that until you have learned the Lua-Fundamentals).

Good-to-Know's

Lua is case-sentitive. That means variableName and variableNAME are different things.

Whitespace doesn't matter in Lua, apart from the fact that there must be a linebreak or a semicolon in between different statements.









 Lua:

--Let's say we want to use the print-function twice in a row
--You can either put them in separate lines
print("Hello")
print(" World")
--Or you separate them by a semicolon
print("Hello"); print(" World")

Warcraft uses Lua version 5.3.

Debugging

For beginners, Lua is more prone to errors than JASS. The main reason is the Lua syntax check that happens upon saving your map: it doesn't check for function parameter types and function argument counts. In fact it can't, because Lua functions do allow flexible argument counts and function params simply do not have a type to check for.

This behaviour results in a lot of coding-freedom, but clearly has a huge downside: Error Potential.

Consider you have mistyped on a variable name that you passed to a function. Lua syntax check will not inform you about it and your code will probably break ingame without visible error message. You can't even rely on Lua's native debug-library for debugging such a case, because Blizzard disabled it in Warcraft Reforged.

But fear not, we have solutions. The chapter Getting Started - Executing Code suggested to import Debug Utils to your map for a good reason.

When you encounter an error you can't solve, come back to this chapter.

In doubt, ask for help in the Hive forums or on Discord.

Resource Spotlight: Debug Utils

As the name suggests, Debug Utils is a resource that helps you debugging your Lua code. It was made specifically for Wc3 to compensate for the missing Lua debug-library.

Importing Debug Utils into your map will automatically print visible error messages ingame, whenever any trigger attempts to run erroneous code.

It further allows you to execute code via Ingame Console and offers several additional useful debugging tools.

It's a topic on its own, so I recommend just importing the resource for now (to activate error messages) and give it a read after you have learned the Lua fundamentals.

Debugging different code sources

Debugging your code is a very different story depending on where it runs. You can generally think of four categories: Code executed by triggers, through timer callbacks, by Total Initialization and in the Lua root.

The print-function for instance is a very helpful tool for debugging, but certainly not applicable during Lua root (because the game won't even start for Lua root errors, so you won't see the prints).

Debug Utils provides automatic error handling for code executed by triggers and timers (error handling means that a visible error message will appear on screen).

The resource will also give advice on the debug options available, so give it a read as soon as necessary.

Total Initialization on the other hand (which we discussed in the chapter Getting Started - Executing Code in more detail) provides error handling for loading screen errors occuring within the hooks provided.

Errors occuring in the Lua root are hard to debug in general. You know that you encountered such a bug, when the test-map button doesn't load the game at all and instead returns you to the Warcraft main menu. A famous cause are dependencies in the Lua root, but having script files placed in the wrong order in the trigger editor (if B needs A, place B below A).

If you don't immediately spot an obvious cause for your issue, you can investigate using one of the following methods:
Close Wc3 (base game, not editor) to let the game create an error log at C:\Users\%USERNAME%\Documents\Warcraft III\Logs\War3log.txt (assuming you are on Windows). Open that log, scroll down. The last two lines should look similar to this, containing an error message:







 Code:

10/2 22:44:09.712 Map contains invalid Jass scripts that couldn't be compiled by war3, file: war3map.lua @ 5, error: attempt to index a nil value (global 'x')
10/2 22:44:34.140 GameMain Ended

Don't worry about the message saying something about Jass scripts, it's properly referring to the Lua error. The message also includes the line number of occurance in war3map.lua (here: line 5).

Alternative: Comment out parts of your root code, until the map loads again. Continue enabling/disabling pieces until you got to the point, where it's breaking.

Another alternative: Pack code from your Lua root into function definitions and run these using OnInit.root from Total Initialization.

You can even do this for all scripts at once: Create a new script below Total Initialization and one at the very bottom of the trigger editor. Put OnInit.root( function() into the first and end) into the second. These are two pieces are incomplete on their own, but together run your whole script in one single protected function call.

Line number in War3map.lua

All methods presented above have in common that they give you an error message to work with, such as "war3map.lua:622: bad argument #1 to 'CreateUnit' (player expected)". The message includes the reason for the error (which is not always easy to understand), the source document (usually war3map.lua) and a line number in that document.

If you don't immediately know the issue by looking at the message, you will want to look up the breaking line of code. This is easier said than done, because Warcraft copies all your script files from your trigger editor to one gigantic document (war3map.lua!) upon map save and the line number stated in the error message actually refers to that document.

That means, you have to look into war3map.lua and scroll to the desired line.

You have the following options of doing so:

Recommended:

You can get the World Editor to show you the complete war3map.lua by forcing a syntax error in your code. I usually add a plain x at the beginning of some script file and save the map. A popup will show up, which displays the whole war3map.lua. From here, scroll to the line of interest (for big map files, use PageUp/PageDown buttons). Note that the erroneous line in war3map.lua might be one off from the line number you saw in the message (leaving three lines as potential causes of the error), but that usually is enough to spot the issue.

If not, you still got to the the place, where you can insert helpful print-statements (but do so before the erroneous line, because the code breaks at the error).

And don't forget to remove the syntax error from your script after you found what you needed

Alternative:

Export war3map.lua via File -> Export Script and open it with any text editor.
Note: Debug Utils provides the option to convert war3map.lua-references in error messages to those matching the files in your development environment. Doing so requires a little bit of work (essentially writing Debug.beginFile(<fileName>) and Debug.endFile() into every of your script files), but it will save you the effort of looking into war3map.lua for every error in the future.

Global Variables

Global variables can be assigned anywhere in your code without previous declaration.

Just write <variableName> = <value>.

In programming, variables typically need to be declared, before they can be used.

That's not the case for global variables in Lua. We can just read and write to any variable name from anywhere in your code.

That means, you can forget about the globals-block from JASS.

To assign a value to a variable, simply write <variableName> = <value> (no set-keyword in Lua).

You can even read a variable that doesn't exist: it will just be nil (which is Lua's representative for the missing value and equivalent to JASS's null).

 Lua:

-- Reading a variable that doesn't exist:
print(UnknownVariable) --> prints "nil"

-- Assigning a variable:
KnownVariable = 5
print(KnownVariable) --> prints "5"

-- Lua is case-sensitive, so KnownVariable and KNownVAriable are different things:
print(KNownVAriable) --> prints "nil"

-- Deleting a variable:
KnownVariable = nil
print(KnownVariable) --> prints "nil"

We can also assign values to multiple variables in the same line of code like this:

 Lua:

a,b = 5, "Hello"
print(a) --> prints "5"
print(b) --> prints "Hello"

You see in the example above that assigning values to a variable doesn't require mentioning its type.

That is, because Lua-variables don't have a type. In fact, you can use the same variable to assign different types of data to it (although this programming style is a bit questionable):

 Lua:

a = 5 -- a holds an integer
print(a) --> prints "5"
a = "Hello" -- and now a string
print(a) --> prints "Hello"

Data Types

Variables don't have a type, but can store any value.

Variables don't have a type, but values have. After writing a = 5, a is not an integer-variable, but it just stores an integer. The value 5 however is undeniably an integer.

You can use the type-function to get the type of the value stored in any variable:

 Lua:

x = "Hello"
print(type(x)) --> prints "string", because x stores a string

The following types do exist in Lua:

TypeExampleInformation

string"Hello World"Call by value.

Refer to the "Strings and Math"-section.

number

integer

float1 (integer)

1.5 (float)Call by value.

number is simply the parent type of both float and integer.

Refer to the "Strings and Math"-section.

booleantrueCall by value.

true and false are written lower-case in Lua.

functionfunction() endCall by reference.

function is a data type in Lua, because functions can be saved in variables, passed as parameters and so on, just like any other value.

Refer to the "Functions" section.

table{1,2}Call by reference.

The main and single container type in Lua.

Refer to the "Table" section.

threadcoroutine.create()Call by reference.

A type specifically for coroutines. Coroutines are advanced stuff and will not be covered in this tutorial.

userdataPlayer(0)Call by reference.

userdata is the type of all objects that are not part of the Lua-language, but rather part of the program that runs Lua as a script language.

All warcraft-types are userdata, i.e. units, players, locations, etc.

 Lua:

print(type(Player(0))) --> prints "userdata"

Warcraft types still exist, just the type-function doesn't know them.

nilnilCall by value.

nil is both a type and a value (actually the value nil is the only single value of this type).

Warcraft Types

Warcraft-Types (like unit, player, ...) are not known to the Lua-engine, but only to Warcraft itself. Lua refers to them as 'userdata' (chunks of binary data that it doesn't understand).

We have seen above that the type-function always returns "userdata", when you input a Warcraft-object. Lua is designed to be a scripting language that can be incorporated into any program by its developers, so it's no surprise that Lua often deals with data types originating from the underlying program and even has its own dedicated type for it. For the Lua engine, units, destructables, players and all other handles are just binary data that it doesn't understand. Lua itself is not able to distinguish a unit from a player. Which should not be an issue, because you as a programmer still know what value you have stored in which variable, right? So Lua basically doesn't need to know. You must still pass the right values to the right Warcraft natives, but that's up to you.

However, there is still a way to identify the Warcraft-type for a piece of userdata due to the way the tostring-function got implemented in the Wc3-Lua-engine: If you have a unit u, tostring(u) will return "unit: <hexCode>", so you can pretty much extract the type by taking what stands before the colon.

Hence the following function can be used to identify a Warcraft-type:

 Lua:

---Will return the Warcraft-type for Warcraft handles and the Lua type otherwise.
---@param input any
---@return string
function Wc3Type(input)
 local typeString = type(input)
 if typeString == 'userdata' then
 typeString = tostring(input) --tostring returns the warcraft type plus a colon and some hashstuff.
 return typeString:sub(1, (typeString:find(":", nil, true) or 0) -1) --string.find returns nil, if the argument is not found, which would break string.sub. So we need to replace by 0.
 else
 return typeString
 end
end

Local Variables

Local variables can be declared via local <varname> at any point in your code. They last until the end of the current scope block (function end, loop end, etc.).

In contrast to global variables, local variables need to be declared.

The reason is simple: as discussed in the global variables section, every name that has not been declared before will be interpreted as a global variable, which doesn't leave space for undeclared locals.

Hence, a name becomes local only, if you explicitly declare it to be local.

To declare a local variable, simply write local <variableName>.

Afterwards, modify its value with the same syntax as with globals: <variableName> = <value>.

You can also combine the two statements to one: local <variableName> = <value>.

 Lua:

--Declaring a local variable
local x = 5
print(x) --> prints "5"
x = 10 -- refers to the local x, because x was declared to be local above
print(x) --> prints "10"

Local variables can be declared anywhere in your code. That means, unlike JASS, the declaration is not limited to the beginning of a function definition! The place of declaration however pretty much decides about where the local variable can be accessed.

As a rule of thumb, the scope of a local variable ends at the next end-keyword in your code (because that is used as a closing keyword for most of Lua's basic language elements like function definitions, if-then-else and loops).

Consider the following example:

 Lua:

if 1 == 1 then
 local a = 5
 print(a) --> prints "5"
end
print(a) --> prints "nil", because the scope of the local a has ended at the end-keyword above

The scope of local variables is a deep topic on its own and will be covered in more detail in the section about Functions, Closures and Scoping.

Strings

You can define a string with either single or double quotation marks.

Strings can be concatenated by using the ..-operator.

Strings in Lua can be defined by surrounding any word with either single or double quotation marks or even double square brackets. 'Hello', "Hello" and [[Hello]] all define the same string. The main advantage of having all options is that they escape each other:

print("Hello'World") and print([[Hello'World]]) will visually print Hello'World.

print('And then he said "You will soon learn Lua."') will visually print And then he said "You will soon learn Lua.".

Strings defined by double square brackets are called long strings. In contrast to strings defined by quotation marks, long strings support line breaks and do not interpret escape sequences (so you can span them across several lines, but you can't use \n to insert a linebreak).

 Lua:

--Multiline-string using square brackets. This would not work with quotation marks.
someString = [[Hello
World]]
--Same string with quotation marks. Escape sequences like \n would not work with square brackets:
sameString = "Hello\nWorld"

Strings can be concatenated by using the ..-operator.

print("Hello" .. " World") prints Hello World.

The +-operator can not be used for concatenation.

Concatenating a string with nil will throw an error (unlike JASS, where it resolved to the original string).

tostring

Lua offers the tostring-function for converting anything to a string. You can use it on numbers, booleans, tables, Warcraft objects and everything else.

 Lua:

s = tostring(3) --s now holds "3"

Using tostring on non-primitive objects (tables, functions, Wc3 objects, ...) will result in a string consisting of their type followed by a colon and a hexcode indicating their memory position:

 Lua:

t = {} --defines a table, which is a non-primitive type
print(tostring(t)) --> prints "table: 0x55ade2871d80" (the hexcode can be different for you)

The hexcode depends on memory allocation on the executing local machine, so the tostring-function will not be synchronous in a multiplayer game! For instance, sorting an array of tables by lexicographical order given by tostring will most likely lead to different results for different players and hence lead to a desync.

Some functions implicitly apply tostring. The most relevant example is the print-function. print(3) yields the same result as print(tostring(3)).

Escape Sequences

The backslash \ is an escape character in Lua strings. It will resolve to something different, depending on what comes after it.

You can read about them in the Lua reference manual, but I will also point out the ones that I consider most useful:

SequenceEffect

\nLinebreak

\tHorizontal Tab

\dddddd must be a sequence of 3 decimal digits. It resolves to the character with that numerical position in the ascii-character-set. For instance "H\101y" denotes the same string as "Hey", because 101 is the decimal ascii-index of the e-char.

\xHHSame as above, but HH must be a sequence of 2 hexadecimal digits. "Hell\x6f" denotes the same string as "Hello", because 6f is the hexadecimal ascii-index of the o-character.

\\Resolves to a single Backslash.

\"

\'Resolves to the specified quotation mark.

The String-Library

Lua offers its own native string-library, which is much more powerful than Warcraft's string natives.

It contains string formatting, substring operations, pattern matching, ascii integer conversion and a lot more.

The library is documented here, so I'm only showing you two example uses as reference.

 Lua:

s = "Hello World"

-- string.sub(s,i,j) returns the substring of s between the char positions i and j (including both). First char has index 1.
t = string.sub(s, 2, 4)
print(t) --> prints "ell"

--string.len(s) returns the length in characters of the specified string
print(string.len(s)) --> prints "11"
--you can also use the length-operator
print(#s) --> prints "11"

All String-library-functions taking char positions can also deal with negative positions. A negative position -x represents the x-th position from behind. print(string.sub("Hello",2,-1)) would visually print the substring from the second to the last character (first from behind), which is ello.

Lua also offers the "colon-notation" for strings, which allows you to call the library-function directly on the string instead of passing it as the first parameter. For instance, given a string s, you can write s:sub(i,j) instead of string.sub(s,i,j).

 Lua:

-- Using colon-notation on a variable:
s = "Hello"
print( s:sub(2, -1) ) --> prints "ello"

-- Raw strings need brackets to apply colon-notation:
print( ("Hello"):sub(2,-1) ) --prints "ello"
print( "Hello":sub(2,-1) ) --throws an error

Note that the string-library is based on ascii characters. In other words, it assumes that every character is exactly one byte long, which is not the case for non-ascii chars like the german ä. As per internal string representation of Warcraft 3, ä would be two bytes long and hence be treated as two characters.

You are probably opting for releasing your map in english language anyway, in which case all characters are part of the ascii-set, so it shouldn't really matter for you (except for processing player names).

The %-character

Prior to patch 2.0.4, using the % character in strings and even comments could crash your World Editor upon saving your map (WE bug).

If you are on an older version, don't use that character under any circumstances in your code.

Within strings, replace it by\x25instead, no matter the purpose (printing, string formatting, pattern matching).

From patch 2.0.4 on, % can safely be used in your code, but you need to write %% instead of % at all times. If you do, your Lua code becomes unable to run in other Lua interpreters, so using \x25 is still recommended.

The %-character is used for different things in Lua, like in format definitions, pattern matching or in the modulo operator (see below).

Unfortunately, Warcraft is internally also using % as an escape-character, so you need to write %% instead of % specifically in Warcraft, anywhere in your code.

Obviously, using %% will make your Lua code unable to run in other Lua interpreters (like the online one I've suggested you to practice with), because other Lua machines use single percents.

 Lua:

string.format("%a", 419) --this would be right under normal circumstances
 string.format("%%a", 419) --this is correct in Warcraft 3

Even worse, prior to patch 2.0.4 forgetting to put in the second % can crash the world editor. This is even true for percent-characters inside comments. Yes, you heard right. Try (or better don't try) to write a comment like --%s in your map and save it -> it will crash the WE.

That's very obviously terrible news and can make you lose unsaved progress, so I can only give you the advice of never using the %-character in your code, not even in comments.

Instead, you can use the percent-char's hex-notation \x25 within strings (and whatever you like within comments):

 Lua:

--DO
string.format("\x25a", 419)

--DONT
string.format("%%a", 419)

This doesn't only save you from editor crashes, but also makes your code similarly executable in both Warcraft 3 and other Lua machines (at least for pure Lua code not containing Warcraft natives).

Numbers

Integers and floats can be used in the same calculation without any conversion.

You are maybe used from JASS to differentiate between integers and reals, where using both types in the same calculation required a type-conversion via I2R or R2I.

Both types do exist in Lua (although reals are called float now), but good news, they have a common parent-type, which is just called number.

This fact makes them usable in the same formula without any conversion:

 Lua:

n = 1 -- this is an integer
m = 2.5 --this is a float
print(n + m) -- will print "3.5"

Most of the time though, the difference between integers and floats simply doesn't matter in Lua. Just think in numbers and Lua will handle all implicit integer-float-conversions behind the scenes.

Results from arithmetic calculations will only be integers, if all numbers involved were integers in the first place (with one exception: division always returns a float). Having one float participating will make the result a float: 1 + 2. resolves to a float, because 2. is a float.

Of course, this process does come with accuracy implications, but these normally don't matter for Wacraft 3 modding, so you might just not think about it.

Well, if a Warcraft native requires an integer, you should probably not pass a float, so I admit that you cannot completely forget about the types (the other way around is fine, though).

If you want to know, if a number is an integer or a float, Lua's math-library provides the math.type function for that purpose:

 Lua:

--The normal type-function always returns 'number' for numbers:
print(type(1)) --> prints "number"

--But we have the math.type-function to differentiate between integers and floats:
print(math.type(1)) --> prints "integer"
print(math.type(2.5)) --> prints "float"

Lua also provides the tonumber-function to convert a string to a number. This is basically the same as Warcraft's S2R.

 Lua:

print(tonumber("3") + 2) --> prints "5"

Mathematical Operators

Lua knows the following mathematical operators that work on all numbers:

OperatorNameComment

+Addition

-Subtraction

*Multiplication

/DivisionIn contrast to the other operators, the division of two integers will result in a float. For instance, 1 / 2 resolves to 0.5.

If you want to have proper integer division like you are used from JASS, use the floor division presented below.

//Floor divisionDivides two numbers and rounds the result down to the next integer.

E.g. 10//4 will resolve to 2 (similar to JASS integer division).

^Exponentiation2^3 is the same as 2*2*2.

%ModuloThe modulo-operator calculates the remainder after a division.

Again, Warcraft needs one additional %, which makes your calculation unable to run in a different Lua-interpreter:

 Lua:

10 % 3 --works in other Lua-interpeters, doesn't in Wc3
10 %% 3 --works in Wc3, doesn't in other Lua-interpreters

For patches prior to 2.0.4:

We have discussed in the %-section of the strings-chapter that using the %-character in your map is a no-go.

If you need the modulo-operation, stick to Warcraft's ModuloInteger and ModuloReal-natives instead.

If you need a modulo operation that both works in Wc3 and other Lua interpreters, you can use this function definition (for both integers and floats):

 Lua:

function math.mod(dividend, divisor)
 return dividend - (dividend // divisor) * divisor
end

Yes, the existing library function math.fmod also works, but please note that it returns negative results for negative dividends, which is usually not desired.

As you are used from JASS, exponentiation is resolved before multiplication and division, which in turn is resolved before addition and subtraction. You can use brackets to resolve a term in a different order.

Bitwise Operators

Bitwise operators are those operating on the binary representation of a number.

Unlike arithmetic operations, they only work on integers.

I'm not going into detail why bitwise operators can be useful, but they shall at least be mentioned:

OperatorName

&Bitwise AND

|Bitwise OR

~Bitwise exclusive OR

>>Logical Right Shift

<<Logical Left Shift

~ (unary)Logical NOT

The Math-library

Lua offers quite a powerful math-library. All functions plus their documentation can be found here. They contain all the standard stuff you would expect from a math library, like roundings, trigonometrics, log, abs, min, etc. pp.

We do only discuss the math.random()-function here, which is overloaded in Lua (i.e. it has multiple variants):

math.random() returns a random float between 0. and 1.

math.random(n) returns a random integer between 1 and n.

math.random(n,m) returns a random integer between n and m.
The random-function can safely be used in multiplayer maps. It is based on seeds and synced across players.

Coercions

We do have tostring and tonumber for explicit conversions, but Lua also allows for implicit conversions between these two types.

These conversions occur, if you use an arithmetical operator on a string or vice versa. Lua will automatically convert the input to the type needed to run the operator. This works in both directions.

 Lua:

print("2" + 3) --> prints "5". "2" was converted as a number, because + works on numbers.
print("2" .. 3 ) --> prints "23". 3 was converted to a string, because .. works on strings.

FourCC

In Lua, 'hfoo' defines a string and doesn't resolve to an object id (which would be an integer).

You need to pass FourCC('hfoo') to Warcraft natives requiring object ids.

When you come from JASS, you are probably used to write 'hfoo' to refer to the integer object Id for the human footman. This doesn't work in Lua, because single quotation marks define a string instead.

If you want to get an object Id, you have to use the FourCC-function, which is part of the Warcraft-implementation of Lua (not available in native Lua).

So, to create a human footman via the CreateUnit-native, you would need to write it like this:

 Lua:

--Creates a human footman for Player 1 at the center of the map, facing towards 0 degrees.
u = CreateUnit(Player(0), FourCC('hfoo'), 0, 0, 0)

This takes some time to get used to and will most likely produce some bugs, when you fall back to your JASS habits.

If you are using Debug Utils (you should), watch out for error messages like bad argument #x to <functionName> (integer expected), which will be thrown upon forgetting FourCC.

Syntax

The basic syntax of if-then-else in Lua is:

 Lua:

if <condition> then
 <doSomething>
elseif <condition> then
 <doSomething>
else
 <doSomething>
end

elseif and else are optional. elseif can be included as many times as you wish.

Relational Operators

SyntaxNameDescription

a==b

a~=bequal_to

 not_equal_toEquality and Inequality can compare any type, even nil. As nil is a unique static value, nil == nil always resolves to true. Hence, there is no is nil-operation in Lua and you will see below that checking for nil doesn't even require the equality-operator in most cases.

Remember that the equality-operator checks for identity on call-by-reference-objects, so - teasering tables - {} == {} would return false, because the two objects involved are different tables.

The JASS-equality-operator tolerated inaccuracies of like ~0.001 on numbers, probably to prevent GUI-users of having to deal with the inaccurate addition on the binary float format.

Lua doesn't have this tolerance, so 1 == 1 - 0.1 + 0.1 would resolve to true in JASS and to false in Lua (at least in the Wc3 implementation of Lua).

a<b

a<=b

a>b

a>=bless_than

 less_or_equal_to

 greater_than

 greater_or_equal_toThese operators apply the standard mathmatical order on numbers and the lexicographical order on strings.

The behaviour of all relational operators can be changed/customized by using metatables. This is an advanced topic and will be covered in a separate chapter.

Non-boolean conditions

You can use any value or statement as a Lua-condition, even non-booleans. The values false and nil will make the condition false, all other values will make it true.

Conditions are usually something that resolves to a boolean, like x > 0.

One big specialty about Lua is that it allows non-boolean conditions as well. In fact, you can write any statement inside an if-condition. The values false and nil count as false (hence will run the else-block) and everything else (including the number 0) counts as true (hence will run the then-block).

This fact has an important application: We can use any variable as a condition to simply check, whether it has been assigned a value before (that means, it's not nil):

 Lua:

--Consider the global variable x that is maybe nil, depending on what happened before.
--You want to pass x to the function f, but f doesn't support nil-values.
--In that case, simply do this:
if x then
 f(x)
end

Well, there's one exception: Variables holding booleans can not be checked this way (because false equals nil in terms of condition resolution).

Syntax

Logical operators are and, or and not.

Their syntax and behaviour on boolean values is similar to Jass:

 Lua:

print( true and false) --> prints "false"
print( true or false) --> prints "true"
print( not false) --> prints "true"

Short-circuit

and and or are short-circuit in Lua. They don't evaluate their second argument, if the result is already clear from the first.

For instance, you can use the statement if x ~= 0 and y/x > z to prevent a division-by-zero-error, while other languages might need a separate if-statement for x ~= 0.

false and <whatever> always resolves to false, and true or <whatever> always resolves to true. In these two cases, the second statement will not be evaluated.

This doesn't only save performance, but also has useful applications, like the error prevention above and several use cases presented further below.

Note that this behaviour is not a matter of course in the programming world. For instance, Warcraft triggers have trigger conditions, which are not short-circuit. All trigger conditions will always be checked regardless of how the prior conditions resolved.

Behaviour on non-boolean values

Logical Operators can be used on any value, not just booleans. This has many useful applications, as we will see below.

Similar to conditions, logical operators can be applied on non-boolean values as well, even on Warcraft objects.

As mentioned in the chapter about Conditions, nil and false will be treated as false, while any other value will be treated as true, including the number 0.

When applied to non-boolean values, and and or won't even resolve to a boolean, but instead return one of the input values. The following rules apply:

and resolves to the first argument, if that counts as false, and to the second argument otherwise.

or resolves to the first argument, if that counts as true and to the second argument otherwise.

not always resolves to a boolean: to true, if the argument is something that counts as false and vice versa.

 Lua:

--Let's look at some theoretical examples without much practical use:
print(1 and 2) --> prints "2", because 1 counts as true, so "and" resolves to the second argument.
print(nil and 2) --> prints "nil", because nil counts as false, so "and" resolves to the first argument.
print(1 or 2) --> prints "1", because 1 counts as true, so "or" resolves to the first argument.
print(nil or 2) --> prints "2", because nil counts as false, so "or" resolves to the second argument.
print(not 1) --> prints "false", because 1 counts as true.
print(not nil) --> prints "true", because nil counts as false.

--This would not even throw a "Divide by 0"-error, because the second statement is not evaluated:
print( nil and (1/0) ) --> This is due to the and-operator being short-circuit

We will later see that using logical operators on non-boolean values has a lot of useful applications, like optional function parameters and the ternary operator.

The behaviour presented above can be hard to keep in mind, so it might be the better approach to just memorize the use-cases presented further below.

Coalesce

x = a or b or c or d or ... assigns the first value to x that is neither nil nor false.

Due to or being short-circuit, later values will not even be evaluated.

Ignoring false for a sec, you can think of this as "give me the first non-nil value", which is really useful.

Some languages like SQL have a coalesce-function that takes any number of parameters and returns the first one that is not nil. Lua doesn't have a dedicated function for this purpose, but you can use the or-operator to simulate the same behaviour:

a or b will resolve to a, if a is neither nil nor false and to b otherwise. You can chain arbitrarily many elements this way.

The short-circuit nature of the or-operator ensures that values after the first true one don't even evaluate.

 Lua:

--this assigns the first value to x that is neither nil nor false:
x = a or b or c or d

--Example: Assign a new trigger to x in case it doesn't already hold one.
x = x or CreateTrigger()
--> If x holds a value (except false), it counts as true and the or-operator resolves to the first argument. The or-operator is short-circuit, so the CreateTrigger()-call is not evaluated. No trigger will be created. This knowledge is key!
--> Vice versa, if x doesn't currently hold a value, it's nil, hence the or-operator resolves to the second argument and x will be assigned a new trigger.
-- This trick is very often used in Lua-programming.

We will get to see one particular use case in the chapter about Optional function parameters.

Be careful, if any values involved are booleans, because the chained or-operator jumps over both nils and falses. Hence it can not be used to determine the first non-nil value, if false is potentially among those values. I personally keep forgetting about this fact and it can be really hard to find a bug resulting from this mistake.

If you want to find the first non-nil value out of potential booleans, better use the classical method:

 Lua:

if a ~= nil then
 x = a
elseif b ~= nil then
 x = b
elseif c ~= nil then
 x = c
else
 x = d
end

If exists

x = a and b will assign b to x, if a is neither nil nor false.

You can think of it as "if a exists, give me b", or in a similar fashion "if a holds, give me b".

This behaviour of the and-operator can be used to return a certain value only if another value exists:

x = a and b assigns b to x, if a exists (that means it is neither nil nor false). Again, it's easy to forget about the false, so stay careful.

 Lua:

--Example 1: You want to get the remaining health of the first hero of player 1, but you aren't sure, if player 1 has even trained a single hero
local player1Hero = GetPlayerHero(Player(0),1) --might be nil
local remainingHealth = player1Hero and GetHealth(player1Hero) --only assigns the remaining health to the variable, if the hero exists

--Example 2: You want to access a table entry t[x], but the table t might not exist (so accessing t[x] would throw an error)
--This never throws an error:
val = t and t[x] --assigns t[x] to val in case t exists. Otherwise, assigns nil to val.

Ternary operator

x = <condition> and y or z will assign y to x, if the condition is true, and z otherwise.

Sometimes we want to assign a value to a variable, depending on whether a given condition is true or false.

Using normal if-then-else can be very annoying to write that down:

 Lua:

a = <someNumber>
if a > 0 then
 b = 1
else
 b = -1
end

It would be great, if we could just write b = if a>0 then 1 else -1 end, but the if-then-else construct doesn't support this style.

Some languages do provide a dedicated operator for this: the Ternary operator.

Lua doesn't have such an operator, but we can still achieve the same shortcut by utilizing the special behaviour of logical operators:

 Lua:

--we assign the value 1 to b, if a is greater than 0. Otherwise we assign the value -1 to b.
b = a>0 and 1 or -1

It's a good training for your Lua-brain to figure out why this is working. However, you don't necessarily need to memorize the reasoning behind it, but rather keep the syntax in mind to make it work.

 The and-operator is resolved first. If a>0 yields true, then true and 1 resolves to 1. Afterwards, 1 or -1 resolves to 1 as well.

If on the other hand a>0 yields false, then false and 1 resolves to false and false or -1 resolves to -1.


Click to expand...

Again, be cautious! In general, the Lua Ternary operator condition and x or y only behaves the way presented, if x is neither false nor nil. As a rule of thumb, don't use the ternary operator on booleans.

While JASS only had the while-loop to offer, Lua grants many different loops to choose from.

While-Loop

Standard-loop-structure, which exists in literally every language on earth.

Repeats its body as long as the condition is met. That can be 0 times.

 Lua:

local i = 5
while i > 0 do
 print(i)
 i = i-1
end

Again, consider the option to use a non-boolean value as the condition, which is basically checking for whether that value is not nil:

 Lua:

--Let's say we want to loop through the array A, which has unknown size.
--As we will learn in the tables-chapter, non-assigned array-slots can still be read, but return nil.
--So we can loop through the array like this:
local i = 1
while A[i] do
 <doSomething>
 i = i+1
end

Repeat-Until-Loop

A repeat-until-loop repeats its body, until the specified condition is met.

Major differences to the while-loop are:

Meeting the condition will break the loop instead of continuing it.

The condition is checked after executing the body, so the body will be executed at least once.
Example:

 Lua:

local i = 1
repeat
 print(i)
 i = i+1
until i == 10

Numerical For-Loop

Repeats its body for each value between the specified startValue and endValue.

You can choose the name of the loop variable.

 Lua:

for var = startValue, endValue do
 <doSomething>
end

--Example:
for i = 1, 3 do
 print(i) -- will print "1", "2" and "3"
end

You can also choose a different step size than 1, which will make the loop go through each value that you get by successively adding it to the startValue until reaching the endValue.

 Lua:

for var = startValue, endValue, stepSize do
 <doSomething>
end

--Example: (stepSize can also be negative)
for i = 2, -4, -2 do
 print(i) -- will print "2", "0", "-2", "-4"
end

Note that all three parameters will only be calculated once, before the loop starts. That allows you to include measures that might change within the loop:

 Lua:

n = 5
for i = 1, n do --loops 5 times, although n is changed in the body
 n = n + i
end
print(n) --> prints "20"

The Break-keyword

break is a keyword that can only be used inside a loop and will immediately cancel that loop.

 Lua:

--Print all integers between 1 and 100, but have a 1/3-chance after each print to stop early.
local i = 1
while i < 100 do
 print(i)
 if math.random() < 1./3. then
 break
 end
 i = i+1
end

Table-Loops

There are two constructs specifically to loop through tables: pairs and ipairs.

Both are very useful. We will be discussing those in the chapter about tables.

Generic For-Loop

Generic For-Loops are customized loops you can define for your own later use.

Sometimes, the above loop-methods are still not convenient enough.

Consider the case, where you have programmed a data structure (let's say a Linked List) and you want to loop through it.

That data structure can be very complicated on its own, and the way of looping through it might be something that very specifically depends on it. Taking the Linked List example, we would probably start with some Linked List node, fetch node.next repeatedly, until we get node.next == nil. But it might as well be totally different (maybe your Linked List is circular?).

Remembering how exactly to loop through your data structure can be tedious.

Wouldn't it be great, if we could just use the following syntax?:

 Lua:

for element in datastructure do
 doSomething(element)
end

Well, that's perfectly possible in Lua and is called a Generic For-Loop.

It's basically a custom loop that you can define to receive a simpler syntax (or just make your structure loopable without remembering the internals).

Defining custom loops is advanced stuff and will be covered in the advanced chapters.

Tables

Tables are the single data structure in Lua. There are no dedicated arrays, hashtables or sets, but just Tables, which are basically everything at once.

From a technical perspective, a Table is a container that holds any number of (key,value)-pairs, where both key and value can have any type except nil. Keys are unique in the sense that the same key can't be included twice in the same table.

Lua-Tables can contain keys and values of any type (except nil).

This even includes Warcraft Objects.

Basic Syntax

 Lua:

--Creating an empty table
t = {}

--Adding a key-value-pair to a table
--Let a, b and c be any variable or value
t[a] = b --The table now contains the pair (a,b)
t[c] = b --The table now contains the pairs (a,b) and (c,b)

--Assigning a new value to an existing key
t[a] = c --The table now contains the pairs (a,c) and (c,b)

--Removing a key-value-pair from a table
t[a] = nil --The table now contains the pair (c,b)

--Table-Constructor with existing elements:
t = {[a] = b, [c] = b} --creates a table with the existing pairs (a,b) and (c,b)

Dot Syntax

For string keys, Lua offers the dot-notation as syntactic shugar:

t.s is a shortcut for t["s"].

Likewise, {s = x} is a shortcut for {["s"] = x}.

 Lua:

--Example
Arthas = {}
Arthas.isLichKing = true --same as Arthas["isLichKing"] = true
Arthas.swordName = "Frostmourne"
--or in short:
Arthas = {isLichKing = true, swordName = "Frostmourne"} --same as Arthas = {["isLichKing"] = true, ["swordName"] = "Frostmourne"}

You will notice that we've already seen the dot-syntax, when we discussed Lua library functions like math.sin(x).

It's not obvious on first glance, but in fact the exact same thing: The math-library is nothing less than a global Lua-table storing some functions. You can even call print(type(math)) and it will print 'table'.

math["sin"] stores the sin-function, so we can write math.sin instead.

Clearly, math["sin"](x) looks much weirder than math.sin(x), but it's technically the same.

Nil Elements

Assigning t[a] = nil for any key a will remove the key-value-pair from the table.t[b] will return nil for any unused key b.

In particular, t[nil] always returns nil.

The fact that non-assigned keys return nil is very similar to global variables (which is no coincidence, because global variables are actually stored in a global table called _G, so they naturally adapt to this rule).

Arrays

Lua-Arrays are just tables using positive integer keys.

They are always dynamic and start at index 1.

{a,b,c} is a shortcut for {[1] = a, [2] = b, [3] = c}.

As mentioned, there is no dedicated array datastructure in Lua. Instead, we call any table an array, if it's only using positive integer keys.

 Lua:

--Creating an "array":
t = {}
t[1] = "a"
t[2] = "b"

Tables and consequently arrays are always dynamic in Lua. Size will grow as needed and you can't declare them with a fixed size anyway.

In Lua, arrays are supposed to start at index 1. Of course you are free to start at 0 (or any other number) instead, but some features of the language only work on arrays starting at index 1, as we will see below.

This Lua best practice might be difficult to adapt to, especially if you come from JASS, but it's definitely worth it.

Again, Lua offers some syntactic shugar to create arrays:

{"a","b","c"} is a shortcut for {[1] = "a", [2] = "b", [3] = "c"}.

This array constructor style can be combined with the others we already know: {"a", bla = true, "b", [true] = 1.5} for instance is equivalent to {[1] = "a", ["bla"] = true, [2] = "b", [true] = 1.5}.

Like any table, arrays ca not store nil-values, so setting t[index] = nil will completely remove the value at the given index from the array, potentially leaving a hole in between remaining integer indices.

 Lua:

t = {"a","b","c"} --create an array with 3 elements
t[2] = nil --now the array only consists of indices 1 and 3, leaving a hole at index 2.

Arrays without holes are called sequences.

The #-operator

Given a sequence t, #t returns the number of elements in t. As Lua-arrays start at index 1, this is identical to the maximum index in use.

The #-operator can potentially lead to wrong results on arrays with holes, so only use it on sequences!

If you need the number of elements for any array (or general table), you are responsible for counting the number of elements yourself.

The #-operator has undefined behaviour on non-sequences and might even yield different results for different players. Only use it on sequences to prevent desyncs.

Looping over Sequences

Lua offers a convenient way to loop over sequences: The ipairs-iterator.

 Lua:

S = {"a", "b", "c"}

for key, value in ipairs(S) do
 print(key, value) --> prints "1 a", "2 b" and "3 c"
end

ipairs loops through integer-keys, starting at index 1, counting upwards (1 at a time) and stopping at the first nil-value (which in the above example would be key 4).

That means, if the array provided has holes, ipairs will ignore everything after the first hole. It will also ignore non-integer keys.

Note that we have two control variables here: key and value. The first stores the current loop position (so 1 initially and counting upwards), while the second stores the value at the current index (i.e. value is just S[key]).

Both control variables can be freely named. If you don't need one of the control variable inside the loop, it is best practice to use _ instead of a name.

 Lua:

local players = {Player(0), Player(1), Player(2), Player(3)}
for _, player in ipairs(players) do
 print(GetPlayerName(player))
end

Of course, classic methods can also be used to loop over sequences. ipairs is just the most convenient option in most cases.

To summarize, the following methods can be used to loop over a sequence t:

while-loopnumerical for-loopipairs

 Lua:

local i = 1
while t[i] do
 print(i, t[i])
 i = i+1
end

 Lua:

for i = 1, #t do
 print(i, t[i])
end

 Lua:

for k, v in ipairs(t) do
 print(k, v)
end

If you have an array with holes, your best option is to know the maximum index in use n and conduct a numerical for-loop via for i = 1,n do.

It is possible to create multi-dimensional arrays in Lua that support the t[index1][index2] syntax.

Please refer to the Advanced Concepts- Multidimensional tables tab for further information.

The Table-Library

Lua's incorporated table-library provides additional tools for working with sequences.

A good documentation can be found here.

I'm just going to cite the most honorable mentions:

table.insert(list, value)

or

table.insert(list, pos, value)Inserts element value at position pos in list, shifting the elements list[pos], list[pos+1], ···, list[#list] upwards. The default value for pos is #list+1, so that a call table.insert(l,x) inserts x at the end of list l.

table.remove(list)

or

table.remove(list, pos)Removes from list the element at position pos, returning the value of the removed element. When pos is an integer between 1 and #list, it shifts down the elements list[pos+1], list[pos+2], ···, list[#list] and erases element list[#list]; The index pos can also be 0 when #list is 0, or #list + 1; in those cases, the function erases the element list[pos].

The default value for pos is #list, so that a call table.remove(l) removes the last element of list l.

table.sort(list)

or

table.sort(list, comp)Sorts list elements in a given order, in-place, from list[1] to list[#list]. If comp is given, then it must be a function that receives two list elements and returns true when the first element must come before the second in the final order. If comp is not given, then the standard <-relation is used instead.

General Tables

Lua-Tables are very convenient for storing data related to Warcraft objects like players and units, because you can use those objects as keys! Given a table t and a unit u, you can and should save data to t[u] instead of t[GetHandleId(u)].

One major advantage of Lua Tables is that they can use any object as keys and values (except nil, of course).

This makes them way more convenient than JASS hashtables, which required all keys to be integers (thus requiring us to use GetHandleId, StringHash, etc. for integer conversion).

There is absolutely no reason to use Wc3 hashtables in Lua, because Lua tables are strictly superior.

For instance, we might define the table of all player colors, using the player as a key and the color hexcode as a value:

 Lua:

PLAYER_COLORS = {
 [Player(0)] = "FF0303"
 , [Player(1)] = "0042FF"
 , [Player(2)] = "1CE6B9"
 , [Player(3)] = "540081"
}

This would allow us to retreive a player's color from a table without using its player number or handle id:

 Lua:

--might use this in a trigger action:
local p = GetTriggerPlayer()
print("|cff" .. PLAYER_COLORS[p] .. GetPlayerName(p) .. "|r is the triggering player")

Note how easy this is. We don't need to work with restricted or overly complicated data structures anymore.

There is also no limit of Lua tables in your map, in contrast to Wc3 hashtables.

Take care of memory leaks, when using Warcraft objects as keys or values:

(Key,value)-pairs containing an Wc3 object will remain in the table even after the object has been destroyed or removed.

A famous example is when you use units as keys to store information about them - at some point the unit might die, decay and leave the game, but the (unit,value)-pair still stays in the table.

To get rid of these unnecessary pairs, you need to either manually remove them (preferrably when you destroy the object in question) or let them go automatically by making the table use weak keys (see Advanced Concepts - Memory Leaks for further information).

To be fair, this fact also holds for good old Wc3 hashtables: a tuple (GetHandleId(object),otherKey,value) stays in the hashtable even after the object is being destroyed.

Only unit groups were smart enough to remove units that left the game.

Looping over a general table

You can loop over all (key,value)-pairs of any table by using the pairs-function. Remember that nil-elements practically don't exist in a Lua-table, so attempting to loop over {"a", nil, "b"} will include the pairs (1,"a") and (3,"b"), but not (2, nil).

pairs is another example for a generic for-loop, so the syntax is similar to ipairs:

 Lua:

--Using PLAYER_COLORS as defined above
for player, colorCode in pairs(PLAYER_COLORS) do
 print("Player " .. GetPlayerName(player) .. " has color code " .. colorCode)
end

Note that table keys in general don't have an order, so the pairs-loop does not guarantee to loop in any particular order.

That also means, the loop-order can be different this time and next time.

Even worse, the loop order of pairs is not even guaranteed to be the same for all players in a multiplayer game.

That means, changing game state inside such a loop can lead to a desync.

Still, using pairs for looping over a general table is very convenient and I personally need it really often during coding. This is why I created the SyncedTable library, which allows for creating tables with a multiplayer-syncronized pairs-iteration. See Resource Spotlight below for further details.

As already mentioned in the last section, table-pairs including Warcraft objects stay in a table even after the objects have been destroyed or removed. That means, looping over such a table can potentially include references to objects that don't exist anymore.

This issue even holds for tables using weak keys (or values), because garbage collection doesn't happen instantly (don't worry, if you don't yet understand this statement).

Please refer to the chapter Advanced concepts - Memory Leaks to learn how to prevent this issue.

Resource Spotlight: SyncedTable

This library allows you to create tables with multiplayer-synchronous pairs-iteration.

It does not add network-synchronisation (preventing network-delay), but just customizes the pairs-iteration in a way that it iterates elements in the order of their insertion, which makes it deterministic and thus synchronous.

 Lua:

--Example: Create a multiplayer-synced table holding each player's playercolor.
PLAYER_COLORS = SyncedTable.create()

Once created, SyncedTables act like any other table (except that it already has a metatable, which you shouldn't change).

 Lua:

--Assign values to the SyncedTable we created above
PLAYER_COLORS[Player(0)] = "FF0303"
PLAYER_COLORS[Player(1)] = "0042FF"
PLAYER_COLORS[Player(2)] = "1CE6B9"
PLAYER_COLORS[Player(3)] = "540081"

--You can now safely loop over PLAYER_COLORS via pairs
for player, playercolor in pairs(PLAYER_COLORS) do
 print("|cff" .. playercolor .. GetPlayerName(player) .. "|r") --prints every player name in its own color
 [you can change gamestate here, because the loop is synced]
end

Next - Check if table is empty

You can use next(T) == nil to check, if T is empty.

This works on arrays and general tables.

next is a so called iterator-function. For any general table T and key x used in T, next(T,x) returns the key that a pairs(T)-iteration would iterate afterx (can be nil, if x was the last key in order).

In particular, next(T, nil) (or just next(T)) will return the first element of the pairs-iteration. As such, you can use next(T) == nil to check, whether a table is empty.

next(T,x) is asynchronous (can have different results for different players in a multiplayer-game) and exactly the reason for pairs being async as well.

The is-empty-checknext(T) == nil however is safe to use even in a multiplayer game.

__jarray - Tables with default values

Lua Warcraft 3 comes with a hidden native called __jarray (two underscores up front).

It creates an empty table with a default value to be returned upon accessing nil-keys.

 Lua:

--__jarray is Wc3-specific, so executing this in external Lua interpreters is not possible.
T = __jarray(5) --creates an empty table where accessing nil-keys returns the specified value instead of nil.
T[1] = "bla"
print(T[1], T[2]) --prints "bla" and "5".

Function Basic Syntax

You define and execute Lua-functions with the following syntax:

 Lua:

--Function definition
function functionname(parameters)
 <doSomething>
 return <something> --optional
end

--Function execution:
functionname(arguments) --executing a function in Lua doesn't require the "call"-keyword

Example:

 Lua:

function foo(x,y)
 return (x+y)/2.
end

function bar(x,y)
 print(foo(x,y))
end

bar(10,20) --> prints "15"

Parameter Types

Parameters of Lua-functions don't have a type.

In contrast to JASS, Lua-functions don't require you to specify the types of input- and output-parameters. In fact you can't, even if you wanted to. This is similar to variables, which also don't allow for specifying a data type.

This fact unfortunately leads to substantial risk of producing bugs. As Lua-functions don't know the type of their parameters, the Lua-syntax check will consequently not warn you about passing a wrong value.

If you happen to pass a wrong value that leads to the function not being executable, Lua will throw an error at runtime instead:

 Lua:

function foo(x)
 return x/2.
end

y = foo("Hello") -- throws an "Attempt to perform arithmetics on a string value" error. The code will break at this point.
print(y) --will not execute anymore, because of the error above.

To make things even worse, Warcraft 3 by default will not inform you about Lua-runtime errors ingame. If you have a game-breaking bug as shown above, the code will break "silently", i.e. ingame it might look like "nothing happened".

But don't worry, there are solutions.

Please refer to the thread Lua Debug Utils to see a collection of debugging tools specifically for Warcraft 3 Lua, including automatic error messages for situations like the above.

The biggest learning from Debug Utils certainly is that you can and should use Emmy Annotation to add parameter types to your Lua functions. Annotations are just comments that help your developing environment providing type-checks that the normal Lua-syntax-check would not be able to.

Local Variables in Functions

Local variables can be declared with local <varname>anywhere in the function body.

In contrast to JASS, it's not necessary to declare local variables on the very top of Lua functions. You can declare them anywhere really and they will be accessible from the point of their declaration until the end of their scope (which is usually the end of the function definition, but we will discuss this in more detail in the chapter about function scope).

 Lua:

--This function prints the square-root of the specified number x., if x is non-negative, and an error-message otherwise.
function foo(x)
 if x >= 0 then
 --we only need local y inside the then-block. We declare it here to improve performance for the else-case.
 local y = math.sqrt(x)
 print(y)
 else
 print("Error: You shall not specify a negative number!")
 end
end

All input parameters are considered local variables, too. It's perfectly fine to reassign those inside the function and the reassignment will only happen within the function, not outside of it.

Keep in mind that manipulating call-by-reference objects (i.e. changing table entries of an input table) will in fact apply outside the function (because you are actually editing the referenced object itself instead of just reassigning the variable).

 Lua:

--Call-by-value example:
function foo(x)
 x = x+1 --x is considered a local variable, because it's an input parameter
 print(x)
end

x = 5
foo(x) -- prints "6"
print(x) -- prints "5", because the outer x was not affected by the reassignment within foo.

--Call-by-reference example:
function bar(someTable)
 someTable[1] = 10 --manipulating the object itself
 print(someTable[1])
end

someTable = {5}
bar(someTable) -- prints "10"
print(someTable[1]) --also prints "10", because the first entry of the input-table has been altered within bar.

Even for tables, just reassigning input-variables will never manipulate their stored values:

 Lua:

function baz(t)
 t = {1,2,3} -- we are reassigning the local variable t instead of editing the object behind it!
 print(t[1])
end

someTable = {4,5,6}
baz(someTable) -- prints "1"
print(someTable[1]) -- prints "4". The input variable t in baz was not altered, but just reassigned.

Global Variables in Functions

Global variables can both be read and assigned within functions, although the latter might be considered poor programming style:

 Lua:

function foo(x)
 y = x*x + 10 --we assign the square of x plus 10 to the global variable y
 print(y)
end

As a general rule of thumb, always(!) use local variables whenever possible to avoid name clashes.

Order of Functions in your Map Script

The order of functions in your map script does not matter.

Functions on the top can call functions below them and vice versa.

We are used from JASS that functions need to be sorted by dependence: If a function g used a function f, then g had to be located belowf.

That restriction sometimes made simple tasks a pain. Refacturing your code, defining callbacks or using recursion always came with organizing overhead and the resulting code for sure wasn't always optimized for human reading.

Fortunately, Lua doesn't come with this restriction. Any function in your code can reference any other function, no matter their order in the map script. At least if they can "see" each other from a scope perspective (you will understand that later), which would definitely hold after writing every function to global scope (which I don't recommend, but many people do it).

This fact doesn't only allow you to structure your code in the way you find best to read, but also enables options that simply weren't possible in JASS, like recursion across more than one function (e.g. two functions referencing each other).

Return Value(s)

As we've seen above already, we can specify a function return-value by using the return-keyword followed by the value you want to return.

 Lua:

function bar(x)
 return x+1
end

Function execution will end at the return-keyword, even if you don't specify a return-parameter.

You can use that fact to prevent a later part of the function from running:

 Lua:

---@param x number
function PrintIfPositive(x)
 if x <= 0 then
 return --stops function execution, if the input-parameter is not positive
 end
 print(x)
end

A big specialty about Lua in contrast to JASS is that Lua even allows for multiple return values from a single function.

The syntax is easy: Just write return followed by a comma-separated list of values.

 Lua:

---Calculates the center between two points.
---Returns both x- and the y-coordinate separately.
function GetCenterPoint(x1,y1,x2,y2)
 return (x1+x2)/2. , (y1+y2)/2.
end

Question remains, how do we continue to process these multiple return values? Can we save them into variables or a table or pass them directly to another function?

All of that is possible and rather straightforward, with some tricky nuances.

 Lua:

local x1, y1 = 0., 0.
local x2, y2 = 512., 1024.

--Save return values to different variables:
local centerX, centerY = GetCenterPoint(x1,y1,x2,y2) --centerX will be 256., centerY will be 512.

--Pass multiple return values to another function
print(GetCenterPoint(x1,y1,x2,y2)) -- prints "256" and "512".

--Save all return values to a table
local tab = {GetCenterPoint(x1,y1,x2,y2)} -- this is basically {256., 512.}, i.e. tab[1] = 256. and tab[2] = 512.

The following nuances apply:

 Lua:

local function exampleFunc()
 return 1,2,3
end

--If you specify less variables than there are return values, all excess values will be discarded.
x,y = exampleFunc() -- x = 1, y = 2 - the last return value (3) is discarded

--If you specify more variables than there are return values, all excess variables will be nil
a,b,c,d = exampleFunc() -- a = 1, b = 2, c = 3, d = nil

--If you specify other values AFTER the multi-return function call, only the function's first return value will be kept.
tab = {exampleFunc(), 4} -- will be the array {1,4}
a, b, c, d = exampleFunc(), 4 -- a = 1, b = 4, c = nil, d = nil
print(exampleFunc(), 4) -- will print "1" and "4"

--Including the return-value into some calculation will only keep the first one as well:
print(10 + exampleFunc()) -- will print "11"

--Having the multi-return-function-call as your last statement however will keep all return values
tab = {4, exampleFunc()} -- will be the array {4,1,2,3}
d, a, b, c = 4, exampleFunc() -- a = 1, b = 2, c = 3, d = 4
print(4, exampleFunc()) -- will print all values "4", "1", "2", "3"

Functions as First-class Values

In Lua, functions are first-class-values. They can be stored in variables and passed as arguments to other functions, like any other data type.

In fact, the function definition of foo from the beginning of this chapter can also be written like this:

 Lua:

--Syntax we learned so far:
function foo(x)
 return x/2.
end

--Can also be written like this:
foo = function(x)
 return x/2.
end

--Both ways also work with tables:
t = {}
--Style 1:
function t.foo(x)
 return x/2.
end
--Style 2 (identical):
t.foo = function(x)
 return x/2.
end

This means that technically, foo is not even the function name (in fact, functions cannot have a name), but rather the name of the variable storing that function. This is true no matter which of the two syntax styles above you use (the first is just a shortcut for the second). Functions are just values, like booleans, integers, strings etc.

To emphasize this even more, consider the following example:

 Lua:

a = print -- "print" is just the name of the global variable storing the print-function. We can set the same value to variable "a" (call by reference).
a("Hello World") --> prints "Hello World"
print = "This is a string now" -- we assign another value to the print-variable. The print-function is still stored in "a".
a(print) --> prints "This is a string now"

--Can we still use the "print"-variable as if it was a function?
print("Hello World") --> No. This throws an error "Attempt to call a string value", because "print" currently stores a string and thus can't be called with parameters anymore.

Overwriting Warcraft natives

Last section's example looks like terrible programming style, but it's the main reason for why Warcraft natives can be overwritten so easily in Lua. Things like CreateUnit, GetPlayerName and TriggerSleepAction are just global variables that can be reassigned.

 Lua:

--Example: Overwriting the CreateUnit native.
--The below code is part of the Lua Root (because it's not part of a function), so it will be directly executed during early loading screen (before any units can be created). In other words, we are sure to overwrite the native before its first use.
do
 --First we save the original function into a local variable, which has its scope limited to the outer "do ... end" block. That means "oldCreateUnit" will not be accessible from anywhere else.
 local oldCreateUnit = CreateUnit
 --Second we reassign the global "CreateUnit"-variable to a new function.
 --The new version below doesn't only create a unit, but additionally checks, whether the given coords are within the playable map:
 CreateUnit = function(player, unitCode, x,y, face)
 if RectContainsCoords(GetPlayableMapRect(), x, y) then
 return oldCreateUnit(player, unitCode, x, y, face)
 else
 print("|cffff0000Error: Attempt to create a unit outside of playable map area|r")
 end
 end
end

As functions are first-class-values, we can even store them in local variables and even inside other functions:

 Lua:

---Creates a trigger that prints "<unitname> is in danger", whenever the specified unit is attacked.
---@param unit whichUnit
---@return unit
function CreateAttackWarning(whichUnit)
 local trigger = CreateTrigger()
 --define a local function that we only use for the trigger just created
 local function actionFunc()
 print(GetUnitName(whichUnit) .. " is in danger")
 end
 TriggerRegisterUnitEvent( trigger, whichUnit, EVENT_UNIT_ATTACKED )
 TriggerAddAction(trigger, actionFunc)
 return trigger
end

Yes, we could have achieved the same result by using a global trigger action utilizing GetTriggerUnit(), but we want an educative example, right?

Callbacks

Functions can easily be passed to and executed within other functions.

A callback is a function that you pass to another function to execute it there. The TimerStart and TriggerAddAction natives are two of the few available examples in Warcraft that use callbacks. JASS's support for custom callbacks is very limited and based on workarounds using the mentioned natives, often resulting in really messy code.

Lua on the other hand supports callbacks with ease. Because functions are first-class-values, you can simply pass them as parameters to other functions without any particular preparation.

 Lua:

--Consider the following function that adds 1 to the input parameter
function f(x)
 return x+1
end

--Further consider this function executing another function with the specified argument.
function ExecuteAndPrint(func, x)
 print(func(x)) --Quite obviously, this will throw an error at runtime (not compile time), if you passed a non-function-value for "func"
end

--Pass f to ExecuteAndPrint
ExecuteAndPrint(f, 5) --prints "6"

Callbacks are incredibly useful. They allow for customizing processes on a very high level.

Consider a sorting algorithm that shall be able to sort an array in any user-defined order. Lua allows for creating that sorting algorithm as a function that takes both the array and the user-defined order as parameters. The user-defined order would be represented by a function taking two array elements x and y and returning a boolean saying whether x shall be positioned before or after y in the final sorted array.

 Lua:

--We are using the inbuilt library function table.sort, which takes exactly an array and a comparison function, as described above

--Let's sort this array:
T = {Player(1), Player(5), Player(0)}
--Help function to sort via player-number:
function sortByPlayernumber(p1, p2)
 return GetPlayerId(p1) < GetPlayerId(p2) --returns true, if the first playernumber is smaller than the second
end
--actual sorting:
table.sort(T, sortByPlayernumber)

Anonymous Functions

You might have already concluded after reading the previous sections that all functions are anonymous in Lua, because they cannot have a name. And that is true.

In this section though, we talk about functions anonymous in the sense that they are not stored in any variable, but directly passed as a parameter into another function.

As the syntax foo = function(x) return x/2. end suggests, function(x) return x/2. end is the actual function definition. function() end is basically a function constructor, similar to {} being a table constructor. The resulting function is a value and can be used in every context, where writing a value makes sense, including passing it as a parameter to another function:

 Lua:

--Consider this function, which executes the passed function with the passed parameter
function ExecuteAndPrint(otherFunc, valueToPass)
 print( otherFunc(valueToPass) )
end

--Now we can pass an anonymous function.
ExecuteAndPrint(function(x) return x/2. end, 10.) -- will print "5".

Anonymous functions often come in handy, when you aim to use Warcraft natives requiring callbacks on the fly. TimerStart for instance takes a function to execute, but doesn't allow to specify arguments for that function. Hence, even a simple example like printing the value after 5 seconds would be rather tricky to implement in JASS.

Lua solution is simple:

 Lua:

function PrintAfter5Seconds(msg)
 local t = CreateTimer()

 --TimerStart requires a function without parameters, which we create on the fly.
 --It will both destroy the timer and print the message.
 TimerStart( t, 5., false, function()
 DestroyTimer(t)
 print(msg)
 end )
end

Optional Parameters

You can implicitly make any function parameter optional by assigning a value to it in the function body in case it doesn't already hold one.

 Lua:

--Example: Optional parameter with default value 5.
function InputIsOptional(x)
 x = x or 5 --assigns 5 to x, if x is nil (has not been passed). Don't do this for booleans!
 <doStuff>
end

Lua doesn't have a dedicated syntax to specify optional function parameters, but it is very easy to implicitely allow for them.

In general, Lua-functions can be called with any number of arguments without throwing an error. Non-passed parameters will just be nil.

 Lua:

function print3Values(x,y,z)
 print(x,y,z)
end

print3Values(1,2) --will print "1", "2" and "nil"

At this point you could argue that every function parameter is optional, but for the complete deal, we still need a way to specify default values. nil is obviously not always a sensible value.

To achieve this, we remember one thing we have learned in the chapter about Logical Operators:

Theor-operator can be used with any data type. It resolves to the first argument, if it is neither falsenornil, and to the second argument otherwise.

That means, if x is a non-boolean parameter, then x = x or <defaultValue> will assign the default value to x, if x has not been passed.

 Lua:

--This function prints the specified message in the specified color. Color is an optional parameter.
--Color must be passed as standard ARGB hex-code, like "ffffcc00".
function PrintInColor(message, color)
 color = color or "ffff0000" --default value is "red"
 print("|c" .. color .. message .. "|r")
end

PrintInColor("Hello World") --prints "Hello World" in red letters, because red was default and the color has not been passed.
PrintInColor("Hello World", "ff00ff00") --prints "Hello World" in green letters.

Calling a function without specifying some particular optional parameter can be tricky, if that optional parameter is not the last one in order. In such a case, you explicitely need to pass nil to the preceeding parameters to prevent misinterpretation:

 Lua:

--Adds up x and y and afterwards multiplies by z. Prints the result.
--y is optional. Default: 2
function f(x,y,z)
 y = y or 2
 local result = (x+y) * z
 print(result)
end

--We want to pass x = 3, z = 10, but nothing for y. But y is the second parameter, not the last. What shall we do?
f(3, 10) --this doesn't result in what we wanted. f will think that you have passed x and y instead of x and z. It even throws an error, because z remains nil and we can't multiply with that.
f(3, nil, 10) --this does work as intended. But it's definitely not convenient to have optional parameters in the middle...

To sum it up, it's best practice to have optional parameters come after the mandatory ones in the list of function parameters.

If you do have multiple optional parameters in a single function and only want to pass one of the later parameters (but not the prior ones), then you can't avoid explicitely passing nil-values for the prior.

The x = x or <value> syntax presented above should only be used for non-boolean parameters.

The reason is, false is treated the same as nil, so if you explicitely pass false for x, it would look like if you passed nothing, applying the default value instead.

For boolean parameters, better use regular if then else. Or even better, use the fact that nil counts as false to your advantage: using false as a default doesn't require any code:

 Lua:

--DON'T
function Bad_DefaultTrue(x)
 x = x or true --buggy: this always yields true, even if you explicitely passed false.
 if x then
 <doStuff>
 end
end

--DO
function Good_DefaultTrue(x)
 if x == nil then --this works, because we explicitely ask for nil
 x = true
 end
 if x then
 <doStuff>
 end
end

--EVEN BETTER, USE DEFAULT FALSE
function Good_DefaultFalse(x)
 if x then --you don't need to declare a default, because nil counts as false anyway
 <doStuff>
 end
end

Recursion

Recursion is also easy to conduct in Lua, but we need to learn more about variable scope before.

Please refer to the Scope and Closures tab for further information about recursive function calls.

The ...-operator

Lua allows for functions that take any number of arguments.

One example is the print-function, which you can call with as many arguments as you like (and all of them will be printed): print("a") is just as fine as print("a", "b", "c").

You can declare similar functions using the ...-operator.

Please refer to the Advanced Concepts tab for further information about the ...-operator.

This chapter can be considered further reading. Skip it, until you need it.

Scope of Local Variables

Local variables are limited to the place, where they've been declared. This fact makes them resistent against name clashes, which is why they should be preferred over global variables whenever possible. But it also requires us to understand, where the scope of a local variable starts and ends.

To put it simply, a local variable starts to exist with its declaration and ceases to exist at the end of its scope block. Scope blocks are:

function definitions

if-then-else (then and else are separate blocks)

loops

do ... end constructs (which we will get to know further below)
A local variable declared inside one of these blocks is only visible inside the same block.

 Lua:

if 1 == 1 then
 local v = 5
 print(v) --> prints "5"
else --end of scope for local v
 print(v) --> prints "nil" (refers to global v)
end
print(v) --> prints "nil" (refers to global v)

Of course, scope blocks can be nested and the visibility of local variables applies to all inner blocks.

 Lua:

-- outer block
if 1 == 1 then
 local v = 5
 -- inner block
 for i = 1,2 do
 local w = 10
 print(v, w) --> prints "5" and "10". v is in scope, because the then-block has not yet ended.
 end --end of scope for local w
 print(v, w) --> prints "5" and "nil". w is out of scope, because it was limited to its scope block, the for-loop.
end --end of scope for local v

We can also explicitely declare a scope block by using do ... end. This method is often used in the Lua root, where we don't always have an existing scope block to work with.

 Lua:

do
 local v = 5

 -- v is local in the do... end block, but extends to inner scope blocks like this function definition.
 function foo()
 print(v) --prints "5"
 end
end -- end of scope for v

--bar on the other hand refers to the global v
function bar()
 print(v)
end

foo() --prints "5"
bar() --prints "nil"

If you declare identically named local variables on different scope levels, all operations will prefer the innermost one:

 Lua:

do
 local v = 5
 for i = 1,2 do
 print(v) --> prints "5"
 local v --> this v has higher priority than the outer v, but the outer one still exists.
 print(v) --> prints "nil"
 v = 10 --> assigns the value 10 to the inner v
 print(v) --> prints "10"
 end -- end of scope for the inner v
 print(v) --> prints "5"
end -- end of scope for the outer v

Upvalues

Functions can access local variables from outside, so called Upvalues.

Upvalues can be used as a private data storage to save values across function calls.

You have seen in the examples above that local variables from outer scope blocks extend to inner scope blocks.

Such a variable, from the perspective of the inner block, is called an upvalue.

 Lua:

do
 local v = 5

 -- v is a local variable in the outer block and an upvalue from the perspective of the foo-function.
 function foo()
 print(v) --prints "5"
 end
end

Upvalues play a big role in Lua, because they allow to safely memorize values across multiple calls of the same function.

In JASS, you would have either needed to use global variables or hashtables to achieve the same result, which is always risky, because it can lead to name clashes if you don't pay attention.

The following function prints an integer, which increases by 1 on every call. The integer is declared once in the Lua root with starting value 0. The Lua root only runs once during loading screen, so the starting value will never be assigned again after that.

 Lua:

do
 local i = 0
 function printCounter()
 print(i) -- prints "0" on first call, "1" on second call, etc.
 i = i+1
 end
end

In other words, i is private for the printCounter function. It's only visible within the do ... end block and there is nothing else but a single function referring to it.

You might now ask, why can't we just declare iinside the function? Well, if you did that, i would be redeclared on every function call, thus starting with a fresh value of 0 every time. Which is what we normally expect from local variables, but the point of this example was to preserve values across function calls, right?

The same logic as above can be used to create permanent private data structures for functions or even a whole function library:

 Lua:

do
 --We create a private table to remember the kill count for all players
 --The table is local, so you can safely declare other tables named "killCounters" at other places of your code.
 local killCounters = {}

 function SetPlayerKillCount(whichPlayer, killCount)
 killCounters[whichPlayer] = killCount --killCounters is an upvalue from the perspective of this function
 end

 function GetPlayerKillCount(whichPlayer)
 return killCounters[whichPlayer]
 end
end

Note how both functions above have access to the private table, while functions outside the do... end block do not.

Closures

Closure is just a fancy name for a function remembering its own local scope and upvalues.

For example, SetPlayerKillCount remembers its upvalue killCounters, so the two combined (plus all other upvalues it might have) would be called a Closure.

We have already discussed one of the most important applications in the upvalues section above: private data structures.

Let's use this section to look at three other educative examples:

local functions, function generators and delayed function calls.

We have already learned that Lua functions are just values on their own, so they too can be saved in local variables in any scope block.

It's good practice to localize every function that just serves as a helper function for others.

 Lua:

do
 --Let's say, we want to create a UnitUtils-library.
 UnitUtils = {}

 --This function creates a dummy unit with different useful attributes that can be used for casting abilities.
 --It is only visible within the do...end block.
 local function createCasterDummy(whichPlayer, locX, locY, faceX, faceY, abilityId, abilityLevel)
 -- assume we have a caster dummy 'ha00' in the object editor
 -- create a dummy unit facing towards its target
 local dummyUnit = CreateUnit(whichPlayer, FourCC('ha00'), locX, locY, math.deg(Atan2(faceY - locY, faceX - locX)))
 UnitApplyTimedLifeBJ(2.00, FourCC('BTLF'), dummyUnit)
 UnitAddAbility(dummyUnit, abilityId) --add ability to cast
 SetUnitAbilityLevel(dummyUnit, abilityId, abilityLevel) --set it to the right level
 BlzSetAbilityRealLevelField( BlzGetUnitAbility(dummyUnit, abilityId), ABILITY_RLF_CAST_RANGE, abilityLevel - 1, 99990.) --unlimited range
 BlzSetAbilityRealLevelField( BlzGetUnitAbility(dummyUnit, abilityId), ABILITY_RLF_COOLDOWN, abilityLevel - 1, 10.) --add a cooldown to prevent additional casts
 return dummyUnit
 end

 --Creates a Dummy casting an ability cast from the specified location on the targetWidget.
 function UnitUtils.issueDummyTargetCast(castingPlayer, sourceX, sourceY, abilityId, abilityLevel, orderstring, targetWidget)
 local dummyUnit = createCasterDummy(castingPlayer, sourceX, sourceY, GetWidgetX(targetWidget), GetWidgetY(targetWidget), abilityId, abilityLevel)
 IssueTargetOrder(dummyUnit, orderstring, targetWidget)
 return dummyUnit
 end

 --Creates a Dummy casting an non-targeted immediate ability.
 function UnitUtils.issueDummyImmediateCast(castingPlayer, x, y, abilityId, abilityLevel, orderstring)
 local dummyUnit = createCasterDummy(castingPlayer, x, y, 0,0, abilityId, abilityLevel)
 IssueImmediateOrder(dummyUnit, orderstring)
 return dummyUnit
 end
end

issueDummyTargetCast and issueDummyImmediateCast are able to access the local createCasterDummy, because it is part of their closure.

A function generator or function factory is a function that creates and returns another function.

The example below shows, how the input parameter of the function factory becomes part of the closure of the generated function.

 Lua:

--Returns a function that adds the specified constant to a number.
function AddXFactory(constant)
 return function(summand)
 return summand + constant --the created function remembers the constant passed to the function factory, because it's part of the closure
 end
end

Add10 = AddXFactory(10) --Add10 is the function, which adds 10 to the input number

print(Add10(5)) --> prints "15"

The following function is able to call another function after some delay. It is a good example for the power of closures, because there are multiple nested levels, where functions remember values from outside.

Native Lua doesn't have good timer utility, so the below example uses Warcraft natives.

 Lua:

--Calls another function after the specified delay (uses Wc3 natives)
function CallDelayed(delayInSeconds, callbackFunc)
 local timer = CreateTimer()
 TimerStart(timer, delayInSeconds, false,
 function() --we use an anonymous function to both destroy the local timer after its expiration and call the real callback
 DestroyTimer(timer)
 callbackFunc() --the anonymous function remembers the callback function, because it is part of its closure
 end)
end

CallDelayed(5., function() print("Hello") end) --> prints "Hello" after 5 seconds

Recursion

Like in JASS, Lua functions can easily reference themselfes in their own definition to build a recursion:

 Lua:

--Adds 1 to the specified number, until the result is bigger than or equal to 10.
--We choose the var = function syntax here for educative reasons.
AddUntil10 = function(number)
 if number >= 10 then
 return number
 end
 return AddUntil10(number + 1)
end

print(AddUntil10(4)) --> prints "10"

Things get a bit trickier, when you try to build a recursion in a local function. If you just added the local keyword in front of AddUntil10 in the above example, it would not work.

The reason is that the right side of the local AddUntil10 = ... is resolved first and assigned to the local variable afterwards. In other words, at the time the function definition is resolved, the local variable doesn't yet exist, so the AddUntil10(number + 1) call from the function body refers to the global scope (and the global variable AddUntil10 is probably nil, so it would just throw an "Attempt to call a nil value" error on execution).

As we have discussed in the chapter about closures, a function remembers the scope of all its variables, so it will continue referring to the global scope even after the local AddUntil10 has been declared.

To get the scope right, you need to declare the local variable first (so now it is known as being local) and assign the function afterwards:

 Lua:

--this works!
do
 local AddUntil10 --define local variable first

 --and then build the recursion
 AddUntil10 = function(number)
 if number >= 10 then
 return number
 end
 return AddUntil10(number + 1) --this now refers to the upvalue AddUntil10 instead of the global name.
 end

 print(AddUntil10(4)) --> prints "10"
end

Tail Recursion

A recursive function calls itself, which theoretically implies that the outer call has to wait for the result of the inner call to resolve.

AddUntil10(4) above seemingly waits for AddUntil10(5), which in turn waits for AddUntil10(6) and so on.

Having too many function calls waiting on the stack can lead to a stack overflow.

Good thing, Lua is optimized for Tail Recursion. If the recursive call is the last thing to happen within function execution, the function at that point will logically just return the result of the recursive call, nothing more. E.g. AddUntil10(4) will simply return AddUntil10(5) without conducting additional actions on it. Lua is optimized for this kind of recursion: it recognizes that AddUntil10(4) simply returns AddUntil10(5) (as soon as the calculation reaches that point), so it replaces the first by the second on the stack instead of waiting for it. This behaviour keeps the stack small, saves performance and prevents stack overflow.

We as programmers should make sure that the Tail recursion hits as often as possible to optimize our code:

 Lua:

--BAD: non-tail-recursive version of AddUntil10
function AddUntil10(number)
 if n >= 10 then
 return number
 end
 local result = AddUntil10(number + 1) --recursive call is not the last thing to happen, because there is further code below
 return result
end

--GOOD: tail-recursive version (the original version above was tail-recursive)

Overview

Lua has several incorporated libraries for different use cases. Not all of them are made available in Warcraft 3, because Blizzard has disable several of them - either for security reasons or because they just don't make much sense in a Warcraft environment.

A list of all libraries and their functions can be found here (scroll down to the list of Lua functions, ignore the C-API stuff on the right). We will focus on giving a quick overview on the libraries available so that you understand your options. It however will not go through each individual library function, because those are perfectly documented on the linked Lua site.

Note that a library in Lua is nothing else but a global table containing functions.

For instance, the global variable string holds a table containing functions for string processing - the string-library. You can check that the table exists by calling print(string) (and recognize that the string printed is not "nil"). Likewise, you can prove that the string-library contains the sub-function by calling print(string.sub).

List of available Libraries

LibraryDescription

Basic FunctionsThis includes all native Lua functions that are directly accessible from global scope and not packed into any table. Their uses have been or will be discussed in the relevant chapters, so I focus on listing, which of them are enabled or disabled in Warcraft 3:

Enabled:

assert, error, getmetatable, ipairs, load, next, pairs, pcall, print, rawequal, rawget, rawlen, rawset, select, setmetatable, tonumber, tostring, type, xpcall

Disabled:

collectgarbage, dofile, loadfile, require

mathMathmatical library containing all the standard stuff. Roundings, exponentials, logarithm, modulo, trigonometrics, exactly what you expect.

string

&

utf8Libraries for string processing. The string-libary is optimized for strings consisting of 1 byte per char. It is ideal to use as long as you stay on the Ascii-charset, which you automatically do upon using English only (apart from processing player names).

Disabled: string.dump

The utf8-library can deal with strings following the utf8 standard. I can't tell you, whether or not Warcraft complies to the standard (actually it's too old for that), but I still got valid results upon testing it with German special characters. It shouldn't matter much for you. If you stick to English, you can stick to string

tableThe library is named table, but it's actually not designed to work on general tables, but rather on sequences (arrays without holes).

It provides tools to insert and remove new elements, sort the existing ones or create a table from any number of values.

osLibrary for accessing date and time information provided by your operating system. Functions inside this library are asynchronous by design, so you should only use them, if you don't intend to change game state with the result.

Several functions from this library are disabled in Warcraft. The only enabled ones are os.clock, os.date, os.time and os.difftime.

coroutineProvides functions to work with coroutines, which allow for halting and resuming function calls. Coroutines are an advanced topic and will not be covered in this tutorial.

io,

file,

package,

debugThese libraries are disabled in Warcraft 3, so we will not go into them.

The biggest bummer is the absence of the debug-library, but you can consult Lua Debug Utils, if you want to learn how to debug in Wc3 Lua.

### The ...-Operator

### Data Structures

### Multi-dimensional Tables

### Metatables

### Memory Leaks

### OOP Pt. 1

### OOP Pt. 2

### Generic for-loop

The ...-operator

You can use the ...-Operator to define functions that take any number of arguments.

The ... has to be the rightmost parameter.

First things first, every Lua-function can be called with any number of arguments.

For example, function foo(x,y) print(x,y) end supports 2 arguments by definition, but you can still call it with more than that without prompting syntax errors (like foo('a','b','c','d') is fine syntax-wise, but it will still only print 'a' and 'b' as per function definition).

There are built-in functions like the print-function however, which can effectively process all arguments passed.

Lua also allows for user-defined functions like that. For this purpose, it provides the ...-operator.

You simply need to add ... to the list of input parameters (but it needs to be the rightmost one).

In the function body, you can write ... again to refer to all arguments passed that come after the named ones. Think of it as a comma-separated list of values.

 Lua:

--This example function has one named parameter x. The three dots afterwards are a placeholder for any number of following arguments.
--The function shall print x first, followed by the value 42. Afterwards, it shall print all remaining arguments.
function foo(x, ...) --using the three dots as a parameter
 print(x, 42, ...) --passing all values into another function (we know that print can process all of them)
end

--we pass the value "a" for the parameter x and the next three letters for the ...-operator.
foo("a","b","c","d") --will print "a", "42", "b", "c", "d"

You have the following options to process the values within ... in the function body:

1) Dump all values into another function

We have already seen an example above. All values represented by ... can simply be dumped into another function. This makes sense, if that other function also supports the given number of arguments.

2) Assign ... to named local variables

This is only possible for a limited number of variables (because you have to explicitely write them down), so this kind of goes against the idea of unlimited input arguments. However, it can be an easy way to extract the first few arguments from the ..., so it can sometimes come in handy.

 Lua:

function bar(...)
 local x,y,z = ... --will only use the first 3 arguments, even if you passed more than 3. y and z will stay nil, if you passed only 1 argument.
 print(x,y,z)
end

--Excess Values are discarded
bar(1,2,3,4) --will print "1", "2", "3". The fourth parameter is discarded in local x,y,z = ... , because we only used 3 named variables.
--Excess Variables remain nil
bar(1,2) --will print "1", "2", "nil". The z-variable was not assigned a value, so it remained nil. The function still prints the value by definition.

3) Dump all values into an array

This option is one of the two ways of processing all values one-by-one (the other one is presented in the next section below).

The following syntax can be used:

t = {...} creates an array including all specified elements (starting from index 1). We can loop through the array with either a numerical for-loop from 1 to #t or by using ipairs. Both looping methods are incompatible with nil-values in the middle of the list, so if nil might be included, better use the second option.

t = table.pack(...) creates an array including all specified elements, which additionally is assigned the number of elements to t.n. You can then loop via numerical for-loop from 1 to t.n. This method is safe to use with nil-elements, so you might just generally prefer it over the first.

 Lua:

--We define a function that takes an unlimited amount of numbers, adds them up and prints the result.

--Using {...}
function CalcSum(...)
 local input = {...}
 local result = 0
 for i = 1, #input do
 result = result + input[i]
 end
 return result
end

--Using table.pack(...)
function CalcSum2(...)
 local input = table.pack(...)
 local result = 0
 for i = 1, input.n do
 result = result + input[i]
 end
 return result
end

4) Using select() within a numerical for-loop

Lua offers the incorporated select-function to select the last part from a given list of values as single separated values. This is especially useful with the ...-operator.

select provides the following uses:

For any positive integer i, select(i, ...) returns all values within ... from position i onwards.

 Lua:

local x,y,z = select(2, 'a', 'b', 'c', 'd', 'e') -- resolves to "local x,y,z = 'b', 'c', 'd', 'e' " (so 'e' will be discarded)

function CalcSumButIgnoreFirst(...)
 return CalcSum(select(2, ...)) --ignores the first passed argument and passes all further to CalcSum.
end

For any negative integer -j, select(-j, ...) returns the last j values within ....

 Lua:

print( select(-3, 'a', 'b', 'c', 'd', 'e') ) --resolves to print('c', 'd', 'e')

function CalcSumLastFive(...)
 return CalcSum(select(-5, ...)) --passes the last 5 arguments within ... to CalcSum
end

select('#', ...) returns the number of arguments within ..., including nil-values.

 Lua:

print( select('#', 42, 'b', 'c', nil, nil) ) --prints "5"

select() in combination with numerical for-loop can also be used to process values within ... one-by-one:

 Lua:

--Using select() and numerical for-loop
function CalcSum3(...)
 local sum = 0
 for i = 1, select('#', ...) do
 sum = sum + select( i, ... ) --select(i, ...) resolves to ALL values starting from i-th position, but the +-operator only uses the first of them, which is exactly the i-th value
 end
 return sum
end

5) Using Recursion and a named parameter

We can also extract the first parameter from the ... by having one named parameter in the original function:

 Lua:

--Using recursion and a named parameter
function CalcSum4(x, ...)
 if x == nil then
 return 0
 end
 return x + CalcSum4(...) --this calls CalcSum4 again, so the first parameter of the dots will be the named parameter of the recursive call
end

This method as presented above doesn't support nil-values in the middle of the list (and those wouldn't make sense in a numerical calculation anyway). If you have a function that shall support nil-values, you can support them by using the condition if x == nil and select('#', ...) == 0 instead.

Note: The recursion presented above is not tail-recursive, but it could be made so.

Which Method to choose?

Method 3), 4) and 5) fullfil the same purpose, as they are all options to process values one-by-one.

The recursive and the select-method have the advantage that they don't need to create a table. They do produce performance overhead though, because they need to pass all arguments in every single step.

As a rule of thumb, you can prefer recursion or select-processing, if you plan to pass a few arguments to the function, and table.pack(), if you plan to pass many arguments at once.

Regarding Warcraft, you will typically only deal with a few arguments, so you can generally prefer the recursion or select-method over table.pack(...). Plus, the garbage collector doesn't currently seem to work too well in Wc3, so producing less tables has additional upsides.

table.unpack

After seeing the table.pack function, you might not be surprised to hear that also the inverse function exists: table.unpack(A,i,j) takes an array A and two integers i, j and returns all values from A[i] to A[j] as single separate values. Both i and j are optional parameters; not specifying them will unpack everything between indices 1 and #A.

 Lua:

local x,y,z = table.unpack({1,2,3}) --resolves to "local x,y,z = 1,2,3"
print(table.unpack({"a","b","c"},2,3)) -- resolves to print("b","c")

Data Structures

We've said in the chapter about Tables that they can imitate literally every other data structure you might think of.

We will use this chapter to look at a few popular ones and how to implement them in Lua.

Sets

Sets in theory are containers that only contain single elements (instead of key-value-pairs). Elements have no order and no element can be included more than once. For instance, Warcraft's native unit groups are Sets.

Sets can be easily imitated with Lua-tables:

Due to the fact that keys can only be included once in every table, we can just use table keys to store elements (the value being set to anything except nil, like true).

 Lua:

--Example: Using a table as a Set
S = {}
--Adding the elements 5 and Player(0) to the Set:
S[5] = true
S[Player(0)] = true
--Adding the element 5 again will not do anything:
S[5] = true --same as before
--Removing an element
S[5] = nil
--Checking for elements:
if S[Player(0)] then --print "yes", if Player(0) is contained in the Set.
 print("yes")
end

The implementation above has an important downside: It doesn't allow for storing methods inside the table. For instance, it would be nice to have an S.add(element)-method, but defining such would use the "add"-key within the table, preventing you from adding the string "add" as an element.

You can of course store the add-function somewhere else. Using object-oriented notation is also possible, but requires more work. I recommend to save you the work and just use the existing resource spotlighted below.

Sets like the above can be iterated using the pairs-iteration, but as we've learned before, this is potentially asynchronous in multiplayer. If you need to iterate over Sets in multiplayer (which I do), again just import the resource below, which provides a multiplayer-synchronous iteration for x in Set:elements() do.

Resource Spotlight: Set/Group Datastructure

A lot of work and testing went into this resource.

It offers object-oriented syntax, multiplayer-synced iteration, all the basic Set operations (like union, intersection, etc.), functions to create Sets of units (like all units owned by Player 1) and much more.

I strongly suggest using Sets instead of Unit groups and Player groups, because they are so much more convenient.

Linked Lists

Linked Lists, like Sets, only contain elements, no key-value-pairs. In contrast to Sets though, elements have an order, i.e. they are linked to their neighbours (either one or both) and thereby form a chain.

Technically, there is no outer data structure containing everything - instead, a Linked List is represented by a head node that is linked to the next node in the chain, which again is linked to the next node and so on. Each node contains exactly one element and knows its successor (and maybe predecessor) - that's all. As such, checking for whether particular elements are included in the Linked List requires iterating through the whole List, which is pretty slow.

I'm not going too much into when you should use Linked Lists and rather recommend the Wikipedia article for further reading.

Linked Lists are rather straightforward to implement in Lua.

All nodes, including the head, are represented by a separate table.

 Lua:

--The LinkedList is represented by the head node, which we call LL in this example
LL = {}

--Adding an element to the list requires creating a new node and linking it to the existing one
--Let's add the number 5 to the List.
local newNode = {element = 5, prev = LL}
LL.next = newNode

--Lets add another element 10 up front
local newNode = {element = 10, next = LL.next, prev = LL}
LL.next.prev = newNode
LL.next = newNode

--Iterating through the List:
local loopNode = LL.next
while loopNode do
 print(loopNode.element) --> prints "10" and "5"
 loopNode = loopNode.next
end

These are the basic operations. You probably want to define functions that do the adding, removing and iterating for you. Doing all the required table operations by hand every time would be way to cumbersome and error-prone. Luckily, LinkedLists easily support storing those functions in the head node for instance.

 Lua:

--Storing a dedicated add-method in the head node.
LL.add = function(newElement) --Adds a new element directly after the head, shifting all existing nodes backwards.
 local newNode = {element = newElement, next = LL.next, prev = LL}
 if LL.next then
 LL.next.prev = newNode
 end
 LL.next = newNode
end

--Using it to add an element
LL.add(42)

This works, because the key "add" is not occupied by anything else. We had defined LinkedList nodes in a way that they were guaranteed to only hold the keys "element", "prev" and "next", allowing us to reserve further keys for methods.

It would be further beneficial to provide a function that can create a fresh LinkedList together with all of its methods. This requires knowledge about object-oriented programming in Lua, though (see relevant chapter).

Before implementing your own, I recommend using the following existing resource on Hive:

Resource Spotlight: Definitive Doubly-Linked List

To save you the effort of programming a Linked List yourself (which can take a while), better use this resource present on Hive.

It has seen quite an elaborate time of development and went through many stages of improvement.

It supports Object-oriented notation, features a doubly-linked circular style and performs great in reality.

Dictionaries

Dictionaries contain (key,value)-pairs and as such, are already perfectly represented by Lua tables.

Create a table and use it like a dictionary - not much else to say here.

Stacks

Stacks are a simple data structure that basically only allows for adding an element (push) and retreiving plus removing the last element added (pop).

This again is very simple to do in Lua, because we can use arrays to store elements without caring for array limits - they are always dynamic after all.

The fact that we only need the array part of the table to store our elements allows us to save additional information under different keys, for instance the number of elements n and the functions that perform the push- and pop-operation.

 Lua:

--The stack is represented by one Lua table. We save the number of elements n inside.
Stack = {n = 0}

--Define push and pop operations
Stack.push = function(newElement)
 Stack.n = Stack.n + 1
 Stack[Stack.n] = newElement
end

Stack.pop = function()
 if Stack.n == 0 then
 return nil --return nil in case the stack is empty
 end
 local element = Stack[Stack.n]
 Stack[Stack.n] = nil
 Stack.n = Stack.n - 1
 return element
end

--Add the elements 10 and "abc"
Stack.push(10)
Stack.push("abc")

--Remove and print the element last added
print(Stack.pop()) --prints "abc"

Mirrored Tables

One big advantage of Lua tables is that they can contain keys and values of any type.

This fact allows us to create tables that mirror other tables with swapped keys and values.

I.e. given a table t that contains no value more than once, you can define another table m with m[b] = a for every t[a] = b.

Consider the following example:

You create a racing game and need to maintain player positions for the current race. You want to use a player-array SortedPlayers that is sorted by current position, i.e. SortedPlayers[1] contains the player in the leading position (and so on).

Maintaining an array like this allows you to easily sort it by an external parameter (such as the position of the player's vehicle on the racing track) on demand.

But there is a drawback (so far): To ask for the position of a particular player, you would need to search through the whole SortedPlayers-array, which takes time.

We can solve this by mirroring the array to another table:

 Lua:

--Create table that will mirror SortedPlayers
PlayerPositions = {}

--this works, because SortedPlayers contains no value more than once
for position, player in ipairs(SortedPlayers) do
 PlayerPositions[player] = position
end

This allows us to read the position of Player 1 from PlayerPositions[Player(0)] in O(1) time.

Obviously, we need to update PlayerPositions after every update of SortedPlayers to keep them synced, which can be done more or less efficiently depending on how our code works.

Effectively, we add overhead to writing into the data structure (because we maintain two containers now) and reduce the overhead of reading it (at least the overhead of reading the mirrored data like the position of a particular player in our example). This obviously adds the most benefit, when we read often and write rarely. It also simplifies our code, when we read the data from many different places, but only write from one place.

Multi-dimensional tables

You can create multi-dimensional tables supporting table[index1][index2] syntax by nesting 1-dimensional tables.

That means you have to create subtables table[index1] = {} before accessing table[index1][index2].

Let's say you have a Free-for-all-map, where players collect points by defeating other player's heroes - and you want to remember how often each player has scored on each other player individually. We naturally think of using a 2-dimensional data-structure like scores[scoringPlayer][onWhichPlayer].

Can we define the scores-table in a way that it allows more than one index? Just declaring scores = {} doesn't do the job, because tables are always 1-dimensional in Lua. Accessing scores[scoringPlayer] works fine, but attempting to access the second index would just yield an error. So what can we do? The answer is: Nesting 1-dimensional tables.

 Lua:

t = {} --create base table
t[x] = {} --create another table into t[x] for any given key x
t[x][1] = "Hello" --as t[x] is a table, we can assign a value to any key inside (1 being just an example)
print(t[x][1]) --> prints "Hello"

That method works for any depth of dimension. Nobody hinders us from also setting t[x][1] = {} and then accessing t[x][1]["bla"] (and so on).

So far though, we have only prepared the primary key x for holding further dimensions, while for any other key y, t[y] is still nil and t[y][1] can not yet be accessed.

In fact, we need to manually prepare every key that should open up for another dimension.

Going back to the player scoring example we started with, it could look like this:

 Lua:

--prepare base table
scores = {}
--add second dimension for every player in the lobby
for i = 0, GetBJMaxPlayers() -1 do
 scores[Player(i)] = {}
end
--now you can freely use the second dimension as long as the first index is a player
scores[Player(0)][Player(1)] = 1 --works

This process is convenient to conduct, when you know all primary indices in advance, as we did in the player scores example.

But what shall we do, if the primary index is a dynamic thing that we don't have before runtime? What's the solution, if the number of primary indices grows indefinitely during playtime?

For instance, we might want to save for every unit how often it has scored on each player, but the number of units is not known in advance, changes dynamically during map runtime and can grow beyond any limit (technically not, but you get the point).

To tackle this, we obviously need a more dynamic approach:

Dynamic Multi-dimensional tables

If we don't know the primary keys in advance, we can't prepare subtables in advance, so we must add them during runtime.

For the score counter discussed above, this means that for any unit u we must create a subtable in scores[u] upon accessing it for the very first time.

 Lua:

do
 --Declare Base Table
 local scores = {}

 --We define a function that increases the score for any unit by 1.
 --It will add a subtable to our base table, if it hasn't already been added.
 function IncreaseScore(scoringUnit, onWhichPlayer)
 --First make sure that scores[scoringUnit] contains a subtable, but don't overwrite any existing subtable.
 scores[scoringUnit] = scores[scoringUnit] or {} --It's really this easy. We assign scores[scoringUnit] to itself, if already present, and to a new table otherwise.

 --Now we can access scores[scoringUnit][p] for any player p.
 --We increase the existing score for this particular unit and player by 1. If there is no existing score, we default to 0.
 scores[scoringUnit][onWhichPlayer] = (scores[scoringUnit][onWhichPlayer] or 0) + 1 --again, the or-operator allows us to specify a default value, if we don't have an existing one.
 end
end

You see in the implementation that we need to take care for both adding additional dimension to primary keys on demand and caring for starting values for the secondary keys. However, once you understood the functionality of the or-operator, this becomes quite straightforward to implement.

Two lines of code aren't too much after all, are they?

Now, how would we read a value from the dynamic multi-dimensional table? Maybe we want to know for a particular unit u and player p how often u has scored on p. But has u scored on pat all? If not, scores[u] would still be nil and attempting to read scores[u][p] would throw an "Attempt to index a nil-value" error and break your code. Furthermore, even if u had scored on some player and scores[u] existed, there would still be no guarantee that u had scored on the particular player p, i.e. scores[u][p] could still be nil.

Long story short, reading from a dynamic multi-dim-table also requires extra work. You need to catch both potential nil-scenarios.

Completing the above example:

 Lua:

--Use a function to return the score for any particular unit and player
--The function is to be placed in the same scope as IncreaseScore above, so that it can access the scores upvalue
function GetScore(whichUnit, onWhichPlayer)
 --only access scores[whichUnit][onWhichPlayer], if values in both dimensions are not nil
 if scores[whichUnit] and scores[whichUnit][onWhichPlayer] then
 return scores[whichUnit][onWhichPlayer]
 end
 --if any of the two dimensions was nil, the unit clearly hasn't scored on the player so far, so we can safely return 0
 return 0
end

The and-operator plays a critical role in this query: we need to be sure that scores[whichUnit] exists before we access scores[whichUnit][onWhichPlayer]. But isn't the if-condition evaluating both statements at once and hence will throw an error upon accessing scores[whichUnit][onWhichPlayer], if scores[whichUnit] is nil?

Fortunately not - the and-operator is short-circuit, i.e. it only evaluates the second argument, if necessary. When the first argument is already nil or false, it will simply resolve to the first argument without touching the second, hence no error.

In general, if you want to access t[a][b][c][d] and any dimension could potentially be missing (maybe even t), you would need to ask if t and t[a] and t[a][b] and t[a][b][c] then. If necessary for your logic, t[a][b][c][d] can be added to the query.

Resource Spotlight: Multi-dimensional Table

We learned above that creating and maintaining multi-dimensional tables is perfectly possible, but can also be cumbersome (especially for higher dimensions).

The resource Multi-dimensional Table highly simplifies that endeavour by conducting all subtable management for you.

With it, using multi-dimensional tables can be this simple and straightforward:

 Lua:

T = MDTable.create(5) --creates a multi-dimensional table with 5 dimensions
T[1][Player(0)]["Hello"][5.5]["World"] = 1000 --immediately works. You don't need to take care of subtables.
print( T[1][Player(0)]["Hello"][5.5]["World"] ) --prints "1000"
print( T[1][2][3][4][5] ) --prints "nil", because you haven't assigned a value to these indices before. Again, you don't need to check for subtables.

--The resource also comes with a multi-dimensional pairs-iteration:
for key1, key2, key3, key4, key5, value in pairs(T) do
 print(key1, key2, key3, key4, key5, value)
end

You can even define default values and prebuild data structures with them. Read more in the resource thread.

Metatables

Lua tables are very powerful by design, being able to imitate basically any other data structure you might think of.

At the same time however, they have quite a bunch of built-in default behaviour that we might not always consider advantageous.

So it's probably no surprise that Lua even allows us to change different aspects of table default behaviour.

The tool of choice are so called metatables, which are basically normal Lua tables that can be attached to other tables to influence how those work. Different tables can get the same metatable attached, so it's common practice to create one metatable that defines behaviour for a whole bunch of other tables.

Aspects of default behaviour that can be changed with metatables are:

Results of relational, arithmetical or bitwise operators (like equality, addition, etc.) on tables.

Results of read and write access.

Result of tostring(table).

Result of attempting to call a table as a function.

Behaviour of the pairs-iteration.

Aspects of garbage collection.
The official documentation can be found here, but it's rather short and doesn't provide a complete list.

Creating and assigning a metatable

You can use any table as a metatable for any other table. There is no specific metatable-creation process, you just attach normal tables to other normal tables. Attaching a metatable works with the setmetatable function. Likewise, any metatable can be retreived from its base table by using getmetatable.

 Lua:

do
 T = {}
 local mt = {}
 setmetatable(T, mt) --we attach mt as a metatable to the base table T
 -- so far the metatable is empty, so it won't do anything.
 -- getmetatable(T) will return mt, so we don't need to keep a global reference to mt.
end

We can also detach an existing metatable from the base table T by calling setmetatable(T, nil).

Defining Metamethods

So how do we teach a metatable to influence the behaviour of its base table? We need to add metamethods to it!

Lua provides a separate metamethod for each of the aspects mentioned in the list further above. Each comes with a name starting with two underscores.

For instance, the metamethod __tostring defines, how the tostring function works on the base table. By documentation, it is supposed to be a function taking the base table and returning a string. To apply this special behaviour, we need to create the function and save it into the metatable at metatable.__tostring.

 Lua:

do
 T = {}
 local mt = {}
 setmetatable(T, mt)
 -- add a metamethod to the metatable
 mt.__tostring = function(t) return t.name end
end

T.name = "bla"
print(tostring(T)) --prints "bla"

--The above code shows the steps involved in different lines for educative purposes.
--Many people will prefer this short version:
T = {}
semetatable(T, {__tostring = function(t) return t.name end} )

Clearly, we must know exactly what Lua expects for any particular metamethod to work. You won't get far without the official documentation or this tutorial. For instance, trying with mt.__tostring = "bla" would just throw an error. __tostring is not supposed to be a string, but a function returning a string.

I know, this can look confusing at first glance. Why does tostring change its output just because __tostring is set in some metatable? It's hard to see any logical connection between the two from just looking at the code. The answer is of course, tostring got implemented in a way that it first checks its arguments metatable for the existence of __tostring and applies that, if possible. That's a background process. We can only know by reading the Lua documentation.

Relational Operators

There is a separate metamethod for each relational operator. All have in common that they are supposed to be a function taking two arguments and returning a boolean.

MetamethodOperatorComment

__eq==The equality operator normally checks for whether the two references in comparison point to the same object or not. It can make sense to change this behaviour, if you want to determine equality based on table values instead, thus imitating a call-by-value based datatype.

For instance, you might want to implement Complex Numbers via tables saving the real part at index [1] and the imaginary part at index [2]. In such a case, it would make sense to let the equation {5,6} == {5,6} resolve to true, even if the two tables involved are different.

The metamethod used would be metatable.__eq = function(a, b) return a[1] == b[1] and a[2] == b[2] end.

If you still want to check for the original table equality after having overwritten the equality operator, you can do that by using rawequal. In the above example, rawequal({5,6}, {5,6}) would still resolve to false.

Note that the __eq-metamethod is only invoked, when both objects involved are tables. Otherwise, Lua will always use the standard equality operator.

There is no separate metamethod for inequality. Instead a ~= b resolves to not (a == b), so the equality metamethod would be used.

__lt<The less-than-operator normally doesn't work on tables at all, but you can make it do so.

Again, there is no metamethod for the >-operator. Instead, a > b is translated to b < a, so the metamethod for the <-operator will be used.

__le<=The less-or-equal-operator.

Once again, there is no separate metamethod for greater-or-equal, because a >= b will be translated to b <= a to use the metamethod for <=.

Mathematical and String Operators

Mathematical Operators (addition, multiplication, ...) normally only work on numbers (and strings, if number conversion is possible), while applying them to a table would simply throw an error. Except when you use the appropriate metamethods, of course.

All metamethods below must be functions taking two arguments and returning any value.

+-*///^%..

__add__sub__mul__div__idiv__pow__mod__concat

Example

Let's consider again the example on complex numbers that we discussed in the section about the equality operator.

Adding two complex numbers works by separately adding the real and the imaginary part. The metamethod could look like this:

 Lua:

--Let mt be the metatable used for our complex numbers
mt.__add = function(a,b)
 local result = {a[1] + b[1], a[2] + b[2]} -- Create a new complex number
 setmetatable(result, mt) -- Assign the metatable to the new number (so __add will work on it as well)
 return result
end

-- Again, the above has a short version based on the fact that setmetatable returns the base table.
-- mt.__add = function(a,b) return setmetatable( {a[1] + b[1], a[2] + b[2]}, mt) end

In reality, we would probably not create a new object manually within the metamethod, but pre-define some CreateComplexNumber function that would do the work for us. That function would also be responsible for applying the correct metatable, making the above code even shorter.

Table Access

This chapter will be an important one. The metamethods in scope are __index and __newindex, probably the most important metamethods in whole Lua.

__index is invoked, when you attempt to read a non-existent key from a table. This would usually just return nil, but __index allows you to return any value instead and produce any side-effects you like.

This enables us to do many different things: Defining default-values for tables, counting read attempts, maintaining parallel data structures, forwarding read attempts to other tables and so on and so forth.

The last point mentioned plays a critical role in Lua's object-oriented programming, as we will see in the relevant chapter.

__index must either be a table (in which case read access is forwarded to that table) or a function taking both the original table and the key accessed and returning any value (in which case that function is invoked and its return value will be used as a result of the read operation).

You can still conduct a normal read access (not invoking __index) afterwards by using rawget.

Examples:

 Lua:

--Example 1: Setting __index to another table.
T = {a = 1} --base table
X = {a = 5, b = 2} --another table
setmetatable(T, {__index = X} ) --set __index to a table. Read access on nil-keys will be forwarded to X.

print(T.a) --prints "1". __index is not invoked, because key "a" exists in T.
print(T.b) --prints "2". __index forwards read access to X, because key "b" doesn't exist in T.
print(T.c) --prints "nil". __index forwards read access to X, and X.c is nil.
print(rawget(T, "b")) --prints "nil". rawget doesn't invoke __index and "b" doesn't exist in T.

--We can even produce a chain:
Z = {c = 10}
setmetatable(X, {__index = Z})
print(T.c) --prints "10". "c" doesn't exist in T, so __index forwards to X. "c" doesn't exist in X, so the __index defined for X forwards to Z.

 Lua:

--Example 2: Setting __index to a function.
T = {a = 1}
--We want to return the requested key as a value by default
setmetatable(T, {__index = function(t, key) return key end} )

print(T.a) --prints "1". __index is not invoked.
print(T[1.5]) --prints "1.5". __index got invoked and returned the key accessed

The counterpart of __index is __newindex. It triggers upon write access to a non-existent key, i.e. upon executing T[k] = value while T[k] is nil.

Much like __index, __newindex can either be a table (in which case write access is forwarded to that table) or a function taking the base table, the key accessed and the value to write (in which case the function is called with these arguments).

Quite obviously for empty base tables, setting __newindex to another table will force the base table to remain empty (because every write access triggers __newindex and hence is being forwarded).

If you want to write to a table without triggering __newindex, you can use rawset.

 Lua:

--Example 1: Forwarding write access to another table.
T = {a = 1}
X = {}
setmetatable( T, {__newindex = X} )
T.a = 2 -- this write attempt is conducted in T. It doesn't invoke __newindex, because "a" exists in T.
T.a = nil -- same, T.a is now nil
T.a = 5 -- this invokes __newindex, because "a" doesn't exist anymore in T. Write access forwarded to X.
print(T.a) -- prints "nil"
print(X.a) -- prints "5"

 Lua:

--Example 2: Using a function to prevent writing keys > 5
do
 T = {a = 1}
 local function newindex = function(t, key, value)
 if type(key) == 'number' and key > 5 then
 return -- write attempts for numeric keys > 5 are just ignored.
 end
 rawset(T, key, value) -- using rawset prevents newindex from triggering just again (preventing recursion and stack overflow)
 end
 setmetatable( T, {__newindex = newindex} )
end

A common scenario is that we actually want to catch every read or write access, not just upon accessing nil-keys.

This is also possible using the __index and __newindex-metamethods, but we have to use some tricks.

First, we need to make sure that our base table always remains empty so that every access triggers a metamethod.

Second, we have to maintain a private data structure saving the actual values.

 Lua:

--Example
do
 T = {}
 local data = {} --private data structure saving values for T
 -- prepare metamethods as local functions (makes it easier to read)
 -- read access
 local function index(t, key)
 <produce side effects>
 return data[key]
 end
 -- write access
 local function newindex(t, key, val)
 <produce side effects>
 data[key] = val --we don't need rawset here, as data will not have a metatable.
 end
 -- Attach the metatable
 setmetatable( T, {__index = index, __newindex = newindex} )

 -- Now all read and write access to T is forwarded to data. T remains empty, ensuring the metamethods will always trigger.
end

Other Table Operations

Let's look at the remaining metamethods:

MetamethodExplanation

__lenDefines the result of the #-operator (which normally calculates the length of a sequence).

Must be a function taking one argument and returning any value.

If you want to apply the original #-operator after having defined this metamethod on a table, you can use rawlen for that purpose.

__callAllows you to call a table as if it was a function and defines the result of that operation.

Example:









 Lua:

T = {value = 5}
setmetatable( T, {__call = function(t) return t.value end} )
print(T()) --prints "5"

Often used in OOP to allow creating an object from the class (i.e. object = Class() instead of object = Class.create()).

__tostringDefines the result of tostring(T) (and consequently print(T)) for a given table T.

Must be a function taking one argument and returning a string.

 Lua:

T = {name = "someName"}
setmetatable(T, {__tostring = function(t) return t.name end})
print(T) --> prints "someName"

Typically used for debugging.

__nameAlters the output of tostring (much like __tostring above), but only changes the type-reference, not the hex-code.

 Lua:

T = {}
print(T) --> prints "table: 0000018F4C24"
setmetatable(T, {__name = 'someName'})
print(T) --> prints: "someName:0000018F4C24"

Typically used in OOP for simplified debugging.

__pairsDefines, how the pairs-loop works on the given table.

Must be a function taking a table and returning an iteration function.

Please refer to the tab about Generic For-Loops to learn, what an iterator function is.

By random example, SyncedTable is using this metamethod to make the pairs-loop synchronized across players.

__modeDefines, whether keys, values or both should be weak references in the given table, which influences garbage collection.

Must be one of the values 'k' (for keys), 'v' (for values) or 'kv' (for both). Technically nil is also allowed (and the default), meaning that neither keys nor values are weak.

Example: metatable.__mode = 'k'

Please refer to the tab about Memory Leaks for further information about weak references.

Memory Leaks

This chapter summarizes the knowledge required to prevent memory leaks in your map.

This is a tough topic that can be further researched and I don't know everything about it. If you have extra information or see something wrong, please write a comment and I will update this information asap.

The Garbage Collector

Lua features a so called garbage collector, which is basically a background process that removes any obsolete data from memory.

Data becomes obsolete at the point, when there are no more active references to it.

That means, data is removed, when there are no more variables, tables, functions or whatever referring to it.

As a consequence, you don't need to actively remove Lua objects. In fact you can't, because there are no destroy-methods for any type of data in native Lua.

Instead, your job is to clear references to an object once you don't need it anymore, so the garbage collector can do its job.

You can take a look at the official documentation for further reading.

Note that by official documentation, Lua provides the collectgarbage function to control and monitor the collection process, but Blizzard has disabled it in Warcraft 3 Reforged (at least after a certain patch). As per patch 1.32.x, the garbage collector runs a fixed number of times per game tick, probably to keep the process synchronized across players.

A few examples:

 Lua:

--Example 1: Global Variable References
X = {}
Y = X
X = nil --X loses reference to the table it held, but the table is still referenced by Y and thus kept in memory.
Y = nil --Y loses reference to the table it held. There are no more references, so the table will be removed by the gc.

 Lua:

--Example 2: Local Variable References
function Example()
 local X = {"Hello"}
 return X[1]
end
--Local var X is being created at the beginning of every function execution and gets out of scope at function end.
--Upon getting out of scope, the variable ceases, thus losing reference to the table it held. The table will be garbarge collected.

 Lua:

--Example 3: Table References
T = {}
T[1] = {} --we save another table within the first
T = nil -- T loses reference to the table it held, which will consequently be removed by the gc.
--Chain reaction: Once the table formerly held by T is removed, it will clear all of its (key,value)-pairs.
--This clears reference to the table held at index 1, which will consequently also be removed by the gc.

 Lua:

--Example 4: Closure References
do
 local X = {}

 local function readX(key)
 return X[key]
 end

 local function writeX(key,value)
 X[key] = value
 end

 function DoWeirdStuff(value)
 X[value] = value --we write to X without using writeX... shame!
 return readX(value + 1)
 end
end
-- All local variables (X, readX and writeX) are now out of scope. Will their contents be collected?
-- DoWeirdStuff is technically a global variable referencing a function. The function references readX. readX references X.
-- So the contents of DoWeirdStuff, readX and X will not be collected.
-- The function contained in writeX however is not referenced anymore and WILL be collected.

-- Now what about this?
DoWeirdStuff = nil
-- The function that was saved in DoWeirdStuff is not being referenced anymore and will be collected.
-- Consequently, readX is not referenced anymore and will also be collected. Afterwards, same holds for X.
-- Classical chain reaction.

Key Takeaways

Local Variables cease at the end of their scope, losing reference to what was inside. You do not need to actively nil them.

Once a table gets garbage collected, it will automatically clear all (key,value)-pairs it held, losing reference to what was inside. This can form a chain reaction to subtables and their entries.

If you want a whole data structure to be collected, you usually only need to clear reference to the outmost table (assuming you haven't referenced subtables from anywhere else).

Removing Warcraft Objects

In theory, warcraft objects can and will also be removed by the Lua garbage collector after losing all references to them.

However, researchers like @TriggerHappy and @ScrewTheTrees found that the gc wasn't always meeting our expectations in this regard. For instance, losing all references to running periodic timers would eventually garbage collect them, but the delay until the collection would potentially still let the timer expire and execute its callback. In worst case, this would even cause a desync.

On another instance, anonymous functions passed to a BoolExpr would not always be collected properly, leading to huge memory leaks.

I don't know if this information is still up to date, but it probably doesn't matter. We don't exactly know, how Warcraft handles its own data in the background, so it might as well be that Warcraft keeps referencing some of its objects from somewhere and hence prevents garbage collection.

That means, it's best practice to manually destroy Warcraft objects, when you don't need them anymore, exactly like you are used from JASS!

I.e., you should still use DestroyTimer, RemoveRect etc.

If you haven't heard of these functions so far, you can read through this guide on Memory Leaks.

Quite obviously, you create and destroy much less Warcraft objects in Lua mode than in JASS mode, because the language has better features to replace them:

Hashtables are not sensible to use in Lua, because Lua tables are strictly superior.

Locations are not sensible to use in Lua, because they are super slow and Lua-functions support multiple return values, so you can just return the x and y-coordinates separately instead of using a location.

Groups and Forces can sometimes make sense to use, but again Lua-tables can do better and I recommend just using Sets.

Dead References to Warcraft Objects

The necessity to use Wc3-destroy-methods creates another problem: dead references. These occur, when you access a Wc3 object after you have previously destroyed it. Variables and Table Entries pointing to the destroyed object will continue to point to it - they are not automatically being cleared (except for local vars that are getting out of scope). Logically, using those dead references in your code after destruction can lead to bugs (the code doesn't necessarily break, but it certainly won't work as intended).

 Lua:

--Example
local mapCenter = Location(0,0)
RemoveLocation(mapCenter)
--the mapCenter variable now holds a dead reference
CreateUnitAtLoc(Player(0),FourCC('hfoo'),mapCenter,0.) -- nothing happens

This scenario is much more likely to happen in Lua than it was in Jass, because we now have tables that can contain objects of any type.

It's easy to save an object into a table and destroy it at some other place in our code, leaving a dead reference behind.

You have to ensure this doesn't happen. When you destroy a Wc3 object, make sure to also remove it from any table it might potentially be included in (if you can't, at least be sure that the table entry is not accessed until the end of the game, including it's not being looped over).

But there is another problem: Some Wc3 objects are not necessarily destroyed by the programmer, but also by the game. This includes units, items and destructables, as a consequence of ingame events. Units for instance get removed from the game, after they have fully decayed. At that point, you might still have references to them inside global variables and tables, which are now dead references.

You need to make sure that there are no issues deriving from this scenario, which isn't always easy.

You can theoretically catch the moment when a unit leaves the game by using a trigger and remove all table references in the trigger action. This is advanced stuff though, because you need a custom A unit leaves the game event for that, which Wc3 doesn't provide by default. The SetUtils resource provides such an event. In case you don't need to save dead units, it's much simpler to use the existing A unit dies.

Another way is to check references for whether they are invalid or not, before you use them. For units, this can be done by using GetUnitTypeId(unit), which returns 0 for invalid references and a positive number for valid ones.

A third way to get rid of dead references is to use weak references in the first place, see section further below.

This basically setups any table in a way that the Lua garbage collector automatically removes the references, when not needed anymore.

Garbage collection doesn't happen instantly, though, so looping over a table with weak references might in fact include references to destroyed objects that just haven't been collected yet. That means, you should probably avoid looping over tables with weak references (or vice versa, avoid giving weak references to tables that you plan to iterate over).

The combination of Warcraft objects and weak references needs further research. Currently, I can neither guarantee that the automatic removal from a table by weak reference works at all (for instance, the game might maintain other references making the object non-garbage-collectable), nor that the removal would happen at the same time for all players.

Feel free to contribute to this section by providing research to the following questions:

Which Warcraft objects can be automatically removed from tables using weak references? Widgets might behave different than other types.

Can using weak references for Warcraft types cause desyncs under any circumstances?

Circular References

We often have situations in Lua coding, where references between objects form a circle, which from what we know so far would make them uncollectable.

Fortunately, the Lua garbage collector is able to recognize circular references and ignores all of them. Instead, it will only consider external references to a circular structure.

Consider the following example:

 Lua:

do
 local t1, t2 = {}, {}
 t1.next = t2
 t2.next = t1
 --now t1 and t2 reference each other. They form a circle.
 T = {t1, t2} --also T references both
end
--The scope of the local variables t1 and t2 has ended, these references are cleared.
T[1], T[2] = nil, nil --Now, T also loses its references to the two tables.

--Will the two tables be collected?
--Yes, they will! The only references left are circular, which is not considered an active reference for the garbage collector.

Weak References

We learned above that the garbage collector will remove any Lua object as soon as there is no active reference to it anymore.

You can however decide for all references in any particular table that they shall not be considered to be an active reference by the garbage collector. Given a table t with metatable mt, setting mt.__mode = 'k' will cause all keys of that table not to count as an active references for the garbarge collector anymore. They become weak references, i.e. the garbage collector can and will collect an object even if you used it as a key in t as long as there is no other strong reference left to it from somewhere else. The same can be done for values by setting mt.__mode = 'v' or to both by setting mt.__mode = 'kv'.

Examples:

 Lua:

--Theoretical example:
T = {}
setmetatable(T, {__mode = 'v'}) -- Values in T become weak references
T[1] = {} --this table will immediately be collected, because there is no strong reference to it.
--Well, probably not immediately, because the garbage collector doesn't work instantly. But you get the point.

 Lua:

--Practical example:
--Let's say we have a game where players can own and name one pet each.
--This scenario might yield the following data structure:
Pets = {} --a table containing (player,pet), if pet is owned by player
PetNames = {} -- a table containing the name of each pet as (pet, name)
setmetatable(PetNames, {__mode = 'k'}) -- Keys in PetNames become weak references

--Create and name a pet for Player 1.
Pets[Player(0)] = <someUnit>
PetNames[Pets[Player(0)]] = "Foxy"

--Clear reference to the pet from the first table.
Pets[Player(0)] = nil --This was the only strong reference, because PetNames has weak keys.

--Consequently, the pet will be collected!

--Important: As soon as the pet is collected, the pair (<pet>, "Foxy") will automatically be removed from the PetNames table.

What do we learn from this example? Well, when we maintain data about the same object at different places, we can decide for one place to be responsible for its live by letting all other places have weak references. This makes it very easy to actively clear references, because you only have to nil one single entry in one single table.

Be careful with weak references. If done wrong, they can lead to unexpected removal of objects and consequently cause bugs.

Also, don't ever loop over a table with weak keys or values. Yes, entries including collected objects will automatically be removed from the table (as we saw at the end of the practical example), but the process is not instant. Loops might sometimes contain elements that you thought had already left the game.

Honestly, from my practical experience, you do rarely really need tables with weak references, especially when working with the object oriented approach. Coming back to the practical example from above, you would rather expect a pet to only have one external reference, which is the owning player (assuming that you also have an object representing the player). The pet name would rather be a property of the pet, i.e. saved inside of it. Consequently, nilling the reference coming from the player would immediately clear the single existing reference to the pet, thus collecting the pet and its entries, including its name.

Object-oriented Programming

Now, we get to the fun part. Lua is not an object-oriented language, but it does allow for object-oriented programming to some degree (more than enough for Wc3 modding). Switching my personal writing from functional programming to OOP has brought great joy to my modding experience. At the same time it made my code more organized and less prone to bugs.

Plus, the well-known sumneko.lua extension for VS Code offers great support for this paradigm.

This chapter will not teach you, what object-oriented programming is. It assumes you have already done OOP before and focusses on how the paradigm works in Lua, plus what to additionally consider in a Warcraft environment.

We will go through this chapter along an example:

Let's say we make a map, where players can own one pet each.

A pet is an animal and as such, it has a race (dog, cat, ...). Pets also have a name that was given by the player. Over time, they can build additional trust to the player.

We will fully annotate this example using Emmy Annotation. Annotations are crucial for Lua object-oriented-programming (or even for Lua in general), because they allow to keep track of all the types we create and the dependencies they have to each other.

Defining a Class

Classes are the core of every object-oriented work. In Lua, a class is simply represented by a table, which (in most cases) holds all default values for the class properties.

Let's start with the class AnimalRace. Let's say we only have Cats, Dogs and Spiders. By game design, we want our animal races to determine the model (appearance) of an animal, whether it can be tamed and the rate at which is builds trust to the player.

 Lua:

---@class AnimalRace
AnimalRace = {
 name = nil ---@type string name of the animal race
 , baseUnittypeId = nil ---@type integer this object id will be used for unit creation
 , models = {} ---@type integer[] holds all models available for changing unit appearance after creation
 , canBeTamed = false ---@type boolean
 , baseTrust = 0. ---@type number
 , trustRate = 10. ---@type number
}

Our first class is finished. It lists all properties that class objects are supposed to have. The values mentioned are default values that class objects can use later.

Note that we even included name = nil in the class, although from a technical perspective, this entry doesn't exist in the table. It's still good practice to mention all properties in the class, even if they have no default value. This makes the whole deal easier to understand later on. It's also required by VSCode's auto-complete features (i.e. it tells the sumneko-extension that name is a property of the class).

Creating an Object

The next step typically is to define a create-function for objects of the given class.

It (like all future methods) will be saved in the class, so we have to make sure to choose names not already taken by a property (to not overwrite anything). Name clashes between methods and properties are unlikely to occur anyway, but you should be aware of that they are possible.

The create-method is supposed to deliver a finished object ready for service.

That also involves the basic technical preparation: making sure that objects can read the default values from the class.

You might remember from the chapter about Metatables what we need for that: a metatable using the __index-metamethod.

This process is the same for all objects, so we only need one metatable with one __index-metamethod.

 Lua:

--Educative Example. There will be a shortened version below.
do
 local mt = {__index = AnimalRace}

 ---Define the create-method.
 ---One important design decision to make: which parameters do you require as input?
 ---I typically choose to require all properties that have no default value in the class. Maybe more, if sensible.
 ---@param name string
 ---@param baseUnittypeId integer
 ---@return AnimalRace
 function AnimalRace.create(name, baseUnittypeId)
 local new = {} ---@type AnimalRace (new objects are basically always empty tables)
 setmetatable(new, mt) --the most important part. Allows objects to fetch default values from the class.
 new.name = name --apply specified property to object
 new.baseUnittypeId = baseUnittypeId
 return new
 end
end

We usually take an additional shortcut at this point: Instead of creating a dedicated metatable for AnimalRace objects, we just use the class itself as a metatable. That means the class will both hold the __index metamethod and be the target of redirection.

Doing so doesn't really do much - we save one table creation and a few lines of code. It's still good practice.

 Lua:

--Shortened Version of the above:
AnimalRace.__index = AnimalRace

---Creates a new Animal Race.
---@param name string
---@param baseUnittypeId integer
---@return AnimalRace
function AnimalRace.create(name, baseUnittypeId)
 local new = {name = name, baseUnittypeId = baseUnittypeId}
 setmetatable(new, AnimalRace)
 return new
end

Creating static Objects

Some classes create and delete objects on demand, while other classes create objects once during initialization and keep them throughout the runtime of the program. AnimalRace is an example for the latter. You know that you only have Cat, Dog and Spider, so you create these objects exactly once during map init and they stay until the map has been finished playing. In other words, Cat, Dog and Spider are static objects.

For static objects, it can be necessary to make them public in some sort of way. In our case, we are probably going to define an Animal.create(fromAnimalRace) method at some point, so we should be able to fetch and pass the Dog object, which obviously requires its accessability.

We will now create our static objects and save them to some public spot (namely AnimalRace.repo) for later use.

Let's say dogs have a high base trust and low trust rate, and vice versa for cats. Spiders are not tameable, so they can't build trust at all.

 Lua:

--Create Animal Races
do
 AnimalRace.repo = {}

 --Dog
 local race = AnimalRace.create('Dog', FourCC('ndog'))
 race.canBeTamed = true
 race.baseTrust = 100.
 --trustRate is not specified, so it will default to 10.
 --models are not specified, but the class doesn't contain defaults. getModel-method must take care of this case later.
 AnimalRace.repo.DOG = race --save new race to public spot

 --Cat
 race = AnimalRace.create('Cat', FourCC('a001')) --custom created unittype
 race.canBeTamed = true
 race.trustRate = 15.
 --Cat base trust is 100 as per default in the class
 AnimalRace.repo.CAT = race

 --Spider
 race = AnimalRace.create('Spider', FourCC('nspb'))
 race.models = {FourCC('nspb'), FourCC('nspg'), FourCC('nspr')}
 --Spiders can't be tamed (which is default), so we are not interested in any of the remaining properties.
 AnimalRace.repo.SPIDER = race
end

Class Methods

A class usually provides functions that are supposed to be used by their objects, so called methods.

In case of AnimalRaces, we might consider to write a method that chooses a random available model (or a particular model, if we want to). Maybe the Animal class can use it later to create an animal with randomized appearance.

 Lua:

--A method must be stored in the class
--A better version of this code will be presented in the section about the colon-operator below

---Returns the i-th model available for this AnimalRace. If i is not specified, will return a random model instead.
---If no models are provided by the race, the baseUnittypeId will be returned instead.
---@param whichRace AnimalRace
---@param i? integer
---@return integer modelId
function AnimalRace.getModel(whichRace, i)
 local numModels = #(whichRace.models)
 --if no models are provided by the race, return the base unittype instead
 if numModels == 0 then
 return whichRace.baseUnittypeId
 end
 --Return model at position i, if i is specified (and whichRace.models[i] actually holds a model). If not, return random model.
 return whichRace.models[i] or whichRace.models[math.random(numModels)]
end

Note that this definition allows us to call the method directly on an object: Given an AnimalRace object x, we can write x.getRandomModel(x, i) instead of AnimalRace.getRandomModel(x, i), because the __index metamethod of the metatable of x redirects to the class.

But honestly, still having to pass x as first argument even upon calling the method on x is not exactly convenient.

This is where the colon-operator jumps in.

The : -Operator

The :-operator doesn't add additional functionality to Lua, but just adds syntactic sugar in the sense that it allows us to write certain things neater and shorter.

For object oriented programming, it simplifies both calling and defining methods.

Calling methods

Writing x:method(...) is equivalent to x.method(x, ...) (in fact, Lua resolves the first to the second).

This enables us to call methods on objects without explicitely passing them as a parameter again.

Given an AnimalRace object x, we can now write x:getModel(i), which is much more convenient than x.getModel(x,i) (although being functionally identical).

Quite obviously, both x.getModel(i) (dot notation, but forgetting to pass x as parameter) and x:getModel(x,i) (colon notation, but accidently passing x as first parameter) will lead to wrong results and most likely throw an error. This type of error happens quickly after starting with Lua OOP, especially when you come from a language that is solely using dot-notation.

In our case, x.getModel(i) would throw an attempt to index a number value error, because the function interior would attempt to evaluate i.models.

So watch out for this mistake in the beginning. You will get used to the right syntax over time

Defining methods

Given a class class, the following two styles of writing are equivalent:

function class:method(...) <doStuff> end

function class.method(self, ...) <doStuff> end
That means, using colon-notation basically adds a hidden parameter called self).

The hidden self-parameter refers to the object itself: methods literally always take a class object as first parameter, because they are designed to do something with them.

This is also what the two colon-operators for calling and defining methods have in common: They both hide the first parameter and assume it's the object itself.

Let's rewrite our method above using colon-notation:

 Lua:

--Old version for reference:
function AnimalRace.getModel(whichRace, i)
 local numModels = #(whichRace.models)
 if numModels == 0 then
 return whichRace.baseUnittypeId
 end
 return whichRace.models[i] or whichRace.models[math.random(numModels)]
end

--Below: Same method as above in colon-notation style.
--We don't specify the first parameter anymore and instead refer to the object via self-keyword. It's basically just one variable being renamed.

---Returns the i-th model available for this AnimalRace. If i is not specified, will return a random model instead.
---If no models are provided by the race, the baseUnittypeId will be returned instead.
---@param i? integer
---@return integer modelId
function AnimalRace:getModel(i)
 local numModels = #(self.models)
 --if no models are provided, return the base unittype instead
 if numModels == 0 then
 return self.baseUnittypeId
 end
 --Return model at position i, if i is specified (and whichRace.models[i] actually holds a model). If not, return random model.
 return self.models[i] or self.models[math.random(numModels)]
end

From now on, we will solely use colon-notation for methods.

Note that colon-notation only works for methods, not for properties.

If you want to access the baseTrust property of an AnimalRace object x, you still need to use dot-notation: x.baseTrust.

Further note that AnimalRace.create is not a method. Methods are functions used by objects to do something on them, but the create-function doesn't have an object to begin with (nor would it make sense to call it on an existing object). As such, using colon-notation with any class-create doesn't really make sense (although there are in fact applications for using create-functions with colon-notation, but we will not discuss those in this tutorial).

The same is true for all other functions that you store in the class, but don't use on class objects: You need to stick to the normal dot-notation for those!

Calling the Class for object creation

If you have programmed in other languages before, you are maybe used to just call AnimalRace() instead of AnimalRace.create() to create a new AnimalRace object. This syntax can be made available by using the __call metamethod:

 Lua:

--Insert this line below the AnimalRace class to enable calling AnimalRace() instead of AnimalRace.create()
setmetatable(AnimalRace, {__call = function(t, ...) return t.create(...) end} )

The class itself didn't yet have its own metatable (it was just used as one for its objects), so we created a new metatable for it providing a __call metamethod executing its create-function.

Note that upon using this style, the sumneko-extension for VSCode might get confused with return types and parameter preview. It doesn't seem to properly draw the connection between AnimalRace() using the __call metamethod and the return values of AnimalRace.create(). Apart from that, there is nothing wrong with this style of coding.

Connecting class objects and Warcraft objects

We often want to add properties to existing Warcraft types. The most relevant case for sure targets the players: We literally always want to save additional information per player based on the type of map we are making. In our example, we might want to save the pet that a player has chosen.

But adding properties to existing Wc3 players is not easily possible, because they are userdata, i.e. we can only deal with them via the Warcraft API.

We are still doing Object-oriented programming, so the next best solution is to create a custom player object per player that we identify with the player itself and that we can use to save custom player properties. For that to work, we also need a way of connecting the Wc3 player with the custom object in the sense that we are always able to get to the one starting at the other.

We will define a class CPlayer for this purpose and create one CPlayer object per player during map init. Each CPlayer will know its corresponding Warcraft player (as one of its properties). Additionally, we maintain a static table in the class that will save all (player,CPlayer)-pairs. This way, the two are connected. We can get from a CPlayer cp to its player by accessing cp.player and from the player p to its CPlayer by accessing CPlayer.fromPlayer[p].

It can also make sense to provide a static CPlayer array (i.e. containing (integer,CPlayer)-pairs), which would allow us loop over all CPlayers using ipairs.

You might be tempted to name the class Player instead of CPlayer, because Lua is case-sensitive and as such, there will be no name clash with the Warcraft type player.

There is another name clash, though: Naming our class Player would overwrite the Player-native (the one taking an integer and returning a player).

Hence we take CPlayer in this tutorial. Be careful and never choose class names that might overwrite existing code!

 Lua:

---@class CPlayer
CPlayer = {
 --object properties
 player = nil ---@type player The warcraft player
 , pet = nil ---@type Pet we will define this class later
 --static repository to retreive CPlayer from a given player
 , fromPlayer = {} ---@type table<player,CPlayer>
 --static array of CPlayers to loop over via ipairs
 , list = {} ---@type CPlayer[]
}

CPlayer.__index = CPlayer

---Creates a new Player object from an existing Wc3 player.
---@param fromPlayer player
---@return CPlayer
function CPlayer.create(fromPlayer)
 local new = {player = fromPlayer}
 setmetatable(new, CPlayer)
 return new
end

--Create CPlayer object for all Wc3 Players
--This is part of the Lua root and hence will be executed during early loading screen.
for i = 0, GetBJMaxPlayers() - 1 do
 --arrays start at index 1 in Lua
 CPlayer.list[ i+1 ] = CPlayer.create(Player(i))
 CPlayer.fromPlayer[Player(i)] = CPlayer.list[ i+1 ]
end

Let's see this connection in action:

Assume that pets have a name property that their owners can choose. We give players the option to do that via chat command. Typing -petname xxx into game chat shall change their pet's name to xxx.

 Lua:

do
 --Create Namechange trigger.
 --Don't do this in Lua root (remember to not create Warcraft objects in Lua root!).

 local function createNameChangeTrigger()

 --Create trigger for pet name change
 local trigger = CreateTrigger()

 --Add event for every player
 for _, cplayer in ipairs(CPlayer.list) do
 TriggerRegisterPlayerChatEvent(trigger, cplayer.player, "-petname ", false)
 end

 --Add trigger action.
 TriggerAddAction(trigger, function()
 local s = GetEventPlayerChatString()
 --make sure that -petname was at the beginning of the chat command
 if s:sub(1,9) == "-petname " then
 --retreive CPlayer from player, then get to the pet.
 local cplayer = CPlayer.fromPlayer[GetTriggerPlayer()]
 cplayer.pet.name = s:sub(10,-1)
 end
 end)
 end

 --Use Total Initialization resource.
 OnTriggerInit(createNameChangeTrigger)
end

We will discuss another approach for the sake of completeness, because it often comes in handy:

Instead of creating one single trigger that fires on every chat input, we could define one trigger per player. This would allow us to store the trigger as a property of the CPlayer object, allowing us to enable and disable the functionality per player on demand. It would also cut the necessity of using the CPlayer.fromPlayer lookup.

 Lua:

do
 --Create Namechange trigger.
 --Don't do this in Lua root (remember to not create Warcraft objects in Lua root!).

 local function createNameChangeTrigger()

 --Loop through all CPlayers, create one trigger for each.
 for _, cplayer in ipairs(CPlayer.list) do
 --Create a trigger per player
 cplayer.petRenameTrigger = CreateTrigger()

 --Add event for this particular player. The Wc3 native needs a Wc3 player, which we previously saved to cplayer.player
 TriggerRegisterPlayerChatEvent(cplayer.petRenameTrigger, cplayer.player, "-petname ", false)

 --Add trigger action.
 local cp_copy = cplayer --make a local copy not changing its value. Upvalue for the anonymous function below.
 TriggerAddAction(cp_copy.petRenameTrigger, function()
 local s = GetEventPlayerChatString()
 --make sure that -petname was at the beginning of the chat command
 if s:sub(1,9) == "-petname " then
 --We don't need to retreive the CPlayer from the class, because we can still access it from local scope
 cp_copy.pet.name = s:sub(10,-1)
 end
 end)
 end

 end

 --Use Total Initialization resource.
 OnTriggerInit(createNameChangeTrigger)
end

Inheritance

Let's continue with our example.

We will define the Animal class that can create animals from an existing AnimalRace.

Aside from their race, animals know their model (their ingame appearance, which corresponds to one of the models given in the race), their owner (which by default is the CPlayer of player neutral passive), their unit (which is the warcraft unit corresponding to the animal) and a boolean isTame (telling us, whether the animal has been tamed or not).

Some of these animals are Pets and as such, have additional attributes (their name and their trust to the player).

We will also create the Pet class and let it inherit the properties from Animal.

 Lua:

--First, create Animal class. Nothing special here, we have already learned all required knowledge.

---@class Animal
Animal = {
 --Mandatory Object Properties to be set in the create-function
 race = nil ---@type AnimalRace
 , model = nil ---@type integer
 --The unit is created on demand (if the Animal is actually spawned on the map)
 , unit = nil ---@type unit
 --Optional object Properties. Animal objects will use the class defaults, until setting their own value.
 , owner = CPlayer.fromPlayer[Player(PLAYER_NEUTRAL_PASSIVE)] ---@type CPlayer
 , isTame = false ---@type boolean
}

Animal.__index = Animal

---Creates a new animal from an existing Animal Race.
---@param fromRace AnimalRace
---@return Animal
function Animal.create(fromRace)
 local new = {}
 new.race = fromRace
 new.model = fromRace:getModel() --assign new animals a random model from the race
 setmetatable(new, Animal)
 return new
end

---Spawns this animal at the specified coordinates for the owner. Will not work, if it has already spawned.
---@param x number x-coordinate of spawn point
---@param y number y-coordinate of spawn point
function Animal:spawn(x,y)
 --Only create unit, if we haven't already.
 self.unit = self.unit or CreateUnit(self.owner.player, self.race.baseUnittypeId, x, y, 0.)
 BlzSetUnitSkin(self.unit, self.model)
end

This code allows us to create a new dog via u = Animal.create(AnimalRace.repo.DOG) and spawn it on the map via u:spawn(x,y). We still have to deal with questions similar to when we created the CPlayer class: where do we save u to prevent garbage collection? Do we need a way to retreive the Animal object from a given Warcraft unit? If so, a static table containing all (unit,Animal)-pairs would do the job (which could be updated in the spawn-method).

Fortunately, most class creations don't bring such problems. It's mainly units and players that need special treatment.

Now, Pets. They are a special kind of animal and as such, supposed to inherit all properties and methods from the Animal-class. We want the Pet class to be able to define new properties and methods, redefine existing ones from the Animal-class and simply take over everything else.

The technical solution is again smart use of the __index metamethod. Pets will be created in a way that they redirect to the Pet class, which in turn must redirect to the Animal class. That's basically it. One more thing, though: The Pet.create function must use Animal.create instead of creating an empty table. This makes sure we have prepared everything that an animal needs, before applying the special properties of Pets.

 Lua:

---@class Pet : Animal
Pet = {
 name = nil ---@type string
 , trust = 0. ---@type number
 , isTame = true ---@type boolean
}

Pet.__index = Pet --this will redirect from Pet objects to the Pet class

setmetatable(Pet, Animal) --this will redirect from the Pet class to the Animal class (Animal has __index pointing to itself)

---Creates a new Pet (by creating an animal and applying Pet specific properties)
---A Pet is owned by a
---@param fromRace AnimalRace
---@param forPlayer CPlayer
---@return Pet
function Pet.create(fromRace, forPlayer)
 local new = Animal.create(fromRace)
 new.owner = forPlayer
 new.trust = fromRace.baseTrust
 setmetatable(new, Pet) --switch metatable from Animal to Pet.
 --also make sure that the CPlayer connects to his pet after creation:
 new.owner.pet = new
 return new
end

---Increases a pet's trust to the player by the rate specified by the AnimalRace
function Pet:increaseTrust()
 self.trust = self.trust + self.race.trustRate
end

Note that Pet.create (in contrast to Animal.create) takes a CPlayer as an additional parameter. Why not the Warcraft player, you might ask? Well, both is theoretically possible, but taking a CPlayer is much cleaner. It directly holds all custom properties that we need and you can get to the Warcraft player by simply accessing the .player property.

Try to always pass your custom objects to your functions and only get to the Warcraft objects upon using the Warcraft API.

This is a clean design rule and makes sure that you rarely run into bugs with wrong arguments.

Further note that we haven't actively changed the isTame-property of any Animal object. Instead we have chosen the defaults of the Pet and Animal class in a way that let pets always have isTame = true and non-pet animals isTame = false.

This allows you to design and use properties with very little effort.

Objects and tostring

If you are like me, you will probably use Ingame Console to debug your code. I often find myself printing properties of specific objects to the console, just to see if they have the right type or not. Consider the case where you have an animal u and you want to check, if u.race actually holds an AnimalRace or not. Printing u.race to the console would result in table: <hexCode>, which could be anything really. I prefer to see AnimalRace: <hexCode> at this point, which we can achieve by using the __name metamethod of the object's metatable (i.e. the class).

The chapter about metatables has its own section about the nuances of this metamethod, so we will just present finished code here:

 Lua:

AnimalRace.__name = 'AnimalRace'

I usually apply this metamethod to every single class, as it simplifies debugging processes and doesn't have any downsides.

A Word about Garbage Collection

We learned in the chapter about Memory Leaks that the Lua garbage collector deletes an object from memory as soon as there is no active reference to it anymore.

Objects in OOP typically have a lot of references, so it can be hard to clear every single one to make an object garbage collectable.

So always remind yourself during coding: It's good practice to have as little references as possible.

Maybe you want to add a Hero class to the example from above. A hero is supposed to know its pet and the owning cplayer. But wait, wouldn't that involve unnecessary circularity? cplayer, pet and hero would all know each other. We can do better! Consider just connecting cplayer and hero (i.e. cplayer.hero = hero and hero.owner = cplayer). If you want to get to the pet starting from the hero, you can still do so via hero.owner.pet.

Essentially, try to never have more than one possible way to go from one object to another, no matter which two objects you choose (if they are connected at all).

That way, you can rely on the classical garbage collection chain reaction.

If you have an object A with A.property = B and B knows another object C which in turn knows object D, you can simply do A.property = nil to let the garbage collector collect B, C and D (assuming you followed my suggestion and had no other references to these objects).

Static properties (like the CPlayer.list table) play a special role at this point. They provide additional references to an object by design, so you also need to actively clear those references to make the object garbage collectable.

Well, you might instead bypass this manual reference cleaning by making the static table hold weak references (see chapter about Memory Leaks for further information). This way, you wouldn't have to care about them any longer.

You obviously shouldn't do that, if the static table is holding the only reference to any object (of course). Also don't make a static table hold weak references, if you plan to loop over it (might cause desyncs).

coming soon...

### Useful resources

### Differences between JASS and Lua

### Desync Causes

There are plenty of Lua resources on the web (and on Hive) that are worth a look. I try to summarize what helped me the most, when I learned Lua in the last few years, so this is merely a loose collection of links.

Learning and using Lua

Programming with Lua 4th Edition

The book that I used for learning Lua. It's very educative, well-designed and perfectly suited for programmers on an intermediate level. It does of course include information that is irrelevant for the context of Wc3 mapping, but I can still highly recommend it. The book goes into much more detail than this tutorial.

Lua 5.3 official documentation

The official documentation overlaps quite a bit with the book mentioned above. It's not a good tutorial, but the go-to-place, when you have questions about Lua's incorporated functions and libraries.

Note that when following the link, Lua's native functions are listed on the bottom of the page.

Lua Cheat Sheet

This cheat sheet provides a short overview of the basic syntax of Lua code, like declaring variables, loops, conditions, functions, etc.

I had opened it at all times on my second monitor, when I was learning Lua.

Emmy annotation

Without any intend of exageration, I think using Emmy annotation with the sumneko.Lua-extension for VSCode has reduced my bug-rate by something around 90% (not to speak of my frustration rate ).

Emmy annotation provides a way of documenting the type of function parameters and thus compensates for Lua's missing type safety.

Using Emmy annotation will save you many many hours that you would have spent on debugging your code. Trust me, even the slightest mistake (like passing a player number instead of a player to some function) will go unnoticed and silently break your code, if you don't use a tool like this.

Online Lua Execution

A website to execute Lua code online. Doesn't support warcraft-natives of course, but well suited to learn the language itself.

I used this a lot in the beginning, when I was unsure about any of the basic concepts. Typing a few lines of code into this online box is just much quicker than starting Warcraft.

Switching from vJASS to Lua

An excellent read for people who think in vJASS and write in Lua. This guide helps you translating known concepts into the new language and answers all your How-do-I-[Insert vJASS concept]-in-Lua?-questions.
Development tools

Visual Studio Code or VSCodium

Your main development environment for writing your Lua code. Use together with sumneko-extension and the Warcraft definition files attached to this post.

Ceres or cheapack or Dencer.warcraft-vscode

Tools to inject your Lua code directly into your map (plus other features) to save you the annoying copy-pasting route from VSCode to trigger editor.

This is something for advanced users.

cjass2lua or vjass2lua-webapp

Converters for Jass -> Lua. The first provides a downloadable .exe and supports Jass and cJass, the second is a webapp and support Jass, vJass and Zinc. Both solutions add Emmy Annotation to the generated code.

You can either use these converters to continue your existing JASS map in Lua or to learn something by looking at generated code. Note that converters are never flawless and the resulting code will not be optimized.
Lua code resources on Hive to import into your map

The following list links to Lua code resources specifically made for mapping in Warcraft that I consider mandatory for every project. Importing them basically only requires copy-pasting their code into your map. They will save you a lot of time.

Total Initialization

Allows you to execute functions at different times during map loading screen, which otherwise would require GUI-triggers to accomplish. Especially useful for trigger creation, as Wc3 in Lua-mode is lacking the "Run during Map init" checkbox for scripts that was present in JASS-mode.

Lua Debug Utils

Debugging in Warcraft 3 can be quite a pain without proper debugging tools. Lua's debug-library is deactivated in Wc3, but this thread provides the tools you can work with instead. One of the most relevant features is the automatically generated error messages upon running erroneous code.
Data Structures

SyncedTable

Only relevant for multiplayer maps. It allows you to use the pairs-loop without risking a desync.

I personally find it really hard to avoid that kind of loop, so I do need SyncedTables really often during coding.

SetUtils

A data structure that can hold any element, but no element more than once. Exactly like unit groups, but not limited to units.

Features object-oriented notation, multiplayer-synced iteration, the ability to create Sets from unitgroups and a lot of other convenience.

Definitive Doubly-Linked List

A well-made Linked List implementation in a circular, doubly-linked style. Features object-oriented notation and a nice generic for-loop.

Multidimensional Table

Allows you to create tables supporting multiple indices (like T[1][2]["bla"]) without manually preparing any subtables.
The Warcraft-API

Common.lua, Blizzard.lua and HiddenNatives.lua

Emmy-annotated files to use in your IDE (not in your map!) are attached to this post.

Jass Documentation Database

A website hosted by @moyackx listing all Wc3 natives. Users like @Luashine have contributed valuable additional information in the comments (same is true for Jassdoc below).

Jassbot

A search engine for @LeP 's Jassdoc on Github listing all natives with annotations and documentation. You can help to improve it by contributing your knowledge.

This chapter will point out things in Lua that were different in JASS.

World Editor

Trigger documents in the Trigger editor lose their "Run on Map Initialization"-checkbox.

Don't use those any longer. Instead, rely on Script documents.

As a consequence of the bullet above, triggers will not automatically be created. Use Total Initialization instead.

World Editor dedicated syntax check (Ctrl + F10) is bugged in Lua-mode. You get a proper syntax check upon saving the map, though (Ctrl + F9).

Lua syntax check will not check function parameter types (because those don't exist in Lua).

WE builds your map script (war3map.lua) from your script files exactly in the order they have in the trigger editor (top first, bottom last), which is very useful to manage dependencies (like if you are importing external resources, always put them on top). No more dependency to the order of trigger creation.

Your whole map script will be executed once during early loading screen. This will basically define your functions (because that's what your code does) and does not mean that functions will immediately be executed.

You can execute arbitrary Lua code at this point by not including it into a function body (but just writing it to the Lua root).

Don't create Warcraft objects at this point. Many natives don't work properly yet and you risk desyncs!

World Editor can crash, if you use the % character anywhere in your map script (even inside strings and comments!!). Don't use %!!! The character is used as modulo operator and Lua pattern matching. See Chapter about Strings and Numbers for workarounds.

Syntax

Keywords set and call are no longer required (nor supported).

endif and endfunction become end.

Comments start with -- instead of //.

You can use block comments, which start with --[[ and end with ]].
(more syntax changes in the sections below)

Variables and Types

Variables don't have a type, only values have.

You can assign different typed values to the same variable.

null is now nil

Global variables can be assigned anywhere without previous declaration (you don't need a globals-block).

Accessing uninitialized globals returns nil.

Local variables must be declared, but can be declared anywhere, not just on the top of a function body.

Functions

Functions became first-class-values. They can be saved into (local) variables, passed to other functions, etc.

Functions don't support parameter types. Parameters are always generic.

Function order in your map script doesn't matter anymore (except for local functions).

Functions support multiple return values, optional parameters (kind of) and unlimited arguments (by use of the ...-operator).

Anonymous functions are possible.

Callbacks are possible in every function (i.e. passing a function to another function and execute it there).

You don't need the function-prefix for passing functions anymore, i.e. TriggerAddAction(trigger, function exampleFunc) becomes TriggerAddAction(trigger, exampleFunc).

Native Warcraft 3 functions can be overwritten.

Data Structures

Instead of arrays and hashtables, we only have one container-type remaining: tables

Tables hold (key,value)-pairs. Both key and value can have any type except nil

You can even use Wc3 objects as key, so GetHandleId becomes obsolete.

By convention, Lua arrays (-> tables only using integer keys) start at index 1.

You can hold values of different types in the same table and hence Lua-array (unlike JASS-arrays)

Lua tables support multi-dimensionality (i.e. T[1][2]["a"]) by nesting 1-dimensional tables.

Lua tables don't suffer from the 128-hashtable-limit (but Wc3 hashtables still do - don't use these anymore!)

Loops

Instead of just the while-loop, Lua also got the repeat-until-loop, numeric-for-loop, dedicated table-loops (pairs and ipairs) und the fully customizable generic-for-loop.

Lua also got the goto-statement.

Arithmetics

Reals (in Lua: float) and integers have a common parent type number and can be included in the same calculation without explicit conversion.

As a consequence of the above, dividing two integers will now result in a float (i.e. 3/2 results to 1 in JASS and to 1.5 in Lua.

You can still use integer division with the // operator (even on floats), i.e. 3//2 == 1 in Lua.

The equality-relation doesn't have any tolerance in Lua.

I.e. 1. + 0.1 - 0.1 == 1. was true in JASS and false in Lua.

This can cause bugs, if you convert resources or maps from JASS.

Lua provides a modulo-operator x % y (but you shouldn't use the percent-char, as stated in the World Editor section).

Strings automatically convert to numbers when used in any mathematical operation. You don't need S2I and S2R any longer.

Strings

String concatenation is now done with s .. t instead of s + t.

Numbers automatically convert to strings, when concatenated. You don't need I2S and I2R any longer.

Strings can be engulfed with either single or double quotation marks, which escape each other.

As a consequence of the above, 'hfoo' is a string in Lua and will not represent an integer object id. If you need the latter, use FourCC('hfoo') instead.

s .. nil will now result in nil, while in JASS it resulted in s.

Garbage Collection

Lua-objects will get automatically garbage collected (removed from memory) once all references to them have ceased.

Warcraft objects still need to be manually removed.

Other

We now have access to many powerful build-in Lua library functions (such as the string-library).

The inequality operator became ~= instead of !=.

if-conditions support any value, not just booleans. false and nil count as false, everything else counts as true (even the number 0).

Same is true for the logical operators and and or, which further allow you to conduct several programming tricks, simulating coalesce, Ternary Operator etc.

Lua's print-function supports arbitrarily many arguments of any type (not just strings).

Using tostring on any Warcraft object will output a string including the Warcraft type.

There is no op-limit in Lua.

Hidden natives and some natives from common.ai are available for use without previous declaration.

GetHandleId is not synchronous among players anymore.

You don't need that function anymore anyway, because you can use every object in a table (no need to get its handle id first).

Warcraft escapes two %-characters to one, so you would need to write x %% y for the modulo operator (but as stated above, you shouldn't use this character at all anyway)

Lua specific Desync causes

Lua has several additional desync sources that JASS didn't have. That's not a big issue as long as we know what we can do and what not. The clues below were already included in the other chapters, but this shall serve as a summary.

I will also try to suggest what you can use instead of the desync-causing elements.

Quite obviously, you can ignore all of this, if you are creating a singleplayer game.

This collection is work-in-progress, so please contribute in case you have additional knowledge.

Asynchronous Functions

The following functions are asynchronous in Lua, i.e. can have different outputs for different players. They desync, if and only if you use them to change game state.

Asynchronous ElementSuggestion

GetHandleId()GetHandleId is synchronous in JASS-mode, but asyncronous in Lua-mode.

Solution: Don't use it in your map. It's simply not useful in Lua, so there is absolutely no reason to rely on it. The main purpose in JASS was to convert an object to an integer to use it as a key within hashtables or other structures. That's not necessary to do in Lua anymore, because you can use any object as a table key directly, which is exactly what you should do.

Exception:GetHandleId is in fact synchronous on some static objects. E.g. GetHandleId(ARMOR_TYPE_METAL) will always return 2.

pairs()pairs is a powerful iteration method for tables with general indices, but its iteration order is not guaranteed to be the same for every player in a multiplayer game.

Solution: Either don't use pairs OR (if you find the pairs-iteration as convenient as I do) use SyncedTable. The library allows you to create tables for which the pairs-iteration is synchronous for all players. Which means, whenever you create a table which you want to use pairs on, you need to create it via T = SyncedTable.create() instead of T = {}.

Sidenote: The pairs-iteration can be customized by using the __pairs-metamethod, which is exactly what SyncedTable does to make it synchronous. It's definitely async for tables without a metatable, though. If you imported a data structure from Hive into your map and want to use pairs on it, better ask the creator if he or she has made it synchronous before risking anything.

next()next is the iterator function used by (non-customized) pairs. It's asynchronous and exactly the reason why the same holds for pairs.

Solution: Don't use it for iteration (which you wouldn't do anyway, because it's just more cumbersome than relying on pairs).

You can however use it to check, if a table is empty or not: next(T) returns nil if and only if T is empty. That behaviour is the same for all players, so you can safely check for next(T) == nil in a multiplayer game.

#TableUsing the #-operator is safe on sequences (arrays without holes), but has undefined behaviour on tables with general keys, so the output might be different for different players.

Suggestion: Use # on sequences only. If your table contains holes or non-integer keys, better count the number of elements on your own.

tostring()tostring is synchronous on primitive data types (Booleans, Numbers, Strings, Nil), but asynchronous on non-primitive data types (Tables, Functions, Warcraft Objects, ...), because it translates them to <type>: <hexCode>, where <hexCode> refers to the object's memory location on player's local machines.

Solution: You will usually only use tostring on primitive data types and/or include it in strings that you display to the players. That is always fine, because printing strings doesn't change game state and thus doesn't desync.

Just avoid doing anything fancy with tostring(nonPrimitiveObject).

Sidenote: Just like pairs, the behaviour of tostring can be customized on tables by using the __tostring metamethod.

print()print applies tostring and consequently can show different values to each player.

Although it's technically async, there is absolutely no possibility of changing the game state by using print. That means, it's safe to use.

os.clock()

os.date()

os.time()Functions taking date and time information from your operating system.

Avoid using those except for maybe debugging and performance analysis.

Other Desyncs

The actions below can also cause desyncs in Lua-mode and should be avoided.

ActionSuggestion

Creating Wc3-objects in Lua rootCredits to @Tasyen for this finding.

The Lua root is executed during early map loading screen, where not all elements of Wc3 are yet fully loaded.

Using natives at this point can yield unexpected behaviour and creating Warcraft objects (like triggers) can lead to desyncs.

Suggestion: Only use pure Lua-code (like defining data structures) in the Lua-root.

If you need to use natives, make sure they are safe and don't create Wc3 objects with them. Honestly, I solely use the Player(i) native at this point, nothing else.

If you want to create Wc3 objects during loading screen, do it at a later point using Total Initialization.

FlushChildHashtableI observed several desyncs after using FlushChildHashtable, potentially related to using GetHandleId for hashtable keys.

Suggestion: Don't use Hashtables. Seriously. Lua Tables are strictly superior and there is absolutely no reason to rely on Wc3 Hashtables anymore.

CreateTimerBJ

StartTimerBJ

GetLastCreatedTimerBJBlizzard.j unfortunately creates a timer bj_lastStartedTimer during Lua root, which is dangerous as mentioned above. The variable reassignment happening within StartTimerBJ and CreateTimerBJ can lead to a garbage collection related desync (read more here). GetLastCreatedTimerBJ uses the same variable, so its use is also discouraged.

Solution: Use the non-BJ functions CreateTimer and StartTimer instead. Save your timer to a variable instead of using GetLastCreatedTimerBJ.

Synchronous Functions

It's also worth pointing out some synchronous functions just to avoid confusion.

You can use these without fearing desyncs:

math.random(as long as you don't use it in GetLocalPlayer-blocks)

math.randomseed(as long as you don't use it in GetLocalPlayer-blocks)

ipairs

#### Attachments

Blizzard.1.33.v2.lua

 452.4 KB

 · Views: 787



HiddenNatives.1.33.v2.lua

 5.4 KB

 · Views: 693



common.1.33.v2.lua

 369.6 KB

 · Views: 702



1707081977214.png

 4.4 KB

 · Views: 234




 Last edited: Jun 28, 2026

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

The guide above will probably be further improved in the future (although it is in a pretty exhaustive state already).

Please contribute with your own knowledge, if you find something that is still missing.

The chapters "Advanced Concepts - Memory Leaks" and "Appendix - Desync Causes" require community proofread, so feel free to provide feedback about these or literally anything else. Any feedback is welcome.

Also please suggest Lua resources from the hive that you feel should be spotlighted within this guide, in addition to those already present. The only requirement (apart from high resource quality) is that they must be related to Lua-programming/mapping on a basic level.

For instance, @chopinski 's Relativistic Missiles is of excellent quality, but not exactly related to Lua-programming apart from being written in the language.


 Last edited: Oct 3, 2022

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

This is the absolute best tutorial on Hive, and should help lots of people.

I recommend expanding on the Global Initialization portion, as I updated it last month to introduce an OnLibraryInit.

OnLibraryInit forces that initialization block to sleep until all of the designated API exists. Therefore, it doesn't matter which order the triggers are in (aside from GlobalInitialization itself), and the top libraries also do not require any special declarations (as OnLibraryInit is only checking the _G table, rather than some fancy internal structure).

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Thanks Bribe,

I will give OnLibraryInit a read next weekend and include it here

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

Absolutely the holy grail of Lua, just marvellous. Truely deserving of a COWABUNGA.

I noticed an error. When you give examples on "Lua fundamentals" > "Logical operators" > "Behavior on non-boolean values", your first "print(nil or 2)" should be AND and not OR, since the following explanation implies so.

I even learned some few things on lua fundamentals, just when I thought I knew them before

You could elaborate a bit more on anonymous functions and how they shouldn't ever be used in periodic loops (periodic timers), whether in BoolExpr's or not, they still leak and the gc can't keep up cleaning them all if we keep pressuring it constantly, just like objects (tables).

If only ... someone made a table recycler so these kinds of leaks stop existing

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Wrda said:

 Absolutely the holy grail of Lua, just marvellous. Truely deserving of a COWABUNGA.

I noticed an error. When you give examples on "Lua fundamentals" > "Logical operators" > "Behavior on non-boolean values", your first "print(nil or 2)" should be AND and not OR, since the following explanation implies so.

I even learned some few things on lua fundamentals, just when I thought I knew them before

You could elaborate a bit more on anonymous functions and how they shouldn't ever be used in periodic loops (periodic timers), whether in BoolExpr's or not, they still leak and the gc can't keep up cleaning them all if we keep pressuring it constantly, just like objects (tables).

If only ... someone made a table recycler so these kinds of leaks stop existing

Click to expand...

The periodic timer that would be problematic would be 0.01 seconds, and to my knowledge you also have to be creating LOADS of Lua objects within those 0.01s timers in order to overload the garbage collector. It needs more thorough testing. 0.01s repeating timers were not used in JASS, I think the lowest I saw in a common system was 0.025, which was in XE.

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

Splitting up chapters in tables breaks CTRL+F. Then it can't be used as a quick reference. I don't like the structure. It'll be still too hard for beginners and too dense in some parts for regular programming.

WC3 is a hostile platform for programming, especially beginners. No proper console output or error reporting by default. The iteration time (code-test) increased ten-fold due to Reforged's startup time. The hardest part for me in WC3 was not Lua or Jass, but lack of any understanding about code logic structure and API (condition-trigger-action, lockstep model with GetLocalPlayer, what are the API options?)*. Of course beginners will have to learn a new language to speak on top of that.

If you don't want to drown in praise, there's one control question I have for you: How long will it take a beginner from opening this page to creating the first tiny test map? If the long text killed motivation by boredom, the beginner will not come back soon. (I will send a book example in DM)

PS: There really is a lot of info. My greatest issue with it is structure and presentation.

* this is why I started working on jassdoc


 Last edited: Sep 21, 2022

#### sadathy

#### sadathy

 Level 5

JoinedJul 27, 2015Messages39

Resources: 1

#### Resources
Ported Models 1

Thank you for posting this wonderful resource. I'm glad you included the section on desyncs because that stuff boggles my mind and is also much more of a pain to test.

Edit: I've noticed that in the guide you have mentioned downloading the attached blizzard.lua and common.lua files. However they don't appear to be attached so I thought I'd let you know - I am aware these are attached to your other post about debug utils

Edit2: In the Executing Code section of Getting Started, your example code for using Bribe's Global Initilization is wrong. The function should be 'OnTrigInit(function)' rather than 'OnTriggerInit(function)'.


 Last edited: Sep 21, 2022

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Thanks for the feedback, guys, much appreciated!

@Bribe@Wrda

When I understand correctly, the leaks resulting from gc under high pressure would only happen in unrealistic scenarios like 0.01s periodic timers, in which case it's probably not necessary to write about that in the guide. But there are for sure plenty of other problematic scenarios that are worth mentioning. For instance, @ScrewTheTrees said in this post that conditionfunc's leak in general, but the leak size increases for anonymous functions. Not sure, if that statement is still up to date. I agree with Bribe that we justneed more testing. Has ScrewtheTrees actually published results from his tests on hive?

Wrda said:

 your first "print(nil or 2)" should be AND and not OR


Click to expand...

sadathy said:

 The function should be 'OnTrigInit(function)' rather than 'OnTriggerInit(function)'


Click to expand...

Good catch, thanks. Main post updated.

sadathy said:

 Edit: I've noticed that in the guide you have mentioned downloading the attached blizzard.lua and common.lua files. However they don't appear to be attached


Click to expand...

Will work on 1.33 files on the weekend and attach them asap

Luashine said:

 Splitting up chapters in tables breaks CTRL+F.


Click to expand...

True, but I don't see a solution for that issue. Ctrl+F search requires text visibility and there is just too much of it to make everything visible. Tasyen used Spoiler-tags instead of tables in his big frame tutorial, but they have the same problem.

From my perspective, a long tutorial like this primarily needs an intuitive chapter and section structure. If people manage to navigate to the right chapter, they can search for their issue. I'm quite happy with the current one, but I'm looking forward to your opposite view

Luashine said:

 It'll be still too hard for beginners and too dense in some parts for regular programming.


Click to expand...

Good point. Do you have any suggestion of how this could be improved? I have been thinking about marking more sections as advanced content (like most parts of the Logical Operators chapter) to help beginners skipping through more stuff on their first read-through and getting quicker results. I could also hide those sections within a Spoiler tag or stuff like that.

Luashine said:

 The hardest part for me in WC3 was not Lua or Jass, but lack of any understanding about code logic structure and API


Click to expand...

Understanding the Warcraft API is not the scope of this tutorial. I think, Warcraft mapping usually starts with fiddling around in GUI to create small test maps (which I'd recommend to every beginner before switching to Lua), so people are probably aware of the Wc3 basics (including how triggers work) before needing Lua input. From that point on, you basically need GUI-into-Code-conversion to find the right natives. I can include a section about that, but it would be a short one.

Luashine said:

 If you don't want to drown in praise, there's one control question I have for you: How long will it take a beginner from opening this page to creating the first tiny test map?


Click to expand...

The guide targets people who have basic programming knowledge already (like the big audience of JASS coders currently present in the Wc3 community) and I still assume readers have at least played around in GUI before. Given a "beginner" with this kind of preparation, I do believe walking your first steps in Lua-mapping is absolutely possible within a relatively short timeframe. You certainly need some days to learn Lua fundamentals, but there is no shortcut around this.

I will take some time on the weekend to look for sections within Lua fundamentals that I can recommend to skip on first read, though. Maybe that will help to get quicker results and this keep people motivated. Other ideas welcome. What do you think?


 Last edited: Sep 23, 2022

#### HerlySQR

#### HerlySQR

 Level 25

JoinedJun 26, 2020Messages1,974

Resources: 2

#### Resources
Maps 1Spells and Systems 1

I don't remember who and where told me this, but the message of errors that happens in the Lua root (those that returns to the W3 menu) appears in Documents/Warcraft III/Errors, you could add that.

If someone is curious, here is a case where someone wanted to use hashtables or GetHandleId in Lua: Typecast objects to string and viceversa


 Last edited: Sep 25, 2022

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

Eikonium said:

 CTRL+F not working


Click to expand...

Click to expand...

True, but I don't see a solution for that issue.


Click to expand...

Yeah the forums aren't too great. Xenforo has a huge issue: it does not allow for page #anchors in links (that's how you jump to a point on a page). With those you could've created chapter markers in a single message.

My only suggestion would've been to create a new thread and reserve the first 5-7 posts for your guide. Although I've seen your guide on its first day, I was too late and people already replied.

Spoilers are helpful if there're fewer than in Tasyen's tutorial  I don't mind opening 5-10 spoilers if I needed to.

Eikonium said:

 It'll be still too hard for beginners and too dense in some parts for regular programming.


Click to expand...

Click to expand...

Do you have any suggestion of how this could be improved? I have been thinking about marking more sections as advanced content (like most parts of the Logical Operators chapter) to help beginners skipping through more stuff on their first read-through and getting quicker results.


Click to expand...

One: style. Of course I like to think that my own style is the easiest to understand (I tried and improved in the past 2 years with one major success in a different community). Ok, now looking back at the function doc I thought to have nailed, it jumps from one point to another: Doc - GetLocalPlayer

I try to keep the language simple, short (i.e. avoid long language constructs unless I get carried away) and rich with examples. I haven't tried yet, but I think a good principle would be to always explain something in two different ways. Maybe start with a real-world analogy (grasp the basics) and then follow up with a proper explanations - where applicable.

The book I told you about; the author says that he explains in a spiral: first explanation is a simple one, but later iterations in the book are more advanced. Sounds reasonable. He also had a huge advantage of being a professor, so he had real feedback where students struggled.

What struck me in your explanations are deviations into details that could've been left out for beginners (or moved to Advanced as you suggested). There're really two different kinds of scripts needed: one to teach, one as a manual. A manual can be detailed (as long as it doesn't read like the C or POSIX standard), a tutorial should be not as dense.

Eikonium said:

 Understanding the Warcraft API is not the scope of this tutorial. I think, Warcraft mapping usually starts with fiddling around in GUI to create small test maps (which I'd recommend to every beginner before switching to Lua), so people are probably aware of the Wc3 basics (including how triggers work) before needing Lua input. From that point on, you basically need GUI-into-Code-conversion to find the right natives.


Click to expand...

I think this is a barrier. When I look at GUI screenshots of people, I have no idea what's going on there. My brain cannot translate "GUI->code functions" but if I looked at code, I could look up various functions to understand it.

Bottom line: There really isn't much I can suggest, because most of my objections is a matter of style, taste and preference. I don't know if my style would be easier to understand for people reading.

I like learning motivation / reasoning of particular decisions though (here: language features), because it "reinforces" the piece of knowledge you just read. For example for local variables you wrote "This fact makes them resistent against name clashes, which is why they should be preferred over global variables whenever possible." but another reason to always prefer local variables, is because they keep code and variables close and when you need to change them, you don't need to worry that a global variable is used in 300 other places.

I like real examples. Not real in the sense of looking at a 100-line function, but a simple enough example to demonstrate a useful purpose.

 Function Basic Syntax

 Lua:

function foo(x,y)
 return (x+y)/2.
end

function bar(x,y)
 print(foo(x,y))
end

bar(10,20) --> prints "15"

Click to expand...

Do you see what it looks like? The area of a right-angled triangle. And suddenly the example makes more sense and is memorable:

 Lua:

function triangleRightArea(width, height)
 return (width + height) / 2.
end

function showTriangleArea(w, h)
 print(triangleRightArea(w, h))
end

showTriangleArea(10,20) --> prints "15"

But you can literally turn it into text understandable to anyone:

 Lua:

function triangleRightArea(width, height)
 local area = (width + height) / 2.
 return area
end

function showTriangleArea(w, h)
 local area = triangleRightArea(w, h)
 print(area)
end

showTriangleArea(10,20) --> prints "15"

Is it better in this example? Debatable, turns short code into a wall of text (almost). Good for very early introduction, bad for later use.

Variable names. Here's the only place I'd strongly suggest to use self-explicable variable names over comments. (oh and mathematicians are stupid for using single letter variables everywhere!)

Pair reading. I'd offer a pair-proof-reading session in voice if I had time to. Maybe you'll find someone who will read it for the first time to learn the topic, except together with you? This way you could better understand the hard spots and refine them.

#### HerlySQR

#### HerlySQR

 Level 25

JoinedJun 26, 2020Messages1,974

Resources: 2

#### Resources
Maps 1Spells and Systems 1

I think that someone could create a "Lua Map Template", similar to the The Ultimate Terraining Map 4.0, but this contains the main Lua resources, and also a REPO to work in VSCode that also contains those resources and the blizzard.lua, common.lua and common.ai.lua to work with the sumneko extension, I could do it myself, but I think is better that someone with the 1.31 version (that I think is the version where Lua was introduced) so everyone can use it.

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

I just finished a big update of the main post.

Changes:

Attached definition files for common.lua and blizzard.lua as per Warcraft version 1.33.0.

These definition files were converted from common.j and blizzard.j by using cJass2Lua.

I also altered annotations in both files to increase compatibility with VSCode's sumneko-extension (the old ones gave a lot of warnings with sumneko's current version).

Most importantly, I've completely replaced the types real and code by number and function (as there is no point in keeping Warcraft types that are identical with existing Lua types) and removed the return-type nothing. I've also added and removed a lot of blank lines to make the right comments stand next to the right functions (to make them show up in tooltips).

Actions in Blizzard.lua:

Regex Find:

"---@Return \r?\n" (added return type minimapicon/commandbuttoneffect for some functions, where this was missing)

"debugBJDebugMsg" (removed "debug". This was wrongly parsed from "debug call BJDebugMsg")

"MeleeGetCrippledRevealedMessage" (replaced + by .. in the function definition)

Regex Find+Replace:

"\s(\r?\n)" -> "$1" (clear all trailing whitespace)

"---@Return nothing\r?\n" -> "" (removed return value nothing from all annotations)

"---@param (.*) real" -> "---@param $1 number" (replace real by number annotation)

"---@Return real" -> "---@Return number" (replace real by number annotation)

"---@type real" -> "---@type number" (replace real by number annotation)

"---@param (.*) code" -> "---@param $1 function" (replace code by function annotation)

"---@param (.*) boolexpr" -> "---@param $1? boolexpr" (made boolexpr arguments optional)

"(-- .*\r?\n)--\r?\n\r?\n" -> "$1" (removed blank line between some functions and their comments to make them show up in tooltips)

"(-- =+)" -> "\n$1\n" (adding blank lines around separator comments to prevent those from showing up in tooltips)

Other actions:

Made string params optional in PickMeleeAI

Actions in Common.lua:

Regex Find+Replace:

"\s(\r?\n)" -> "$1" (clear all trailing whitespace)

"---@Return nothing\r?\n" -> "" (removed return value nothing from all annotations)

"---@param (.*) real" -> "---@param $1 number" (replace real by number annotation)

"---@Return real" -> "---@Return number" (replace real by number annotation)

"---@param (.*) boolexpr" -> "---@param $1? boolexpr" (made boolexpr arguments optional)

"(-- .*\r?\n)--\r?\n\r?\n" -> "$1" (removed blank line between some functions and their comments to make them show up in tooltips)

"--\r?\n" -> "" (remove comments without content)

"(-- =+\r?\n)(-- .\r?\n)+(-- =+\r?\n)" -> "\n$1$2$3\n" (engulf API comments by blank lines to prevent them from showing up in tooltips)

"(^function .* end.*\r?\n)(?!\r?\n)" -> "$1\n" (adds blank line below functions that don't already have one)

Other actions:

Made string params optional in StartMeleeAI

Made timer param optional in CreateTimerDialog

Also attached a definition file for Wc3 hidden natives (including some of the really hidden ones and a few from common.ai). I tried to include the most relevant stuff (FourCC, __jarray, UnitAlive, ...).

Packed several paragraphs from Lua Fundamentals into Spoiler tags marked as Further reading. This shall enable readers to skip advanced information, until they are ready for it.

Included tons of small improvements into the different chapters. Special thanks to @Luashine for PM'ing me all the results of her proof-reading, much appreciated!

Revised chapters about Debugging and Wc3 Setup. Thanks to @HerlySQR (for the hint on the log file) and @Luashine.

Next: @Bribe's OnLibraryInit.

HerlySQR said:

 I think that someone could create a "Lua Map Template", similar to the The Ultimate Terraining Map 4.0, but this contains the main Lua resources, and also a REPO to work in VSCode that also contains those resources and the blizzard.lua, common.lua and common.ai.lua to work with the sumneko extension, I could do it myself, but I think is better that someone with the 1.31 version (that I think is the version where Lua was introduced) so everyone can use it.


Click to expand...

The Wc3 template would probably be a Lua-map only including Debug Utils and Global Initialization. Not sure, if we need that, though. The effort needed to copy-paste these resources into your own map is very low. The template would need to be updated, whenever one of these resources does. Also, people probably want to choose options (map size, tileset) for their map on creation instead of using the template.

The VSCode-template makes more sense, but including the mentioned code resources has the same problem (must be updated too often). I think I prefer to go with the instructions from the chapters Wc3 setup and IDE setup. But I am happy to discuss, if you disagree.

Luashine said:

deviations into details that could've been left out for beginners

Click to expand...

All hidden behind spoiler tags now. Can be skipped by beginners.

Luashine said:

 I think [understanding the Warcraft API] is a barrier.


Click to expand...

True. Not the purpose of this guide, but will think about it.

Luashine said:

 Do you see what it looks like? The area of a right-angled triangle.


Click to expand...

I prefer to not rename these functions in particular, because attaching a meaning to them would drag focus away from the syntax (which is what the section is about).

Anyway, you have PM'd me many other good suggestions that I've incorporated instead

Luashine said:

 Pair reading. [...] Maybe you'll find someone who will read it for the first time to learn the topic, except together with you?


Click to expand...

I actually did that with a friend, who wanted to learn Lua. He was too intelligent and understood everything on first go. Useless!

#### HerlySQR

#### HerlySQR

 Level 25

JoinedJun 26, 2020Messages1,974

Resources: 2

#### Resources
Maps 1Spells and Systems 1

Eikonium said:

 The Wc3 template would probably be a Lua-map only including Debug Utils and Global Initialization.


Click to expand...

I said it more if the number of basic libraries of a Lua map increases, yeah if there were only those 2 libraries it won't be necessary.

Eikonium said:

 The template would need to be updated, whenever one of these resources does.


Click to expand...

I considered that, it wouldn't be a problem if those resources didn't update too frequently, maybe when they are more stablished.

Eikonium said:

 Also, people probably want to choose options (map size, tileset) for their map on creation instead of using the template.


Click to expand...

Those things are editable in the same map file.

One more thing reffering with to the tutorial, you can export the war3map.lua without needing to look into the MPQs by using the option of Export Script in the File menu (thank you @Tasyen):


 Last edited: Oct 3, 2022

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Update

Luashine said:

 I think [understanding the Warcraft API] is a barrier.


Click to expand...

I have now included a chapter about this (Getting Started -> Warcraft API).

Is this roughly what you had in mind?

HerlySQR said:

 One more thing reffering with to the tutorial, you can export the war3map.lua without needing to look into the MPQs by using the option of Export Script in the File menu


Click to expand...

Oh, that's much better. I replaced the MPQ-method with this one. Thank you!

HerlySQR said:

 I considered that, it wouldn't be a problem if those resources didn't update too frequently, maybe when they are more stablished.


Click to expand...

Sounds good. They currently both have active discussion, but we can consider this for the future.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Very elaborate, nice structure. I could probably find some nitpicks but that would be petty given how much material there is.

Approved.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

I saw this quick and dirty video shared on Reddit that could also be instructive:

#### Marcus777

#### Marcus777

Patron

 Level 6

JoinedAug 9, 2019Messages67

As someone who just started learning Lua and making a map, this tutorial is hands down the best that there is. I haven't finished reading it yet, but it already has been super helpful! Thank you very much for your efforts in writing this tutorial!

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

@Marcus777 Thank you for your kind words. This is very encouraging!

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

I've been working on a big quality of life fix for vJass2Lua, and coded an option in the transcompiler to process natives in a way that's legible for declaring them to Sumneko's extension. The result is a much more compact blizzard.lua and common.lua collection, that doesn't give errors about mis-matched return types. I've also attached the extra natives file, which I converted by hand. I am not sure if any more natives from the common.ai folder made it into Lua, but I'm attaching that copy just in case it comes up in the future.

The ---@meta tag at the top of each file helps convey that it's used for documentation purposes, rather than its functional code.

"real" has been changed to "number" and "code" to function. The keywords are synonymous, so I tweaked (the upcoming build of) vJass2Lua to convert those types automatically into their Lua counterparts.

I did some interesting edits to the documentation so as to not throw syntax errors when a "boolexpr" or "function" argument is nil - at least when the situation called for it. For example, TriggerAddCondition expects a non-nil second parameter, but GroupEnumUnitsOfPlayer will allow for ignoring the third parameter.

If anyone is interested in the current code for the upcoming vJass2Lua update, it's here: vJass2Lua/betaconversions.html at main · BribeFromTheHive/vJass2Lua . Aside from it not yet supporting Zinc, this is a strict upgrade over the published vJass2Lua script. To convert natives into NativeName=nil ---@type fun(nativeArgs): nativeReturn format, un-check the option "Comment-out native declarations", and also be sure to leave "Use @type one-liner instead of multi-line @param" checked.

#### Attachments

Blizzard.lua

 479.3 KB

 · Views: 83



CommonAI.lua

 8.2 KB

 · Views: 65



Extra Natives.lua

 4.8 KB

 · Views: 79



common.lua

 395 KB

 · Views: 69




 Last edited: Jan 1, 2023

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Please help me sorting out what exactly your definition files have changed in contrast to those already attached to the main post.

Do I need to update anything?

I see that you have replaced the function annotations by type-annotations in the style NativeName=nil ---@type fun(nativeArgs): nativeReturn to get rid of the missing-return and unused-local warnings. Honestly though, this change is not my cup of tea, because it has some downsides: the function preview now shows the native definition twice (right side of screenshot), the auto-completion-window (left side) now shows blue cubes ( = fields) instead of purple cubes (= functions) for all natives, and the same list does not show parameter names next to the functions anymore.

Also, the function parameter preview is ugly this way and doesn't show descriptions anymore:

That said, simply adding ---@diagnostic disable: missing-return, unused-local on top of the main post files is the better solution in my opinion.

Other than that, wasn't everything you mentioned already included?

real->number, code->function conversion and optional boolexpr arguments were definitely included. That also holds for GroupEnumUnitsOfPlayer, why were you mentioning those?

Also TriggerAddCondition works fine with its second argument = nil.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Hmm, I must have had older copies which were still using real/code as types. But I see what you mean; there is more syntax presented to the user when using auto-completion. The extra definition seen at the top of the "hover" context description is unnecessary; it would work fine with just the bottom definition. I am not sure why Sumneko has it designed like that, but it's worthwhile to provide that feedback to them to see that it should function the same as using ---@param and ---@return.

I have now reported this bug here: Different functionality on hover for ---@type fun() vs ---@param/return · Issue #1802 · sumneko/lua-language-server

Improvements to make on the existing files you have, without changing param to type:

I don't see where handle is defined as a class object, so what I did to resolve that is to add it like so:

 Lua:

---@class handle: userdata

I also added ---@meta tags to the top of each of the scripts, for whatever good that does. Your scripts would also benefit from it, because they are not functional code but reference code.

As a smaller detail, I added definitions for globals and _GV:

 Lua:

_GV = nil ---@type userdata
globals = _GV

_GV and globals are the same object, of type userdata.


 Last edited: Jan 1, 2023

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Updated the main post with improved definition files for common.lua, blizzard.lua and HiddenNatives.lua, as requested by @Bribe.

These are minor changes compared to the previous definition files (added ---@meta, _GV, ---@class handle:userdata and disabled diagnostics), but I'm confident that they will effectively prevent issues with the sumneko-extension for a good amount of time to come.

Disabling diagnostics in these files has also improved performance of the sumneko extension, as there is now several thousand less lines of code to check.

#### Trokkin

#### Trokkin

 Level 9

JoinedJan 23, 2015Messages134

Resources: 4

#### Resources
Spells and Systems 3Tutorials 1

I've been using Dencer.warcraft-vscode extension in VSCode, which doesn't require any additional download and can build lua files into your map. Which is at least a plain improvement over copy-pasting code into editor. So far, I am more than happy with it, and more than that, DebugUtils can even recognize my local files! So, I guess you could add it to the last paragraph in IDE setup.

There's a command to create a sample project, extension.warcraft.project.create.

They also have functionality to build object data with lua definitions, but I couldn't get it to work. Fine by me, but for it to not throw errors, clear the objediting folder.

Also, the following vscode settings preset is useful to get the game / WE running from command line:

 JSON:

 "warcraft.gamePath": "c:\\Program Files (x86)\\Warcraft III\\_retail_\\x86_64\\Warcraft III.exe",
 "warcraft.wePath": "c:\\Program Files (x86)\\Warcraft III\\_retail_\\x86_64\\World Editor.exe",
 "warcraft.weArgs": [ "-launch" ],
 "warcraft.gameArgs": [ "-launch" ],

UPD 2024:

The extension heavily generates the map script, which can totally (ha) mess up Total Initialization.

To fix this, add a trigger in your map, place it where you would've usually place scripts with libraries, and there, call

 Lua:

if dofile ~= nil then
 dofile('init.lua')
else
 print('This map is not built correctly.')
end

In the map project, create a src/init.lua file and use it instead of main.lua to include libraries. Example:

 Lua:

dofile('eik/DebugUtils.lua')
dofile('eik/IngameConsole.lua')
dofile('bribe/TotalInitialization.lua')
dofile('trk/FileIO.lua')
dofile('trk/Base64.lua')
dofile('trk/SyncStream.lua')
-- ... other libraries

And for the love of God, ensure you don't have libraries placed directly in the map.

Edit: I've noticed there are no mentions of lua multiline strings ([[ "Hello" 'world'! ]]) and bitwise operators (|, &, and more importantly << & >>). Could you add them too?


 Last edited: Aug 17, 2024

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Trokkin said:

 Edit: I've noticed there are no mentions of lua multiline strings ([[ "Hello" 'world'! ]]) and bitwise operators (|, &, and more importantly << & >>). Could you add them too?


Click to expand...

I deliberately left out several topics that I considered over the top for the purpose of the tutorial. Among those were long strings, long comments (--[==[ Comment ]==]), bitwise operators, _G, inf, Unicode, goto, _ENV, coroutines and more.

But I still incorporated the suggestions you made. If you find those sensible, other users might as well.

I also didn't know about the Dencer extension. Thanks for pointing that out!

#### MangaPwnz

#### MangaPwnz

 Level 3

JoinedDec 20, 2017Messages13

Hi, thanks for the amazing guide. it finally inspired me to work on a map AND learn a new language properly. There's no way I would have had the energy to keep going (or even start) without this resource to help me set everything up

I just can't wrap my head around memory leaks even after reading your guide and the resource, hope I'm posting them in the right place  Your guide says lua variables doesn't have to be niled, but it also says destroying Wc3 objects isn't enough to prevent leaks.

1. Will local variables containing Wc3 objects automatically be niled, (and thus all references to the wc3 object cleared) once they leave scope, or do we have to do it manually as in JASS? What about globals?

 Lua:

function DoStuff()
 ---@type table<integer, timer>
 local t = {}
 t[1] = CreateTimer()
 DestroyTimer(t[1])
 t[1] = nil -- Is this needed or not? The guide says "That's not a problem for most Warcraft types,
 -- because you do manually destroy them in your code and you can remove the table entry at the exact same spot."
 -- If it's not needed here, maybe you can provide an example where it is?
end

2. I think it should work like this after reading your guide, maybe?

 Lua:

 ---@return Set
 function SpawnWave()
 LevelTimer = CreateTimer()
 TimerStart(LevelTimer, self.levelTime, false)
 -- Do some stuff to spawn the wave
 end

 function CheckIfLevelFinished()
 if enemyUnits:isEmpty() or TimerGetRemaining(LevelTimer) <= 0 then
 -- Wave is finished
 DestroyTimer(LevelTimer) -- The global timer object is destroyed here
 end
 end

 SpawnWave() -- LevelTimer is a lua variable that holds a Wc3 object
 CheckIfLevelFinished() -- The global timer object is destroyed here, but since timer is of type agent,
 --we've still got another agent object in memory that LevelTimer references.
 SpawnWave() -- Now, LevelTimer points to a new timer object, the previous reference is cleared,
 -- and the agent from before is removed from memory, thus we don't have any leaks.

3. Is there a good way to make the sumneko extension not throw errors when setting variables of userdata type to nil? One way is clicking "Add Item" in "Lua › Diagnostics: Disable" in the Settings and adding "assign-type-mismatch" to the disabled diagnostics. Is this good? Or will it disable other useful diagnostics as well, it sounds like it will :/

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

MangaPwnz said:

 Hi, thanks for the amazing guide. it finally inspired me to work on a map AND learn a new language properly. There's no way I would have had the energy to keep going (or even start) without this resource to help me set everything up

Click to expand...

MangaPwnz said:

 Will local variables containing Wc3 objects automatically be niled, (and thus all references to the wc3 object cleared) once they leave scope, or do we have to do it manually as in JASS? What about globals?


Click to expand...

My guide assumes that readers have the basic knowledge presented in this tutorial about memory leaks.

To keep it short, there are two different types of leaks: object leaks and reference leaks.

Object leaks are big in size and happen, when you forget to destroy a Wc3 object.

Reference leaks are small in size and occur for Wc3 agents, when you forget to nil variables pointing to them.

JassLua

Object LeaksWc3 objects need to be removed via RemoveLocation, DestroyGroup, etc.Wc3 objects need to be removed via RemoveLocation, DestroyGroup, etc.

Lua objects are automatically garbage collected, but we need to clear references for that to happen.

Reference LeaksWe need to nil variables pointing to Wc3 agents.Nilling not necessary.

These statements hold for local and global variables alike.

Lua adds another type of reference-leak, though: dead references. This is what the sentence "That's not a problem for most Warcraft types, because you do manually destroy them in your code and you can remove the table entry at the exact same spot." was about.

The necessity to prevent object leaks via Wc3-destroy-methods creates another problem: dead references. These occur, when you access a Wc3 object after you have previously destroyed it. Variables and Table Entries pointing to the destroyed object will continue to point to it - they are not automatically being cleared (except for local vars that are getting out of scope). Logically, using those dead references in your code after destruction will lead to bugs (the code doesn't necessarily break, but it certainly won't work as intended).

 Lua:

--Example
local mapCenter = Location(0,0)
RemoveLocation(mapCenter)
--the mapCenter variable now holds a dead reference
CreateUnitAtLoc(Player(0),FourCC('hfoo'),mapCenter,0.) -- nothing happens

This scenario is much more likely to happen in Lua, because we now have tables that can contain objects of any type.

It's easy to save an object into a table and destroy it at some other place in our code, leaving a dead reference behind.

We have to ensure this doesn't happen, so when you destroy an Wc3 object, make sure to also remove it from any table it might potentially be included in (or at least be sure that the table entry is not accessed until the end of the game).

But there is another problem: Some Wc3 objects are not destroyed by the programmer, but by the game. This includes units, items and destructables (as a consequence of ingame events). Units for instance get removed from the game, after they have fully decayed. At that point, you might still have references to it inside global variables and tables, which are now dead references.

You need to make sure that there are no issues deriving from this scenario.

As said in the guide, there is still research to be made to the topic. For instance, we know that Wc3 compiles Jass to Lua since Reforged. Does that imply that reference leaks are no longer a thing in Jass as well?

What happens, when the garbage collector collects a Wc3 object that we forgot to destroy manually?

Can weak references solve problems for Wc3 objects?

Your code would correctly look like this:

 Lua:

function DoStuff()
 local t = CreateTimer()
 ...
 DestroyTimer(t) --nilling t is not necessary. same goes for t[x], if t is a local table (because t will cease and so will all entries)
end

MangaPwnz said:

 I think it should work like this after reading your guide, maybe?


Click to expand...

Your code is correct in the sense that indeed the only thing necessary to prevent memory leaks here is to call DestroyTimer at the right spot. It might be easier though to use the same timer for every wave (no need to recreate, when you can just restart). If you want to keep separate timers for each level, it would also be cleaner to save it to self.timer instead of a global variable (assuming self represents the level in your code and you plan to have exactly one timer per level). Avoid global variables as much as possible.

It's not visible in your code, but I assume you ensure that the second wave only spawns, if the finished-check is true? (code looks like the second spawns regardless of the result of the check)

I also assume that you run that check periodically, until the condition holds, like whenever a unit dies and when the timer runs out? (code looks like you only run it once).

MangaPwnz said:

 Is there a good way to make the sumneko extension not throw errors when setting variables of userdata type to nil? One way is clicking "Add Item" in "Lua › Diagnostics: Disable" in the Settings and adding "assign-type-mismatch" to the disabled diagnostics. Is this good? Or will it disable other useful diagnostics as well, it sounds like it will :


Click to expand...

Your solution will disable other useful diagnostics, like the warning after you assign a unit to a player variable.

Your desired setting is "Lua.type.weakNilCheck": true.


 Last edited: Feb 19, 2023

#### MangaPwnz

#### MangaPwnz

 Level 3

JoinedDec 20, 2017Messages13

Thanks for all the help, it cleared everything up and fixed those annoying warning messages everywhere! I spent an hour reading the guide on leaks over and over but just couldn't figure out how much it applied to lua^^ I mixed up memory leaks and dead references, but I get it now.

Eikonium said:

 Your code is correct in the sense that indeed the only thing necessary to prevent memory leaks here is to call DestroyTimer at the right spot.


Click to expand...

Yes, that's what I wanted to know. I cut out 90 % of the code and changed things from how I actually do it to spare you from reading a pages of my code. My real code manages to spawn the wave properly though.

Eikonium said:

 If you want to keep separate timers for each level, it would also be cleaner to save it to self.timer instead of a global variable (assuming self represents the level in your code and you plan to have exactly one timer per level).


Click to expand...

Thanks for the advice. I'll create a new field self.timer and save it to that instead. It works perfectly with what I'm trying to do.

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

MangaPwnz said:

 I spent an hour reading the guide on leaks over and over but just couldn't figure out how much it applied to lua


Click to expand...

That's valuable feedback, thanks. I've rephrased the guide to closer match what I wrote in the last comment, which I think helped you more than the original phrasing.

MangaPwnz said:

 Thanks for all the help, it cleared everything up and fixed those annoying warning messages everywhere!


Click to expand...

You are welcome!

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

Sorry if this was mentioned in the guide, but I didn't see it in the Desyncs section. I have a map made in Lua which seems to follow all of the rules you've listed, however I get desyncs pretty frequently.

Would using a wc3 Type as an [index] cause a desync? I've heard that Units have different handles between clients. I'm doing a lot of this in my code:

 Lua:

local u = CreateUnit(...)
MyTable[u] = 100
local p = Player(0)
MyTable[p] = 200

Thanks!

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

Yes, using the object itself as a key can cause desync when you loop through the table. They're not guaranteed to have the same order. SyncedTable solves this

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

Wrda said:

 Yes, using the object itself as a key can cause desync when you loop through the table. They're not guaranteed to have the same order. SyncedTable solves this

Click to expand...

Okay, so this is where I get confused. Is it strictly a "looping through the table" issue? Or would I get desyncs from other use cases? I've been told to completely avoid using Unit as an [index].


 Last edited: Apr 21, 2023

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

Only from looping through the table with pairs has issue. If you don't need to loop through the table, then you're fine in any way, shape or form.

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

I even consider using the unit as index a very good practice for non-loop tables. That is how it's meant to be. What reasoning had the guy telling you the opposite?

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

Eikonium said:

 I even consider using the unit as index a very good practice for non-loop tables. That is how it's meant to be. What reasoning had the guy telling you the opposite?

Click to expand...

###
 lua and unit data


With lua, i suppose you're still using an unit indexer ? and this custom unit value as a key of a list(s), where you can store other data ? I wanted a native way in lua to detect when an unit is removed with weak tables and __gc method, but then i realized garbage collector isn't instant and...

 www.hiveworkshop.com


###
 [Lua] Units are created with different handles


I have posted something similar to this before, but this time it has nothing to do with my character selection and save/load system. Creating units on the map results in different unit handles at some point = desync. Though, this time I disabled everything related to the save/load and character...

 www.hiveworkshop.com


#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

As far as I read your linked threads, they both state that using handle-keys in tables is fine, while pairs-looping through them is not.

#### HerlySQR

#### HerlySQR

 Level 25

JoinedJun 26, 2020Messages1,974

Resources: 2

#### Resources
Maps 1Spells and Systems 1

Hello, is there a clean way to stop the running thread? Because the only way I found is using the error() function, but that would display the message.

There is the coroutine.close() function, but that was added in Lua 5.4 and W3 uses Lua 5.3 so that is not an option.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

HerlySQR said:

 Hello, is there a clean way to stop the running thread? Because the only way I found is using the error() function, but that would display the message.

There is the coroutine.close() function, but that was added in Lua 5.4 and W3 uses Lua 5.3 so that is not an option.


Click to expand...

coroutine.yield() is best, as long as you are in a yieldable coroutine.

#### HerlySQR

#### HerlySQR

 Level 25

JoinedJun 26, 2020Messages1,974

Resources: 2

#### Resources
Maps 1Spells and Systems 1

Bribe said:

 coroutine.yield() is best, as long as you are in a yieldable coroutine.


Click to expand...

That wouldn't cause a memory leak?, because I bearly saw anyone recommend it and who did it didn't get good feedback.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

HerlySQR said:

 That wouldn't cause a memory leak?, because I bearly saw anyone recommend it and who did it didn't get good feedback.


Click to expand...

If the thread has no references, the Lua GC will collect it. Just like it does with any other Lua reference.

#### HerlySQR

#### HerlySQR

 Level 25

JoinedJun 26, 2020Messages1,974

Resources: 2

#### Resources
Maps 1Spells and Systems 1

Bribe said:

 If the thread has no references, the Lua GC will collect it. Just like it does with any other Lua reference.


Click to expand...

Ok, I will trust you.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

HerlySQR said:

 Ok, I will trust you.


Click to expand...

Well, you might trust me. But other readers can trust StackOverflow: Abandoning coroutines

#### HerlySQR

#### HerlySQR

 Level 25

JoinedJun 26, 2020Messages1,974

Resources: 2

#### Resources
Maps 1Spells and Systems 1

Bribe said:

 Well, you might trust me. But other readers can trust StackOverflow: Abandoning coroutines

Click to expand...

You always know much better words to search than me.

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Hi, thank you for this great tutorial!

I found a small error. "math.random(n) returns a random integer between 0 and n." It returns a random integer between 1 and n.

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

Antares is right:

###
 Doc - GetRandomInt


 lep.duckdns.org


###
 Reforged's luahelper.lua and broken FourCC


I won't just feed you, I will teach you how to fish 1. How to extract luahelper.lua 2. luahelper.lua ...Then let's print the file starting at these positions (start at byte 37392400 | only readable text | max 50 lines): user$ tail --bytes +37392400 "Warcraft III.exe" | strings -n 1 | head -n 80...

 www.hiveworkshop.com


 Lua 5.3 Reference Manual


#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Antares A said:

 Hi, thank you for this great tutorial!

I found a small error. "math.random(n) returns a random integer between 0 and n." It returns a random integer between 1 and n.


Click to expand...

Thank you, corrected.

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

I find the current version of the sumneko extension (3.7.4) to be good and stable enough to replace my previous suggestion (2.3.7).

3.X improves on many aspects (such as function parameter type checks), but also introduces some diagnostics that I consider disadvantageous/annoying for Wc3 mapping (strong nil check, inject-type check, ...).

It also can't visually distinguish global and local variables anymore, which is a big bummer for me personally (but still worth switching).

I use the following settings.json to enable/disable the features I want:

 JSON:

{
 "json.maxItemsComputed": 30000,
 "Lua.runtime.version": "Lua 5.3",
 "Lua.workspace.preloadFileSize": 2000,
 "Lua.workspace.maxPreload": 1000,
 "Lua.hint.enable": true,
 "Lua.type.weakUnionCheck": true,
 "Lua.type.weakNilCheck": true,
 "Lua.diagnostics.disable": [
 "lowercase-global",
 "inject-field"
 ],
 "editor.parameterHints.enabled": true,
 "workbench.editor.enablePreview": false,
 "editor.semanticHighlighting.enabled": true
}

I have incorporated the new suggestion into the main post (IDE Setup).

#### Arxos

#### Arxos

 Level 6

JoinedMar 8, 2012Messages51

Resources: 2

#### Resources
Maps 1Ported Icons 1

Regarding the desync section, you should make note of table.sort. Lua does not have a stable sort, which means the sort result can differ for each player in certain circumstances.

Given a table like this:

 Lua:

{
 { a: 5, b: 1 },
 { a: 3, b: 2 },
 { a: 4, b: 3 },
 { a: 5, b: 4 },
 { a: 6, b: 5 },
}

If you sort it by a, the final order will randomly be either:

 Lua:

{
 { a: 3, b: 2 },
 { a: 4, b: 3 },
 { a: 5, b: 1 },
 { a: 5, b: 4 },
 { a: 6, b: 5 },
}

OR

 Lua:

{
 { a: 3, b: 2 },
 { a: 4, b: 3 },
 { a: 5, b: 4 },
 { a: 5, b: 1 },
 { a: 6, b: 5 },
}

This means that if you do anything with b, you are likely to cause a desync.

As such, when using table.sort you need to make sure that either this uncertainty in ordering is not a problem, OR the table that is being sorted only has a single unique order, which may require adding an additional value to the tables that is guaranteed to be unique that you can use to sort identical items with.

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

I would also add that saying math.random is safe to use without desyncs is a bit of an unfortunate wording, because the function will actually cause a desync if you use it asynchronously. It might be worth clarifying that.

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Arxos said:

 Regarding the desync section, you should make note of table.sort. Lua does not have a stable sort, which means the sort result can differ for each player in certain circumstances.


Click to expand...

Unstable sort means that the order of equal elements can change, which doesn't imply that the result is random. Unstable sorting can still always guarantee the same output provided the same input, and we should assume that is the case unless tested otherwise.

I tested sorting your table in different Lua interpreters including Wc3 1000 times each and always got the same result.

If you have seen other results, could you maybe provide a test scenario?

Antares said:

 I would also add that saying math.random is safe to use without desyncs is a bit of an unfortunate wording, because the function will actually cause a desync if you use it asynchronously. It might be worth clarifying that.


Click to expand...

By asynchronously, are you referring to using math.random inside LocalPlayer-blocks? Even simple table operations could lead to desyncs in this scenario (which aren't mentioned either), but I can add the info if you think it would help

 You must log in or register to reply here.


### Similar threads

 LuaCATS / Emmy Annotation


Eikonium

Mar 19, 2022

Lua Tutorials

Replies20Views13K

Feb 4, 2024
Eikonium

 Lua Object Generation


PurgeandFire

Mar 17, 2011

Lua Tutorials

Replies28Views47K

Oct 13, 2016
WaterKnight

[Lua]
 Sort


ZiBitheWand3r3r

Oct 18, 2019

Graveyard

Replies2Views3K

Jun 8, 2022
Bribe

Locked

 [Lua] Control the order of the scripts


HerlySQR

Jul 22, 2022

Advanced Scripting

Replies13Views3K

Sep 3, 2022
Bribe

[Lua]
 GetSyncedData


HerlySQR

Aug 4, 2022

Old Submissions

Replies25Views5K

Oct 22, 2023
HerlySQR

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

Lua Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top