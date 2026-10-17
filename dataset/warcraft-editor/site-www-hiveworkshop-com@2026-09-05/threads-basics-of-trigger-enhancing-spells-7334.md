---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/basics-of-trigger-enhancing-spells.7334/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Basics of Trigger Enhancing Spells
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/basics-of-trigger-enhancing-spells.7334/>
#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Bookmark


I.Introduction


II. Tools and knowledge needed


III. Working with the Object Editor


IV. Triggers. What are they and how do they work?


V. Variables


VI. Arrays


VII. Basic Actions


VIII. Basic Events and Conditions


IX. Event – Response Variables in Detail


X. Manipulating Triggers


XI. Auxiliary Functions


XII. Single-Target Spells


XIII. Area Of Effect (AOE) Spells


XIV. Passive Abilities


XV. Buff Spells


XVI. Channeling Spells


XVII. Selfcast Spells


XVIII. Efficiency


XIX. Final Note


****************************


I. Introduction


This tutorial is more an introduction into trigger enchancing spells, than a complete manual. It will give you a general overview upon making your own trigger enchanced spells. You can achieve better results through practice and experimenting.


II. Tools and knowledge needed


First of all you need Warcraft 3 World Editor, preferably TFT since I don’t remember if ROC had Ability Editor/Buff Editor or if it has all the functions we will need in spellmaking. And as for knowledge, you should be able to work with the World Editor and the Object Editor. You don’t have to know anything about Triggers. I shall introduce you if you don’t know how to work with them.


III. Working with the Object Editor


Before we enter triggers themselves, I would like to point some things out, extremely necessary in order to be able to enchance your spells.


First of all, you have to know that an unit cannot have two abilities based on the same root ability. What does this mean? If for example a unit has two abilities, both copies of the Flame Strike ability, the unit might not cast the spell it has been ordered to. It may cast the other Flame Strike ability, if it is not under cooldown. Be careful! There are enough abilities to copy.


Secondly, you must learn the concepts of Dummy Units. Dummy units are special units (lol, not really) meant to help you enchance the spell with better effects. For now, you must know how to create your own dummy unit in the object editor. After that, you will learn how to use them as well.


So go to the unit editor and copy-paste the footman unit. Rename it as dummy, so that you will be able to recognize it later. And now, change the following fields accordingly:

Abilities – Normal – It should include only the Locust unit ability. This will make the dummy unselectable, invulnerable and will remove its collision, allowing any unit to pass through it. By this you assure the unit cannot be detected in any way.

Art – Model File – Switch to custom and type none.mdl. This will make the unit invisible (not as invisibility, but unseen).

Art – Shadow Image – Switch the value in preset to none.

Art – Special – Delete everything there.

Combat – Attacks enabled – Switch to none.

Combat – Death Type – Switch to Can’t Raise, Does not Decay

Movement – Speed Base – Set its value to 0.

Movement – Type – Switch to Fly.

Sound – Unit Sound Set – Change its value to NONE.

Stats – Can Flee – False.

Stats – Food Cost – Set it to 0.

Stats – Hide Minimap Display – True

Stats – Sight Radius (Day) – 0

Stats – Sight Radius (Night) – 0

Techtree – Upgrades Used – Delete all.


After you have given the unit all these values you can place one on the map and notice that ingame you won’t see it. In World Editor it will appear as a box with green/black squares. That’s because the model you assigned to the unit doesn’t exist. It will disappear ingame.


Another concept we need to study is Dummy abilities. You will give these abilities to the hero and they will practically have no effect, until we enchance them with triggers. What they will have is the mana cost, cooldown, description, eventually requirements, buffs and icons (and obviously name). Practically they should act like a normal hero ability, if you learn for example the ability should be there, with its mana cost and cooldown (when the hero casts it), the description when you put the mouse on the icon and of course it should be AoE if your enchanced spells is supposed to be AoE, or single target if your enchanced spell is supposed to be single target and so on. And this leads us to a second problem: on which spell to base your dummy ability?


Let’s say that you want to do an Area of Effect Sleep (affects all the units in a selectable area). Some of you who have absolutely no experience in spellmaking might say that you must base your dummy spell off Sleep. Wrong! The dummy spell as I mentioned before must mimic the final result spell with no effect (so in this case targets the units in an AoE but doesn’t put them to sleep when cast). However, sleep is a single target spell, not an AoE (like Silence, Blizzard, Cluster Rockets, Flame Strike and other Area of effect spells). You should base your spell off one of the example spells I gave above.

Note


The duration of the dummy spells should be set to 0.01 if it is initially greater than 0. If for these spells you set the duration to 0, it will last forever (or for some until dispelled). And this might lead to other problems. If you want to make a dummy spell which targets a single unit and only damages it (pure example), if you base it off storm bolt, it will do a 0.01 stun, breaking channeling spells. Same thing will happen with silence if you don’t remove the spell disabling effect. Be very careful!


IV. Triggers. What are they and how do they work?


Now that we have clarified the non-trigger side of spellmaking, we are going to study a long chapter: triggers. Since some of you might have no idea about what triggers are, I’m going to make a brief (yeah right) introduction.

Triggers consist of three parts: Events, Conditions and Actions. You can have as many of these as you want, but of course it is not quantity that is important. Throwing events, conditions and actions here and there randomly won’t do it.

Events are that something that fires the trigger. They are like a spark which can burst a house into flames. You can have more just one event but only one is needed for the trigger to fire. After all, a spark is enough to start a fire, isn’t it? Events are something general, unlike conditions.

Conditions are much more accurate than events, referring to things more specific. You can have as many conditions as you want as well but in this case, all of them must be fulfilled if you want the actions to take place. After all, you may have as many sparks as you want because the fire will not start if it is raining outside.

Actions are the most interesting yet extended part. If the trigger has been fired and the conditions are true, then all the actions will take place in the order in which they are placed into the trigger, one after another. Each action will do something ingame, noticeable or non-noticeable.


So a general definition of triggers would be like this: “Triggers are a series of actions taking place one after another when a certain event takes place in certain conditions”.


As you might’ve already noticed, the trigger editor has dozens of actions, events and conditions. Don’t worry, in time you will learn most of them. But for now, you will need to learn the most important. I will try to mention the essential, the most important. You’ll need to explore the rest.


V. Variables


Variables are used to store information into the memory, if you may want to use them during the actions of the same or even different triggers. There are three categories of variables: Global, Local and Event-Response.

Global Variables are extremely used when making trigger enchanced spells. To declare these variables, you need to go to the variable editor and add a variable. The variable editor can be found in the trigger editor where there is an X icon. The sole advantage of Global Variables is that they can be used by anyone who has little knowledge in triggering. However, they have a big disadvantage as well: they are unique. This means that at a precise moment, the variable can store a single value. If the trigger is executed multiple times, only the value in the last execution will be stored into the variable.


In spellmaking this is a pretty big problem. Consider two units casting the same spell at the same time. If you use global variables, only the second unit casting the spell (because casting EXACTLY at the same time is pretty impossible) will succeed. The first caster’s spell will be bugged, the severity depending on how often you used global variables. As an example, imagine two sorceresses casting slow at the same time, and only the second one’s taking effect. Wouldn’t it be annoying?


The spells that allow two or more units to cast them at the same time without causing any bugs because of the global variables problem are called Multiinstanceable (or MUI). To obtain such spells you will have to use Event-Response Variables (which have their limits) or local variables, which unfortunately for now will be inaccessible to you.

Local Variables unfortunately require JASS, which is already a programming language based on which, triggers were made using a Graphic User Interface (GUI), to make it accessible to everyone. But through this, they had to limit some of the language’s possibilities. I don’t recommend learning JASS until you have considerable experience in triggers. I really found it difficult when I started to learn, and took me more than a month to learn it.


But back to the description of the local variables, they store a separate value for each execution of the actions into the memory. Of course this could cause Memory Leaks (variables are stored into the memory from where they are not disposed) if you do not properly remove them. We will discuss this later, once we finish with the basics.

Event-Response Variables are a little bit different and inflexible. Unlike Global or Local variables, these variables cannot receive a certain value. Their value varies, sole depending on the event of the trigger. Moreover, they have a life limit into the memory. After the shortest period of the time, most of them are disposed from the memory. Only a few of them remain there until the last action has been executed as well. To solve this problem, you will have to use local or global variables.

Note


Global/Local variables must have an unique name. In the case of the Globals the Variable Editor will prevent you from giving them the same name but in the case of the Locals, the situation will change. Your own concern by now should be Globals though.


Another classification of the variables can be done by their type. I have enumerated the most important types below:


- Integer – Stores a numeric integer value (ex: 34)


- Real – Stores a numeric real value (ex: 500.32)


- Point – Stores a point on the map with specific coordinates (ex: Target Point of Ability Being Cast)


- String – Stores Multiple Character (ex: “abcd”)


- Unit – Stores an Unit (ex: Casting Unit)


- Boolean – Can Store a True or False value.


- Special Effect – Stores a Special Effect (ex: Last Created Special Effect)


- Unit Group – Stores a Group of Unit (ex: Units In (Playable Map Area))


- Player – Stores a player. There are a total of 16 players. (ex: Owner of Unit)


These are the most important variable types. Even if you don’t understand all the examples, you will soon comprehend all of them.


VI. Arrays


In the variable editor, each variable has the option of becoming an array. This option allows the variables to retain more than one value (but this doesn’t make global variables local). Each value will receive a numeric index. So if you want to get a certain value from the array, you will need the index. So arrays don’t really allow to make your spells MUI. Their advantage is that in some cases, in which you would have to create 5 global variables of the same type, you can just use a single variable as an array. However, each value stored into the array will consume as much memory as it would take to store it in a separate global.

Note


All variables in an array are of the same type. In an unit array you can’t enter integers.


VII. Basic Actions


Its time we begin to analyze actions. There are tons of them and I don’t have enough time and patience to explain each of them. However, I will take a couple of them, very important to comprehend if you want to enchance your spells.


An extremely basic action is Wait. This action has an obvious effect: When reached, it will hold the trigger for a period of time, before going to the following actions. You can set the number of seconds during which, the trigger is hold. Keep in mind that even though it apparently seems possible, you can’t make the wait have a less value than 0.10 seconds. Also you will notice later that most of the Event – Response Variables are disposed from the memory after the smallest wait (0.10 sec). There are exceptions such as (Triggering Unit). The only solution to keep these variables into the memory is to assign them to a global variable.


A similar action is Wait for Condition. Unlike the plain Wait, this action holds the trigger until a condition is fulfilled. The checking every x section is interpreted as the frequency at which the condition is checked if it has been fulfilled, to unhold the trigger. Until it is checked, the trigger will wait, even if the condition is fulfilled.

For Loops are also extremely important actions and for those of you who have no experience in computer programming, they might sound difficult to comprehend. Loops repeat a series of actions for a limited number of times (VERY IMPORTANT, it must be limited). You will have a start value and an ending value. The first value is given to either a constant variable (Integer A, Integer B) or a global integer variable. With each loop, the variable is then incremented (its value increases by 1) until it is greater than the last value. At that point, the loop stops. It is very useful to use the variable into formulas inside the loop, thing that we will discuss later as well.

Note


Don’t use Integer A/B constant variables for more than one spell. If you do, you might get the same problem you would get with a spell which is cast by more than one unit at the same time, and you have used global variables, but this time for the two spells.

If/Then/Else action is yet another basic programming structure (in this case action) you need to learn. The action checks a series of conditions. If all are fulfilled, then a series of actions takes place (IF actions). However, if atleast one condition isn’t fulfilled, other series of actions take place (ELSE actions).


Unit actions are also very frequently in enchancing spells because usually units are targets of spells… I think every spell uses a unit action, one way or another.

Unit – Add Expiration Timer removes a unit after a couple of seconds. Like summoned units, it will receive the blue bar, which shows when it will disappear. Use this on dummy units so that you don’t have to remove them manually.

Unit – Remove Unit simply removes a unit from the game. It doesn’t kill it, just removes it forever, with no way back.

Unit – Hide Unit hides a unit. Hidden units cannot be seen, selected or targeted but they cannot issue any order either. For hidden heroes, their icon still appears in the left corner. You will notice that in the “Storm Earth and Fire” spell, the casting hero is hidden, not just miraculously split in three units.

Unit – Add Ability gives a unit a certain ability, it didn’t initially have. You can give any ability to any unit (yes, that means that you can give hero abilities to normal units) but there are some restrictions. Giving a hero ability will not allow that hero to upgrade the ability to higher level, when leveling himself.

