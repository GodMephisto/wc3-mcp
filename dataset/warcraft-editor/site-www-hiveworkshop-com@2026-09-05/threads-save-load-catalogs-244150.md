---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/save-load-catalogs.244150/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Save Load Catalogs
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/save-load-catalogs.244150/>
#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

https://www.hiveworkshop.com/forums/spells-569/save-load-snippets-v2-1-0-5-a-202714/?prev=mmr=6

Save/Load Catalogs for the GUI User

This tutorial isn't the greatest, but it accomplishes its goal. I don't exactly have a lot of time for this stuff and there seems to be a great need for this tutorial.

A good save/load system can't be made in GUI. The reason is because of what are called catalogs. Catalogs turn big numbers, like hero ids, into very small numbers. They have many constraints on them that are map-specific. Furthermore, the catalogs that are required of a map are also unique to that map.

Catalogs are only implemented in vJASS and can't be implemented in GUI. This is because they use what are called modules.

In order to create a good save/load system for your map, you must use at least a little bit of vJASS so that you can do the catalogs.

First, vJASS is just text. It is a language. If you already know programming concepts from GUI, then you are already halfway there. The only thing that you need to study is what to write and in what order to write it.

There is structure to programming and you have to follow this structure exactly. JASS is translated to vJASS just like GUI is. JASS is translated to something else. Structure is everything.

The structure that will be covered is the struct

 JASS:

 struct NAME
 endstruct

A struct is just something that is used to organize code. The above srtuct actually generates quite a bit of garbage that we don't need. To drop that garbage, we put an "extends array" at the end of it.

 JASS:

 struct NAME extends array
 endstruct

So the big thing to look at is first

 JASS:

 struct
 endstruct

"struct" shows where the thing begins. "endstruct" shows where it ends. After "struct," you need a name for it.

 JASS:

 struct MyStruct extends array
 endstruct

As this tutorial is going over catalogs, let's name it HeroCatalog

 JASS:

 struct HeroCatalog extends array
 endstruct

After this, you need to implement what is called a module. A module is just something that copies and pastes code into a struct. This is the reason why it can't be done in GUI. GUI has nothing like this. Why not just make the code for each individual thing then? Good luck.

 JASS:

 struct HeroCatalog extends array
 implement Catalog
 endstruct

First, the implement keyword implements the module. After it, you put in the module name. Catalog happens to be the name of a module from the Catalog resource, which you can find under core systems in save/load with snippets.

What does a basic hero catalog look like?

Before we delve into that, we need to cover what are called methods very quickly.

A method is just like a list of actions in a trigger. However, unlike actions, you can give it values and it can return a value. It also doesn't have events or anything like that.

The method block is as follows

 JASS:

 private static method NAME takes nothing returns nothing
 endmethod

Because the aim of this tutorial is to make a save/load system for a map as quickly as possible, the details behind a method won't really be elaborated on.

The following should be clear

 JASS:

 method NAME
 endmethod

private - refers to whether other code can see it or not. Anything outside of the struct can't see this. It can also be public or just nothing

 JASS:

 private method NAME
 public method NAME
 method NAME //also public

static - refers to whether the method takes a value called "this" or not. Static means that it doesn't.

 JASS:

 private static method NAME
 public static method NAME
 static method NAME

takes - what the method takes. This is a comma separated list of variables. If it doesn't take anything, the nothing keyword is used.

returns - the type of variable that the method returns. If it doesn't return anything, nothing is used. When I say type, think about variable types in the variable editor.

 JASS:

 private static method NAME takes nothing returns nothing

The above is a method called NAME that takes no values and returns nothing

To call it, use the "call" keyword followed by the method name and then a "()".

 JASS:

 call NAME()

The "()" can actually hold a comma separated list of values. This list of values must be in the same order as the list of values that the method takes.

If it doesn't take anything, then the "()" is just empty. If it took an Integer (think back to GUI), it would take a whole number value, like 5.

 JASS:

 call NAME(5) //if it took an integer

