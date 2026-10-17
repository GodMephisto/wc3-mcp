---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/compiling-vjass-and-wurst-using-vscode.305213/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Compiling vJASS and Wurst Using Vscode
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/compiling-vjass-and-wurst-using-vscode.305213/>
#### Bannar

#### Bannar

 Level 26

JoinedMar 19, 2008Messages3,141

Resources: 23

#### Resources
Spells and Systems 5JASS 15Tutorials 1Wurst 2

Introduction
More and more modders use script languages rather than GUI due to developer-friendly features and great possibilities that come with them.

Vexorian's jasshelper brought vJass to the community many years ago, which soon became a pillar of Warcraft 3 modding.

Unfortunately, project has not been maintained properly for years and none fixed the issues that arose during that time.

Newest addition to the family, Wurst, brings refreshment to the table.

While it might have been launched a bit too early given its previous state, its current form, without a doubt offers greater array of features and is a modern approach to war3 map development.

It is also coupled nicely with VSCode, although, after reading the guide you should be able to use VSCode for vJass too.

Goal of this short tutorial is to get user used to modern approach of map development and allow him to work with vJass and Wurst together in a single map.

Note:

This guide does not explain things in very detailed manner. Target is to present the problem and provide general answer.

Once you have firm grasp over the subject, you will surely conquer any monster that you might face.

Is it assumed that you are familiar with: jasshelper, vJass, Trigger Editor, wurst setup, basic wurst, and general usage of VSCode.

Helpful links:

WurstScript home page

Jasshelper manual

Visual Studio Code documentation

Facts
In a world dominated by vJass, it might not be easy to sneak wurst into the map.

One cannot simply be told to rewrite entire map to wurst in one go.

Let's state some key facts:

Compatibility is important, vJass cannot be simply just scrapped

Vast majority of war3 modders use vJass

Heavily favored over Wurst by new users because of streamlined learning curve: GUI -> Jass -> vJass
Users which want to include wurst code in their map face compatibility problems.

This is because wurst is not fully compatible with vJass, example:

Different reserved keywords: construct

Solution: rename your variables​

Forbidden native TriggerRegisterVariableEvent

Solution: use TriggerEvaluate directly, rather than real variable manipulation​

Target

Need of mechanism for handling situation when both, wurst and vjass code exist in the map.

Behind the scenes
vJass and jasshelper:

Pictures below show high-level view of build process

1) Input single .j file into jasshelper, which outputs pure jass file readable by Warcraft3.

2) Inject compiled.j file into template map to receive fully functional w3x file.

3) Via Warcraft3, players can experience beauty of our map.

Process path for wurst is similar with exception of jasshelper being replaced by wurst compiler.

Combing Wurst and vJass
An output for jasshelper compilation operation is .j file with name specified in the command containing compiled code to pure jass. For build operation, is it map w3x file given the name.

An output for standard build wurst map operation is compiled.j and map file e.g.: WurstRunMap.w3x. There is no separate compilation task.

The key thing to note, which we gonna take advantage of, is fact that wurst compiler searches for war3map.j file within <your_project_path>/wurst directory. This concept is shared with you during the "Hello World" of wurst, that is, creation of your first wurst project. Once saved, you will notice the new .j, war3map file, appearing in your wurst directory.

Wurst compiler will combine both, wurst scripts and war3map.j file together and attempt to compile them into a single compiled.j file.

Let's modify previously shown process path of map creation to reflect what we have just said:

1) Use jasshelper to create map script file and place it under /wurst directory.

2) Combine both, wurst and war3map to create compiled.j file.

Afterwards, the compiled.j will be used for map creation, just like for jass-only map.

Work, work

Pictures displaying origination of Happy gamer for vJass do not list .j file (notice the lack of plural) by mistake.

Let's checkout commands which we use when compiling vJass with jasshelper

 Code:

// compile
jasshelper.exe --scriptonly <path_to_common.j> <path_to_blizzard.j> <path_to_input.j> <path_to_output.j>

// build
jasshelper.exe <path_to_common.j> <path_to_blizzard.j> <path_to_mapscript.j> <path_to_map.w3x>

