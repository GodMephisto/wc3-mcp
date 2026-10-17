---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-a-shared-income-mode-as-a-percentage-of-total-income.311844/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating a Shared Income Mode as a Percentage of Total Income
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-a-shared-income-mode-as-a-percentage-of-total-income.311844/>
#### Eric Jensen

#### Eric Jensen

 Level 4

JoinedOct 11, 2018Messages6

Resources: 4

#### Resources
Maps 1Tutorials 3

## Introduction

Shared Income Mode is a feature used in many custom games: It causes each player in the game to receive the same amount of gold as the other players, regardless of how much gold that player himself earned.

However, maybe you do not want 100% of all income to be shared - that would be unfair! Perhaps you only want a percentage of all income to be shared as a way of lightly "rubber banding" players together, and keeping noobs from falling too far behind in the game. For example, in my map, Azure TD Remastered, I make it so that only 33% of each player's income is evenly distributed among allies.

The triggers to do this are actually pretty simple and versatile. I will show you a step-by-step process for creating a Shared Income system that only distributes a percentage of each player's income.

### Step 1: Count Human Players

The first thing we need to do is create 2 variables: a Player Group variable that includes only Human, non-computer players, and an integer variable that keeps track of the number of Human players. The reason for this is because we may not want to include computer players in the Shared Income system. In melee games, computers earn gold differently than Human players, which can mess up the Shared Income system. In custom games, computers usually only exist as "dummy" players or players meant only to fill a game slot. We will want to exclude them from the Shared Income system. If you, by chance, have a working AI that you want to include into your Shared Income system, then feel free to add computer players into these variables as well:

Count Human Players
Events
Map initialization

Conditions

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked player) controller) Equal to User

Then - Actions
Player Group - Add (Picked player) to ActiveHumanPlayers

Set ActiveHumanPlayerCount = (ActiveHumanPlayerCount + 1)

Else - Actions
Do nothing

As I said, this trigger will keep track of both the players and the number of players that are to be included in the Shared Income system.

### Step 2: Create a Dummy "Distributor" Player

Our next step is to create a dummy player that will be our "distributor"; all the income that is to be distributed will be taxed to this player, and then this player will evenly distribute his received gold back to the other players.

We will do this by setting a gold threshold, that, when the dummy player's gold reaches this threshold, it distributes all its gold evenly back to each player. I recommend setting this threshold to a number that is low enough that it will provide constant income, but high enough that it can be divided among each player without creating rounding issues that will affect the total amount of gold flowing back out to players. In my map, which can have up to 8 players, I set this threshold to 50:

Redistribute Income
Events
Player - Player 12 (Brown)'s Current gold becomes Greater than or equal to 50.00

Conditions

Actions
Set GoldToBeDistributed = ((Player 12 (Brown) Current gold) / ActiveHumanPlayerCount)

Player Group - Pick every player in ActiveHumanPlayers and do (Actions)
Loop - Actions
Player - Add GoldToBeDistributed to (Picked player) Current gold

Player - Set Player 12 (Brown) Current gold to 0

As you can see, this trigger works by creaing a variable, GoldToBeDistributed, that is a quotient of the dummy player's current gold and the number of Human Players. The trigger gives each Human player the amount GoldToBeDistributed, and then resets the dummy player's gold.

### Step 3: Tax each Player the % of Shared Income

However, in order for our "distributor" player to actually do anything, he will need to receive gold from the other players that he can evenly distribute back out. This is done through another trigger that we will use to divert a percentage of each player's income to our dummy player.

Divert Income
Events

Conditions

Actions
Game - Display to (All players) the text: |c00FFFF00Shared In...

Player - Set Player 12 (Brown) Current gold to 25

Player Group - Pick every player in ActiveHumanPlayers and do (Actions)
Loop - Actions
Player - Divert 33% of the Gold income of (Picked player) to Player 12 (Brown)

In my map, this trigger activates when "Shared Income Mode" is picked in the game.

The "33%" is the key number in this trigger - it specifies exactly how much of each player's income will end up being shared. You can change this number to something like 20%, 50%, or even go full-on communism and tax each player 100% of their income. You can even create a voting system that allows players to vote on what % of income may be shared, using a similar set of triggers to those found in this tutorial: Creating a Mode Voting System that averages Player Votes.

## Bonus: Diverting Leaver Income

Another function of this trigger is that it can allow you to divert income from leavers to other players. You may not always want this feature - most games usually simply give full control of leavers and allow other players to spend the leaver's gold. However, in some custom games you may simply want the leaver's gold to be distributed out for other players to spend. To do this, simply make a trigger that diverts all of the leaver's current gold as well as his future income to our dummy "distributor" player.

