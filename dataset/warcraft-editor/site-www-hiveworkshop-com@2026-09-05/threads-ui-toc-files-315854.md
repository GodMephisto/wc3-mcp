---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-toc-files.315854/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Toc Files
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-toc-files.315854/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

##
 Introduction

A TOC file is a file listing one or more fdf (frame definition files). Each line is a path to one fdf. Each fdf is an own file. In Warcraft 3 this TOC-Files are used to load the fdfs beeing listed.​

​

After a fdf was loaded into the game using a TOC lisiting it, main-Frames defined in the fdf can be created. Main-Frames are in my talking Frame definitions outside of Blocks { }.​

​

A TOC-File is not case sensitve.​

​

One can load a TOC-File using the native provided in Warcraft 3 1.31.​

native BlzLoadTOCFile takes string TOCFile returns boolean​

it returns true if the TOC file was loaded successfuly. It might be smart to create a new function telling your, if a TOC load failed/succeeded.​

​

On default "ui/framedef/framedef.toc" is loaded, it contains most default fdf of warcraft 3.​

​

 Code:

UI\FrameDef\GlobalStrings.fdf
UI\FrameDef\InfoPanelStrings.fdf
UI\FrameDef\NetworkStrings.fdf
UI\FrameDef\DateStrings.fdf
UI\FrameDef\Glue\DialogWar3.fdf
UI\FrameDef\Glue\MapInfoPane.fdf
UI\FrameDef\Glue\AdvancedOptionsDisplay.fdf
UI\FrameDef\Glue\AdvancedOptionsPane.fdf
UI\FrameDef\Glue\ListBoxWar3.fdf
UI\FrameDef\Glue\MapListBox.fdf
UI\FrameDef\Glue\DecoratedMapListBox.fdf
UI\FrameDef\Glue\MapPreferenceBox.fdf
UI\FrameDef\Glue\CheckListBox.fdf
UI\FrameDef\Glue\MainMenu.fdf
UI\FrameDef\Glue\MovieScreen.fdf
UI\FrameDef\Glue\Loading.fdf
UI\FrameDef\Glue\LoadSavedGameScreen.fdf
UI\FrameDef\Glue\OptionsMenu.fdf
UI\FrameDef\Glue\SinglePlayerMenu.fdf
UI\FrameDef\Glue\ViewReplayScreen.fdf
UI\FrameDef\Glue\CustomCampaignMenu.fdf
UI\FrameDef\Glue\CampaignMenu.fdf
UI\FrameDef\Glue\TeamSetup.fdf
UI\FrameDef\Glue\PlayerSlot.fdf
UI\FrameDef\Glue\Skirmish.fdf
UI\FrameDef\Glue\ScoreScreen.fdf
UI\FrameDef\Glue\LocalMultiplayerJoin.fdf
UI\FrameDef\Glue\LocalMultiplayerCreate.fdf
UI\FrameDef\Glue\LocalMultiplayerLoad.fdf
UI\FrameDef\Glue\GameChatroom.fdf
UI\FrameDef\Glue\CampaignListBox.fdf
UI\FrameDef\Glue\BattleNetMain.fdf
UI\FrameDef\Glue\BattleNetMatchmakerPanel.fdf
UI\FrameDef\Glue\BattleNetNewsBox.fdf
UI\FrameDef\Glue\BattleNetUserListBox.fdf
UI\FrameDef\Glue\BattleNetChatPanel.fdf
UI\FrameDef\Glue\BattleNetChatActionMenu.fdf
UI\FrameDef\Glue\BattleNetStandardPanel.fdf
UI\FrameDef\Glue\BattleNetTeamPanel.fdf
UI\FrameDef\Glue\BattleNetTeamInvitation.fdf
UI\FrameDef\Glue\BattleNetProfilePanel.fdf
UI\FrameDef\Glue\BattleNetCustomJoinPanel.fdf
UI\FrameDef\Glue\BattleNetCustomCreatePanel.fdf
UI\FrameDef\Glue\BattleNetCustomLoadPanel.fdf
UI\FrameDef\Glue\BattleNetScheduledGame.fdf
UI\FrameDef\Glue\BattleNetChatroom.fdf
UI\FrameDef\Glue\BattleNetFriendsListBox.fdf
UI\FrameDef\Glue\BattleNetFriendsPane.fdf
UI\FrameDef\Glue\BattleNetClanMateListBox.fdf
UI\FrameDef\Glue\BattleNetClanPane.fdf
UI\FrameDef\Glue\BattleNetClanInvitation.fdf
UI\FrameDef\Glue\BattleNetStatusBox.fdf
UI\FrameDef\Glue\BattleNetIconSelectBox.fdf
UI\FrameDef\UI\ConsoleUI.fdf
UI\FrameDef\UI\AllianceSlot.fdf
UI\FrameDef\UI\AllianceDialog.fdf
UI\FrameDef\UI\ChatDialog.fdf
UI\FrameDef\UI\LogDialog.fdf
UI\FrameDef\UI\EscMenuMainPanel.fdf
UI\FrameDef\UI\EscMenuOptionsPanel.fdf
UI\FrameDef\UI\EscMenuSaveGamePanel.fdf
UI\FrameDef\UI\GameResultDialog.fdf
UI\FrameDef\UI\UpperButtonBar.fdf
UI\FrameDef\UI\ResourceBar.fdf
UI\FrameDef\UI\SimpleInfoPanel.fdf
UI\FrameDef\UI\SuspendDialog.fdf
UI\FrameDef\UI\UnresponsiveDialog.fdf
UI\FrameDef\UI\ReplayPanel.fdf
UI\FrameDef\UI\ObserverPanel.fdf
UI\FrameDef\UI\QuestDialog.fdf
UI\FrameDef\UI\TimerDialog.fdf
UI\FrameDef\UI\Leaderboard.fdf
UI\FrameDef\UI\Multiboard.fdf
UI\FrameDef\UI\CinematicPanel.fdf
UI\FrameDef\UI\ScriptDialog.fdf
UI\FrameDef\UI\GameSaveSplashDialog.fdf
UI\FrameDef\UI\BrowserFrame.fdf

