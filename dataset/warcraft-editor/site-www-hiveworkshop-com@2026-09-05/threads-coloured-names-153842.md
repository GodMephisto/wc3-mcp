---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/coloured-names.153842/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Coloured Names
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/coloured-names.153842/>
#### sPy

#### sPy

 Level 22

JoinedApr 10, 2009Messages380

Resources: 77

#### Resources
Models 52Icons 23Tutorials 2

Coloured Name

Well, this is a simple tutorial, that will teach you how to make a player name when display in a text will become coloured. This tutorial is simple, but it is good enough to learn something from it.

Firstly, have you all ever seen a text like this?

Yes? Well, have you think how to make the name become coloured? Well, I'll teach how to make it in this tutorial. Without it, the text will be just like this:

Enough of talking now, let's get started. Before that, prepare a string-type array variable, I named it "PlayerName"

Triggers:

There is bad way and good way of doing this.

Good Way:

I'll show you the good way, it only need 2 triggers and it's MPI.

First, we need to do the settings first:

Players Option
Events
Map initialization

Conditions

Actions
Set PlayerName[1] = ((|CFFFF0303 + (Name of Player 1 (Red))) + |r)

Set PlayerName[2] = ((|CFF0042FF + (Name of Player 2 (Blue))) + |r)

Set PlayerName[3] = ((|CFF1BE6D8 + (Name of Player 3 (Teal))) + |r)

Set PlayerName[4] = ((|CFF540081 + (Name of Player 4 (Purple))) + |r)

Set PlayerName[5] = ((|CFFFFFF01 + (Name of Player 5 (Yellow))) + |r)

Set PlayerName[6] = ((|CFFFE8A0E + (Name of Player 6 (Orange))) + |r)

Set PlayerName[7] = ((|CFF20C000 + (Name of Player 7 (Green))) + |r)

Set PlayerName[8] = ((|CFFE55BB0 + (Name of Player 8 (Pink))) + |r)

Set PlayerName[9] = ((|CFF959697 + (Name of Player 9 (Gray))) + |r)

Set PlayerName[10] = ((|CFF7EBFF1 + (Name of Player 10 (Light Blue))) + |r)

Set PlayerName[11] = ((|CFF106246 + (Name of Player 11 (Dark Green))) + |r)

Set PlayerName[12] = ((|CFF4E2A04 + (Name of Player 12 (Brown))) + |r)

Set PlayerName[13] = (|cff272727 + (Neutral Hostile + |r))

You might thinking, what is this? Well, the settings I mean is make your name coloured and store it in string-type array variables! Without variables this totally can't be done. The code you see in the trigger is obviously the codes of basic player colours.

Since this tutorial is about coloured names, I will show you the codes too:

Red: |CFFFF0303 |R

Blue: |CFF0042FF |R

Teal: |CFF1BE6D8 |R

Purple: |CFF540081 |R

Yellow: |CFFFFFF01 |R

Orange: |CFFFE8A0E |R

Green: |CFF20C000 |R

Pink: |CFFE55BB0 |R

Grey: |CFF959697 |R

Light Blue: |CFF7EBFF1 |R

Dark Green: |CFF106246 |R

Brown: |CFF4E2A04 |R

Neutral: |CFF272727 |R

Okay now, the next trigger will occurred when a unit of hero-type dies:

Events
Unit - A unit Dies

Conditions
((Dying unit) is A Hero) Equal to True

Actions

Game - Display to (All players) for 5.00 seconds the text: (PlayerName[(Player number of (Owner of (Killing unit)))] + ( has just killed + (PlayerName[(Player number of (Owner of (Dying unit)))] + for |cffffcc00250|r gold!!!!)))
Will you believe me if it's done? Well, you are totally done! Simple? No? Let me explain it, as you can see, the event is "A unit dies" that means this trigger starts when a unit dies. You need to add condition "((Dying unit) is A Hero) Equal to True" to make only when a hero dies only this trigger will start. Now, let's get to the actions. As you can see, the actions is to display message to other players. Well, the variables we stored from starting is used in this trigger. "(PlayerName[(Player number of (Owner of (Killing unit)))] + ( has just killed + (PlayerName[(Player number of (Owner of (Dying unit)))]", you see there is two PlayerName right? Now you know why are we putting array? To make it MPI of course! I put it "player number of owner of dying/killing unit" because it can automatically detect the player number of killing unit or dying unit.

