---
type: vendor-doc
package: video-jokerjassvids
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=nSVc26P6DnE
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 366
timestamped: yes
---

# [1/2]Warcraft 3 vJass Tutorial 2: Basic vJass Ability

Source: <https://www.youtube.com/watch?v=nSVc26P6DnE>

Uploader: JokerJassVids | Duration: 6m06s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Hopefully I improved from the first. 

Recommended JASS modding sites: 
http://www.thehelper.net 
http://www.wc3c.net

NewGen:
http://www.wc3c.net/showthread.php?t=90999

## Transcript

**[0:06](https://www.youtube.com/watch?v=nSVc26P6DnE&t=6s)** hey guys this is Joker div from the helper.

**[0:10](https://www.youtube.com/watch?v=nSVc26P6DnE&t=10s)** net uh in this tutorial I'm going to show you how to create a basic spell written in BJs so as you can see I got everything set got the hero has my custom ability and my custom ability ability my bad is all set to go so you look here this code was written from GUI which I have which I had said earlier so the reason I do this is because it creates a lot of the coding for you so all you have to do is alter

**[0:54](https://www.youtube.com/watch?v=nSVc26P6DnE&t=54s)** it a little bit and it'll be much more time efficient now later when you feel more comfortable using Jazz it's a much better idea to create a template for yourself where all you would have to do is copy and paste the code then you could simply start from there for or Hold On by the way uh I'm not going in in any particular order so don't be confused by the order of things I am changing so this could be an example of

**[1:58](https://www.youtube.com/watch?v=nSVc26P6DnE&t=118s)** a template you there are ways to make it better but for now this would be a decent template to start with so now onto our spell if you're at the description it simply kills a Target so we're going to fancy it up a little bit and kill the unit after a short delay and create a special effect on the ding unit so get our caster and get our Target now we wait two seconds now trigger sleep actions are usually not the best way to the code

**[3:01](https://www.youtube.com/watch?v=nSVc26P6DnE&t=181s)** delays but for this case it's the better idea since timers are much more complicated and I don't want to get into structs and timers and things like that just quite yet so we will stick with trigger sleep action for now we will kill the we will kill the Target and then we will create the special effect now the reason I am putting destroy effect first is because most animations have a death animation which is usually long

**[3:53](https://www.youtube.com/watch?v=nSVc26P6DnE&t=233s)** enough for the players to see now it asks for a Model A string model name a widget or a unit and a string attach point so first for the model name what you want to do is go create a GUI this create a GUI and go to special effects since they already have a list for you there's no point for you to go dig it up elsewhere and the original editor shows you the previews so for this spell I guess I will do animate

**[4:44](https://www.youtube.com/watch?v=nSVc26P6DnE&t=284s)** that and hopefully you saw the pre preview right there convert this to Jaz copy the string or the path of the model paste it the units and the attach points units usually have four attach points they have the origin which is the bottom they have the chest which is the middle the head which is the top and overhead which is above their head so for this animation I will be using their origin now since we didn't use

**[5:30](https://www.youtube.com/watch?v=nSVc26P6DnE&t=330s)** a variable for the special effect there's no need to worry about ning but for these unit variables it is a good idea to know now usually Heroes tend to stay in the game forever so you don't have to know those but it's always safe to know all unit variables so here is our first spell when a unit
