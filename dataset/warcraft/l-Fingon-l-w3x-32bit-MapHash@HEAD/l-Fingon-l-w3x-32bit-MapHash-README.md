---
type: vendor-doc
source_repo: "https://github.com/l-Fingon-l/w3x-32bit-MapHash"
source_path: "README.md"
ref: HEAD
---

# .w3x map-hash
An implementation of 32-bit .w3x (warcraft 3 map archive) hash calculation algorithm.  

This hash is usually used for map comparison; it is also baked into the replay file by the game engine in order to compare the map used in replay and an actual map the game client tries to run the replay on.

Another area of extensive usage for this hash is the LAN-game creation where the host has to verify that all of the players got the same map version. This is used in [Flo hosting system](https://github.com/w3champions/flo), for instance. That, in turn, is a backbone of [W3Champion's](https://w3champions.com) server architecture.

## Source
The algorithm was painstakingly constructed following the cryptanalysis of hash function's behaviour, no binaries were disassembled.

A .w3x map archive as an input. A 32-bit value as an output (found in replay file) 2 minutes later. Thorough comparison, another iteration. All over again.

The interface form was taken from a previously existing algorithm by [GHOST](https://github.com/Josko/aura-bot/blob/1e5df425fd325e9b0e6aa8fa5eed35f0c61f3114/src/map.cpp#L200) project, this is the reason the code might look quite alike. The algorithm had been heavily changed with the Warcraft III Reforged release, though.
