---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/2D-arrays.md"
ref: HEAD
---

# 2D arrays

A quick improvement from sized arrays, is the ability to have two-dimensional arrays, n-dimensional arrays are not
implemented, if you really need it very hard, contact me.

Two dimensional arrays in vJass, since vJass is implemented on top of Jass, are just normal arrays in disguise, using
a multiplication trick to convert 2-dimension indexes into a one-dimension one. The way to declare one of these
arrays is: `<type> array name[width][height]`, notice the real size of the array is
width*height, this size suffers the same limitations as normal array&apos;s size, it cannot go above approximately
`40800`, and if this size is bigger than `8191`, you will be using slower function calls instead of array lookups and
multiple arrays in the final script, etc.

The field size would return this total size we are talking about, the fields height and width return the ones we used
to declare the array. As with sized arrays, you can use constants for the width and size.

```sql
globals
   integer array mat1 [10][20]

   constant integer W=100
   constant integer H=200
   integer array mat2 [W][H]

endglobals

function test takes nothing returns nothing
    local integer i=0
    local integer j=0
    local integer c=0
   call BJDebugMsg(I2S(mat1.size)) //displays 200 (10 * 20)

   //fill the array:
   loop
       exitwhen (i==mat1.width)
       set j=0
       loop
           exitwhen (j==mat1.height)
           set c=c+1
           set mat1[i][j]=c
           set j=j+1
       endloop

       set i=i+1
   endloop

   call BJDebugMsg( I2S( mat1[0][1]  )  ) //displays 2

   call BJDebugMsg( I2S( mat2.width) ) //displays 100
endfunction
```