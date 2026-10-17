---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/vjass-oop-lesson.128236/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# vJASS Oop Lesson
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/vjass-oop-lesson.128236/>
#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

#
 vJASS OOP Lesson


One of the most important things to JASS that vJASS introduces is the ability to use Object-Oriented Programming (OOP) features. The most basic OOP feature that vJASS offers is the struct, the equivalent of a class or an object in other languages (such as Java or Python). For the most part, I've seen people use structs as basically data packages: an easy way to store abstract data types since all structs can be treated as integers no matter what data is contained within them. While this nice perk is undoubtedly an invaluable asset, it's only the beginning of how useful structs really are. And that's where this tutorial comes in; it will (hopefully) teach you about the OOP concepts in vJASS so that you may exploit it to make your coding easier in the future.

IMPORTANT: This tutorial assumes that you have background in normal JASS (not vJASS) programming.

##
 Table of Contents

Debunking Some Myths

Struct Instantiation and Instance Members

Destroying Structs and onDestroy()

Static Members

Encapsulation

Interfaces

Struct Inheritance

Merging Inheritance with Interfaces

Overriding Methods

super

typeid and getType()

Function Objects

Function Interfaces

Glossary of Terms

Appendix
Use of Implicit this

Use of Implicit Struct Type Names

Use of this as a Local Variable Identifier

Struct Storage Size

Credits

##
 Debunking Some Myths


I've come across many people who have the idea that OOP somehow makes their code run more efficiently at runtime, and this is why they should use OOP. Not true. Perhaps my CS professor said it best: "If you ask people why OOP is useful, most will say things about inheritance and polymorphism and stuff like that, but in the end it's really just about making programming code easier." Using OOP doesn't necessarily make your code run faster or allow you to anything that couldn't be done using...say...procedural programming. In fact, vJASS is simply another layer of abstraction built on top of JASS, which is a procedural scripting language, so obviously JASS is capable of doing everything vJASS can do. It's just that vJASS lets you do certain things with 50 lines of code while JASS would require several hundred.

##
 Struct Instantiation and Instance Members


Structs are the heart of vJASS's OOP features. Since learning by example is usually a good way to go, here's a simple struct:

 JASS:

struct Hero
endstruct


Indeed, this is about as simple as you can get. Here we see the syntax for declaring a struct type, often simply referred to as simply a struct. The above struct has a name: Hero. Therefore we can say that the above struct is a struct of type Hero, or a struct called Hero.


The basic concept behind structs is that they represent some type of object. These objects can be closely related to physical or game objects, or they can be more abstract in nature. Our example struct, Hero, is supposed to represent...what else?...a hero!


An important property of structs is that they define a data type. Just as integer, real, unit, and location are all data types in JASS, struct types are also data types. Therefore you can do things like:

 JASS:

function Locals takes nothing returns nothing
 local Hero hero // this variable is now able to store a Vector
endfunction

function Parameters takes Hero h returns nothing
 // this function can now take a parameter of type Hero
endfunction

function Returning takes nothing returns Hero
 // this function returns a Hero
endfunction


Besides its own struct type, struct instances can also be treated as integers. So any variable of type integer can reference any struct instance, and any function returning an integer can return a reference to a struct instance.


Naturally, this leads to the question: How can a Hero be stored in a variable, or passed to a function, or returned by one?


The answer is that structs can be created, or instantiated. Instantiation of a struct simply means creating a new copy, or instance, of the struct in memory. This is analogous to creating new units or locations, for example, in normal JASS.


So how are structs instantiated? We can do it by calling [struct name].create(). Doing so returns a new instance of the struct that can be saved in a variable to be referenced for future use. For example:

 JASS:

function Instantiation takes nothing returns nothing
 local Hero hero = Hero.create() // hero now references an instance of Hero
endfunction


What exactly happens when a struct is instantiated? A special function of sorts is actually called. This function is called the struct's constructor. Normally, all the function does is create and return a new instance of a struct, but in fact we can define our own constructor inside of a struct.

 JASS:

struct Hero
 static method create takes player p, real x, real y returns Hero
 local Hero new = Hero.allocate()
 call CreateUnit(p, 'H000', x, y, 270.0)
 return new
 endmethod
endstruct


We have expanded Hero to include a user-defined constructor as well as some other things. The signature of the constructor is:

static method create takes player p, real x, real y returns Hero


A constructor may take any parameters (in the case of Hero it takes a player and two reals), but its return type must be the struct type that it belongs to (in this case: Hero). By convention the constructor of a struct is always named create() but this is not strictly enforced.


A constructor also must always make a call to [struct name].allocate(). It is this call that will allocate a new instance of the struct.


If you define your own constructor for a struct, then every time you call the constructor you must make sure to pass the correct parameters to it:

 JASS:

function Instantiation takes player p, real x, real y returns nothing
 local Hero heroOne = Hero.create(p, x, y)
 local Hero heroTwo = Hero.create() // does not work anymore
endfunction


That's enough about constructors for now.


The constructor for Hero creates a unit. It would be nice if we could keep track of that unit. And it turns out we can:

 JASS:

struct Hero
 unit u

 static method create takes player p, real x, real y returns Hero
 local Hero new = Hero.allocate()
 set new.u = CreateUnit(p, 'H000', x, y, 270.0)
 return new
 endmethod
endstruct


Notice the addition of what looks like a variable inside the struct: u. This is called an instance field or instance variable. Like any other variable, it can store data appropriate for its data type.


What is special about instance fields is that each instance of a struct can store different data in the same fields. So for example, one instance of Hero may have unit A stored in its u field while another instance may have unit B stored in its u field.


Because the same field in different instances can store different data, accessing instance fields requires that you specify which instance of a struct you are accessing them from. To see the syntax for this, take a look at the constructor for Hero:

 JASS:

 static method create takes player p, real x, real y returns Hero
 local Hero new = Hero.allocate()
 set new.u = CreateUnit(p, 'H000', x, y, 270.0) // <===
 return new
 endmethod


Accessing an instance field requires that you first provide a reference to the specific instance from which you want to access the field (in this case it is the local variable new). Then follows a dot (.) and then the field identifier (in this case u).


This syntax is used in code outside the struct as well:

 JASS:

function Access takes nothing returns nothing
 local Hero heroOne = Hero.create(Player(0), 0, 0)
 local Hero heroTwo = Hero.create(Player(1), 100, 100)

 call BJDebugMsg(I2S(GetPlayerId(GetOwningPlayer(heroOne.u)))) // Prints "0"
 call BJDebugMsg(I2S(GetPlayerId(GetOwningPlayer(heroTwo.u)))) // Prints "1"
endfunction


Instance fields belong to a group of struct features called instance members. The other part of this group is comprised of features called instance methods.


