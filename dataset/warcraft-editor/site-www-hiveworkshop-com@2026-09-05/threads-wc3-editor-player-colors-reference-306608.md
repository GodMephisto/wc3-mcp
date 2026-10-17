---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/wc3-editor-player-colors-reference.306608/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# WC3 Editor Player Colors Reference
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/wc3-editor-player-colors-reference.306608/>
#### Raen7

#### Raen7

 Level 4

JoinedMay 14, 2018Messages27

Resources: 2

#### Resources
Tutorials 2

Per suggestion from another member, I have moved this reference sheet here as a "Miscellaneous tutorial/guide/reference". (As opposed to the 'Tools' section.)

WC3 World Editor Player Colors

This is a Google Sheet that shows you all 24 player colors (since they expanded the # of players in the latest patch), along with their decimal, percentage, and hex RGB values. It concludes with full color-tags that you can embed in string fields/values, and shows visual samples of each.

Hope this helps someone, as it helped me in building it. Comments welcome!

Assumptions:

User is familiar with Warcraft 3 World Editor.

User has a basic understanding of how colors are stored and referenced in the editor (i.e. the RGB system).
A bit more information:

Trigger actions that set object colors (such as vertex coloring, lightning effects, etc.) usually use percent values, while things that deal with text strings, such as chat messages, text values in the object-editor (like tooltips) require the color-tag.

A string color-tag is of the form "|c ff rr gg bb", but with no spaces, and optionally ending with "|r". '|c' or '|C' begins the color tag. 'ff' is for transparency, and a string is always 0% transparent. 'rr' is your Red hex value, such as 'ff' for 100% red, or '00' for no red. Similarly for 'gg' as Green, and 'bb' as Blue.
For example, to make a yellow string that says "Bananas", you would enter "|cfffffc00Bananas|r" in the string value field of the trigger or tooltip or whatever you're editing.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Awesome! Thanks for compiling all this info. Would you mind converting it to a table in BBCode? External sites often go down, or get eventually deleted. So it'd be best to keep it as static text on the hive.

I think you can also omit a few things, imo. I would condense it down to "Player" | "Color" | "(R, G, B)" | "Hex string" | "Color Sample".

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Moving to the graveyard in the interim. PM when you'd like to make the changes to get this approved!

#### Acutesharpness

#### Acutesharpness

 Level 15

JoinedAug 14, 2007Messages936

Resources: 9

#### Resources
Maps 8Spells 1

Credit has been added to my project. (Legendary Resistance)

 You must log in or register to reply here.


### Similar threads

 24+1 Player Color Codes


OnGe

Feb 16, 2020

Tutorial Archive

Replies3Views16K

Nov 2, 2020
Chaosy

[vJASS]
 [Snippet] Color


Kazeon

Apr 14, 2015

Graveyard

Replies10Views3K

Aug 21, 2015
Bribe

 [Snippet] String Colors


Nestharus

Jun 28, 2010

Graveyard

Replies47Views11K

Mar 25, 2013
Almia

Locked

[JASS]
 Convert Player Color to String


Hakeem

Jun 12, 2007

Triggers & Scripts

Replies9Views4K

Jun 13, 2007
PurgeandFire

Locked

 A color snippet!


Kazeon

Apr 13, 2015

Advanced Scripting

Replies0Views1K

Apr 13, 2015
Kazeon

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