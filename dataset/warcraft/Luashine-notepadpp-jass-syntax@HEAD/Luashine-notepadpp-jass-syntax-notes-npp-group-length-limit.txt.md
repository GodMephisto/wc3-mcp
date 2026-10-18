---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "notes/npp-group-length-limit.txt"
ref: HEAD
---

30712 + 7 is the absolute maximum
together with \0 probably #define max 30720 in the library code.
usable: 30719

See:
https://github.com/Luashine/wc3-test-maps/tree/bcf38147d7c7714d45500d89da71799732a3a263#generate-string-to-test-max-length


### Generate string to test max length

Lua code. **Specialty:** Generates a string that tracks its own length for max length checks.
Really useful to see the cut-off length.

```lua
local s = ""
local maxLength = 5000

while #s < maxLength do
	s = s .. (#s .. ".")
end
print(s)
```

Example output: `0.2.4.6.8.10.13.`

This must be read as "13." means that the dot character following "10" was the 13th character in the string. Thus we only need to sum up 13 + the three characters of the last number you can see: 13+3 = 16 characters total generated and visible.

Graphic:

```
Visible string:
0.2.4.6.8.10.13.
 ^ ^ ^ ^ ^  ^  ^
 2 | 6 | 10 |  16 (you can see 3 more characters after the last number)
   4   8    13 (last number points to this)
```
