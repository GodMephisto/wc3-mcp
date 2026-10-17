---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/dialog-buttons-advanced-systems.186390/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Dialog Buttons Advanced Systems
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/dialog-buttons-advanced-systems.186390/>
#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

 Dialog Buttons (Advanced Systems) by -Kobas-


Dialog Buttons (Advanced Systems)


by -Kobas-

Requirements


 * Basic knowledge of the Object Editor


 * Basic knowledge of the Trigger Editor


 * Poor knowledge in variables.


 * A moment of your time  (Around 10-15 min).

Introduction


A dialog menu is an easy way to allow players to make decisions about game options. Most dialog menus usually simple enough that they can be done effectively even with only limited knowledge of triggering techniques. At the end of the tutorial we will be discussing some advanced methods of dialog menus.


It is a very simple series of actions to create a dialog menu, but it does require that you are willing to use variables. By the end of this tutorial, you will be comfortable enough with the triggers involved to be able to create your own dialog menu and use it allow the host set the game difficulty, game mode and so on. This tutorial also will describe the use of all GUI dialog action commands and how they should be used.

Let's start!


Ok first of all, some basic things that you should know abiut "Dialog Buttons"

Dialog menus should NOT be created at map initialization – if you want to create a dialog menu soon after the game loads for all players, use the event

Time - Elapsed game time is 0.00 seconds
which can be set to any time after the game starts.

The menu itself has a built in bias - the first button on every dialog menu is the default button. This means that if you have lazy bnet players who just hit return when a dialog menu comes up without bothering to look at the available options, they will get whatever is listed as the first button. Choose your menu selection order carefully, because people sometimes don’t bother to read the options carefully.

Keep your dialog menu triggers as streamlined as possible. Make multiple triggers if you have a complex sequence of events, and stay clear of If / Then / Multiple actions - particularly if they are nested inside the button selection / voting process. Simplifying the triggers and keeping different operations in different triggers will make your life easier and will allow you to easily modify your dialog menus later if you need to.

There isn’t a lot of room on dialog menu buttons for text; so try to keep the options simple and short.

If you have more than ~10 options to choose from, you may want to think of a different method for allowing your players to choose. There are limits to what will be visible to players and a long list of options might be confusing or awkward for players to make selections.

Dialog menus will always pause the game in single player maps. The game will not continue until a selection is made.

1. Our Example


All dialog menus can be broken into basically two components, the dialog setup/display part, and the reaction to the player's decision part. In dialogs with more than one button, each button option will generally get its own trigger, which is driven the player’s selection. For this section of the tutorial we will be making a advanced dialog menu for player 1 (Red).

Idea

How it look after setup!

How it look when we click on some buttons in order to change them!

Final result!

2. Let me explain few things about dialog buttons

 Let's create the dialog menu itself first. As with all triggers, we need an event, some conditions, and then some actions. In our example, we are going to use time elapse as the event. This is a reliable way to get the menu to pop up at a predetermined time.

Time - Elapsed game time is 0.00 seconds
A simple event; it only requires a time to be input. After the map initializes, this will trigger your menu to be created however many seconds after the game starts that you have set it to. Most menus do not require conditions for setup, however in some maps you may want to include bonus selection options that are dependent upon a chat command or some such thing.


You will note that there is no "create new dialog" action; the dialog menu is established when the variable is created, although it is created with no menu or buttons. Our first action will be to set up the title of the dialog menu. The action we will use is:

Action - Dialog - Change the Title of (dialog) to (title)
Your selection options for the action listed above are (dialog) and (title). In the case of dialog, you simply want to click on it and select your dialog variable from the listed variables. It is possible to use an event response for the dialog option, but the only one available for that slot is (Clicked Dialog) - and this would not trigger using the event in this trigger (elapsed time). For the title entry, you can simply click on that and fill in whatever you want your title to be. If you want your title to appear with different colors, you will need to use color codes.


An example of colorized text looks like this:


|c00889e78Your Title Here|r


Once you have a title, you need to start adding dialog buttons to your dialog menu.


To add buttons to a dialog menu you use the action Create a Dialog Button:

Action - Dialog - Create a dialog button for (dialog) labelled (text)
As before, your selection options are limited to which dialog and some text for the button. The (dialog) selection options are identical to those found in the previous action for dialog, and in this case you will want to simply select your dialog variable again. For the (text) component, you simply type in the text you want for this button. As before, you can colorize this text, but be mindful of your text length. (On a side note, Dear Blizzard – the preferred spelling is 'labeled' not labelled.)


