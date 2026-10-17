---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-easy-dialog-buttons-for-e-g-a-simultaneously-vote-for-all-players-just-in-gui.332676/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making Easy Dialog Buttons for E G a Simultaneously Vote for All Players Just in GUI
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-easy-dialog-buttons-for-e-g-a-simultaneously-vote-for-all-players-just-in-gui.332676/>
#### McPhisto2051

#### McPhisto2051

 Level 7

JoinedMar 10, 2005Messages55

Resources: 11

#### Resources
Maps 9Tutorials 2

I read many threads from people complaining about dialog buttons and didn't use it proper myself before today. Even if it takes probably longer clicking the triggers in GUI this should be an easy example of how a dialog for multiple players in just GUI may work.

First of all you need some Variables. Ill explain them first.

Because I wanted just 1 decision running simultaneously but beeing able to repeat it, there's the boolean for the progress. So nobody can start another voting while the first is still running.

Then we have 3 button-arrays. Each has a length of 8 so that up to 8 players can vote (increase the size if you need more players). There are 3 because I want it to keep simple and have just 3 buttons for each player. We can vote for easy, normal or hard.

The dialog array keeps all those buttons and shows them to the players.

The integer counters are used to see how many votes were given to each decision.

The strings show us first the title of the board e.g. "which difficulty do you want?" and the buttons show us e.g. EASY, NORMAL and HARD.

The temporary integer will be used in registering who clicked a dialog button.

The timer and the timer window will show us how much time is left for voting.

Now that we declared our variables, well see that the voting is triggered via writing "dialog"

It doesnt matter which player triggers it.

The trigger checks if a vote is already in progress and runs only if there is no other vote.

Then it will be set to in progress to avoid another vote.

The text will be written into strings for easier change and not touching the buttons anymore.

The counters will be resetted.

A counter is started and gives every player 15 seconds to vote. It will be shown to every player.

We loop through 8 players and just create those dialog buttons for those players who are actually playing the game.

We create the corresponding dialog window and buttons for those players and show the completed dialog.

Now we register the clicked buttons:

The temporary int is set to the number of the player which clicked it and then we look which button was clicked.

We increase the amount of votes according to the button.

Now we wait for the timer to end.

We remove the timer window and hide the dialog for all players, in case somebody didn't vote (perhaps he was afk or sth.)

Then we show which vote got how many voters.

Because there are just 3 vote options and I prefer normal I made the voting result a little awkward.

In this case Im looking if normal has more votes than easy or hard.

If there as many easy as hard votes we get also normal.

In the end we check if there are more easy than hard votes and get either easy or hard.

We show the result via text but you're free to set some variables as you like.

Edited, because I forgot to attach the map.

#### Attachments

dialog-test.w3x

 17 KB

 · Views: 95




 Last edited: May 20, 2021

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

1. Consider posting the triggers with trigger tags instead of images

2. The tutorial lacks structure, titles, colors

3. Instead of looping through 1-8 you should probably use a player group. I think its more efficient if done properly.

4. Instead of having comments in the code I think it would be better to explain in the tutorial itself.

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

I agree with Chaosy. Also for #3, a Player Group makes more sense since there are no assumptions made with it. For example, if Player 1 was a user, Player 2 was a computer, and Player 3 was a user, now the For Loop has to skip 2 but start at 1. Obviously this is not a big deal and can be made to work with some extra Conditions but a Player Group is really meant for situations like these.

Also, I notice this mistake a lot. I think most of the time it's just a bad habit:

And - All (Conditions) are true
^ This is unnecessary in your Conditions. By default all Conditions need to be true.

You would use And in cases like this:

Or - Multiple (Conditions) are True

Condition A = true

Condition B = true <AND> Condition C = true
So the Actions run if A is true or if both B+C are true.


 Last edited: Jul 28, 2021

 You must log in or register to reply here.


### Similar threads

 Voting System


PureOwnage

Jul 28, 2008

Tutorial Archive

Replies2Views3K

Jul 30, 2008
Huurka

 Creating a Mode Voting system that averages Player Votes


Eric Jensen

Jan 16, 2019

Tutorial Archive

Replies2Views2K

Oct 23, 2020
Chaosy

 How to make a Working Dialog


Crazed_seal2

Nov 29, 2009

Tutorial Archive

Replies3Views2K

Jan 18, 2010
Kwah

O

 Basics of a Dialog


Orc_Tamer

Jul 1, 2006

Tutorial Archive

Replies7Views4K

Mar 7, 2007
Wolverabid

 Dialogs


Ralle

Jan 3, 2006

Tutorial Archive

Replies2Views3K

Mar 28, 2006
Ralle

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