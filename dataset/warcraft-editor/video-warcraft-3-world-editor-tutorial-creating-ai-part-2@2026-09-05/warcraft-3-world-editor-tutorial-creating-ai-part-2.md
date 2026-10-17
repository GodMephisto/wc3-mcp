---
type: vendor-doc
package: video-turnro
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=JEUNAPQGwpQ
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 1953
chapters: 12
timestamped: yes
---

# Warcraft 3 World Editor Tutorial: Creating AI Part 2

Source: <https://www.youtube.com/watch?v=JEUNAPQGwpQ>

Uploader: Turnro | Duration: 32m33s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

A comprehensive tutorial on how to write AI scripts using Jass code. This is the second part of the series.

JASSCRAFT DOWNLOAD
http://www.wc3c.net/showthread.php?t=80051

JASS DEMO DOWNLOAD
http://jass.sourceforge.net/download.shtml

## Chapters

- [0:00](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=0s) <Untitled Chapter 1>
- [0:36](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=36s) Introduction to What Jazz Code Is
- [6:04](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=364s) Basic Ki Script
- [12:04](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=724s) Predefined Variables
- [13:48](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=828s) Call Campaign Ai
- [15:14](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=914s) Set Replacements
- [16:43](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1003s) Set Slow Chopping
- [18:12](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1092s) Ai To Build Units and Buildings
- [20:31](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1231s) Wait for Signal
- [22:12](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1332s) Send the Attack Waves
- [22:37](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1357s) Init Assault Group
- [30:44](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1844s) Jazz Demo

## Transcript

**[0:00](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=0s)** hey guys it's turnin row here and today I'll be continuing on with my AI scripting tutorial and today we are going to look at a different way of how we can write our own AI scripts which is a method that I preferably use when writing my own AI scripts for my own maps which is where we are programming or writing the actual code of our AI using jazz code now before we get into that sort of part it is probably best

**[0:34](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=34s)** for me to explain or give you a brief introduction to what jazz code is so jazz code is essentially a library extension of a programming language called C plus which was developed by the programmers at Blizzard to write and create their RTS games such as Warcraft 3 and Starcraft 2 so in other words jazz code is the actual programming use to create the game now there are some people who when it comes to triggers for

**[1:11](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=71s)** example they like writing their triggers using actual jazz code which this is an example of what jazz code can look like and as a result it gives a lot of freedom and choice and ability to do whatever you want in Warcraft 3 now the good news is we're not concentrating on jazz code in triggering since there is a lot to learn we are only concentrating on a very very small aspect of jazz code which is where we are using it to write

**[1:52](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=112s)** our own AI scripts now the idea of writing code may sound daunting to a lot of people especially if you've had very little experience in coding before however as we'll learn throughout this tutorial video writing AI scripts and jazz is actually pretty right forward and easy to learn now in the last tutorial video we went through how to make our own AI scripts using the Warcraft 3 world editor where we learnt

**[2:26](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=146s)** things such as setting the AI to building train units and sending attack waves now the question lies why would we want to write our own AI scripts as opposed to creating them through the AI editor and there's two main reasons for that the first one being that when you're writing your own AI scripts you have the freedom to do whatever you want you can program your AI to do things that are not possible through this AI editor such

**[3:05](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=185s)** as if you want to make super smart AI that acts very close to how a player would act in certain situations there are numerous occasions on myself where I have written or used AI scripts and used it in certain situations that you cannot do using the AI editor for example when it comes to sending attack waves say for example you want your AI script to run because you want them to train units to defend their base and to collect

**[3:39](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=219s)** resources but you don't want them to send any attacks to the player until they've reached a certain point in the game there's no way of doing this using the AI editor another limitation that I've come across plenty of times is when it comes to setting the amount of seconds between each attack wave for example and you may want these attack waves how long they wait to be dynamic so for example you want the AI to

**[4:15](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=255s)** increase the number of attack waves if the player is playing on a harder difficulty or send less attack waves if they're playing on an easier difficulty again you can't do this with the AI at so now that I've explained jazz code and some of the advantages that it has over using the AI editor we are now ready to actually look at the actual code itself and how we can write our own AI scripts now the first thing we need to do is we

