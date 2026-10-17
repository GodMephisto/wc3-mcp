---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/public-or-private.md"
ref: HEAD
---

# public or private

Library members (variables, functions, structs, interfaces, types) can be either private or public.

private
: Only your library has access to the member in question, other libraries may have their own private or public members
using the same name, it would not matter.

public
: The member is available for use on other libraries. If two public such members in different libraries have the same
name, an error will happen, therefore, please be careful, and name your public stuff carefully.

When declaring a library member, you can place it inside a `public` or `private` block to specify what access
rule you want. Note that library members are `private` unless you specify otherwise.

```C++
library PublicTest
{
    public
    {
        integer PublicTest1;
        integer PublicTest2;
        integer PublicTest3;
        //all these variables are public.
    }
}
```

If you are only including a single member in one of these blocks, the `{}` are optional. There is also no need to include
all public members in the same block. This looks like vJass/Java/etc code, but is equivalent to the previous example:

```C++
library PublicTest
{
    public integer PublicTest1;
    public integer PublicTest2;
    public integer PublicTest3;
}
```