Unit – Set Level of Ability allows you to change the level of a certain ability (to whatever you want, even if it greater or lower than the current level). Thanks to the latest patches, unit abilities can have multiple levels. Accessing those levels can be done only through this action.

Unit – Issue Order  allows you to order an unit to do something. Orders can include moving, attacking, patrolling, casting a specific ability, practically everything the player could order the unit to do. There are more types of orders, each being a separate action. For example, Unit – Issue Order Targeting a Unit orders the unit to do something to another unit, such as attacking that unit or casting a certain ability at that unit.

Note


The player can remove orders, if he or she orders the unit to do something else.

Note


If you want to order an unit to cast an ability, you will need to order the unit to cast the root ability (ability based on which you made your dummy custom ability, doesn’t always have to be a dummy ability though). Let’s say that you based your ability off Neutral Hostile Anti-Magic Shell. You can order the unit to cast exactly the custom ability. However, you can order the unit to cast Undead – Banshee – Anti-Magic Shell. Through this, the action will order the unit to cast an Anti-Magic Shell. If the unit has more than one ability with the same root (Anti-Magic Shell in this case), the trigger will cast the first ability of that type which doesn’t have cooldown activated. That is why it is advised not to give an unit more than two abilities based off the same root ability, even if you base one off the Neutral Hostile and the other one off the race ability (in this case Undead).

Unit – Create Unit is an action which creates a certain type of unit, for a certain player, at a certain point on the map. The units appear out of nowhere, but they are not considered as being “trained” by the player, meaning that they won’t appear as trained units in the scores at the end of the map. Except that, the unit retains all its functions, from abilities to properties and to food cost.

Unit – Damage Target orders an unit to damage another unit. However, this will not be done in “Unit attacking another unit” manner. This function was introduced by Blizzard so that instead of substracting the life of an unit when you want to simulate damage, actually be able to order an unit to damage another unit. This also gives access to bounty (experience and money for the caster of the spell), thing unavailable through substraction of life. So, the damage is done instantly, and it won’t be noticeable. The damaging unit will not face, nor will it throw a projectile at the target or even show the “Attack” animation.


You will need to mention the amount of damage so no, this won’t just order the unit to do the damage in the range it is capable of. For example if an unit has 10-15 damage the unit won’t do damage between 10 and 15 to the target, instead you will need to give that amount of damage. Moreover, this trigger is capable of allowing you to choose the type of damage (Chaos, Magic, Piercing, Siege etc) and so, the damage dealt will depend on the target’s type of armor, if it is of Magic type it won’t affect Spell Immune units and so on.

Unit Group – Pick Up Every Unit in Unit Group and do Actions is one of the most important actions, used not only in spellmaking, but usually when working with groups of units. It takes each unit in an unit group and does a series of actions, for EACH of those units. The series of actions are run at the same time, making it an excellent method for some AoE spells.

Note


If in the series of actions the picked units are not mentioned, the actions will just be a series which will occur N times (N being the number of picked units) at the same time.

Unit Group – Pick Up Every Unit in Unit Group (Matching Conditions) and do Actions does quite the same thing, except that the units for each the series of actions are executed only for those units which matches a series of conditions. This can be again used in AoE spells when, for example, you want to pick only friendly units, or enemy, or spellimune or others.


VIII. Basic Events and Conditions


I’m not going to explain all the Events and Conditions, though they are not as numerous as the actions are. However, some of them are of Great importance in spellmaking, without which spell enchancement is impossible.


a) Events


Maybe the simplest event is Map Initialization. It activates the trigger just when the map starts. It is recommended not to use any conditions with this event, because it will occur only once, at the beginning of the map, regardless the circumstances. After that, it becomes completely useless. So, adding a series of conditions which may not all be fulfilled at the beginning of the map, will just make the trigger just a waste of memory.

Unit – Generic Unit Event pack of events is also very useful. These activate the trigger when ANY unit does something general, such as casting an ability (any, not a certain one), dying, being attacked and so on. Those we will need mostly are: Unit Starts the Effect of an Ability, Unit Begins Casting an Ability, Finishes Casting an Ability, Begins Channeling an Ability and Stops Casting of an Ability.


a) Begins Casting vs Starts the Effect


If you try both these events, you will say that they do the same thing, with no difference. But Blizzard is not stupid. They didn’t add both for nothing. You will notice at a closer look at the small difference if not noticed by the spellmaker can lead to severe bugs, which can be used to abuse the spell.


An Unit Begins Casting an Ability fires the trigger when the unit prepared to cast the ability, just before it wasted the mana or fired the cooldown. Players abusing the spell, by moving the caster just before the cooldown is fired (it’s not even hard to do that), will obtain the effect of the spell without firing the cooldown or losing any mana. This means that they can cast the ability an infinite number of times, with no restrictions. Quite bad eh? And then, you may wonder when should you use this event. Well, by now the sole use I found to it is to validate targets. Object Editor may not always assure the valid targets of the spell for you. In this case, you will need to validate them yourself. Make a trigger which activated by this event checks if the target is valid. If it isn’t, you just order the casting unit to stop, followed by an error message.


An Unit Starts the Effect of an Ability just when the cooldown is fired and the mana is wasted. So yeah, the main trigger which activates when the spell is cast should have this event, in most of the cases atleast.


b) Finishes Casting An Ability and Stops Casting an Ability difference ca be noticed only in the case of the channeling spells.


Finishes Casting an Ability fires the trigger when the unit has fully cast the ability, to its full effect. This means that if a Priestess casts Starfall but a damned Dreadlord casts sleep at her, the trigger will not occur in her case because her ability hasn’t finished casting. It was actually stopped! In conclusion, you may get some errors if you haven’t used the right event.


Stops Casting a Ability fires either when the unit stops casting the ability before it has reached full effect (either willingly or not) or the unit achieves full effect. I believe that this is an error for Blizzard, since normally it shouldn’t occur into the second case as well. But apparently, it does! So if you want to know when to stop the effect of a channeling spell, use this event, instead of “Finishes Casting an Ability”.

Note


I don’t suggest you use either of them with non-channeling spells.

Event – Unit is Attacked is again a buggy event. Like “Begins Casting an Ability”, this fires the trigger only when the unit begins to attack. If, for example an archer attacks an unit but stops before the arrow is fired, this trigger still occurs. This probably cannot be solved yet until Event – An Unit is Damaged event is added by Blizzard. This event exists for now only for specific units.

Timer – Every x seconds event is also used, either for channeling spells (which we will be studying later) or to make a trick wait, lower than 0.10 (again, I will explain later). This event fires the trigger every x seconds. It is very useful if you want to repeat a certain series of actions. A great advantage is that timers can have a value down to 0.01 seconds, which is 10 times lower than the wait limit. It is important to know that this event exists, and to know what it does. Who knows when you will need it?

Event – An Unit is being Summoned can be used to track units summoned through spells, such as Inferno and Water Elemental. This doesn’t track the process of casting the spell, but it tracks the simple fact that an unit has been summoned by a spell.


b) Conditions


There isn’t much to say about conditions. All conditions are just comparisions. The compare if two variables are equal or not or in the case of numeric variables Less than, less than or equal to, greater than, greater than or equal to comparisions also appear. You can compare only two variables of the same type, but you can replace variables with Auxiliary functions (check chapter XI).

Note


Every related to trigger Conditions is also true for the condition of an IF.


I will stop upon some special functions, which are more like a helpful tool for applying conditions, than a comparison. These functions are the last into the conditions chart in the trigger editor. They can ALL be used in place of a condition, and you will soon understand why.

Note


A comparison or function which return or is false, when involving conditions, is interpreted as if that condition is not fulfilled.


First is AND function. It has two fields, and into each you can place another condition. This function returns a true value (meaning that the condition is fulfilled) if both the conditions are true. If atleast one of the conditions is false, then the function returns false as well.


Second function is OR function. It again has two fields, each for a condition. If both conditions are false, then the function returns false too. So atleast one of the conditions must be fulfilled in order for the function to return true as well.


The third function is AND (All Conditions Are True). Unlike the other two functions, you can insert into it as many conditions as you want. However, keep in mind that all those conditions must be fulfilled if the function is to return true.


The last function OR (Any Condition Is True). Like the previous function, you can insert as many conditions as you want. In this case, only one of the conditions must be fulfilled if the function is to return a true value.


That’s all the philosophy for conditions. I would also like to mention that there can be conditions in conditions. There is a type of comparison called Boolean Comparison. There are situations in which certain functions can return a true or a false value (for example, Group is Empty function checks if an unit group is empty, and if it is, it returns true but if there is atleast an unit in the group, it returns false). However, conditions themselves have this property. If a condition is correct, then it returns true, but if it isn’t then it returns false.


So, Group is Empty function can also be replaced by a comparison: Number of Units in Group is equal to 0. If it is so, then the comparison will return true and the group can be considered empty, but if there is atleast a unit in the group, it will return false. The code would like this:

 Code:

((Number of Units in Group) is equal to 0) is equal to false

It may sound confusing at first, but if you study this chapter a little bit more, you will realize that it truly is like this.


IX. Event – Response Variables in Detail


In this chapter, I’m going to take the most important Event – Response variables and explain when to use them.

(Attacking Unit) – Used along with the event “Unit is being Attacked”. Refers to the attacker.

(Attacked Unit) – Used along with the event “Unit is being Attacked”. Refers to the attacked unit (kinda obvious, eh?).

(Killing Unit) – Used along with the event “Unit dies”. Refers to the killer (unit whose attack killed the victim).

(Dying Unit) – Used along with the event “Unit dies”. Refers to the victim (unit killed by the Killing Unit).

(Casting Unit) – Use with casting events (read previous chapter). The variable refers to the unit which casts the ability, spell’s source.

(Target Unit of Ability Being Cast) – Use with casting events (read previous chapter). The variable refers to the the unit targeted by the spell (if the spell is meant to directly target an unit). Such spells include Soul Burn, Chain Lightning and Storm Bolt.

Note


In the case of chain spells (Chain Lightning and Healing Wave), the variable refers only to the first unit of the chain.

(Target Point of Ability Being Cast) – Use with casting events (read previous chapter). The variable refers to the point at which the spell was cast (if it was meant to target a point). Such spells include targeted AoE spells (Blizzard, Flame Strike, Earthquake, etc) or single point spells (Shockwave, Carrion Swarm, Pocket Factory etc).

(Summoned Unit) – Use it with “An Unit is summoned” event to refer to that unit. If more than one unit is summoned by the spell (such as Seer’s Spirit Wolves), the variable may cause problems.

(Summoned Unit) – Use it with “An Unit is summoned” event to refer to the summoner (the source from which the unit is summoned). Cannot be replaced by (Casting Unit) because the event refers to the fact that an unit has been summoned, not that a spell has been cast.

(Triggering Unit) – Can be used with all unit events. It is usually the unit to which the event directly refers, so it can replace (Attacked Unit), (Casting Unit), (Dying Unit), (Summoned Unit) and others not mentioned above. I highly recommend that you use this variable because it is unaffected by waits (it can be used even after waits because it is not reset until the end of the trigger).

(Picked Unit) – Should be used along with the Pick up Every Unit or Pick up Every Unit Matching Conditions actions. You need to use this so that you can refer to the unit picked. The variable exists only inside the actions of the Pick up Every Unit/Pick Up Every Unit Matching Conditions.

(Matching Unit) – Should be used everytime you want to refer to an unit inside a “Unit Matching Conditions” condition. Yes, this involves the Pick up Every Unit Matching Conditions[/u] action too.

(Last Created Unit) – Use this right after the Unit – Create Unit action, to refer to the unit you have just created.

Note


Do not create more than one unit at a time if you want to use this constant or it will store just one of the units you created.


X. Manipulating Triggers


There are some techniques through which you can manipulate triggers. This can mostly be done through special actions, made soley for this.


A first technique is to Turn On/Off triggers. All triggers are normally Turned On but there is a checkbox (initially unchecked) labeled Turn Off. If you check it, the trigger will be initially Turned Off. You can also turn the triggers On/Off through the function Trigger – Turn Off and Trigger – Turn On.


But what exactly is a Turned Off trigger? Usually, if an event takes place in certain conditions, the actions take place. This is the basic definition of triggers. But in the case of the Turned Off triggers, the actions never take place, no matter the conditions. A very good technique is to combine Turned On/Off triggers with Timer – Every x seconds event. Turn On the trigger so that the actions take place a number of times, and then just Turn if Off. This can be a very handy technique for channeling spells.