Now, so far you have created a dialog title, and one button. The problem is, the computer doesn't know one button from another (there is no reference to any button number) so when someone clicks on that button as it is, nothing will happen. This is where our other variable comes in to play.

Action - Set (Variable) = (Value)
Here we are going to set our dialog button variable array (one element of it) to equal the button we just created. For the (variable) part of the above trigger, you will want to click on it and select you button variable array. In the example above, this was called DialogButton. As is always the case when you are setting a component of a variable array, you need to specify the index number. In this case, we will simply index our buttons by their position in the menu.

Set DialogButton [1] = (last created dialog button)
Each subsequent dialog button you create will get its own [#] index value. If you opted to create several dialog button variables (instead of an array) you would simply select the dialog button variable that is related to the text label of this button.


At this point, you can simply copy these procedures to create more buttons – make sure that you are assigning them to a different element of the variable array all the way down your menu.


Once you have created your dialog menu buttons, you just need to show the dialog to your player.


The action that shows players the dialog box is:

Dialog - Show/Hide (dialog)
When you select this action, it will change to ask you which dialog (same options as before) and who do you want to show it to. In our first example, we are just going to show the dialog menu to Player 1 (Red), however if you were making this for a mulitplayer voting system, you would want to show your menu to all of your players. We will cover this approach later.


In some cases, you may want to consider placing a time limit on the dialog menu, to limit the time players are allowed to choose an option. To make your dialog menu so that if nothing is chosen after a period of time, it automatically disappears, you would simply put in a Action - Wait (game time) after the dialog has been shown to the players and in the same trigger include a Dialog - Hide Dialog action, but you will need to make sure there is a default option in the case that nothing is selected.


Now your dialog menu should be set up. Here’s a sample of a complete trigger that will create and show a menu:

SelectDifficulty

Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
Dialog - Change the title of DialogMenu to |c00889e78Choose Difficulty|r

Dialog - Create a dialog button for DialogMenu labelled EASY

Set DialogButton [1] = (Last created dialog Button)

Dialog - Create a dialog button for DialogMenu labelled MODERATE

Set DialogButton [2] = (Last created dialog Button)

Dialog - Create a dialog button for DialogMenu labelled HARD

Set DialogButton [3] = (Last created dialog Button)

Dialog - Show DialogMenu for Player 1 (Red)

When your game is started, it will appear after the appropriate elapsed time, however when you click on the buttons nothing will happen at this point. Why? Because you haven't set up any triggers that respond to dialog button clicks. We will cover this in the next section.

3. Setup Trigger

Kobas Button Setup
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
-------- Game Modes. Difficulty... --------

Set Kobas_String_GameMode[1] = |c00FFFFFFNormal|r

Set Kobas_String_GameMode[2] = |c00FFFFFFAll Pick|r

Set Kobas_String_GameMode[3] = |c00FFFFFFAll Random|r

Set Kobas_String_Difficulty[1] = |c00FFFFFFNormal|r

Set Kobas_String_Difficulty[2] = |c00FFFFFFEasy|r

Set Kobas_String_Difficulty[3] = |c00FFFFFFHard|r

Set Kobas_String_BattleArena[1] = |c00FFFFFFNormal|r

Set Kobas_String_BattleArena[2] = |c00FFFFFFSnow|r

Set Kobas_String_BattleArena[3] = |c00FFFFFFSand|r

Set Kobas_String_SpecialEvent[1] = |c00FFFFFFNormal|r

Set Kobas_String_SpecialEvent[2] = |c00FFFFFFNo Power Ups|r

Set Kobas_String_SpecialEvent[3] = |c00FFFFFFNo Neutrals|r

-------- Integer Counter --------

For each (Integer A) from 1 to 4, do (Actions)
Loop - Actions
Set Kobas_DB[(Integer A)] = 1

-------- Dialog --------

Dialog - Clear Kobas_Dialog

Dialog - Change the title of Kobas_Dialog to |c00FF0303Game Opti...

Dialog - Create a dialog button for Kobas_Dialog labelled (|c00FEBA0EGame Mode: |r + Kobas_String_GameMode[Kobas_DB[1]])

Set Kobas_Button_GameMode[Kobas_DB[1]] = (Last created dialog Button)

Dialog - Create a dialog button for Kobas_Dialog labelled (|c00FEBA0EDifficulty: |r + Kobas_String_Difficulty[Kobas_DB[2]])

Set Kobas_Button_Difficulty[Kobas_DB[2]] = (Last created dialog Button)

Dialog - Create a dialog button for Kobas_Dialog labelled (|c00FEBA0EBattle Arena: |r + Kobas_String_BattleArena[Kobas_DB[3]])

Set Kobas_Button_BattleArena[Kobas_DB[3]] = (Last created dialog Button)

Dialog - Create a dialog button for Kobas_Dialog labelled (|c00FEBA0ESpecial Effects: |r + Kobas_String_SpecialEvent[Kobas_DB[4]])

Set Kobas_Button_SpecialEvent[Kobas_DB[4]] = (Last created dialog Button)

Dialog - Create a dialog button for Kobas_Dialog labelled |c0020C000OK!|r

Set Kobas_Button_OK = (Last created dialog Button)

Dialog - Show Kobas_Dialog for Player 1 (Red)

-------- We destroy this trigger to optimize map a little --------

Custom script: call DestroyTrigger( GetTriggeringTrigger() )

As you can see we used

Set Kobas_String_GameMode[1] = |c00FFFFFFNormal|r
To create string variable that will display game modes (normal in this case)


Same goes for difficulty, battle arena and so on!


|c00FFFFFF is white color (example) if you want another one you can find it here as well http://users.rcn.com/giant.interport//COLOR/1ColorSpecifier.html


We also set default value of our buttons to 1 with next trigger actions!

-------- Integer Counter --------
For each (Integer A) from 1 to 4, do (Actions)
Loop - Actions
Set Kobas_DB[(Integer A)] = 1

Next few trigger actions create dialog and add buttons to it (we explained above how):

Dialog - Create a dialog button for Kobas_Dialog labelled (|c00FEBA0EGame Mode: |r + Kobas_String_GameMode[Kobas_DB[1]])
Set Kobas_Button_GameMode[Kobas_DB[1]] = (Last created dialog Button)

This one create 1st dialog button and it will set it's name to |c00FEBA0EGame Mode: |r + Kobas_String_GameMode[Kobas_DB[1]]


that means it will be:


Game Mode: Normal -> for Kobas_String_GameMode[1]


Game Mode: All Pick -> for Kobas_String_GameMode[2]


Game Mode: All Random -> for Kobas_String_GameMode[3]


If you want to add few more game modes, add more string variable and list them above just like we did in our example!


Example:


Game Mode: Normal -> for Kobas_String_GameMode[1]


Game Mode: All Pick -> for Kobas_String_GameMode[2]


Game Mode: All Random -> for Kobas_String_GameMode[3]


Game Mode: Same Hero -> for Kobas_String_GameMode[4]


 Game Mode: Funny Mode -> for Kobas_String_GameMode[5]


And at the end we:

Dialog - Show Kobas_Dialog for Player 1 (Red)
Showed trigger to host (red player) so he can pick mode and other game settings!


After this we destroyed trigger! (We won't use it again right!)

Custom script: call DestroyTrigger( GetTriggeringTrigger() )


We finished with setup for now, so lets move on!

Kobas Button Click
Events
Dialog - A dialog button is clicked for Kobas_Dialog

Conditions

Actions
-------- We check witch dialog button was clicked --------

-------- We added 1 if then else function under another to prevent some bugs! --------

-------- Still because we use this on start and there are not to many triggers turned on, this won't change performance! --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Clicked dialog button) Equal to Kobas_Button_GameMode[1]

(Clicked dialog button) Equal to Kobas_Button_GameMode[2]

(Clicked dialog button) Equal to Kobas_Button_GameMode[3]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
Kobas_DB[1] Equal to 1

Kobas_DB[1] Equal to 2

Then - Actions
Set Kobas_DB[1] = (Kobas_DB[1] + 1)

Else - Actions
Set Kobas_DB[1] = 1

Trigger - Run Kobas Button Edit <gen> (ignoring conditions)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Clicked dialog button) Equal to Kobas_Button_Difficulty[1]

(Clicked dialog button) Equal to Kobas_Button_Difficulty[2]

(Clicked dialog button) Equal to Kobas_Button_Difficulty[3]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
Kobas_DB[2] Equal to 1

Kobas_DB[2] Equal to 2

Then - Actions
Set Kobas_DB[2] = (Kobas_DB[2] + 1)

Else - Actions
Set Kobas_DB[2] = 1

Trigger - Run Kobas Button Edit <gen> (ignoring conditions)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Clicked dialog button) Equal to Kobas_Button_BattleArena[1]

