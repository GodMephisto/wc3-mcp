---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Changelog.md"
ref: HEAD
---

# Changelog

## 0.A.2.A
- Fixed a bug that allowed scope members to have invalid names and still compile.
- Fixed a bug that allowed natives and BJ to compile when given more arguments than necessary.
- `_F`, `_I` and `_V` do not get created anymore for array structs.
- Manual: Fixed the getFromKey example.
- Zinc: Fixed bugs that prevented some real literals to compile.
- Zinc: `while(true)` does not add a useless `exitwhen(false)`.
- Zinc: Added `do{..}while(..);`.
- Zinc: You may now separate assignments with commas. These commas can be used in the for construct. In other
  words: `for ( x=0, y=0, z=0; x<=10 && y<=10 && z<=10; x+=1, y+=x, z+=y)` is now possible.

## 0.A.2.9
- All module initializers run before any struct initializer.
- Fixed a bug that forbade comments after then in static ifs.
- `Structname.methodName.exists` should work outside static ifs.
- Static ifs should now work correctly with members/methods that are explicitly
  public or private. Note that private members/methods will not be visible to static ifs outside
  the struct.
- Zinc: Fixed random compile error bugs introduced in last version.

## 0.A.2.8
- double import check now considers full path instead of just file name.
- static ifs can check if a method exists using .exists.
- private module destroy now works like module create (is not hidden from the implementing struct)
- Zinc: structs `onInit` and library `onInit` no longer conflict.

## 0.A.2.7
- Added `comment;` import mode.
- Added `+=`,`/=`,`-=`,`/=`, and for(assignment; condition; assignment) to Zinc.
- Fixed a bug with import doing a double import when given the same filename twice in OSes that use `/` instead
  of `\`
- Fixed a crash when attempting to use special methods like allocate for function interfaces, now it gives a
  syntax error...
- Fixed a bug with function literals not working correctly when the function returned a custom type.
- Fixed a bug with function interfaces not working correctly when they had themselves as argument/return
  value.
- Fixed a bug with `//! import` inside `//! zinc` tags
- Unclosed `//! zinc` tags inside a file will get reported.

## 0.A.2.6
- `externalblock` now allows a `$FILENAME$` argument and a extension= property.
- Fixed a bug that made `/* */` comments mess up with the syntax error line number in the zinc/libraries phase
  when import was not used.
- Improved the library requirements error message.
- Zinc: Make `[implement]` cause syntax errors.

## 0.A.2.5

- Limit of strings in a single line raised from 27 to 61.
- Zinc: Fix not being able to parse certain real literals.

## 0.A.2.4

- Fixed a bug that allowed .getType() to cause jasshelper crashes.
- thistype works in static ifs. (not implicit though)
- When debug mode is on, static if phase will comment out lines instead of deleting them.
- Fixed a bug with optional requirements sometimes not working correctly.
- Zinc: `jasshelper.conf`&apos;s `[noimplicitthis]` option does not affect Zinc code.
- Fixed a bug with the shadow helper phase duplicating some comments.

## 0.A.2.3

- Fixed a bug that made .exists cause obscure syntax errors.

## 0.A.2.2

- Fixed a bug that prevented using .evalaute/ .execute on methods.
- Fixed a bug that caused syntax errors when you used a hook for a native that takes nothing.

## 0.A.2.1

- .pointer has been removed from the syntax for using methods for function interfaces.
- You may now also use non-static methods for function interfaces (they are treated as functions that take an
  extra integer first)

- Zinc: Added anonymous methods.

## 0.A.2.0

- Fixed some struct member declaration syntax errors appearing in line 1.
- Fixed a bug that made method.name return wrong values when using the method implicitely (without this).
- Fixed missing syntax error messages in the structs phase.
- Static Methods can be used as function pointers. Just do struct.method.pointer to get such function pointer.
  Implicit casting between methods and function interfaces will come later (so you do not have to use
  .pointer).

- Zinc: Allow anonymous functions in global / struct variable declarations.

## 0.A.0.1

- Fixed some undefined code related to implicit this in member usage. Which could have caused very odd code to
  get generated.

- Fixed an old bug that sometimes added indentation and comments to syntax errors.
- Zinc: Fixed a random crash related to comments.
- Zinc: Fixed a lame mistake that made all of a zinc struct's members LIBRARY-private.