Instance methods are one type of method. Methods are similar to functions in normal JASS. They carry out certain actions by executing certain lines of code when called. Just like functions, they can take parameters and have a return type. Let's define a simple instance method in Hero:

 JASS:

 method move takes real x, real y returns nothing
 call IssuePointOrder(this.u, "move", x, y)
 endmethod


The new method in Hero is called move() and simply orders the hero to move to a certain point.


You can see the syntax for defining a method is nearly identical to that for defining a function; you just replace the keywords "function" and "endfunction" with "method" and "endmethod". One important difference is that methods must always be declared inside a struct.


Instance methods behave similarly to instance fields in the sense that they behave differently for different instances of a struct. Because of this, when calling instance methods, you must specify a target. In other words, you need to call the method on a specific instance of a struct. Depending on which instance you call the method on (the target instance), the results may be different. For example, calling move() on instance A of Hero will only order instance A to move, while calling the method on instance B only orders instance B to move. The syntax for calling instance methods is similar to that for accessing instance fields:

 JASS:

function CallMethod takes nothing returns nothing
 local Hero hero = Hero.create(Player(0), 0, 0)
 call hero.move(100, 100)
endfunction


It is almost identical to calling regular functions except that you must specify a target for the method.


Notice the this keyword used in move():

 JASS:

 method move takes real x, real y returns nothing
 call IssuePointOrder(this.u, "move", x, y)
 endmethod

this is a reserved keyword when used inside instance methods. It is a pointer that always references the "current" instance of the struct. In other words, it always references the instance in which the code is currently executing. Therefore, to refer to the struct instance on which the currently executing instance method was called (i.e. the target of the currently executing instance method) you use this. An alternate syntax allows you to simply leave out the this keyword and simply use the dot:

 JASS:

 method move takes real x, real y returns nothing
 call IssuePointOrder(.u, "move", x, y)
 endmethod


This syntax works fine most of the time but there are certain situations (which I'm not going to get into right now...for more information see the appendix) in which leaving out this can cause compile errors. For the purposes of this tutorial, I will always explicitly use this inside instance methods.


You are not, of course, restricted to only using this as a target within instance methods; you may also reference other instances:

 JASS:

 method isAlly takes Hero other returns boolean
 return IsUnitAlly(other.u, GetOwningPlayer(this.u))
 endmethod


This concludes the section on instantiating structs and using instance members.

##
 Destroying Structs and onDestroy()


If you can create new instances of structs, it makes sense that you can also destroy them. In fact, there can be only 8190 total instances of a given struct type existing in memory at any one time (See the appendix for more information). Once you hit this limit, creating more struct instances won't do anything until you destroy some instances to free up space.


The syntax to destroy a struct is:

 JASS:

function Die takes Hero hero returns nothing
 call hero.destroy()
endfunction


The instance method destroy() exists for every struct even if you don't manually declare it. If you wish to run specific code when an instance is destroyed, you may do so using the instance method onDestroy():

 JASS:

struct Hero
 // ...
 method onDestroy takes nothing returns nothing
 call RemoveUnit(this.u)
 endmethod
endstruct

onDestroy is run when destroy() is called on an instance but before the instance is actually destroyed. The method signature for onDestroy() is always identical to the one shown above.


A warning about onDestroy(): Do not call destroy() on the same instance that was just slated to be destroyed (or do anything that will eventually result in destroy() being called from that instance), or else you will cause an infinite loop.

##
 Static Members


We have already discussed instance members of structs. There also exist what are called static members. Let's define some in Hero:

 JASS:

struct Hero
 static Hashtable ht // <==

 // ...

 static method create takes player p, real x, real y returns Hero
 local Hero new = Hero.allocate()
 set new.u = CreateUnit(p, 'H000', x, y, 270.0)
 call SaveInteger(Hero.ht, 1, GetHandleId(new.u), new)
 return new
 endmethod

 // ...

 static method getHeroFromUnit takes unit u returns Hero // <==
 return LoadSavedInteger(Hero.ht, 1, GetHandleId(u))
 endmethod
endstruct


We discussed earlier how instance members can behave differently for different struct instances. Static members do not have such variability. They are not associated with any particular instance of the struct type to which they belong. You may think of static fields as global variables and static methods as just regular functions.


Because static members are only associated with a struct type rather than with struct instances, when accessing them you must provide a struct type, in the form of the struct's name, instead of a reference to an instance. Look in the constructor in the above example to see how the static field ht is accessed: using the name of the struct type ("Hero"), then a dot, then the field identifier. The syntax for calling static methods is similar. You may access static members from outside the struct as well:

 JASS:

function AccessStatic takes unit u returns nothing
 local Hero hero = Hero.getHeroFromUnit(u)
endfunction


Static methods cannot use the this keyword like instance methods do (Note in the appendix on this). Static methods may still reference struct instances, but they need to obtain the reference from a source other than this.


Because of these limitations of static members, there are some things to keep in mind about using them:

Static methods are used when access to a specific instance is either not needed or not practical.

Static fields are used to store data that is not just pertinent to a single instance. Usually this means data that is shared by or is relevant to all instances of a struct type without having to be changed.

##
 Encapsulation


An important part of vJASS and OOP in general is encapsulation, which, to put it somewhat inelegantly, is hiding things from those who don't need or shouldn't have access to them.


By default, all members of a struct are public, meaning as long as you have access to the struct type (static members) or an instance of a struct (instance members), you can access the members.


However, in certain situations, you might not want code outside of a certain scope to have access to certain members. Usually this is the case when releasing public scripts or systems and you don't want users who might not know what they are doing to be accessing fields and calling methods they shouldn't be accessing and calling.


One of the ways to achieve this is to declare members as private:

 JASS:

struct Hero
 private static Hashtable ht // <==

 private unit u // <==

 static method create takes player p, real x, real y returns Hero
 local Hero new = Hero.allocate()
 set new.u = CreateUnit(p, 'H000', x, y, 270.0)
 call SaveInteger(Hero.ht, 1, GetHandleId(new.u), new)
 return new
 endmethod

 method getUnit takes nothing returns unit
 return this.u
 endmethod

 // ...

 static method getHeroFromUnit takes unit u returns Hero
 return LoadSavedInteger(Hero.ht, 1, GetHandleId(u))
 endmethod
endstruct


The static field ht has been declared private using the private keyword. Private members cannot be accessed by code outside the struct in which the members were declared. Code within the struct can still freely access private members (as seen in the constructor and static method getHeroFromUnit()).


Static methods and instance members (look at the instance field u) are declared to be private in the same manner, and the effect is the same.

##
 Interfaces


Now that we know a bit about structs, it's time to move on to a very important concept in OOP: polymorphism. In short, polymorphism is the ability to treat multiple types of structs (objects) as if they were all ONE type of object. The simplest way to achieve this is with interfaces.


Just like structs, interfaces define a data type (and in a sense, also a type of object). The difference is that an interface is an abstract data type (ADT). ADTs have the property of being abstract, meaning that they do not define specifically the properties of their types.


To see what this means, take a look at an example:

 JASS:

interface Killable
 method die takes nothing returns nothing
endinterface


The interface Movable seems to be declaring a method: move(). But take a close look and you will see only the method signature (identifier, parameter list, and return type). The method is missing its body, where the code for the method usually goes. Killable simply declares that anything treated as a Killable can be killed via a method called die() but doesn't specify how that method is supposed to work. Another way to look at this is that Killable puts forth a general contract: any Killable object must be capable of being killed via the die() method.


This is how an interface works. It declares instance members (meaning methods and fields) but doesn't actually implement them (or equivalently, puts forth contracts but doesn't specify how they should be fulfilled). This lack of specificity is what renders an interface abstract. In fact, an interface is too abstract to be instantiated like structs. If they can't be instantiated, how are interfaces useful then?