Although there are 3 further default fdf, providing quite useful basic frame templates which might be good to have in your map, if one does not create own custom fdf.​

 Code:

ui\framedef\ui\escmenutemplates.fdf[
ui\framedef\glue\standardtemplates.fdf
ui\framedef\glue\battlenettemplates.fdf

The path written in the TOC file is the file the game will load. For imported fdf write the path you imported it into your map.​

It is recommented to add an empty line at the end of your TOC-Files text. It failed on the ptr without the empty ending line.​

##
 Example

"myStatusBar.toc" imported to "war3mapImported\myStatusBar.toc"​

"myStatusbar.fdf" imported to "war3mapImported\myStatusBar.fdf"​

​

TOC-Content​

​

in this example this line of code would load the imported TOC-File.​

call BlzLoadTOCFile("war3mapimported\\mystatusbar.toc")​

​

TOC-Files are important when you want to create new frames or load further fdf.​

​

Edit: Added a zip containing "Templates.toc" it is loading the 2 templates mentioned.​

###
 Other UI-Frame Tutorials

The following links might provide more insight into this subject.​

UI: Change Lumber Text​

[JASS/AI] - UI: Create a TextButton​

​

[JASS/AI] - UI: Positionate Frames (important)​

UI: toc-Files​

UI: Reading a FDF​

UI - The concept of Parent-Frames​

[JASS/AI] - UI: FrameEvents and FrameTypes​

UI: Frames and Tooltips​

​

[JASS/AI] - UI: Creating a Bar​

UI - Simpleframes​

​

UI: What are BACKDROPs?​

UI: GLUEBUTTON​

UI: TEXTAREA the scrolling Text Frame​

UI: EditBox - Text Input​

[JASS/AI] - UI: Creating a Cam control​

​

UI: Showing 3 Multiboards​

​

UI: OriginFrames​

Default Names for BlzGetFrameByName (Access to Existing game-Frames)​

[JASS/AI] - UI: List - Default MainFrames (built in CreateAble)​

#### Attachments

Templates.zip

 181 bytes

 · Views: 569




 Last edited: Mar 9, 2023

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Probably stands for 'Table Of Contents'.

The paths in the file are not case-sensitive? Since the mpq archive uses some hash algorithm to locate a file, that's a bit strange. Either this must already not consider case or mentioning the FDFs in listfile would be a must?

Can you reference another toc in a toc?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

TOC beeing an acronym for Table of Contents would make sense.

No, one can not load a toc in a toc. I would kinda doubt the practical use of it, if it would be possible.

#### GetLocalPlayer

#### GetLocalPlayer

 Level 4

JoinedApr 11, 2018Messages24

Tasyen said:

 It is recommented to add an empty line at the end of your TOC-Files text. It failed on the ptr without the empty ending line.


Click to expand...

It depends on the end of line sequence of the text file you're working in. If it's "CRLF", you need at least 1 empty line. If it's "LF", you need at least 2 empty lines.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I am cautious about this one.

The introduction gives a brief rundown of what the file format and a few fun facts about how it works in the background. This I have no problem with. But the important knowledge is just 3-4 sentences of information.

Importing a file is basic knowledge and does not really need to be taught. There are other tutorials that cover such.

Lastly, loading the file is also very useful but can also be explained in a single line.

This leaves the tutorial in my mind to feel extremely short. Yet those few lines are useful.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

What is wrong with short tutorials?

Unlike my other UI-Tutorials this one can be of use to people that don't create custom UI themself. They can change the paths fdf/tocs take in their map to fit their file structure.

Any of my tutorials can be graveyarded, if you like.

 You must log in or register to reply here.


### Similar threads

 UI Frames, starting guide


Anchorist

Sep 8, 2019

Tutorial Archive

Replies2Views9K

Oct 3, 2020
Chaosy

 UI: Showing 3 Multiboards


Tasyen

Jun 25, 2019

Tutorial Archive

Replies11Views7K

Aug 30, 2023
orgiamode

 UI: Reading a FDF


Tasyen

May 29, 2019

Scripting Tutorials

Replies15Views16K

Dec 1, 2020
Tasyen

 UI: Change Lumber Text


Tasyen

May 29, 2019

Scripting Tutorials

Replies17Views9K

Apr 1, 2021
pr114

 UI: TEXTAREA the scrolling Text Frame


Tasyen

Sep 20, 2019

Scripting Tutorials

Replies20Views9K

Dec 6, 2021
Seago

S

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top