Notice that both arguments <path_to_input.j> and <path_to_mapscript.j> are expected to be a single file each. Thus one is forced to generate a single .j file from all of our jass scripts visible in World Editor's Trigger Editor.

We are tasked with the following:

Extract and compile our vJass map code into a single war3map.j file

Have the war3map.j under /wurst directory

Run wurst compiler to generate final compiled.j file

Once you read this through and give yourself a moment for reflection, you will realize how tedious would be to work with a single .j file which might be tens of thousands lines long.

This is where our next player comes in.. the VSCode.

On wurst side, there not much left to do. As said already, it is coupled nicely with VSCode. Building and running map from the IDE is fully automated.

The highlighter and syntax checker are great, auto-completion is here too. You wouldn't tell a difference from working with typical programming language in VSCode.

Automation is key here. Launching everything manually once or twice is not a problem. However, doing this repeatedly will prove to be a nuisance.

Below are displayed the standard commands available for wurst map:

Ideally, we would like to see something similar for our old vJass friend:

To achieve our goal, we are going to take advantage of VSCode tasks.

It was mentioned previously that we need a single .j file for jasshelper to not complain about parameters. It was also noted that it is tedious to work with one big file on a daily basis.

However, nowhere it is said how the organization of files must look like before the compilation. We need that single file only for the moment of compilation. Before and after, doesn't matter.

Since the spirit of this tutorial is: a modern approach to war3 map development, we would like to keep things that way.

Our neighbour, wurst, already acts as a good example. All wurst files are kept under /wurst directory, and their organization is yours to decide.

We are going to copy that approach, and create /jass directory where all the .j files are going to reside.

If you want, you can even mirror script organization from World Editor's Trigger Editor. In such case, triggers would be replaced by .j files and categories by folders.

To sum up what we have just said:

jasshelper expects single .j file as an input (apart from common.j and blizzard.j)

Scripts have to be extracted and placed under single directory

Content of those scripts has to be combined and only then passed to jasshelper for compilation
Example below shows the organization of map project where vJass and wurst and present simultaneously:

VSCode tasks can be used to execute simple task for current workspace which is great when we want to perform repeatable operations, such as building and running maps. Array of available tasks should be stored in task.json file under /.vscode directory.

What we need?

task.json file to specify range of tasks available

Scripts to perform actual tasks
For latter, I've decided to go for powershell which is preferred shell-oriented scripting language on Windows machines. You can choose whatever language you prefer and are comfortable with. Shell scripts, python scripts. It's all fine. Just be sure to edit 'command' accordingly.

Let's show an example, because example is worth more than a thousand words:

 Code:

 "tasks": [
 {
 "label": "Compile map",
 "type": "shell",
 "command": "powershell -ExecutionPolicy ByPass -File ./compile.ps1",
 "group": {
 "kind": "build",
 "isDefault": true
 }
 }
 ]

Using VSCode task page reference, here is simple description of task fields:

label: The task's label used in the user interface.

type: The task's type. For a custom task, this can either be shell or process.

command: The actual command to execute.

windows: Any Windows specific properties. Will be used instead of the default properties when the command is executed on the Windows operating system.

group: Defines to which group the task belongs. Usually it ends up being "test" or "build".

presentation: Defines how the task output is handled in the user interface.
It should be clear now how this simple task operates - and remember, you always want to keep it simple.

You give it name that is going to be depicted on the task list, kind, and command. This is the same command that you would execute from your shell so it is easy to test this out if something goes wrong.

Using this method, one can create several tasks. Personally, I recommend having:

compile-only task, without map script injection

compile-only task, without map script injection with --debug option added

build task, which compiles map script and injects it to the map

run task, which works as previous one, but runs the map afterwards
I would not forgive myself without providing you with yet another example. This time with compile-task script. Our goal is to combine the content of all files under /jass directory in order for jasshelper to compile the map script:

 Code:

# Create GeneratedJ file from content of all .j and .zn files
$InputFiles = Get-ChildItem -Path $JassFolder -Include *.j, *.zn -Recurse
New-Item $GeneratedJ -ItemType File -Force