## 0.A.0.0

- . or this. are not required anymore to use members. Note that this may cause issues if for some (incredibly
  weird) reason you try to use global variables from a method of a struct that has variables of the same name.
  To disable this feature, you can add `[noimplicitthis]` to `jasshelper.conf`.

- Improved the syntax error when you place a function inside a struct.
- Code values might get implicitly casted to boolexpr in some occasions, specifically, when using them as
  arguments for natives/bjfunc that take boolexpr. More cases will get added when type safety gets on its way
  for more stuff...

- Zinc: Added anonymous functions, but they cannot use locals from their parent (yet).
- Zinc: Fixed a crash that could happen when the zinc input is much smaller than the vJass output.
- Zinc: Fixed a couple of missing ; mistakes in the examples.

## 0.9.Z.5

- Added externalblock.
- optional textmacros work.
- static ifs support elseif.
- static ifs support a struct's static constant booleans.
- Missing thens in static ifs are reported as a syntax error.
- Comments inside static ifs are deleted correctly when the condition is false.

## 0.9.Z.4

- Reversed the deprecation of automatic method TriggerEvaluate, you may enable the syntax error adding
  `[forcemethodevaluate]` to `jasshelper.conf`.

- Added a syntax error when . and other unsupported operations are used in static ifs.
- Added a --zinconly command line argument that will just compile a zinc file into vJass code.
- Fixed a probable error with --macromode removing the first line of code.
- You may now use the identifier DEBUG_MODE as a constant boolean that is true if and only if debug mode is
  on.

## 0.9.Z.3

- Correct operator precedence in structs phase, this change is not noticeable unless you had an overloaded ==
  operator.

- structs phase's "Syntax Error" message is now slightly more detailed.
- You may now use a pair of decorative parenthesis in static ifs.
- custom operators used from above their declarion will once again use .evaluate automatically. (as it is not
  possible to do it manually).

- Hopefully fixed issues regarding using deallocate on child structs.
- Fixed a bug with deallocate requiring evaluate for no reason.
- Fixed a syntax error regression that happened when calling .destroy from above onDestroy.
- Zinc: When an if is all that is inside an else&apos;s contents, it is translated into elseif.
- Zinc: Compiler will try its best to keep comments, though it might place them in awkward positions...
- Zinc: Fixed a z.2 regression that made Zinc eat up parenthesis...

## 0.9.Z.2

- Fixed a crash related to `==`.
- Fixed a bug that for some reason required slks to have more than 2 columns, instead of more than 1...
- If a SLK value for a boolean member is 0 or 1, jasshelper will convert it into true or false.
- Added runtextmacro optional.
- `.evaluate()` is mandatory on methods if you want to call them from above their declaration to disable this new syntax
  error, add a `[automethodevaluate]` option to `jasshelper.conf`.
- Zinc: Make grammar allow negating a negation.
- Zinc: A while&apos;s condition is compiled neatly, being able to avoid unnecessary not operators.

## 0.9.Z.1 {id="0.9.Z.1"}

- Important: Removed virus that sneaked into some hidden folder inside the source tree
- destroy can get replaced inside a struct
- Added a deallocate method that works like allocate
- Added static versions of the `name` and `name=` operators
- Added `==` overloading (and `!=` for that matter)
- Zinc: add operator `==` to grammar

## 0.9.Z.0 {id="0.9.Z.0"}

- Added static ifs
- Added optional library requirements
- Added [](Zinc.md).

## 0.9.K.0 {id="0.9.K.0"}

- Variable shadowing is now part of correct vJass syntax. Compiler guarantees (or at least should) that there
  won't be global-local conflicts in the compiled jass code.

## 0.9.j.2

- Fixed a memory out of bounds error related to some bugged native declaration
  usage.

- Fixed issues with undeclared variables and also array member leaks when you
  use array members on an interface and do not have onDestroy declared on one of
  its children.

- Fixed a bug with methods called the same as any function not being callable.

## 0.9.j.1

- Removed returnfixer from the default, you may still toggle it on but it is not necessary anymore.
- Fixed some crashes in windows with non-cli jasshelper.

