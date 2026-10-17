---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/modding-the-we-trigger-editor.52529/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Modding the WE Trigger Editor
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/modding-the-we-trigger-editor.52529/>
#### Shvegait

S

#### Shvegait

NOTE: I didn't think to post this here before, but since some people have found this useful, I figured I might as well submit this tutorial, rather than have the plagiarized version just sit in the graveyard.

Introduction


Blizzard designed the World Editor to be very easy to modify. In this tutorial, we'll take a look at just one aspect of the World Editor that you can modify, the Trigger Editor. I'll show you everything you need to know to add your own trigger categories, events, conditions, actions, functions, variable types, and variable presets. If you prefer the GUI, but find yourself constantly needing to add bits of JASS here and there, this is for you.

Getting Started


First and foremost, create a folder named "UI" in your main Warcraft III directory. It must be named "UI" for the World Editor to recognize the files inside it.


Now, you will need the latest versions of the following files:


UI\TriggerData.txt


UI\TriggerStrings.txt


UI\WorldEditStrings.txt


These can be extracted from War3Patch.mpq. Put these files in the UI directory you created.


These files are read whenever you load up the World Editor, but only then. So if you make a change to any of these files, and you have the WE open, you will need to close and restart the WE for the changes to take effect.

File Formats


Let's take a look at what kinds of information is stored in each of these files. This will give you an idea of what kinds of things you can change about the Trigger Editor. Each file is divided into sections, and each section starts by its name in brackets. You shouldn't add lines outside the appropriate section. For example, all trigger categories should go after [TriggerCategories] but before [TriggerTypes]. Don't just add stuff to the end of the file. One trick is to know the section after the one that you want to add on to. Then do a "Find" for the name of that section, and you will jump right to the end of the previous section, where you can add things right away.


The first and most important file is TriggerData.txt. This is where all the functionality gets built in. The file contains the following sections:

[TriggerCategories] - Categories are a way to organize actions and functions. They show up as a prefix followed by a hyphen, and when selecting an action, you can choose to show only actions from a particular category. For example, in "Unit - Kill Unit", the category is "Unit", and the action is "Kill Unit".

[TriggerTypes] - All of the Variable types are defined here. You may notice that there are very many more variable types listed here than you see in the Variables editor. This is because you can define types that the Trigger Editor can use internally, but would not be meaningful as regular (global) variables. More on that later.

[TriggerTypeDefaults] - Some Variable types are initialized to default values. For example, Integers are set to 0 by default. Some types must be initialized by a function (usually "Create"____) in order to work properly, so it's good practice to do this here. For example, timers are initialized to CreateTimer() and unit groups are initialized to CreateGroup().

[TriggerParams] - These are known as "Presets" in the Trigger Editor. These are defined values for a variable type that you can choose from a list. Some variable types have only these presets and no other purpose. For those of you with outside programming experience, these are basically "enumerated types". An example is the list of orders you can issue targetting a point ("Move", "Human Archmage - Blizzard", "Orc Tauren Chieftain - Shockwave", etc.).

[TriggerEvents] - All events are defined here.

[TriggerConditions] - All conditions are defined here.

[TriggerActions] - All actions are defined here.

[TriggerCalls] - These are known as "Functions" in the Trigger Editor, when you need to fill in a value as a parameter. Examples are "Math - Random Number" and "Region - Center of Region".

[DefaultTriggerCategories]

[DefaultTriggers] - Whenever a new map is created, trigger categories (for organization) and triggers can be created by default. The only things listed here are the familiar Initialization trigger category and Melee Initialization trigger. You might find occasion to add your own here, or you might remove the lines in this section if you're tired of having to delete Melee Initialization.


Whereas TriggerData.txt defines the functionality and structure of the Trigger Editor, the other two files, TriggerStrings.txt and WorldEditStrings.txt, provide all the text that will be displayed to you when creating and editing triggers. WorldEditStrings.txt is simply a list of strings, and is used by other parts of the World Editor as well. TriggerStrings.txt strings follow a special format, since you have to tell the Trigger Editor what parts of the string are editable parameters (underlined).


The sections in TriggerStrings.txt are as follows:

[TriggerEventStrings] - Strings for all events.

[TriggerConditionStrings] - Strings for all conditions.

[TriggerActionStrings] - Strings for all actions.

[TriggerCallStrings] - Strings for all functions.

[AIFunctionStrings] - These are used by the AI editor.


And WorldEditStrings.txt:

[WorldEditStrings] - Strings for categories, variable types, variable type defaults, and presets are in this file.


Notice that WorldEditStrings.txt has only one section. This means you can add lines anywhere in the file. I find it easiest just to add lines to the end of this file, but it is up to you.

Adding an Action


OK, let's get down to business. I'll start here with a simple example of adding an action and come back to actions much later in the tutorial with a slightly more detailed example.