Give Leaver Gold and Control
Events
Player - Player 1 (Red) leaves the game

Player - Player 2 (Blue) leaves the game

Player - Player 3 (Teal) leaves the game

Player - Player 4 (Purple) leaves the game

Player - Player 5 (Yellow) leaves the game

Player - Player 6 (Orange) leaves the game

Player - Player 7 (Green) leaves the game

Player - Player 8 (Pink) leaves the game

Conditions

Actions
Player Group - Remove (Triggering player) from ActiveHumanPlayers

Set ActiveHumanPlayerCount = (ActiveHumanPlayerCount - 1)

Set LeaverGold = ((Triggering player) Current gold)

Player Group - Pick every player in ActiveHumanPlayers and do (Actions)
Loop - Actions
Player - Add (LeaverGold / ActiveHumanPlayerCount) to (Picked player) Current gold

Set LeaverGold = 0

Player - Set (Triggering player) Current gold to 0

Game - Grant shared vision and full shared unit control of (Triggering player) units with his/her allies

Player - Divert 100% of the Gold income of (Triggering player) to Player 12 (Brown)

Notice how I also removed the player from our Human Players variables, so that he would not receive any gold back from Player 12 after he left.

## Conclusion

That concludes this tutorial on creating Shared Income modes. I hope that these triggers will be useful in future games and maps that you guys create!

-Eric Jensen [BYU_FOOTBALL]

#### Ken Christopher

#### Ken Christopher

 Level 7

JoinedApr 18, 2007Messages247

Resources: 6

#### Resources
Icons 6

Nice one. Good tutorial.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Good tutorial, though integer rounding can be a bit off but they're shitty anyway.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I am quite split on this at the moment.

On one hand, the triggers required are very few, which is a big pro.

However, I really dislike encouraging a method that puts one player out of the game.

Here is a pure GUI solution which I came up with.

init
Events
Map initialization

Conditions

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
Set VariableSet gold_tracker[(Player number of (Picked player))] = ((Picked player) Current gold)

Player Group - Add (Picked player) to player_group

do stuff
Events
Player - Player 1 (Red)'s Current gold becomes Greater than 0.00

Conditions

Actions
Set VariableSet gained = (((Triggering player) Current gold) - gold_tracker[(Player number of (Triggering player))])

Game - Display to (All players) the text: (String(gained))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
gained Greater than 0

Then - Actions
Game - Display to (All players) the text: Income is positive

Set VariableSet gained = (gained / 3)

Trigger - Turn off (This trigger)

Player - Set (Triggering player).Current gold to (((Triggering player) Current gold) - gained)

Trigger - Turn on (This trigger)

Set VariableSet gold_tracker[(Player number of (Triggering player))] = ((Triggering player) Current gold)

Game - Display to (All players) the text: (String(gained))

Set VariableSet player_group = (All players)

Player Group - Remove (Triggering player) from player_group.

Set VariableSet size = (Number of players in player_group)

Game - Display to (All players) the text: (String(size))

Set VariableSet math = (gained / size)

Game - Display to (All players) the text: (Added + (String(math)))

Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
Trigger - Turn off (This trigger)

Player - Add math to (Picked player).Current gold

Trigger - Turn on (This trigger)

Player Group - Add (Triggering player) to player_group

Else - Actions
Set VariableSet gold_tracker[(Player number of (Triggering player))] = ((Triggering player) Current gold)

Game - Display to (All players) the text: Income is negative

This is not a finished example as you can see, it only works if a unit from player 1 dies and there are no config variables.

I am also unsure if turning the trigger off might cause the system to fail if you are unlucky and a player gains gold while the trigger is disabled (to prevent infinite loop)

I am fairly certain I could fix with with a few lines of JASS code however.

In short, I think the method can be improved.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Graveyarded, a fair bit of time has passed.

 You must log in or register to reply here.


### Similar threads

 Creating an income that loops


Parrothead

Sep 22, 2007

Tutorial Archive

Replies3Views2K

Nov 17, 2007
PurplePoot

 Tutorial: Easy income system


Hordeon

Jul 6, 2009

Tutorial Archive

Replies17Views5K

Jul 12, 2009
Pyritie

 Simple income trigger


Arisato

Feb 6, 2010

Tutorial Archive

Replies5Views3K

Feb 9, 2010
Kwah

Poll

 Creating an income that loops [Awaiting Improving: Ralle]


Parrothead

Feb 10, 2008

Tutorial Archive

Replies14Views3K

Sep 7, 2008
Hoernchen

Locked

 Income Trigger???


iown_azz

Jul 11, 2010

Triggers & Scripts

Replies14Views3K

Jul 13, 2010
arielavi

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