If it took two integers

 JASS:

 call NAME(-1, -3) //two random integers, notice the comma

Now to implement a Catalog.

The Catalog module copies a few methods and variables into the struct

This is straight from the Catalog documentation. Keep in mind that structs can be used as variable types.

 JASS:

/*
* readonly static Catalog catalog
* - Retrieves the instance id of the catalog. Used for adding it to other
* - catalogs.
* readonly static integer count
* - Retrieves the total amount of values inside of the catalog. Includes totals
* - of added catalogs.
* readonly integer raw
* - Gets the raw value given a catalog value. Raw values are the
* - original values that were added.
*
* - Ex: integer raw = Catalog[1].raw
* readonly integer id
* - Gets the catalog id given a raw value
*
* - Ex: integer id = Catalog['hpea'].id
*
* static method add takes integer value returns nothing
* - Adds a value to the catalog. Values already inside of the catalog
* - or inside of any catalog that it contains are not added.
* static method addCatalog takes Catalog catalog returns nothing
* - Adds a catalog and all of its inner catalogs to the catalog.
* - Catalogs already inside of the catalog are not added.
*/

So now to create a basic Catalog.

 JASS:

 struct HeroCatalog extends array
 implement Catalog

 private static method onInit takes nothing returns nothing
 endmethod
 endstruct

Methods named onInit are special. They run when the map loads.

This is the specific method of interest that will be used

 JASS:

 static method add takes integer value returns nothing

The value it takes is a unit type id, which can be found in the object editor. Type Ctrl + D to view the objects as raw values. Type it again to revert back.

The four letter thing to the left of the (name) is the type id. This is what Catalogs take.

 JASS:

 struct HeroCatalog extends array
 implement Catalog

 private static method onInit takes nothing returns nothing
 call add('Hpal') //paladin
 call add('Hamg') //archmage
 call add('Hmkg') //mountain king
 endmethod
 endstruct

Why is the implement Catalog at the top of the struct? You should never call methods that are below you, always call ones that are above.

Note that implement Catalog is above onInit.

The calls to add put those type ids into the Catalog. From here, to use them

 JASS:

 set udg_MyGlobal = HeroCatalog['Hpal'].id

The above will convert 'Hpal' into a small number, in this case 1.

First, [ ] is the syntax for arrays. However, HeroCatalog is not an array, it's a struct. What magic is this? [ ] is an operator just like +, -, *, and /. vJASS allows users to create methods for those operators. Pretty cool? The use of [ ] is actually a method call that takes a unit type id. It returns a value, like 1. This value is the struct type (not an integer). We'll get to why in a moment.

Just to show how one would create [] as a method

 JASS:

 static method operator [] takes integer value returns integer
 return 0
 endmethod

So the operator thing is used to show that the method is an operator and then the name is passed in, in this case [].

The "." in

 JASS:

 set udg_MyGlobal = HeroCatalog['Hpal'].id

Is an accessor. It accesses a member of the struct. Remember how earlier I said that non-static things inside of a struct take "this?" Notice HeroCatalog['Hpal'] returns a value. It then does value.id. Value is passed in as the "this." The id thing also happens to be a method. vJASS allows users to create methods that look like variables too...

So, when you are saving your values, you want to do

 JASS:

 HeroCatalog['Hpal'].id

When you are loading them, you want to do

 JASS:

 HeroCatalog[1].raw

Because 'Hpal' was the first value added to our catalog, it's raw value happens to be 1. HeroCatalog[1].raw would return 'Hpal'.

If you were to do this in GUI custom script, you'd have your GUI variable to store the unit type id

 JASS:

 set udg_Hero = HeroCatalog[1].raw

Now that we know the basics of catalogs, let's move on. Under Catalog Tools, there are a variety of catalog filters.

LevelGroupSlotVersion Filter

This is primarily used for items.

Level: minimum level to us the item

Group: required hero class. Why group? Evolving heroes. Let's say that a rogue can evolve to a thief or an assassin. Thieves and assassins should be able to use rogue gear.