**[4:52](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=292s)** need a program in order to write our own AI code now if you really want to you can open your a AI scripts in notepad and you will be able to enter your code this way however my recommendation is to use a program called jazz craft which I will provide a link to download and install in the description below but what it is is it is a program that allows you to easily write jazz code and as we compare what the code looks like

**[5:34](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=334s)** in jazz craft as opposed to just opening up a note pad you can see that the code in jazz craft is much more visibly readable because it makes use of colors to color in the actual code and the background which is a lot harder to do in notepad where it is just plain black and white so without further ado this is a basic ki script that is used for one of the missions in the Warcraft 3 campaigns and I will go through line by

**[6:14](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=374s)** line exactly what is doing so first of all you've got code with hashes at the front like main or wave 1 jazz script demo etc and in programming these are called comments now when the AI script runs these lines don't do anything but they are there because when it comes to writing code you may want to as a programmer explain what the code actually does for example we could rename this main to explain more what

**[7:02](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=422s)** this particular function does so this will call when the AI script runs for the first time you can literally put whatever comments you want to explain to other people who read it what exactly it's trying to do now starting at the top we've got a section called Global's and what this does is it allows us to define our own variables we want to use within the AI script now for those who are unfamiliar what a variable is if you

**[7:45](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=465s)** have watched my previous tutorial videos you will know that in the AI editor for example when we want to specify a unit that's been created we can create a variable call it paladin and set it as a unit variable type and then we go into actions and we go set variable set paladin equal to and we select a paladin for example and then we can specify that paladin throughout our triggers so for example set the manor of paladin or

**[8:32](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=512s)** better paladin to 100 the same basis applies to what we're doing here so first of all we are setting a player variable called player two now what this is is it essentially is the player slot that you want to use for the AI script so what this AI script does is it sends attack waves to a player that you're able to specify which player the AI attacks so in this example if we go back into the editor you will notice when you

**[9:17](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=557s)** go to player properties is that there are player slots from 1 to 12 so in the AI script where it has a 2 for example it means that the AI will attack player 2 which is blue if we wanted to change it for example to player 4 it would mean that the the AI will attack player 4 for example etc so just as we can set variables for units there is one used to set for a unit here or Vera matharis the dreadlord

**[9:56](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=596s)** now what this is is is for every single units item tree whatever that exists within the game that is an actual object it has a four-digit code which is used to identify that unit item tree etc within the code with while you're coding in jazz so I'll explain briefly more about what this means so for example when we're using AI scripting and we want to say we want the AI to build a peasant now the AI scrip

**[10:42](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=642s)** you can't just say peasant because it doesn't know what a peasant is program computers don't speak English so instead it works by reading a four-digit code which we can see what it is by going into the object editor and pressing control D on the keyboard and notice that there is a bunch of four letters that appear before the peasant which is HP ei this is the actual code of the peasant itself so if we go into

**[11:21](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=681s)** here we could say for example go constant integer peasant custom equals HP ei now skipping ahead a bit you will notice that there are some English words used throughout this script such as acolytes abomination slaughterhouse etc within Warcraft 3 there are predefined variables that you can use for some common units so that you don't have to remember their four-digit code as you're writing these scripts and you can find

**[12:16](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=736s)** out what they are by when you're installing jazz crafts on your computer there will be a file you can open called common AI and when you open it if we scroll down a bit you will actually see all the predefined variables for different units buildings upgrades heroes etc that you can use so you can use arc mage paladin mountain king etc now that we have defined our Global's for example we can actually go through

**[13:03](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=783s)** this function called main which if those of you who don't know what a function is it's essentially a container of a bit of code that you want to run now main is called whenever the AI script runs for the first time so this it is within this function that you will actually order the AI to build units and send attack waves so starting from the very beginning we've got a few calls here and some of them are essential

**[13:47](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=827s)** while others are not so call campaign AI is an essential method and this is used for specifying what sort of food production building this AI will be relying on for their food supply so for example because we are writing an AI script for an undead faction we specify that we want their food production building to be a ziggurat if it's a knight of faction for example we would rename this to moon well and as you'll

**[14:23](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=863s)** see as you're writing the code in jazz craft if the function or if the variable that were writing turns to gray as we're writing it we can easily see them there's a predefined variable whereas if it's remained white it is not and we have to define the variable ourselves within this global spot so if we rewrite cigarettes one just undo here you can press ctrl-z to undo the code you write by the way which is pretty

