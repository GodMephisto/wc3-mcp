---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/lib/popups.lua"
ref: HEAD
---

POPUP_SYMBOL_PRE_PLUS = 0
POPUP_SYMBOL_PRE_MINUS = 1
POPUP_SYMBOL_PRE_SADFACE = 2
POPUP_SYMBOL_PRE_BROKENARROW = 3
POPUP_SYMBOL_PRE_SHADES = 4
POPUP_SYMBOL_PRE_MISS = 5
POPUP_SYMBOL_PRE_EVADE = 6
POPUP_SYMBOL_PRE_DENY = 7
POPUP_SYMBOL_PRE_ARROW = 8

POPUP_SYMBOL_POST_EXCLAMATION = 0
POPUP_SYMBOL_POST_POINTZERO = 1
POPUP_SYMBOL_POST_MEDAL = 2
POPUP_SYMBOL_POST_DROP = 3
POPUP_SYMBOL_POST_LIGHTNING = 4
POPUP_SYMBOL_POST_SKULL = 5
POPUP_SYMBOL_POST_EYE = 6
POPUP_SYMBOL_POST_SHIELD = 7
POPUP_SYMBOL_POST_POINTFIVE = 8

if Popup == nil then
	print ( '[Popup] creating Popup' )
	Popup = {}
	setmetatable(Popup, {
		__call = function(t, ...)
			return t:Numbers(...)
		end
	})
	--Popup.__index = Popup
end

function Popup:Healing(target, amount, player)
	Popup:Numbers(target, "particles/msg_fx/msg_heal.vpcf", Vector(0, 255, 0),
		1.0, amount, POPUP_SYMBOL_PRE_PLUS, nil, player)
end

function Popup:Damage(target, amount, player)
	Popup:Numbers(target, "particles/msg_fx/msg_damage.vpcf", Vector(255, 0, 0),
		1.0, amount, nil, POPUP_SYMBOL_POST_DROP, player)
end

function Popup:AddGold(target, amount, player)
	Popup:Numbers(target, "particles/ability/msg_crit.vpcf", Vector(255, 200, 33),
		2.0, amount, POPUP_SYMBOL_PRE_PLUS, nil, player)
end

function Popup:DamageBig(target, amount, player)
	Popup:Numbers(target, "particles/ability/msg_crit.vpcf", Vector(255, 0, 0),
		2.0, amount, nil, POPUP_SYMBOL_POST_DROP, player)
end

function Popup:DamageColored(target, amount, color, player)
	Popup:Numbers(target, "particles/msg_fx/msg_damage.vpcf", color,
		1.0, amount, nil, POPUP_SYMBOL_POST_DROP, player)
end

function Popup:CriticalDamage(target, amount, player)
	Popup:Numbers(target, "particles/msg_fx/msg_crit.vpcf", Vector(255, 0, 0),
		1.0, amount, nil, POPUP_SYMBOL_POST_LIGHTNING, player)
end

function Popup:CriticalDamageColored(target, amount, color, player) 
	Popup:Numbers(target, "particles/msg_fx/msg_crit.vpcf", color,
		1.0, amount, nil, POPUP_SYMBOL_POST_LIGHTNING, player)
end

function Popup:DamageOverTime(target, amount, player)
	Popup:Numbers(target, "particles/msg_fx/msg_poison.vpcf", Vector(215, 50, 248),
		1.0, amount, nil, POPUP_SYMBOL_POST_EYE, player)
end

function Popup:DamageBlock(target, amount, player)
	Popup:Numbers(target, "particles/msg_fx/msg_block.vpcf", Vector(255, 255, 255),
		1.0, amount, POPUP_SYMBOL_PRE_MINUS, nil, player)
end

function Popup:GoldGain(target, amount, player)
	Popup:Numbers(target, "particles/msg_fx/msg_gold.vpcf", Vector(255, 200, 33),
		1.0, amount, POPUP_SYMBOL_PRE_PLUS, nil, player)
end

function Popup:ManaGain(target, amount, player) 
	Popup:Numbers(target, "particles/msg_fx/msg_gold.vpcf", Vector(33, 200, 255),
		1.0, amount, POPUP_SYMBOL_PRE_PLUS, nil, player)
end

function Popup:Miss(target, player)
	Popup:Numbers(target, "particles/msg_fx/msg_miss.vpcf", Vector(255, 0, 0),
		1.0, nil, POPUP_SYMBOL_PRE_MISS, nil, player)
end

function Popup:Numbers(target, pfx, color, lifetime, number, presymbol, postsymbol, player)
	if number == 0 then
		return 
	end
	local pidx = nil
	if player then
		pidx = ParticleManager:CreateParticleForPlayer(pfx, PATTACH_OVERHEAD_FOLLOW, target, player)
		-- pidx = ParticleManager:CreateParticle(pfx, PATTACH_OVERHEAD_FOLLOW, target)
	else
		pidx = ParticleManager:CreateParticle(pfx, PATTACH_OVERHEAD_FOLLOW, target)
	end

	local digits = 0
	if number ~= nil then
		digits = #tostring(math.floor(number))
	end
	if presymbol ~= nil then
		digits = digits + 1
	end
	if postsymbol ~= nil then
		digits = digits + 1
	end

	ParticleManager:SetParticleControl(pidx, 1, Vector(tonumber(presymbol), tonumber(number), tonumber(postsymbol)))
	ParticleManager:SetParticleControl(pidx, 2, Vector(lifetime, digits, 0))
	ParticleManager:SetParticleControl(pidx, 3, color)
	
	ParticleManager:ReleaseParticleIndex(pidx)
end

GameRules.Popup = Popup