Slot: required equipment slot. For example, a sword can't fit into a chest slot.

Version: the current version of your *saver.* As you add more stuff to your game, you increase the version. Will hit on this more later.

LevelGroupVersion Filter

Same as above minus the slot stuff.

LevelVersionFilter

VersionFilter

Within Catalog tools are demos of each type. Just copy the demo, rename it to what you want, and then fill it up with your stuff. Be sure to detele the extra versions, you only need version 1 at the beginning.

When do you increase the version? Under this scenario.

You release your map. It's on version 1.

You add new items to it. Increase the version number. Then release it.

Only increase the version number *up to* one time between each release. If you didn't add anything, don't increase the version number.

Why are old version kept? For backwards compatible codes.

The filters have a more complicated API than the standard Catalog does.

This is the method from the really big filter

 JASS:

 method get takes integer ver, integer groupId, integer slot, integer minLevel, integer maxLevel returns Catalog

You don't just do

 JASS:

 Catalog['Hpal'].id

You need to do

 JASS:

 HeroCatalog.get(1)['Hpal].id

The 1 would be the version of the catalog you want.

If we were to use the super giant filter

 JASS:

 ItemCatalog.get(1, 1, 1, 1, 60)['item].id

Yes, I just filled it up with 1's for the hell of it and used 'item' for the item type id. ItemCatalog was also chosen on a whim for a name.

These filters have everything that Catalog has. They implement the Catalog module internally.

Don't continue to call .get over and over again, only call it once for each filter you need.

 JASS:

 set udg_Catalog = ItemCatalog.get(1, 1, 1, 1, 60)

Then

 JASS:

 Catalog(udg_Catalog)['item'].id

I repeat, do not call "get" over and over again.

Always use the bare minimum of the VersionFilter, always. If you do not use this, you will not be able to add anything to your catalog after your first release. You will be stuck.

The only thing that requires custom script are catalogs. The rest can be done with the GUI.


 Last edited: Nov 11, 2013

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

 Save/Load Catalogs for the GUI User


Click to expand...

I'm not sure how many, if any GUI users will find this useful or even understand it.

If it was for GUI users you wouldn't be talking about structs how to code in vJass lol.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Please read the very beginning :\

 A good save/load system can't be made in GUI. The reason is because of what are called catalogs. Catalogs turn big numbers, like hero ids, into very small numbers. They have many constraints on them that are map-specific. Furthermore, the catalogs that are required of a map are also unique to that map.

Catalogs are only implemented in vJASS and can't be implemented in GUI. This is because they use what are called modules.

In order to create a good save/load system for your map, you must use at least a little bit of vJASS so that you can do the catalogs.


Click to expand...

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

Nestharus said:

 Please read the very beginning :\


Click to expand...


That just proves my point.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Right now this tutorial isn't the best. It needs to be refined a bit. I put it up here because it's a necessary tutorial and I doubt anyone will write it =). I'll slowly improve it over time (or try to at least).

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Since the content is all there and fine, it is approvable in its current state. Feel free to refine it as you see fit.

Approved.

 You must log in or register to reply here.


### Similar threads

Locked

[General]
 A New Look At Advanced Save/Load Catalogs : )


Nestharus

Nov 8, 2011

World Editor Help Zone

Replies0Views922

Nov 8, 2011
Nestharus

Locked

[vJASS]
 Making a GUI-vJASS Save system, need feedback


Arhowk

Mar 17, 2012

Triggers & Scripts

Replies6Views1K

Mar 18, 2012
Nestharus

Locked

[General]
 Save/Load Algorithms (what you guys want?)


Nestharus

Aug 27, 2011

World Editor Help Zone

Replies10Views2K

Aug 28, 2011
defskull

 Catalog Functions


mrzwach

Sep 12, 2010

StarCraft II Tutorials

Replies0Views8K

Sep 12, 2010
mrzwach

Locked

 Catalog - accessing arrays


BexoDust

May 21, 2010

StarCraft II Modding

Replies4Views2K

May 28, 2010
BexoDust

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