Another technique of manipulating triggers is by Destroying them. A destroyed trigger is simply discarded from the memory, so the actions will never take place, no matter the event or the conditions. In fact, not even those exist. The trigger is gone until you restart the map! Use it for Map Initialization triggers and destroy them at the initialization, to save some memory.

Note


Destroying a trigger or Turning it Off prevents only further executions of the trigger. The current series of action is made to the last, no matter what, if the trigger was fired under the event with the correct conditions.


A final technique I would like to present you is not really manipulating triggers, but the actions. The action Skip Remaining Actions does what its name says it should do: it skips all actions after it. This doesn’t affect the trigger. If the event takes place in the conditions, the series of actions will start once again. Skip Remaining Actions only prevents the remaining actions (with no exceptions) to take place during the current executions.


XI. Auxiliary Functions


There are several other functions, separate from the events/conditions/actions, but which can be used inside them. These functions are used to replace variable types, like units, points, and even numbers.

Note


In JASS, you will notice that the Event – Response variables are in form of actions. That is because they are truly Auxiliary functions. I have labeled them as variables, because in the stage they are, they act like them. They have a duration of life, depend on the event, and are extremely frequently used.

Point with Polar Offset will help you get a point on a circle, with a certain radius and a central point known. The function has three parameters (blue fields in GUI): first to determine the center of the circle (which is a location), second to get the radius of the circle and third is used to get where the point is actually on the circle, in degrees. You can use this function for example to make dummy units cast radial shockwaves.

Note


In Warcraft 3, degrees values are as following:


 0 for east


 90 for north


 180 for west


 270 for south

Random Integer and Random Real are also very good actions. Use them to substitute an integer or a real value. What these functions do is get a random number between a value x and a value y. The value seems to be completely random, do I'm sure that at a point, the randomization series start to repeat.

Owner of Unit returns a player value. It checks to which player does a certain unit belong, and so, you can use this to replace a field which requires a player (such as the one in Unit – Create Unit).

Math - Distance Between Points calculates the distance between a point A and a point B. Distance in Warcraft 3 is calculated in pixels. This function has its own uses, which we might discover later.

Math - Angle Between Points returns a real value (in the form of an angle). To better understand this function, I’ll call the source point A and the destination point B. Now, back to Polar Offset, considering that A is the center of the circle, the radius is equal with the distance between point A and point B, and that B is the point on the circle, the angle is equal with the location of B on the circle, in degrees. For more information, check Polar Offset at the beginning of this chapter.

Arithmetic is usually the essential function when you work with numbers. It allows you to add, substract, multiply or divide two values. To do these operations with more than two values, you will have to insert an arithmetic in place of another value, since the returned value is either real or integer.

Convert to is also an essential set of auxiliary functions. They allow you to convert values from a type to another. The convert functions are: Integer to Real, Real to Integer, String To Real, Real to String, Integer to String, String to Integer, Degrees to Radians and Radians to Degrees. You will need these functions if you want to insert integer values for example into fields which allow only real values, or vice versa.


XII. Single-Target Spells


This is probably the simplest class of spells, when referring to enchancing. These spells are those which target a single, specific target. This means that the player chooses the target and only it is affected by the spell. Such spells include Cripple, Soul Burn, Storm Bolt, Entangling Roots, Rejuvenation, Possession, Holy Light, Death Coil, Life Drain, Siphon Mana, Mana Burn, Shadow Strike, Finger of Death and others.

Note


Chain Lightning, Healing Wave or other chain spells are not single target. Even though the player selects the first target, to trigger the effect, it’s not that easy, because it affects other units, “in chain”. However, they can be used as dummy spells, if they are supposed to hit only 1 target, and do nothing to that one anyway.


For now, we shall stick to non-channeling or buff spells. Once you learn how to make spells of each category, then channeling/buff spells require separate technique, which once learned, you won’t need to struggle with each category again, theoretically of course.


So, let us begin. I’ll start with the theory and then we will make a practical spell. The idea of the Single Target spells is simple. You check whenever a unit casts the custom ability, and then you create dummy units and order them to cast various spells at the target, or damage it, or maybe something else, depending on your spell. That is for the pure effect of the spell. In this tutorial I won’t explain special effects. Maybe I’ll make a separate one later!


And now, we are getting to do a practical example. Here are the basic steps:

Step 1 – The idea of the spell


We are going to make an apparently difficult spell (especially if you haven’t trigger enchanced a spell before) called Shock Destructor (blah, probably not the best name, you can change that if you want). Its description is pretty tricky so here it is:


->The caster emits a bolt of lightning which targets an enemy unit. Depending on its nature, it does a different effect.


 -Buildings take damage (50 at level 1, 75 at level 2 and 100 at level 3)


 -Mechanical Units are stunned (6 seconds at level 1, 7 at level 2 and 8 at level 3)


 -Organic Units are purge (3 seconds at level 1, 4 level 2 and 5 at level 3)

Step 2 – Making the dummy ability, dummy unit and other abilities


Let’s search for a dummy ability. Remember, it must be an instant, single target spell. I have already mentioned most of the spells up, though they are not all instant spells (their effect lasts a couple of seconds and can be dispelled). However, you can make those abilities instant as well. Just set their duration to 0.01 and it will appear as if they were instant.

Note


A duration of 0.00 doesn’t mean no duration, it means that the spell will last forever, or until it is dispelled. It is a convention.

Note


Some abilities, may leave secondary effects even if their duration is almost unnoticeable. Spells that interrupt channeling abilities (cause stun, sleep, silence etc) may be a problem if your spell is not supposed to actually break the channeling. I suggest you avoid those spells as much as possible, unless you intend to break channeling spells anyway!


Now, looking back at the description of the spell, you will notice that I mentioned that The caster emits a bolt of lightning. Hmm, don’t we already have a spell which does this? In fact, we do. Chain Lightning that’s what it does. It causes the caster to emit a bolt of lightning. However, we want this spell to affect only a single target. Make a copy of the spell, and let’s change some of its fields.


First of all, we will make the spell a dummy spell, meaning that it won’t affect the target in a real way. Set the initial damage to 0 for all the levels, number of targets equal to 1 (it would be strange to bounce but affect only the initial target)… and that’s kinda it. If you would test the spell, it would do nothing.


Now its time to make the spell look realistic. Leaving the old Chain Lightning description, name and icons would make the player think that he is casting Chain Lightning. So change its name to Shock Destructor, all descriptions accordingly, and I would even saw that you change the icon. I for one used the Dark Portal ability icon, but these stuff really don’t matter. Make it look convincing!


And now, we still have some technical things to solve. The spell has the old mana cost and cooldown of Chain Lightning. I have thought as some values, but you may change them as you like, to make the spell balanced. The mana cost I chose is 70/65/55 (level 1/level 2/level 3) and the cooldown 8/8/8. One last thing to change are the targets. Chain Lightning affects only organic targets. Here are the valid targets I gave (and you’d better follow them): Air, Ground, Enemy, Organic, Structure, Mechanical, Vulnerable. Its good to give all the values you find valid to your spell. In this case, not giving Vulnerable may not cause the spell to be cast on Invulnerable units anyway, but you shouldn’t risk. Some spells may have this field bugged.


You can now give the ability to a hero and test it. It will look extremely realistic, but if you order the hero to cast it, you will only get a unit struck by a bolt of lightning, with no consequences, but mana lost and a cooldown fired. It’s a promising start though. Let’s get back to the Object Editor, since we ain’t done.


We will have to give some abilities to the dummy units. We need a single-target ability that stuns (I suggest Storm Bolt) and Purge (the description was kinda obvious). Copy-paste both the abilities, and edit them in the following way.


-> Make them 3 level abilities, but leave them unit abilities.


-> Change their mana cost to 0/0/0, duration to 3/3/3 (usually a high value so you can assure that the dummy unit is able to cast it only when ordered and not again unwillingly), their duration to how long the spell is supposed to affect the target and you should even change their buffs.


-> Watch out for targets. Make sure that the dummy unit is able to cast the ability on the valid unit. In this case, make sure Storm Bolt can target Enemy, Mechanical units and that Purge can target Enemy, Organic units.


-> Icons and descriptions usually should be left blank, because you won’t see them anyway


-> To know for which spells you use the custom abilities, I would suggest that you add an editor suffix, with the name of the dummy spell. In this way, it will make life easier for you. In this case, I got Purge (Shock Destructor) and Storm Bolt (Shock Destructor)


And last but not least, don’t forget the dummy unit. If you don’t remember how to make a dummy unit, go back to chapter 3 and get the instructions from there.


 Last edited by a moderator: Mar 31, 2007

#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

(Offtopic note: I can't post the whole tutorial into a single post, because it's too long).

Step 3 – Trigger Enchancing the Ability


Probably the hardest step. It may even seem scary, but in truth, it is not. Let’s take the code step by step. We need events, conditions and actions. If you don’t remember how any of these work, please reread the previous chapter, until you fully understand how triggers work. Do not skip those chapter, because they are extremely important to comprehend trigger enchancing. Keep reading and trying to understand them until you are completely sure you know how triggers work.


Create a new trigger. Again, I suggest that you give triggers the name of your spell (if you need more than one trigger follow them name by something else too), so you can recognize them later, if you ever want to change your spell. It is very important to keep an order into both the object editor and the trigger editor.


Now, we know that this trigger should activate when an unit casts Shock Destructor. In code, it would look like this:

Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Shock Destructor

It’s nothing new, if you have read chapter VIII. The basic event, the basic condition. Now, let’s get deeper to actions, where things get messy. We know that we need a dummy unit, to either damage or cast an ability at the target. So, create a dummy unit at the position of the target, which belongs to the caster. Moreover, the unit should disappear after a short period of time, so it would be good to add it an expiration timer with a very short life value. The action would look like this:

Actions

Unit - Create 1 Dummy for (Owner of (Triggering unit)) at (Position of (Target unit of ability being cast)) facing 0.00 degrees

Unit - Add a 1.50 second Generic expiration timer to (Last created unit)
Now, things get more complicated, because depending on the type of the target (organic, mechanical or building), the dummy does something. It is clear that this is the step where things get different for each single-target spell. We are going to use multiple Ifs, each to check if the target is Organic, or Mechanical or Building. I don’t suggest you insert in the else of an if another IF because you may slowly realize that you get nothing from the code, especially when more complicated codes come out.


So, we will start with the first IF. It checks if the target is a building, and if it is, it damages the target. However, for the damage you need a formula, because if the level of the ability is bigger, then the damage is bigger as well. So your formula must depend on the level of the ability. Let’s have a look at the damage:


50 for level 1, 75 for level 2 and 100 for level 3. It is clear that with each level, the damage increases by 25. This difference between levels MUST be a constant if you want to use a formula. The difference is called Ratio. Usually the formula is something like this:

 Code:

(Level 1 damage – Ratio)+(Ratio*Level of the Ability)


And suddenly things are getting simpler eh? In code, the formula would look like this:

 Code:

Damage = (50-25)+(25*(Current Level of (Shocking Destructor) for (Triggering Unit))


This formula can even be simplified, to ease the work of the PC and make your spell more efficient. You already know that 50-25 is 25. Instead of putting the game calculate it everytime the spell targets a building, do it yourself. It doesn’t cost you anything, but it does the PC. Make your formula as efficient as possible!


Now that we have the formula, the next step of the action is just implementing what you learned. Here is what you should get:

Final
If (((Target unit of ability being cast) is A structure) Equal to True) then do (Unit - Order (Last created unit) to damage (Target unit of ability being cast) for (25.00 + (25.00 x (Real((Level of Shock Destructor for (Triggering unit)))))) using attack type Magic and damage type Fire.) else do (Do nothing)

Real() makes a conversion to real. In this case, I converted an integer, because in an operation you can use numbers of the same type (real or integer) but you can’t mix them. In this case, the damage is supposed to be real so since (Level of Ability for unit) function returns an integer value, you have to convert it yourself. You will encounter this a lot.


Now if you will test your ability on a building, that building will get damaged! However, we still need to make it affect organic and mechanical units. Here, the theory is the same, just that you use different abilities. So I will explain only for organic.


You have to give the dummy unit the custom purge ability, and order it to cast it at the target. But once again, the spell should last more for each level. Still, I didn’t ask you to give the spell three levels for nothing. And if you remember, there is an action which allows you to set the level of normal abilities too. So all you have to do, is give purge ability to the dummy, set its level to the level of Shock Destructor, and order the dummy to cast it at the target. An organic unit is that unit which isn’t either mechanical or structure. Here is the code:

Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is A structure) Equal to False

