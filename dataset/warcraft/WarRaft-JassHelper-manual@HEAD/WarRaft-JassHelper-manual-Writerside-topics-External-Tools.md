---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/External-Tools.md"
ref: HEAD
---

# External Tools

JassHelper allows the `//! external` preprocessor that let you call other tools on the map after the map is compiled
with JassHelper, this way these tools may work the same with grimoire and WEHelper

The preprocessor is `//! external EXTERNAL_TOOL_NAME [external arguments]`

EXTERNAL_TOOL_NAME must match the name of the tool in the configuration (Either the dialog in the wehelper plugin or
the `.conf` file for the grimoire version)

The text after the external name is optional and is given to the tool as command line arguments.

You may also use the `externalblock` preprocessor if you want to specify an input that will be send to the external
tool in stdin:

```sql
//! externalblock EXTERNALNAME ARGUMENTS LIST
//! i These lines will get send to
//! i The tool as stdin
//! i The i and the space after the i are ignored.
  
     //comment and whitespace not to appear in stdin

//! i
//! i That one up there was just an empty line to send to the program
//! endexternalblock
```

What if the tool supports not stdin but a file? Well, you can make jasshelper save the contents of `externalblock` in a
temp file and then send that file to the tool using `$FILENAME$`

```sql
//! externalblock EXTERNALNAME SOME_ARGUMENTS $FILENAME$ OTHER_ARGUMENTS
```

If you try this you will find out that some tool makers out there for some strange reason require you to use specific
extensions for the provided file names. Well, in that case you may use `extension=`

```sql
//! externalblock extension=lua OBJECTMERGER SOME_ARGUMENTS $FILENAME$ OTHER_ARGUMENTS
```

The next is kind of a section for programmers:

## What kind of tool?

For a tool to work correctly with the external preprocessor it must have a very specific behaviour.

First of all JassHelper will pass these arguments to the tool

- The map&apos;s file path.
- A tokenized chain of directory paths ( `c:/;d:/somepath;c:/anotherlocation/` ) These are the lookup paths used by
  JassHelper, in order to fix some issues the `\` character is replaced with `/` here.
- The rest of the line in the external preprocessor. (The tool might or might not have more arguments)
- Then the tool must use the 0 code if it was succesful, otherwise return any number different to 0 and should write an
  error message to either stdin or stderr.