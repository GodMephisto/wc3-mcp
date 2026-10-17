---
type: vendor-doc
source_repo: "https://github.com/Vlod-github/Rotating-model"
source_path: "README.md"
ref: HEAD
---

# Rotating model
This vjass library allows you to create and manage rotating models.
## Installation
1. Copy trigger **RotatingModel**.
2. Copy custom Effect unit. If the unit rawcode has changed ('h000'), then change the **static integer unitid** in RotatingModels to the new one.
3. Import yourself a model from a map in the Import Manager. Check that the Effect unit has an imported model installed in the Model file column.
## Description
1. Create a sample
```scala
local RotatingModel obj = RotatingModel.create()
```
2. Change default parameters before start
```scala
set obj.z = real // height relative to terrain
set obj.scale = real // model scale
set obj.radius = real // radius from center
set obj.count = integer // number of models
set obj.red = integer // red color (0-255)
set obj.green = integer // green color
set obj.blue = integer // blue color
set obj.alpha = integer  // model transparency (0-255)
set obj.pcolor= playercolor // for models with timcolor
set obj.model = string // path to the model
set obj.dangle = real // rate of change of angle (radian)
set obj.timeout = real // animation update time
```
3. Run rotating model
```scala
call obj.start(x, y) // real x, real y - coordinates
call obj.startUnit(target) // unit target - attach to unit
```
4. Pause animation
```scala
call obj.pause()
```
5. Resume animation
```scala
call obj.resume()
```
6. Change parameters of rotating models during animation
```scala
set obj.x = real // center on X
set obj.y = real // center on Y
set obj.target = unit // attached unit
call obj.reCount(count) // integer count - change count 
call obj.reScale(scale) // real scale - change scale
call obj.reColor(red, green, blue, alpha) // integer - change colors
call obj.rePlayerColor(pcolor) // playercolor pcolor - timcolor
```
7. Accessing rotating model data
```scala
set integer = obj.count
set unit = obj.unit[1] // first element
set unit = obj.unit[obj.count] // last element
```
8. Stop and delete a rotating model
```scala
call obj.destroy()
```
## Example 1
```scala
local RotatingModel obj = RotatingModel.create()
set obj.scale = 0.5
call obj.start(0,0)
```
https://user-images.githubusercontent.com/103655830/165262553-cc726174-03df-4354-8dca-f89e09b89dd9.mp4
## Example 2
```scala
local RotatingModel obj = RotatingModel.create()
set obj.count = 2
set obj.model = "Abilities\\Weapons\\SentinelMissile\\SentinelMissile.mdl"
call obj.startUnit(unit)
```
https://user-images.githubusercontent.com/103655830/165283732-303d0fe3-e7c4-4cca-af92-97356beaae71.mp4