((Triggering unit) is Mechanical) Equal to False

Then - Actions
Unit - Add Purge (Shock Destructor) to (Last created unit)

Unit - Set Level of Purge (Shock Destructor) for (Last created unit) to (Level of Shock Destructor for (Triggering unit))

Unit - Order (Last created unit) to Orc Shaman - Purge (Target unit of ability being cast)

Else - Actions

Now change this code for Mechanical Units. Down you can see the complete code. If you’ve copied it correctly, the spell should work:

Shock Destructor
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Shock Destructor

Actions
Unit - Create 1 Dummy for (Owner of (Triggering unit)) at (Position of (Target unit of ability being cast)) facing 0.00 degrees

Unit - Add a 1.50 second Generic expiration timer to (Last created unit)

If (((Target unit of ability being cast) is A structure) Equal to True) then do (Unit - Order (Last created unit) to damage (Target unit of ability being cast) for (25.00 + (25.00 x (Real((Level of Shock Destructor for (Triggering unit)))))) using attack type Magic and damage type Fire.) else do (Do nothing)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is A structure) Equal to False

((Triggering unit) is Mechanical) Equal to False

Then - Actions
Unit - Add Purge (Shock Destructor) to (Last created unit)

Unit - Set Level of Purge (Shock Destructor) for (Last created unit) to (Level of Shock Destructor) for (Triggering unit))

Unit - Order (Last created unit) to Orc Shaman - Purge (Target unit of ability being cast)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is Mechanical) Equal to True

Then - Actions
Unit - Add Storm Bolt (Shock Destructor) to (Last created unit)

Unit - Set Level of Storm Bolt (Shock Destructor) for (Last created unit) to (Level of Shock Destructor for (Triggering unit))

Unit - Order (Last created unit) to Human Mountain King - Storm Bolt (Target unit of ability being cast)

Else - Actions

Congratulations, you have completed your first trigger enchanced spell! As you will practice, you will be able to do more complicated single target spells. But now, it’s time to get to the second category.


XIII. Area Of Effect (AOE) Spells


Another class of custom spells, the AoE spells affect units in an area. They can be enemy, friendly, ground, air… all you have to do is have the right effect. As we will enter triggers, you will understand the entire mechanism but until then, it’s time to get the spell step by step. AoE spells can also be classificated in two types: targeted and non-targeted.


Targeted AoE spells are those spells for which the players specifies the area in which the spell will take effect. Such spells include Blizzard, Flame Strike, Cluster Rockets, Devour Magic, Silence, Inferno, Dispel Magic, Disenchant, Earthquake, Death and Decay, Volcano and Rain of Fire. Specific for these spells is that when the player wants to cast them, a circle (different for each race) appears. The circle covers the area in which the spell takes place. When making the spell, you will need to be careful so that it covers the entire circle, not more, nor less.


Non-targeted AoE spells on the other hand don’t have a selectable area. The effect of the spell takes place around the caster. Such spells include Thunder Clap, Resurrection, War Stomp, Big Bad Voodoo, Starfall, Tranquility, Fan of Knives, Roar, Taunt, Mana Flare and Howl of Terror. These spells can also be cast accidentally, because only clicking on their icon or hotkey is enough to activate them. But this is not a problem spellmaking can solve. Only the player can.

Note


Non-targeted AoE spells can be confused with selfcasting abilities (such as Berserk, Windwalking and Mirror Image) because of the way they are activated. We will discuss differences once we get to that class of abilities.

Step 1 – The idea of the spell


Once again, we are going to do a non-channeling, preferably non buff-triggered. We will attempt to understand these later. So for now, we will just stick to a Targetable AoE spell. I will then teach you how to make this non-targetable, just by changing a single thing in the code, and change the dummy unit.


The spell we are going to do now is pretty classic: Mass Sleep. It’s effect is pretty obvious: all the units in an area of effect are put in a dark sleep, and cannot do anything until attack or the buff wears off. The stats should be something like this:


Mana Cost – 95/95/95


Cooldown – 14/14/14


Area of Effect – 300/400/600 (notice that the bonus AoE between levels is not constant)


Duration – 15/25/35 (for normal units)


 - 3/5/6

Step 2 – Making the dummy ability, dummy unit and other abilities


This time it will be easier to work with the object editor, because you have already done it. If you haven’t, go to the previous chapter and learn how. Don’t skip!


The dummy unit is simple. You can either do one once again, or use the dummy unit you made for the previous spell. I usually make only one dummy unit, even if I have more than one spell. After all, you add the abilities through triggers to the dummy units, so there is no difference if you use the dummy unit you use for another spell as well.


In the case of the dummy ability, you are going to need a targetable AoE spell, if you plan to make the Mass Sleep targetable, or non-targetable AoE spell, if you plan to make the Mass Sleep target units around the hero. It’s up to you.


If you base your dummy spell off channeling spells (such as Starfall and Blizzard), make sure that you set the duration to 0.01. Nevertheless, make the spell have no effect. Remove all armor/life/mana/damage reductions and bonuses. Assure that the spell does nothing. Then set the mana cost, duration, and in the case of the targetable AoE spell, the correct AoE for the three levels. Give the spell a good description, tooltips, name, hotkey and icons. Don’t forget the button position! Lastly, check to see if it works like a dummy ability.


Now, let’s make the ability for the dummy unit. In this case, we will need only one ability: Sleep. Give it the correct duration stats, buff, reduce mana cost to 0 and don’t forget that it must be a three levels unit ability. You don’t need to mess up with targets, because in this case, we want the mass sleep to affect all units a normal spell would.

Step 3 – Trigger Enchancing the Ability


It’s time we begin the coding. Since you’ve already made a spell, it shouldn’t be very difficult. First, give the correct event and conditions:

Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Mass Sleep

Now, let’s analyze the actions first. The trigger is supposed to take every unit in the area/around the hero which is a valid target, create a dummy unit for it, and cast sleep on it. Valid targets would be enemy, non-mechanical, alive and non-structure units. We have a total of four conditions for valid targets.


However, we are still not done. We have a problem. The radius is different for each level. If it would be constant, it would be just perfect eh? But I want to complicate things, and make the spell’s radius different for each level. And to make things worse, the radius increase is not constant. Even in this case, you can get a formula, or you can use the second method, with multiple Ifs, which branches the actions. You can use any you want, but I advise you to read them both, because there will be cases when you will need to use the second method, when for example the spell has different effects each level (like, level 1 puts to sleep, level 2 entangles and level 3 stuns).


a) Formula method


Let’s have a look at the radius once again. 300/400/600. The bonus is 100 for level 2, and twice for level 3. Hmm… twice. This means that you can multiplicate 100 with the number of the level minus 1, and add it to 300. So the formula would look like this:


300+(100*(Level of Mass Sleep-1)


That wasn’t hard was it? Ingame it would look like this:

 Code:

300+(100*(Level of Mass Sleep for (Triggering Unit)-1))


That’s all for method 1. But what if the radius would’ve been 300/400/550? It would’ve meant that it gives 100 for level 2 and 150 for level 3. Isn’t that 300+(level * 100 /2)? Try to find the formula, do some math, and if you just can’t find it, then just go to method 2.


b) Branching method


The idea is very simple. You will have a different series of actions for each level. Though it may not seem really efficient, it is worth the effort, especially when you can’t find a formula. This could be avoided if you would use a group variable, but I prefer not to get involved into variables, for now, because they break your MUI.


So, the main idea is to have a series of conditions, for each level, and change the radius only, per level. So the trigger would look like this (general sketch):

 Code:

If – Level of Mass Sleep for (Triggering Unit) is equal to 1
Then – Series of actions for level 1
Else – Do Nothing

If – Level of Mass Sleep for (Triggering Unit) is equal to 2
Then – Series of actions for level 2
Else – Do nothing

If – Level of Mass Sleep for (Triggering Unit) is equal to 3
Then – Series of actions for level 3
Else – Do nothing


This would work for any other spell, in whose case you can’t use a formula for all levels, and you have to split the actions. However, this code might cause bugs, if into the THEN there is a wait. Let’s take this abstract example:


You make a Cluster Rockets ability, and you want to trigger it all by yourself. The series of actions would include a considerable wait, because Cluster Rockets waits until the impact of the missiles. After the wait, the last part of the actions is executed. Then the trigger will check level 2, and then level 3, if the hero had level 1. But consider this: the caster shoots the rockets, and then increases the level of the ability, since it has points to spend. In this case, the second IF is taken in consideration as well, because by the time the trigger gets there, the caster has level 2 for the ability.


You would say then that we could just change the order of the IFS. We should start with the highest level and finish with the smallest. In this way, the trigger cannot be abused, by increasing the level of the ability while it is being executed. True! In most cases this would be the solution. But a problem might be Tome of Retraining. In the case of waits with a period higher than 2 seconds, the caster has enough time to use the tome and relearn the ability. In this way, if before the training the hero had level 3 let’s say, and now he has only level 1 for the ability, both series for level 1 and level 3 will occur. Sorry if it sounds confusing!


So we need to find a solution to fix this. For that, you need to understand how an If/Then/Else series of actions really works. As I told you before, if the IF conditions are true, the THEN actions are executed. But if atleast one condition is not fulfilled, the ELSE actions take place. But IF/Then/Else can also be considered an action so inserting it into Else actions of another If/Then/Else is possible. Check this code:

 Code:

If – Level of Mass Sleep for (Triggering Unit) is equal to 1
Then – Series of actions for level 1
Else – If – Level of Mass Sleep for (Triggering Unit) is equal to 2
 Then – Series of actions for level 2
 Else – Series of actions for level 3


And now to explain. When the spell is cast, it is level 1. The series of actions executed is those of THEN. Now, even if the level of the ability is set by the player to 2, the series of actions for level 2 won’t occur anyway, because the ELSE actions don’t take place, since the THEN actions already took place. If you plan to have more levels, you will have to insert in the ELSE conditions of the second If/Then/Else another If/Then/Else. This may sound extremely complicated, so if you just don’t get it, you’ll have to learn the code, and not understand it. But it would be better to understand it. Spellmaking (actually programming) is something logical!


And now back to our spell. You can try to make the series of actions with the IF/Then/Else branching, but I will use the formula. It would be a challenge for all of you to adapt it to the branching method! You can even try to do both the methods yourself since I already gave you all the instructions necessary.


Nevertheless, here is the complete code of the spell:

Mass Sleep
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Mass Sleep

