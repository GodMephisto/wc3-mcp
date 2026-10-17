---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/warcraft-iii-color-tags-and-linebreaks.31386/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Warcraft III Color Tags and Linebreaks
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/warcraft-iii-color-tags-and-linebreaks.31386/>
#### h3ih02

H

#### h3ih02

Introduction

This is a basic tutorial about creating color codes for WC3. It will explain what all the numbers and letters mean and how to make them. Next there will be a small list of some colors (The color of the text may not be the same as in the game because you cant make custom colors for the forums.) If you still need help or I did not do a good job, please post a reply. Creating 'rainbows' (gradients.) Wc3 color code generators made by me!

Explanations

You have probably seen WC3 Color Codes before, something like |c002A3B4CBuild|r. But what does it all mean!?!? WC3 Color Codes use RGB(Red Green and Blue) values to create the colors, but its not as easy as putting a couple of numbers together. The |c tells WC3 that you are starting a color, and the |r resets colors(making it the default color of what you are editing). |c is required at the begining of the color whenever doing WC3 Color Codes. The first 2 numbers/letters(00 in the example) is the transparency, i will not get into transparency in this tutorial. The second, third and fourth pairs of numbers/letters(2A, 3B, and 4C in the example) is the Red, Green, and Blue values in RGB. But RGB is set up like ###,###,### isn't it? Yes, but WC3 use's Hexadecimal to create its RGB, just like HTML!

Hexadecimal

Hexadecimal can be confusing for some. When counting in Decimal(Regular counting), the base numbers that make up every other number, are 0-9. Hexadecimal is a bit different. Hexadecimal base numbers are: 0 1 2 3 4 5 6 7 8 9 A B C D E F, 16 base numbers!?!?

Color CodesEdit!

To count in Hexadecimal up to 255(the max you can have in Red, Green, or Blue in RGB), you use 2 digits. To convert HEX-DEC you would take the value of the left number/letter in the number(0 = 0, 1 = 1... A = 10, B == 11, and so on) multiplied by 16, then add the value of the right digit. For example: 2A = 2(2) * 16 + 10(A) = 42, and BC = 11(B) * 16 + 12(C) = 188. To convert DEC-HEX you have to do the oposite. For the first digit of HEX you do # dividend 16(dividend just means division without decimals or remainders: 10 dividend 3 = 3), for the second digit, its # modulus 16(modulus just means remainder: 10 modulus 3 = 1). For example: 42 dividend 16 = 2(2), and 42 modulus 16 = 10(A), which gives you 2A. So you get your Red, Green, and Blue values, convert them to Hexadecimal, and put them in this format |c00RRGGBBText|r. Here is a list counting up to 30 in Decimal and Hexadecimal:

0 00

1 01

2 02

3 03

4 04

5 05

6 06

7 07

8 08

9 09

10 0A

11 0B

12 0C

13 0D

14 0E

15 0F

16 10

17 11

18 12

19 13

20 14

21 15

22 16

23 17

24 18

25 19

26 1A

27 1B

28 1C

29 1D

30 1E

Colors!Edit!

Various Colors:

|c00FFFF00Yellow|r

|c00FF7F00Orange|r

|c00FF9696Light Red(Pink)|r

|c00FF0000Red|r

|c00640000Dark Red|r

|c0096FF96Light Green(Lime)|r

|c0000FF00Green|r

|c00006400Dark Green|r

|c006969FFLight Blue(Sky Blue)|r

|c000000FFBlue|r

|c00000064Dark Blue(Navy)|r

|c00FFFFFFWhite|r

|c007d7d7dGrey|r

|c00000000Black|r

In Game Player Colors:

|c00FF0303Player 1|r

|c000042FFPlayer 2|r

|c001CE6B9Player 3|r

|c00540081Player 4|r

|c00FFFC01Player 5|r

|c00fEBA0EPlayer 6|r

|c0020C000Player 7|r

|c00E55BB0Player 8|r

|c00959697Player 9|r

|c007EBFF1Player 10|r

|c00106246Player 11|r

|c004E2A04Player 12|r

Player 13: Maroon|cff9c0000