(Clicked dialog button) Equal to Kobas_Button_BattleArena[2]

(Clicked dialog button) Equal to Kobas_Button_BattleArena[3]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
Kobas_DB[3] Equal to 1

Kobas_DB[3] Equal to 2

Then - Actions
Set Kobas_DB[3] = (Kobas_DB[3] + 1)

Else - Actions
Set Kobas_DB[3] = 1

Trigger - Run Kobas Button Edit <gen> (ignoring conditions)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Clicked dialog button) Equal to Kobas_Button_SpecialEvent[1]

(Clicked dialog button) Equal to Kobas_Button_SpecialEvent[2]

(Clicked dialog button) Equal to Kobas_Button_SpecialEvent[3]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
Kobas_DB[4] Equal to 1

Kobas_DB[4] Equal to 2

Then - Actions
Set Kobas_DB[4] = (Kobas_DB[4] + 1)

Else - Actions
Set Kobas_DB[4] = 1

Trigger - Run Kobas Button Edit <gen> (ignoring conditions)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to Kobas_Button_OK

Then - Actions
Game - Display to (All players) for 20.00 seconds the text: |c00FF0303Game Opti...

Game - Display to (All players) for 20.00 seconds the text: |c00FFFC01---------...

Game - Display to (All players) for 20.00 seconds the text: (|c00FEBA0EGame Mode: |r + Kobas_String_GameMode[Kobas_DB[1]])