**[15:08](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=908s)** standard in a lot of programs okay so this second call here set replacements say for example you have an AI team that has some units on the ground so for example if we have a base somewhere where we place the undead and we set a couple of ghouls and we place down an abomination now what this means is that this call allows us to say to the AI if the unit dies how many times that unit will actually be replaced so for example

**[15:55](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=955s)** when this abomination dies it means that the AI will retrain that abomination at a slaughterhouse and you'll notice that there are three numbers here and what these represent the different difficulties in Warcraft 3 so the first number is for easy difficulty the second number is for normal difficulty and the third number is for hard difficulty so what this means is for easy and normal difficulty the AI will only replace this

**[16:26](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=986s)** abomination once and then for hard difficulty the AI will replace the abomination three times and you can set this to whatever number you want this next call set slow chopping this is where you want the AI to only collect one gold from a gold line at a time or only cut down one lumber from a tree at a time so that the AI doesn't run out of resources which is the same feature that is available in the AI editor where we

**[17:05](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1025s)** can set slow harvesting for example and then the next call set peons repair is also another similar a feature that is in the AI editor where we can specify if the AI repairs their buildings if they're under attack or if they're damaged so these two calls you don't have to specify for an AI script since if you do not call them these will be true by default so we can get rid of these and it will do the exact same

**[17:45](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1065s)** thing as this but if you want them to be false for example so there's no slow chopping or buildings aren't repaired then you will need to specify them and type in false for example so it only accepts a true or a false value now this next section here is essentially we're telling the AI to build units and buildings which is if we go into the AI editor which is essentially the same as this part right here where we're

**[18:27](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1107s)** ordering the AI to build a peon within the J's code it's achieved in this line where the three numbers representing easy difficulty normal difficulty and hard difficulty so as we could see in this particular AI for example we've specified 0 for easy and normal difficulty meaning that the AI doesn't bother to rebuild their buildings if they're destroyed but they do on Hard difficulty and you will see the usual

**[19:03](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1143s)** undead buildings such as the necropolis the the crypt graveyard altar etc this next section here is where we specify units that are going to defend the base which is essentially different from this AI scripting here for the AI editor we simply say build this unit build the structure build this unit what this actually does is the unit that is trained using campaign defender act X actually behaves differently compared to

**[19:47](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1187s)** a normal unit that is trained this way because what this particular unit will do is it will actually go and patrol around the Town Hall of the AI and then defend at the base when it's under attack where as opposed to this call for example where it just builds a unit it just builds unit and it will only attack the player if they get within sight range of that unit so if you want the AI to defend themselves you make use of

**[20:26](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1226s)** these calls the next call where we go wait for signal now this is not essential to use but what it does is the AI will start running from here it will set the different options we want the AI to do it will train units and build structures and set defending units and then the AI script will simply stop here and wait for a signal within the triggers of the game and in Warcraft 3 what editor you can send this signal for

**[21:08](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1268s)** the AI to continue running by going into triggers and yet going into actions and go send AI command and you would send player 4 purple the AI command 0 0 now what numbers you use where the 0 or 1 or 2 it doesn't really matter what numbers you use all the AI is waiting for is just a command of any kind and then the AI will start or continue to run and this is useful for example where we want the AI to run

**[21:49](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1309s)** so that they train units and build structures but we don't want them to send any attacks to the player yet so this is a way you can achieve that by making use of this wait for call of wait for signal call now this next part here is where we simply send the attack waves so in the AI editor it's essentially we have an AI group and we sent and we specify what units and how many and then send the actual attack this is all done

**[22:31](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1351s)** through this small block of code so the very first thing you need to do is you need to call init assault group this is telling the AI that you are preparing the AI to send an attack at a player so you have to use this every time you want to send an attack wave and then you specify by unit type a campaign attacker X where you simply say the unit you want to send a be part of the attack wave which in this case is subbamma nation

**[23:07](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1387s)** and you have the easy normal hard to specify how many abominations within this particular attack wave and you can do this for as many different unit types you want such as we're making use of the Vera Mathis unit that we specified in the Global's part so and then the last part is where we actually send the attack web suicide on player X and we specify how many seconds it waits so this can either be a number so for