Player 14: Navy|cff0000c3

Player 15: Turquoise|cff00ebff

Player 16: Violet |cffbd00ff

Player 17: Wheat|cffecce87

Player 18: Peach |cfff7a58b

Player 19: Mint |cffbfff81

Player 20: Lavender |cffdbb8eb

Player 21: Coal |cff4f5055

Player 22: Snow|cffecf0ff

Player 23: Emerald|cff00781e

Player 24: Peanut|cffa56f34

Player Neutral: Black |cff2e2d2e

Making A "Rainbow" Effect(Gradients)!

This is kind of hard to explain but i will try my best(there is colored text to help people see where a number is coming from) First thing your gunna need to do is get 2 RGB format colors, start and end, for example i'll use 255 0 0 (red) and 0 255 0  (green). Then you'll need the text, i'll use 'Cool!!'.

Now, count how many letters/numbers your text has(not counting spaces), 'Cool!!' has 6, then minus 1 from that, 6 - 1 = 5. Remember that number because we will use it later. For the next step, we need to find the difference of all the R's, G's, and B's. So take the R's from the first color code, and the R's from the second, and minus the smaller one from the bigger one. In my example it is 255, and 0, so i do 255 - 0 = 255. Do that with for the G's and B's(in my example, 255 - 0 = 255, and 0 - 0 = 0).

Now you take all the numbers that you got, and divide them by the number of letters/numbers your text has(if one of the numbers you got was 0, you do not do this because it stays the same through the whole gradient). In my example i would have 255 / 5 = 51, 255 / 5 = 51, and a 0 so i dont do anything(i will refer to these numbers later as the numbers that where divided).

Now that you have all those numbers, you can start making the color codes. Start by setting the first letter/number of your text to the first color, in my example i would get |c00FF0000C|r. For your second letter/number, if your R in the begining color is bigger than the R in the second color, you would minus the numbers that where divided for the R value, from the R value, in my example it would be 255 - 51 = 204. Then you do that for the G and B values too(since my green value is bigger in the second color i must add the numbers that where divided 0 + 51 = 51, and i have a 0 so the number stays the same). Those are your second letter/numbers colors, it would be |c00CC3300o|r.

If you repeat those steps with everytime adding or subtracting from what you got last time you will end up with a perfect gradient. This is the rest of my example gradient, 204 - 51 = 153, 51 + 51 = 102, and a 0(|c00996600o|r), 153 - 51 = 102, 102 + 51 = 153, and 0(|c00669900l|r), 102 - 51 = 51, 153 + 51 = 204, and 0(|c0033CC00!|r), 51 - 51 = 0, 204 + 51 = 255, and 0(|c0000FF00!|r).

