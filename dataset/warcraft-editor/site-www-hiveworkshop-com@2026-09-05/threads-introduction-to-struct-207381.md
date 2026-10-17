---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/introduction-to-struct.207381/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Introduction to Struct
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/introduction-to-struct.207381/>
#### iAyanami

#### iAyanami

 Level 9

JoinedDec 3, 2010Messages162

Resources: 13

#### Resources
Spells and Systems 11JASS 1Tutorials 1

Table of Contents

Introduction

Concept of Instances

Struct Declaration

Struct Instantiation

Constructor Overloading

Multiple Constructors

Members

Methods

Static vs Instance

thistype

Destroying Instances & onDestroy

onInit Method

Naming Conventions

Closing

FAQ

1. Introduction


Structs are very powerful and should be learned as soon as possible if you're planning to use vJASS. This tutorial assumes that you have general knowledge of JASS. If you have ever taken other Programming Languages, a Struct is more commonly known as a Class. You must understand that a Struct generally represents an object. This object can represent anything you want (ex: Car, Book, Person, etc). This object would then be a variable type that you can use (like integers, reals, strings, etc).

 JASS:

// struct declaration, will go through this later on
struct Person
endstruct

function Test takes nothing returns nothing
 local real r
 local Person p // you can treat structs as a variable type
endfunction

Summary: Structs are objects and they can be treated like variable types.

2. Concept of Instances


The next important concept about Struct is the various instances that you can have inside the Struct. Instances are exactly what they sound like. In a struct, you can have many instances created inside of it.


I'll elaborate this with an example. Let's say you have a Struct called Bicycle. So basically, this would mean that you could have multiple bicycles inside the Struct. Let's say the Struct has 2 instances instantiated. This means that there are currently 2 bicycles that are present under the Struct Bicycle. You can also imagine Struct instances as variable array indices. For example, you can have Bicycle[1] and Bicycle[2].


There is a limit for number of instances in a single struct. The magical number is 8190, remember this number. Once you pass this limit, your struct will basically go haywire from that point.

Summary: Instances are basically the number of objects that exist in the struct. They are basically the array indices of an array. The instance limit for a struct is 8190.

3. Struct Declaration


To use a struct, it must obviously be declared. It's very simple to declare a struct. The format is: struct<name>. Pretty simple, right? Do note that you need the endstruct to close the struct, much like a function.

 JASS:

struct A
endstruct


The above example basically declares a struct called "A".

Summary: Format for struct declaration is: struct<name> followed by endstruct

4. Struct Instantiation


So a struct can have multiple instances. Thus, you need to know how to create an instance. A special function exists to create an instance for you. This is known as a constructor. A constructor's job is to basically create a new instance for the user. The format for using the constructor is <struct name>.create(). This calls the constructor which returns the created instance:

 JASS:

struct Bicycle
endstruct

private function Test takes nothing returns nothing
 local Bicycle d = Bicycle.create()
endfunction


As shown above, .create() is the syntax to create a new instance. Since you're trying to create a instance of struct Bicycle, you need to specify the struct name, this Bicycle.create(). The constructor always returns the struct type. It's quite obvious; you need to store the created instance. What's the point of creating an instance and not storing the instance anywhere, right? So basically, your variable d holds an instance of the Struct type Bicycle.


Instances are actually integers. If you were to do:

 JASS:

struct Bicycle
endstruct

private function Test takes nothing returns nothing
 local Bicycle b = Bicycle.create()

 call BJDebugMsg(I2S(b)) // this is valid and would print an integer
endfunction


The maximum struct instances you can have is 8190, like stated before. Just remember that struct instances are integers.

Summary: To instantiate (create) an instance, you need to call the constructor, which has the format: <struct name>.create(). Instances are actually integers, just like array indices.

5. Constructor Overloading


So now you're able to instantiate instances. However, what if we want to take parameters for our constructor? You can override the default constructor, the .create() method.


Note that a function inside of a struct is called a method. As for the static part, you can ignore it for now. I'll be covering that in a while.

 JASS:

struct Bicycle
 public static method create takes unit u returns Bicycle
 local Bicycle this = Bicycle.allocate()

 call BJDebugMsg(GetUnitName(u))

 return this
 endmethod
endstruct

private function Test takes nothing returns nothing
 local Bicycle b = Bicycle.create(SomeUnit)
endfunction


