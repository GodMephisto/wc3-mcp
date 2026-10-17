---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-a-mode-voting-system-that-averages-player-votes.311825/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating a Mode Voting System That Averages Player Votes
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-a-mode-voting-system-that-averages-player-votes.311825/>
#### Eric Jensen

#### Eric Jensen

 Level 4

JoinedOct 11, 2018Messages6

Resources: 4

#### Resources
Maps 1Tutorials 3

##
 Introduction

Many custom maps in Warcraft 3 have difficulty modes in order to allow players to play the game at their appropriate difficulty level. In multiplayer games, the game will often allow all the players to vote on the difficulty mode; however, almost all of these voting systems simply tell the game to select the difficulty mode that received the most votes from players. This can create problems in some situations when, say, a game is filled with either expert players or noobs, with few or no average players. The expert players might vote to play "Hard" mode and the noobs "Easy" mode, and the game will then have to select either Hard or Easy mode, even though the average of all the players might have been best fit for "Medium" difficulty. If the game selects Hard difficulty, then the game will be too difficult for the average of the players. On the other hand, selecting Easymode would make the game too easy to be enjoyable.

What if it were possible to create a mode voting system that would average player votes? For example, if the votes are split half-and-half between Easy and Hard difficulty, could we make the game pick Medium difficulty as a mode that is best suited for the average of the players' skill levels? In order to get a computer to average anything, we would need to assign number values to different difficulty modes, and figure out a way to average those numbers and convert the new average back to an appropriate difficulty mode, which sounds like both a lot of math and a lot of trigger work. Fortunately, its a bit more simple than it might seem at first.

This "voting average" system can be useful in a variety of situations. Here I will show you how to create a basic Difficulty Vote averaging system:

### Step 1: Create a Voting Dialog

The first thing that we need to do is allow players to vote on a difficulty mode. While there are several methods of doing this, the easiest and best-looking way to take a vote is to create a dialog with a clickable button for each mode. There are many tutorials on HIVE and other websites that go into detail on how to make and customize dialogs that you can use as a resource. I will post an example dialog trigger that creates a difficulty vote in my map, Azure TD Remastered:

Show Difficulty Vote Menu
Events
Time - Elapsed game time is 0.01 seconds

Conditions

Actions
Dialog - Clear DifficultyMode

Dialog - Change the title of DifficultyMode to Choose ...

