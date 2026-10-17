---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/README.md"
ref: HEAD
---

LUA Declarative Cutscene & Dialog System
by Macielos

Let's say you wrote (or you plan to write) SHITLOADS of dialogs for your map or campaign in a form like below:

Arthas: I'm glad you could bake it, Uther.
Uther: Watch your tone with me, boy...

How much work do you need to actually turn it into a full-fledged in-game dialog or cutscene? Well, hell of a lot. That was my thought too and because I'm lazy, I decided to code something that will automagically manage cutscenes and dialogs for me, based on some configuration (a list of messages with some additional properties when necessary). In time, I added a lot more features to create a more interactive, RPG-style dialog structure in which you can make choices, react differently, have some extra topics you can ask NPCs about.

It's one of several larger pieces of code I'm working on for Exodus 2 campaign, the first bigger thing I coded in LUA (before I coded in Java, C#, Python and more) and so far the only one polished to a degree I'm not ashamed to show it to others. It still has some issues and you can surely expect me to work on it, but it's usable and already really time-saving, so I decided to share it to gather some feedback before you can see it in all its glory in Exodus 2.

===========

Features:

Declarative Dialog System
- Send lists of unit transmissions one after another, with custom dialog window, auto-management of display times and nice text rendering animation
- Automated scrolling up/down when your text is too long to fit
- Skip single lines immediately with RIGHT arrow when you're done reading or just want to get through a dialog faster without skipping the entirety of it
- Easily declare order of messages, branching, loops, resolving next message based on LUA functions or variables, allowing you to easily prepare highly interactive and non-linear dialog structures
- Involve choices in your dialogs - display a list of options and make a scene halt until you pick a dialog option (UP/DOWN arrows) and confirm it (RIGHT arrow)
- On each message, with delays if required, you can run your own trigger, custom function or even a list of actions
- Every message is logged into a transmission log (under F12) so you can re-read them later

Automated Cutscenes
- Playing a dialog with cutscene configuration requires FAR less code/trigger actions than preparing a cutscene trigger by hand
- You can handle cutscene logic (e.g. camera movements, units moving or turning to one another) by attaching triggers or custom functions to specific messages (with delays if needed)
- Unlike standard cutscenes, automated ones have user controls enabled so that the player can skip lines or make choices. A pleasant "side effect" is that you can pause a game, change options, see transmission log (with previous messages you missed), save and load a game during cutscenes. So far - surprisingly enough - I encountered no issues when loading a game in cutscene. You can easily disable this feature in configuration, but that way you won't have skipping lines or choices
- Automated cutscene skipping - you can either skip single lines with RIGHT arrow, or an entire cutscene with ESC. You no longer need to have if-skipped-then-skip-actions-else-do-nothing after every action in a cutscene trigger. You only need to implement a dialog ending trigger and the mid-game logic is attached to specific dialog lines. For actions happening in the midst of a dialog line you can use SimpleUtils.timedSkippable(delay, function() ... end). You can also make certain parts of a dialog unskippable (I plan to add some icon to show when a player can skip).
- Fadeins/fadeouts handled by simply defining e.g. fadeOutDuration = 2.0 in a message and fadeInDuration = 2.0 in a following one

Dialog types and customization
- Use a prepared screenplay in a cutscene, in an RPG-style dialog or a simple non-interrupting list of messages.
- Use one of predefined screenplay variants, modify them or prepare your own. You can modify size and positions of UI elements, as well as dialog behaviour like whether or not to lock camera, pause units, show unit flashes or make cutscene interactive
- Just fair warning - not every combination of configuration parameters has been tested, some options are not intended to use together, e.g. simple dialogs will act weird with choices because right arrow also moves the camera right. I suggest to start with predefined configs, but feel free to experiment at your own risk and report suggestions how to make configuration more intuitive and less prone to errors

===========

Sources:
The source code and screenplays from demo are available on Github:
https://github.com/Macielos/Warcraft3LuaLibs

Getting Started:
I suggest to familiarize with the system on a demo map which tells a story of a fellow footman on an epic quest to find
beer. See the screenplays within the map triggers or on Github, then try to modify them and experiment. It's best to do
it in some IDE (I personally use IntelliJ Idea with LUA plugin) or text editor that highlights syntax, brackets,
does formatting etc. You can find a list of available fields and their descriptions in a file ScreenplaySystem.lua,
near the beginning. If there's demand, I can prepare some more detailed docs.

Usage:
- Download a demo map
- Copy Import folder
- Open your map, make sure you have LUA as your script language in map options
- Paste Import folder
- Prepare your screenplays and use them in triggers
- Enjoy

You can speed up working on screenplays by using a small tool I wrote (requires Java 18+, so far you need to build the
project yourself using Maven, I can provide a jar if there's any interest in the tool):
https://github.com/Macielos/ScreenplayGenerator/

===========

Notes:
- This library overrides InitBlizzard() function to call custom initialization code
- This library was never meant to be used in multiplayer. Scenes are displayed for local player only and there is no data syncing mechanism. My guess would be that non-pausing config variants will work, but for pausing ones you'd have to handle pausing game for other players and events like: one player has a choice window open and the other one attacks him.
- This library uses the following globals:
  - global variable screenplayActive
  - a camera called sceneCam (must be created)
  - all global functions it adds are in GlobalFunctions.lua

Compatibility:
- Probably Reforged only. It COULD work on 1.31 as it only uses very basic native functions, if you manage to get LUA working there (I couldn't). 

Known issues:
- You can unlock a camera in cutscene by pressing e.g. F10. To fix it I periodically 'adjust' the camera. 
- Because of above, instead of standard camera functions/trigger actions you should use ScreenplayUtils.interpolateCamera(cameraFrom, cameraTo, duration) or ScreenplayUtils.interpolateCameraFromCurrent(cameraTo, duration). See elf screenplay in demo for examples.
- If the camera is kept idle, it does not get locked at all, so you can freely move it during cutscene. Using ScreenplayUtils functions prevent that by making camera move back veeery slowly if it completed movement and there is no next one submitted

Credits:
@Planetary - my system began as a modification of this system:
https://www.hiveworkshop.com/threads/lua-jrpg-dialogue-system-v1-3.327674/
In time I expanded and reworked it to a degree there's hardly any original code left, but I still use its UI files and some basic code structure. Planetary gave me permission to use fragments of his system. He said he doesn't need any credits, but his system is cool and gave me lots of inspiration, so I'm giving him credits anyway :P. 
