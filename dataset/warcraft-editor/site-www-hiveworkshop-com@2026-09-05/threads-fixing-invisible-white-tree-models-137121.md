---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/fixing-invisible-white-tree-models.137121/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Fixing Invisible White Tree Models
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/fixing-invisible-white-tree-models.137121/>
#### Slaydon

#### Slaydon

 Level 20

JoinedJan 6, 2008Messages2,622

Resources: 10

#### Resources
Icons 4Tutorials 6

#
 How to correctly import a tree model

Hello, i was in the model section and downloaded a custom tree, i see the posts and they say "My Tree is white, it only works for units, etc... etc.."

Now, im going to tell you how to import one of these trees, as a "Tree"

Go up to your editor and Destructibles, Tree's and find "Support Column"

CnP That and rename to tree and disable "Selectable Ingame" Off and rename it to Tree and set the pathing to Ashenvale Tree Base, and then insert your model file to that object, and your done, Say goodbye to Unit / Doodad trees, hello real trees

###
 Why do we need to base it on another thing than a tree?

Because editing the model file of a tree, will not show cuz its a special kind of object, If you are going to use a tree, you need to use a skin, you cant edit the model file, if you take off the Art - Replaceable Texture file, you will see that its just white, so its a model thats, well, hard to explain, but its a shape and you can only edit the wrapping.

So if you tried so hard on creating a Destructible Tree, This will show you how.

Its not a big tutorial, but some ppl really have problem with this.

~Slaydon

#### Attachments

Tree Pic.png

 45.7 KB

 · Views: 3,939




 Last edited by a moderator: Aug 14, 2009

#### Septimus

#### Septimus

 Level 31

JoinedMay 3, 2008Messages3,146

Resources: 50

#### Resources
Packs 1Maps 35Spells and Systems 8Tutorials 6

https://www.hiveworkshop.com/forums...a-tutorial-please-read-before-posting-119320/

Make sure your signature is disabled.


 Last edited by a moderator: Jul 26, 2009

#### Slaydon

#### Slaydon

 Level 20

JoinedJan 6, 2008Messages2,622

Resources: 10

#### Resources
Icons 4Tutorials 6

~.~


Stupid meh, forgot, fixed now :d

#### Thyrael

#### Thyrael

 Level 11

JoinedApr 28, 2008Messages696

I really don't see the sense in this tutorial. To use a custom tree, simple import the needed model + textures. CnP an existing tree, change the modelfile of the copy to the desired one, uncheck "has lightwight model" and set variations to 1. Thats it.

#### Slaydon

#### Slaydon

 Level 20

JoinedJan 6, 2008Messages2,622

Resources: 10

#### Resources
Icons 4Tutorials 6

:O, well, this is a simpler way D:

#### Fiko13

#### Fiko13

 Level 2

JoinedJan 20, 2026Messages9

Hello Slaydon! Actually, I find your tutorial very helpful for one simple reason: Because when I copy an existing tree (like Thyrael suggested, and like I used to do up until now), the tree sometimes manifests unexpected complications.

For example, when I used some custom tree models for a destructible based on an originally copied tree, the model did not show - it simply stayed hidden while only showing shadows in its place. It really bugged me for quite some time, I spent like two hours searching for a solution. Not even lightweight model true/false check worked...

BUT when I used the very same model using the edited "support column" doodad instead, the custom tree model shows nicely.

So thank you!

 You must log in or register to reply here.


### Similar threads

 New Trees in World Editor


Tails96

Nov 18, 2009

Misc/General Mapping Tutorials

Replies24Views34K

Oct 4, 2018
Raka Uthada

 How To Change Textures on a Tree or other Doodad (Without importing several models)


InfernalTater

Sep 3, 2012

Misc/General Mapping Tutorials

Replies6Views19K

Mar 11, 2024
fostaa

L

Locked

 White Trees...Why?


lagger-

Nov 17, 2004

Map Development

Replies2Views1K

Nov 17, 2004
RaD-

Locked

 "White" Trees


Sp4WnEd

May 31, 2005

Map Development

Replies3Views1K

Jun 1, 2005
Sp4WnEd

Locked

 Why can't i import a tree model file?


FiremanBO

Apr 23, 2016

World Editor Help Zone

Replies5Views1K

Apr 24, 2016
FiremanBO

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top