The finished result is, |c00FF0000C|r|c00CC3300o|r(|c00996600o|r|c00669900l|r|c0033CC00!|r|c0000FF00!|r. This was hard for me to explain, i dont know if this will help anyone but i hope it does.

Wc3 Color Code Generators Made By Me!

This is an alias i wrote for IRC to return the wc3 color code of an rgb value. To install this you just open up your IRC, hit ALT+R, go into the file menu, go to new, then copy-n-paste the code bellow into the text box. Hit OK. Now to work it, you either type /wc3c RRR GGG BBB TEXT in a channel or type //echo -a $wc3c(RRR,GGG,BBB,TEXT). I will update it soon to have a gradient option.

 Code:

alias wc3c {
 if ($4) && ($1 isnum && $2 isnum && $3 isnum) && ($1 >= 0 && $1 <= 255 && $2 >= 0 && $2 <= 255 && $3 >= 0 && $3 <= 255) {
 $iif($isid == $true,/return,//echo -a) |c00 $+ $base($1,10,16,2) $+ $base($2,10,16,2) $+ $base($2,10,16,2) $+ $4 $+ |r
 }
 else {
 //echo $color(info) -a * /wc3c: The format is $wc3c(RRR,GGG,BBB,Text) or /wc3c RRR GGG BBB Text.
 }
}


 Last edited by a moderator: Nov 2, 2020

#### Shados

#### Shados

 Level 18

JoinedJan 24, 2006Messages1,938

Resources: 4

#### Resources
Maps 1Tutorials 3

Originally written by Ralle and Shados; pasted from another thread.

Introduction


Warcraft III has its own text formatting system. This system is not really very advanced but it is still worth considering and learning. This tutorial describes the formatting of color tags and how to use them. Each tag starts with the symbol '|' followed by one letter. For coloring this letter will be a 'c' and for linebreaks this will be an 'n'.

Linebreaks


In some places in World Editor it isn't necessary to actually write the break, just pressing "return" will make it automatically but some places it's impossible to make a linebreak with the "return" key so you can as an alternative write the linebreak. You write the linebreak with the tag '|n'.


For you who do not know what a linebreak is, here are 4 of them .

 this


is


a


linebreak


Click to expand...

The above example as written linebreaks would look like this:

 'this |n is |n a |n linebreak'


Click to expand...

Color Tags


Color codes are tags used to color text. The syntax of color codes is:

 |c RR GG BB XX sometext |r


Click to expand...

The above example might look very advanced so I will describe it. It starts with '|c' which indicates that a color code is to follow. Then RR which is the amount of red color in hexadecimal colors, then green and blue. The XX is the transparency of the text which does not work in text messages so just leave it as 'ff'. The tag ends with |r which closes the color code.


Here is an example of a colored string:

 |cffRRGGBBtext_here|r


Click to expand...


RR = The amount of red in Hexadecimal, 00-ff (000-255 in Decimal)


GG = The amount of green in Hexadecimal, 00-ff (000-255 in Decimal)


BB = The amount of blue in Hexadecimal, 00-ff (000-255 in Decimal)


Because I am aware that most people don't know hexadecimal color codes, here's a list of 140 hex color codes organized alphabetically to help get you started:

 Code:

Alice blue = F0F8FF
Antique white = FAEBD7
Aqua = 00FFFF
Aquamarine = 7FFFD4
Azure = F0FFFF
Beige = F5F5DC
Bisque = FFE4C4
Black = 000000
Blanched almond = FFEBCD
Blue = 0000FF
Blueviolet = 8A2BE2
Brown = A52A2A
Burlywood = DEB887
Cadet blue = 5F9EA0
Chartreuse = 7FFF00
Chocolate = D2691E
Coral = FF7F50
Cornflower blue = 6495ED
Cornsilk = FFF8DC
Crimson = DC143C
Cyan = 00FFFF
Dark blue = 00008B
Dark cyan = 008B8B
Dark goldenrod = B8860B
Dark gray = A9A9A9
Dark green = 006400
Dark khaki = BDB76B
Dark magenta = 8B008B
Dark olive green = 556B2F
Dark orange = FF8C00
Dark orchid = 9932CC
Dark red = 8B0000
Dark salmon = E9967A
Dark sea green = 8FBC8F
Dark slate blue = 483D8B
Dark slate gray = 2F4F4F
Dark turquoise = 00CED1
Dark violet = 9400D3
Deep pink = FF1493
Deep sky blue = 00BFFF
Dim gray = 696969
Dodger blue = 1E90FF
Firebrick = B22222
Floral white = FFFAF0
Forest green = 228B22
Fuchsia = FF00FF
Gainsboro = DCDCDC
Ghost white = F8F8FF
Gold = FFD700
Goldenrod = DAA520
Gray = 808080
Green = 008000
Greenyellow = ADFF2F
Honeydew = F0FFF0
Hot pink = FF69B4
Indian red = CD5C5C
Indigo = 4B0082
Ivory = FFFFF0
Khaki = F0E68C
Lavender = E6E6FA
Lavenderblush = FFF0F5
Lawn green = 7CFC00
Lemon chiffon = FFFACD
Light blue = ADD8E6
Light coral = F08080
Light cyan = E0FFFF
Light goldenrod yellow = FAFAD2
Light green = 90EE90
Light grey = D3D3D3
Light pink = FFB6C1
Light salmon = FFA07A
Light sea green = 20B2AA
Light sky blue = 87CEFA
Light slate gray = 778899
Light steel blue = B0C4DE
Light yellow = FFFFE0
Lime = 00FF00
Lime green = 32CD32
Linen = FAF0E6
Magenta = FF00FF
Maroon = 800000
Medium auqamarine = 66CDAA
Medium blue = 0000CD
Medium orchid = BA55D3
Medium purple = 9370D8
Medium sea green = 3CB371
Medium slate blue = 7B68EE
Medium spring green = 00FA9A
Medium turquoise = 48D1CC
Medium violetred = C71585
Midnight blue = 191970
Mint cream = F5FFFA
Misty rose = FFE4E1
Moccasin = FFE4B5
Navajo white = FFDEAD
Navy = 000080
Old lace = FDF5E6
Olive = 808000
Olive drab = 688E23
Orange = FFA500
Orangered = FF4500
Orchid = DA70D6
Pale goldenrod = EEE8AA
Pale green = 98FB98
Pale turquoise = AFEEEE
Pale violetred = D87093
Papayawhip = FFEFD5
Peachpuff = FFDAB9
Peru = CD853F
Pink = FFC0CB
Plum = DDA0DD
Powderblue = B0E0E6
Purple = 800080
Red = FF0000
Rosy brown = BC8F8F
Royal blue = 4169E1
Saddle brown = 8B4513
Salmon = FA8072
Sandy brown = F4A460
Sea green = 2E8B57
Seashell = FFF5EE
Sienna = A0522D
Silver = C0C0C0
Sky blue = 87CEEB
Slate blue = 6A5ACD
Slate gray = 708090
Snow = FFFAFA
Spring green = 00FF7F
Steel blue = 4682B4
Tan = D2B48C
Teal = 008080
Thistle = D8BFD8
Tomato = FF6347
Turquoise = 40E0D0
Violet = EE82EE
Wheat = F5DEB3
White = FFFFFF
Whitesmoke = F5F5F5
Yellow = FFFF00
Yellow Green = 9ACD32


That list should give you a good start. Additional color codes may be found here.

The following tools can save everyone a lot of work. They create formatted color tags that can be easily pasted into a map or trigger.
Chaos Spectrum 1.1

Color Template 1.0


Ralle


 Last edited by a moderator: Apr 20, 2007

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Originally written by Ralle and Shados; pasted from another thread.

Introduction


Warcraft III has its own text formatting system. This system is not really very advanced but it is still worth considering and learning. This tutorial describes the formatting of color tags and how to use them. Each tag starts with the symbol '|' followed by one letter. For coloring this letter will be a 'c' and for linebreaks this will be an 'n'.

Linebreaks


In some places in World Editor it isn't necessary to actually write the break, just pressing "return" will make it automatically but some places it's impossible to make a linebreak with the "return" key so you can as an alternative write the linebreak. You write the linebreak with the tag '|n'.


For you who do not know what a linebreak is, here are 4 of them .

 this


is


a


linebreak


Click to expand...

The above example as written linebreaks would look like this:

 'this |n is |n a |n linebreak'


Click to expand...

Color Tags


Color codes are tags used to color text. The syntax of color codes is:

 |c RR GG BB XX sometext |r


Click to expand...

The above example might look very advanced so I will describe it. It starts with '|c' which indicates that a color code is to follow. Then RR which is the amount of red color in hexadecimal colors, then green and blue. The XX is the transparency of the text which does not work in text messages so just leave it as 'ff'. The tag ends with |r which closes the color code.


Here is an example of a colored string:

 |cffRRGGBBtext_here|r


Click to expand...


RR = The amount of red in Hexadecimal, 00-ff (000-255 in Decimal)


GG = The amount of green in Hexadecimal, 00-ff (000-255 in Decimal)


BB = The amount of blue in Hexadecimal, 00-ff (000-255 in Decimal)


Because I am aware that most people don't know hexadecimal color codes, here's a list of 140 hex color codes organized alphabetically to help get you started:

 Code:

Alice blue = F0F8FF
Antique white = FAEBD7
Aqua = 00FFFF
Aquamarine = 7FFFD4
Azure = F0FFFF
Beige = F5F5DC
Bisque = FFE4C4
Black = 000000
Blanched almond = FFEBCD
Blue = 0000FF
Blueviolet = 8A2BE2
Brown = A52A2A
Burlywood = DEB887
Cadet blue = 5F9EA0
Chartreuse = 7FFF00
Chocolate = D2691E
Coral = FF7F50
Cornflower blue = 6495ED
Cornsilk = FFF8DC
Crimson = DC143C
Cyan = 00FFFF
Dark blue = 00008B
Dark cyan = 008B8B
Dark goldenrod = B8860B
Dark gray = A9A9A9
Dark green = 006400
Dark khaki = BDB76B
Dark magenta = 8B008B
Dark olive green = 556B2F
Dark orange = FF8C00
Dark orchid = 9932CC
Dark red = 8B0000
Dark salmon = E9967A
Dark sea green = 8FBC8F
Dark slate blue = 483D8B
Dark slate gray = 2F4F4F
Dark turquoise = 00CED1
Dark violet = 9400D3
Deep pink = FF1493
Deep sky blue = 00BFFF
Dim gray = 696969
Dodger blue = 1E90FF
Firebrick = B22222
Floral white = FFFAF0
Forest green = 228B22
Fuchsia = FF00FF
Gainsboro = DCDCDC
Ghost white = F8F8FF
Gold = FFD700
Goldenrod = DAA520
Gray = 808080
Green = 008000
Greenyellow = ADFF2F
Honeydew = F0FFF0
Hot pink = FF69B4
Indian red = CD5C5C
Indigo = 4B0082
Ivory = FFFFF0
Khaki = F0E68C
Lavender = E6E6FA
Lavenderblush = FFF0F5
Lawn green = 7CFC00
Lemon chiffon = FFFACD
Light blue = ADD8E6
Light coral = F08080
Light cyan = E0FFFF
Light goldenrod yellow = FAFAD2
Light green = 90EE90
Light grey = D3D3D3
Light pink = FFB6C1
Light salmon = FFA07A
Light sea green = 20B2AA
Light sky blue = 87CEFA
Light slate gray = 778899
Light steel blue = B0C4DE
Light yellow = FFFFE0
Lime = 00FF00
Lime green = 32CD32
Linen = FAF0E6
Magenta = FF00FF
Maroon = 800000
Medium auqamarine = 66CDAA
Medium blue = 0000CD
Medium orchid = BA55D3
Medium purple = 9370D8
Medium sea green = 3CB371
Medium slate blue = 7B68EE
Medium spring green = 00FA9A
Medium turquoise = 48D1CC
Medium violetred = C71585
Midnight blue = 191970
Mint cream = F5FFFA
Misty rose = FFE4E1
Moccasin = FFE4B5
Navajo white = FFDEAD
Navy = 000080
Old lace = FDF5E6
Olive = 808000
Olive drab = 688E23
Orange = FFA500
Orangered = FF4500
Orchid = DA70D6
Pale goldenrod = EEE8AA
Pale green = 98FB98
Pale turquoise = AFEEEE
Pale violetred = D87093
Papayawhip = FFEFD5
Peachpuff = FFDAB9
Peru = CD853F
Pink = FFC0CB
Plum = DDA0DD
Powderblue = B0E0E6
Purple = 800080
Red = FF0000
Rosy brown = BC8F8F
Royal blue = 4169E1
Saddle brown = 8B4513
Salmon = FA8072
Sandy brown = F4A460
Sea green = 2E8B57
Seashell = FFF5EE
Sienna = A0522D
Silver = C0C0C0
Sky blue = 87CEEB
Slate blue = 6A5ACD
Slate gray = 708090
Snow = FFFAFA
Spring green = 00FF7F
Steel blue = 4682B4
Tan = D2B48C
Teal = 008080
Thistle = D8BFD8
Tomato = FF6347
Turquoise = 40E0D0
Violet = EE82EE
Wheat = F5DEB3
White = FFFFFF
Whitesmoke = F5F5F5
Yellow = FFFF00
Yellow Green = 9ACD32


That list should give you a good start. Additional color codes may be found here.

The following tools can save everyone a lot of work. They create formatted color tags that can be easily pasted into a map or trigger.
Chaos Spectrum 1.1

Color Template 1.0


Ralle


 Last edited: Apr 9, 2007

#### Earth-Fury

#### Earth-Fury

 Level 11

JoinedFeb 18, 2004Messages394

Resources: 1

#### Resources
Tutorials 1

Player Colours:

JASS Player ID: 0

GUI Player ID: 1

Red: 255

Green: 2

Blue: 2

Hex Code: FF0202

Preview:#####

JASS Player ID: 1

GUI Player ID: 2

Red: 0

Green: 65

Blue: 255

Hex Code: 0041FF

Preview:#####

JASS Player ID: 2

GUI Player ID: 3

Red: 27

Green: 229

Blue: 184

Hex Code: 1BE5B8

Preview:#####

JASS Player ID: 3

GUI Player ID: 4

Red: 83

Green: 0

Blue: 128

Hex Code: 530080

Preview:#####

JASS Player ID: 4

GUI Player ID: 5

Red: 255

Green: 255

Blue: 0

Hex Code: FFFF00

Preview:#####

JASS Player ID: 5

GUI Player ID: 6

Red: 254

Green: 137

Blue: 13

Hex Code: FE890D

Preview:#####

JASS Player ID: 6

GUI Player ID: 7

Red: 31

Green: 191

Blue: 0

Hex Code: 1FBF00

Preview:#####

JASS Player ID: 7

GUI Player ID: 8

Red: 228

Green: 90

Blue: 170

Hex Code: E45AAA

Preview:#####

JASS Player ID: 8

GUI Player ID: 9

Red: 148

Green: 149

Blue: 150

Hex Code: 949596

Preview:#####

JASS Player ID: 9

GUI Player ID: 10

Red: 125

Green: 190

Blue: 241

Hex Code: 7DBEF1

Preview:#####

JASS Player ID: 10

GUI Player ID: 11

Red: 15

Green: 97

Blue: 69

Hex Code: 0F6145

Preview:#####

JASS Player ID: 11

GUI Player ID: 12

Red: 77

Green: 41

Blue: 3

Hex Code: 4D2903

Preview:#####


 Last edited by a moderator: Feb 7, 2008

#### Arthassadar

#### Arthassadar

 Level 3

JoinedJul 19, 2014Messages38

Damn, I love this page, you really make me happy guys!

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

you understand that this is 7 years old thread right?  And Im not helping the cause either

#### Acutesharpness

#### Acutesharpness

 Level 15

JoinedAug 14, 2007Messages936

Resources: 9

#### Resources
Maps 8Spells 1

Earth-Fury said:

Player Colours:

JASS Player ID: 0

GUI Player ID: 1

Red: 255

Green: 2

Blue: 2

Hex Code: FF0202

Preview:#####

JASS Player ID: 1

GUI Player ID: 2

Red: 0

Green: 65

Blue: 255

Hex Code: 0041FF

Preview:#####

JASS Player ID: 2

GUI Player ID: 3

Red: 27

Green: 229

Blue: 184

Hex Code: 1BE5B8

Preview:#####

JASS Player ID: 3

GUI Player ID: 4

Red: 83

Green: 0

Blue: 128

Hex Code: 530080

Preview:#####

JASS Player ID: 4

GUI Player ID: 5

Red: 255

Green: 255

Blue: 0

Hex Code: FFFF00

Preview:#####

JASS Player ID: 5

GUI Player ID: 6

Red: 254

Green: 137

Blue: 13

Hex Code: FE890D

Preview:#####

JASS Player ID: 6

GUI Player ID: 7

Red: 31

Green: 191

Blue: 0

Hex Code: 1FBF00

Preview:#####

JASS Player ID: 7

GUI Player ID: 8

Red: 228

Green: 90

Blue: 170

Hex Code: E45AAA

Preview:#####

JASS Player ID: 8

GUI Player ID: 9

Red: 148

Green: 149

Blue: 150

Hex Code: 949596

Preview:#####

JASS Player ID: 9

GUI Player ID: 10

Red: 125

Green: 190

Blue: 241

Hex Code: 7DBEF1

Preview:#####

JASS Player ID: 10

GUI Player ID: 11

Red: 15

Green: 97

Blue: 69

Hex Code: 0F6145

Preview:#####

JASS Player ID: 11

GUI Player ID: 12

Red: 77

Green: 41

Blue: 3

Hex Code: 4D2903

Preview:#####

Click to expand...

Dude stop with the gates.. it's no longer funny :[

#### HandsOfficial

#### HandsOfficial

 Level 9

JoinedSep 5, 2015Messages369

Resources: 1

#### Resources
Maps 1

too complicated. i just copy and paste the examples and use them on my maps lmfeaooeofeoeoeo.

#### Sapprine

#### Sapprine

 Level 28

JoinedMay 24, 2016Messages803

Resources: 73

#### Resources
Models 1Icons 15Maps 3Templates 1Reforged HD Models 15Ported Icons 31Reforged HD Icons 7

So isefull! I used these like 20 times! I even got the URL of this page saved into my favourites! (-sorry for bad english, I come from Holland)

#### Acutesharpness

#### Acutesharpness

 Level 15

JoinedAug 14, 2007Messages936

Resources: 9

#### Resources
Maps 8Spells 1

Let me explain myself for people outside the gates. A normal color code like |cffffcc00|r tag, has a total of 9 letters therefore, for a better tutorial, the poster would have to make it easier to understand by including the 3 basic letters. This example being, cff as the first 3 letters to complete the set. The gates meaning that the author did not care for the readers and made it so that users had to figure it out themselves or find other sources. Now that is something we see around these days people.

#### Mistablockout

#### Mistablockout

 Level 4

JoinedNov 3, 2015Messages47

One of my favorite pages on this site, needs to be updated with new colors!

#### RealistKilla

#### RealistKilla

 Level 5

JoinedAug 9, 2013Messages63

Mistablockout said:

 One of my favorite pages on this site, needs to be updated with new colors!


Click to expand...

 Here you go.

1: 255,3,3 /#FF0303 2: 0,66,255 /#0042FF 3: 25,232,185 /#19E8B9 4: 84,0,129 /#540081 5: 255.252.1 /#FFFC01 6: 254.138.14 /#FE8A0E 7: 32.192.0 /#20C000 8: 230.90.176 /#E65AB0 9: 149 150 151 /#959697 10: 126 191 241 /#7EBFF1 11: 16 98 70 /#106246 12: 78 42 4 /#4E2A04 13: 155 0 0 /#9B0000 14: 0 0 195 /#0000C3 15: 0 234 255 /#00EAFF 16: 190 0 254 /#BE00FE 17: 235 205 135 /#EBCD87 18: 248 164 139 /#F8A48B 19: 191 255 128 /#BFFF80 20: 220 185 235 /#DCB9EB 21: 40 40 40 /#282828 22: 235 240 255 /#EBF0FF 23: 0 120 30 /#00781E 24: 164 111 51 /#A46F33

So for example, the new light purple color would be - |cFFBE00FE


 Last edited: Jun 9, 2018

#### Sapprine

#### Sapprine

 Level 28

JoinedMay 24, 2016Messages803

Resources: 73

#### Resources
Models 1Icons 15Maps 3Templates 1Reforged HD Models 15Ported Icons 31Reforged HD Icons 7

damn.. been awhile since i had read this... I now actually master color codes, all thanks to practice and this very thread.

#### Mistablockout

#### Mistablockout

 Level 4

JoinedNov 3, 2015Messages47

To be honest, this should be a stickied thread and all the info in the replies should be moved to the OP.

Also thank you to @RealistKilla for that, although it could probably be formatted like the OP for easier viewing.

#### maxxxus

#### maxxxus

 Level 5

JoinedAug 25, 2015Messages144

thanks for the info but... where in WE would i write all of these codes??

#### tulee

#### tulee

 Level 31

JoinedJul 26, 2008Messages1,575

Resources: 26

#### Resources
Maps 26

15 years later, the tooltip previewer feature that has been implemented in 1.31 means that map makers no longer need to spend time thinking about these kinds of things (colours, line-breakers, etc.)

#### adde

A

#### adde

i need help to create rainbow color name with 2 colors. i saw i guy with name XaXa and first Xa was red other was green or something like that. can anyone do this?

#### RealistKilla

#### RealistKilla

 Level 5

JoinedAug 9, 2013Messages63

adde said:

 i need help to create rainbow color name with 2 colors. i saw i guy with name XaXa and first Xa was red other was green or something like that. can anyone do this?


Click to expand...

This should just be the color code before Xa and the second color code before the next Xa.

#### Harold Grey

#### Harold Grey

 Level 7

JoinedSep 4, 2016Messages116

Resources: 1

#### Resources
Maps 1

I am curious what colors would be recommended if, for example, one wanted to put resource costs in an ability description?

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

There's a mismatch in examples. h3ih02's original colors codes have alpha value of 0: "|c00FF0000Red|r". But the Reforged color code update has set it to 255: "Player 13: Maroon|cff9c0000".

I think it should be FF in both cases, because 00 is ought to be fully transparent.

PS: Display text to player always shows fully opaque text, regardless of alpha value set.


 Last edited: Sep 26, 2022

#### yki01

#### yki01

 Level 4

JoinedJul 9, 2012Messages55

May I ask for the official in-game color tags for neutrals (hostile/passive/victim/extra)? I think they're kinda goldish and pinkish

#### Beeša

#### Beeša

 Level 4

JoinedJul 5, 2022Messages17

Resources: 1

#### Resources
Maps 1

Hello, I would like to contribute to this thread with colors Blizzard uses in their campaigns (for quests, hints, new units etc.) as I was unable to find them anywhere and always had to open maps in the map editor and try to find them there. Let's say I'm probably doing this more for myself so I don't have to search for these codes in the campaigns ever again.

|cffffcc00 is used for MAIN QUEST/OPTIONAL QUEST/SECRET FOUND messages

|cff32CD32 is used for HINT messages

|cff87CEEB is used for NEW UNIT ACQUIRED/NEW BUILDING AVAILABLE messages

|cffff8c00 is used for NOTICE messages

|cffff0000 is used for WARNING messages

|cff808080 is used for completed quest requirements


 Last edited: Jun 26, 2023

#### Sapprine

#### Sapprine

 Level 28

JoinedMay 24, 2016Messages803

Resources: 73

#### Resources
Models 1Icons 15Maps 3Templates 1Reforged HD Models 15Ported Icons 31Reforged HD Icons 7

Beeša said:

 Hello, I would like to contribute to this thread with colors Blizzard uses in their campaigns (for quests, hints, new units etc.) as I was unable to find them anywhere and always had to open maps in the map editor and try to find them there. Let's say I'm probably doing this more for myself so I don't have to search for these codes in the campaigns ever again.

|cffffcc00 is used for MAIN QUEST/OPTIONAL QUEST/SECRET FOUND messages

|cff32CD32 is used for HINT messages

|cff87CEEB is used for NEW UNIT ACQUIRED/NEW BUILDING AVAILABLE messages

|cffff8c00 is used for NOTICE messages

|cffff0000 is used for WARNING messages


Click to expand...

|cffffdead is used for artifact descriptions in the rexxar campaign

 You must log in or register to reply here.


### Similar threads

T

Locked

 How To Make Your Text Colored!!!


TensaZangetsu

Nov 4, 2007

World Editor Help Zone

Replies14Views2K

Nov 6, 2007
brad.dude03

[Lua]
 Color


HerlySQR

Nov 27, 2022

Old Submissions

Replies2Views2K

Jul 21, 2023
HerlySQR

 Color Coding Text


UniEditXtreme

Nov 19, 2008

Tutorial Archive

Replies1Views2K

Nov 19, 2008
Ezikielth

 Understand the Numbers of WCIII


Gamecrafter_DK

Nov 1, 2009

Misc/General Mapping Tutorials

Replies33Views12K

Jun 17, 2010
Gamecrafter_DK

 Hexidecimal Color Codes Explained


JonnyLocke

Mar 23, 2008

Tutorial Archive

Replies4Views4K

Mar 24, 2008
JonnyLocke

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