Game - Display to (All players) for 20.00 seconds the text: (|c00FEBA0EDifficulty: |r + Kobas_String_Difficulty[Kobas_DB[2]])

Game - Display to (All players) for 20.00 seconds the text: (|c00FEBA0EBattle Arena: |r + Kobas_String_BattleArena[Kobas_DB[3]])

Game - Display to (All players) for 20.00 seconds the text: (|c00FEBA0ESpecial Event: |r + Kobas_String_SpecialEvent[Kobas_DB[4]])

-----------------------------------------------------------

HERE GO IF THEN ELSE FUNCTIONS THAT DO THINGS YOU SELECTED ABOVE!

EXAMPLE:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Kobas_DB[1] Equal to 1

Then - Actions
Trigger - Run GM Default <gen> (ignoring conditions)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Kobas_DB[1] Equal to 2

Then - Actions
Trigger - Run GM All Pick <gen> (ignoring conditions)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Kobas_DB[1] Equal to 3

Then - Actions
Trigger - Run GM All Random <gen> (ignoring conditions)

Else - Actions

-----------------------------------------------------------

This trigger above is most needed part, it calculate number of clicks, set new string values (button names) and so on.


Example below will check did you clicked on right button:

Or - Any (Conditions) are true
Conditions
(Clicked dialog button) Equal to Kobas_Button_SpecialEvent[1]

(Clicked dialog button) Equal to Kobas_Button_SpecialEvent[2]

(Clicked dialog button) Equal to Kobas_Button_SpecialEvent[3]

And then set counting integer value:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
Kobas_DB[4] Equal to 1

Kobas_DB[4] Equal to 2

Then - Actions
Set Kobas_DB[4] = (Kobas_DB[4] + 1)

Else - Actions
Set Kobas_DB[4] = 1

If you want to use 10 different buttons you will need to add events here like this:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Kobas_DB[4] Less or Equal to 9

Then - Actions
Set Kobas_DB[4] = (Kobas_DB[4] + 1)

Else - Actions
Set Kobas_DB[4] = 1

That should be easy right!


Kobas_DB[1] use 1st button, Kobas_DB[2] use 2nd button and so on...


Trigger below edit button values (set them to new ones) and show buttons again!

Kobas Button Edit
Events

Conditions

Actions
-------- We use this trigger actions to edit our dialog --------

Dialog - Clear Kobas_Dialog

Dialog - Change the title of Kobas_Dialog to |c00FF0303Game Opti...

Dialog - Create a dialog button for Kobas_Dialog labelled (|c00FEBA0EGame Mode: |r + Kobas_String_GameMode[Kobas_DB[1]])

Set Kobas_Button_GameMode[Kobas_DB[1]] = (Last created dialog Button)