Well, the answer is that structs can extend interfaces. When a struct extends an interface, two things happen. Firstly, the struct must implement any methods that the interface declares. Secondly, instances of that struct can be treated as if they were the same data type as the interface.


For example:

 JASS:

struct Hero extends Killable
 // ...
endstruct


Here, the struct Hero has extended the Killable interface. In essence, by extending Killable, Hero is saying that it wants to be treated as if it were a Killable.


But right now there's a problem; interfaces require that all extending structs implement the methods declared within those interfaces, but Hero has not implemented the method declared inside Killable: die(). Remember the general contract of Killable: any Killable must be capable of being killed by the method die(). Hero must fulfill this contract if it wants to extend Killable, so it must implement die():

 JASS:

struct Hero extends Killable
 // ...
 method die takes nothing returns nothing
 call KillUnit(this.u)
 endmethod
 // ...
endstruct

Hero has implemented die() and by doing so fulfilled Killable's general contract. Now instances of Hero can be treated as the same data type as Killable. So for example:

 JASS:

function InterfaceTypeLocal takes player p, real x, real y returns nothing
 local Killable m = Hero.create(p, x, y) // Saving an instance of Hero into a variable of type Killable
endfunction

function InterfaceTypeReturn takes player p, real x, real y returns Movable
 return Hero.create(p, x, y) // a function with return type Killable returning an instance of Hero
endfunction


You may be saying: this is all great, but why bother with interfaces? Why not just make Hero implement die() without the use of an interface? Well, lets suppose there was another struct that extended Killable:

 JASS:

struct Bomb extends Killable
 // ...
 method explode takes nothing returns nothing
 // makes a big boom
 // ...
 endmethod

 method die takes nothing returns nothing
 call this.explode()
 endmethod
endstruct


Both Hero and Bomb extend Killable, meaning they both implement die() and both can be treated as if they were of type Killable.


Now, let's assume for a second that we wanted a function with a single job: to kill things. The function doesn't care what those things are; as long as they can be killed, it kills them. Well, right now we have two types of objects that can be killed: Hero and Bomb. Without using interfaces, we would probably have to resort to using two functions to get what we wanted:

 JASS:

function KillHero takes Hero hero returns nothing
 call hero.die()
endfunction
function KillBomb takes Bomb bomb returns nothing
 call bomb.die()
endfunction


Well...this seems kind of silly. We have two functions essentially doing the exact same thing: calling die() on something that can be killed. The only difference between the two is that one operates on Hero's and one operates on Bomb's. Wouldn't it be nice if we could achieve the same effect with only a single function?


And in fact we can. If the contract of Killable is that any Killable object can be killed via die(), then we can simply create one function that takes in a Killable instance and calls die() on it:

 JASS:

function Kill takes Killable k returns nothing
 call k.die()
endfunction


We've just cut down our code by 50% by taking advantage of interfaces. The above function can take in instances of both Hero and Bomb since by extending Killable, both struct types can be treated as if they were of type Killable. Therefore the following is possible:

 JASS:

function Macabre takes Hero hero, Bomb bomb returns nothing
 call Kill(hero)
 call Kill(bomb)
endfunction


In the above example we've treated Hero and Bomb, two different struct types, as if they were the same type, namely Killable. This is polymorphism, and you can probably already see the potential advantages of exploiting it.


You may be wondering: wait, if you treat both Hero and Bomb as Killable and simply call die() using a variable of type Killable, which version of die() gets called each time: Hero's or Bomb's? The answer is that the version belonging to the actual struct type of the instance referenced by the Killable variable is always called. So if the variable references a Hero, then Hero's die() will be called, and if the variable references a Bomb, then Bomb's die() will be called. This will be true in all cases:

When calling a method declared by an interface using a variable of the interface type as a reference, the version of the method that is called is always the version belonging to the actual struct type of the instance referenced by the variable.


If you are still a little confused, a good analogy is ordering units to attack. No matter what type of unit you select, when you tell them to attack the same order is given: "attack". If you give that order to a footman it will swing a sword, but give it to a rifleman and it will shoot a rifle, even though the same order is being given. But no matter what, the correct action is always taken; you never see a footman trying to shoot a rifle or a rifleman trying to swing a sword.


If you use a variable of an interface type to reference some struct instance, you may only access members declared by the interface. Any other members that the struct may have cannot be accessed:

 JASS:

function CantHappen takes player p, real x, real y returns nothing
 local Killable k = Hero.create(p, x, y)
 call k.getUnit() // this will result in a compile-time error
endfunction


Even though the struct instance is in actuality of type Hero, the variable k has been declared as type Killable, so the compiler can only be sure that whatever k references has implemented die() and nothing more.


To call getUnit(), you would have to make an explicit cast:

 JASS:

function CanHappen takes player p, real x, real y returns nothing
 local Killable k = Hero.create(p, x, y)
 call Hero(k).getUnit() // explicit cast
endfunction


Or:

 JASS:

function CanAlsoHappen takes player p, real x, real y returns nothing
 local Killable k = Hero.create(p, x, y)
 local Hero hero = k // explicit cast through an implicit cast...
 call hero.getUnit()
endfunction


This essentially tells the compiler: the instance is actually a Hero, so calling getUnit() is legal! However, you should NEVER make casts unless you are absolutely sure that what you are casting is in fact compatible with the data type to which you are casting. For example:

 JASS:

function CanAlsoHappenButShouldnt takes player p, real x, real y returns nothing
 local Killable k = Hero.create(p, x, y)
 local Bomb bomb = k
 call bomb.explode()
endfunction


This code is obviously wrong and will create all kinds of bugs but the compiler will not flag this as an error. It simply trusts that you know what you are doing when making explicit casts.


I think it's important to note that (sadly) interfaces cannot extend other interfaces.

##
 Struct Inheritance


I now introduce to you a struct called Sword:

 JASS:

struct Sword
 private real damage
 private Hero hero

 static method create takes real damage returns Sword
 local Sword new = Sword.allocate()
 set new.damage = damage
 set new.hero = 0
 return new
 endmethod

 method getDamage takes nothing returns real
 return this.damage
 endmethod

 method getWielder takes nothing returns Hero
 return this.hero
 endmethod

 method wield takes Hero hero returns nothing
 set this.hero = hero
 endmethod

 method attack takes unit u returns nothing
 call UnitDamageTarget(this.hero.getUnit(), u, this.damage, true, false, ATTACK_TYPE_NORMAL, DAMAGE_TYPE_NORMAL, WEAPON_TYPE_WHOKNOWS)
 endmethod
endstruct


A sword can be wielded by a hero and can attack units, damaging them.


Now let's say we want to create a new type of sword. This sword would be able to do everything normal swords do, but also have the ability to heal its wielder. We could define this new sword like this:

 JASS:

struct HolySword
 private real damage
 private real heal
 private Hero hero

 static method create takes real damage, real heal returns HolySword
 local HolySword new = HolySword.allocate()
 set new.damage = damage
 set new.heal = heal
 set new.hero = 0
 return new
 endmethod

 method getDamage takes nothing returns real
 return this.damage
 endmethod

 method getWielder takes nothing returns Hero
 return this.hero
 endmethod

 method getHeal takes nothing returns real
 return this.heal
 endmethod

 method wield takes Hero hero returns nothing
 set this.hero = hero
 endmethod

 method attack takes unit u returns nothing
 call UnitDamageTarget(this.hero.getUnit(), u, this.damage, true, false, ATTACK_TYPE_NORMAL, DAMAGE_TYPE_NORMAL, WEAPON_TYPE_WHOKNOWS)
 endmethod

 method heal takes nothing returns nothing
 call SetWidgetLife(this.hero.getUnit(), GetWidgetLife(this.hero.getUnit()) + this.heal)
 endmethod
endstruct


Wait a minute. HolySword is almost identical to Sword. The only differences are the private field heal and the method heal(), as well as some minor changes in the constructor. We've just duplicated over a dozen lines of code! And as it turns out, we've done it needlessly, because we could have taken advantage of struct inheritance.


What is struct inheritance? To put it simply: it's when a struct extends another struct. Let's put it to use with HolySword and see what we get:

 JASS:

struct HolySword extends Sword
 private real heal

 static method create takes real damage, real heal returns HolySword
 local HolySword new = HolySword.allocate(damage)
 set new.heal = heal
 return new
 endmethod

 method getHeal takes nothing returns real
 return this.heal
 endmethod

 method heal takes nothing returns nothing
 local Hero wielder = this.getWielder()
 call SetWidgetLife(wielder.getUnit(), GetWidgetLife(wielder.getUnit()) + this.heal)
 endmethod
endstruct

HolySword is now a child of Sword, and Sword is the parent of HolySword. Notice how the code for HolySword is now 50% shorter!


Child structs inherit all the instance members of their parent structs, meaning they automatically implement all those fields and methods, even if the code isn't written out for them in the child. So in our HolySword example, HolySword has the fields damage and hero as well as the methods getDamage(), getWielder(), wield(), and attack(). So you can do things like:

 JASS:

function Inheritance takes Hero hero, unit u returns nothing
 local HolySword s = HolySword.create(100, 100)
 call s.wield(hero)
 call s.attack(u)
endfunction


Note that HolySword cannot access the private field wielder in Sword and must use the public getter method getWielder(). This is a general rule. Child structs cannot access the private members of their parents.


By convention, child structs are less abstract than their parents. For example, Sword represents a sword that can be wielded and can damage units. Beyond that, Sword is very abstract about its properties. HolySword inherits all of these properties, and on top of that it says that a holy sword can heal its wielder.


Just like how structs that extend interfaces can be treated as if they were of the interface type, structs that extend other structs can be treated as if they were of the type of their parents. Therefore, child struct instances can be referenced by variables of the parent struct type. However, like with interfaces, if using a variable of the parent struct type to reference some struct instance, you may only access members defined for the parent struct.


An important thing to remember is that multiple inheritance is not allowed. In other words, a struct may extend one and only one struct. However, arbitrarily many structs may extend the same parent.


(This should be obvious but I should probably say it anyway. You can't have two or more structs circularly extending one another. In other words, you can't have struct A extending struct B extending struct C extending struct A.)


Struct inheritance creates what is called a hierarchy or a hierarchal relationship among structs. The struct hierarchy is ordered such that parents are always situated above their children. In general, the higher up the hierarchy you go, the more abstract the structs are.


When a struct extends another, constructor and onDestroy() behavior change. The new behavior is critical to the correct functioning of struct inheritance.


If the child struct does not explicitly declare a constructor, then for all intents and purposes it inherits its parent's constructor. Practically what this means is that the constructor of the child will require the same parameters as its parent. (Note that this is a very special case of inheritance. Normally static members, including static methods like create() cannot be inherited.)


In the case where the child does explicitly declare a constructor, the call to the static allocate() method will call the constructor of the parent struct. The allocate() method now also has to take the same parameters as the constructor of the parent struct:

 JASS:

 static method create takes real damage, real heal returns HolySword
 local HolySword new = HolySword.allocate(damage) // <==
 set new.heal = heal
 return new
 endmethod


Notice that the parameter list for HolySword (two reals) doesn't match the list for Sword (one real). This is completely legal. The constructor of a child struct may have a completely different parameter list than that of its parent.


Because allocate() calls the constructor of a child struct's parent, when the child struct is instantiated, the constructor(s) of its parent(s) is (are) also called. The order in which they are called is always from the top of the hierarchy downward; parents first then childs.


What about onDestroy()? It turns out that when a struct is destroyed, its own onDestroy() method is called, then the onDestroy() method of its parent is called, and then that of the parent's parent is called, etc. Note that if a struct is destroyed, the onDestroy() methods of its children, grandchildren, etc. will not be called.

##
 Merging Inheritance with Interfaces


You can make structs extend structs that extend interfaces. So for example let's create a new type of hero:

 JASS:

struct VainHero extends Hero
 // ...
 method strikePose takes nothing returns nothing
 // the hero strikes a bombastic pose
 // ...
 endmethod
 // ...
endstruct


In this case, VainHero is extending Hero, which in turn extends an interface: Killable. This means that VainHero is indirectly extending Killable. Because of this, VainHero can be considered as being both of type of Hero and Killable.

VainHero does not necessarily need to explicitly define the method die() even though Killable requires them. This is because Hero has already defined that method and VainHero automatically inherits it from Hero. However, you can explicitly define die() in VainHero by overriding it (see the section on method overriding).


Note that a single struct cannot extend both another struct and an interface at the same time. For obvious reasons, interfaces cannot extend structs.

##
 Overriding Methods


In vJASS, there are two ways to do method overriding. The first is to have a child struct override a method in a parent struct declared by an interface that the parent struct extends. The other is to have a child struct override a stub method in a parent struct.