In the sentence, you can see there is "for |cffffcc00250|r Gold!". That one is optional, simply remove it by replacing "(PlayerName[(Player number of (Owner of (Dying unit)))] + for |cffffcc00250|r gold!!!!)))" into "(PlayerName[(Player number of (Owner of (Dying unit)))] + !!!"

NOTE: Variables won't automatically put spacebars for you, so you have to set it manually, like using concatenate strings to put spacebar after the variable. I won't show you the bad way because it might confuse your mind and is probably not MPI. Now you are done! Good luck in triggering!


 Last edited: Dec 27, 2009

#### Peter Canvis

#### Peter Canvis

 Level 5

JoinedOct 18, 2009Messages149

Simple yet nice.

#### PlasmaBear

#### PlasmaBear

 Level 3

JoinedDec 19, 2009Messages53

Yeah this can prove useful for those who doesn't know about this.

#### Peter Canvis

#### Peter Canvis

 Level 5

JoinedOct 18, 2009Messages149

Yet can you make the entire sentence coloured?

#### sPy

#### sPy

 Level 22

JoinedApr 10, 2009Messages380

Resources: 77

#### Resources
Models 52Icons 23Tutorials 2

From the rules I read, it says "Don't use too much colour".


Edit: I try to colour some important notes.

#### Peter Canvis

#### Peter Canvis

 Level 5

JoinedOct 18, 2009Messages149

Yeah, what I've tried to do in the past was colour the player's messages in the colour of their team, makes it usefull for melee teams.

#### Statharas

#### Statharas

 Level 23

JoinedJul 9, 2008Messages2,552

Resources: 20

#### Resources
Icons 1Maps 1StarCraft II Resources 5Tutorials 13

This guy barely even knows how this works.

|CFFFF0303 If i recall correctly, the first 6 letters/numbers are RGB colors. I don't remember what the 2 last ones do.

Also, |CFFFF0303 |r is like blabla

Lemme show you.

Consider that

|CFFFF0303

is your [(without this >.>)URL="http://blabla"] part.