## 0.9.J.0

- Jasshelper now comes with a phase that will do its best to fix return bug false positives at the cost of an
  extra function call. This is meant as a temporary fix while blizzard fixes the bugs caused by patch 1.24 ,
  or you update your functions to avoid multiple return statements. This phase can be disabled through the
  config file. Check the updated manual for more info.

## 0.9.I.2

- Fixed a bug with function hooks not working correctly if nothing else related to function interfaces is used
  by the map.

- Fixed a bug with function hooks not working correctly at all most of the time.
- The manual no longer wrongfully states that the order of execution of onInit methods is undefined (as it
  turns out it isn't).

## 0.9.I.1

- Fixed a crash related to extends and array members.
- Fixed some chance that clijasshelper would attempt to create window.
- Fixed a bug with stub keywords causing childless struct not to call onDestroy correctly.
- Added hooks.
- It is more likely that methods using evaluate will not use TriggerEvaluate if they only call
  natives/blizzard.j functions.

## 0.9.I.0

- Jasshelper can now support native declarations around the map script, and move them to the top of the
  script.

## 0.9.H.3

- Added GetHandleId, StringHash, the gamecache Get and HaveStored natives and the hashtable Load and HaveSaved
  natives to the list of natives that do not modify the state. This means that functions that directly or
  indirectly call these natives are more likely to get inlined.

- Fixed a small typo bug inside a comment of sample `jasshelper.conf` .

## 0.9.H.2

- Fixed a freeze and out of memory bug with mass storage arrays/structs/dynamic arrays when the storage size
  was greater than 8191*13.

- The mass storage arrays/structs/dynamic arrays picker functions will now take slightly less lines of code,

## 0.9.H.1

- Fixed an off-by one error in the mass size arrays/structs/dynamic array code that caused various issues on
  boundary cases.

- `jasshelper.conf` can now determine the command line arguments given to the Jass syntax checker.
- Added key

## 0.9.H.0

- Added block comments.
- If call InitBlizzard() is not found in the main function, jasshelper will add the initializing code to the
  end of the function, instead of raising an error

## 0.9.G.3

- Fixed a crash when there were empty lines on some methods.
- Fixed a bug that prevented array structs from having static array members.
- Fixed private delegates/constants not working correctly inside modules (and possibly causing further bugs)

- Fixed problems related with clijasshelper not working in windows&apos; cmd.exe.
- If for some bizare reason, there's no stdout assigned for clijasshelper, it will automatically send its
  output to stdout.txt.

- GetUnitUserData is now considered a non-state changing function by the inliner, which should increase the
  chances of functions that use it to get inlined.

## 0.9.G.2

- Fixed a regression introduced in G.0 that caused various issues with function interfaces.
- Fixed &quot;operators that return self&quot; adding a chance to cause odd syntax errors, stack overflows and
  access violations.

## 0.9.G.1
- Modules&apos; private members are now truly private, unlike the other members, they are not visible to the
  calling struct, and their names will not collide with other names declared in the struct.

## 0.9.G.0

- Added Modules and thistype.
- Will now call the optimize phase after PJass, as it was always intended, this should prevent some crashes
  during optimizations.

- Functions can now use .name in a similar way to methods.
- Fixed a bug that made child structs ignore the parent&apos;s storage size if a constant was used.
- Fixed a crash when the user attempts to assign a static array member.

## 0.9.F.7

- $ Is now supported as hexadecimal prefix in integers - turns out wc3 always did.
- You can now tweak jasshelper.conf to set a different Jass compiler (i.e. change pjass.exe requirement into
  foojassc.exe).

- Fixed bugs related with displaying errors in blizzard.j / common.j in an unusual jasshelper setup is unusual
  like newgen&apos;s - several situations in which it would fail to show the correct file in the syntax error
  window/report have been fixed.

- clijasshelper will now specify the script file in which the errors were found.

## 0.9.F.6

- jasshelper will avoid using TriggerEvaluate when the evaluated function/method does not contain function
  calls.

- function interfaces will consider all custom types as integers when comparing for validity, later it will
  have some type safety (i.e. you will not be able to use a integer in place of a struct, but you would be
  able to do the opposite) but for now it is type unsafe, use with care.

- member declarations with odd characters between the type and the name are now reported as syntax errors.

- static 2d arrays used to calculate their storage space incorrectly which caused issues like them not using
  get/set functions correctly, this has been fixed.

- It is not anymore possible to declare a member variable with the same name as a method operator.
- Fixed a documentation bug in the section about interfaces.

## 0.9.F.5

- Old OS/X line breaks are now supported in input .j files, this would most likely be useless to everyone
  unless a bugged text editor saves the file using those...

- Fixed a crash bug introduced in 0.9.F.4 related with structs that extend interfaces/stub methods and don&apos;t
<i>override</i> the method.

## 0.9.F.4

- stub on a childless struct is not going to cause a syntax error anymore.
- The getType() method can be called on any struct instance.

## 0.9.F.3

- More stub related bug fixes.

## 0.9.F.2

- Fixed a bug when using super on methods that had arguments.
- Fixed a bug with bigarray.size using an undefined type which caused some type comparisons errors.

## 0.9.F.1

- Fixed some bugs with stub methods on structs that extended interfaces.
- clijasshelper?

## 0.9.F.0

- Fixed the return bug detector, there will not be false possitives, which means more functions will be
  inlined.

- Added stub methods.
- Added super.

## 0.9.E.1 {id="0.9.E.1"}

- Fixed issue with operator priority in result code of using 2D arrays.
- Fixed compile error caused by .execute on static methods with arguments.
- Fixed an usual chance to incorrectly inline return bug exploiters.
- Single-line return bug exploiters now recognized as non-state changing functions by the inliner (Increases
  chance to inline certain functions).
- dynamic array declarations now report garbage code after the end of the declaration.
- Fixed a bug with scope initializers making a next library unable to have nested scopes.
- Array structs can now have a max size specifier after `array`.
- Fixed a readme bug, it incorreclty stated the command line arguments order.

## 0.9.E.0

- Added --macromode.
- Added method.exists.
- Added 2D global arrays (and 2D static array members)
- Extra text after certain array size declaration is not ignored anymore (a correct syntax error now
  appears)

- Fixed certain readme bugs.

## 0.9.D.3

- Array structs now work as intended.
- Fixed yet another regression with method calls.

## 0.9.D.2
- Fixed a `[list out of bounds]` error when using `.method()` syntax.

## 0.9.D.1

- Non-static methods that take no arguments are possible to be called again i.e: .destroy().
- Delegate cycles will now cause a crypting syntax error, which is better than jasshelper overflowing the
  stack.

## 0.9.D.0

- Added delegate.
- Added functionname for function pointer values.
- Added a way to get the return value of `.name=` and `[]=` assignment operators.
- Added extends array.

## 0.9.C.1

- Fixed a problem caused by waits inside library initializers, they prevented other library initializers from
  running.

- SLK cells with either - or _ as ignored, whitespace inside these cells is also ignored.
- Fixed syntax errors caused by international compatibility issues in certain setups.

## 0.9.C.0

- Fixed a crash that happened if you attempted to assign a method. (set
  x.create=2 )?

- Fixed a bug that made children struct cause syntax errors if the parent
  uses extra space.

- Fixed a conflict between --nopreprocessor and using script arguments (the
  script used to be ignored)

- Fixed a conflict between --nopreprocessor and --scriptonly though it would
  be nonsense to use both simultaneously anyway...


- Inline phase no longer gets confused by control characters in strings.
- Inline phase no longer gets confused by control characters in strings
  (Both of these were meant to handle those things correctly, but there were
  small bugs in certain functions that made the provisions fail)


- Added the colon operator.
- Improved the syntax error caused by getFromKey not taking a single argument
- Fixed a bug with the SLK parser that made it unable to parse SLKs with UNIX
  (normal) linebreaks.

- Certain loaddata syntax errors will now also point to the location of the
  related struct/member.

- Cells with - are now ignored (that would have created bad syntax anyway).
- Compatibility with yet another odd quote sequence used by openoffice in SLKs.
- Given how absurdly hard and tool-dependent it is to use &quot; in a SLK
  cell, jasshelper will now automatically add &quot;&quot; to cells in columns for
  string-typed cells that don&apos;t have them. (unless it is - or an empty cell,
  in which case it will use the default value, which is probably "" anyway)

- Similarly, If a column&apos;s field is of integer type, it will add &apos;&apos; automatically to
  non-integer values of length 4 or 1 inside cells.

- Code generated by loaddata is now split in function batches of 100 loaded
  structs each (each using its own thread), long functions cause PJass errors
  and might also cause thread crashes...

- If you use //! import from an imported file, you are able to use
  relative file paths based on the importing file's location.


- Backwards incompatibility: I hope no one was using variables in SLK cells. It should still
  work if the type is not string or integer, if the type is integer, variables
  would still work provided their name length is not 4 or 1.

## 0.9.B.1

- Once again struct members can be initialized.

## 0.9.B.0

- Fixed a bug with the first jasshelper phase which used to cut the last line of an input file potentially
  causing problems if WE decides not to print a last empty line (which apparently happens sometimes).

- Fixed a chance for access violation after the structs phase finishes.
- Jasshelper no longer ignores extra code after a struct member declaration (It now shows an error).
- Jasshelper no longer ignores extra code after a local variable declaration (It now shows an error).
- Structs now allow sized static array members.
- Added //! novjass and //! endnovjass.
- Added --scriptonly and --warcity
- global variable addition order is now affected by library requirements.

- Fixed a couple of &quot;&amp;aquot;&quot; bugs in the readme file.
- The readme file comes with description about jasshelper&apos;s command line options.

## 0.9.A.0

- Added inline phase and --nooptimize

## 0.9.9.B

- Fixed a bug that would cause syntax errors when two or more scopes got initializers.
- Maximum extended array size limit increased to 409550.
- Dynamic arrays allow sizes smaller than array&apos;s storage limit / 8.

## 0.9.9.A

- Fixed yet another bug that prevented f__arg_this from being created.
- Scopes now use normal calls instead of ExecuteFunc for initializers.
- Updated readme, scope initializers are mentioned, made it aware //! for libraries and scopes now cause a
  syntax error.

## 0.9.9.9

- Hopefully fixed onDestroy issues with extends and similar.
- Fixed memory corruption when using `[]` on structs/interfaces to increase index limit.
- Added big sized global arrays.
- Library declarations more likely to survive comments.
- Fixed hang outs related to wrong use of extends.
- Fixed bug with defaults on methods that return custom types.
- Scopes can have initializers.

## 0.9.9.8 (test release)

- Fixed a bug with f__arg_this not being declared when required if extends is involved.
- Fixed a crash related to misplaced endscope inside a library.
- Fixed a bug with array members in child structs, possibly &quot;leaking&quot;, which means they did not get
  recycled properly and the struct would eventually malfunction.


- Can declare methods to replace variable access and write operators (method operator name and method operator
  name=)

- &quot;member is private&quot; syntax error now also shows the name of the involved struct.
- Access violations will report a related line of code if possible
- Can setup max index space on dynamic arrays and structs ( type arrayname extends typename array
  `[ instancesize, spacerequired ]` )( struct `name[spacerequired]` )( interface `name[spacerequired]`
- In order for last feature to work I have to modiffy plenty of things, expect an unstable version, releasing
  it so I could get free testing...

## 0.9.9.7

- Fixed a bug with methods, pseudo inheritance and interfaces that is just too hard to explain.
- Fixed a bug with third generation (and above) child structs and array members.
- Scopes and libraries may now use numbers in their names (Still not _).
- External command line length limit extended to 1000.

## 0.9.9.6

- Fixed a bug with onDestroy if it contains calls to methods from other structs and is used on an struct
  extending another struct.

- `[]` and `[]=` operators can also be declared as static.
- Order of addition of libraries that don&apos;t extend each others is now sorted by name.
- empty interfaces or onDestroy methods are assigned to null rather than not assigning their arrays, it
  probably was all right but this sounds better.

## 0.9.9.5

- Fixed more bugs related to onDestroy and extends.
- Fixed various bugs relating to syntax errors and library declarations.
- Fixed a certain line off-set present for syntax errors after files are imported.
- Fixed a readme bug with the changelog.

## 0.9.9.4

- Fixed a bug with static methods that return custom types and require evaluate.
- Fixed plenty of bugs related to &quot;running&quot; undeclared textmacros.
- Fixed plenty of bugs related to onDestroy methods when inheritance or interfaces are involved.
- Fixed an access violation crash when the `[]` operator is used on methods
- Fixed probable (local-not-set-to-null) memory leak with function interfaces, methods called from above their
  declaration, evaluate, execute and function interfaces

- Long external calls will now popup an error instead of crashing jasshelper.
- Added onInit method support for structs.

## 0.9.9.3

- Fixed a bug with return statements in interface functions that return nothing
- Fixed a crash with --nopreprocessor that introduced a long ago.

## 0.9.9.2

- Fixed a major bug causing desyncs (All functions generated by jasshelper that are passed to Condition() will
  have a boolean return value)

- struct.typeid now returns an accurate integer not dependant on parent ids and is replaced by a constant
  added to the map script instead of a single number.

## 0.9.9.1

- Fixed a bug with static methods that return custom types and required evaluate mode, causing some pjass
  parse errors.
- Structs may now extend other structs.
- Interfaces are now allowed to declare a rule so that constructors of the interface&apos;s children do not
  declare a create method that takes arguments.

- interface.create() will now return 0 when there is attempt to call the private allocate() method.

## 0.9.9.0

- Fixed a critical error causing infinite loops when there were external commands used for grimoire version.

- Improved Wine compatibility again, should work with more versions of Wine including the newest.
- Grimoire version&apos;s compile error window used to have a chance to look akward under certain windows UI
  settings, this problem is fixed.

## 0.9.8.9

- Documented inject.
- Added information about problems related to sync natives and methods/evaluate().
- Improved the error message given if InitBlizzard() is not present in the main function.
- New command line option to use an external war3map.j instead of the one found in the map.
- Improved compatibility with WINE, at least on the recent WINE versions I have tested jasshelper.exe and it
  can compile a map fine.

- Added some syntax errors for maluse of dynamic arrays or array members
- Fixed issues with commented-out/incomplete return statements inside certain methods or functions.
- private or public are ignored when using on scope declarations, used to silently cause other bugs before.

- Made usage of public and private more strict to prevent bugs (Instead there would be syntax errors)
- Added: keyword
- `[scope symbol redeclared]` syntax error will now also specify the first declaration of the symbol.
- Made certain statements more strict, some used to allow extra text after the statement (scope or globals for
  example)

- Made a syntax error related to libraries more understandable.
- Struct member initializers may now use . syntax (it used not to parse them)
- Added a .name field for static methods, returns the string of the generated function name.
- interfaces can use .create if given a typeid value.

## 0.9.8.8

- Fixed plenty of issues with parsing of runtextmacros and handling of syntax errors in textmacros.
- Child structs may override the initia default values of members of the interface.
- structs allocation needs one less array and is a littler faster.
- Improved error message given when someone makes an attempt to make an struct that extends an struct.
- Can use .execute() on methods.

## 0.9.8.7

- Fixed a bug that caused issues with functions that returned custom types.

## 0.9.8.6

- Fixed a bug with scope private/public members used inside an struct.
- Optimized performance of struct stage.
- Compiler will now prevent name conflicts and rais errors if it finds them, these prevent bugs later but it
  is possible that an old name conflict
  in your map survived for a lot of time and this new jasshelper version will popup a previously unheard error
  for that map.

- Undeclared InitTrig functions are ignored instead of poping syntax errors, this allow for cleaner usage of
  the trigger editor with libraries.

## 0.9.8.5

- Added defaults keyword for interface methods.
- Fixed some bugs with the installer for WEHelper, should be easier to install to 1.8 although you still have
  to specify the path.

- Fixed a bug that made jasshelper unable to recognize hex integers if they had lower case letters.
- Formatted the manual a little.

## 0.9.8.4

- Fixed a bug with interface extending structs with multiple array members.
- Fixed a bug with international characters inside strings.
- Fixed some scoping issues, locals are now handled correctly by the structs convertor.
- Added getType() and typeid for interfaces.

## 0.9.8.3

- Fixed a possible compiler crash.
- Fixed a bug that made function interfaces unable to have arguments of custom types.
- Modified the way grimoire version works in many ways, just replacing the executable will not work. A new
  version of newgen pack is released which you should update, else you may also have to wait for a new
  grimoire version.

## 0.9.8.2

- Fixed a 0.9.8.0 bug that prevented dynamic arrays from being used.

## 0.9.8.1

- Fixed a bug with function evaluate/execute methods using wrong variables.

## 0.9.8.0

- Structs now come with an internal private static method called allocate that does what .create used to do.

- If no correct static method create is declared within an struct body, a default one which calls .allocate is
  added.
-
- Functions are now objects, you can call methods evaluate and execute on function names no matter the
  position of the function declaration, execute() runs the function in another thread.

- Added function interfaces, this allows function variables and other fun stuff.

## 0.9.7.4

- Fixed bugs with array members in structs that extend interfaces.
- Fixed syntax error bug with calling .destroy above an onDestroy declaration.
- Improved performance of interfaces (reduced a function call when calling a method, and improved constructor
  performance).

- Also improved performance of methods when called from above their declaration

## 0.9.7.3

- structs may now have array members.
- Constant integer variables may now be used for size of dynamic array and array member declarations.

## 0.9.7.2

- &lt;, &gt; comparissons between zero and an struct type are allowed again.
- Fixed logic flaw in implementation of &lt; operator for interfaces that made them unable to use it.
- Added SCOPE_PREFIX and SCOPE_PRIVATE, assist to use ExecuteFunc / real variable events on scope
  private/public members, and are also useful for debugging.

- Public InitTrig function is now translated to InitTrig_ScopeName instead of ScopeName_InitTrig (makes some
  stuff way easier, specially for JESP spells)

- Grimoire version now writes logs in a logs subfolder as compliance with newest grimoire version.
- Fixed multiple bugs related to handling SLKs saved by certain versions of MSExcel

## 0.9.7.1

- Fixed a major bug introduced on 0.9.7.0 that made interfaces useless.

## 0.9.7.0

- Fixed bug with big string literals.
- Fixed major bug with the way dynamic array indexes are handled.
- Added an integer() typecast operator (for structs and dynamic arrays only, we&apos;ll soon have an actual
  typecast operator for native types).

- Added operator overloading for `[]` (both get and set) and &gt;
- Fixed a bug with some syntax error showing extra, debugging information that was supposed to be removed
- Structs extending an interface no longer have to be declared after the interface
- Will now show a proper syntax error if a method derived from interface is declared as static, instead of
  generating bugged code.

## 0.9.6.3

- Fixed requirement of whitespace before `[` in dynamic array declarations.
- Fixed some issues with nested methods causing misleading syntax errors.
- Fixed some problems with local variables/arguments with the same name of previous local variables/arguments
  that were of custom types while the new ones were not (causing some confusion/odd syntax errors).

- Fixed a probable issue with dynamic array indexes
- Added instructions about how to update jasshelper in newgen pack

## 0.9.6.2

- Fixed syntax errors that could appear if there were special characters in strings or comments.

## 0.9.6.1

- (WEHelper only) fixed a bug that made worleditor unable to ever finish compiling.

## 0.9.6.0

- Added dynamic arrays (type name extends anothertype array `[size]`).
- Added typecast operators (for struct types, soon we will have them for all the types).
- It might now detect some few syntax errors before the pjass stage (prevents confusion when there are errors
  in code that is already compiled by JassHelper)

- Instances might also call static members
- Fixed a bug with the readme&apos;s html

## 0.9.5.2

- Fixed a bug with struct methods that had more than one argument of different types.
- JassHelper now repeats its process after external tools are executed.
- Added an interfaces demo.

## 0.9.5.1

- Fixed a bug that could cause `[Undeclared variable f__arg_this]` pjass errors when saving
- Fixed a bug with `//! import` not importing the last line of the file.
- Fixed a bug with `//! import` not being able to import a file if quotes weren&apos;t used for the path and
  there were comments after the command (may happen if import is the last line of a world editor `[trigger]`])

## 0.9.5.0

- Can now convert slk files to struct assignments with the //! loaddata preprocessor.
- Added the //! external preprocessor which allows you to configure jasshelper to run command line tools, the
  way the command line tools have to work is very specific so if you should check out the manual if you are
  interested in making them.
- WEHelper&apos;s plugin has now dialogs to configure lookup folders and external tools. Grimoire&apos;s
  mapcompiler can take advantage of a .conf file.
- Grimoire mapcompiler is now able to run wewarlock once configured correctly.
- Jasshelper&apos;s import can now be used in WEHelper if WEHelper&apos;s is disabled. The advantage you can
  get from it is the ability to configure the lookup folders.

- Fixed some terrible typos in the interfaces explanation of the readme

## 0.9.4.4

- Fixed a compiler crash when there was an struct (something) extends (something else) when the parent struct
  name wasn't declared yet.

- Fixed a chance for the struct usage to generate game-crashing code.

## 0.9.4.3

- Fixed a bad bug that could cause access violations if textmacros are used extensively
- Fixed a bug with public/private members not being replaced accordingly on lines that had a / in them.

- Grimoire version allows relatives paths for //! import , you can specify where to look for files in the
  newly set mapcompiler.conf file

## 0.9.4.2

- It is again safe to call jasshelper twice. (Fixes some issues with testmap and WEHelper)

- Grimoire version now includes a beta of //! import , use //! import on complete paths only (for example: //!
  import c:\goo.j )