In both cases, the end result of method overriding is the same: the overriding method essentially replaces the method it is overriding.


To have a method override a method in a parent struct, the new method must have the same signature (identifier, parameter list, and return type) as the method in the parent struct. Once again, learning by example is best:

 JASS:

struct VainHero extends Hero
 // ...
 method die takes nothing returns nothing
 // dies in style.
 call this.strikePose()
 call KillUnit(this.getUnit())
 endmethod
 // ...
endstruct

VainHero still satisfies the requirement to extend Killable: it has a die() method. Notice that the method signature is identical to Hero's die() and the declaration of die() in Killable. However, VainHero's die() does something different from Hero's method: it makes a call to strikePose(). This new die() method essentially replaces Hero's die(). Calling die() on an instance of VainHero would result in the hero striking a pose before succumbing to death.


Now let's say we want VainHero to strike a pose whenever he is ordered to move somewhere; we would need to change how move is implemented for VainHero. We can do this using the second way of overriding methods. First we change move() in Hero to a stub method:

 JASS:

struct Hero
 // ...
 stub method move takes real x, real y returns nothing // <==
 call IssuePointOrder(this.getUnit(), "move", x, y)
 endmethod
 // ...


Adding the stub keyword before the method signature essentially declares the method to be capable of overridden in any children that the current struct may have.


Now that move() can be overridden, we declare a new move() method in VainHero:

 JASS:

struct VainHero extends Hero
 // ...
 method move takes real x, real y returns nothing
 call this.strikePose()
 call IssuePointOrder(this.getUnit(), "move", x, y)
 endmethod
 // ...
endstruct

VainHero has now overridden move(). Like with the previous example using die(), whenever move() is called on an instance of VainHero, VainHero's move() would be called instead of Hero's move(), and the hero would strike a pose before moving.


Overriding methods naturally leads us to the following question. Suppose we were to code the following:

 JASS:

function WhatsGoingOn takes nothing returns nothing
 local Hero hero = Hero.create(Player(0), 0, 0)
 local VainHero vainHero = VainHero.create(Player(0), 100, 100)
 local Hero anotherHero = VainHero.create(Player(0), -100, -100)
 call hero.move(1000, 1000) // Line 1
 call vainHero.move(1000, 1000) // Line 2
 call anotherHero.move(1000, 1000) // Line 3
endfunction


What would happen in lines 1, 2, and 3? Which move() is being called in each line: Hero's or VainHero's? The answer is:

Line 1: Hero's

Line 2: VainHero's

Line 3: VainHero's


The take-home point here is that when a method is called on some instance A, the only thing that matters when determining which version of the method is called is the actual struct type of A. The data type of the variable used to reference A does not matter.


A final note about method overriding:

Static methods cannot be overridden.


If you attempt to create a static method in a struct with the same method signature as a static method in the struct's parent, you will get a compile error.

##
 super


Sometimes, you might want an overriding method to call the parent's version of the method. To do this, you use the super keyword. For example, if you look at VainHero's move, it is the same thing as Hero's version except that it just adds a call to strikePose(). We can take advantage of this fact and rewrite VainHero's move():

 JASS:

struct VainHero extends Hero
 // ...
 method move takes real x, real y returns nothing
 call super.move(x, y)
 call this.strikePose()
 endmethod
 // ...
endstruct


Notice the use of the keyword super. When used within an instance method of struct that extends another, super allows us to reference the current struct instance (like this) except that it forces the instance to be treated as if its actual struct type were its type's parent type.


So in the above example, even though VainHero has overridden move(), calling super.move() actually causes Hero's version of move() to be called.


If you were wondering, you can't do something like:

 JASS:

 method foolish takes nothing returns nothing
 call super.super.foolish()
 endmethod


In fact, if you ever find yourself needing to do this, you need to refactor your code because this is a sign that your struct hierarchy is flawed.

##
 typeid and getType()


There will be times where you need to check the actual struct type of some struct instance. For example:

 JASS:

function Risky takes Hero hero returns nothing
 local VainHero vainHero = hero
 call vainHero.strikePose()
endfunction


In the above code, we don't know if the parameter hero references an instance of VainHero or Hero. If it references an instance of Hero, casting it to VainHero and calling strikePose() on it would be very bad. So how do we fix this? Like this:

 JASS:

function Safe takes Hero hero returns nothing
 local VainHero vainHero
 if (hero.getType() == VainHero.typeid) then
 set vainHero = hero
 call vainHero.strikePose()
 endif
endfunction


The getType() method is an instance method defined for all struct types that returns an integer unique to the struct type of the instance on which getType() is called. The typeid field is an implicitly defined static field that returns the same unique integer of the struct type. So, if getType() of instance A returns the same integer as typeid of struct type T, you can be sure that A's actual struct type is in fact T.


The primary drawback of getType() and typeid is that we can only use it to compare actual struct types. We cannot use it to check to see if an instance is of a struct type that extends another struct type. To see how this can be a problem, let's take a look at this example:

 JASS:

function Unfortunate takes Killable k returns nothing
 local Hero hero
 if (k.getType() == Hero.typeid) then
 set hero = k
 call hero.move(0, 0)
 endif
endfunction


Now assume that k references an instance of VainHero. In this case k.getType() returns a different integer from Hero.typeid because those integers are unique to each struct type and VainHero and Hero are two distinct struct types. It's perfectly valid to call move() on any instances of Hero and structs that extend Hero, including VainHero, but in this case the code will not recognize instances of VainHero as valid instances on which to call move().

##
 Function Objects


vJASS has the option of letting you treat even functions as objects. The idea isn't really that shocking, considering normal JASS already had a data type for functions (code), so it was already taking a tiny step toward treating functions as their own type of object.


An example of a function object:

 JASS:

function DisplayHeroDeath takes Hero hero returns nothing
 call DisplayTextToPlayer(GetOwningPlayer(hero.getUnit()), 0, 0, "Your hero has died.")
endfunction


Notice that the syntax is exactly the same as for normal functions. In fact, every function that you define is automatically considered to be a function object. Native functions, however, cannot be treated as function objects.


There are two methods you can call on function objects: evaluate() and execute(). Both of these methods have the same parameter list as the function and simply run the code in the function body. The advantages of using these rather than a simple call is that they allow you to run functions before you define them in the map script:

 JASS:

function Awesome takes Hero hero returns nothing
 call DisplayHeroDeath.evaluate(hero)
endfunction

function DisplayHeroDeath takes Hero hero returns nothing
 call DisplayTextToPlayer(GetOwningPlayer(hero.getUnit()), 0, 0, "Your hero has died.")
endfunction


