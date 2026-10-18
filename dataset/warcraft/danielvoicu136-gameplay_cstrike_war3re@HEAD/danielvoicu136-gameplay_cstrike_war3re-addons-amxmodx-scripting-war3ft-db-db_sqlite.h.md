---
type: vendor-doc
source_repo: "https://github.com/danielvoicu136/gameplay_cstrike_war3re"
source_path: "addons/amxmodx/scripting/war3ft/db/db_sqlite.h"
ref: HEAD
---

/*
*	SQLite Header File
*/

forward SQLITE_Init();
forward SQLITE_Upgrade();
forward SQLITE_Close();
forward SQLITE_Save( id );
forward SQLITE_Save_T( id );
forward SQLITE_GetAllXP( id );
forward SQLITE_SetDataForRace( id );
forward bool:SQLITE_Connection_Available();
forward SQLITE_Prune();
forward SQLITE_UpdateTimestamp( id );
forward SQLITE_FetchUniqueID( id );
forward SQLITE_Error( Handle:query, szQuery[], id );
