---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/classical-ribbons-vs-particles-ribbons.7320/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Classical Ribbons vs Particles Ribbons
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/classical-ribbons-vs-particles-ribbons.7320/>
#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Classical ribbons vs particles ribbons


1. Introduction


Even though blizzard’s classical ribbons have become extremely popular and they are being used by many modelers, in some cases, regardless the low number of polies, their quality can be really poor, and simply not serve the purpose you seek. Deciding to experiment with blizzard particle emmiters, I have succeeded to come up with a new way of creating ribbons.


In order to be able to complete the tutorial successfully, you will need 3dsmax with Art Tools. I will not teach you here modeling, ribbons, or particles, but just this technique I have come up to, step by step.


2. Setting the particles ready


Go to geometry – Particle Systems. Select BlizPart and create a group of particles.


Having them selected, apply the following settings (blue are fixed values, red vary)

Emitter Options


Speed: -0.5


Cone Angle: 0

Timing Options


Life(sec): 1.00


Parts/sec: 50.00

Size Options


Width: 5,0


Length: 5,0

Particle Options

Head

Other Options


Particles in model space: off


XY Quads: off

Click to expand...


The important parameters here are the variable ones. Be very careful how you change them.


I usually keep speed to a small value. I also find the ribbons better when the value is negative. Never give speed a value of 0 because then the ribbon will look flat.


Life also depends a lot on the speed. If you add a longer lifetime, make sure that speed is smaller and vice-versa.


Number of particles/second is also very important. On them depends the number of polies your effect will have. Too many will mean a highpoly model. Too small may make your ribbon look like it has been cut into pieces.


3. Creating the ribbon


If you keep the particles in place you will obtain the head of the ribbon but no tail. In order to give the effect of a ribbon you must move the particles around.


Note: Moving the particles too fast may require more “particles/sec” unless you want to obtain an interrupted ribbon. You can lower in this case polycount by reducing their lifetime.


I have attached two examples comparing the two types of ribbons.


4. Which to choose?


It’s a good question. You can choose any of them, since both will do in most situations. Here are the differences between the two types of ribbons:

 Code:

Classical ribbons Emitter ribbons
¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯ ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯
Always uninterrupted Interrupted when moved too fast.
Invisible when staying in place. Head visible when steady.
Excellent for long textures. Excellent for ribbons made out
 of smaller pieces.
Lower polycount. Higher polycount.
Stretched texture in some situations. Always good mapping.


I cannot tell you where to use one or another. This is totally up to you and your needs. Experiment with both a little and then decide which one would fit better the model (and the poly limit you can afford).


If you have any questions, suggestions or comments, do not hesitate to post them here.


~Daelin

#### Attachments

RibbonsExample1.jpg

 58.6 KB

 · Views: 2,191



RibbonsExample2.jpg

 61.9 KB

 · Views: 1,990



 You must log in or register to reply here.


### Similar threads

 Basics of Special Effects (SFX)


Daelin

Nov 21, 2006

3D Modeling Tutorials

Replies1Views30K

Nov 21, 2006
Archian

Locked

 Ribbon Making Program?


FhelZone

Feb 11, 2008

Warcraft Editing Tools

Replies1Views1K

Feb 12, 2008
D a w n

A

Locked

 Strange ribbon emitter behavior


Ardenian

Nov 23, 2015

Modeling & Animation

Replies6Views2K

Nov 25, 2015
Ardenian

A

Locked

 Rotating Totem Ribbon Emitters, HELP!


shamanyouranus

Aug 28, 2008

Modeling & Animation

Replies1Views1K

Sep 4, 2008
anarchianbedlam

Locked

 Ribbon help - Tyrael model


expresso

Aug 26, 2007

Modeling & Animation

Replies19Views17K

Nov 29, 2007
Hack

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

3D Modeling Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top