There are limitations to these two methods and differences between them. Both are slower than a normal function call. Using evaluate() is not compatible with GetTriggeringTrigger(), but is compatible with all other event responses, and is also not compatible with waits. Using execute() runs the function in a new thread - unlike evaluate() - so obviously it is not compatible with event responses. A perk of execute() is that it is faster than using the native ExecuteFunc(), not to mention it won't crash the map if you typed in the function name incorrectly (you get a compile-time error instead).

##
 Function Interfaces


Like structs, function objects have interfaces for them! Instead of requiring fields and methods, function interfaces require a specific parameter list and a specific return type. Unlike struct interfaces, where a struct extending an interface can implement extra fields and methods besides those required by the interface, functions extending an interface must match the parameter list and return type specified by the interface exactly.


The conventions for struct interfaces hold for function interfaces. Function interfaces are abstract types while the functions that extend them are less abstract, providing greater specificity.


An example of a function interface:

 JASS:

function interface OnHeroDeath takes Hero hero returns nothing


For a function to extend an interface, all that is required is that its parameter list and return type match the interface; if this is satisfied, then the function will automatically extend the interface. There is no explicit "function extends interface" syntax involved.


Like struct interfaces, a function interface defines a data type. Because of this, functions can now be stored into variables or passed as parameters or even returned by functions or methods:

 JASS:

struct Hero extends Killable
 // ...
 private OnHeroDeath onHD
 // ...
 static method create takes player p, real x, real y, OnHeroDeath onHD returns Hero
 local Hero new = Hero.allocate()
 set this.u = CreateUnit(p, 'H000', x, y, 270.0)
 set this.onHD = onHD
 return new
 endmethod
 // ...
 method die takes nothing returns nothing
 call this.onHD.evaluate(this)
 call KillUnit(this.u)
 endmethod
endstruct


Notice how I can take in a function of type OnHeroDeath in the constructor and then save that function to an instance field of type OnHeroDeath. All that is required is that whatever function is passed in extends OnHeroDeath. Notice how I can also call evaluate() on the field onHD. I could also call execute() on it if I wanted to.


Now that we have a function interface set up, we can instantiate a Hero that when killed, displays to its owning player that one of his/her heroes has died:

 JASS:

function InformativeHero takes nothing returns nothing
 call Hero.create(Player(0), 0, 0, OnHeroDeath.DisplayHeroDeath)
endfunction


Notice the syntax for using a function as a variable: [function interface name] + . + [function name]. You can also drop the function interface name:

 JASS:

function InformativeHero takes nothing returns nothing
 call Hero.create(Player(0), 0, 0, DisplayHeroDeath)
endfunction


The alternate syntax requires less typing but the compiler will not be able to check if the function does in fact extend what it's supposed to extend.

##
 Glossary of Terms

abstract


The property of not being specific (or being less specific) as to how something is supposed to be done.

abstract type


A type that is abstract enough that it is either unwise or not possible to create working objects of that type. Abstract types are usually extended by other types that are less abstract.

abstraction


The act of hiding irrelevant and/or unimportant processes or data, leaving behind only what is immediately necessary. Not to be confused with encapsulation.

child


In the case where a struct extends another struct, this is the struct that is doing the extending. Lower in the struct hierarchy and less abstract (more specific).

constructor


The static method - named create() - that is called on struct instantiation.

encapsulation


The practice of hiding, or preventing access to, certain data within a scope (for example a struct) from code outside of that scope. Not to be confused with abstraction.

extends


Keyword used when an object or type type wishes to obtain the properties of a more abstract type. By contract the object or type that is doing the extending is less abstract and provides more specificity than the object or type that is being extended.

hierarchy


The organization of relationships among struct types with regard to their parents and children. Parents are always higher in the hierarchy than their children. In general, structs higher in the hierarchy are more abstract than those lower in the hierarchy.

inheritance


The process of a child struct absorbing or copying a parent struct's fields and methods into itself. Can also be used to refer to the act of a struct extending another struct.

instance


One specific copy of a struct type that exists in memory.

instance member


A member that is defined on an instance-by-instance basis. Instance members may behave differently for different instances of the same struct type. Access to instance members requires a reference to a struct instance.

instantiation


The act of creating a new instance of a struct.

interface


An abstract type that defines (abstractly) the properties of a group of objects. Struct interfaces require implementation of fields and/or methods while function interfaces require a specific parameter list and return type.

member


A field or a method that is declared inside a struct.

method


A type of struct member that represents an action that a struct may undertake. Similar to a function.

method overriding


The act of replacing a method in a parent struct with a newly defined method in the child struct.

parent


In the case where a struct extends another struct, this is the struct that is being extended. Higher in the struct hierarchy and more abstract (less specific).

polymorphism


The ability to treat multiple types of objects as a single, common object type.

private


A property of data whereby the data is inaccessible from outside the scope in which it is declared. Also the keyword to declare data to be private.

public


A property of data whereby the data is accessible from all scopes. Also the keyword to declare data to be public.

static member


A member that is defined identically for all instances of a struct type. Access to static members only requires reference to a struct type.

struct


A type of object. Contains fields and methods that serve to describe the properties of the object.

struct type


See struct.

stub method


A method that is declared to be capable of being overridden.

super


A keyword implicitly defined within instance methods of structs that extend a parent. This keyword references the struct instance on which the method was called (the "current" instance) but forces the instance to be treated as if its actual struct type were its type's parent type.

this


A keyword implicitly defined within instance methods that always references the struct instance on which the method was called (the "current" instance).

##
 Appendix


The appendix is a section reserved for discussing issues that are not absolutely critical to understanding OOP in vJASS but vJASS beginners should eventually be aware of anyway. I chose to separate these topics from the main tutorial mainly to avoid confusing readers with extra information that is not immediately pertinent.

###
 Use of Implicit this


I discussed in the section dealing with instance members that inside instance methods, the keyword this may be left out when referencing the "current" struct instance. Most of the time, this syntax is fine. However, when using vJASS's private keyword feature to encapsulate instance members within a library or scope, dropping the this when referencing those encapsulated members will generate a compile error (the compiler will tell you that the member doesn't exist). To see an example:

 JASS:

library A
private keyword i
struct S
 integer i
 method foo takes nothing returns nothing
 set .i = 100 // Generates compile error
 endmethod
endstruct
endlibrary


To fix this, simply explicitly use this:

 JASS:

library A
private keyword i
struct S
 integer i
 method foo takes nothing returns nothing
 set this.i = 100
 endmethod
endstruct
endlibrary

###
 Use of Implicit Struct Type Names


When referencing static members within a struct, you may drop the struct type name before the dot (similar to dropping this when referencing instance members):

 JASS:

struct S
 static integer i = 5
 static method increment takes nothing returns nothing
 set .i = .i + 1
 // Same as: set S.i = S.i + 1
 endmethod
endstruct


This is acceptable syntax most of the time. However, like with implicit this, it causes compile errors when using private keywords:

 JASS:

library A
private keyword i
struct S
 static integer i = 0
 static method increment takes nothing returns nothing
 set .i = .i + 1 // Generates compile error
 endmethod
endstruct
endlibrary


To fix this, simply use the struct type name:

 JASS:

library A
private keyword i
struct S
 static integer i = 0
 static method increment takes nothing returns nothing
 set S.i = S.i + 1
 endmethod
endstruct
endlibrary


There is another time when using implicit struct type names can cause compile errors. This is when using static methods as callback functions in certain natives such as TimerStart(), ForGroup(), and the GroupEnum functions:

 JASS:

struct S
 static integer i = 0
 static method increment takes nothing returns nothing
 set .i = .i + 1
 endmethod
 static method start takes nothing returns nothing
 call TimerStart(CreateTimer(), 1.00, true, .increment) // Generates compile error
 endmethod
endstruct


In these cases you must use the full struct type name:

 JASS:

struct S
 static integer i = 0
 static method increment takes nothing returns nothing
 set .i = .i + 1
 endmethod
 static method start takes nothing returns nothing
 call TimerStart(CreateTimer(), 1.00, true, S.increment)
 endmethod
endstruct

###
 Using this as a Local Variable Identifier


Because the this keyword is not defined in static methods and regular functions, you may use this as a local variable identifier in them:

 JASS:

struct S
 static method foo takes nothing returns nothing
 local S this = S.create()
 endmethod
endstruct
function Bar takes nothing returns nothing
 local S this = S.create()
endfunction

###
 Struct Storage Size


The default storage size (usually equivalent to instance limit) of any struct type is 8190. This means that only 8190 instances of any given struct type can exist in memory at any one time.


Note that if a struct extends anything (doesn't matter if it is another struct or an interface) or is extended by another struct, instances of that struct type and instances of any struct types "related" to it (i.e. is a child or shares a common parent) all share a common storage size (of 8190). So if struct A, B, and C all extend interface I, there can only be 8190 total instances of A, B, andC. If a new struct D were to extend B, then there could only be 8190 total instances of A, B, C, and D.


There are ways to increase the storage size of a struct. You can read about it in the jasshelper manual.

##
 Credits


Thanks to Dreadnought[dA], Eleandor, and PurplePoot for their helpful suggestions. And of course a big thank you to Vexorian for creating vJASS and jasshelper.


 Last edited: Sep 26, 2009

#### Dreadnought[dA]

#### Dreadnought[dA]

 Level 12

JoinedFeb 23, 2007Messages1,030

Resources: 1

#### Resources
Tutorials 1

After reading this tutorial I have come to the conclusion nobody will ever make a good vJASS tutorial for programming language illiterate people and I'll just stick with Game Cache. (I self taught myself most of what I know, so I don't know all that EXPLICIT and EXTENDS stuff. You tried to explain it, but unfortunately you failed to get into the mind of a non-programmer, therefore your words are lost on me. If you can explain it using gamecache comparisons, then maybe I could understand it. @_@

Basically I think this is a good tutorial, but only for those who already know how to do this stuff LOL.

Dumb it down a bit. For dummies like me

In the meantime, call DestroyGroup(noobs)

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

Thanks for the input, I'll update this soon and add in some stuff to try to explain a bit more about some of the basics, like instantiation and the "extends" stuff.


By the way, this tutorial doesn't really focus on the aspects of vJASS that make it an alternative to gamecache; the topic is how to exploit the OOP features of vJASS.

#### Dreadnought[dA]

#### Dreadnought[dA]

 Level 12

JoinedFeb 23, 2007Messages1,030

Resources: 1

#### Resources
Tutorials 1

But a lot of people tell me that vJASS is much better than GameCache. Aren't structs and GameCache used for pretty much the same thing?

IDK, I just can't understand structs. It looks like you make the same thing 3 times. Attach the thing to itself and call itself to call another thing. I mean WTF @_@

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

Yea, structs are a nice way to attach things since they can be treated as integers, so you can put them into unit/item custom values. It also makes life simpler if you have a storage system that only needs to work with integers rather than all the primitive types and then handles and all that H2I() stuff...


But structs can be used to do a lot more than just to attach/store variables to things, and this other stuff is what my tutorial focuses on.


EDIT: Updated tutorial, added more explanation in most sections, added three new sections at the end for function objects, function interfaces, and a glossary.


 Last edited: May 20, 2009

#### Dreadnought[dA]

#### Dreadnought[dA]

 Level 12

JoinedFeb 23, 2007Messages1,030

Resources: 1

#### Resources
Tutorials 1

The whole *capturable* base explanation just doesn't seem to work for me. It would be nice if you made a simple Leap ability that used a struct to attach the facing value and stuff to the unit. @_@ I'm really trying to understand it, but it's just not coming to me. If you could show an example of something I've made before with GameCache (a leap spell) than I think I might be able to understand it.

You're doing a great job so far and I really look forward to the perfection of this tutorial.

Multiple examples would be the best IMO. Different ways of using structs and the different things you mentioned. Perhaps Hide/Unhide tags on them to make it easier to read.

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

Update. Added in discussions on the concept of abstractness and abstract types, updated glossary, fixed some errors, and placed

 tags around all jass code to shorten length.

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

If you talk about a constructor, mention something about the destructor (and the onDestroy companion)?

Perhaps using the typical "Car / Truck / Bus" example could be useful to explain the concepts of structs, inheritance (a truck is a car, but a car ain't no truck), stub methods (turning a car goes faster than turning a truck, but it's still both turning), and statisism of certain methods (a car always has 4 wheels, so getting the weelcount will always return 4, hence why it could be static; a specific car has a color, which is why it shouldn't be static, ...)

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

Hmmm, you're right...I will add in stuff about destroying structs.


Btw, I like your analogy. It's certainly better than how I first learned about all this stuff: looking at different implementations of ADTs (sad, isn't it?...)


EDIT: Update. Added section on destroying structs. Also added credits section.


 Last edited: May 22, 2009

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

 In fact, if you ever find yourself needing to do this, you need to refactor your code because this is a sign that your struct hierarchy is flawed.


Click to expand...

Why? IMO you should be able to call super.super, regardless of whether it's possible or not.

#### Deaod

#### Deaod

 Level 14

JoinedNov 18, 2007Messages816

Resources: 12

#### Resources
Maps 1Spells and Systems 11

using delegate is a better way of making structs extend other structs, imo.

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

 Why? IMO you should be able to call super.super, regardless of whether it's possible or not.


Click to expand...


If you ever need to call super.super, it means that whatever class you're trying to call it in shouldn't be extending the struct it's currently extending. By making the struct bypass its parent's functionality, you are saying that at least one aspect of the struct is not compatible with its parent, and under OOP paradigm this should not happen. You should make the struct extend the parent's parent instead.

 using delegate is a better way of making structs extend other structs, imo.


Click to expand...


I've heard a lot of people say that. Personally I avoid delegate unless I need to "extend" a struct and an interface at the same time.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Seems good. My only complaints are that you should use this.* rather than .* for clarity (as these are new users, after all), and this should not be used except in its proper usage (to refer to the relevant struct a method is part of), again for clarity.

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

Yeah, I agree. Will update soon...prolly will include something about delegate as well, since that seems to be popular, though I must say I don't agree with using delegate as an (easier) alternative to extending structs (except of course when it is absolutely necessary to get some kind of pseudo-inheritance otherwise not possible, like with a struct having to "extend" an interface and another struct, which I've already mentioned). I'll probably include a discussion on how delegate is (supposed to be) different from extending structs.

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

