---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Array-members.md"
ref: HEAD
---

# Array members

Structs may have array members but you also require to declare the size of them.

```sql
struct stack
   private integer array V[100]
   private integer N=0

   method push takes integer i returns nothing
      set .V[.N]=i
      set .N=.N+1
   endmethod

   method pop takes nothing returns nothing
      set .N=.N-1
   endmethod

   method top takes nothing returns integer
      return .V[.N-1]
   endmethod

   method empty takes nothing returns boolean
      return (.N==0)
   endmethod

   method full takes nothing returns boolean
      return (.N==.V.size)
   endmethod

endstruct
```

In some way, this is syntax candy for declaring a new array type and making it a member of the struct, but this way
is a little more optimizer and handles the array allocation/deallocation for you, with the exchange of some
limitations.

Notice that this drastically reduces the instances limit of an struct type, for example, we can only have `80`
instances (`8190 div 100`) of the above declared stack object.

An struct may have as many array members as you can type, notice that the array with the maximum size is the one
considered when setting the instances limit, so if an struct has 2 array members, one of size 4 and one of size 100,
the struct will have a limit of 80 instances.

The disadvantage of this method over declaring the dynamic array type separatedly is that you have less freedom in
what concerns assigning to the member another array you create in other occation and things like that... The
advantage is that it is faster and takes less code.

As dynamic arrays, array members may also use the `.size` field.