To keep things simple, let's add a function that is declared in common.j but not exposed in the Trigger Editor. One thing that many people concerned with efficiency note is that many GUI actions make calls to functions defined in Blizzard.j whose only purpose is to call a native function in common.j. There are cases in which this can be justified, but there are also cases where it's just completely redundant and only adds to overhead. (Some use this as an attack against using the GUI, but as you will see, it is very simple to work around this "problem".) One example is the function DestroyEffectBJ, which simply calls DestroyEffect. However, it is DestroyEffectBJ that is exposed in the Trigger Editor, not the ever so slightly more efficient DestroyEffect. So for this first example, let's add DestroyEffect to the Trigger Editor.


Open up TriggerData.txt. The order in which lines appear in TriggerData.txt directly affects the order in which items appear in lists in the Trigger Editor. So if you want to add this new action, you probably want it to be right before or right after the existing Destroy Effect action. We know that DestroyEffectBJ will appear in the file, so let's do a Find on that. We then see the lines:

 Code:

DestroyEffectBJ=0,effect
_DestroyEffectBJ_Defaults=GetLastCreatedEffectBJ
_DestroyEffectBJ_Category=TC_SPECIALEFFECT


What does each line say?


The first line says that we have a function called DestroyEffectBJ. The value of "0" means that this function is defined in WC3: Reign of Chaos. If the function is only defined in WC3: The Frozen Throne, you would put a value of "1" here. The remaining values are the variable types of the parameters. In this case, we have an "effect", a special effect.


The next line provides the defaults for any parameters. In this case, GetLastCreatedEffectBJ ("Last Created Special Effect"), a function, is the default value.


The last line states what category this action is in. Remember that the categories are defined at the top of the file. Since this action deals with special effects, it is placed in TC_SPECIALEFFECT.


So now we want to add an entry for the function DestroyEffect. It has the same purpose and parameters as DestroyEffectBJ, so in this case we can just copy and paste the lines, changing only the name of the function. So we add the lines:

 Code:

DestroyEffect=0,effect
_DestroyEffect_Defaults=GetLastCreatedEffectBJ
_DestroyEffect_Category=TC_SPECIALEFFECT


And we are finished with TriggerData.txt for this example.


Next, we need to add the corresponding strings to TriggerStrings.txt. Open up TriggerStrings.txt, and just as before, do a Find on "DestroyEffectBJ". This file has its entries in the same order as TriggerData.txt, and it is probably a good idea to keep with this convention. You will see the lines:

 Code:

DestroyEffectBJ="Destroy Special Effect"
DestroyEffectBJ="Destroy ",~Special Effect
DestroyEffectBJHint=


The first line is what you see when you are choosing an Action from the list. The second line is what you see when you have the Action currently selected. This is also the line that displays if the action is part of your trigger. The third line, the "Hint", displays in small gray print when you have the action selected. DestroyEffectBJ does not have a Hint, so this line is left blank.


Take a close look at the second line. We need to tell the Trigger Editor where in the string we are leaving gaps for parameters. The way you have to do this is by ending the string with a double quote ("), followed by a comma, then a tilde (~) and the "name" of the parameter. You can put anything for this name. If no default is specified for the parameter which this represents, you will see this name in red in the Trigger Editor, signifying that you need a parameter. If you do have a default value, you will never see this name, so you can call it anything.


If we needed to add on to the string, we would have to place a comma after the word Effect, followed by a double quote to show that we are starting the string again, and if we had more parameters, we would have to break the string again with a double quote and comma, and so on. (You can browse TriggerStrings.txt for examples.)


OK, so now let's make the string entry for DestroyEffect. We can mostly just copy and paste here again as before. However, we do not want both actions to have identical strings. If they did, we wouldn't be able to tell them apart! So we want to add a note that this is the "efficient" version. We can also fill in the Hint field. We don't really need to change the second line, because we have enough information to keep the two actions apart with just modifying the first line.


So then we add the lines:

 Code:

DestroyEffect="Destroy Special Effect (Efficient)"
DestroyEffect="Destroy ",~Special Effect
DestroyEffectHint="Calls DestroyEffect directly -- more efficient."


Save and close TriggerStrings.txt. Start the World Editor, open up a map, and check to make sure the action shows up in the Trigger Editor. Now throw it in a trigger and save the map to make sure you get no compilation errors. See how simple that was?


Next time I'll discuss categories, some things about variable types, variable presets, events, and conditions.

Make sure you've followed everything up to this point before moving on.


--------------------------------------------------------------------------


For this next section, I'll walk through how to add the functionality of "trackables" to the Trigger Editor. If you are not familiar with trackables, they are objects that can detect mouse clicks. (Note to link here to a trackable tutorial -- someone should write one!) Adding trackables to the editor will get us through most of the rest of the types of things we can add to the GUI.

Adding a Category


First, we need to add a category for our trackable functions. This is very simple. In TriggerData.txt, you'll see the list of categories. Scroll to near the end of the list. A category definition looks like this:

 Code:

TC_VISIBILITY=WESTRING_TRIGCAT_VISIBILITY,ReplaceableTextures\WorldEditUI\Actions-Visibility


First is the internal name of the category, the next part is the string that is shown in the Trigger Editor whenever the category comes up (and we see that it is defined in WorldEditStrings.txt), and the last part is the little icon that shows up next to Actions in your trigger.


Using the same model, we'll make one for trackables:

 Code:

TC_TRACKABLE=WESTRING_TRIGCAT_TRACKABLE,ReplaceableTextures\WorldEditUI\Actions-Nothing


We don't have an icon for trackable actions, so we'll just use the default of Actions-Nothing.


You might notice that some categories have a ",1" at the end. As you'll see in the comments at the top of the file, this means that the trigger category's name will not display. We want it to display here, so we won't add the ",1".


Now we have to define the string WESTRING_TRIGCAT_TRACKABLE. This goes in WorldEditStrings.txt, so open that file, go to the bottom, and add the line:

 Code:

WESTRING_TRIGCAT_TRACKABLE="Trackable"


Optionally, you can search for the other trigger categories and add it among that list, but this isn't necessary.

Adding a Variable Type


Next we want to define the variable type so that the Trigger Editor understands what a trackable is.


First, let's take a look at how variables are defined. I think the comments at the top mostly speak for themselves, so I'll just paste them here.

 Code:

[TriggerTypes]
// Defines all trigger variable types to be used by the Script Editor
// Key: type name
// Value 0: first game version in which this type is valid
// Value 1: flag (0 or 1) indicating if this type can be a global variable
// Value 2: flag (0 or 1) indicating if this type can be used with comparison operators
// Value 3: string to display in the editor
// Value 4: base type, used only for custom types
// Value 5: import type, for strings which represent files (optional)
// Value 6: flag (0 or 1) indicating to treat this type as the base type in the editor


As before, "Value 0" is the flag that says whether the type is valid in TFT-only (1) or RoC and TFT (0).


If a variable has "Value 1" set to 0, you won't be able to make variables of this type in the Variables editor.


"Value 2" has to do with conditions. If you can compare two values of the type, then this would be set to 1 (for example, "integer" has this value set, because you can compare integers). For types where it doesn't make sense to compare two values, this can be set to 0 (for example, "gamecache" has this flag disabled, because comparing two game caches doesn't really make sense).