Dialog - Create a dialog button for Kobas_Dialog labelled (|c00FEBA0EDifficulty: |r + Kobas_String_Difficulty[Kobas_DB[2]])

Set Kobas_Button_Difficulty[Kobas_DB[2]] = (Last created dialog Button)

Dialog - Create a dialog button for Kobas_Dialog labelled (|c00FEBA0EBattle Arena: |r + Kobas_String_BattleArena[Kobas_DB[3]])

Set Kobas_Button_BattleArena[Kobas_DB[3]] = (Last created dialog Button)

Dialog - Create a dialog button for Kobas_Dialog labelled (|c00FEBA0ESpecial Effects: |r + Kobas_String_SpecialEvent[Kobas_DB[4]])

Set Kobas_Button_SpecialEvent[Kobas_DB[4]] = (Last created dialog Button)

Dialog - Create a dialog button for Kobas_Dialog labelled |c0020C000OK!|r

Set Kobas_Button_OK = (Last created dialog Button)

Dialog - Show Kobas_Dialog for Player 1 (Red)

And that's it!

Author's notes:


This DB systems are not perfect! You can find a lot better systems than mine (as well as a lot of worse ones! And I do mean A LOT!!!)


If this tutorial helped you to understand how the DB works (or at least gave you a glimpse of it), then my job here is done!


Special thanks to CHUNKfor:

2. Let me explain some things about dialog buttons

#### Attachments

Kobas Dialogs.w3x

 20 KB

 · Views: 1,107



DB 01.jpg

 119.6 KB

 · Views: 7,667



DB 02.jpg

 121.3 KB

 · Views: 6,242



DB 03.jpg

 298.5 KB

 · Views: 6,280




 Last edited by a moderator: Jan 6, 2011

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice job. Dialogs are very annoying, but I'm sure people will have a much easier time after reading this.

 WORKING ON IT


Click to expand...


So, the tutorial is not finished yet? If so, then tell me and I'll keep it in submission. At the moment, it is pretty ready to be approved. Keep it up.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Thanks, but yes, I still want to explain 2 big triggers below ^^

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

BUMP

Here you go buddy, it's finished!

Please guys report any grammar errors and shitz you find unexplained or bad created! After all we all constantly learn ♥

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Pretty good tutorial.


One thing that youcould mentions is that if one uses dialog arrays, then always remember to se the initial size to n+1, where n is the max index used from the array.


For example you create 4 dialogs, and set them to [0] , [1] , [2] and [3] , then you must set the initial size to 4.


To make it more simple, dialogs must be initialized in order to be used, and they won't be automatically. There are two ways to do it. Either by adjusting the initial size in variable editor, or with

set udg_Dialogs[2] = DialogCreate()

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Thanks

But dialog buttons are totally different than other things, once created they can be shown or removed. We use variables to save informations!

Also I always use array variables from 1...

So if I understand you right you wanted to say that we could save whole dialogs instead of creating them each time, right?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

He's basically saying that if you make an array variable, the dialogs are not initialized for the most part except up to the index specified.


It is nice to know, and can save you from some annoying debugging, but I'll still approve the tutorial as is. Nice job.


~Approved.

#### Orcnet

#### Orcnet

 Level 30

JoinedJul 31, 2010Messages5,248

Resources: 5

#### Resources
Maps 4Tutorials 1

Good Good Good!

This one helps very much!

#### x3GlikE

#### x3GlikE

 Level 11

JoinedJul 7, 2010Messages709

Resources: 11

#### Resources
Icons 3Maps 1Spells and Systems 7

Holy Sh*t very nice -kobas-

#### millzy

#### millzy

 Level 15

JoinedJul 9, 2008Messages1,553

Resources: 7

#### Resources
Maps 5Spells and Systems 2

this tutoirial is great gave me a greate idea of instead of swapign the dialogs around to atacherly open up new ones to set the modes instead of cycleing though the modes. problem is thats alot of if then elses

#### theHunt_errr

#### theHunt_errr

 Level 5

JoinedJul 18, 2009Messages145

Resources: 3

#### Resources
Maps 3

Nice  this made me add advance dialog to my wmw, yeah a wmw, but i made it much shorter and in roc!

#### Deleted member 177737

D

#### Deleted member 177737

I already knew how to do everything in this tutorial, but if this tutorial was around when I needed to learn how to create them it would have saved me hours of googling.


Great job!

#### cris333

C

#### cris333

