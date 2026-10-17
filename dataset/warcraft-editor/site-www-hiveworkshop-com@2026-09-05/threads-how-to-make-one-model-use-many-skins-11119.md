---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-make-one-model-use-many-skins.11119/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Make One Model Use Many Skins
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-make-one-model-use-many-skins.11119/>
#### oz02

#### oz02

 Level 13

JoinedJan 9, 2004Messages1,037

Resources: 6

#### Resources
Tools 1Tutorials 5

Using More Than One Skin For A Model


For Windows.


Pictures taken in Windows XP with Paint Shop Pro 8.

Tools Needed


Warcraft III - Reign of Chaos or The Frozen Throne


Warcraft 3 Viewer - Get It Here


War3File Converter - Get It Here

Introduction


Have you ever downloaded a skin and wanted to use the original skin, and you've been unable to? This tutorial explains how to download multiple skins that wrap onto the same model, and still be able to import every one into your map and use them all at the same time. This process is short and easy, and in a very short time you will be importing many different skins of the same unit into your maps.

The Process


1. Exporting the MDX - You begin by opening Warcraft 3 Viewer. You should see something like this:


Next look in the Treeview window. On the top click the MDX radio button.


Then, find the model that your skin is based on.

TIP: Usually the model has the same file path as the skin.
Next click Current File->Extract MDX... Choose a folder that you will remember, and click Save. Finally, close Warcraft 3 Viewer.


2. Converting the MDX - Now open War3File Converter.


Click Load File and select the model you just extracted. Now click Convert to MDL. Now close War3File Converter.


3. Changing the File Path - Open the MDL file with Notepad. Now press Ctrl+F and look for Textures.


Now you should see this:


There are two or more file paths: *your model's file path*.blp (usually begun with Textures\), Textures\gutz.blp, and sometimes a few others. Focus only on your skin's file path. Now change the file path to whatever you want it to be. So, for example, if your skin is of a business man, you would probably change the file path to BusinessMan.blp (you can get rid of the Tetures part).


Now click File->Save and close Notepad.


4. Converting the MDL - Open War3File Converter once again, and load the MDL you just edited. Now click Save as MDX.

TIP: After converting the MDL back to MDX, you should delete all other files you were working with earlier (the original MDX and the MDL file) and rename the new MDX as the file path you gave it (so if you changed the path to BusinessMan.blp, you would name the MDX BusinessMan.mdx).


5. Importing the Files - Now open the Warcraft III World Editor and import the mdx and the skin, changing the paths to the one you specified (I will soon write a tutorial on importing skins if you have trouble importing), and make a unit for that skin based on the model (you can change the model in the field Art - Model).

End Note


Congratulations! Now you will be able to import as many skins as you want based on one model. Unfortuantly, every model takes up space, so every time you import another model your map size will get larger.


 Last edited by a moderator: Sep 19, 2007

#### Mecheon

#### Mecheon

 Level 25

JoinedMar 31, 2004Messages4,465

Resources: 13

#### Resources
Models 12Ported Models 1

Gawd, I'm getting sick of every second topic here being about this question, so answer time


The answer? Simple


TOOLS REQUIRED


1 x WC3 Viewer


1 x WC3 File Converter


1 x Warcraft 3, Frozen Throne


1 x Word Editing program: I use Notepad, but you can use Word Pad if you want


INSTRUCTIONS


1. Make a folder somewhere to keep all of your WC3 tools, I suggest putting a folder called "Tools" in your WC3 folder, but your choice. Then download the tools to this folder.


Why can't you put them in the WC3 folder? Because then you'll screw up WC3 and the tool


2. Open WC3 Viewer, click "File, Open MPQ" and select either "wc3.mpq" or "wc3x.mpq". wc3.mpq has the original units, and wc3x.mpq has the expansion ones. Neither of them have the Firelord, Tinker or Alchemist, seeing as how those three are in wc3patch.mpq, which you can't open due to its lack of a listfile


3. Find the model you want, and extract it somewhere


4. Use the File Converter to convert the mdx file you just extracted into a mdl file