Then, after those, XXX is like the ]blabla..[/ur..

And |r is to activate and end the coloring, like [/URL]

#### sPy

#### sPy

 Level 22

JoinedApr 10, 2009Messages380

Resources: 77

#### Resources
Models 52Icons 23Tutorials 2

Don't understand...What do you mean???

#### Zynxx

Z

#### Zynxx

Perfect! I used long form but you showed short form! Thanks! 5/5 +rep

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Stathisdjs said:

 This guy barely even knows how this works.


|CFFFF0303 If i recall correctly, the first 6 letters/numbers are RGB colors. I don't remember what the 2 last ones do.


Also, |CFFFF0303 |r is like blabla


Lemme show you.


Consider that


|CFFFF0303


is your [(without this >.>)URL="http://blabla"] part.


Then, after those, XXX is like the ]blabla..[/ur..


And |r is to activate and end the coloring, like [/URL]


Click to expand...


Actually, say you had |cff55AAFF:

|c or |C - This starts the color coding.

FF - This determines the alpha of the text. However, wc3 doesn't really support alpha for these texts so you can put any value there.

55 - This is the red value.

AA - This is the green value.

FF - This is the blue value.


Basically, the numbers go from 0-9, then A-F. 0 is the lowest, F is the highest. They use hexadecimals.


0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F Are all the values.


They are paired up with each other to represent different values. Here is a list of what the different combinations result in:

 Code:

Full Colour Code List
---------------------------
0 = 00
1 = 01
2 = 02
3 = 03
4 = 04
5 = 05
6 = 06
7 = 07
8 = 08
9 = 09
10 = 0A
11 = 0B
12 = 0C
13 = 0D
14 = 0E
15 = 0F
16 = 10
17 = 11
18 = 12
19 = 13
20 = 14
21 = 15
22 = 16
23 = 17
24 = 18
25 = 19
26 = 1A
27 = 1B
28 = 1C
29 = 1D
30 = 1E
31 = 1F
32 = 20
33 = 21
34 = 22
35 = 23
36 = 24
37 = 25
38 = 26
39 = 27
40 = 28
41 = 29
42 = 2A
43 = 2B
44 = 2C
45 = 2D
46 = 2E
47 = 2F
48 = 30
49 = 31
50 = 32
51 = 33
52 = 34
53 = 35
54 = 36
55 = 37
56 = 38
57 = 39
58 = 3A
59 = 3B
60 = 3C
61 = 3D
62 = 3E
63 = 3F
64 = 40
65 = 41
66 = 42
67 = 43
68 = 44
69 = 45
70 = 46
71 = 47
72 = 48
73 = 49
74 = 4A
75 = 4B
76 = 4C
77 = 4D
78 = 4E
79 = 4F
80 = 50
81 = 51
82 = 52
83 = 53
84 = 54
85 = 55
86 = 56
87 = 57
88 = 58
89 = 59
90 = 5A
91 = 5B
92 = 5C
93 = 5D
94 = 5E
95 = 5F
96 = 60
97 = 61
98 = 62
99 = 63
100 = 64
101 = 65
102 = 66
103 = 67
104 = 68
105 = 69
106 = 6A
107 = 6B
108 = 6C
109 = 6D
110 = 6E
111 = 6F
112 = 70
113 = 71
114 = 72
115 = 73
116 = 74
117 = 75
118 = 76
119 = 77
120 = 78
121 = 79
122 = 7A
123 = 7B
124 = 7C
125 = 7D
126 = 7E
127 = 7F
128 = 80
129 = 81
130 = 82
131 = 83
132 = 84
133 = 85
134 = 86
135 = 87
136 = 88
137 = 89
138 = 8A
139 = 8B
140 = 8C
141 = 8D
142 = 8E
143 = 8F
144 = 90
145 = 91
146 = 92
147 = 93
148 = 94
149 = 95
150 = 96
151 = 97
152 = 98
153 = 99
154 = 9A
155 = 9B
156 = 9C
157 = 9D
158 = 9E
159 = 9F
160 = A0
161 = A1
162 = A2
163 = A3
164 = A4
165 = A5
166 = A6
167 = A7
168 = A8
169 = A9
170 = AA
171 = AB
172 = AC
173 = AD
174 = AE
175 = AF
176 = B0
177 = B1
178 = B2
179 = B3
180 = B4
181 = B5
182 = B6
183 = B7
184 = B8
185 = B9
186 = BA
187 = BB
188 = BC
189 = BD
190 = BE
191 = BF
192 = C0
193 = C1
194 = C2
195 = C3
196 = C4
197 = C5
198 = C6
199 = C7
200 = C8
201 = C9
202 = CA
203 = CB
204 = CC
205 = CD
206 = CE
207 = CF
208 = D0
209 = D1
210 = D2
211 = D3
212 = D4
213 = D5
214 = D6
215 = D7
216 = D8
217 = D9
218 = DA
219 = DB
220 = DC
221 = DD
222 = DE
223 = DF
224 = E0
225 = E1
226 = E2
227 = E3
228 = E4
229 = E5
230 = E6
231 = E7
232 = E8
233 = E9
234 = EA
235 = EB
236 = EC
237 = ED
238 = EE
239 = EF
240 = F0
241 = F1
242 = F2
243 = F3
244 = F4
245 = F5
246 = F6
247 = F7
248 = F8
249 = F9
250 = FA
251 = FB
252 = FC
253 = FD
254 = FE
255 = FF
---------------------------


The one on the right is the hexadecimal pair. Basically, the values go from 0-255.

Anyway, this tutorial is fine. It is a tad short and would've been better as a complete color code tutorial, but there is nothing wrong with it in this state so I'll approve it.

#### HardOn

#### HardOn

 Level 4

JoinedMay 19, 2010Messages119

could really be useful.

#### adonisis

#### adonisis

 Level 3

JoinedMay 31, 2010Messages35

Helps alot

#### The_Key

#### The_Key

 Level 4

JoinedDec 6, 2009Messages75

I am going to use this one day

#### XAlexX

#### XAlexX

 Level 2

JoinedJun 16, 2010Messages22

Nice!


Usefull tut.(for noobs like me)

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

PurgeandFire111 said:

 Actually, say you had |cff55AAFF:

|c or |C - This starts the color coding.

FF - This determines the alpha of the text. However, wc3 doesn't really support alpha for these texts so you can put any value there.

55 - This is the red value.

AA - This is the green value.

FF - This is the blue value.

Basically, the numbers go from 0-9, then A-F. 0 is the lowest, F is the highest. They use hexadecimals.

0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F Are all the values.

They are paired up with each other to represent different values. Here is a list of what the different combinations result in:

 Code:

Full Colour Code List
---------------------------
0 = 00
1 = 01
2 = 02
3 = 03
4 = 04
5 = 05
6 = 06
7 = 07
8 = 08
9 = 09
10 = 0A
11 = 0B
12 = 0C
13 = 0D
14 = 0E
15 = 0F
16 = 10
17 = 11
18 = 12
19 = 13
20 = 14
21 = 15
22 = 16
23 = 17
24 = 18
25 = 19
26 = 1A
27 = 1B
28 = 1C
29 = 1D
30 = 1E
31 = 1F
32 = 20
33 = 21
34 = 22
35 = 23
36 = 24
37 = 25
38 = 26
39 = 27
40 = 28
41 = 29
42 = 2A
43 = 2B
44 = 2C
45 = 2D
46 = 2E
47 = 2F
48 = 30
49 = 31
50 = 32
51 = 33
52 = 34
53 = 35
54 = 36
55 = 37
56 = 38
57 = 39
58 = 3A
59 = 3B
60 = 3C
61 = 3D
62 = 3E
63 = 3F
64 = 40
65 = 41
66 = 42
67 = 43
68 = 44
69 = 45
70 = 46
71 = 47
72 = 48
73 = 49
74 = 4A
75 = 4B
76 = 4C
77 = 4D
78 = 4E
79 = 4F
80 = 50
81 = 51
82 = 52
83 = 53
84 = 54
85 = 55
86 = 56
87 = 57
88 = 58
89 = 59
90 = 5A
91 = 5B
92 = 5C
93 = 5D
94 = 5E
95 = 5F
96 = 60
97 = 61
98 = 62
99 = 63
100 = 64
101 = 65
102 = 66
103 = 67
104 = 68
105 = 69
106 = 6A
107 = 6B
108 = 6C
109 = 6D
110 = 6E
111 = 6F
112 = 70
113 = 71
114 = 72
115 = 73
116 = 74
117 = 75
118 = 76
119 = 77
120 = 78
121 = 79
122 = 7A
123 = 7B
124 = 7C
125 = 7D
126 = 7E
127 = 7F
128 = 80
129 = 81
130 = 82
131 = 83
132 = 84
133 = 85
134 = 86
135 = 87
136 = 88
137 = 89
138 = 8A
139 = 8B
140 = 8C
141 = 8D
142 = 8E
143 = 8F
144 = 90
145 = 91
146 = 92
147 = 93
148 = 94
149 = 95
150 = 96
151 = 97
152 = 98
153 = 99
154 = 9A
155 = 9B
156 = 9C
157 = 9D
158 = 9E
159 = 9F
160 = A0
161 = A1
162 = A2
163 = A3
164 = A4
165 = A5
166 = A6
167 = A7
168 = A8
169 = A9
170 = AA
171 = AB
172 = AC
173 = AD
174 = AE
175 = AF
176 = B0
177 = B1
178 = B2
179 = B3
180 = B4
181 = B5
182 = B6
183 = B7
184 = B8
185 = B9
186 = BA
187 = BB
188 = BC
189 = BD
190 = BE
191 = BF
192 = C0
193 = C1
194 = C2
195 = C3
196 = C4
197 = C5
198 = C6
199 = C7
200 = C8
201 = C9
202 = CA
203 = CB
204 = CC
205 = CD
206 = CE
207 = CF
208 = D0
209 = D1
210 = D2
211 = D3
212 = D4
213 = D5
214 = D6
215 = D7
216 = D8
217 = D9
218 = DA
219 = DB
220 = DC
221 = DD
222 = DE
223 = DF
224 = E0
225 = E1
226 = E2
227 = E3
228 = E4
229 = E5
230 = E6
231 = E7
232 = E8
233 = E9
234 = EA
235 = EB
236 = EC
237 = ED
238 = EE
239 = EF
240 = F0
241 = F1
242 = F2
243 = F3
244 = F4
245 = F5
246 = F6
247 = F7
248 = F8
249 = F9
250 = FA
251 = FB
252 = FC
253 = FD
254 = FE
255 = FF
---------------------------

The one on the right is the hexadecimal pair. Basically, the values go from 0-255.

Anyway, this tutorial is fine. It is a tad short and would've been better as a complete color code tutorial, but there is nothing wrong with it in this state so I'll approve it.


Click to expand...

Nice, but where did you get that? I've seen this somewhere before, I just can't recall where. It really helped me, mind giving me the link? Thanks.

Also, Stathisdjs...

Your example makes no sense. Where did you get an idea like that? No offense intended.

Hehe, excellent tutorial. I've used this 'system' in my map, and it works great. Thanks.

#### CoLd Bon3

#### CoLd Bon3

 Level 14

JoinedAug 8, 2010Messages1,022

Resources: 6

#### Resources
Maps 2Spells and Systems 4

Lmao! When you write this |CFFFE8A0E |R you must paste the thing that you want to be colored between |CFFFE8A0E and |R. Like for example - i want to color the ' I LOVE THE HIVE WORKSHOP' sentence in red - i do the following : |CFFFE8A0EI LOVE THE HIVE WORKSHOP|R. I hope i've helped

#### kimberker

#### kimberker

 Level 3

JoinedNov 8, 2009Messages46

Can u send me the map wif this trigger?

#### sPy

#### sPy

 Level 22

JoinedApr 10, 2009Messages380

Resources: 77

#### Resources
Models 52Icons 23Tutorials 2

Kimberker,

#### Attachments

Sample.w3x

 17 KB

 · Views: 246



#### Orcnet

#### Orcnet

 Level 30

JoinedJul 31, 2010Messages5,248

Resources: 5

#### Resources
Maps 4Tutorials 1

Similar in DotA

This is useful in Team Games, a good example! thanks for the tutorial!

#### jokerman10000

#### jokerman10000

 Level 9

JoinedNov 3, 2010Messages448

Resources: 2

#### Resources
Spells 1Tutorials 1

hey mabye you can add this page because mabye people want other coloured names


but nice tutorial

 You must log in or register to reply here.


### Similar threads

Locked

 Hero Kill Announcement Issue


Quehn

Jul 22, 2010

Triggers & Scripts

Replies9Views2K

Jul 22, 2010
watermelon_1234

Locked

 Player Color in Chat Message


1337Nemo

Aug 14, 2007

World Editor Help Zone

Replies2Views2K

Aug 14, 2007
1337Nemo

Locked

[Trigger]
 Player Names


cHeRoL

Sep 26, 2009

Triggers & Scripts

Replies7Views2K

Sep 26, 2009
cHeRoL

Locked

[Spell]
 Why is this trigger reacting to a unit dying when it's OFF at the time of death


Ugabunda

Sep 8, 2021

World Editor Help Zone

Replies5Views1K

Sep 11, 2021
Ugabunda

Locked

 Multiboard colouring weird


Quehn

May 26, 2010

Triggers & Scripts

Replies5Views1K

May 27, 2010
Berb

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Trigger (GUI) Editor Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top