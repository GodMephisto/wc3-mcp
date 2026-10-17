---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/structs-a-rough-crash-course.293663/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Structs a Rough Crash Course
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/structs-a-rough-crash-course.293663/>
#### MyPad

#### MyPad

 Level 31

JoinedMay 9, 2014Messages1,878

Resources: 27

#### Resources
Models 1Icons 4Maps 6Spells and Systems 7JASS 7Lua 1Tutorials 1

### Intro

### Encapsulation

### Credits

#

Introduction to Structs

(Part I)

(By MyPad)

 Hello reader. By having clicked this link here, I assume that you want to learn structs, right? Yes, you do, and what a wordy tutorial this would be.

 What we'll need to have is (either):

 1. JNGP or

 2. Sharpcraft WEX (for versions 1.24. - 1.29.2)

3. Vanilla World Editor (For 1.30+ above)​

 What this tutorial will cover:

 Part I - Intro

 - What is a Struct?

 - Declaration of (Independent) Structs

 - Members

 - Methods

 Part II - Encapsulation

 - History

 - Libraries

 - Scopes

#

I. What is a Struct?

 A struct is a collection of data fields or data types which acts as a data type itself.

 In other programming languages, a struct is called a class.

#

II. Declaration of (Independent) Structs

 Structs as defined above seem ambiguous to you. What does it actually mean? Let's just show a struct, since an example is worth a thousand words..

 JASS:

 struct someType
 integer count
 endstruct

 What did we declare here? We declared a struct in vJASS. (JASS does not support this, and vJASS parses a struct into JASS) (You'll know why later on)

 Note that to declare a struct (in vJASS), you will have to type it like this:

 JASS:

 struct <name_of_struct>
 endstruct

 ... and you're good to go.

#

III. Structs (Members)

 Notice that the example before had integer count inside the struct. Didn't notice?

 Well, that was a member of the struct.

 The integer count is a member of the struct someType or a data field in the struct. There are two kinds of members: static and normal

 Normal members are those that are present per instance.

 Static members are those that are reserved to the whole struct.

 What's their difference? Let's take a look:

 Normal members

 JASS:

 struct someType
 integer count
 endstruct

 ...

 local someType that = 3
 set that.count = 5

 Static members

 JASS:

 struct someType
 static integer count
 endstruct

 ...

 set someType.count = 5

 As you can see, instances or pointers of a struct can use members but they can't use static members because they're not the struct itself (E.g., they're only associated with the struct), while the struct-type (Let's call this struct-type) can use static members but not normal members. Hence, the syntax for using members and static members is:

 JASS:

 set <struct_instance>.<member> = <some_value>
 set <struct_type>.<static_member> = <some_value>

 You can also do this:

 JASS:

 set <struct_type>(<instance>).<member> = <some_value>

#

IV. Structs (Methods)

 Methods are the functions associated with the struct. They also come with two types: static and normal.

 The static method follows the logic of a static member while a normal method follows the logic of a member.

 Syntax:

 JASS:

 struct SomeType
 method foop takes nothing returns nothing
 call BJDebugMsg("The enemy of Foop(" + I2S(this) + ") is Poof(" + I2S(this) + ").")
 endmethod

 static method foo takes nothing returns nothing
 call DoNothing()
 call BJDebugMsg("You've been bamboozled")
 endmethod
 endstruct

 ...

 local SomeType fool = 5
 call SomeType.foo()
 call fool.foop()

#

V. Conclusion

I hope that you have learned something here. If not, just leave a reply here asking why you do not understand or you can send a query via PM. Who knows, I might add you to the credits list.

#

Encapsulation

(Part II)

 It looks like we meet again, fellow reader. Welcome to Part II of the tutorial. By clicking on this tab, I assume that you've already learned everything about Part I.

 So, let us begin!

# History

 vJASS was and still is the go-to scripting language for the majority of the community members. In essence, vJASS is how some compiler engineers or enthusiastic coders would have programmed the language, and is like the first-born child of JASS2 (figuratively speaking).

 Under all fanciful syntax and such, vJASS is an abstraction for JASS2, which then gets compiled to JASS2 (which shall be referred hereafter as JASS) through JassHelper. How JassHelper compiles vJASS to JASS is another topic altogether and shall be linked here:

# Coverage

 What this part will discuss:

 - Libraries

 - Scopes

 - Scopes within Libraries

 - Scopes within Scopes

# Pre-Discussion Questions

 Q: Wait, all this part is talking about isn't even about structs! Where's our structs?

 A: It will be covered, although structs are going to be talked about a little bit here.

# I. Libraries

 Libraries are containers of packets of code. They're like juice cartons, where the juice is the code and the carton is the enclosure, which is representative of a library.

 Libraries are very useful in keeping the code organized, or at least, the thought-process or the logic of the code. Libraries are what enforces modularity in writing code, but we'll deal with it later. To declare a library:

 JASS:

 library MyLibrary
 public function foo takes nothing returns nothing
 endfunction
 endlibrary

 One must declare the keyword "library" and assign a unique name to the library. Due to vJASS being based on JASS, one must also declare "endlibrary" at the end of the script. This becomes second-nature very quickly.

 If you can notice, a function foo was declared within the library. Now, what's with the prefix "public"?

 Let's see the compiled result:

 JASS:

// MyLibrary starts
function MyLibrary_foo takes nothing returns nothing
endfunction
// MyLibrary ends

 Looks like foo got prefixed with the name of the library. Neat'o, isn't it?

 From the example, we can see that the function that's prefixed with "public" inherits the name of the library as a prefix. Why is it so?

 JASS:

library MyLibrary1
 public function foo takes nothing returns nothing
 endfunction
endlibrary

library MyLibrary2
 public function foo takes nothing returns nothing
 endfunction
endlibrary

// ...
call foo() // error

 Now, let's look at the compiled result: (Order may vary, explanation later)

 JASS:

// MyLibrary1 begins

function MyLibrary1_foo takes nothing returns nothing
endfunction

// MyLibrary1 ends

// MyLibrary2 begins

function MyLibrary2_foo takes nothing returns nothing
endfunction

// MyLibrary2 ends

// ...
call foo() // error

 As you can see, libraries look like they are giving the packets of code within them their own space. This gives them a sense of organization. Remember that the functions were "public"? Well, it does look like the functions are given unique names, which means that you can keep using the same name of a function, as long as you add some encapsulation prefixes.

 The most commonly used prefixes for encapsulation are "public, private". These prefixes for encapsulation, these keywords, allow functions with the same name in compile-time to be differentiated from each other, as long as the library names are different.

 A simple explanation for "public" and "private" keywords are as follows; they behave just as their meaning implies.

 Now, the code within libraries is moved to the top, which allow functions below to call the library code. However, the compiler will mix the order of compilation of libraries, which could make for some unintended errors if trying to call functions from other libraries. How can we resolve this?

 JASS:

library MyLibrary1
endlibrary

library MyLibrary2 uses MyLibrary1 // keywords needs, requires are also usable here.
endlibrary

 So, what's with the keyword "uses"? The library doesn't seem to be doing anything, right?

 The keyword "uses" is instructing the compiler to put the desired library before the library itself.

 In another sense, it tells the compiler that that used library should be placed above it, since it may use its' features or functions.

 To make it more clear, an example of it is as follows:

 JASS:

// MyLibrary1 begins
// MyLibrary1 ends

// MyLibrary2 begins
// MyLibrary2 ends

So far, nothing has changed from our previous example except we took out some unnecessary functions. However, the main difference between the former and the latter is that the order is deterministic, which means in here that MyLibrary2 will always be below MyLibrary1 no matter what. This further organizes the script into the way that would seem most logical.​

# II. Scopes

Scopes are functionally similar to libraries, only that they are moved way below the library section. It would only make sense when it is understood in a sense that scopes are just the local functions relative to the map, while the libraries are global, being shared across maps. In another sense, they're only relevant to the map at hand.

 A good example of such scopes are spells. Spells do not need to be placed in the library section, since nothing from below is presumed to call them. They are, in a sense, reserved to themselves, and nothing should change that by calling them.

 While this is all fine and dandy, an example should best explain what happens.

Input:

 JASS:

library MyLibrary1
endlibrary

library MyLibrary2
endlibrary

scope MyScope
endscope

Output:

 JASS:

// MyLibrary2 begins
// MyLibrary2 ends

// MyLibrary1 begins
// MyLibrary1 ends

// MyScope begins
// MyScope ends

Don't let the scrambling of the order of libraries fool you. Look at MyScope. It is at the bottom of the libraries.

Let's try compiling it again.

Output:

 JASS:

// MyLibrary1 begins
// MyLibrary1 ends

// MyLibrary2 begins
// MyLibrary2 ends

// MyScope begins
// MyScope ends

Huh, looks like we encounter this case again. MyScope can't seem to go up.

It is rather bittersweet to know that scopes can never be above libraries, no matter how many times we compile them.

There is also another feature of scopes, the ability to enclose individual scopes within it. Basically, you can declare a scope inside a scope, and the same inside a library. But, what would happen if we place a library within a scope?

 JASS:

scope MyScope
 library MyScopeLib
 endlibrary
endscope
// Won't compile ...

Huh? It would not compile, no matter how many times you will save it. So, declaring a scope within a scope or a library is A-Okay but placing a library inside a scope is a no-no? Yes, it is, because of a simple reason, a scope can never be above libraries, and trying to move that library in the scope up would cause a violation of that rule.

​

# III. Recap

 To recap, libraries are containers that give code their own space (or something like it). They encapsulate the functions so as to add protection from unintentional reading or access. When using the keywords "uses, requires or needs", the library that requires the other library will be moved below it.

 Scopes are similar to libraries in that they give code their own space as well. In basic form, they are libraries that require all libraries before it, but not needing to declare them as requirements.

 Now, without compiling the code, would the following code compile without a trigger-evaluation (e.g. not compiling in the vanilla trigger editor)?

 JASS:

library MyLibrary2 uses MyLibrary1
 public struct Struct
 static method doElse takes nothing returns nothing
 endmethod
 endstruct
endlibrary

library MyLibrary1
 function doSomething takes nothing returns nothing
 call MyLibrary2_Struct.doElse()
 endfunction
endlibrary

 Or, say we have this. Would this compile?

 JASS:

library MyLibrary1
 private function foo takes nothing returns nothing
 call BJDebugMsg("boop")
 endfunction
endlibrary

library MyLibrary2 uses MyLibrary1
 private function bar takes nothing returns nothing
 call MyLibrary1_foo()
 endfunction
endlibrary

Cuore for asking questions and clarifications about the struct tutorial...

IcemanBo for providing links to other tutorials...

Structs for Dummies

vJASS OOP Lesson

Introduction to Struct

Thanks for taking your time to read this tutorial!


 Last edited: Jul 7, 2018

#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

 In vJass, structs are actually integers


Click to expand...

 JASS:

struct Data2
 unit whichUnit
 real whichLife
endstruct

function SomeInitFunc takes nothing returns nothing
 local Data2 this = 1
 loop
 exitwhen this > 5
 set this.whichUnit = <some_random_unit>
 set this.whichLife = GetWidgetLife(<some_random_unit>)
 set this = this + 1
 endloop
endfunction

Would've been nice if structs were a bit more type safe, i.e requiring explicit a type casts (integer(foo), or Bar(foo)) though, because that catches mistakes, I think.

 JASS:

struct Foo
 string a
endstruct

struct Bar
 integer b
endstruct

function main takes nothing returns nothing
 local Foo foo = Foo.create()
 local Bar bar = foo // no error
 // =>
 // local integer foo= s__Foo__allocate()
 // local integer bar= foo

endfunction

Might be worth mentioning that static members can be accessed from a struct instance and the "struct name" itself:

 JASS:

struct Foo
 string a
 static string b

 static method bar takes nothing returns nothing
 endmethod
endstruct

function main takes nothing returns nothing
 local Foo foo = Foo.create()

 call foo.bar() // ok
 call Foo.bar() // ok
 call BJDebugMsg(foo.a) // ok
 call BJDebugMsg(foo.b) // ok
 // call BJDebugMsg(Foo.a) // error: a not a static member of Foo
 call BJDebugMsg(Foo.b) // ok
 //
 // =>
 //
 // local integer foo= s__Foo__allocate()
 //
 // call s__Foo_bar() // ok
 // call s__Foo_bar() // ok
 //
 // call BJDebugMsg(s__Foo_a[foo]) // ok
 // call BJDebugMsg(s__Foo_b) // ok
 // // call BJDebugMsg(Foo.a) // // error: a not a static member of Foo
 // call BJDebugMsg(s__Foo_b) // ok
 //
 // its funny how vJass preserves comments =)
 //
endfunction

Oh, you have it at the bottom but it seems a bit convoluted.

PS: You could also have an explanation of why one would bother to learn about structs in the first place instead of sticking to GUI, "dynamic indexing", hashtables, etc.

PPS: structs are more readable, efficient, composable (and type unsafe ;P)

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Seems like something you'd need prior programming to understand, it does not feel very beginner friendly to me.

I personally would prefer the existing struct tutorials.

#### MyPad

#### MyPad

 Level 31

JoinedMay 9, 2014Messages1,878

Resources: 27

#### Resources
Models 1Icons 4Maps 6Spells and Systems 7JASS 7Lua 1Tutorials 1

Woah, comments. This is the first tutorial I've written. I'll polish it up though.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I apologize for the delay. I like this tutorial. In fact, when I was first transitioning to vJASS, I used a similar "light" tutorial and learned from there. Sometimes it is easier to just see how structs are used in a basic sense rather than going into the nitty gritty of trying to figure out how they work internally.

Keep it up. I definitely would delve into creating struct instances in Part II.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Any plan to update? @MyPad

#### MyPad

#### MyPad

 Level 31

JoinedMay 9, 2014Messages1,878

Resources: 27

#### Resources
Models 1Icons 4Maps 6Spells and Systems 7JASS 7Lua 1Tutorials 1

I've been busy with my entry in the Techtree Contest. I'll see if I come around.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Would be good to have. A focus would be definitly useful, for not to cover all points from vjass, but maybe seperating it into packs/different tutorials.

@BlueSaint made a good but also quite big tutorial, but did not fully finish it because it's just too big to overview and to maintain.

References can also be Structs for Dummies, vJASS OOP Lesson, Introduction to Struct, to be aware of what kind of learn attempt it will be.

Personally I would also love to have a basic crash course/introduction to have combined with little tasks, like maybe extending the Jass Class with vjass, to have theory and practice together. Tutorial(s) are for sure useful too, but I'm just sceptical if it covers too much from all features in one.

#### MrModify

#### MrModify

 Level 4

JoinedMar 4, 2020Messages21

Resources: 1

#### Resources
Tutorials 1

Great job thanks

#### Ouroel

#### Ouroel

 Level 3

JoinedJul 12, 2020Messages12

Resources: 1

#### Resources
Tutorials 1

Someone conspicuously forgot that you can only have 8192 instances of a struct. Less if your struct contains 2 of another type of struct.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Wasnt that limit changed in a patch a while back?

(I seem to remember the array limit being increased)

#### Ouroel

#### Ouroel

 Level 3

JoinedJul 12, 2020Messages12

Resources: 1

#### Resources
Tutorials 1

oh yeah it's 32768 now

still way too little to store the entire navgrid though for physics collision purposes

#### MyPad

#### MyPad

 Level 31

JoinedMay 9, 2014Messages1,878

Resources: 27

#### Resources
Models 1Icons 4Maps 6Spells and Systems 7JASS 7Lua 1Tutorials 1

If arrays won't do, then hashtables probably will. As a sacrifice for speed, you get a lot of space.

#### Ouroel

#### Ouroel

 Level 3

JoinedJul 12, 2020Messages12

Resources: 1

#### Resources
Tutorials 1

wasn't there a 20 hashtable limit?

srsly just use Lua

(also structs are array-based)

#### MyPad

#### MyPad

 Level 31

JoinedMay 9, 2014Messages1,878

Resources: 27

#### Resources
Models 1Icons 4Maps 6Spells and Systems 7JASS 7Lua 1Tutorials 1

The limit to the number of hashtables is actually around 255-256.

#### Ouroel

#### Ouroel

 Level 3

JoinedJul 12, 2020Messages12

Resources: 1

#### Resources
Tutorials 1

not sure on the speed of lua tables vs hashtables though

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

@MyPad plans to update this?

And if you do, I figure there would need to be a motivation of what this does better than other options that already exist that cover the same topic.

#### MyPad

#### MyPad

 Level 31

JoinedMay 9, 2014Messages1,878

Resources: 27

#### Resources
Models 1Icons 4Maps 6Spells and Systems 7JASS 7Lua 1Tutorials 1

Probably not. Graveyard away

 You must log in or register to reply here.


### Similar threads

 vJass Indexer Tutorial - Structs


Dynasti

Jan 18, 2009

Tutorial Archive

Replies4Views8K

Jan 20, 2009
Dynasti

Locked

 [vJASS] Why doesn't this library see my struct?


sethmachine

Jul 28, 2014

World Editor Help Zone

Replies5Views2K

Jul 29, 2014
Bannar

Locked

 vJASS - Initialisers


Mr_Bean

Jun 4, 2013

World Editor Help Zone

Replies4Views2K

Jun 5, 2013
Mr_Bean

Locked

[Solved]
 Issue with statics and struct children


Anachron

Apr 11, 2017

Triggers & Scripts

Replies0Views1K

Apr 11, 2017
Anachron

Locked

 Struct member (method chaining)


Arathor

Jan 14, 2022

World Editor Help Zone

Replies3Views1K

Jan 15, 2022
HerlySQR

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