## 0.9.4.1

- Documented 0.9.4 features.

- Fixed a bug with static methods on interface extending structs causing PJASS errors.
- Fixed a bug with static methods with no arguments having a chance to cause compile errors that to make
  matters worse were not detectable by PJASS.

- Fixed a bug with default values being ignored on structs that extend interfaces.

## 0.9.4

- structs can now have methods.
- Added interfaces
- Added //! inject
- Again, documentation of new features would take a while
- Compiler for grimoire now got a progress bar and uses SFMPQ.dll directly instead of mpqutils, which should
  be faster and also be compatible with a later version of grimoire which will remove mpqutils and replace
  them with mpq2k.

## 0.9.3

- Fixed multiple bugs in compatibility between scopes and structs.
- Fixed a minor issue with the grimoire compiler.
- Updated some sections of the manual, added more info about structs.

## 0.9.2

- Fixed grave bugs probability when many structs were used
- Destroying the 0 struct will do nothing instead of sending it to the recycle stack.
- Include compiler to be used by grimoire&apos;s wehack.dll.
- Documented 0.9.0 aditions.

## 0.9.1

- Fixed a wrong syntax error when comments with triple / were present
- Fixed a syntax error caused by having dot characters inside comments
- The new additions to the syntax are not documented yet

## 0.9.0