i am new to this , i have added Time - Elapsed game time is 0.00 seconds , changed Title to title ,but cant change that Dialog word with that wrong option to clicked dialog and i dont know how to proceed, it says load value of value....Sorry but your tutorial is lacking in first steps for beginners.Any advices ?

I have created tutorials for anno 1404 venice game editor for any beginners not for advanced ones.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

As I get it you want the title to be equal to the text on the last pressed button?

make sure the "string_variable" matches the actual text on the button of choice.

Untitled Trigger 001
Events
Dialog - A dialog button is clicked for my_dialog

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to dialog_buttons[1]

Then - Actions
Set string_variable = some random text the happen to be on the first button.

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to dialog_buttons[2]

Then - Actions
Set string_variable = some random text the happen to be on the second button.

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to dialog_buttons[3]

Then - Actions
Set string_variable = some random text the happen to be on the second button.

Else - Actions

then just use:

Dialog - Change the title of my_dialog to string_variable

#### Map Designer

#### Map Designer

 Level 17

JoinedMay 2, 2011Messages1,351

Resources: 6

#### Resources
Maps 2Spells and Systems 4

this will be long if you have 9 weathers or 9 difficulties

Edit: wait I think I can shorten this trigger by half just by inverting the button then vs else in Kobas_DB[x] condition

in the last case 9 you will push Kobas_DB back to 1, and on all other cases you will add+1

im pretty sure I can shrink the other half by some kind of custom script loop (or even GUI loop), but that goes beyond my level

Edit: I also notice the end of your initialization trigger (0 time elapsed trigger) isthe same as Edit Buttons trigger.

instead of repeating the lines, it might be better if you just ask the initial trigger to (run Kobas_Button_Edit trigger, ignoring conditions)

anyway here is my GUI code after this minor change:

MapDesignerButtonClick
Events
Dialog - A dialog button is clicked for Training_Grounds_Dialog

Conditions

Actions
-------- We check witch dialog button was clicked --------

-------- We added 1 if then else function under another to prevent some bugs! --------

-------- Still because we use this on start and there are not to many triggers turned on, this won't change performance! --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Clicked dialog button) Equal to Training_Button_Measure[1]

(Clicked dialog button) Equal to Training_Button_Measure[2]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Training_Grounds_DB[1] Equal to 1

Then - Actions
Set Training_Grounds_DB[1] = 2

Else - Actions
Set Training_Grounds_DB[1] = 1

Trigger - Run MapDButtonEdit <gen> (ignoring conditions)

Else - Actions
-------- First Digit --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Clicked dialog button) Equal to Training_Button_Amount01[0]

(Clicked dialog button) Equal to Training_Button_Amount01[1]

(Clicked dialog button) Equal to Training_Button_Amount01[2]

(Clicked dialog button) Equal to Training_Button_Amount01[3]

(Clicked dialog button) Equal to Training_Button_Amount01[4]

(Clicked dialog button) Equal to Training_Button_Amount01[5]

(Clicked dialog button) Equal to Training_Button_Amount01[6]

(Clicked dialog button) Equal to Training_Button_Amount01[7]

(Clicked dialog button) Equal to Training_Button_Amount01[8]

(Clicked dialog button) Equal to Training_Button_Amount01[9]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Training_Grounds_DB[2] Greater than or equal to 9

Then - Actions
Set Training_Grounds_DB[2] = 0

Else - Actions
Set Training_Grounds_DB[2] = (Training_Grounds_DB[2] + 1)

Trigger - Run MapDButtonEdit <gen> (ignoring conditions)

Else - Actions
-------- Second Digit --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Clicked dialog button) Equal to Training_Button_Amount02[0]

(Clicked dialog button) Equal to Training_Button_Amount02[1]

(Clicked dialog button) Equal to Training_Button_Amount02[2]

(Clicked dialog button) Equal to Training_Button_Amount02[3]

(Clicked dialog button) Equal to Training_Button_Amount02[4]

(Clicked dialog button) Equal to Training_Button_Amount02[5]

(Clicked dialog button) Equal to Training_Button_Amount02[6]

(Clicked dialog button) Equal to Training_Button_Amount02[7]

(Clicked dialog button) Equal to Training_Button_Amount02[8]

(Clicked dialog button) Equal to Training_Button_Amount02[9]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Training_Grounds_DB[3] Greater than or equal to 9

Then - Actions
Set Training_Grounds_DB[3] = 0