Actions
Unit Group - Pick every unit in (Units within (300.00 + (100.00 x (Real((Level of Mass Sleep for (Triggering unit)))))) of (Target point of ability being cast) matching ((((Matching unit) is A structure) Equal to False) and ((((Matching unit) is Mechanical) Equal to False) and ((((Matching unit and do (Actions)
Loop - Actions
Unit - Create 1 Dummy for (Owner of (Triggering unit)) at (Position of (Picked unit)) facing 0.00 degrees

Unit - Add Sleep (Mass Sleep) to (Last created unit)

Unit - Set Level of Sleep (Mass Sleep) for (Last created unit) to (Level of Mass Sleep for (Triggering unit))

Unit - Order (Last created unit) to Undead Dreadlord - Sleep (Picked unit)

Unit - Add a 1.50 second Generic expiration timer to (Last created unit)

I have one more note. When you pick units, for those who want it to target units around the caster, change the (Target Point of Ability Being Cast) to (Position of (Triggering Unit)). It’s pretty logical, isn’t it? You picked units around caster, not around a target point.

Exercise


For those who made the spell non-targetable, make it targetable and for those who made it targetable, make it non-targetable.


XIV. Passive Abilities


Passive Abilities are those spells which don’t cost any mana, and can’t be activated. They either randomly take place or give permanent bonuses (unless unlearnt by Tome of Retraining). Example of Passive Abilities include Bash, True Sight, Critical Strike, Reincarnation, Evasion, Elune’s Grace, Moon Glaives, Vorpal Blades, Slow Poison, Resistant Skin, Hardened Skin, Poison Sting, Exhume Corpses, Poison Cloud, Freezing Breath and others.


As for a further classification, I would say Permanent, Offensive and Defensive. Permanent (Hardened Skin, Freezing Breath) are those which give bonuses to the caster and will not just have an effect which occurs randomly. On the other hand, Offensive and Defensive passives take place usually when the hero attacks/is attacked, and have a chance of success (like Evasion and Critical Strike). We will study them all, in the limit of possibilities.

Step 1 – The idea of the spell


I will try to make a Passive skill which combines all these three classes. It is not a conventional skill, maybe even unbalanced, but atleast it works nice. The skill we’ll make is called Elemental Essence. The description is like this:


->Nearby enemy units take 5/10/15 damage every second


->Enemy units that attack the caster have a 15/30/45% chance to be chilled for 3 seconds.


->Each time the caster attempts to attack, there is a 10/20/30% chance that he will summon a bolt of lightning as well to strike the aimed unit for 50 damage.


Even if it looks overpowered, it will be great to explain the passive abilities system. It no longer has mana cost and cooldown problems so we don’t need to worry about that anymore.

Step 2 – Making the dummy ability, dummy unit and other abilities


Some of you may already haste to choose the abilities for this spell but they should stop at once, because I need to explain some last issues about the object editor.


First, we need the dummy ability. It will be a passive with no effect. So I suggest that you use Evasion, Critical Strike or similar passives which have a chance of success, and just set the percentage to 0%. This will practically make the spell a perfect dummy ability. Now give it adequate tooltips and icons.


For the dummy unit, just use the previous dummy units you made before, or you may go ahead and do yet another one (though this is just a waste of objects). It’s up to you!


Last but not least, here comes the happy part. We need to make abilities part. First, we need to pick the ability for the dummy unit. It should chill units… Hmm, isn’t chilling quite the same thing with slow? Make a slow ability which lasts 3 seconds and has the chilling buff. It needs only one level, because difference between levels is just the chance of success. Set mana cost to 0.


For the damage all units around the caster use Permanent Immolation. Make it a three levels ability and give it the right stats. As for the bolt of lightning, make a chain lightning which hits only one target, and does 50 damage. Don’t forget to change mana cost to 0.


If you succeeded to get the right dummy abilities as well, I think you already know everything about dummy abilities. If you don’t, you hopefully will be able next time!

Step 3 – Trigger Enchancing the Ability


This ability is already tough. We need multiple triggers this time! So, let’s analyze the spell step by step. First, the passive ability is supposed to have a chance to send a bolt of lightning when the caster attacks. So make a new folder in the trigger editor and call it Elemental Essence. Then, inside it create a trigger called Essence Bolt.


The event of this trigger may cause a problem. The trigger is supposed to activate when the caster attacks. However, there isn’t a generic event “An Unit Attacks”, only “An Unit is Attacked”. So we’ll have to use that one. As I said before, the event is buggy, and the event may take place even only when the caster attempts to attack, not when he really attacks. “An Unit is Damaged” generic event does not exist, and we cannot get the exact moment every target of the caster is damaged, unless we use advanced triggers, so we’ll have to stick to it. It should be fine for now…

Elemental Essence

Events
Unit - A unit Is attacked

Now, the condition is a little different as well. It should check if the (Attacking Unit), aka the caster, has the passive ability. However, “Unit Has Ability” condition doesn’t exist. So again, we will have to trick the condition. There is an auxiliary function which allows you to determine the level of a certain ability for a certain unit.

 Level of Ability for Unit

Click to expand...


Also, you should know that if the returned value is equal to 0, then the unit doesn’t have the ability. So the condition for the trigger would be like this:

Elemental Essence

Conditions
(Level of Elemental Essence for (Triggering unit)) Not equal to 0

Note


The condition could also check if the level for the ability is greater than 0 (so it cannot be less than or equal to 0). But since negative levels for abilities do not exist, “not equal to 0” is enough.


However, the conditions are not done. We know that there is only a 10/20/30% chance so that the attacked unit is struck by a bolt as well. If the roll fails (you are not lucky enough for the spell to occur), then the spell cannot take place. So how do we calculate the chance?


We will use (Random Integer between







 Code:

Integer

 and







 Code:

Integer

) auxiliary function. You will soon notice that auxiliary functions are the essence of triggering. So, we know that this function takes a random integer between two values. But let’s get back to probabilities.


We suppose that there is a chance of 25% for a spell to take place. 25% is 25/100 which is 1/4. Well, supposing that we have a random number between 1 and 4, the random number must be equal to a fixed value (either 1,2,3 or 4 but always the same) if the spell is to take place. There are four possibilities, and only one is correct. 1 from 4 is correct, meaning that there will always be a 25% chance for the spell to succeed.

 Code:

Chance of success = number of possible cases/number of successful cases


And now, since we have a percentage chance, the number of possible cases is 100. So the random integer number would go between 1 and 100, so that we have 100 possible cases. Now, the number of successful cases is equal with the percentage chance. 25% chance means that from 100 chances, 25 are successful. In conclusion, the random integer must be less than or equal with the chance of success, in order for the spell to take place (from 1 to the chance of success, there are as many numbers as the chance of success is). Confusing? Then just follow the formula below:

 Code:

(Random integer number between 1 and 100) Less than or equal to ChangeOfSuccess


This is a general formula, and you can use it whenever you do anything that involves percentage chances. But we are still not done. In this case, the chance of success is different per level. You might say, OMG, so many formulas to deduce? Hehe, in this case I suggest that you make the formula possible, and don’t come with ridiculous percentage chances. However, it is possible to work the conditions so that it works for any percentage values.


Formula is simple.

 Code:

10*(Level of Elemental Essence for (Attacking Unit))

The general method is also pretty simple, because it involves the OR – Any Conditions are True auxiliary function for conditions. If you remember, this function/condition is fulfilled if atleast one condition inside it returns true. So inside it, you need to check if the spell should take place for a lower percentage at level 1, medium percentage at level 2, and high percentage at level 3. Without any blablaing, here is how it should look:

Elemental Essence

Or - Any (Conditions) are true
Conditions
((Random integer number between 1 and 100) Equal to 10) and ((Level of Elemental Essence for (Attacking Unit)) Equal to 1)

((Random integer number between 1 and 100) Equal to 20) and ((Level of Elemental Essence for (Attacking Unit)) Equal to 2)

((Random integer number between 1 and 100) Equal to 30) and ((Level of Elemental Essence for (Attacking Unit)) Equal to 3)

And this works for every percentage you want, and can be easily changed. I suggest you learn both ways, since the first one might work almost all the time (after all you design your spell) but you never know what chances you get when it comes to balancing.


Actions are extremely simple. You create a dummy unit to cast chain lightning at the caster. No comments needed, here is the whole trigger (formula method)

Essence Bolt
Events
Unit - A unit Is attacked

Conditions
(Random integer number between 1 and 100) Less than or equal to (10 x (Level of Elemental Essence for (Attacking Unit)))

(Level of Elemental Essence for (Attacking unit)) Greater than 0

Actions
Unit - Create 1 Dummy for (Owner of (Attacking unit)) at (Position of (Attacking unit)) facing Default building facing degrees

Unit - Add a 1.50 second Generic expiration timer to (Last created unit)

Unit - Add Chain Lightning (Elemental Essence) to (Last created unit)

Unit - Order (Last created unit) to Orc Far Seer - Chain Lightning (Attacked unit)

Phew, this wasn’t that hard, was it? Now, the second trigger we are going to do is the chilling effect. Make a second trigger and call it Essence Chill. Now, it activates when the caster is attacked. Caster switch places, from attacker becoming attacked. So without any further comments, the event and conditions would look like this.

Essence Chill
Events
Unit - A unit Is attacked

Conditions
(Level of Elemental Essence for (Attacked unit)) Greater than 0

(Random integer number between 1 and 100) Less than or equal to (15 x (Level of Elemental Essence for (Attacked Unit)))

Actions
Unit - Create 1 Dummy for (Owner of (Attacked unit)) at (Position of (Attacking unit)) facing Default building facing degrees

Unit - Add a 1.50 second Generic expiration timer to (Last created unit)

Unit - Add Slow (Elemental Essence) to (Last created unit)

Unit - Order (Last created unit) to Human Sorceress - Slow (Attacking unit)


The last part of the spell involves more than one trigger, if we want to avoid all bugs. Firstly, we need a trigger which adds the Permanent Immolation or upgrades it, when the hero learns/upgrades a skill. Through upgrading I mean increasing with a level.


So, the event already exists. “An Unit Learns a Skill”. Condition is a hero skill comparison, and it checks if “(Learned Hero Skill) is equal to Elemental Essence”. Not something uncommon, but I still have to explain something. A Unit Learns a Skill doesn’t refer only when a hero uses an ability bonus to learn an ability he didn’t previously have (for example, a level 2 Far Seer learns Chain Lightning because at level 1 he learnt Feral Spirit). The event also activates when the hero learns further levels of the ability.


Now, in the actions we should practically see if we either need to add the immolation or upgrade it as well. The test is pretty simple. We check if the level of Elemental Essence is equal to 1, and if it is, then we simply add the Permanent Immolation. However, if the ability already exists, then we need to equalize the Permanent Immolation to it as well, giving it the same level the Elemental Essence had. So the actions look like this:

Actions
If ((Level of Elemental Essence for (Triggering unit)) Equal to 1) then do (Unit - Add Permanent Immolation (Elemental Essence) to (Triggering unit)) else do (Unit - Set Level of Permanent Immolation (Elemental Essence) for (Triggering unit) to (Level of Elemental Essence for (Triggering unit)))

And now only one thing remains. The tome of retraining may cause some problems, because if the ability is lost through this item, the Permanent Immolation still exists. But Tome of Retraining is not an ability, but an item. So all we have to do is see when an unit with Permanent Immolation (Elemental Essence) uses such a tome, and remove the Immolation. The trigger would be like this:

Essence Removal
Events
Unit - A unit Uses an item

Conditions
(Item-type of (Item being manipulated)) Equal to Tome of Retraining

Actions
Unit - Remove Permanent Immolation (Elemental Essence) from (Triggering unit)

End of story, spell finished! However, I would still want to teach you an additional thing. As you will notice for now, the spell works, but the Permanent Immolation icon appears on the hero, and it doesn’t look great. We need a way to remove the icon. A solution would be to assign the ability a black icon but not only does it take the place of another icon but the tooltip still appears.


There is a secret (well, not really) which will allow you to hide certain abilities, by disabling them. You may wonder what a disabled ability means. Well, disabled abilities are removed from players, so none of that player’s units can use the ability. However, other players can still use it. Solution, remove it from all players. But then, you may wonder why disabled abilities are useful. Well, first of all, disabled abilities lose their icons. It’s true that you are no longer allowed to use them (so activateable spells become useless) but some passive abilities still retain their bonuses.


To disable an ability, you need the trigger “Player – Disable/Enable Ability for Player”. Keep in mind that not all disabled passives retain their effect. For spellmaking, abilities are disabled usually to hide their icon, and so, it is used for passives. The trigger to disable abilities for this purpose is this:

Disable Ability
Events
Map initialization

Conditions

Actions
Player - Disable Ability for Player 1 (Red)

Player - Disable Ability for Player 2 (Blue)

Player - Disable Ability for Player 3 (Teal)

Player - Disable Ability for Player 4 (Purple)

Player - Disable Ability for Player 5 (Yellow)

Player - Disable Ability for Player 6 (Orange)

Player - Disable Ability for Player 7 (Green)

Player - Disable Ability for Player 8 (Pink)

Player - Disable Ability for Player 9 (Gray)

Player - Disable Ability for Player 10 (Light Blue)

Player - Disable Ability for Player 11 (Dark Green)

Player - Disable Ability for Player 12 (Brown)

Notice that I didn’t disable the ability from neutral units. That is because players cannot control neutral units, only if the units belong to them, case in which the ability is already disabled.

Note


Some abilities which can both be activated and have passive effects when disabled retain their passive effect.


Try the spell and, bad luck! When you try to, the immolation no longer works. You may wonder why is that. Permanent Immolation is between those abilities which once disabled, lose any passive effect they might’ve had, if enabled. You may say that there is nothing we can do, but in truth there is.


Due to patch 1.18, a great asset has been inserted into World Editor. Go to the object editor, and search between the item abilities for an ability called Spellbook. The ability itself is supposed to store more spells, and when clicked, it shows all those abilities. If they are active, you can activate one of them, and if they are passive they keep their passive effect all the time, as long as you keep the item with this ability. The secret of this ability is that the unit which has this ability, may benefit of the passives effects of the spells it contains, even if the Spellbook ability itself is disabled, hiding along with it all the abilities it contains. So whenever you are having problems with passive abilities that won’t work when disabled, add them to a spellbook, and give it to the hero, hiding it of course.

Note


Add only the passive abilities you plan to use for a single trigger enchanced spell to the spellbook. Do not combine more passives for more spells into the same spellbook.


So in this case, just add the Permanent Immolation (Elemental Essence) to a custom spellbook (don’t forget to remove all other abilities and set minimum and maximum abilities to number of abilities the spellbook contains, in this case 1), and change in the triggers the Permanent Immolation with the spellbook. Surprise, the spell now works properly!


There is one more thing I want to warn you about spellbooks! If you give more than a spellbook ability to an unit, you can screw things up badly. I remember that I tried once and got some messy bugs with the abilities. Try avoiding spellbooks unless necessary, or unless you feel they can’t interact with other spellbooks.


Another use of the spellbook would be to pack more bonuses into a single ability. Let’s say that you make an ability which adds armor, damage, and other stuff. You add them all to a spellbook, and then all you will have to do, is add the spellbook to the unit affected by the spell, instead of adding all those abilities. Same for removal! So spellbook can also be used as a pack, to get more abilities into it, and compress things!


And with this, I end the passive spells chapter as well!

#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

XV. Buff Spells


Buff spells are those custom abilities which have a temporary effect upon an unit. They are usually followed by a small effect above or around the unit which lasts as long as the spell lasts, so that the player knows that a certain unit is touched by such a spell, and more specific, by a certain buff spell. Buff spells include Slow, Inner Fire, Banish, Divine Shield, Thunder Clap, Polymorph, Invisibility, Hex, Wind Walking, Earthquake, Spirit Link, Berserk, Purge, Bloodlust, Lightning Shield, Envenomed Spears, Entangling Roots, Shadow Strike, Roar, Rejuvenation, Cyclone, Faerie Fire, Sleep, Curse, Anti-Magic Shell, Unholy Frenzy, Cripple, Howl of Terror, Soul Burn, Doom, Silence and others.


“Ok ok, but don’t some of the previous spells we made involve buffs?” some of you may ask. Well, of course they do, because if you want spells not to be dull, you will use buffs, atleast in most of the cases. But you didn’t trigger those buff abilities yourself, but just used them. My triggering a buff spell, it means doing the effect of that spell itself (aka Slow). You will need to add/remove things from the target (such as Movement Speed and Attack Speed for slow) whenever the buff appears/disappears.


Think at buff as being an attribute of a spell. A classification was by their type of targets. The spells were either AoE, Passive, Single Target, SelfCast (Single Target but with the caster as the target) or Wave/Cone (which will not be included into this tutorial since they are too advanced). A spell may not be both AoE and Single Target (unless using combined spells which result in hybrids), but both AoE and Single Target spells can be buffed. Like channeling (which will study in the next chapter), buff can apply to almost all types of abilities (except defensive passives). I hope that this clears the concept of a buff spell! We will now make a buff spell ourselves.


Now, like passive abilities, buff spells are also classified as bonus buffs, and event buffs. Bonus buffs simply add/remove things to/from the target, such as armor, damage and movement speed. However, there are also event buffs, which make something happen when something happens to the buff unit. For example, consider a buff mana flare, which causes the unit to receive damage everytime it casts an ability, as long as the unit has the buff. This is an example of event buff spell.


And so, we are going to make three buff spells. Why three you may say because there are only two types of buff spells. I said only three, because the trigger-enchancing technique for bonus buff spells is different for AoE spells from Single-Target spells. However, for event buffs, it is the same.


The first spell we will do was already made by The_Raven, but I think it would be a good idea to teach you how to do it, since you are now capable enough to be able to do it. The spell is called Seduce, and it gives temporary control upon a unit (buff charm). The spell will give control upon an unit for 10/20/30 seconds, unless dispelled. Cooldown will also be equal with 10/20/30 seconds so that there are no multiple instance problems (we will variables this time). The spell will not work on heroes or creeps with level greater than 5.


So, use as a dummy unit a single target buff ability (for example Unholy Frenzy), which lasts 10/20/30 seconds, has cooldown of 10/20/30 seconds and does not increase attack rate or damage target. You know that it must be a three level hero ability. The rest of the fields are up to you.


Now let’s get to the coding. We are going to need an unit variable, so create one. I named it SeduceUnit. Also, you will need a player variable. I named it SeducePlayer. And now, let’s proceed to event/conditions/actions.


Event and condition is classic. The validation of the spell will be separate.

Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Seduce

Now, we know that the spell is supposed to give the target to the owner of the casting unit, until the buff disappears, when it returns to the previous owner. So the only way the computer will know who the previous owner was, is by remembering that player into a variable. The Seduce Player. The action is this

 Code:

set SeducePlayer = (Owner of (Target Unit of Ability Being Cast))


There is also another problem. We will obviously need to use a Wait for Condition, waiting until the target doesn’t have the buff anymore. But we know that after the slightest wait, (Target Unit of Ability Being Cast) is set to (No Unit). The only way to solve this problem is by keeping the target into a variable before any wait.

 Code:

set SeduceUnit = (Target Unit of Ability Being Cast)


The next action will “rally” the target. The Change Color will change the color of the target to the color of the owner of the caster. If the unit retains its color, it will change ownership but color will remain the same, causing confusions upon the battlefield.

 Code:

 Unit - Change ownership of (Target unit of ability being cast) to (Owner of (Casting unit)) and Change color


And now, normally the wait for condition action would come. However, there is another thing you must know about buffs. They don’t appear on the unit exactly when the caster starts the effect of an ability. The delay is of about 0.10 seconds. This means that if you add the “wait for condition” action right when the caster starts the effect of the ability, then the condition will be true and nothing will happen. You will have to insert a wait of 0.10 just before it, to assure that the buff shows up. This is something you must remember! It’s a very common mistake, I also used to do it a lot, before I realized that this was the problem!!!

 Code:

Wait 0.10 seconds
Wait until ((SeduceUnit has buff Seduce) Equal to False), checking every 0.10 seconds


After this, we just change the ownership back and clear the variables.

 Code:

 Unit - Change ownership of SeduceUnit to SeducePlayer and Change color
Set SeduceUnit = No Unit
Set SeducePlayer = (Matching Player)

Note


I gave the SeducePlayer the value of (Matching Player) because in this case, (Matching Player) returns nothing, and so, the variable is nullified. In JASS, you can give it the value of null, but until you start learning it, I suggest you either leave them as they are, or you give them a value you know it’s nothing (value returned by a function).


If you check the spell it should work! Keep in mind that it is not MUI because you used global variables.


And now, let’s make the validation. The unit can still be cast upon creeps with level higher than 5. So you will need another trigger for this ability, for validation, which will stop the caster immediately if he is attempting to cast the ability at an invalid target. In this case, we will use the “An Unit Begins Casting an Ability” trigger, so that the caster doesn’t fire for good the spell and can still be stopped. As a condition use the same as above.


A second condition should check if the level of the target is higher than 5, and if it is, only then the actions will fire. The condition is this:

 Code:

 (Level of (Target unit of ability being cast)) Greater than 5


And now, all you have to do is order the caster to stop.

 Code:

Unit - Order (Triggering Unit) to Stop


Moreover, you can add a game text if you want. Color it in white if you are having issues, and voila, spell is valid. The single target seduce is finished!


And now comes the challenge, how to make seduce target more units in an area? Some of you might say that we can give the single target seduce to a dummy unit and order it to cast it at all the units in an AoE, in the classical way I presented in chapter XIII. But since the spell is not MUI we have two possibilities: attempt to make the first ability MUI or do some tricky coding. I will show you the second method, and then you can try to adapt it to the first if you want.


First of all, make a new dummy ability, and make it AoE. I have used a Silence which doesn’t disable anything.


And now, the buff spell affects units until the buff disappears. We need a system which tracks units who were affected by the spell, and their buffs disappeared, but the effect of the spell still exists. Sounds easy, but it isn’t like that. For this spell, you are going to need a group variable. I called it SeduceGroup.


And now, coding will be similar yet interesting. The idea is like this: when an unit is affected by a buff spell, you give it the buff bonuses, and you add it to the group. Then, you will need a second trigger which detects every unit in the group which doesn’t have the buff, and remove the bonuses of the spell.


Another problem this spell might cause is the fact that even though you will no longer need the SeduceUnit, you can’t use the second variable either to track multiple players. You might think that arrays are a solution but for arrays, you can only access each value by the index. And in this case, you won’t know for each unit which index corresponds. So the solution is the Custom Value, something new for almost all of you. The Custom Value is an integer value which is linked to the unit. Linked means that you can access the value through the unit. So to get to the value, you only need the unit. Thing which is perfectly fine with the spell!


But some of you may still be confused. “How can integers be related to players?” Well, each player has an index (Player 1, Player 2, etc). Even neutral players have an index (integer value). You can get that value through (Index of Player) function which that’s what it returns, the integer value assigned to the player.


And now, that you know everything you have to know, here is the code (you should practically understand everything). For variables, I use once again SeduceGroup, as a group variable, and targ as a point.

Seduce Main
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Seduce

Actions
Set targ = (Target point of ability being cast)

Wait 0.10 seconds

Unit Group - Pick every unit in (Units within 600.00 of targ matching ((((Matching unit) is A structure) Equal to False) and ((((Matching unit) is Mechanical) Equal to False) and ((((Matching unit) is dead) Equal to False) and (((Matching unit) belongs to an enemy of (Owner of (Triggering Unit))) equal to true and do (Actions)
Loop - Actions
Unit Group - Add (Picked unit) to SeduceGroup

Unit - Set the custom value of (Picked unit) to (Player number of (Owner of (Picked unit)))

Unit - Change ownership of (Picked unit) to (Owner of (Triggering unit)) and Change color

Trigger 2

Untitled Trigger 002
Events
Time - Every 0.10 seconds of game time

Conditions
(SeduceGroup is empty) Equal to False

Actions
Unit Group - Pick every unit in SeduceGroup and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked unit) has buff Seduce) Equal to False

Then - Actions
Unit Group - Remove (Picked unit) from SeduceGroup

Unit - Change ownership of (Picked unit) to (Player((Custom value of (Picked unit)))) and Change color

Else - Actions

And surprise, the spell is MUI as well, almost completely, if two units don’t cast the ability, at the very exact time, with a delay of atleast 0.15 seconds between them, so the spell is 99% MUI. It seems that global groups can help in such cases. And now, two exercises for you.

Exercise


1. Make Seduce (Single Target) Multiinstance


2. Modify Seduce so that instead of changing the ownership of the unit, it makes the unit have a 15/25/35% chance to do half damage everytime it attacks. Make sure that only one custom ability is given to the unit at a time, if two seduce spells of different levels are cast at about the same time.


And now we will move to event buffs, which are much simpler and can be made MUI in almost all cases. First we will take a spell, and then we will study how to do it. I told you before about a Mana Flare Buff spell. Well, we will make a similar spell, but in our case the unit will lose additional mana, instead of life. I won’t complicate the spell so much to make the unit lose more mana each level. For now…


In this case, you can make it either single target or multiple target. It doesn’t really matter. Only keep in mind that in this case the custom buff is really important. I will call it Mind Rot, and you should do the same. You need no other additional abilities. Do whatever you want to do with the Mind Rot spell, just make sure that it is a correct dummy buff spell, and that it has the Mind Rot custom buff.


And now, let’s start analyzing the coding itself. In the case of the event buffs, the trigger works for units who have the buff, not when the spell is cast. It doesn’t matter when the buff appears, or when it disappears, it matters if a certain event takes place upon an unit with the buff. As an example, take Spirit Link. Everytime the unit is damaged, it is immediately healed, and damaging other linked units for the same amount of life, as it was healed, and giving the impression that they share the life. In this case, the event is clear “An Unit is Attacked”, and the condition is “((Attacked Unit) has SpiritLink Buff equal to true)”. It doesn’t matter when the unit received the buff, but it matters if an unit with the buff is attacked.


In our case, we need to track everytime an unit with the Mind Rot buff casts an ability, regardless which it is. Both the event and the condition should be familiar to you.

Mind Rot Simple
Events
Unit - A unit Starts the effect of an ability

Conditions
((Triggering unit) has buff Mind Rot) Equal to True

Actions
Unit - Set mana of (Triggering unit) to ((Mana of (Triggering unit)) - 10.00)

That was really simple, wasn’t it? Well, now let’s complicate the spell, and say that I want the spell to be three levels. In this case, we will need more than just one trigger, and we will need three unit groups. But hey, since all of them are unit groups why don’t we make a single variable, but array. Call it MindRotGroup and make sure that its length is atleast equal to 4.


And now, the idea is a little more complicated, so that we can have multiple levels. When the spell is cast, depending on the level of the Mind Rot for the caster, we add the target to one of the groups. We will also need to remove it from the other groups, since the buffs don’t stack.

Mind Rot
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Mind Rot

Actions
Unit Group - Add (Target unit of ability being cast) to MindRotGroup[(Level of Mind Rot for (Triggering unit))]

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Mind Rot for (Triggering unit)) Equal to 1

Then - Actions
Unit Group - Remove (Target unit of ability being cast) from MindRotGroup[2]

Unit Group - Remove (Target unit of ability being cast) from MindRotGroup[3]

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Mind Rot for (Triggering unit)) Equal to 2

Then - Actions
Unit Group - Remove (Target unit of ability being cast) from MindRotGroup[1]

Unit Group - Remove (Target unit of ability being cast) from MindRotGroup[3]

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Mind Rot for (Triggering unit)) Equal to 3

Then - Actions
Unit Group - Remove (Target unit of ability being cast) from MindRotGroup[1]

Unit Group - Remove (Target unit of ability being cast) from MindRotGroup[2]

Else – Actions

And now, when an unit with the buff casts the ability, we will need to check in which group it is, and with this, we will know what level does the spell affecting the unit have.

Mind Rot Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
((Triggering unit) has buff MindRot) Equal to True

Actions
If (((Triggering unit) is in MindRotGroup[1]) Equal to True) then do (Unit - Set mana of (Triggering unit) to ((Mana of (Triggering unit)) - 10.00)) else do (Do nothing)

If (((Triggering unit) is in MindRotGroup[2]) Equal to True) then do (Unit - Set mana of (Triggering unit) to ((Mana of (Triggering unit)) - 20.00)) else do (Do nothing)

If (((Triggering unit) is in MindRotGroup[3]) Equal to True) then do (Unit - Set mana of (Triggering unit) to ((Mana of (Triggering unit)) - 30.00)) else do (Do nothing)

And now last but not least, we need constantly check whenever there is an unit in either of the three groups which doesn’t have the buff, and if it is you remove all units without the buff from each group. Here is the code.

Mind Rot Check
Events
Time - Every 0.50 seconds of game time

Conditions
Or - Any (Conditions) are true
Conditions
(Number of units in MindRotGroup[1]) Equal to 0

(Number of units in MindRotGroup[2]) Equal to 0

(Number of units in MindRotGroup[3]) Equal to 0

Actions
Unit Group - Pick every unit in MindRotGroup[1] and do (Actions)
Loop - Actions
If (((Picked unit) has buff Mind Rot) Equal to True) then do (Unit Group - Remove (Picked unit) from MindRotGroup[1]) else do (Do nothing)

Unit Group - Pick every unit in MindRotGroup[2] and do (Actions)
Loop - Actions
If (((Picked unit) has buff Mind Rot) Equal to True) then do (Unit Group - Remove (Picked unit) from MindRotGroup[2]) else do (Do nothing)

Unit Group - Pick every unit in MindRotGroup[3] and do (Actions)
Loop - Actions
If (((Picked unit) has buff Mind Rot) Equal to True) then do (Unit Group - Remove (Picked unit) from MindRotGroup[3]) else do (Do nothing)

And now, your spell works for 3 levels and is MUI. Phew!


Now, imagine another situation. What if you want to actually damage the unit everytime it casts an ability and you want bounty to be given to the hero who cast the buff, if it dies due to that damage? I’ll give you the idea and you can experiment.


The idea is to use an array for an unit group, with 13 positions (since positions start from 0 in arrays). And now, when the ability is cast, you add the unit to the unit group with the index of the array which is equal with the player’s number, to whom belongs the caster. Then you have to remove it from all the other groups.


Moreover, think that you have to check for the second trigger to which group the unit belongs, and create a dummy unit depending on the group to damage the target.


And last but not least, you need to check all the 12 groups every 1.00 seconds for units who don’t have the buff anymore and remove them. Without any further comments, here is the code (though I said I wouldn’t give it to you):

Mana Flare
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Mana Flare

Actions
Unit Group - Add (Target unit of ability being cast) to ManaFlareGroup[(Player number of (Owner of (Triggering unit)))]

For each (Integer FlareLoop1) from 1 to 12, do (Actions)
Loop - Actions
If ((Player number of (Owner of (Triggering unit))) Not equal to FlareLoop1) then do (Unit Group - Remove (Target unit of ability being cast) from ManaFlareGroup[FlareLoop1]) else do (Do nothing)

Mana Flare Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
((Triggering unit) has buff Mana Flare) Equal to True

Actions
For each (Integer FlareLoop2) from 1 to 12, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is in ManaFlareGroup[FlareLoop2]) Equal to True

Then - Actions
Unit - Create 1 Dummy for (Player(FlareLoop2)) at (Position of (Triggering unit)) facing Default building facing degrees

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Unit - Order (Last created unit) to damage (Triggering unit) for 10.00 using attack type Spells and damage type Cold.

Else - Actions

Mana Flare Removal
Events
Time - Every 0.50 seconds of game time

Conditions

Actions
For each (Integer FlareLoop3) from 1 to 12, do (Actions)
Loop - Actions
Unit Group - Pick every unit in ManaFlareGroup[FlareLoop3] and do (Actions)
Loop - Actions
If (((Picked unit) has buff Anti-magic Shell) Equal to False) then do (Unit Group - Remove (Picked unit) from ManaFlareGroup[FlareLoop3]) else do (Do nothing)

Happy end, this spell works as well. I used Fors to avoid too much coding. FlareLoop1, FlareLoop2 and FlareLoop3 are all integer variables. I used them to avoid repeating (Integer A) or (Integer B) because it could cause problem to the instanceability of the spell.

Exercise


Make the Mana Flare buff spell a three levels spell, and make it give bounty like above. I’m being a jerk, ain’t I?


Advice: you will need three group arrays, one for each level. I suggest you combine the two codes.


XVI. Channeling Spells


Just like buff spells, channeling is an attribute of spells. Channeling spells force the caster to stay in place and practically maintain the spell. If the caster moves, then the effect of the spell stops at once. Examples of channeling spells include Aerial Shackles, Cloud, Flame Strike, Mass Teleport, Blizzard, Siphon Mana, Life Drain, Rain of Fire, Tornado, Volcano, Big Bad Voodoo, Earthquake, Death and Decay, Starfall, Tranquility and others.


As you can see, there are both single target and AoE channeling spells. There are also strange combinations which include both buff and channeling spells (such as Cloud). Keep in mind that the channeling property of a spell gives it a disadvantage, because it forces the caster to do nothing else but maintain the effect of the spell, thing which can make him remain very vulnerable. Also, there are methods to forcefully break dangerous channeling spells such as Starfall. These techniques include spells like Sleep, Hex, and Stunning spells (Storm Bolt, War Stomp, Cluster Rockets etc). So think at channeling as a way to balance a spell.


Also, to make a channeling spell non-channeling, just change its duration to 0.01 and it will make the spell last for a very small period of time, too small to be noticed.


And now, let’s get busy doing a channeling spell. First, let’s see the idea of the spell. Hmm, something classic… I know! Let’s do this spell, called Apocalypse! It casts random flame strikes around the caster every 1.50 seconds, and lasts 45 seconds. And I won’t bother you a lot this time, and just do it a single level spell!


So, we will need a dummy unit, an AoE, channeling spell with no target (I’d say Starfall or Tranquility) and a Flame Strike spell for the dummy unit. I’ll let you handle all these!


Let’s get to the trigger. The steps of a channeling spell are extremely simple. First step is detection when the ability is cast, it will activate the second step. The second step is the effect itself of the spell, which will usually be a repetitive one. It can already be done, and so all you have to do is use the already made spell. The third step is the end of the step. It stops the effect, when the caster stops/finishes channeling.


So in theory you need three triggers. The second step may be complex, so sometimes you might need more than just a trigger. And when I said that the second step may be skipped, I meant that for example you can make the Cloud spell, be a non-targetable AoE, by ordering a dummy unit to cast it at the position of the caster, as long as the caster channels. But that’s something too simple!


So, let’s get to our trigger. The first step, is the activation of the trigger. It will probably be the same for every channeling spell. It may vary in some cases, but the principle is the same. With no further comments upon the event/conditions, I’m presenting them to you:

Apocalypse Initialization
Events
Unit - A unit Begins channeling an ability

Conditions
(Ability being cast) Equal to Apocalypse

Pretty simple by now. And now, here comes the problem. We can’t make the spell MUI because you will need to store the caster into a variable (or in the case in which you don’t use a trigger for the second step, for the dummy unit). So yes, we will need an unit variable, which I called ApocCaster. The first action will store the caster in the variable, and the second will activate the trigger for the second trigger, “Apocalypse Effect”.

 Code:

Actions
Set ApocCaster = (Triggering Unit)
Trigger - Turn on Apocalypse Effect


And now, we are getting to step two. After the initialization has been made, by turning on the effect, let’s get to the second trigger. As some of you might've guessed, it must be initially off, so do this! Now, the effect must take place every 1.00 seconds. Well, this means that the event will be a repeating timer, without any conditions, because if nobody channels the spell, you will see that this trigger is turned off.

Apocalypse Effect
Events
Time - Every 1.00 seconds of game time

Conditions

Now, the actions are also pretty simple. We will have a dummy unit cast Flame Strike at a random point around the caster. The random point is obtained by using the Polar Offset, with random angle and random distance from the caster. I decided that an area of 600 is big enough for the spell. And I made the distance of the flame strike only between 300 and 600 so that the caster isn’t affected by the flame strikes.

Actions
Unit - Create 1 Dummy for (Owner of ApocCaster) at (Position of ApocCaster) facing 0.00 degrees

Unit - Add Banish to (Last created unit)

Unit - Order (Last created unit) to Human Blood Mage - Flame Strike ((Position of ApocCaster) offset by (Random real number between 300.00 and 600.00) towards (Random real number between 0.00 and 360.00) degrees)

Unit - Add a 3.00 second Generic expiration timer to (Last created unit)

Now, that wasn’t hard at all. This is the effect. It will keep pumping dummy units to cast Flame Strike before the trigger is disabled once again. I added such a big expiration timer so that the dummy unit has enough time to cast the Flame Strike. Flame Strike requires about 1.50~2.00 seconds to be cast. I put 3.00 to make sure that the dummy unit is not removed too fast. You may experiment with lower values too, but make sure that the Flame Strike is fired, and only then the dummy unit is removed.


And now, let’s disable the spell. Again, the trigger is basic.

Apocalypse Halt
Events
Unit - A unit Stops casting an ability

Conditions
(Ability being cast) Equal to Apocalypse

Actions
Trigger - Turn off Apocalypse Effect

Set ApocCaster = No Unit

Have fun testing the spell! It will work! That’s all the philosophy about channeling spells, basic of course.


There is something else however you have to know. There are certain spells which don’t require channeling but have a similar effect. An example of such a spell is Locust Swarm, which even though it spawns locusts, the caster can move. In our case, if you change Flame Strike adequately, by changing casting time to 0.50 seconds and the green circles while the spell is being cast with Inferno meteor, and you change duration to 2.00, you can obtain a real nice Armageddon spell, like in Diablo II. After you’ve changed the Flame Strike, we have to change the triggers a little. Oh, don’t forget to change the channeling spell to last 0.01 so that you can get it non-channeling. Or you can base the Armageddon off Fan of Knives, and make it a dummy ability.


The first trigger needs a change of Event. It should be “An Unit Starts The Effect of an Ability”. Moreover, you will need a timer, which should be a one shot and last as much as you want the spell to last. Don’t forget to assign it to a variable (timer variable). The second trigger can remain untouched.


The third trigger needs both event and conditions changed. Well, normally the spell should stop when the timer expires. That’s one event. However, if the caster dies, the second trigger will continue to fire until the spell expires. You also need the check if the caster dies. Also, last but not least, you nullify the variable which contained the timer.

Armageddon Halt
Events
Time - ArmageddonTimer expires

Unit - A unit Dies

Conditions
ArmagCaster Equal to (Dying Unit)

Actions
Trigger - Turn off Armageddon Effect

Set ApocCaster = No Unit

Congratulations! You’ve made the armageddon spell from Diablo 2!


XVII. Selfcast Spells


Selfcast spells are spells which affect only the caster, but are activable (and some of them even deactivable), and give certain bonuses to the caster himself. Example of Selfcast spells include Divine Shield, Berserk, Wind Walking, Bladestorm, Immolation, Defend, Mana Shield, Chemical Rage, and many others.


Again, there are more categories of Selfacst spells. There are some which give bonuses, and are activated until their effect disappears, such as Berserk, Wind Walking and Bladestorm. They act like a buff spell, which affects only the caster. However, there are spells which affect the caster until stopped. These spells are called toggleable abilities and usually have some negative effects as well. They either drain mana constantly like Immolation, slow movement speed like Defend, or keep draining mana until you are off and then the spell stops (Mana Shield).


In this tutorial, I will teach you how to make a basic spell: Barrier. I’ve made it before, and I think it is the perfect example for a toggleable ability. What it does is add 3/5/7 armor to the caster and give 25% magic resistance. However, the ability itself drains 6 mana each second.


So, let’s see what abilities you need. There aren’t many toggleable abilities and you may want to use more than one for the unit. In conclusion, you can’t always rely on Immolation to save you if you eventually want to drain mana from the target every second. In this case, we will use defend. Make it a complete dummy ability. Also, have a magic resistance skill, a three levels item armor bonus ability. I called them BarrierResistance and BarierArmor.


And now, let’s see the trigger. The idea of this spell is pretty simple. Add bonuses when activated, and remove them when deactivated. Moreover, you would need a third trigger to eventually deactivate the spell if the caster doesn’t have anymore mana. Another trigger must drain mana from the caster constantly. Also, you will need to add further bonuses to the caster, if it upgrades the ability, or remove any bonuses when the caster uses a tome of retraining. Quite some work, so let’s get started.


Firstly, there is something more I have to explain about these abilities. They are not like any others, because when activated, it isn’t considered as if you are starting the effect of an ability or even channeling a spell. The only way you can detect them is by their order ID. In this case, the activation order string is “defend” and the deactivation order is “undefend”. You can see the Ids in the object editor for each ability.


Well, now let’s start with the first trigger. The event to detect orders is “An Unit is Issued an Order” and it activates just when the unit either prepares to issue that order, such as auto-aiming units for attacking. If you for example order an unit to cast an ability, the trigger will activate just when the unit prepares to get to that unit, if it is out of range. So it is an even earlier event than “An Unit Begins Casting an Ability”. But in this case, there’s no way you can stop the toggleable ability from taking place, abusing from it, because it is turned on/off instantly.


Condition much compare two orders. The first field will be equal with (Issued Order), while the second one will be an Order to String conversion, and the string will be equal with defend. Moreover, you will have to check if the unit has barrier, because we don’t want this trigger to affect units who cast the classical defend ability. Conclusion, the event and condition would look like this:

Start Barrier
Events
Unit - A unit Is issued an order with no target

Conditions
(Issued order) Equal to (Order(defend))

(Level of Barrier for (Triggering unit)) Greater than 0

And now, the actions. Again, to make this spell MUI I will use an unit group called BarrierGroup. Since we use same mana drain for each level, it won’t be a problem. So, we will practically drain mana from each unit to the group, and also do the tome of retraining stuff depending on the units in group. There are a lots of things around this group, as you will notice. By now, just add the unit to the group and add all the bonus abilities it needs.

Actions
Unit Group - Add (Triggering unit) to BarrierGroup

Unit - Add BarrierArmor to (Triggering unit)

Unit - Set Level of BarrierArmor for (Triggering unit) to (Level of Barrier for (Triggering unit))

Unit - Add BarrierResistance to (Triggering unit)

The second step will be to add the mana drain. For this, we will need a trigger which takes every unit in the group every second, and reduces their mana by 6.

Barrier Drain
Events
Time - Every 1.00 seconds of game time

Conditions
(Number of units BarrierGroup) Greater than 0

Actions
Unit Group - Pick every unit in BarrierGroup and do (Unit - Set mana of (Picked unit) to ((Mana of (Picked unit)) - 6.00))

Now, the third trigger will be the protection, which orders the unit to disable the ability, forcefully if it doesn’t have enough mana. It will automatically make the trigger which removes all the bonuses activate, because the unit is actually the undefend order.

Barrier Protection
Events
Time - Every 0.10 seconds of game time

Conditions
(Number of units in BarrierGroup) Greater than 0

Actions
Unit Group - Pick every unit in BarrierGroup and do (Actions)
Loop - Actions
If ((Mana of (Picked unit)) Less than 6.00) then do (Unit - Order (Picked unit) to Human Footman - Stop Defend) else do (Do nothing)

And now, for the fourth trigger, it just removes all the bonuses from units which undefend and have barrier

Barrier Stop
Events
Unit - A unit Is issued an order with no target

Conditions
(Issued order) Equal to (Order(undefend))

Actions
Unit Group - Remove (Triggering unit) from BarrierGroup

Unit - Remove BarrierArmor from (Triggering unit)

Unit - Remove BarrierResistance from (Triggering unit)

By now, the spell works almost perfectly. However, we still have the upgrade and unlearn issues. Let’s solve with the unlearn first.

Barrier Unlearn
Events
Unit - A unit Uses an item

Conditions
(Item-type of (Item being manipulated)) Equal to Tome of Retraining

Actions
Unit Group - Remove (Triggering unit) from BarrierGroup

Unit - Remove BarrierArmor from (Triggering unit)

Unit - Remove BarrierResistance from (Triggering unit)

And now, the last trigger will just make sure that whenever the unit upgrades the ability while it is enabled, it also receives the armor bonus immediately. Here is the code:

Barrier Upgrade
Events
Unit - A unit Learns a skill

Conditions
(Learned Hero Skill) Equal to Barrier

((Triggering unit) is in BarrierGroup) Equal to True

Actions
Unit - Set Level of BarrierArmor for (Triggering unit) to (Level of Barrier for (Triggering unit))

And now all you have to do is test your ability, and enjoy it!


This is the last chapter in which I will be explaining how to make spells. From now on, you are ready to learn by your own and master the art of spellmaking. There are some things I still have to teach you, but now you know the basics, and maybe a little more.


XVIII. Efficiency


A last thing you have to learn about abilities is how to make them efficient. The first step is to make them work, but once that is done, you will want to optimize it, so it doesn’t lag, it doesn’t consume too much memory, and it doesn’t have any similar problems. Here are some simple advises:


1) Always try to nullify variables (example: set units to (No Unit)). Each variable which points at an object takes memory, and you can free that memory by making them to point to nothing. Even by the fact that they exist, variables consume memory, but that cannot be avoided. However, in GUI not all variables can be nullified, properly, because the function of setting it to null or destroying it doesn’t exist for all. All you can do is set those which you can nullify to nothing, and sometimes you can even trick the editor by giving the variable the value of a function, which would return nothing (for example, give units (Matching Unit) when you don’t use a “Matching Conditions” and you don’t use it in the condition).


2) Check the FAQ in the Spells section to find out more about memory leaks. Though your spells may seem as if they don’t leak, because you nullified all variables, there are some auxiliary functions which leak, such as (Position of unit), Polar Projection and Pick Up Every Unit. In the FAQ, there is a link to a tutorial by Cubasis which explains in an excellent way how to avoid leaks.


3) Try to make the spell do a minimum number of actions. Keep in mind that by adding a for the computer doesn’t have to execute less actions. It just makes the code look shorter, but the computer actually repeats the actions in the four. Also, sometimes formulas can help you out, instead of doing branching and other methods.


4) Always add a function inside Map Initialization trigger to make them destroy themselves. These triggers are executed only once, and then they just take memory. By destroying them, you free that memory.


5) Keep your codes clean. Write comments, give suggestive names to triggers and variables, make it easy to read. You are not the only one who will read the code, especially if you make the spell public, either by putting it into a map or by releasing it as a spell. And maybe not even you will know what you did, after a week in which you didn’t look over the code. You forget codes faster than you can think.


6) Leave your maps unprotected so that people can learn from your codes. It is a good thing to keep your codes safe from stealing, but it is even better to know that other people can learn from your creation.


