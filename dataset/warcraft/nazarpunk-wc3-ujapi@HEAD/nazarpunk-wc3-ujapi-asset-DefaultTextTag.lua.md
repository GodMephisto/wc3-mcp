---
type: vendor-doc
source_repo: "https://github.com/nazarpunk/wc3-ujapi"
source_path: "asset/DefaultTextTag.lua"
ref: HEAD
---

---@class DefaultTextTag
DefaultTextTag = {}
DefaultTextTag.__index = DefaultTextTag;

---@return DefaultTextTag
function DefaultTextTag:new()
    return setmetatable({}, DefaultTextTag);
end

---@param x number
---@param y number
---@return DefaultTextTag
function DefaultTextTag:position(x, y)
    self.x = x
    self.y = y
    return self;
end

---@param w widget
---@return DefaultTextTag
function DefaultTextTag:widget(w)
    self.x = GetWidgetX(w);
    self.y = GetWidgetY(w);
    return self;
end

---@param color number[] @[red, green, blue, alpha]
---@param setting number[] @[xvel, yvel, fadepoint, lifespan, offsetX]
---@param text string
---@return DefaultTextTag
function DefaultTextTag:create(color, setting, text)
    local t = CreateTextTag();
    self.texttag = t
    local x, y = self.x, self.y;

    SetTextTagText(t, text, .024);
    SetTextTagPos(t, x + setting[5], y, 0);
    SetTextTagColor(t, color[1], color[2], color[3], color[4]);
    SetTextTagVelocity(t, setting[1], setting[2]);
    SetTextTagFadepoint(t, setting[3]);
    SetTextTagLifespan(t, setting[4]);
    SetTextTagVisibility(t, IsVisibleToPlayer(x, y, GetLocalPlayer()));
    SetTextTagPermanent(t, false);
    return self;
end

---@param text string
function DefaultTextTag:goldBounty(text)
    return self:create({ 255, 220, 0, 255 }, { 0, .03, 2, 3, -16 }, text);
end

---@param text string
function DefaultTextTag:lumberBounty(text)
    return self:create({ 0, 200, 80, 255 }, { 0, .03, 2, 3, -16 }, text);
end

---@param text string
function DefaultTextTag:miss(text)
    return self:create({ 255, 0, 0, 255 }, { 0, .03, 1, 3, 0 }, text);
end

---@param text string
function DefaultTextTag:criticalStrike(text)
    return self:create({ 255, 0, 0, 255 }, { 0, .04, 2, 5, 0 }, text);
end

---@param text string
function DefaultTextTag:manaBurn(text)
    return self:create({ 82, 82, 255, 255 }, { 0, .04, 2, 5, 0 }, text);
end

---@param text string
function DefaultTextTag:shadowStrike(text)
    return self:create({ 160, 255, 0, 255 }, { 0, .04, 2, 5, 0 }, text);
end