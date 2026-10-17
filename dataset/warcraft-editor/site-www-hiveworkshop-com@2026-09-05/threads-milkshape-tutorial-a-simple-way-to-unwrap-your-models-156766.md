---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/milkshape-tutorial-a-simple-way-to-unwrap-your-models.156766/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Milkshape Tutorial a Simple Way to Unwrap Your Models
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/milkshape-tutorial-a-simple-way-to-unwrap-your-models.156766/>
#### Mulgrim

#### Mulgrim

 Level 15

JoinedMar 31, 2009Messages1,556

Resources: 4

#### Resources
Models 3Tutorials 1

Milkshape tutorial - A simple way to unwrap your models.

 In this tutorial, you will learn my technique of unwrapping models in Milkshape 3d. If you are tired of trying to guess how to make your textures fit by just looking at the model, or if you're like me, wondering how to start at all, then let's begin.

IMPORTANT: I'll be assuming you know the basics of modeling; how to make a model, how to export from Wc3 Model Editor, how to UV map, etc.

Necessary Tools:

Milkshape 3d, (duh)

A texturing program, for this tutorial I will be using Photoshop CS4, but if you need one, I'd recommend downloading the latest version of GIMP for free here.

Microsoft paint, but that's just me, if you want you can use your texturing program.

Wc3 Model Editor, you'll just need it for this tutorial, we'll be exporting a model from the mpq to use for this tutorial.

 For this tutorial we will be using Grunt.mdx, so go export that right now as .mdx

Getting started

1. Open MS paint and resize it to something that will be comfortable for you to start your texture. Let's try 800x600. to do that go under Image, and click Attributes (ctrl + E). Next you'll want to insert: Width: 800, and Height: 600.Make sure Pixels and Color is checked.

Note: I know it's a big size, but you can resize it later, right now we're worrying about unwrapping only.

2. Pick any color you'd like, hell, pick your favorite if you want to, but make sure it will be noticeable against white, so pick a dark color. Then choose the Bucket tool and paint your backround of that dark color.

Now save this and keep it, we'll be using it later.

3. Open up your model, or in this case, Grunt.mdx. in Milkshape 3d. If you want, right click on the 3d window and click Wireframe Overlay, it helps see what you are selecting.

Delete all the unnecessary parts, which are: Mesh01, Mesh04, Mesh05, Mesh06, and Mesh07

4. All done? good. Now what we have left (or should have left...) is the grunt and his axe (Meshes 02, 03 and 08). Now go to the Materials tab and click on Material01 and give it the background we just made on paint.Remove the other materials, we won't be needing those for the unwrap.

5. Perfect, now with the remaining things, select everything that you think goes "separately", example: the head, the arms, the rest of the body and the axe. But in other models, these would sometimes have different textures for different viewing perspectives (top, back, left, etc.) An example of this would be the inside of the mouth of the grunt, it's "separate" and has a teeth texture that would fit under the top viewing perspective. So now that you know, I want you to find all these things that go "Separately" and assign them to their own different groups (don't worry, they won't stay in separate groups like that forever).

I have regrouped the inner mouth, the horn on the helmet (just one, we'll be mirroring that after mapping it), one tusk, the chest, right arm, left arm, axe, cap, face, and legs. The left over horn and tusk can be deleted, we won't be using them until we map them.

Note: I know there are many things I left out, like the back of the head and legs, etc. But I won't be covering that in this tutorial, because it is not necessary. Though if your custom models need it, do it, separate them into different groups for this step.

6. Make sure all groups are assigned to Material01. Now we select every group, starting from the first, and open our Texture Coordinator (ctrl + T). I have selected Mesh02 here. Make a region somewhere to make it smaller. Remap it to the Front on the coordinator, click "Fit Selection" and take a screenshot of it.

Great, now open Paint again, and go to Edit, and click Paste (ctrl + V). Now try to crop only the part that contains our mesh, like so. (You do this by clicking on the Select tool, and make a square around it, then go to Image, and click Crop.) Now, save it.

Repeat this for every group.

Note: make sure to use common sense when doing this, you don't want to remap a horn or tusk from the front, that would be stupid because they are tilted to the front. Instead put them on a right view, same for the inner mouth.

7. Open every image on your texturing program and lay the screenshots out, cut them each piece out and place it as to save space. When you're done, crop it. This is what mine looks like.

Once that is done, edit each piece and start your texture, and after that, you map it, congratulations you're done! here's my mapped result (lol) Ignore the fact that on that screenshot, there's like a hundred groups still, don't keep those like that, after your model is fully mapped, regroup them.

Remember to resize your texture to a power of 2 or else it won't convert to .blp!

Note: By the way, I messed up a bit during coloring, so that explains the black parts on the body, but i figured that you'd get the message of what I'm trying to teach by this point. Also, I know it's not the best unwrap, but you get the idea.

...And that's all, hope you enjoyed!

#### Attachments

pic 6.jpg

 36.8 KB

 · Views: 1,120




 Last edited by a moderator: Apr 30, 2010

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

your unwrap is a complete mess

#### Mulgrim

#### Mulgrim

 Level 15

JoinedMar 31, 2009Messages1,556

Resources: 4

#### Resources
Models 3Tutorials 1

it wasn't my intention to make a good unwrap, my intention was to teach this method. but if it matters at all, I can edit that.

EDIT: updated.


 Last edited: Jan 25, 2010

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I've updated the colors to fit the rules (they were a little too bright when it wasn't necessary, so I just colored them with that of the theme) =)


I've also updated it with a couple of grammar checks. Overall though, it is a neat technique, and since you've updated it, it is a fine addition. Good job, approved.

#### makai

#### makai

 Level 9

JoinedApr 30, 2010Messages323

Resources: 3

#### Resources
Models 2Icons 1

to download milkshape program need any payment??? coz there're show trial and standard..

how do i open the model??

 You must log in or register to reply here.


### Similar threads

 Milkshape Attachment Modeling


HappyTauren

Feb 15, 2008

3D Modeling Tutorials
2

Replies64Views37K

Nov 23, 2018
Amigoltu

 UVMapping in Milkshape 3D


ElectricSaiyan

Feb 14, 2008

3D Modeling Tutorials

Replies11Views18K

Jun 20, 2008
BlinkBoy

 Creating Doodads in Gmax (simple grass)


eubz

Aug 14, 2011

3D Modeling Tutorials

Replies8Views12K

Mar 7, 2012
stonneash

 GMax Exporting Tutorial


AirHead

Aug 22, 2008

3D Modeling Tutorials

Replies10Views14K

Jan 18, 2009
Pomegranate

P

 Animated Textures Tutorial


Amigurumi

Feb 9, 2010

3D Modeling Tutorials

Replies39Views49K

Dec 24, 2024
Greedy Martyrdom

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