"Value 3" is the string to use when this type is displayed in the editor (both in the Variables editor and as the title of a dialog box when you need to fill in something of this type as a parameter).


Values 4 through 6 are optional and we can ignore them for now. I'll get to them later.


It makes sense to add types close to related types, but it doesn't really matter except for the order of variable types in the Variables editor. If you want to keep things alphabetical, Trackable would have to be added between "timerdialog" ("Timer Window") and "trigger".


So let's figure out what flags we need. I actually don't remember if trackables are defined in RoC. I think they are, but to be on the safe side, you can always just put a "1" for the version. If you are making maps for TFT only, it doesn't matter what you put here.


We do definitely want to be able to make variables of type trackable, so let's put a 1 for the global variable flag.


Yes, trackables can be used in comparison operators; we might need this functionality to determine whether a clicked-on trackable is a certain one, so we will put a 1 in this field.


Finally, we need to add a link to a string for the name "Trackable". So, our added line will look like this:

 Code:

trackable=1,1,1,WESTRING_TRIGTYPE_trackable


Don't forget to add the appropriate string in WorldEditStrings.txt.

Adding a Default Value for Type


I will skip over adding a type default. This is unnecessary for trackables, since we want them to start out as null. It is a very simple procedure though, and I'm sure you can figure it out for yourself. But to save you the trouble, I'll let you know that if you decide to implement the following data types: region, unitpool, itempool; you will want to fill something in for this section.

Adding a Preset


Presets for trackables don't really make sense. However, there is one preset value that does make sense for trackables, and all handle-derived types: null. You see this in the Trigger Editor as "No item", "No unit", etc. The value "null" has to be defined separately for each type. We will want a "No trackable" option for comparisons, to check if a value is null or not.


Here (as everywhere) it is easiest to find an example similar to what you want, copy it, and modify it for your purposes. Take the example of "No unit":

 Code:

UnitNull=0,unit,null,WESTRING_TRIGUNIT_NULL


First let's look at what each part means. The key name here is just for reference, "UnitNull" has no meaning in JASS or anywhere. "0" indicates that this value is valid in RoC. "unit" is the type of preset that we are defining. "null" is the value of the preset. And then finally is the string that will display in the GUI when we use this preset (in WorldEditStrings.txt, this is "No unit").


We want to copy that for trackables. The most obvious way is just to change all instances of unit to trackable. We can set the version flag to 1 to mark this as TFT-only (keeping it as 0 will work just as fine too). Again, don't forget to add the string in WorldEditStrings.txt.

 Code:

TrackableNull=1,trackable,null,WESTRING_TRIGTRACKABLE_NULL

Adding an Event


There are two events that apply to trackables, defined in common.j:

 Code:

native TriggerRegisterTrackableHitEvent takes trigger whichTrigger, trackable t returns event
native TriggerRegisterTrackableTrackEvent takes trigger whichTrigger, trackable t returns event


The first is a "Mouse Click" event, the second is a "Mouse Over" event. Let's add both of them.


Adding an event is almost identical to adding an action. The only differences are that you define them in the [TriggerEvents] section, and that you omit the first parameter, which is assumed to be of type "trigger". Using the same model as with actions (minus the first parameter), add these to the bottom of the events section. Remember, you don't have to specify Defaults. And don't forget to add entries in TriggerStrings.txt!