Notice that .create takes a unit now? You've successfully overloaded the constructor. Basically, now I've customized the constructor to take a unit as an argument and print that unit's name. Then, return created instance. The .allocate() is always needed when you want to instantiate an instance. Remember to include that inside your custom constructor.


A custom constructor only has 2 rules. Firstly, it needs to be a static method. I'll be covering this later on. Secondly, it needs to always return the struct type. If you happen to do things like returns nothing, it'll give you a compilation error. However, doing returns integer would be valid, as instances are actually integers, which I mentioned previously.


If a custom constructor is not declared, a default constructor will be automatically declared for you, which has no parameter.

Summary: A custom constructor can be used to take parameters. However, constructors must be static and must returns the struct type (or integer). A default constructor is automatically created if no custom constructor is declared, which has no parameters.

6. Multiple Constructors


It is possible to have many constructors.

 JASS:

struct A
 public static method new takes nothing returns A
 local A this = A.allocate()

 return this
 endmethod
endstruct

private function Test takes nothing returns nothing
 local A a = A.create() // works
 local A b = A.new() // would work as well
endfunction


So what's the point of having multiple constructors? Well here's the thing: as long as your constructor isn't called .create(), you can choose to returns nothing. However, that would kind of defeat the purpose of a constructor and it wouldn't really be called a constructor anymore.


The main reason why we might want multiple constructors is that you might need different constructors for different purposes. For example, you might need a constructor with no parameters sometimes and you might need a constructor with parameters another time. Just note that you always need the .allocate() method to instantiate an instance, it should always be inside your constructor.

Summary: You can have multiple constructors as long as they are static methods. As long as your constructor isn't called .create(), you can actually return any type of variable or return nothing at all. Most of the times, you only need a single constructor.

7. Members


If methods are equivalents of functions, members are equivalents of variables. Basically, members are variables inside of a struct.

 JASS:

struct A
 real r
 integer i
 boolean b
 static unit u // yes, there are static members as well, I'll be covering that soon
endstruct


Members can also have access modifiers. What do I mean by this? Simply put, they can have the private, public and readonly modifiers.

private members are basically only, and only accessible inside the struct. public members are accessible by things outside the struct. By default, members are public if you do not add any modifier. readonly members are kind of special. The user can access this variable outside of the struct, however the user is not able to modify it. You're only able to "get" the value. They can only be modified from inside the struct.

 JASS:

struct A
 private static integer a = 0
 public static integer b = 0
 static integer c = 0
 readonly static integer d = 0
endstruct

private function Test takes nothing returns nothing
 set A.a = 1 // ignore the A. for now. Not legal, as "a" is private
 call BJDebugMsg(I2S(A.a)) // still not legal, as "a" is private

 set A.b = 1 // legal, it is public
 call BJDebugMsg(I2S(A.b)) // legal as well, it is public

 set A.c = 1 // legal, "c" is automatically defined as public since no modifier was given
 call BJDebugMsg(I2S(A.c)) // legal as well, it is public

 set A.d = 1 // not legal, as "d" is readonly
 call BJDebugMsg(I2S(A.d)) // legal, since readonly allows you to "get" the value
endfunction

Summary: Members are variables inside a struct. There are three types of access modifiers for members: private, public and readonly. A member with no modifier is automatically a public member.

8. Methods


I've already mentioned that methods are basically functions in struct.

 JASS:

struct A
 function Catastrophic takes nothing returns nothing // compiler screams at you for doing this, you must use methods
 endfunction
endstruct


Methods are exactly same as functions. You have parameters and a return type. All you need to change is the function keyword to method. Methods can be static as well. I'll be covering that the next section (promise).


Methods, can have access modifiers as well. There are only 2 modifiers for methods though, private and public. private methods can be accessed only inside the struct. public methods can be accessed by things outside of the struct. A method without a specified modifier becomes automatically public. It's the exactly same like member modifiers.

Summary: Methods are functions inside a struct. There are two types of access modifiers for members: private and public. A member with no modifier is automatically a public member.

9. Static vs Instance


So you've been seeing the keyword static all over the place. I'm going to explain the different between static and instance.


In a struct, you can have static and instance (non-static) methods and members. Basically, instance members and methods are associated with an instance. Let's look at instance members first:

 JASS:

struct Bicycle
 real speed // this is public as no access modifier is specified
endstruct

