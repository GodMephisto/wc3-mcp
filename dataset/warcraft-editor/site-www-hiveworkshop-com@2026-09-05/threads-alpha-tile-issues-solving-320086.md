---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/alpha-tile-issues-solving.320086/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Alpha Tile Issues Solving
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/alpha-tile-issues-solving.320086/>
#### V

#### V

 Level 7

JoinedJan 13, 2019Messages284

Resources: 5

#### Resources
Spells and Systems 4Tutorials 1

Hello, if you want to know what is alpha tile go to here:

Alpha Tile for Dummies

This tutorial is a solution to the problem of alpha tile use:

It looses transparency in fog of war and black mask

It looses transparency if it is the bottom tile as mentioned in the overlapping part above.

It is white or black on the minimap
1) The transparency issue can be solved with this configuration:

Create a map initialization triggers with

Player Group - Pick every player in (All players) and do (Actions)

Loop - Actions

Visibility - Create an initially Enabled visibility modifier for (Picked player) emitting Black mask across (Playable map area)

2)The overlapping can solve with this:

When adding tiles place the Alpha tile in second position

3)If you want solve the minimap issue you can do this:

Select a dummy tile for coloring the minimap (snow in this time)

Travel across the map changing every dummy tile with our alpha tile

 JASS:

function Trig_Swap_Func007Func001Func003C takes nothing returns boolean
 if ( not ( udg_TEMP_TILE_TYPE == udg_DUMMY_TILE_TYPE ) ) then
 return false
 endif
 return true
endfunction

function Trig_Swap_Actions takes nothing returns nothing
 call DisplayTextToForce( GetPlayersAll(), "TRIGSTR_018" )
 set udg_min_x = GetRectMinX(GetEntireMapRect()) / 128
 set udg_min_y = GetRectMinY(GetEntireMapRect()) / 128
 set udg_max_x = GetRectMaxX(GetEntireMapRect()) / 128
 set udg_max_y = GetRectMaxY(GetEntireMapRect()) / 128
 set bj_forLoopAIndex = R2I(udg_min_x)
 set bj_forLoopAIndexEnd = R2I(udg_max_x)
 loop
 exitwhen bj_forLoopAIndex > bj_forLoopAIndexEnd
 set bj_forLoopBIndex = R2I(udg_min_y)
 set bj_forLoopBIndexEnd = R2I(udg_max_y)
 loop
 exitwhen bj_forLoopBIndex > bj_forLoopBIndexEnd
 set udg_TEMP_TILE_POS = Location(I2R(( GetForLoopIndexA() * 128 )), I2R(( GetForLoopIndexB() * 128 )))
 set udg_TEMP_TILE_TYPE = GetTerrainType(GetLocationX(udg_TEMP_TILE_POS), GetLocationY(udg_TEMP_TILE_POS))
 if ( Trig_Swap_Func007Func001Func003C() ) then
 call SetTerrainTypeBJ(udg_TEMP_TILE_POS, udg_ALPHA_TILE_TYPE, -1, 1, 1)
 else
 endif
 set bj_forLoopBIndex = bj_forLoopBIndex + 1
 endloop
 set bj_forLoopAIndex = bj_forLoopAIndex + 1
 endloop
endfunction

//===========================================================================
function InitTrig_Swap takes nothing returns nothing
 set gg_trg_Swap = CreateTrigger( )
 call TriggerRegisterPlayerChatEvent( gg_trg_Swap, Player(0), "swap", true )
 call TriggerAddAction( gg_trg_Swap, function Trig_Swap_Actions )
endfunction

3.5) If you want find the code of a tile:

place that tile in the center of the map (where the tree is) and use chat command "findcode" later change this value with dummy_tile_type variable with the obtained value.

NOTE:

1. "Masked Areas Are Partially Visible" can be checked or unchecked (Map Properties)

2. Alpha Tile Felwood Poison

3. Dummy Tile Snow

RESULTS:

See the attachment map to make test.

Commands:

fogon

fogoff

blackon

blackoff

swap

findcode

#### Attachments

Alpha_tile.w3x

 28.1 KB

 · Views: 151




 Last edited: Jan 7, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I believe the alpha tile does not work in reforged which should probably be mentioned.

The structure is fine, no big problems there.

I think you should clean up the jass code however as it looks like a GUI trigger converted to JASS without edits.

Either showcase GUI or write cleaner JASS, function names stand out along with standard GUI loop.

Also, it might be good to explain what the code does. Otherwise it might be better to simply submit it as a system that users can use.

#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


Chaosy said:

 I believe the alpha tile does not work in reforged which should probably be mentioned.


Click to expand...

Actually, you can set a "ruined ground"'s model file to none.mdl and then you will have alpha tiles in Reforged. Placement is a bit clunky, but it works.

 You must log in or register to reply here.


### Similar threads

Locked

[JASS]
 I fail again at gui - jass conversion


Squiggy

Aug 22, 2008

Triggers & Scripts

Replies5Views1K

Aug 22, 2008
Equal

 Alpha Tile for Dummies


bhusta

Dec 23, 2012

Misc/General Mapping Tutorials

Replies13Views25K

Nov 8, 2021
Mayday

Locked

[JASS]
 2050 errors with this trigger


Squiggy

Aug 22, 2008

Triggers & Scripts

Replies5Views1K

Aug 22, 2008
PurplePoot

Locked

 Complex trigger, need leak check for pure JASS


lelyanra

Mar 26, 2010

World Editor Help Zone

Replies18Views2K

Mar 29, 2010
lelyanra

Locked

[General]
 GUI Code Need Fix


Trakya

Dec 17, 2014

World Editor Help Zone

Replies12Views2K

Dec 18, 2014
chobibo

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