Adding a Condition


The only conditions besides And and Or are comparisons. So we will want to add a "Trackable Comparison". The difference between this section and Events or Actions is that conditions aren't function calls, but are handled internally by the Trigger Editor. Actually the operators are converted to JASS directly, using ==, !=, etc. Let's take a look at "Trigger Comparison":

 Code:

OperatorCompareTrigger=0,trigger,EqualNotEqualOperator,trigger
_OperatorCompareTrigger_Defaults=_,OperatorEqualENE,_
_OperatorCompareTrigger_Category=TC_CONDITION


As always, we have the version flag first. Then we have one parameter of type trigger, the next of type EqualNotEqualOperator, and the last another of type trigger. There are two types of operators that comparisons use: EqualNotEqualOperator offers only the options "Equal to" (==) and "Not equal to" (!=), while ComparisonOperator offers "Equal to" (==), "Not equal to" (!=), "Greater than" (>), "Greater than or equal to" (>=), "Less than" (<), and "Less than or equal to" (<=). The default of OperatorEqualENE means that the operator will default to "Equal to". This is normally what we want, so that's a good default.


It doesn't make sense to have a trigger as being "greater than" another trigger, so the EqualNotEqualOperator is appropriate here. Similarly, for trackables, we want this operator. So, as is the usual method, we copy and paste these lines, changing "trigger" to "trackable". If we want to keep the lines alphabetical, we would put these lines right above those for OperatorCompareTrigger. Then we have:

 Code:

OperatorCompareTrackable=0,trackable,EqualNotEqualOperator,trackable
_OperatorCompareTrackable_Defaults=_,OperatorEqualENE,_
_OperatorCompareTrackable_Category=TC_CONDITION


And don't forget to add the appropriate strings to TriggerStrings.txt.

Adding a Function


I'm skipping the obvious action CreateTrackable for now and jumping to the function section ([TriggerCalls]). Let's add a simple function that returns a value, since we haven't done it before. The one we need concerning trackables is GetTriggeringTrackable. This is defined in common.j:

 Code:

constant native GetTriggeringTrackable takes nothing returns trackable


Let's add this all the way at the end of the [TriggerCalls] section. Again, let's look at an example, the currently last function:

 Code:

GetLastCreatedTextTag=1,0,texttag
_GetLastCreatedTextTag_Defaults=
_GetLastCreatedTextTag_Category=TC_LAST


Of course, first comes the name of the function, then the version flag. As you can see, there is another flag in this section. Looking at the comments at the top of this section, it says that this value tells the GUI whether or not this function can be used in events. Sometimes it makes sense for a function to be allowed to be used in events, other times no. Use your judgment here, although most of the time it doesn't matter. If you set it to 0, and change your mind, you can always set it to 1.


Next comes the return type. It's easy to get confused here, because normally at this point you have a parameter list. After the return type, the parameter list comes next, but in this case the function takes no parameters.


So now go ahead and add the entry for GetTriggeringTrackable. As a tip, this is an event response function, so the category should be TC_EVENTRESPONSE. And, as always, don't forget to add strings in TriggerStrings.txt.


By now you have trackables almost fully implemented. What is left to add is the CreateTrackable function. As you will see, it is not as simple as you might think.


In the next section I'll discuss actions in detail, the rest there is to know about variable types, common errors, organization and style tips, and suggestions for possible modifications.


Stay tuned for the gripping conclusion!

Make sure you've followed everything up to this point before moving on.


--------------------------------------------------------------------------

Adding Functions (continued)


OK, so now we want to add the CreateTrackable function. I'll give you the function declaration from common.j for reference:

 Code:

native CreateTrackable takes string trackableModelPath, real x, real y, real facing returns trackable


Creating a unit, creating an item, creating a weather effect, etc. are all actions. So it is logical to presume that creating a trackable is an action. Then, let's add an entry for creating a trackable in [TriggerActions].


Up to what you have learned so far, you might come up with the following entry:

 Code:

CreateTrackable=1,string,real,real,real
_CreateTrackable_Defaults=_,_,_,_
_CreateTrackable_Category=TC_TRACKABLE


This can be improved in a number of ways. First we need to learn about derived variable types.

Custom Variable Types


A custom type is a variable type that is really some other type in JASS, but the GUI treats it differently. There are tons of examples of these. The most well known are the "-code" types: unitcode (Unit-Type), itemcode (Item-Type), destructablecode (Destructible-Type), abilcode (Ability), ordercode (Order), techcode (Tech-Type), etc. You have probably used most or all of these types before. Depending on your level of JASS experience, you may know that all of these types are actually just integers. So why don't you just use integers? The reason is that the way you think about them is different. It wouldn't make sense to make a unit of type 1004364275, or to tell a unit to 852450. The idea is to make sure that you can only create units of valid unit-types, and to issue valid orders. It's easier to store them all internally as integers, but it's easier to work with them if they are separate types.


Defining a custom type is simple. All you have to do is add an extra value to the end of your type definition. This type will be the actual type of this variable when everything gets converted to JASS.


For example, we see that unitcode (Unit-Types) are defined like this:

 Code:

unitcode=0,1,1,WESTRING_TRIGTYPE_unitcode,integer


The last two possbile parameters deal only with extensions of the "string" type. The purpose for this support is so that different types of files display their own special dialog. The final parameter flag of 1 just means that these types will be treated as "String" in the editor (this is why you don't see a "Model File" etc. variable). Anyway, you probably won't have to mess with these types much, but you will at least want to know that these exist (especially "modelfile").

 Code:

aiscript=0,0,0,WESTRING_TRIGTYPE_aiscript,string,AIScript,1
modelfile=0,0,0,WESTRING_TRIGTYPE_modelfile,string,Model,1
anyfile=0,0,0,WESTRING_TRIGTYPE_anyfile,string,Any,1
preloadfile=0,0,0,WESTRING_TRIGTYPE_preloadfile,string,Preload,1
imagefile=0,0,0,WESTRING_TRIGTYPE_imagefile,string,Image,1


There are additional types that are handled specially by the editor. You can find these at the end of the list of variable types in TriggerData.txt. Most of these are designed for very specific uses. One to be aware of is "StringExt", which is the same as "string", except it gives you a large box of text to type in (supports Return key for new lines). This is used in creating Quest Log entries and some other places.

Actions Continued


If we look at the common.j definition of CreateTrackable, we see that the first string is the path to the model of the trackable. Thus, we don't want this parameter to be any string, but we want it to be specifically a "modelfile" string. After making this change, our action entry now looks like this:

 Code:

CreateTrackable=1,modelfile,real,real,real
_CreateTrackable_Defaults=_,_,_,_
_CreateTrackable_Category=TC_TRACKABLE


There are a couple optional fields that actions support in addition to Defaults and Category. One of these is Limits, which lets you set bounds on a value. This is both for error-proofing and for information. The way limits works is that you must provide a minimum and maximum for every parameter in the list, if you are supporting limits. For parameters that don't have bounds, use an underscore (_), but you still need to provide two values, so you need to use "_,_". It can get somewhat hard to read if your parameter list is long.


Modelfiles don't have bounds, so we give a _ for their min and a _ for their max. The next parameter is the X position. We can't assume any sort of map size, so we'll ignore a minimum and maximum in this case. Same for Y position. Our last parameter is the facing angle of the trackable. The angle measure is in degrees, so our valid values would be 0 to 360.


So, we add the line

 Code:

_CreateTrackable_Limits=_,_,_,_,_,_,0,360


This field is normally placed between Defaults and Category.


We probably want to add some defaults too, since sometimes you may know that you want to create a trackable, but don't have the exact values for the parameters yet. When deciding what the defaults should be, think about a value you might often want for the field. If that doesn't apply, it's a good idea to stick with standard values that the WE uses in other places, or simple values.


For the modelfile, I chose the one that is used in AddSpecialEffectTargetUnitBJ. For the X and Y values, I chose 0 and 0, the center of the map. And for the facing value, I used the default building facing, a good choice if you have no preference.


Putting it all together, our entry for the action CreateTrackable now looks like this:

 Code:

CreateTrackable=1,modelfile,real,real,real
_CreateTrackable_Defaults="Abilities\Spells\Other\TalkToMe\TalkToMe.mdl",0,0,RealUnitFacing
_CreateTrackable_Limits=_,_,_,_,_,_,0,360
_CreateTrackable_Category=TC_TRACKABLE


So we're done, right? Well...... Suppose we are done. How would you go about using Trackables in the Trigger Editor? First you'd use your action "Trackable - Create". Then what? Now you set it to a variable. Set MyTrackable = (Last Created Trackable). What? We never defined a "Last Created Trackable". This is the problem we now face. As it stands, we have no way of accessing trackables after we create them!


The thing is, one of the things those "pesky" BJ functions do is store newly created objects in temporary variables defined in Blizzard.j. We don't have anything like that for trackables. If we wanted to keep with the style of the Trigger Editor, we really ought to write a wrapper function and store the last created trackable, and make another function that returns the "Last Created Trackable". We can't (rather, shouldn't) modify Blizzard.j (if we did, we'd have to import it into any map that needed it, which adds a bunch to filesize and can cause some other issues as well). So the method I'll outline here is to break with the normal "GUI" way of thinking about things, and use a more "JASS" way of thinking.


What is an Action in the Trigger Editor? It is just a function that returns nothing. More accurately, it is a function that returns anything, but that thing is ignored. In our case, CreateTrackable does return a value, one of type trackable. We don't want that value to be ignored, because we need to store it in a variable. How can we store it in a variable? Well, since CreateTrackable returns a value of type trackable, we should define it as a Function (in [TriggerCalls]), rather than an Action. Then we can use the Set Variable action to store the newly created trackable in a variable.


Remember, with Functions, there are two more values we need to fill in. The first if we want this function to be usable in events, and the second being the return type. It is up to you whether you want this usable in events. I am uncomfortable with this type of construction, so I put a 0 for that flag. If you do put a 1 here and choose to use this function in events, keep in mind you'll have no variable pointing to the trackable; and, if the event is not dynamically added with Trigger - Add Event, the trackables in the events will be created at Map Initialization. Adding the two values needed, and moving the code to the [TriggerCalls] section (Don't forget also to modify TriggerStrings.txt as appropriate.), we wind up with the following entry:

 Code:

