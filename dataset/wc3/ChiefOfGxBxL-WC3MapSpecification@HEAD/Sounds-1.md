---
type: vendor-doc
source_repo: https://github.com/ChiefOfGxBxL/WC3MapSpecification
source_path: Sounds/1.md
ref: HEAD
---

# Sounds/1.md

File structure:

* `int` File version = 0x01 (1)

* `int` Number of sounds

  * `string` Sound name

  * `string` Sound path

  * `string` Sound EAX

  * `int` Flags (bitstring)

    | 8 | 4 | 2 | 1 |

    |:-:|:-:|:-:|:-:|

    | isMusic | stopWhenOutOfRange | is3dSound | isLooping |

  * `int` Fade rate in

  * `int` Fade rate out

  * `int` Volume

  * `float` Pitch

  * `unknown(4)`

  * `unknown(4)`

  * `int` Channel

  * `float` Min distance

  * `float` Max distance

  * `float` Cutoff distance

  * `unknown(4)`

  * `unknown(4)`

  * `unknown(4)`

  * `unknown(4)`

  * `unknown(4)`

  * `unknown(4)`