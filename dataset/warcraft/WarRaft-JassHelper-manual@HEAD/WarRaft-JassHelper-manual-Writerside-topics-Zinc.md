---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Zinc.md"
ref: HEAD
---

# Zinc

I've kept thinking that vJass' must remain as close to JASS as possible. This is a good thing as it keeps some
consistency, but it might be bad as it forces it to JASS ideas that were not really good. vJass as the single language
alternative was not going to work forever, so it has always been a plan to somehow let different languages in. Another
reason for making a new language was the fun that is designing a language's syntax...

I only had the vague idea of making another language, until one day, I started trying to come up with it, and started to
write random text in a [wc3c.net](http://www.wc3c.net) pastebin thread, and asking some fellows for opinion. This event
was inspired by some other things that were going on at that time that convinced me of this necessity. Basically, a lot
of discussions about JASS' verbosity came to fruition, which caused me to really feel lame while re-coding
my [Hydra spell](http://www.wc3c.net/showthread.php?t=107709), suddenly I felt like typing more than I should. It was
not just the verbosity, it was stuff like not making all members of a library implicitely private, or having to declare
library private members before they are ever used. At that time there were alternatives to vJass that were very
powerful, perhaps they were too powerful, and were full of things I was not looking for while not really taking real
care of the problems I noticed besides of adding new problems. All was pointing towards it, it was the time to do it.

Zinc serves a purpose different to Jass, to be honest, it is main purpose is to test the grounds for more multi-language
support, and to have fun implementing yet another language. But it should also be a good alternative to vJass in its
own.

Zinc is actually an acronym that stands for **Z**INC **i**s **n**ot **C**. However, to continue with JASS' tradition,
you may call it Zinc or ZINC indistinctively.

Why call it Zinc is not C? Well, because while the syntax tries to follow that classic C-style paradigm, it does not
follow it 100%, there are parts of that style of syntax I didn't want to use. It also avoids giving the false impression
that learning Zinc would somehow help you learn C, or that people that know C-style languages will have it easier with
Zinc, and such non-sense...

Let me list the values, the rules behind its syntax and workings.

## C-like syntax

So, the syntax is mostly inspired by C, why? Well, that's just because I like this syntax the most. In many ways, it is
not really _that great_. It is not a panacea that will make your code more readable or anything like that, in fact,
it has quite the potential to make your code incredibly hard to follow. Yet, I like it, it is for simple things
like {} and not having to use them for a single statement. (Using `{}` gives you a certain advantage when your editor
has bracket highlighting)

## Less verbosity

Many guys do not like verbosity, yet it is really one of the things that makes Jass (and vJass for that matter), much
easier to follow. It has pros and cons, Zinc is meant as a complement to vJass. vJass fails at being quick to write, yet
it is very readable for the most part. Zinc will sacrifice much of this readability to allow some code that is faster to
write.

It is not just the large amount of keywords you have to write, it is also the amount of keywords you can write. Zinc
aims to reduce both. Although you will still see things like **library** you will not see anything like **array**...
Zinc tries to use operators where possible or recycle keywords.

This is again, no panacea, it is a trade off. Code will become more symbollic or context-requiring, and will need some
extra attention and knowledge to read.

## Readability

That Zinc does not likes verbosity should not be an excuse to make the language downright unreadable, this is one of the
reasons Zinc does not blindly follow C-like syntax. It is also the reason you won&apos;t see silly abbreviations to
merely cut a token&apos;s length.

## Segregation

This is a very important thing, if we allowed vJass and Zinc code to get thrown together anywhere without distinction,
a) Everything would get very hard to read, b) There would be no consistency and most importantly: c) it would be very
hard to parse...

Zinc is a different language than Jass, and thus it should be kept in different files and &apos;triggers&apos; than it.
vJass syntax should give errors when used inside Zinc areas, and viceversa.

## Structured programming

What really made me start this all over, was seeing the loop..exitwhen..endloop construct while coding Hydra. Zinc
introduces while and loop, and kills that loop stuff. This makes it closer to a well structured code. Rather than
spaguetty all around-

## vJass interoperability

We should be able to use Zinc in a library even if all the remaining libraries in the map are in vJass. We should be
able to call vJass libraries from Zinc and vice versa. Nuff said. Why? Because having to port whole code to other
languages is work that we can&apos;t spare. Also because vJass is a good language by itself, and we cannot expect
everyone to switch from it. In fact, they shouldn&apos;t, as there is a lot more documentation to vJass, and it has a
more human readable syntax, it is a good language for many people.

## Do not &quot;live a lie&quot;

It is best to avoid implementing features that look cool in paper but do not plain work when compiled.

## Rigidity

If you used vJass and read the stuff in this page, you will soon notice Zinc has fewer features. For example, scopes are
missing from the picture. There are lot of rules to Zinc that force you to do things more correctly. For example, you
are pretty much forced to use libraries if you wish to use Zinc at all. You cannot even write a function without a
library. What&apos;s more, all things inside a library are private - not public - by default.

## Fix vJass mistakes

This and rigidity go together. There are many things about vJass that I ended not liking at all, yet removing/changing
them would break backwards compatibility. I am taking the chance that Zinc is a new language to kill them.