private function Test takes nothing returns nothing
 local Bicycle a = Bicycle.create() // creating an instance of Bicycle
 local Bicycle b = Bicycle.create() // creating another instance of Bicycle

 set a.speed = 10.0
 set b.speed = 30.0
endfunction


The above demonstrates instance members. Basically, the member speed can be associated with instances and each instanced speed can have different values. You can imagine them as arrays:

 JASS:

struct Bicycle
 real speed
endstruct

globals
 private real array SpeedArray
endglobals

private function Test takes nothing returns nothing
 local Bicycle a = Bicycle.create() // creating an instance of Bicycle
 local Bicycle b = Bicycle.create() // creating another instance of Bicycle

 set a.speed = 10.0
 set b.speed = 30.0

 set SpeedArray[0] = 10.0 // notice the similarities?
 set SpeedArray[1] = 30.0
endfunction


So basically, you can imagine instances a and b to be the array indices 0 and 1. This is how an instance member works.


Instance methods are similar to instance members, they are associated with an instance.

 JASS:

struct Person
 string name // instance member

 public method displayName takes nothing returns nothing
 call BJDebugMsg("This person's name is: " + this.name)
 endmethod
endstruct

private function Test takes nothing returns nothing
 local Person a = Person.create()
 local Person b = Person.create()

 set a.name = "John" // setting Person a's name as John
 set b.name = "Mary" // setting Person b's name as Mary

 call a.displayName() // almost like a function call, except you associate an instance with it by using a.
 call b.displayName()

 // the result would be:
 //
 // This person's name is John
 // This person's name is Mary
endfunction


So basically, the instance is passed into the method. You might be wondering what is the this keyword for. Basically, this inside a instance method refers to the instance that you passed in. So you call a.displayName(), the instance a would be passed into the method displayName. Now you need a way to refer to the instance a inside of the method. Thus, we use the this keyword to refer to that instance inside of an instance method. You can imagine the above as such:

 JASS:

globals
 private string array Name
endglobals

private function DisplayName takes integer this returns nothing
 call BJDebugMsg("The person's name is: " + Name[this])
endfunction

private function Test takes nothing returns nothing
 set Name[0] = "John"
 set Name[1] = "Mary"

 call DisplayName(0)
 call DisplayName(1)
endfunction


The above code is basically to illustrate what instance methods do.


The this keyword can be omitted and simply just put as .

 JASS:

struct Person
 string name

 public method displayName takes nothing returns nothing
 call BJDebugMsg("The person's name is: " + .name) // you can do .name instead of this.name
 call BJDebugMsg("The person's name is: " + name) // you can also simply refer it as name instead of .name or this.name
 endmethod
endstruct


The static keyword is the opposite of instance keyword. A static member and method does not associate any instances. Let's touch on static members first. Static members are simply global variables inside of a struct. They are not associated with an instance.

 JASS:

struct Person
 static integer population
endstruct

private function Test takes nothing returns nothing
 set Person.population = 10 // setting a static integer to 10, just like a global

 // notice the Person. in front. This basically is saying that you're trying to access a static member in struct Person
 // whenever you want to use a static member outside of the struct, you need to include the struct name in front followed by a .
 // this can be omitted inside of the struct itself, but it's good practice to always include it, even inside of the struct
endfunction


Essentially, static members are global variables inside of a struct. They have no instance associated, thus they have a single value for one struct, unlike instance members.


Static methods are simply your normal functions inside of a struct.

 JASS:

struct Person
 public static method displayHello takes nothing returns nothing
 call BJDebugMsg("Hello!")
 endmethod
endstruct

private function Test takes nothing returns nothing
 call Person.displayHello() // notice the Person. again, this is the rule to call static methods
endfunction


This summarizes the difference between static and instance (non-static).

Summary: You can have Static and Instance methods and members. Instance methods and members are associated by an instance. Inside an instance method, you use the this keyword to refer to the instance that was passed in to the method. Static methods and members are not associated with an instance, and act like global variables and normal functions inside of a struct.

10. thistype


The thistype keyword is only used inside of a struct. It's basically a keyword to refer to the struct name.

 JASS:

struct A
 public static method create takes nothing returns thistype // instead of saying "return A", we can just say "return thistype"
 local thistype this = thistype.create() // you can do this instead of doing local A this = A.create()

 return this // notice I used "this". Basically, in a static method, you can use "this" as a variable name.
 endmethod
