---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Delimited-comments.md"
ref: HEAD
---

# Delimited comments

These are the typical `/* ... */` comments, that you can use to comment out blocks of code not necessarily ending with
a line break. These comments are then just deleted from the map script. You can also nest these comments and do
funny things as well...

```sql
/* Delimited comments example
 They are a lot more useful than normal
 comments, really

 */
function test takes nothing returns nothing
    call Something( /*5*/ 66) /*We temporarily commented out 5, and replaced it with 66*/

    /*
    call Something( /*5*/ 66)
    */

    // That comment up there contains another delimited comment... /*
    call BJDebugMsg("Notice how the previous comment start was ignored" + /*
    */+"because it was inside a 'normal' comment "+/*
    */+"Also notice how we made the parser skipped the previous "+/*
    */+"line breaks because they were inside a comment"+/*
    */"These comments do not count if they are /*inside a string*/ ... ")

endfunction
```