CreateTrackable=1,0,trackable,modelfile,real,real,real
_CreateTrackable_Defaults="Abilities\Spells\Other\TalkToMe\TalkToMe.mdl",0,0,RealUnitFacing
_CreateTrackable_Limits=_,_,_,_,_,_,0,360
_CreateTrackable_Category=TC_TRACKABLE


Save, close, open the WE, and make sure everything is working. Congratulations, you've fully implemented trackables into the GUI!


So now, instead of using an Action "Trackable - Create", you would use Set Variable: Set MyTrackable = Create Trackable ...


It might look a little clunky, but it's something done all the time in JASS, so it shouldn't be too unfamiliar.

The ScriptName Field


There's one more useful field provided for Actions (and only Actions, unfortunately): ScriptName. This lets you separate the name of the JASS function from the name of the GUI action. This is actually quite powerful for several reasons.


One advantage is that you can give multiple GUI names to the same function. Maybe you want one function to work for both "integer" and "unitcode". In JASS, of course this would work out, since both types are really integers. In the GUI, you need two separate entries, but by using the ScriptName field, you could have two names that point to the same function.


The bigger advantage with this field is that it separates the GUI implementation from the JASS implementation. Maybe the GUI thinks that you are using one function, but when you save the map, a different JASS function is really used. It is important to know some of what goes on when you save a map in the World Editor to appreciate how this works. When you save a map in the WE, all of the triggers and your triggers' structure are saved in a triggers file inside the map. This file serves no purpose in Warcraft III, and is only used by the WE. (This is why almost all map protector programs destroy this file.) Additionally, all of your triggers are converted to JASS and are placed inside a script file inside the map. This file is the one that is actually used by Warcraft III, and includes other information as well, such as pre-placed units and regions, variable initializations, and player start locations.


When you use the ScriptName field, you change the JASS part, the script file that is created when you save, but not the GUI part, the triggers file created when you save. This means that changes to the ScriptName field will change how the map compiles in your editor, without preventing the map from being opened in other versions of the editor.


Here is a prime example. Let's revisit our first example of an Action: DestroyEffectBJ. The entry looks like this:

 Code:

DestroyEffectBJ=0,effect
_DestroyEffectBJ_Defaults=GetLastCreatedEffectBJ
_DestroyEffectBJ_Category=TC_SPECIALEFFECT


Now, we know that DestroyEffectBJ is redundant and inefficient to use, and that DestroyEffect is preferred. We can use the ScriptName field to replace all instances of DestroyEffectBJ with DestroyEffect, without having to edit anything in any map! The WE will still think you are using DestroyEffectBJ just as before, but when you convert a trigger to JASS, or save the map, DestroyEffect will be used.

 Code:

DestroyEffectBJ=0,effect
_DestroyEffectBJ_Defaults=GetLastCreatedEffectBJ
_DestroyEffectBJ_Category=TC_SPECIALEFFECT
_DestroyEffectBJ_ScriptName=DestroyEffect


This method is greatly preferred to using two separate actions (as we did in the first example) in a situation like this, because you don't need to change any maps using the GUI, and when you use this function, your map will still open in other editors. (It's also less work, you don't need to do anything in TriggerStrings.txt when you do this.) Unfortunately, you will find that this nifty technique is quite limited, since the parameters list of the two functions must be identical. The purpose of many BJ functions is to reverse the order of parameters (for natural language purposes), so these can't be simply changed by using the ScriptName field.

Adding Your Own Functions


This tutorial used as examples native functions that are not defined in the GUI. When you implement any function from common.j or Blizzard.j into the GUI, there is nothing you need to do to get them to work; they will just work.


Everything you learned applies for adding your own functions. The Trigger Editor is agnostic as to whether the functions it supports are defined or not. The only thing you have to remember is that for any of your own functions you add, you have to include the functions in the Custom Script of your map. You only have to do this if you are actually using these functions in your map, though. I find it easiest to keep a documented list of all the functions I've added in a separate file, then copy and paste that file into any map I create that will need the functionality.

Common Errors


I don't want to go into errors too much, because there is bound to be some error I haven't gotten yet, but here are some common errors you might make while editing in haste:


Forgetting TriggerStrings.txt entry


Forgetting WorldEditStrings.txt entry


Bad formatting on your TriggerStrings.txt entry (misplaced double quote, misplaced comma)


Missing a parameter (or having an extra one) in your TriggerStrings.txt entry


Counting return type in a Function as one of your parameters


Using the wrong parameter types, or in the wrong order, or having the wrong number of them, than the actual function expects (The World Editor will not complain until your try to save your map using one of these functions; then you will get a JASS compilation error.)


Using an Action or Function that requires imported code, but forgetting to import the appropriate functions into Custom Script.