XIX. Final Note


I know this tutorial may have some problems, and that there might be a lot of things to mention, but keep in mind that spellmaking is a very ample subject. I did my best to give you a… brief introduction into spellmaking. From now, all you have to do is be creative, practice, and get better. Once you feel like you can do almost every spell (limited by the editor of course), you can attempt to learn JASS, but not before! This doesn’t include MUI for all the spells of course, because this is the main reason people use JASS. I suggest atleast a 4 months experience of spellmaking, in which you worked intense in this domain.


Oh, and I almost forgot... There are other sources from which you can learn: other people's spells. The idea is to look over their code for learning, not just to steal their work. In the spells section you can search only for trigger enchanced spells... Some are better than others, you may even notice that some people made mistakes, and if you notice that, it means you are better than them in some areas. However, you may also learn new things from those people. There is always something more to learn...


I am waiting for your further suggestions. Have fun making cool spells!


~Daelin

#### Deadly Nightshade

#### Deadly Nightshade

 Level 4

JoinedJul 29, 2014Messages65

I'm on the job (for learning this)!

#### Sclammerz

#### Sclammerz

Patron

 Level 25

JoinedApr 12, 2014Messages1,893

Resources: 4

#### Resources
Skins 1Maps 3

##
 Legacy of the Third War


Nice!

