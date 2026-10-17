---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/maya-and-3ds-max-5-integration-reviewed-ghan.86015/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Maya and 3ds Max 5 Integration Reviewed Ghan
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/maya-and-3ds-max-5-integration-reviewed-ghan.86015/>
#### anetherond

#### anetherond

 Level 9

JoinedJul 11, 2006Messages423

Resources: 2

#### Resources
Tutorials 2

 Maya and 3ds Max 5 integration.

 Chapters:

-Introduction.

-The FBX format.

-Installing the Plug-ins.

-Exporting the scene from Maya and importing in Max.

-Conclusions.

 Introduction.

Hello. Following my last tutorial, in which few questioned Maya's relevance to Warcraft 3 modding, I have made this one. In this tutorial we will learn how to export a scene from Maya 2008 (But the method matches for any version from Maya 5 and higher) and importing it back in 3ds Max 5. The benefits of this method are to use Maya's friendly interface and advanced tools but still be able to manage the final details required for Warcraft 3 in 3ds Max, as well as importing the final model as .mdx, without losing any information. As you will see, this method is very simple and quick to use.

 The FBX format.

In order to export our scene from Maya without losing any information we will be using a special format Autodesk has developed for such process; The FBX format.

The FBX supports all major 3D data elements, as well as 2D audio and video media elements. This means you can interchange all major NURBS and polygon surface types, keyframe and mocap animation, shapes and morph targets, materials and textures, lights, cameras, hierarchical information; and character animation data including IK, envelopes, and deformations.

Of course, the Fbx can be used for integration with other applications as well,

 Installing the Plug-ins.

In order to use the Fbx Format, we must install certain plug-ins in both Maya and Max.

We will start with Max.

I have attached for you the Plug-ins for max to this tutorial, but you can also get it and the Plug-ins for the other versions here:

http://usa.autodesk.com/adsk/servlet/mform?siteID=123112&id=10775855

Usually Max comes with FBX within it, but I suggest you replace it with this one.

After you download the .rar file and export it, you will get two new files:

fbxmax50exp.dle and fbxmax50exp.dle

To install the Plug-in, just Copy the .dli and .dle files to your 3ds max 5 plugin

directory. Just Make sure that there is only one FBX plugin .dli and .dle file in your 3ds max plugin directory and that they are identical to the current version you are using.

Now for Maya, download the identical Plug-in for the version you are using. It is a simple installation file.

Install the plug-in and Start Maya. Navigate to

Window>Settings/Preferences>Plug-in Manager.

Search for the Plug-In called "fbxmaya.mll" and activate the Loaded column to load it.

Activate Autoload column to have Maya load the FBX plugin automatically at start-up.

 Exporting the scene from Maya and importing in Max.

Open the scene in Maya and go to File->Export All and click the little box or, if you only want to export one mesh from the scene, select the mesh you want to export and go to File->Export Selection and click the little box.

An options window will pop out. In General Options, in file type slide down until you see FBX and select it, than click on "Export Selection" or "Export All" (Depends what kind of exportation you have selected) and Export the scene to the desired folder.

A window will pop out, just click yes to all and the file will be written. (If you want you can chagne the defult setting but it doesn't necessary)

Now, lunch 3ds Max and go to File>Import and import the FBX file we have exported from Maya.

The window "Import FBX File" will pop out. Select your Import type (Merge or Add to the current scene or Start a new scene) and click "Ok".

You have done! Now all the information you saved from Maya will be in your scene in Max!

Now you can export the file as .Mdx or edit it with Max's tools.

 Conclusions.

We have come to the end of our discussion about the integration between Maya and Max. This method will allow Maya users to create models and animations for Warcraft 3 and still be using Maya's powerful and advanced tools.

I hope you enjoyed this tutorial and if you have any questions, you know where to find me.

Anetherond.

#### Attachments

fbxplugin60_3dsmax50_windows.zip

 1.4 MB

 · Views: 393




 Last edited: Jun 7, 2008

#### Ghan_04

#### Ghan_04

 Level 22

JoinedFeb 26, 2008Messages893

Resources: 2

#### Resources
Tutorials 2

This tutorial is great to have in addition to your other one. Though, it is more of a How-To....


There is one thing that could be improved, and that is your grammar. There are a few (small) errors in the explanations that detract from it slightly.


> Now, lunch 3ds Max and go to File>Import


OM NOM NOM


Tasty.


Anyway. Just do a bit of polishing, and this will be ready for approval.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Could be useful, despite the small spelling mistakes.

#### Skylark

S

#### Skylark

This is great!

Since I want to help Pyritie and all my experience is in Maya

with basic to intermediate knowledge in Max ^_^

 You must log in or register to reply here.


### Similar threads

Locked

 tools needed? or just need a suggestion?


xintell

Apr 2, 2013

Warcraft Editing Tools

Replies0Views1K

Apr 2, 2013
xintell

Locked

 help me on my dragon model


xintell

Mar 23, 2013

Modeling & Animation

Replies3Views1K

Apr 2, 2013
Talavaj

Locked

 Using Maya


Echo

Apr 26, 2014

Modeling & Animation

Replies13Views3K

Apr 27, 2014
Tauer

Locked

 3ds or maya?


Idaero Ace

Dec 21, 2013

Modeling & Animation

Replies5Views2K

Dec 21, 2013
Idaero Ace

Locked

 Help With Animation PLZ


Chaoticalism

May 15, 2005

Modeling & Animation

Replies1Views1K

May 15, 2005
eXciTe

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