I guess I should point out one more thing here, if you haven't picked it up already. If you use any actions, etc. that you add to the GUI, and try to load up the map in an unmodified WE, the WE will crash with an error saying that it can't find whatever function. So if sharing your map is of high importance, be careful when using stuff you added to the GUI. You could either share your WE mod, or convert the offending triggers to JASS. As a tip, always put a note in the Hint field in TriggerStrings.txt in functions you add that they were added by you. You should also put a note on functions that are not defined in common.j or Blizzard.j that you will need to include them. These will help save you some headaches, but are also intended to serve as documentation if you choose to share your WE mod.

Notes on Organization


You probably noticed by now that you can make comments in TriggerData.txt, etc. by starting lines with "//". Since order matters, and since there are many sections in especially TriggerData.txt, you may have modifications all over the place. I strongly suggest that you put a comment above every change you make to the editor, or if you added a ton of related functions in a row, above the first one. Give it a "tag" name, so you can do a Find on this tag, and easily jump to any of your modifications. Why do this? If a new Warcraft III patch is released that modifies TriggerData.txt, chances are you will want to upgrade. You don't want to lose any of your modifications, so by tagging all of them, you can easily import them into a fresh TriggerData.txt (and TriggerStrings.txt and WorldEditStrings.txt). Although at this point a major patch is unlikely, you don't want to be caught unprepared, and it's just a good idea besides.

Notes on UI Design


I would not call myself an expert in User Interface design, but if you are working on a large JASS system that you want to implement in the GUI, here are some tips:


Consolidate similar functionality. In common.j, you have SetHeroStr, SetHeroAgi, SetHeroInt. In the GUI, you have Modify Hero Attribute, with options to Add, Subtract, or Set to a value. You will need an additional function to handle the parameters, but it can lead to actions and functions that are much easier to work with.


Define custom types and presets liberally. They don't cost anything, except on your own hard drive, and add immensely to implicit documentation. You will thank yourself when you dust off a map after 1-2 years.


Use the most specific variable type possible. If you really want a Unit-Type, use unitcode, not integer. If you have some opcode parameter that is an integer from 0 to 4, don't use the integer type. Instead, define a custom type based on integer, and list the 5 values as presets.


Don't add a function to the GUI unless you can see a use for it in the GUI framework. Think about how you would go about using a particular function in the context of the GUI. You may need to rethink your approach.

Exercises


Here are some ways you might consider extending your Trigger Editor:


Add the "region" variable type (as opposed to rect) and all its functionality.


Add the native functions that take real (coordinate) parameters, as opposed to location parameters.


Find other examples of "dummy" BJ functions that can be replaced with their native counterparts just by using the ScriptName field (and let me know about them!)


Add a system like KaTTaNa's Handle Variables or Vexorian's CSCache to the GUI.


Browse the Jass Vault (www.wc3jass.com), and implement whatever functions you think would be useful to you.


If you are working on a large system that relies on JASS, add the most important "high level" functions to the GUI.


Well, that's all folks. I hope you see how easy it is to customize the Trigger Editor to your needs and, if you don't see it yet, I hope you will one day see how silly the debate between GUI and JASS really is.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Well, since I found the CnP'ed version fine (sorry about that, didn't know at the time ), there's really no reason not to approve this one!

#### Triax

#### Triax

 Level 5

JoinedFeb 5, 2008Messages109

Resources: 2

#### Resources
Tools 1Maps 1

I'm wondering how to create a function for GUI setting variables local.


Please have a look at this thread and edit your tutorial.


By the way, very good. =)

#### Shvegait

S

#### Shvegait

Declaring local variables is not possible through editing TriggerData.txt (see post in the other thread). If extensions of CustomScriptCode were allowed, any JASS construction would be possible as GUI actions (I mean, rather than having to use Custom Script as usual), but alas, this is not the case.

#### Triax

#### Triax

 Level 5

JoinedFeb 5, 2008Messages109

Resources: 2

#### Resources
Tools 1Maps 1

But the SetVariable-function uses the set-command in Jass, too. There must be a way to create a function saying "set x y".

#### Shvegait

S

#### Shvegait

OK, first of all, SetVariable is NOT a function. It is a placeholder for some line of code, just like everything in TriggerData.txt. If you check it out, there's a comment above it saying "Specially handled by the editor". This means the editor interprets SetVariable different from anything else you define. When converted to JASS, these lines become "set param1 = param2" instead of "call SetVariable(param1, param2)". Similar things exist for If/Then/Else, For loops, and so on. But it's not possible through editing TriggerData.txt to add your own lines that are interpreted specially -- these are handled at a deeper level. Anything you try to add will be interpreted as a function.

#### ady_illidan

#### ady_illidan

 Level 11

JoinedDec 11, 2007Messages879

Resources: 2

#### Resources
Maps 2

a very useful tutorial

good work!

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

Nice tutorial.

i've a question is there a way to remplace a function call buy a reading of a variable, for example GetLastCreatedUnit() by bj_lastCreatedUnit ?

i think not but maybe i missunderstood something

#### DesKaladA

#### DesKaladA

 Level 15

JoinedJun 28, 2007Messages1,424

Resources: 7

#### Resources
Models 2Maps 1Templates 1Tutorials 3

anytihing with BJ is just calling blizzard.j to call the jass function and i wont replace funtcions ide jsut add new ones

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

