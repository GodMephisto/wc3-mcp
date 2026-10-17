---
type: vendor-doc
package: video-thenerdsocietyadmin
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=wg-CV4x_seU
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 608
timestamped: yes
---

# [HD] Warcraft 3 world editor, player life (how to create a TD part2)

Source: <https://www.youtube.com/watch?v=wg-CV4x_seU>

Uploader: TheNerdSocietyAdmin | Duration: 10m08s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

[NEW ACCOUNT: tobitherobitv] This is part 2 of the tutorial: how to make the basic triggers in youre Tower Defense. In part 2 i'am going to show you how to create a leaking life system, when a player leaks a unit, the player loses a life. And again, if you have any suggestions for a thing i should put it one of my tutorials, please let me know!
Thenerdsociety.com

## Transcript

**[0:01](https://www.youtube.com/watch?v=wg-CV4x_seU&t=1s)** hello this is top of the Robie from the nurse I calm here to give you the part 2 of how to create at our defense tutorial in the Warcraft 3 world editor in this tutorial I'm gonna show you how to create a losing life system so on the enemy unit enters the final region the leaking player was a life pretty simple but this is as I said earlier and the player sorry the part 2 of this tutorial I recommend you watch part 1 but it's

**[0:38](https://www.youtube.com/watch?v=wg-CV4x_seU&t=38s)** not acquired I just recommend you to do it anyway if you have been following along and if you watch the part 1 of this trail you will have trigger which enables the bounty of player brown and in the same trigger we're going to create an action but first we're gonna have to create the variable we're going to be using just click control B or click on the variable button create a new variable call it where your red life

**[1:16](https://www.youtube.com/watch?v=wg-CV4x_seU&t=76s)** all right look II go down and find integer integers are numbers with no comma no decimals in other words you can't like write 3.12 doesn't can't do that you can only write like 1 2 3 4 5 6 7 8 but if you have a real you can write with commas but I don't need any comment for this or decimal I only need 1 2 3 4 5 numbers and I'm just gonna pick integer and click OK and now go up and find if you need action button control are if

**[2:01](https://www.youtube.com/watch?v=wg-CV4x_seU&t=121s)** you want to do that I click s and it's that variable that's called and set which variable the only viable I have created life sorry they're real life okay and we're gonna set this to true 30 so that's our amount you know the amount of life a player has if you want more or less you can of course of course you can pick that like 20 or 50 or 40 it doesn't matter you can pick anything this is just the amount of lives

**[2:37](https://www.youtube.com/watch?v=wg-CV4x_seU&t=157s)** the player is going to have all right now create a new trigger and this one is going to be called let's say PLR in other words player read loose life the event is going to be when a union enters the final region so cook you find unit unit enters region which region we're gonna select our region yeah your own arm sorry last region you know the final region if the unit enters that region need a condition because we only

**[3:19](https://www.youtube.com/watch?v=wg-CV4x_seU&t=199s)** want our enemy when our enemy unit enters the final region that's the only time we're going to lose life for example if your builder went in there and you would just lose life I'd be kind of weird and so when to check what your owner which is the owner of the entering in it so go ahead and click on the condition button go down and find click P player comparison on rough on if unit owner off entering you know so honor for

**[3:52](https://www.youtube.com/watch?v=wg-CV4x_seU&t=232s)** entering an equal to player 12 or brown alright so after entering your equals to player brown then do the action and the action is the - life action yes that variable which variable do you play real-life rebel and this is kinda interesting now we're gonna go ahead and find the arithmetic I think it's pronounced that way anyway that's value plus value or in this case valuables one but we're gonna put it in clear red life that's

**[4:34](https://www.youtube.com/watch?v=wg-CV4x_seU&t=274s)** the first value and quick - up here and the amount of lives you want the pleasure loose each time the Union enters the final region I'm just gonna pick one so there you go when the unit into the region or if entering or entering equal to player Brown then set plea real-life to itself minus one so if you play your house 30 lives left it will set itself to 29 and if he asks 29 I left it will status set

**[5:15](https://www.youtube.com/watch?v=wg-CV4x_seU&t=315s)** itself to 28 fairly simple but and one problem now the player doesn't know how much life you got how many lives you got so we got to tell him go ahead and take any action or control R plus G game fixed all the time you just play to play red since playing red is the leaking the player here at least in my case and the text I'm going to find constants innate strengths I think its name uncle player red just lost one life and

**[6:09](https://www.youtube.com/watch?v=wg-CV4x_seU&t=369s)** yes alright that's it um player red just lost in life and has because Jing's tubeless string to string go ahead and find conversion convert integer to string or if you have a real convert every other string we have on integer can wear it Institute to string cooking that and find your variable okay so now it will display on is true yeah value of their read life in other words the lives of our leaking player

**[6:50](https://www.youtube.com/watch?v=wg-CV4x_seU&t=410s)** okay has lost amount of lives there's night life okay so when you test this map it will probably say play you read just lost my life and has amount of life also in other words they read play read life amount to us it will say life's left I want to watch out that you have I have though that you take other it wasn't name of that you press the spacebar you get out some area in between okay so what this does you

**[7:34](https://www.youtube.com/watch?v=wg-CV4x_seU&t=454s)** listen life the Bible and it will display the player what how many lives he has left and that in just lost life but now we got then that part finished and now we have the variable stored you can also do this when this happens you go ahead and create a new action and click I if then else multiple functions okay and this is a genius little thing you can put a condition inside the action kind of if then else yeah

**[8:21](https://www.youtube.com/watch?v=wg-CV4x_seU&t=501s)** so now we're gonna check if this guy is zero alright integer comparison as our Bible equal to 0 so if weif of red life pralaya ready life equal to zero then we're gonna set the defeat action if the red glare red light equal to zero so taking new action I'm go and find click G find game feet field play red with message you're a message okay kind of lame but we'll stick with that else do nothing oh gosh the upper

**[9:40](https://www.youtube.com/watch?v=wg-CV4x_seU&t=580s)** action okay and do nothing so here we go you got um you got a basic TD with no towers but who cares I hope you know how to do that on your own India object editor anyways thanks for watching I'll be coming out with more tutorials soon see you then bye bye