ForEach ($File in $InputFiles)
{
 If ($File.Extension -eq '.zn')
 {
 Add-Content -Path $GeneratedJ -Value "//! zinc"
 Get-Content -Path $File | Add-Content $GeneratedJ
 Add-Content -Path $GeneratedJ -Value "//! endzinc"
 }
 Else
 {
 Get-Content -Path $File | Add-Content $GeneratedJ
 }
}

# Run jasshelper to perform actual compilation of map script
$Params = $args + @('--scriptonly', $CommonJ, $BlizzardJ, $GeneratedJ, $OutputJ)
& $Jasshelper $Params

This implementation is straight forward. Create new file as a sum of content of all files under JassFolder. Once generated, this file is passed to jasshelper for compilation.

Notice the special care has been taken for Zinc. Preprocessor directives are appended before and after every .zn file to ensure that jasshelper will indeed treat them as Zinc files. If your .zn files already have those directives, you can drop the 'If'.

Summary

In order to compile map where vJass and Wurst are both present using VSCode task automation one needs to:

Extract scripts from Trigger Editor and put them under single directory

Create simple script for combining content of all files and launching jasshelper

Have task.json prepared and present in his workspace directory

Compile vJass scripts, using compile task. On successful compilation, war3map.j should be present in /wurst folder

Run build or run wurst map task from VSCode
Right after script extraction and first compilation you might face issues such as: missing variable, function not found. This is especially true if you are going to adjust map containing pure jass or converted GUI code using this approach. You will have to manually repair the order of your files within /jass directory to fix those. I recommend having separate .j file for globals and separate for map main functions. Ensure that the former is always on top (put it into directory with name starting with '0') and the latter is the end (have it outside of any directory, make it start with 'z' letter) of file hierarchy.

Live example of map which contains jass, GUI, vJass, Zinc and Wurst code combined: Island Troll Tribes. Map is currently under transition of becoming wurst-only map.

Is the reverse possible? Meaning, compiling wurst map script first, then appending some jass scripts and outputting compiled.j via jasshelper?

It is, afterall, wurst compiler outputs two files, compiled.j and w3x map file (e.g.: WurstRunMap.w3x) into _build directory.

You can use the exact same method described above for integrating Wurst code into vJass map - combine the /_build/compiled.j with your .j files and pass that to jasshelper. The output of this, the combined map script .j file, can be used with jasshelper build command to generate functional map.

However, before you proceed, ask yourself if perhaps, you are not doing something wrong. If you are planning to use wurst, you should not be thinking about injecting vJass code into it.

Something is missing? Not implemented? Frotty and the boyz are more than happy to hear you out!

You can meet them on github and their irc, see wurst contact page.


 Last edited: Apr 29, 2018

#### Bannar

#### Bannar

 Level 26

JoinedMar 19, 2008Messages3,141

Resources: 23

#### Resources
Spells and Systems 5JASS 15Tutorials 1Wurst 2

Reserved.

#### MyPad

#### MyPad

 Level 31

JoinedMay 9, 2014Messages1,878

Resources: 27

#### Resources
Models 1Icons 4Maps 6Spells and Systems 7JASS 7Lua 1Tutorials 1

Quite a definitive approach you have right here. This explains quite a lot on the tasks in VS Code, which I have no knowledge of.

Can one do the heavy lifting of updating jassHelper to use the more modern approach, outsourcing the script to VS Code, instead of many who will try and fail at said task? This would mean that only one has to do it and bundle the entire thing, rather have the following scenario that I described occur.

#### Bannar

#### Bannar

 Level 26

JoinedMar 19, 2008Messages3,141

Resources: 23

#### Resources
Spells and Systems 5JASS 15Tutorials 1Wurst 2

Professional approach to software development:

- isolate bugs from features

- fix bugs first, ensure sw is working

- only then, start adding new features

Long story short: none ever fixed known issues properly without contaminating jasshelper with new features which were never finished eventually leading to that version being unusable.


 Last edited: Apr 29, 2018

#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

#### HungryHusky

#### HungryHusky

 Level 11

JoinedSep 14, 2009Messages284

Resources: 6

#### Resources
Templates 5User Interface 1

I want to learn to use wurst but the tasks in VSCode don't show up when I wanto ro run the map etc. How do I fix this?