- Fixed some wrong instructions that could end up causing access violations
- Fixed a bug with some textmacro declaration errors giving the wrong line number.
- Fixed a bug that made libraries unable to have child scopes unlike what the documentation said.
- Fixed a bug with private/public that made it unable to rename identifiers correctly after single /
  characters.

- Added library_once
- Added textmacro_once
- Added structs, dynamically allocated object types. Seriously.
- The new additions to the syntax are not documented yet

## 0.8.0

- Added `//! textmacro` support.
- Fixed a bug with private/public that didn&apos;t process global variables correctly if they were initialized
  and had = stuck to the name.

## 0.7.0

- Better handling of some syntax errors.
- Nested scopes are now legal.
- Added the public keyword.
- Cut the file size.

## 0.6.1
- Added installer for the WEHelper plugin.

## 0.6

- Added private keyword and `//! scope`.
- Various optimizations, specially for the plugin edition.
- JASSHelper is called again after WEWarlock, so you can use features like debug or private in files called by
  `//! require`

## 0.5.2

- For WEHelper 1.5.2

## 0.5

- Fixed wrong error messages in the case of unclosed strings causing issues.
- Fixed a bug that made debug cut the last character

## 0.4

- Fixed a bug that made this preprocessor unable to recognize `globals//comment` and `endglobals//comment`.
- Removed the progress bar from WEHelper plugin
- Added WEWarlock support to WEHelper plugin (Can call the WEWarlock compiler, and you can use wewarlock&apos;s
  features in your map by just saving.).

- For WEHelper 1.5.1

## 0.3

- initializer will now call the init functions AFTER call InitBlizzard() allowing to use blizzard globals on
  init functions.
- requires and uses also work in the place of needs
- For WEHelper 1.5

## 0.2 (initial public release)