**[23:45](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1425s)** example 60 seconds 30 seconds or in this second attack wave it can be M 5 m 3 now what M 5 an m3 are they are also predefined variables within the common AI script and it is essentially used to make it easier to say for example I want the AI to wait three minutes before sending the attack wave so we make use of m3 or I want the AI to wait five minutes before sending the attack wave in which we say m5 so it is there's essentially a

**[24:29](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1469s)** predefined variable for every single minute from one minute to fifteen minutes that you can make use of so we can say M 15 we can say m 12 go M 3 M 2 you will again you will notice if the variable exists by typing it and it doesn't turn grey so and you'll see the next sort of bits of code is where we are also sending additional attack waves at the player and one thing I actually forgot to mention earlier is that this suicide

**[25:18](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1518s)** on player X call relies on the fourth number or part of the call to be the actual player we want to send the attack wave to which user is actually the predefined variable that we've specified in the global spot now if we really wanted to we could skip using this global variable and just say player 2 or play x2 or play x1 for the player slot 1 which is read for example by going back into the player properties the player

**[26:02](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1562s)** slot 1 is red player Splott player slot 2 is blue etc the reason why it is easy to set it as a variable is so if we do a change within the actual map where we change the actual player slot of the actual player that's being attacked so for example we want this AI to attack the playing player which is red and then all the sudden we change the actual player slot to blue the AI will not attack the correct player and it means

**[26:45](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1605s)** you have to update the code and make this change with every single time you reference the player however if we set it as a variable and set it in one place it means that if the player slot is changed we only have to change once as opposed to changing it multiple times throughout the code so continuing on you've got these additional calls set build upgrade X and this essentially tells the AI to research and upgrade

**[27:24](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1644s)** which again has the different the level of the upgrade for easy normal and hard which is exactly the same thing by going into building and going add property build specific upgrade now this last bit here is what is called a loop where what it will do what the game will do when it reads this code is it will start here it will call this attack wave it will call the second attack wave and it will call

**[28:10](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1690s)** all the attack waves that are within the loop and then it gets to the end and it goes back to the beginning of the loop and then runs the AI R code again so what this will do is it will run this bit of code inevitably which means it will run forever which is also something that is a bit tricky to achieve when this AI editor where it gets to the very last AI wave and you want the AI to start again and send it to tack waves

**[28:51](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1731s)** all over again this is easier to achieve in jazz code where we can specify a loop and we can we can put in all the different attack waves that we want within the in the code and then it will forever run one thing you will need to know when writing your AI scripts is that in this replies decoding general is that it is very easy to make a mistake whereas when you're writing your AR scripts and J's craft how it will notify

**[29:30](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1770s)** you if there is something that seems out of place by color-coordinating the code it is still somewhat easy to make an actual mistake within the code and it can be easily missed and debugging your code which is another word of saying proofreading your code to make sure it's right is something that you will have to do when writing your own code so for example we put in a random s here now if we input or import this AI script as it

**[30:14](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1814s)** is now into our actual map going into the import manager when we load the AI script to run if there is a bug in the AI script the AI script wrote run and so one technique you can use to help proofread your code and make sure that it works fine is by using a program called jazz demo we'll also be within the downloads of in the description of the video below but essentially what it does is it goes through your entire AI script and it

**[31:01](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1861s)** checks each line to make sure that there's no mistakes within the actual code so if we go to browse and we select on the AI script that we want to check which is this one here if we click on the check button it will come up with what areas they were here so it says here hey on line 39 there was this random s which doesn't belong so it tells you what line there is an error on and it will give an indication on what

**[31:34](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1894s)** exactly the error is so if we remove this s and save again and now we rerun the the program to check the AI script success no syntax errors found so in short we have achieved writing our AI scripts using two different methods we have the AI editor which we went through in our first tutorial video and then in the second video we've concentrated on writing our own AI scripts using J's code and that pretty much concludes this

**[32:18](https://www.youtube.com/watch?v=JEUNAPQGwpQ&t=1938s)** tutorial video I hope you guys enjoyed it if you like this video feel free to like this video and subscribe to my youtube channel but until next time guys I'll see you later bye