endstruct


Thus, when you use static methods from within a struct, you can do thistype.<static method name instead of <struct name><static method name. This is also same for static members. But remember, you can only use thistype inside of a struct.

Summary: Instead of using the struct name, you can use thistype to refer to the struct name. This only applies inside of a struct.

11. Destroying Instances & onDestroy


I mentioned that struct have a limit of 8190 instances. This would mean we would hit the limit eventually if we continuously create instances. Thus, we need to destroy instances when we don't need them anymore.

 JASS:

struct Bicycle
endstruct

private function Test takes nothing returns nothing
 local Bicycle d = Bicycle.create() // creating an instance

 // other actions...

 call d.destroy() // destroying the instance
endfunction


You would have noticed that you don't do Bicycle.destroy(). This is because Bicycle is the struct itself. When you destroy something, you want to destroy the instance of Bicycle, not the struct itself. Thus, it's d.destroy(). The variable d holds the instance, which is the reason why you do d.destroy(). That's the syntax to destroy.


Similar to .create(), you can overload the .destroy() method. However, remember that the destructor (.destroy()) is an instance method, not a static method. However, you can also have a static destructor as well. Destructor doesn't have any parameter or return specifications. You need to remember to include a line though, if you happen to override the destructor or define a custom destructor; this.deallocate().

 JASS:

struct Bicycle
 public method destroy takes nothing returns nothing
 call this.deallocate() // the line of code that you NEED for any deconstructor
 endmethod
endstruct


There's also something known as the onDestroy. This is a special method that is ran right before an instance is destroyed:

 JASS:

struct Bicycle
 string name

 private method onDestroy takes nothing returns nothing
 call BJDebugMsg(name + " is being removed!")
 endmethod
endstruct


The above method is ran every time when an instance is about to be destroyed. However, it's better off overriding the destructor directly rather than using onDestroy for most situations. onDestroy has to take no parameters and return nothing. It also has to be an instance method.


Remember to always destroy your instances after you're done with them, or else it will be catastrophic once you hit the instance limit.

Summary: You can destroy instances by using the .destroy() method. If onDestroy is declared, that method will run right before an instance is destroyed. It has to be an instance method as well as take no parameter and no return.

12. onInit Method


There's also another special method called the onInit method. This is an initialization method. Simply put, this method runs right when the game starts. The onInit method must be static and cannot take parameters.

 JASS:

struct A
 private static method onInit takes nothing returns nothing
 call BJDebugMsg("Hello!") // would print "Hello!" right after the loading screen; when game starts
 endmethod
endstruct


Another thing to take note about onInit is that it is ran before scope/library initializers.

 JASS:

struct MyStruct
 private static method onInit takes nothing returns nothing
 call BJDebugMsg("Struct says hello!")
 endmethod
endstruct

library MyLib initializer OnInit
 private function OnInit takes nothing returns nothing
 call BJDebugMsg("Library says hello!")
 endfunction
endlibrary

// the above would print:
//
// Struct says hello!
// Library says hello!


From the above, you can see that struct onInit is always ran before any library/scope initializers.

13. Naming Conventions


There are naming conventions for members, methods and struct name. These conventions are generally followed by most people. For more information, you can take a look at this.

 JASS:

struct ThisIsMyName // struct names in ProperCase

method thisIsMyMethod // method names in camelCase

unit thisIsMyUnit // member names in camelCase

14. Closing


I hope this helped all of you who are struggling to grasp the concept of struct. I hope you will be appreciate struct after reading this tutorial. If you have any questions, feel free to ask, and I'll add the important ones in the FAQ section.

15. FAQ


 Last edited by a moderator: Mar 20, 2012

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Nice approach and formatting.

What is the point of the default create/destroy/onDestroy methods anyway? I would never ever want to have an empty (thus probably unfunctional) instance allocated outside of the struct. That speaks against protection. Writing your own constructors/deconstructors is simple, done fast, clear and you have precise controls.

Methods are not denoted as members?

Do you keep this lean or do more concepts get added like inheritance, array structs, delegates...?

#### iAyanami

#### iAyanami

 Level 9

JoinedDec 3, 2010Messages162

Resources: 13

#### Resources
Spells and Systems 11JASS 1Tutorials 1

WaterKnight said:

 Nice approach and formatting.