aznricepuff said:

 If you ever need to call super.super, it means that whatever class you're trying to call it in shouldn't be extending the struct it's currently extending. By making the struct bypass its parent's functionality, you are saying that at least one aspect of the struct is not compatible with its parent, and under OOP paradigm this should not happen. You should make the struct extend the parent's parent instead.


Click to expand...

I know at least one language that allows something like super.super.

I can't think of a good example where you would indeed need this, but I'm sure there must be an example somewhere...

Still, that's not really the point of this thread.

 though I must say I don't agree with using delegate as an (easier) alternative to extending structs


Click to expand...

IMO delegates aren't supposed to be an alternative to extending structs. Delegates are there for those occasions where full inheritance wouldn't be a good idea, but inheriting the methods still prove useful.

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

 IMO delegates aren't supposed to be an alternative to extending structs. Delegates are there for those occasions where full inheritance wouldn't be a good idea, but inheriting the methods still prove useful.


Click to expand...


Then we agree, lol. My point was that delegates shouldn't be used to replace extending structs...but I guess I didn't make that clear

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

If I were you, I'd stick with this and end the tutorial. If you want to include more advanced vJass features (delegates, operator overloading, modules, etc), they should be in a separate tutorial so as not to scare away the newbies.

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

Meh, duly noted. I guess I'm just gonna fix the this stuff, go thru it one more time for any mistakes, and finalize it.

#### Deaod

#### Deaod

 Level 14

JoinedNov 18, 2007Messages816

Resources: 12

#### Resources
Maps 1Spells and Systems 11

delegate...

 - imports every member of the delegated struct into the "child" struct (if it has not already been declared in the struct itself or a previous delegate)

 - does not make the "child" struct a subtype of the "parent" struct (to which you delegated certain things)

 - allows you to "extend" multiple "parent" structs

so, as noted before, delegate is not really like extending, but for me a powerful tool to do something resembling extending a parent.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Tell me when you're done, aznricepuff.

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

Update. Finished tutorial. I don't expect to make any more major additions or changes (unless some huge mistake is found).

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Alrighty, approved.

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

Ok, so basically I've been TAing a CS course for a couple weeks now and after teaching a bunch of people who have limited/no programming experience how to use Java and seeing which explanations make sense to them and which don't, I realize that I need to modify this tutorial to make it more effective.


Before I do though, I just wanted to get a little feedback about the examples I use. While I was writing this, I thought it would be advantageous to use a single "track" for all examples so readers could follow along with a sort of psuedo-dev process. Now I realize this might just confuse some people since for them to fully understand the examples, they need to understand what the code in them is trying to accomplish, which, is not so clear at times.


So, basically, should I keep the current example scheme or overhaul it so that the examples are no longer necessarily connected, but instead all very simple and easy to understand?

#### Dreadnought[dA]

#### Dreadnought[dA]

 Level 12

JoinedFeb 23, 2007Messages1,030

Resources: 1

#### Resources
Tutorials 1

Have different easily identifiable examples for each scenario. You can connect them as well, but don't try to force it.

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

Updated...finally. Also added appendix to discuss some extra issues that are pretty important but don't really belong within the main body of the tutorial.

#### Deon

#### Deon

 Level 12

JoinedFeb 13, 2009Messages388

Resources: 30

#### Resources
Icons 30

This article is pretty darn awesome for me.

Today I've decided to start mapmaking. I have some limited experience but with GUI only, but this time I decided to go full JASS way.

The question is: JASS or vJASS.

Does vJASS require NEWGEN only and cannot be used in usual WE? Is NEWGEN updated for the latest WC3 version?

That's all I want to know, If I you can help me to find the newest version of NEWGEN or if you say me that common WE accepts vJASS (I doubt so though), then I will read the 2nd part of your tutorial. I've read the first one out of curiosity

Thank you, +rep and all... So just big thanks!

#### syltman

#### syltman

 Level 14

JoinedJun 13, 2007Messages1,430

Deon said:

 This article is pretty darn awesome for me.


Today I've decided to start mapmaking. I have some limited experience but with GUI only, but this time I decided to go full JASS way.


The question is: JASS or vJASS.


Does vJASS require NEWGEN only and cannot be used in usual WE? Is NEWGEN updated for the latest WC3 version?


That's all I want to know, If I you can help me to find the newest version of NEWGEN or if you say me that common WE accepts vJASS (I doubt so though), then I will read the 2nd part of your tutorial. I've read the first one out of curiosity


Thank you, +rep and all... So just big thanks!


Click to expand...


you don't need newgen to install vjass but it's the easiest way to install it. The latest newgen works with the latest patch and you can find it on wc3c

#### Miss_Foxy

#### Miss_Foxy

 Level 19

JoinedOct 15, 2008Messages3,228

What is the difference between vJASS and JASS?

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

Thanks for writing this tut man  + rep


I have had some small jass experience about 1 and a half year ago and knew like nothing about it annymore.. and in that 1 and a half year I learned how to program in an object orientated way using Java.


I seriously didnt knew anything about jass anymore but vJass kind of looks like the same language as java  This helps me to go further into vJass.


I thought maybe thats usefull to notice to other people who knows java verry well.


I really like the tutorial, it helped me out a lot. Another thanks for that

 You must log in or register to reply here.


### Similar threads

 [Lua] Switching from vJass to Lua (structs, methods, modules and more)


Bribe

May 6, 2022

Scripting Tutorials

Replies4Views5K

Aug 10, 2022
Bribe

Locked

[JASS]
 (vJass) Inheritance problem


Anachron

Mar 31, 2010

Triggers & Scripts

Replies3Views2K

Apr 1, 2010
Anachron

 Structs for Dummies


Magtheridon96

Jun 15, 2011

JASS Tutorials

Replies19Views26K

May 3, 2016
Zwiebelchen

Locked

[vJASS]
 Struct extension constructor problem


Wietlol

Aug 18, 2016

Triggers & Scripts

Replies12Views3K

Aug 22, 2016
LeP

Locked

 Need feedback for my system


Thanathos

Jan 28, 2010

Triggers & Scripts

Replies1Views769

Jan 30, 2010
Darkt3mpl3r

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top