---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Creating-and-destroying-structs.md"
ref: HEAD
---

# Creating and destroying structs

Structs are pseudo-dynamic, you would often need to create and destroy structs, and you should create an struct an
assign it to a variable before using it.

The syntax to _create an struct_ (It is actually to get a unique id) is : `structtypename.create()`

In the case of the above struct you would have to use `pair.create()` to get a new struct.

JassHelper is just a preprocessor not a hack so whatever adition to Jass we add is still limited by Jass&apos; own
limitations, in this case, structs use arrays which have a 8191 values limit, and we cannot use index 0 which is null
for structs, so there is an 8190 instances limit. This limit is for instances of each type, so you may have 8190 objects
of a pair struct type and still are able to have many other instances of other types.

It means that if you keep creating many structs of a type without destroying them you would eventually reach the limit.
So keep in mind this: IN the case the instances limit of a type is reached `structtype.create()` WILL RETURN 0.

The limit is not usually a worrying issue, 8190 is in practice a huge number, unless you want to make linked lists or
things like that, but those should be solved by a lower level approach.

For example, if you are only using structs for spell instance data, it is even impossible to get more than 9 instances.
And many other practical applications would never need more than 2000 instances.

UNLESS, of course some structs that are not used anymore are not getting destroyed. In that case the limit is reached by
a bug in the usage of structs that should be fixed (In the case of structs, unlike handles, not destroying them does not
increase the memory usage, but the risk of reaching the limit)

If you make calculations, if you create an struct per second and always forget to remove it, the map would need 2 hour,
16 minutes in order to reach the limit.

Either way if you are making this for usage by other people and are unsure about the possibility of reaching the limit
you can always use a comparission with 0 after calling create() and block the process somehow in case the error is
found.

In case the limit is reached and you don&apos;t have a way to catch it, struct 0 would be used and assigned and probably
cause some conflicts later, the strenght of the conflicts could be null or huge depending on the way you are using the
structs.

If debug mode is on when compiling the script, create() will show a warning message whenever the limit is reached.

To destroy an struct you simply use the destroy method which can work as an instance method or as a class method, in the
above example call pair.destroy(a) is used to destroy the instance, but you could also use call a.destroy().

In the case you attempt to destroy the zero struct destroy will do nothing or would show a warning message if debug mode
is on.