#### Bannar

#### Bannar

 Level 26

JoinedMar 19, 2008Messages3,141

Resources: 23

#### Resources
Spells and Systems 5JASS 15Tutorials 1Wurst 2

Try: [CTRL + SHIFT + P] mate.

Edit: Glad it worked out : )


 Last edited: May 10, 2018

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Awesome tutorial. Really useful for people who want to migrate their maps to wurst (or take advantage of existing vJASS libraries), or just switch up their workflow a bit. I love the amount of detail and the nifty diagrams.

Approved. One request though: I think it would be helpful to include the build task and run task script. :] This tutorial is really useful for people who want to configure their own workflow, but I imagine a lot of people will just want some ready-made scripts that they can plug in to VSCode and be on their way.

#### Trokkin

#### Trokkin

 Level 9

JoinedJan 23, 2015Messages134

Resources: 4

#### Resources
Spells and Systems 3Tutorials 1

Tutorial is great. Everything else was already said.

Actually, instead of updating jasshelper, one could create proper vscode extension that will solve compilation problems with vjass. And maybe add proper language support, at least for syntax highlighting. There is already one extension, but it isn't being updated and highlight do not support loops or bj/natives.

Then two extensions for two languages could be combined through settings file so that workflow for two languages will become even more smooth and easy.

But combination of both wurst and vjass seems for me kinda bad. They both use different approach for compiled naming which may result in name collision, and they can't access each other's part. Well, like, wurst can use functions from war3map.j, but not vice versa, which at best means that vjass part can be used in a pretty ugly to use, because of the compiled function names.

Nice that it allows for a smooth transition from vjass to wurst, but I can hardly imagine that somebody would actually develop a map sitting on these two chairs.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I have a question and see two possibilities, both give unwanted answers.

It's regarding the conversion.

So let's say I have my huge-ass map with a hundred triggers.

I extract the gigantic .j file with all the code and place it in the jass folder.

Problems:

1. The script is an unreadable state and I cannot edit it without my eyes bleeding.

1b. Which means I need to extract each time I make a jass update.

2. The code is in a giant file which makes it hard to update

The desired outcome is that I have a bunch of .j files of uncompiled code

Supposedly the task should handle this? but I do not understand how.

Jasshelper is no a native program on my computer so I got no clue how the task is supposed to find a copy of jasshelper and run it.

Maybe I went through the tutorial a bit quickly but I did not see any instruction to download jasshelper which make me confused.

#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

Chaosy said:

 1. The script is an unreadable state and I cannot edit it without my eyes bleeding.

1b. Which means I need to extract each time I make a jass update.

2. The code is in a giant file which makes it hard to update

The desired outcome is that I have a bunch of .j files of uncompiled code

Supposedly the task should handle this? but I do not understand how.


Click to expand...

It's in the summary. The tutorial says the user has to extract their jass scripts from triggereditor into separate files. If you just take a map, jasshelper will output 1 big .j.

Also, you of course aren't supposed to modify the compiled .j files. These are just generated so wurst can parse and interact with your (v)jass code.

This makes your points kinda moot. You're not supposed to touch generated code and I don't get why you would ever even want to do that.

Chaosy said:

 Jasshelper is no a native program on my computer so I got no clue how the task is supposed to find a copy of jasshelper and run it.


Click to expand...

Neither is wurst? In fact, there are no installation instructions at all, I guess this is expected. Download JH binaries and put them somewhere where you can call the .exe from the vscode task.

I suppose this should be added to the tutorial somehow.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

The whole point of having an external jass folder is so I can edit the code on demand from vscode. (at least for me)

Otherwise I see no difference than just using the editor and remove all the extra steps for no benefit.

It is also supposed to be automated, and updating jass code is clearly not automated if I need to extract code manually each time.

This is why I think it is very odd that the conversion step from uncompiled jass to compiled is not included.

Frotty said:

 Neither is wurst? In fact, there are no installation instructions at all, I guess this is expected. Download JH binaries and put them somewhere where you can call the .exe from the vscode task.


Click to expand...

If it is not an application which appears in the list of installed programs; I need to specify a path to the install path, something which is missing from the script as far as I can tell.