#### Eat_Bacon_Daily

#### Eat_Bacon_Daily

 Level 13

JoinedJan 4, 2014Messages558

Resources: 1

#### Resources
Maps 1

Holy shit Excellent tutorial

#### SparrrrroW

#### SparrrrroW

 Level 1

JoinedAug 1, 2016Messages4

i couldnt complete it :| that step

Final

If (((Target unit of ability being cast) is A structure) Equal to True) then do (Unit - Order (Last created unit) to damage (Target unit of ability being cast) for (25.00 + (25.00 x (Real((Level of Shock Destructor for (Triggering unit)))))) using attack type Magic and damage type Fire.) else do (Do nothing)

there was no dmg thing (Unit - Order (Last created unit) to attack (Target unit of ability being cast) else do (Do nothing)

i am not getting how u got formula and that action

#### Hive_Parasite

#### Hive_Parasite

 Level 8

JoinedAug 19, 2010Messages172

Resources: 2

#### Resources
Maps 2

Awesome guide,maybe you can help me out....I am making a triger based spell,based on Storm Bolt used to damage enemies,stun them and put on them a variant of the Permanent Immolation ability which is set to damage allies.

My only problem is that Permanent Immolation (the one I used is neutral hostile 2) only damages up to 2 targets,which is wierd and I need to double the ammount of targets permanent Immolation can damage.

#### Silva

#### Silva

 Level 6

JoinedFeb 6, 2015Messages266

This was a fun ride. Thanks a bunch. However, can you solve the exercises? I managed to pull off something similar to the 2nd exercise, but I don't think it's the right solution.

What I did is basically this: once a debuffed unit attacks, there's a chance to create a dummy unit at the location of the attacked unit, and cast Spirit Link on it, distributing the damage between the two at 50%.

#### Xerxes

#### Xerxes

 Level 20

JoinedApr 14, 2012Messages2,906

Resources: 8

#### Resources
Icons 4Maps 1Spells and Systems 3

Great refresher for someone like me who hasn't touched triggers in almost a decade xD

#### Shyvv

#### Shyvv

 Level 3

JoinedAug 1, 2017Messages9

I started out when the editor was first released but didn't have the time to really dig in and learn everything.

Now 10ish...15ish? idk... many years later I decided to show my now soon-to-be wife how it works and show her "a piece of my childhood" but I'm still sloppy

so I GREATLY appreciate the time and energy you put into this comprehensive tutorial, and I'll be coming back to it from time to time because I've never used dummy spells or dummy units in my abilities before. I've done a little bit with variables but mostly I just changed a few settings or made simple (but leaky) triggers.

Now that I've done the research (with the help of posters like you) I'm feeling really confident about the project I'm working on.

Again, Many thanks!

 You must log in or register to reply here.


### Similar threads

 Introduction To Trigger-Enhancing Spells


whitenights

Jul 27, 2004

Trigger (GUI) Editor Tutorials

Replies6Views36K

Jun 8, 2016
TheLordOfChaos201

 Creating some basic abilities


paskovich

Apr 22, 2007

Trigger (GUI) Editor Tutorials
23

Replies113Views104K

May 26, 2021
McPhisto2051

 Basic Triggering


Ralle

Jan 3, 2006

Trigger (GUI) Editor Tutorials

Replies4Views39K

Jul 7, 2016
billel

 Making any Spell Autocasting


Ralle

Mar 15, 2006

Trigger (GUI) Editor Tutorials

Replies12Views59K

Dec 22, 2017
Rogue Kessler

 MUI Spells Using Artificial Waits


Tank-Commander

Sep 28, 2012

Trigger (GUI) Editor Tutorials

Replies32Views20K

Oct 16, 2015
BETABABY

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