What is the point of the default create/destroy/onDestroy methods anyway? I would never ever want to have an empty (thus probably unfunctional) instance allocated outside of the struct. That speaks against protection. Writing your own constructors/deconstructors is simple, done fast, clear and you have precise controls.


Methods are not denoted as members?


Do you keep this lean or do more concepts get added like inheritance, array structs, delegates...?


Click to expand...


Hmm well, in the vJASS manual, seems like the instance and static variables were referred as "members". Thus, I followed that naming method.


Well, it is pretty much not practical to have a default create/destroy/onDestroy method. I'm just pointing it out here because this is supposed to help fresh beginners understand the concept of struct. Most probably I won't be adding other concepts, as this is just a basic introduction to structs.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

You don't have to include naming conventions, you can link to this instead.

Anyways, this tutorial looks good, but you have to discourage the use of the onDestroy method.


 Last edited: Nov 23, 2011

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

He does discourage its use although it could be better defined why it is discouraged (copying of the method, running it through a trigger condition instead of simply calling it, etc).

#### fakedrake

#### fakedrake

 Level 3

JoinedApr 5, 2010Messages31

Resources: 1

#### Resources
Maps 1

Great tutorial, I learned a lot. thx ^^

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Hmm... I didn't approve this already?

It is in an approvable state, sorry for the wait Ayanami. ~Approved.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

The information on onDestroy is incorrect.


onDestroy is used for polymorphism and is evaluated in a trigger. It is called whether it exists or not if the struct is extended by another struct.


onDestroy is called within the deallocate method, which is in turn called by the destroy method.


destroy -> deallocate -> onDestroy


Destroy is not called properly in the case of extending structs.


struct A


struct B


b.destroy() -> b.deallocate() -> a.onDestroy -> b.onDestroy()


notice that a.destroy is never called. This is because it is not in a trigger.


If you really need polymorphism in vjass, it is going to cost you in performance. If you can get away without doing it, then you can just as easily chain the destroys.


Keep in mind that if you extend off of structs at all, even if you don't declare onDestroy and never call deallocate, the onDestroy triggers will still be generated and will still be evaluated in the deallocate method.


Also note that if you destroy a struct allocated as B but stored as A, it will run like this


a.destroy() -> a.deallocate() -> a.onDestroy -> b.onDestroy()


The reason for the order of the onDestroy is because of the allocation method, when the trigger is built.


b.create() -> b.allocate() -> a.allocate() -> register a -> return to b -> register b


b gets its instance from a, meaning a gets its onDestroy registered before b does

edit


the thing on methods being functions is also wrong. It shows a method as

 JASS:

function Eh takes nothing returns nothing
endfunction


When it should be

 JASS:

function Eh takes integer this returns nothing
endfunction


Is

 JASS:

method Eh takes nothing returns nothing
endmethod


And

 JASS:

function Eh takes nothing returns nothing
endfunction


Is

 JASS:

static method Eh takes nothing returns nothing
endmethod

edit


The onInit section is misleading. Using cohador's version, a struct's onInit runs in the order that they are found in a given library in the order of libraries found followed by scopes/general structs found.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nestharus said:

 The information on onDestroy is incorrect.


onDestroy is used for polymorphism and is evaluated in a trigger. It is called whether it exists or not if the struct is extended by another struct.


onDestroy is called within the deallocate method, which is in turn called by the destroy method.


destroy -> deallocate -> onDestroy


Destroy is not called properly in the case of extending structs.


struct A


struct B


b.destroy() -> b.deallocate() -> a.onDestroy -> b.onDestroy()


notice that a.destroy is never called. This is because it is not in a trigger.


If you really need polymorphism in vjass, it is going to cost you in performance. If you can get away without doing it, then you can just as easily chain the destroys.


Keep in mind that if you extend off of structs at all, even if you don't declare onDestroy and never call deallocate, the onDestroy triggers will still be generated and will still be evaluated in the deallocate method.


Also note that if you destroy a struct allocated as B but stored as A, it will run like this


a.destroy() -> a.deallocate() -> a.onDestroy -> b.onDestroy()


The reason for the order of the onDestroy is because of the allocation method, when the trigger is built.


b.create() -> b.allocate() -> a.allocate() -> register a -> return to b -> register b


b gets its instance from a, meaning a gets its onDestroy registered before b does


Click to expand...


