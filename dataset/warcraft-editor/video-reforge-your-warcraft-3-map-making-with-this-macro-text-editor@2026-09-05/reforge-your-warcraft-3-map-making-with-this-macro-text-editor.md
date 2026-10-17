---
type: vendor-doc
package: video-ai-gamer
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=lgQMasnwsk0
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 602
timestamped: yes
---

# Reforge Your Warcraft 3 Map Making with This Macro Text Editor!

Source: <https://www.youtube.com/watch?v=lgQMasnwsk0>

Uploader: AI Gamer | Duration: 10m02s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

https://github.com/speige/WC3TextEditor

This is a custom TextEditor control I created for the Warcraft 3 World Editor. I've open-sourced it, so you can allow your users to enter long-form custom text in your maps. I also show how to use it as a code editor so gamers can execute commands as if it were a console terminal. Also useful for in-game debugging.

In this video, we will introduce you to a powerful tool that will revolutionize your Warcraft III map making experience - the Macro Text Editor. We'll guide you through the process of downloading and installing the Macro Text Editor, and then show you how to use its features to improve your Warcraft III map. Whether you're a beginner or an experienced map maker, this tool will help you take your maps to the next level.

## Transcript

**[0:01](https://www.youtube.com/watch?v=lgQMasnwsk0&t=1s)** hey guys this is a demo of a text editor that I created for the Warcraft 3 map editor so this is a mod that if you're a map maker you could copy and paste this code into your map to get this feature so why I made this text editor is if you hit the enter key you can type you know a random message but this is a very rudimentary text editor that's built into the Warcraft 3 engine you can't do anything except type one line of text

**[0:25](https://www.youtube.com/watch?v=lgQMasnwsk0&t=25s)** and you can't use arrow keys it's just going to scroll your map and so if you make a mistake you got to hit the backspace to delete before you can start typing again I mean it's worse than notepad you can't do undo you can't copy and paste nothing and then obviously once you hit enter it's going to display on the screen which you could use a trigger if you wanted to get that data out and not necessarily display it to

**[0:45](https://www.youtube.com/watch?v=lgQMasnwsk0&t=45s)** the user but for the map that I'm making I wanted something much more advanced so let me pull up my text editor so you can see what I'm talking about so right now it's set up to be full screen but I have a basic API for my text editor that is documented on my GitHub so if you use this code you don't necessarily have to make a full screen but anyways if I type something like this it is a message if I

**[1:06](https://www.youtube.com/watch?v=lgQMasnwsk0&t=66s)** want to use the arrow keys I can and you can see the cursor moves so I can type in the middle I don't have to delete everything that I had typed in addition I can hit the enter key and I can get a second line of text and then typical shortcut keys that work in other text editors are going to work here so if I want to go to the front of the line I hit home if I want to go to the end of the line I hit end and so if I wanted to

**[1:27](https://www.youtube.com/watch?v=lgQMasnwsk0&t=87s)** tap something in I could you know hit home and then hit Tab and I can move one space at a time of the tab but if I hit delete at the beginning of the tab it'll delete the whole thing I can also hit Ctrl Z to undo and so you'll see a little like one key at a time it'll start deleting as I hit Ctrl Z but then if I hit Ctrl y for redo it will start retyping it all okay so then also if I hold Ctrl and hit home it takes me to

**[1:53](https://www.youtube.com/watch?v=lgQMasnwsk0&t=113s)** the front of the file and control end to the end so instead of one line at a time it's the entire text and if I hit control with arrows it'll a word at a time so control left it'll jump a whole word at a time and control right same thing and if you hit the backspace it does one character at a time backwards but if you hit the delete key goes one character forward at a time that's deleting I'm going to undo that but if

**[2:16](https://www.youtube.com/watch?v=lgQMasnwsk0&t=136s)** instead I hold the control key when I hit backspace it'll delete one word at a time gonna do and then if I did the control delete key it'll do the same thing but one word at a time gets deleted in front if you wanted to move a line of text you can hit Ctrl X for cut now you can't select anything and so it's just going to do the entire line but since it's not been cut I could come down here and paste it with Ctrl V and because it's in

**[2:39](https://www.youtube.com/watch?v=lgQMasnwsk0&t=159s)** my clipboard I can paste it multiple times however this does not access the operating system clipboard so it's just going to be inside of Warcraft that you could copy and paste and it's only going to be inside of this text editor unless you as a map maker make that variable available to other parts of the warcraft engine and that's due to the limitation of the warcraft's engine itself it does not give you access to the clipboard as

**[3:02](https://www.youtube.com/watch?v=lgQMasnwsk0&t=182s)** far as I've been able to find out unless someone comes up with a hack and that's a point to bring up as far as mods in general for Warcraft you have to work within the confines of the engine so for example if you notice at the bottom of the screen here when I hit enter it will open and close the message for a Split Second and that's a hack I had to do because I could not get it to never show up at all but I did find the workaround

**[3:25](https://www.youtube.com/watch?v=lgQMasnwsk0&t=205s)** which was just to hide it as soon as it shows up you can look at the code on my GitHub I'll put it up here at the top so if you wanted to like see how I coded this whole thing or if you want to put into your map there's instructions on this GitHub for how to copy and paste the files into your map and also documentation on what apis it provides but um the way I hide that chat menu is I basically click the menu up here and

**[3:48](https://www.youtube.com/watch?v=lgQMasnwsk0&t=228s)** then I instantly cancel it so you don't actually see the menu pop up but it does force the chat to hide um so that's just one particular hack so other hacks I had to do for example Warcraft when you use arrow keys it's going to scroll uh and if you hit the delete key it's going to rotate so those are also hacks I had to do to like basically lock the camera bounds and turn on like cinematic camera mode to

**[4:12](https://www.youtube.com/watch?v=lgQMasnwsk0&t=252s)** prevent when you hit delete or when you hit the arrows to prevent the camera from moving around and then some features I wish I could have done but I just haven't done yet and maybe I'll get to eventually is for example I mentioned that you can cut and paste a line at a time with control X and Ctrl V I prefer to be able to hold the shift and use the arrows to actually highlight it or to use the mouse to highlight it I that's

**[4:35](https://www.youtube.com/watch?v=lgQMasnwsk0&t=275s)** not something I've coded yet so that's why it's one line at a time um also I didn't mention but you can also do control C so if you don't want to cut the line you just want to copy it so I guess I'll do this line down here copy it and then I'll paste it up here and then I also don't support the caps lock key so if you want to type in all caps you have to hold down the shift key while you're typing if I just hit caps

**[4:53](https://www.youtube.com/watch?v=lgQMasnwsk0&t=293s)** lock and type it without shift it's going to be lowercase so that's something that I probably could code I just didn't bother so feel free to create a pull request if that's important to you and then one other limitation that I chose not to fix because I don't think it's a huge deal is say you type something like an exclamation mark because exclamation mark is done by holding shift and pressing the number one then that is

**[5:16](https://www.youtube.com/watch?v=lgQMasnwsk0&t=316s)** also a hotkey for creating a control group in Warcraft which is basically highlight a bunch of units and then save those units to that number so you can reactivate those units in your selection later so if you're as you're typing it's it thinks that you're using that hotkey to create a control group even though you're in the text editor so that's something that probably could be hacked around but it doesn't really hurt

**[5:39](https://www.youtube.com/watch?v=lgQMasnwsk0&t=339s)** anything so I just chose to leave it it is slightly annoying to the user so if it bothers you in your map feel free to create a pull request to fix it and then one last feature I didn't show is if you hold the ALT key and use the up and down arrows it will move a line of text at a time so this just obviously a much more advanced editor than what's built in so I'm going to delete all this code well it doesn't look like code it's just a

**[6:06](https://www.youtube.com/watch?v=lgQMasnwsk0&t=366s)** bunch of text but I put comments there because in this particular demo the way I've set this up is it actually will execute the code when you hit Escape which closes the text editor so for example this is Lua code so I said print the letter A on the screen so if I had Escape type A shows up on the screen that is actually the real reason I have created this text editor is because I'm making a map where I want a console or

**[6:29](https://www.youtube.com/watch?v=lgQMasnwsk0&t=389s)** terminal window for my map similar to like in Minecraft where people can type in their own mods while they're playing the game and that was the reason I created this text editor so you could use this text editor for anything you want it's not designed to run code when you close the text editor that's not part of this custom control on my GitHub that is a separate piece that's coded just for my map so it's not too

**[6:54](https://www.youtube.com/watch?v=lgQMasnwsk0&t=414s)** complicated essentially Lua has a load function which if you pass it any arbitrary text it will try to execute it as if it's code so let me show why this would be really cool to have on your map Blizzard's engine has a built-in function called create unit and the first parameter is the player that the unit should be created for the next parameter is a unit type and 4 CC stands for character code so all of the units

**[7:24](https://www.youtube.com/watch?v=lgQMasnwsk0&t=444s)** in Warcraft have a string identifier that's four characters long and so hpea stands for human peasant and that's just a string variable but when I pass it to the 4cc function it converts it to a unit type and then the next three parameters are the location on the map for it to create it and the direction that it should be rotated so it's going to run this actual code so if I hit Escape I now have a unit created which I

**[7:52](https://www.youtube.com/watch?v=lgQMasnwsk0&t=472s)** can select you know move it around tell it to attack you know anything a normal unit could do so blizzard has a huge number of apis that they offer to map makers to do custom mods and if you wanted to like in this demo you could allow your users full access to those apis so that they could do anything crazy they wanted in your map while it's running or you could use it as a map maker for debugging purposes so you

**[8:19](https://www.youtube.com/watch?v=lgQMasnwsk0&t=499s)** could have only available to you and instead of doing a lot of setup just to find out you've got a bug in your code and then go kill the game fix the code re-run it and then do all that setup again to get to that exact moment in time you can instead just modify the code directly in here for testing purposes and then after it's all working then you could put it into the real map so just some ideas where it might be

**[8:43](https://www.youtube.com/watch?v=lgQMasnwsk0&t=523s)** useful and then one last hack to mention that I had to do you'll notice as I have the peasant selected if I Mouse over these different commands I can have them do they all have hotkeys so the letter a is going to do attack so I could click it but I could also just press the letter A to do that command well in the text editor I don't want as they're typing the letter A or for example like if you right click it'll make the

**[9:09](https://www.youtube.com/watch?v=lgQMasnwsk0&t=549s)** character move if they right click I don't want the character to move the hack I did for that is if you have a unit selected and you open up the text editor it deselects the unit so you'll notice a green circle around them is gone his portrait disappeared his actions disappeared so if I press a which would normally be attack it's not going to do it because he's not selected but it remembers your collection so as

**[9:33](https://www.youtube.com/watch?v=lgQMasnwsk0&t=573s)** soon as you close the text editor he gets reselected so again that's a hack just to make you aware but I don't think it's going to affect your gameplay and your map so it shouldn't be an issue thanks for watching hopefully this control comes in useful to you if you're a map maker and uh please provide feedback if you notice any bugs submit issues on my GitHub or if you have questions about how to make your own

**[9:55](https://www.youtube.com/watch?v=lgQMasnwsk0&t=595s)** Warcraft Maps or controls send a message on my YouTube and I hope this mod is helpful for you thanks