I do not know powershell (or any shell language really) so I have no clue how to do it myself.

#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

Yes Chaosy, obviously you edit your raw vJass resources in vscode. Those raw files are then passed to JH, which generates the compiled Jass. This compiled Jass is used by Wurst and you shouldn't touch it.

e: maybe we wait for the tutorial creator to clear up these concerns  I might understand it differently too.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Alright I gave this another go.

I have some success but also came across more issues regarding this approach.

The problem comes from the fact that jasshelper forces you to have a main function in your jass code.

Easily fixed.

However, this means my code cannot co-exist with the default map script, since it also use a main function which causes collision.

This forces me to scrap the standard war3map.j (which is said in the tutorial, but not explaining why) and overwrite it with my own

There are several drawbacks to this as there are a lot of things defined in the war3map.j

Such as preplaced unit positions. (and a bunch of other stuff, but this is the dealbreaker for me)

The simple fix for this would be if wurst had a way to specify the location of the extracted war3map.j

Because if war3map.j was automatically put in the jass folder it would compile perfectly

Doing it manually each time I move a unit is too bothersome to me.

My opinion aside, I think it should be worth pointing out the downsides in the tutorial

edit: You could remove the war3map.j from the compile.ps1 after using it to compile which would remove the problem.

With the 'minor' downside of having to run three commands every time you move a unit

compile vjass > build wurst > run wurst


 Last edited: Nov 17, 2018

#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

Imho this workflow isn't something recommended to do, just explains how to do it if you plan to maintain a legacy map with wurst.

 However, this means my code cannot co-exist with the default map script


Click to expand...

Shouldn't there be a way to pass the map/war3map.j to JH? Since this happens automatically in JNGP/wex.

Maybe post your concrete example? Perhaps we can find a neat solution using vscode tasks.


 Last edited: Nov 18, 2018

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Frotty said:

 Shouldn't there be a way to pass the map/war3map.j to JH? Since this happens automatically in JNGP/wex.

Maybe post your concrete example? Perhaps we can find a neat solution using vscode tasks.


Click to expand...

There is.

So if my compile script does the following:

combine "./wurst/war3map.j" + all .j files in "./jass"

Output "./wurst/vjass.j

Now we have war3map.j and vjass.j with a main function which wont work.

My best solution would be to have some sort of file watcher that automatically moves the war3map.j into the jass directory when it is generated in the wurst folder.

Because the main issue to me is that the war3map.j is placed in the wurst folder which injects it into the map. Combined with the fact that even if I do something like move the war3map.j after compiling.. problem re-emerge when I test the map, forcing me to compile after every single test which is just annoying

Alternatively make some script to merge the two files which I think is pretty complicated. And also it is just a bandaid solution.

#### Regno

#### Regno

 Level 9

JoinedSep 16, 2016Messages248

I know that when I build my map using wurst, if I open it with World Editor and save, it gets "broken". Presumably so, the WE overwrites my wurst script. Is this true? and if yes, will this tutorial help me avoid that?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Regno said:

 Is this true


Click to expand...

Yes.

Regno said:

 and if yes, will this tutorial help me avoid that?


Click to expand...

No.

 You must log in or register to reply here.


### Similar threads

Locked

 This Summer in Wurst


Cokemonkey11

Nov 15, 2017

Latest Updates and News

Replies28Views7K

Dec 30, 2017
Cokemonkey11

Locked

 Problem starting with Wurst


HerlySQR

May 26, 2021

Triggers & Scripts

Replies24Views4K

Jun 24, 2021
HerlySQR

Locked

[vJASS]
 Compiling a vJASS Blizzard.j for Mod


Retera

Aug 8, 2012

Triggers & Scripts

Replies2Views1K

Aug 9, 2012
Retera

 WURST: Starting Guide


Anchorist

Sep 9, 2019

Tutorial Archive

Replies1Views3K

Oct 2, 2020
Chaosy

Locked

 Choosing a scripting language, is Wurst worth it ?


Numenor010

Jul 9, 2021

World Editor Help Zone

Replies6Views2K

Jul 31, 2021
Uncle

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

Transpiled Languages

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top