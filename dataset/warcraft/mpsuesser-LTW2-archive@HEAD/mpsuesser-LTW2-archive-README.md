---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "README.md"
ref: HEAD
---

LTW2!

Some quick but important notes:
* LineTowerWars, LineTowerWarsServer, and LineTowerWarsShared are all _separate_ Unity projects.


## LineTowerWars

The client project. Once being built, this project would be the downloadable, playable version.


## LineTowerWarsServer

The server project. Once being built, this project would be the version that gets run on the server side and manages a few things:
* The game settings
* The state of the game
* Connections between all active clients

The flow of the server build from start to finish looks like:
* Initialize the game settings
* Establish the lobby, wait for connections, manage the lobby state
* Start the game, wait for clients to be ready to begin the game, then initiate the actual game loop (i.e. begin the countdown to the start of the game)
* Maintain the state of the game in real time, until the game ends
* When the game loop finishes (i.e. someone wins), aggregate the stats, send them to the players, and upload the match data to a centralized backend DB

At its conclusion, the server should be killed. There is _not_ currently support for cleaning up the state and starting fresh for a new lobby.

## LineTowerWarsShared

Not really a playable build! This project intended to be the place I kept assets that are _shared_ between the client and server project. It's a messy way to keep things together and eventually should be retired in favor of a better, more automated system. Even as I write this, I'm pretty sure it's a bit out of sync with the actual client/server shared script directories because I got lazy and stopped copy/pasting updates to a _third_ project, and instead just kept in sync between client and server.

Also intends to be the place where map editing happens -- the "map" being the physical game world where a LTW match takes place (the lanes, the environment scene, etc).

----

Nobody else is working on this with me, so that's the high level stuff. Surely the finer details are important to getting onboarded, so... TODO: Improve this README and the general documentation before expecting anyone else to enjoy working on this!