5. Open the mdl file in notepad/wordpad


6. Find the part where it says "Textures", find the skin that this model uses and change the path to something else for the skin. REMEMBER THIS PATH


7. Save, then convert back to mdx


8. Get open WC3 World Editor, start a new map and then import the model and the skin. Put the skin in the path you gave it earlier, don't need to do anything for the model


9. If all went well, save and restart, then place the original unit and the one with the changed skin. They should use a different skin.


There, 9 steps to solving this ever-asked about question

#### YoshiRyu

#### YoshiRyu

 Level 10

JoinedJun 26, 2007Messages659

Resources: 4

#### Resources
Models 1Icons 1Maps 2

How to use different skins associated with the same model

How to use different skins associated with the same model


If you want to use both a original model and its reskined version, or more,


you can't simply import the skin, you've to export the model from the warcraft official datas files and apply the skin to it!

sample : in my map i need both Jaina original model and a Jaina-based orcish model


the second one use the Jaina model with this skin : OrcSorcerer.blp

 &

Required tools to proceed : War3 Model Editor and Mdx Pather


Fist create the directory where you want to store the model like "OrcSorcerer"


create a Textures directory in the previous on and copy the skin into it.

Step 1 : Export the model as a mdx file


Then launch War3 Model Editor, clic on "Windows" and open "MPQ Browser"


in MPQ Browser, clic on "File" and load the correct .mpq

some explanations about the mpq:


in the War3.mpq are stored the ressources of the reign of chaos version (like the old priest)


in the War3x.mpq are stored the ressources of the frozen throne version (like the crypt lord)


in the War3Patch.mpq are stored the ressources added in patch (like the tinker)


there is no model in War3xLocal.mpq

in the sample, i need Jaina model, so i load the War3.mpq


wait during the loading, look for your model in the tree and double clic on it


(for the sample, i need to open "Units/Human/Jaina/Jaina.mdx")


Then go back to the War3 Model Editor main windows (You should see the model), save the model in the directory you've previously created with the filename you want ("OrcSorcerer.mdx" in the "OrcSorcerer" directory in my sample) and you can close War3 Model Editor.

Step 2 : Apply the texture to the model


Afterwards, launch Mdx Pather and open the mdx you've just saved


You'll see all the texture's default paths, you must correct the main texture's path.

be carefull to this two details :


- sometime, there is many texture, you've to find the main one


- sometime, the main texture is use several time, replace all lines

in the sample, i replace units\Human\Jaina\Jaina.blp with Textures\OrcSorcerer.blp


Save the change and close Mdx Pather


You've finished, you can lauch War3 Model Editor again and open your .mdx to see the result.


Now, you're able to import the created model and use it with the original model in the same map.

Jaina meeting the OrcSorcerer :


PS: You can do all this with the portrait mdx too if you need it.


 Last edited by a moderator: Sep 19, 2007

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Horn said:

 Regardless, your tutorial achieves exactly the same as this one:

http://hiveworkshop.com/forums/showthread.php?t=11119

Click to expand...

How to use different skins associated with the same model Approved.


~ Threads merged.

Status
 Not open for further replies.


### Similar threads

 Creating Ghosts


Daelin

Jul 14, 2005

3D Modeling Tutorials

Replies10Views27K

Aug 14, 2023
DREADMAN

Locked

 How to Import Custom Models


PurgeandFire

Apr 6, 2007

3D Modeling Tutorials

Replies0Views114K

Apr 6, 2007
PurgeandFire

 How to create new unit using existing textures


GaLaxY_256

Jun 1, 2014

3D Modeling Tutorials

Replies16Views9K

Jul 10, 2018
GaLaxY_256

[2D Art]
 How to use skin with same type of model


rysnt11

Dec 16, 2010

Tutorial Archive

Replies1Views7K

Dec 23, 2010
PurgeandFire

 How to add/remove hero glow with modelling


Alok

Feb 25, 2017

3D Modeling Tutorials

Replies7Views11K

Feb 2, 2021
Gumberbunbder

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