I suppose if Ayanami wants to clarify this then it would be better. However, the actual "uses" for onDestroy (in terms of polymorphism) are somewhat advanced for the average struct learner. I remember when I first learned structs, I learned more from a tutorial called "vJASS lite" that gave a simple explanation of each concept + an example rather than a detailed explanation. If he were to give a long, detailed explanation, it would most likely lead to confusion.


Although, if he has the order incorrect then he should fix that.

edit


the thing on methods being functions is also wrong. It shows a method as

 JASS:

function Eh takes nothing returns nothing
endfunction


When it should be

 JASS:

function Eh takes integer this returns nothing
endfunction


Is

 JASS:

method Eh takes nothing returns nothing
endmethod


And

 JASS:

function Eh takes nothing returns nothing
endfunction


Is

 JASS:

static method Eh takes nothing returns nothing
endmethod

Click to expand...


He goes over that in static vs. instanced methods. He just represents it as a function in the introduction as a visual.


IMO it should be left that way, unless he says "ignore the integer this part until the next section". But if he does that, the tutorial loses its fluidity.


As long as the reader doesn't skip the static vs. instance part, they should come out with the knowledge that instanced (normal) methods take an integer "this" while statics do not, so overall no harm is done.

edit


The onInit section is misleading. Using cohador's version, a struct's onInit runs in the order that they are found in a given library in the order of libraries found followed by scopes/general structs found.[/QUOTE]


This was written before cohadar made that fix, but I suppose it could be mentioned in there.


I'll let Ayanami know to look over your comment, thanks for the input. I will leave it in approval for my reasoning above, but if Ayanami doesn't respond I'll move it back or make the updates myself. (since I like this tutorial)

#### iAyanami

#### iAyanami

 Level 9

JoinedDec 3, 2010Messages162

Resources: 13

#### Resources
Spells and Systems 11JASS 1Tutorials 1

This tutorial is catered for people who have none or very little knowledge of Structs. I personally think it would be better to leave out the details and focus on the concepts.


So now onInit works based on the order of library found? Since when was this changed?

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

since cohadar


 Last edited: Apr 21, 2012

#### iAyanami

#### iAyanami

 Level 9

JoinedDec 3, 2010Messages162

Resources: 13

#### Resources
Spells and Systems 11JASS 1Tutorials 1

Nestharus said:

 since cohador


Click to expand...

When was it? Sorry, kinda outdated here x.x

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

Here or there.

Btw, it's "cohadar".

EDIT : But "cohador" seems more sexy, like a warrior.


 Last edited: Apr 21, 2012

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

oh my, this tut is so much more understandable than Maghter's *_*

thnks for the tut

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

WHY DOES EVERYONE WRITE MAGHTER and not MAGTHER. WHY. WHY. WHY WHY WHY.

And yes, mine is terrible.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

lol I'm sorry for that

and yes, your tut is too out-leveled my brain

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

@Maggy - almost same question I had when people spell my name as Adikutz

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Magtheridon96 said:

 WHY DOES EVERYONE WRITE MAGHTER and not MAGTHER. WHY. WHY. WHY WHY WHY.


Click to expand...


I still don't understand using Magther to shorten the name. Why not Mag? Magther just sounds weird. It is just a weird cutoff point. It would be like calling me "PurgeAnd" (which did happen before).

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

What are you talking about PurgeandFi?

<:

#### muzzel

#### muzzel

 Level 14

JoinedJun 27, 2008Messages1,322

Resources: 2

#### Resources
JASS 1Wurst 1

I think we can agree to call him Meg?

 You must log in or register to reply here.


### Similar threads

 Structs for Dummies


Magtheridon96

Jun 15, 2011

JASS Tutorials

Replies19Views26K

May 3, 2016
Zwiebelchen

 [vJass] Meet vJass - Extending structs


BPower

Jul 15, 2015

JASS Tutorials

Replies29Views18K

Sep 1, 2018
Bribe

 vJASS Spell Templates


Magtheridon96

May 13, 2012

JASS Tutorials

Replies32Views19K

Mar 29, 2016
PurgeandFire

Locked

[vJASS]
 Struct extension constructor problem


Wietlol

Aug 18, 2016

Triggers & Scripts

Replies12Views3K

Aug 22, 2016
LeP

 [vJASS] Dynamic Indexing Tutorial


Malhorne

Dec 25, 2013

JASS Tutorials
2

Replies58Views16K

Jul 13, 2015
Zwiebelchen

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

JASS Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top