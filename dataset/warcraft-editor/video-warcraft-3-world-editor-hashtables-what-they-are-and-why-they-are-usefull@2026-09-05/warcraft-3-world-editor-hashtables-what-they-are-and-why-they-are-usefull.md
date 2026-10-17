---
type: vendor-doc
package: video-keytorch
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=Y9GQ2X1W_vQ
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 614
timestamped: yes
---

# Warcraft 3 - World Editor - Hashtables - What They Are and Why They Are Usefull

Source: <https://www.youtube.com/watch?v=Y9GQ2X1W_vQ>

Uploader: KeyTorch | Duration: 10m14s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Hashtables are very nice learn when you want to create maps in the world editor. They give you the power to store unique information about all the units, sounds, effects, items, buildings, etc. And they can store A LOT of information. I hope you find the video usefull and learn something!

Take a look at the other videos in my Warcraft playlist if you haven't already. They might be easier to understand if this video is too complicated.

Sound Effects from https://pixabay.com

## Transcript

**[0:02](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=2s)** our trigger and H spell from the last video works well when one hero cast it at a time but if two Heroes were to cast it at the same time we would have a problem variables would overwrite each other and only one of the heroes would get the effect of the spell there is a solution to this though we're going to use something called hash [Music] tables but what is a hash table a hash table is something that stores

**[0:29](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=29s)** information it can store reals integers booleans strings and handles and it can store a lot of them because it organizes the data in a coordinate type of system we call the x-axis for keys and the y- AIS for indexes it's like a list of lists all the Dots here represent a point in the hash table where we can store a value now let's say we want to store a value here the key would be four and the index will be two this is because the

**[0:59](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=59s)** hashtable starts at zero on the X and Y AIS let's quickly look at storing a value to the hash table if we choose new action and change the type to hashtable we can see that there is a long list of commands but there are essentially just three things you can do you can create a new hash table you can save something to it and you can destroy a hashtable let's create a hash table and save a number to it find the hash table actions and pick

**[1:27](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=87s)** save integer now we get quite a few options it can be a bit confusing in the beginning this window but it'll make sense pretty soon the first value here is the value that you want to store the second value is the key which you can think of as the value along the x- axxis and the last value is the index which will be your value along the y- AIS I'm going to add the number 42 as a value I'm going to choose four as the key and

**[1:56](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=116s)** two as the index and remember to organize your actions now I'm going to try and retrieve the number from the hash table and print it to the chat add a text message action and change the text first we have to choose conversion from integer to string we have to do this because integers are numbers that the computer uses to do calculations with and strings are texts that we can display then for the integer

**[2:25](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=145s)** we want to convert which chuse the function hashtable load integer value the first value here is our key the x-axis and the second value here is our index the y- axis and again we have to remember to organize our actions and then create an event so our trigger will run and TAA we've now saved and retrieved the value from a hash table but what are hash tables good for and how does it help us fix our spell hash

**[2:58](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=178s)** tables are really helpful when it comes to storing information about stuff in your map with a hashtable we can for example store different information about every unit in the game like how many times it has entered a portal casted a spell or used an item to do this we need the ID of the unit we can get this with something called handles what is it it's basically just a data type a special way to store

**[3:25](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=205s)** numbers and it's used to store stuff for all the things in your map like the ones you can see here the way we're going to use the unit ID is to set it as the key in the hash table and with that key we can store for example a position on index zero an integer on index one etc etc and this will be unique information for this unit let me show you a simple example of getting a handle of a unit I've created a new trigger and I'm

**[3:54](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=234s)** executing it every 2 seconds I'm then creating a footman that we want to get the ID of to display the handle of the unit I will add a text message action the handle is an integer so I have to choose convert integer to string then for the integer I want to convert I choose get handle ID and for the key I will choose last created unit and remember to reorganize we can now see the unique ID of each unit now let's implement this into our

**[4:33](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=273s)** superbolt spell so far we have created a hash table inside the trigger that we use the hash table in this will not work since it will not be accessible after the trigger has finished instead we need to create the global variable and store the hash table to that I'm creating a global variable and setting its type to Hash table but this is not enough if we want to use it we also have to create a hash table like before and then save

**[4:59](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=299s)** that hash table to our Global hash table variable I'm doing it here in the initialization trigger so it will be ready to use right after the map loads to be well organized I'm renaming the trigger to something more descriptive I'm then saving a location handle which is the same as a position and I'm saving that to the hash table that we just created the position I'm going to save is Target position of ability being cast

**[5:59](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=359s)** which is the same position that we used to store in a single variable before as the key in the hash table we will use the unit ID of the triggering unit and we'll set the index to zero remember to choose our Global hash table and not the last created then we have to change our unit action to get the position from the hash table instead of the variable the first value here will be the key which will be the ID of our

**[6:43](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=403s)** unit and the second value is the index which is just zero and remember to change the hash table to our Global hash table now we need to do exactly the same for the position where we order our D unit to cast the blizzard spell and if we test our spell now you can see that it works exactly like before but this time we're using a hash table to store our position instead and this position is stor only for our

**[7:31](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=451s)** hero but again there is a leak that we need to take care of every time we cast this spell the First Position will be overwritten by the next position but the first position isn't actually removed it is still stored in memory so each time we cast our spell we also need to destroy the position to do this we're going to reuse our variable because if we store the position from our hash table into our single variable

**[8:00](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=480s)** and destroy the variable with our JavaScript we actually also destroy the variable in the hash table because the single variable references the hashtable value and both of them gets destroyed I'm now going to rename our variable to something more descriptive then I'm going to copy the name so we can paste it into the JavaScript and I'll create a new text action so we can see the changes for when we're using the variable to remove

**[8:56](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=536s)** the value and when we're not using it to remove the value I'm choosing convert real to string here instead of integer to string because the position is a real value I'm then going to copy it and paste it under the custom script so you can see that the value actually gets removed as you wish and there you go the value is removed from the hash table and if we disable the single variable you can see that the value is still in the

**[10:05](https://www.youtube.com/watch?v=Y9GQ2X1W_vQ&t=605s)** hash table thank you so much for watching I hope you learn something from this video and have a great day time to go