DesKaladA said:

 anytihing with BJ is just calling blizzard.j to call the jass function and i wont replace funtcions ide jsut add new ones


Click to expand...

it's not my question, i already know what you said

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

i've found a solution

We can add a predifined variable, so no more need to call a funcion that return a variable.

example for Integer A :

 Code:

//***************************************************************************
[TriggerParams]
// Defines possible values for variable types
// Key: arbitrary text
// Value 0: first game version in which this parameter is valid
// Value 1: variable type
// Value 2: code text (used in script)
// Value 3: display text
//
// Note: If the code text is a literal string, surround it with backward single quotes (`),
// and they will be converted to double quotes in the script.
//

// Integers
IntegerA=0,integer,"bj_forLoopAIndex","Integer A"

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

In Warcraft 3 Patch 1.30 Warcraft 3 changed its file structure to casc.

Edit: The technique mentioned in this post does not work for Wracraft 3 V1.31+.

One can still overlap the wanted files by placing them inside Warcraft 3 inside a subfolder named as the mpq they earlier were placed.

TriggerData.txt needs to be placed into a folder named "UI" which is in a folder named "war3.mpq".

 Folder Change


UI\TriggerData.txt

->

war3.mpq\ui\TriggerData.txt

Also Patch 1.30 gave each language an unique place into the casc which one has to address if one wants to change World Editor text. Therefore the text Files and the data-Files are in 2 seperate folders.

"TriggerStrings.txt & WorldEditStrings.txt" are not placed inside "war3.mpq\ui\" place them into a folder named as your used local mpq instead (the list below was shown for me in cascview)

 Locals list


dede-war3local.mpq

enus-war3local.mpq

eses-war3local.mpq

frfr-war3local.mpq

itit-war3local.mpq

kokr-war3local-mpq

plpl-war3local.mpq

ruru-war3local.mpq

zhcn-war3local.mpq

zhtw-war3local.mpq

Disclaimer only tested the german version which worked.

 Example German


UI\TriggerStrings.txt

UI\WorldEditStrings.txt

->

dede-war3local.mpq\UI\TriggerStrings.txt

dede-war3local.mpq\UI\WorldEditStrings.txt

Edit December 2018:

One can also place all custom TriggerData/String.. files into Folder "war3mod.mpq\ui\"

then the ones inside war3mod.mpq will be loaded regardless of local language installation.

Warning do not open warcraft3 while opened in cascview, Warcraft 3 breaks and has to be repaired in that case.


 Last edited: Jun 23, 2019

#### 4rum_Arts

#### 4rum_Arts

 Level 2

JoinedJun 5, 2011Messages24

또한 모든 사용자 정의 TriggerData / String .. 파일을 "war3mod.mpq \ ui \"폴더에 저장할 수 있습니다.

war3mod.mpq 파일은 로컬 언어 설치 여부에 관계없이로드됩니다.

버전 1.31.1부터는이 방법으로 수정할 수 없습니다. 경로가 다시 바뀌 었습니까?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

I used bing to translate your text, I can not read korean.

Yes, war3mod.mpq is gone in Warcraft 3 V1.31. It should be replaced by a new system extension ".w3mod". But that currently doesn't seem to work (or at least I did not succeed in using it).

 The data layout in CASC has changed, each directory with the “.w3mod” suffix acts as an overlay for the engine


Click to expand...

From 1.31.0 Patch Notes

Currently V1.31.1 one has to use "Allow Local Files" and place the custom triggerData/triggerStrings inside a folder named "ui" inside the main warcraft 3 folder, the one with the folders "Data", "x86_64" and "x86" and the launcher.

To enable using the custom ui file one adds in regedit a dword named "Allow Local Files" with value 1 inside HKEY_CURRENT_USER\Software\Blizzard Entertainment\Warcraft III


 Last edited: Jun 23, 2019

#### 4rum_Arts

#### 4rum_Arts

 Level 2

JoinedJun 5, 2011Messages24

Oh ... I'm sorry. I thought Google Translator was in automatic translation state, but I wrote it in Korean.

Thank you. Last time I got help, but this time it was a great help too!

 You must log in or register to reply here.


### Similar threads

Poll

 [Tutorial] Modifying the World Editor's Trigger Editor


Sevion

Jan 2, 2008

Tutorial Archive

Replies10Views5K

Jan 29, 2008
PurplePoot

 WE Map Editor for Newbies!


Slaydon

May 8, 2009

Misc/General Mapping Tutorials

Replies38Views18K

Jun 10, 2009
Ricardo Irving

 Basic Trigger Editor Tutorial


Wizardum

May 2, 2010

StarCraft II Tutorials

Replies15Views17K

Dec 16, 2010
Twisting

Locked

[Trigger]
 Temporary variables patterns in the trigger editor


Barade

Jun 24, 2021

Triggers & Scripts

Replies5Views2K

Jun 27, 2021
GhostWolf

 Setting up NewGen WE and Intro to Trigger Editor


Nestharus

Jan 28, 2011

Misc/General Mapping Tutorials

Replies17Views23K

Aug 11, 2011
Nestharus

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top