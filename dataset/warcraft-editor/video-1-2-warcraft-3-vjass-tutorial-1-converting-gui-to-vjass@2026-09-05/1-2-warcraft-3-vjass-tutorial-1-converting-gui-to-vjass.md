---
type: vendor-doc
package: video-jokerjassvids
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=TuN1mgLw7ms
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 382
timestamped: yes
---

# [1/2]Warcraft 3 vJass Tutorial 1: Converting GUI to vJass

Source: <https://www.youtube.com/watch?v=TuN1mgLw7ms>

Uploader: JokerJassVids | Duration: 6m22s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Bear with me, this was my first video and tutorial ever.  I'm sorry if I miss/skip a lot of things, I promise I'll do better with time. :)

Recommended JASS modding sites:
http://www.thehelper.net
http://www.wc3c.net

NewGen:
http://www.wc3c.net/showthread.php?t=90999

## Transcript

**[0:08](https://www.youtube.com/watch?v=TuN1mgLw7ms&t=8s)** hey guys this is jokerd from the.net I was reading some posts the other day on the Jazz section where some people wanted video Jazz tutorials so here's my first attempt at creating a tutorial I was checking out some other tutorials for Jaz in YouTube and I all I found was some guys creating random scripts with annoying music in the background so hopefully I will do much better than those uh I'm going to be very brief

**[0:48](https://www.youtube.com/watch?v=TuN1mgLw7ms&t=48s)** about the basics since there's a lot of tutorials already out there so in this video all I'm going to show you is converting this into VJ I'm going to be skipping Jaz because VJ is simply Jazz with extra features so if I convert this to Jaz you can see the event condition and action and this is the function for the condition this is the function for the action so V Jazz let you create scopes which allows

**[1:35](https://www.youtube.com/watch?v=TuN1mgLw7ms&t=95s)** encapsulation or however you say it there's a thread in the Jaz section earlier about why we do this and you can check it out later right scope and scope now what this lets you do is you are able to declare public and privates the private means that that function is only allowed to be used inside the scope so it lets you create functions with the same names for example for this trigger I am going to name this

**[2:28](https://www.youtube.com/watch?v=TuN1mgLw7ms&t=148s)** conditions and this actions this makes your coding a lot easier to read also down here this is a GUI Global trigger which is unnecessary so I can replace that with a local trigger and there it looks much cleaner already now the reason I don't know this trigger is because it is never ever going to be destroyed so it is pretty pointless to know the variable if this trigger was never going to be destroyed all right now GUI requires

**[3:21](https://www.youtube.com/watch?v=TuN1mgLw7ms&t=201s)** this in it trig to run this initializer the vjas allows you to create your own so you can name it whatever you want I will name mine in it now this condition that GUI makes is pretty messy not sure why it does this but all you have to do is just simply cut this up and replace that and delete this rest of this junk now what you should do next is to get rid of this GUI variable it's a global variable which is pretty unnecessary for

**[4:19](https://www.youtube.com/watch?v=TuN1mgLw7ms&t=259s)** this task and also VJ allows you to declare your own globals inside your own Scopes which I will get into in later videos so now I have to replace all these with you and I am ging this variable U because I won't know if this unit is going to stay alive in the game for the entire duration so it is usually it's smart to know unit variables now this function here is called a blizzard fun blizzard Jazz if

**[5:12](https://www.youtube.com/watch?v=TuN1mgLw7ms&t=312s)** you look at what it is it's this loop with groups and such that blizard does and it's pretty inefficient so injest what you should do in creating units is use this function instead it is a native so it is much faster and much more efficient all right and this is our final product