Dialog - Create a dialog button for DifficultyMode labelled |c00B6FFB6Easy (cre...

Set EasyDifficulty = (Last created dialog Button)

Dialog - Create a dialog button for DifficultyMode labelled |c005050FFNormal (c...

Set NormalDifficulty = (Last created dialog Button)

Dialog - Create a dialog button for DifficultyMode labelled |c00FF0000Hard (cre...

Set HardDifficulty = (Last created dialog Button)

Dialog - Create a dialog button for DifficultyMode labelled |c00FFCC00Expert (c...

Set ExpertDifficulty = (Last created dialog Button)

Player Group - Pick every player in ActiveHumanPlayers and do (Dialog - Show DifficultyMode for (Picked player))

As you can see, this creates a Voting dialog that will appear on each player's screen at the start of the game. It has 4 buttons; one for each of the four difficulty levels: Easy, Normal, Hard, and Expert. In my map, the difficulty modes affect the armor and movement speed of enemies.

Along with the Voting Dialog, you should also create a Voting Timer specifying how long players have to vote on a difficulty mode:

Create Vote Timer
Events
Time - Elapsed game time is 0.01 seconds

Conditions

Actions
Countdown Timer - Create a timer window for VoteTimer with title Time Left to Vote

Set VoteTimerWindow = (Last created timer window)

Countdown Timer - Show VoteTimerWindow

Countdown Timer - Start VoteTimer as a One-shot timer that will expire in 30.00 seconds

We will average the votes and select the mode once the timer expires.

### Step 2: Tally the Votes for each Mode

The next thing we need to do is keep track of the votes for each mode, as well as the total number of votes (we will need to use this amount later). To do this we make a trigger that detects when a dialog button is pushed:

Difficulty Button Clicked
Events
Dialog - A dialog button is clicked for DifficultyMode

Conditions

Actions
Dialog - Hide DifficultyMode for (Triggering player)

Set DifficultyVotes = (DifficultyVotes + 1.00)

If ((Clicked dialog button) Equal to ExpertDifficulty) then do (Set ExpertVotes = (ExpertVotes + 1.00)) else do (Do nothing)

If ((Clicked dialog button) Equal to HardDifficulty) then do (Set Hardvotes = (Hardvotes + 1.00)) else do (Do nothing)

If ((Clicked dialog button) Equal to NormalDifficulty) then do (Set NormalVotes = (NormalVotes + 1.00)) else do (Do nothing)

If ((Clicked dialog button) Equal to EasyDifficulty) then do (Set Easyvotes = (Easyvotes + 1.00)) else do (Do nothing)

This trigger detects which dialog button was clicked, hides the dialog (so that players can only vote once), and then adds +1.00 to both the appropriate vote tally and the total vote tally. You will notice from the decimals that I made all the tally values "real" number variables rather than "integer" variables. This is because we will need to divide some of these numbers in a minute and get non-integer values. These variables must be created as "real" variables otherwise our averaging formula will not work with them!

###
 Step 3: Assign a Number Value to each Difficulty Mode

Now we need to assign a number value to each difficulty mode, in order for the computer to average them out. The best way to do this is to use integers starting at 1 for Easy mode, and going up one integer for every difficulty mode as you get to harder modes.

For example:

Easy Mode = 1

Normal Mode = 2

Hard Mode = 3

Expert Mode = 4

and so on.

You do not need to specify these values in any variables - we will simply use these numbers in our averaging function to represent the different difficulty modes.

###
 Step 4: Calculate the Average of the Votes

Here is where we need to start using math. We need to create a formula that adds all of the voting values together and divides them by the total number of votes to find the voting average.

Calculate Voting Average
Events
Time - VoteTimer expires

Conditions

Actions
Countdown Timer - Destroy VoteTimerWindow

Set DifficultyVoteAverage = (((Easyvotes x 1.00) + ((NormalVotes x 2.00) + ((Hardvotes x 3.00) + (ExpertVotes x 4.00)))) / DifficultyVotes)

This function activates when our Voting Timer expires. It multiplies the votes for each mode by their mode's number value, adds all the votes together, and then divides that number by the total number of votes to get the DifficultyVoteAverage.

Because DifficultyVoteAverage is a real number variable, all the other numbers and variables in the function need to be real numbers as well. This is why we createdt all the vote tally variables as "real" variables - the trigger editor will not allow you to use real and integer variables in the same function.

### Step 5: Convert the Vote Average back to a Difficulty Mode

The last thing we need to do is convert DifficultyVoteAverage back into a Difficulty Mode, and then have the game select that difficulty mode to be played.

The concept is pretty simple; our function should have returned a number between 1 and 4; we simply need to round that number to the nearest whole, and then select the difficulty mode that corresponds to the number. For example, if 2 people voted for Hard mode and 1 person voted Normal mode, our function would return DifficultyVoteAverage=(0+1*2+2*3+0)/3=2.67, which rounds to 3, meaning Hard Mode would be selected as the game's difficulty. Meanwhile, a game with 2 Easy votes, 3 Normal votes, 2 Hard votes, and 1 Expert vote would give a voting average of DifficultyVoteAverage=(2*1+3*2+2*3+1*4)/8=2.25, meaning Normal Difficulty would be chosen.

Unfortunately, there is no magical "rounding" button in the triggers, so we will have to measure whether the Voting Average is between certain benchmarks in order to determine the difficulty mode.

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
DifficultyVoteAverage Greater than 3.50

DifficultyVoteAverage Less than or equal to 4.00

Then - Actions

These conditions, for example, will measure to see if the Voting Average falls within the required bounds to select Expert Mode.

You will notice that the rounding is actually incorrect in this function. A Voting Average of exactly 3.5 should round up to 4, yet I specify in the trigger that only values above 3.5 can be used to select Expert Mode, and this is on purpose. Even though it is mathematically correct to round up from a half, you may not want to do this in your trigger as it will result in the game defaulting towards harder difficulty modes.

For example, rounding the Voting Average up would cause a tie between Hard and Expert difficulty votes to result in the game picking Expert difficulty, when you may want the game to err on the side of the easier difficulty in mode selection. For this reason, you may want the game to round halves down instead of up.

In my map, I have the game default towardsNormal Difficulty, meaning that a tie between Easy and Normal modes would result in Normal Difficulty being selected, whereas a tie between Normal and Hard, or Hard and Expert modes would result in the easier mode being selected.

We will specify in our trigger the benchmarks for selecting a Difficulty Mode, and have the trigger select a Difficulty Mode for the game if DifficultyVoteAverage falls between its respective benchmarks. As with our last trigger, you should have this trigger wait until the voting time is over before activating.

Select Difficulty Mode
Events
Time - VoteTimer expires

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
DifficultyVoteAverage Greater than or equal to 1.00

DifficultyVoteAverage Less than 1.50

Then - Actions
Unit - Create 1 Easy Difficulty for Player 11 (Dark Green) at (Center of Research <gen>) facing Default building facing degrees

Game - Display to (All players) for 15.00 seconds the text: |c00B6FFB6Easy Diff...

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
DifficultyVoteAverage Greater than or equal to 1.50

DifficultyVoteAverage Less than or equal to 2.50

Then - Actions
Unit - Create 1 Normal Difficulty for Player 11 (Dark Green) at (Center of Research <gen>) facing Default building facing degrees

Game - Display to (All players) for 15.00 seconds the text: |c005050FFNormal Di...

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
DifficultyVoteAverage Greater than 2.50

DifficultyVoteAverage Less than or equal to 3.50

Then - Actions
Unit - Create 1 Hard Difficulty for Player 11 (Dark Green) at (Center of Research <gen>) facing Default building facing degrees

Game - Display to (All players) for 15.00 seconds the text: |c00FF0000Hard Diff...

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
DifficultyVoteAverage Greater than 3.50

DifficultyVoteAverage Less than or equal to 4.00

Then - Actions
Unit - Create 1 Expert Difficulty for Player 11 (Dark Green) at (Center of Research <gen>) facing Default building facing degrees

Game - Display to (All players) for 15.00 seconds the text: |c00FFCC00Expert Di...

Else - Actions
Do nothing

In my map, selecting a Difficulty Mode creates a unit that gives an aura adding or subtracting armor and movement speed from the monsters in my tower defense. You can make the game run a specific trigger for each mode, or use some other method of implementing the difficulty mode.

## BONUS - Tallying and Averaging Custom Number Values from Players

We can even use a similar set of triggers to average custom string value inputs typed in the chat. This can be used to allow players to vote for a custom value in a mode, such as the HP Percentage that enemy units will have. Here is an example trigger set that does exactly this:

Record HP Votes
Events
Player - Player 1 (Red) types a chat message containing -HPVote as A substring

Player - Player 2 (Blue) types a chat message containing -HPVote as A substring

Player - Player 3 (Teal) types a chat message containing -HPVote as A substring

Player - Player 4 (Purple) types a chat message containing -HPVote as A substring

Conditions
(Substring((Entered chat string), 1, 8)) Equal to -HPVote

Actions
Set HPVotes[HPVoteNumber] = (Real((Substring((Entered chat string), 9, 11))))

Set HPVoteNumber = (HPVoteNumber + 1)

Set HPVotesCount = (HPVotesCount + 1.00)

Average HP Votes
Events
Time - VoteTimer expires

Conditions

Actions
Set HPVoteAverage = ((HPVotes[1] + (HPVotes[2] + (HPVotes[3] + HPVotes[4]))) / HPVotesCount)

Set Monster HP Percentage
Events
Time - VoteTimer expires

Conditions

Actions
Unit Group - Pick every unit in (Units owned by Player 12 (Brown)) and do (Actions)
Loop - Actions
Unit - Set life of (Picked unit) to ((Max life of (Picked unit)) x (HPVoteAverage / 100.00))

The first trigger allows players to input votes by typing in chat "-HPVote xxx", with xxx being a 3-digit number representing the HP Percentage that the player would like enemies to have. The trigger stores each vote in an array (another trigger would have to be implemented to prevent multiple votes from a single player), and also counts the total number of votes that have been cast.

The second trigger then adds all of the values in the HPVotes array and divides them by the HPVoteAmount to get the HPVoteAverage. The third trigger then sets the life of all enemy units to the percentage value of HPVoteAverage!

This set of triggers has great potential and can be used to allow players to vote on custom values of any type, such as the life of units, amount of gold given to players, or the damage of an ability!

## Conclusion

I hope this tutorial was useful in helping you learn how to create a Voting Average system! As I said, this can be used to vote on Difficulty Modes, as well as any other game mode or in-game parameter.

-Eric Jensen [BYU_FOOTBALL]

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Overall I like this tutorial in terms of color usage and whatnot.

I do think the "select Difficulty Mode" trigger can be done in a cleaner way however. Just convert the average value (real) to an integer and the game will round the value for you.

I also want to raise a more core problem to the system idea itself.

Let's imagine the following scenario:

8 players

Expert votes: 4

Easy votes: 4

According to your formula:

Expert: 16 points

Easy: 4 points

With some quick maffs

(4 + 16) / 8 = 2.5 (rounds up to 3)​

So now half of the players get to play two levels above what they want. In my eyes this seems like A LOT. Especially if the gameplay difficulty increases exponentially rather than linearly.

I don't think I can reject a tutorial based on personal gameplay philosophy as it should be up to users if they think this fits their map.

However, I want to point this out to perhaps make a config trigger to make it so you can modify the difficulty weights easily. There are a few other things that should be configured too.

Very split on this so would be nice to have more thoughts from others.

edit: also a test map would be a nice addition.


 Last edited: Oct 18, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Graveyarded due inactivity

 You must log in or register to reply here.


### Similar threads

 Voting System


PureOwnage

Jul 28, 2008

Tutorial Archive

Replies2Views3K

Jul 30, 2008
Huurka

Locked

[Trigger]
 Game Mode Voting System (+Rep)


asdfADRiAN

Sep 11, 2010

Triggers & Scripts

Replies29Views3K

Sep 19, 2010
Bond009

 Making easy dialog buttons for e.g. a simultaneously vote for all players (just in GUI)


McPhisto2051

May 20, 2021

Tutorial Archive

Replies2Views2K

Jul 28, 2021
Uncle

Locked

 Dialog Vote System using Get Local Player


Xace

Sep 28, 2013

Triggers & Scripts

Replies3Views2K

Sep 29, 2013
PurgeandFire

Locked

 Team Voting System


Mechcash

Aug 23, 2021

World Editor Help Zone

Replies6Views2K

Aug 25, 2021
Uncle

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