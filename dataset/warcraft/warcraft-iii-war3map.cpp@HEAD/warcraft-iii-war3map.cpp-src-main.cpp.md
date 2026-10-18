---
type: vendor-doc
source_repo: "https://github.com/warcraft-iii/war3map.cpp"
source_path: "src/main.cpp"
ref: HEAD
---

#include "JassAPI.h"

namespace jass = war3mapcpp::api;

int WAR3MAP_FUNC Unload()
{
    jass::UnInstallCodeCallback();
    return 0;
}

int WAR3MAP_FUNC main()
{
    jass::DisplayTextToPlayer(jass::GetLocalPlayer(), 0.f, 0.f, _jstr("Hello World"));

    auto trig = jass::CreateTrigger();

    auto action = [=] { //
        jass::DisplayTextToPlayer(jass::GetLocalPlayer(), 0.f, 0.f, _jstr("PRESS ESC"));
    };
    jass::TriggerAddAction(trig, action);
    jass::TriggerRegisterPlayerEvent(trig, jass::GetLocalPlayer(), jass::ConvertPlayerEvent(17)); // ESC key

    return 0;
}
