---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Loading-structs-from-SLK-files.md"
ref: HEAD
---

# Loading structs from SLK files

It is possible to load (convert) an slk into code to be added to the map&apos;s script, specifically struct
assigments. This can be really useful when a system uses structs to store data, since SLK is a table format it can
save some work and make things easier to edit without the manual struct assigning.

The preprocessor to load an slk file is `//! loaddata "path.slk"` . The file path argument follows exactly the same
rules as the ones I already specified in import (including lookup folders)

Both the slk and the struct type to be loaded need to follow very specific rules.

## The SLK

The SLK file requires to have an struct name at (row 1, column 1) Then the first row contains field names, the next
rows contain a `[key]` and values for the names specified up there.

<table>
    <tr>
        <td>stname</td>
        <td>this</td>
        <td>is</td>
        <td>just</td>
        <td>an</td>
        <td>example</td>
    </tr>
    <tr>
        <td>1</td>
        <td>2</td>
        <td>3</td>
        <td>4</td>
        <td>5</td>
        <td>6</td>
    </tr>
    <tr>
        <td>7</td>
        <td>8</td>
        <td>9</td>
        <td>10</td>
        <td>11</td>
        <td>12</td>
    </tr>
</table>

In the example, the name of the struct type to be loaded is stname, the keys of the instances that will be loaded are
1 and 7, and the rest is information about fields and values.

## The struct type

The struct type to be used (in the example, stname) requires a getFromKey static method that returns a value of the
struct type.

`getFromKey()` would simply convert a key value and return an equivalent struct, this is because you often want data
structs to be related to something else. (Most of the times the `[something else]` will be an object id.)

For this example, `getFromKey` would have to take an integer value

The struct type also requires to have the fields declared in the SLK file, the SLK is not forced to list all the
fields of the struct type.

So if we have this struct definition:

```sql
struct stname
    integer this
    integer is
    integer just
    integer an
    integer example

    static stname array values

    static method getFromKey takes integer i returns thistype
        if (stname.values[i]==0) then
            set values[i]=stname.create()
        endif
     return stname.values[i]
    endmethod
    
endstruct
```

And we also load the SLK from the previous example, the loaded init code would be:

```sql
    set s=stname.getFromKey(1)
    set s.this=2
    set s.is=3
    set s.just=4
    set s.an=5
    set s.example=6
    set s=stname.getFromKey(7)
    set s.this=8
    set s.is=9
    set s.just=10
    set s.an=11
    set s.example=12
```

Notice that this code is then run after the struct and libraries initialization

For a more practical explanation check out the SLK demo included in the JassHelper distribution.
