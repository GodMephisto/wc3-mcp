---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/24-1-player-color-codes.322502/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# 24 1 Player Color Codes
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/24-1-player-color-codes.322502/>
#### OnGe

#### OnGe

 Level 5

JoinedFeb 3, 2020Messages25

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

You may find color codes for 12 players all over the internet. Since we have 24 players now, it is kind of outdated. Also note that predefined colors you may find in editor does not match player colors.

Here are colors for all 24 players + neutral, taken from color displayed on starting location. It is most close to ingame color as possible. So, this is current Warcraft III: Reforged player color palette.

Player 1: Red |cffff0303

Player 2: Blue |cff0042ff

Player 3: Teal |cff1be7ba

Player 4: Purple |cff550081

Player 5: Yellow |cfffefc00

Player 6: Orange |cfffe890d

Player 7: Green|cff21bf00

Player 8: Pink |cffe45caf

Player 9: Gray|cff939596

Player 10: Light Blue|cff7ebff1

Player 11: Dark Green|cff106247

Player 12: Brown |cff4f2b05

Player 13: Maroon|cff9c0000

Player 14: Navy|cff0000c3

Player 15: Turquoise|cff00ebff

Player 16: Violet |cffbd00ff

Player 17: Wheat|cffecce87

Player 18: Peach |cfff7a58b

Player 19: Mint |cffbfff81

Player 20: Lavender |cffdbb8eb

Player 21: Coal |cff4f5055

Player 22: Snow|cffecf0ff

Player 23: Emerald|cff00781e

Player 24: Peanut|cffa56f34

Player Neutral: Black |cff2e2d2e

(some colors are hard to see because of background - watch out)

Need to display colored name of player? You may easily do that with player color code index. In attached map you find trigger containing color index and simple loop that display each player color. You may easily modify message, use Number of Triggering player (or Owner of Triggering unit, etc.) instead of Integer A and use it for whatever you need.

#### Attachments

24colorsa.w3m

 12.7 KB

 · Views: 381




 Last edited: Feb 16, 2020

#### OnGe

#### OnGe

 Level 5

JoinedFeb 3, 2020Messages25

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

Few tips about colors:

Obviously, dark colors are hard to see on dark background and light colors on light background. Think about it when tweaking with colors at UI and messages.

Important message should be colored in way to be easily readable. White is best in most cases, very bright colors are fine too. But if you put white and light blue on text messages with Ice Crown tileset, it will be hard to read. If you change name of hero to player color, you make brown, purple, coal, navy (and neutral) players virtually unreadable.

Colors look different bit depending on their surroundings.

High contrast of text and background is important for good readability for anyone, but even more important for people with less than perfect vision. That means most of the worlds population.

There is few percent of people with color vision deficiency (colorblind) who see colors differently than rest of the world. For example red and green may seem fairly similar. This is the reason why common competing team colors are Red and Blue and not Red and Green.

Player colors are not optimal. Dont blame Blizzard for that - there is not enough optimal colors for 24 players.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I am inclined to graveyard this since this tutorial exists already: Warcraft III Color Tags And Linebreaks

My thinking is I just go and throw in the last 12 colors to make it up to date.

You are welcome to try and outdo the mentioned tutorial by adding something beyond the list, I'll get back to this eventually and see if I have changed my mind.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Graveyarded.

As I mentioned in the other post, I added the last 12 colors to the existing tutorial to cover up the main flaw.

 You must log in or register to reply here.


### Similar threads

H

 Warcraft III Color Tags And Linebreaks


h3ih02

Oct 26, 2004

Misc/General Mapping Tutorials

Replies22Views211K

Jun 26, 2023
Sapprine

[Crash]
 Players desync when more than 3 join


Draxient

Apr 20, 2025

World Editor Help Zone

Replies10Views2K

Apr 21, 2025
Draxient

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