Else - Actions
Set Training_Grounds_DB[3] = (Training_Grounds_DB[3] + 1)

Trigger - Run MapDButtonEdit <gen> (ignoring conditions)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Clicked dialog button) Equal to Training_Button_Amount11[0]

(Clicked dialog button) Equal to Training_Button_Amount11[1]

(Clicked dialog button) Equal to Training_Button_Amount11[2]

(Clicked dialog button) Equal to Training_Button_Amount11[3]

(Clicked dialog button) Equal to Training_Button_Amount11[4]

(Clicked dialog button) Equal to Training_Button_Amount11[5]

(Clicked dialog button) Equal to Training_Button_Amount11[6]

(Clicked dialog button) Equal to Training_Button_Amount11[7]

(Clicked dialog button) Equal to Training_Button_Amount11[8]

(Clicked dialog button) Equal to Training_Button_Amount11[9]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
Training_Grounds_DB[4] Greater than or equal to 9

Then - Actions
Set Training_Grounds_DB[4] = 0

Else - Actions
Set Training_Grounds_DB[4] = (Training_Grounds_DB[4] + 1)

Trigger - Run MapDButtonEdit <gen> (ignoring conditions)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Clicked dialog button) Equal to Training_Button_Amount12[0]

(Clicked dialog button) Equal to Training_Button_Amount12[1]

(Clicked dialog button) Equal to Training_Button_Amount12[2]

(Clicked dialog button) Equal to Training_Button_Amount12[3]

(Clicked dialog button) Equal to Training_Button_Amount12[4]

(Clicked dialog button) Equal to Training_Button_Amount12[5]

(Clicked dialog button) Equal to Training_Button_Amount12[6]

(Clicked dialog button) Equal to Training_Button_Amount12[7]

(Clicked dialog button) Equal to Training_Button_Amount12[8]

(Clicked dialog button) Equal to Training_Button_Amount12[9]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Training_Grounds_DB[5] Greater than or equal to 9

Then - Actions
Set Training_Grounds_DB[5] = 0

Else - Actions
Set Training_Grounds_DB[5] = (Training_Grounds_DB[5] + 1)

Trigger - Run MapDButtonEdit <gen> (ignoring conditions)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to Training_Grounds_Button_OK

Then - Actions
Set Training_Ground_Temp_String = |c00FF0303Training Squad:|r

Game - Display to (All players) for 20.00 seconds the text: Training_Ground_Temp_String

Set Training_Ground_Temp_String = |c00FFFC01------------------------------------|r

Game - Display to (All players) for 20.00 seconds the text: Training_Ground_Temp_String

Set Training_Ground_Temp_String = (|c00FF0303First Unit: |r + (String(((10 x Training_Grounds_DB[3]) + Training_Grounds_DB[2]))))

Game - Display to (All players) for 20.00 seconds the text: Training_Ground_Temp_String

Set Training_Ground_Temp_String = (|c00FF0303Second Unit: |r + (String(((10 x Training_Grounds_DB[5]) + Training_Grounds_DB[4]))))

Game - Display to (All players) for 20.00 seconds the text: Training_Ground_Temp_String

Else - Actions


 Last edited: Feb 4, 2021

#### Njord

#### Njord

 Level 16

JoinedJun 11, 2018Messages300

Resources: 11

#### Resources
Maps 1Ported Models 7Ported Icons 3

I don't understand how to create more game modes. The tutorial just says "If you want to add few more game modes, add more string variable and list them above just like we did in our example!"

List them above? What example? Can someone help me with this?

 You must log in or register to reply here.


### Similar threads

 Altering Hero Stats For Noobs


Faith

May 1, 2013

Trigger (GUI) Editor Tutorials

Replies24Views11K

Oct 2, 2015
XxkosumaxX

 Basics of a Dialog


Orc_Tamer

Mar 7, 2007

Trigger (GUI) Editor Tutorials

Replies6Views29K

Dec 5, 2019
tough_zai

Locked

 Dialog Box Problems


BradPittlord

Jul 19, 2019

World Editor Help Zone

Replies2Views2K

Jul 21, 2019
BradPittlord

 Hero Selection Systems


Eleandor

Aug 20, 2007

Trigger (GUI) Editor Tutorials
2

Replies51Views81K

Feb 28, 2021
Caalanuaa

 Hero Selection Systems


Tonks

Mar 23, 2007

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jun 8, 2023
dantles1992

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