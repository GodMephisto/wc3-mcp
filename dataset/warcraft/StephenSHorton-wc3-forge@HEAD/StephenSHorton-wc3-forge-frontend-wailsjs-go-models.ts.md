---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/wailsjs/go/models.ts"
ref: HEAD
---

export namespace doodadsdoo {
	
	export class ItemDrop {
	    ItemID: string;
	    Chance: number;
	
	    static createFrom(source: any = {}) {
	        return new ItemDrop(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.ItemID = source["ItemID"];
	        this.Chance = source["Chance"];
	    }
	}
	export class Doodad {
	    TypeID: string;
	    Variation: number;
	    Position: number[];
	    Rotation: number;
	    Scale: number[];
	    SkinID: string;
	    Flags: number;
	    Life: number;
	    MapItemTable: number;
	    ItemDrops: ItemDrop[];
	    CreationNumber: number;
	
	    static createFrom(source: any = {}) {
	        return new Doodad(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.TypeID = source["TypeID"];
	        this.Variation = source["Variation"];
	        this.Position = source["Position"];
	        this.Rotation = source["Rotation"];
	        this.Scale = source["Scale"];
	        this.SkinID = source["SkinID"];
	        this.Flags = source["Flags"];
	        this.Life = source["Life"];
	        this.MapItemTable = source["MapItemTable"];
	        this.ItemDrops = this.convertValues(source["ItemDrops"], ItemDrop);
	        this.CreationNumber = source["CreationNumber"];
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}

}

export namespace forge {
	
	export class Diagnostic {
	    lua_line: number;
	    reason: string;
	    source?: string;
	
	    static createFrom(source: any = {}) {
	        return new Diagnostic(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.lua_line = source["lua_line"];
	        this.reason = source["reason"];
	        this.source = source["source"];
	    }
	}
	export class HistoryEntry {
	    index: number;
	    label: string;
	    active: boolean;
	
	    static createFrom(source: any = {}) {
	        return new HistoryEntry(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.index = source["index"];
	        this.label = source["label"];
	        this.active = source["active"];
	    }
	}
	export class HistoryState {
	    undo: HistoryEntry[];
	    redo: HistoryEntry[];
	    open_group_depth: number;
	
	    static createFrom(source: any = {}) {
	        return new HistoryState(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.undo = this.convertValues(source["undo"], HistoryEntry);
	        this.redo = this.convertValues(source["redo"], HistoryEntry);
	        this.open_group_depth = source["open_group_depth"];
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class ImportEntry {
	    path: string;
	    flag: number;
	    size: number;
	    present: boolean;
	
	    static createFrom(source: any = {}) {
	        return new ImportEntry(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.path = source["path"];
	        this.flag = source["flag"];
	        this.size = source["size"];
	        this.present = source["present"];
	    }
	}
	export class RegionInfo {
	    name: string;
	    creation_number: number;
	    min_x: number;
	    min_y: number;
	    max_x: number;
	    max_y: number;
	    weather_id: string;
	    ambient_id: string;
	    color: number[];
	    gg_ref: string;
	
	    static createFrom(source: any = {}) {
	        return new RegionInfo(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.name = source["name"];
	        this.creation_number = source["creation_number"];
	        this.min_x = source["min_x"];
	        this.min_y = source["min_y"];
	        this.max_x = source["max_x"];
	        this.max_y = source["max_y"];
	        this.weather_id = source["weather_id"];
	        this.ambient_id = source["ambient_id"];
	        this.color = source["color"];
	        this.gg_ref = source["gg_ref"];
	    }
	}
	export class StartLocationInfo {
	    index: number;
	    creation_number: number;
	    position: number[];
	    rotation: number;
	
	    static createFrom(source: any = {}) {
	        return new StartLocationInfo(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.index = source["index"];
	        this.creation_number = source["creation_number"];
	        this.position = source["position"];
	        this.rotation = source["rotation"];
	    }
	}
	export class TerrainTileInfo {
	    col: number;
	    row: number;
	    ground_tile_id: string;
	    height: number;
	
	    static createFrom(source: any = {}) {
	        return new TerrainTileInfo(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.col = source["col"];
	        this.row = source["row"];
	        this.ground_tile_id = source["ground_tile_id"];
	        this.height = source["height"];
	    }
	}
	export class UnitInstanceItemDrop {
	    item_id: string;
	    chance: number;
	
	    static createFrom(source: any = {}) {
	        return new UnitInstanceItemDrop(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.item_id = source["item_id"];
	        this.chance = source["chance"];
	    }
	}

}

export namespace main {
	
	export class BridgeInfo {
	    pid: number;
	    port: number;
	    token_short: string;
	    map_name: string;
	    map_path: string;
	
	    static createFrom(source: any = {}) {
	        return new BridgeInfo(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.pid = source["pid"];
	        this.port = source["port"];
	        this.token_short = source["token_short"];
	        this.map_name = source["map_name"];
	        this.map_path = source["map_path"];
	    }
	}
	export class CommandButtonEntry {
	    path: string;
	    name: string;
	
	    static createFrom(source: any = {}) {
	        return new CommandButtonEntry(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.path = source["path"];
	        this.name = source["name"];
	    }
	}
	export class ConvertBlockerDTO {
	    trigger_id: number;
	    trigger_name: string;
	    kind: string;
	    reason: string;
	
	    static createFrom(source: any = {}) {
	        return new ConvertBlockerDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.trigger_id = source["trigger_id"];
	        this.trigger_name = source["trigger_name"];
	        this.kind = source["kind"];
	        this.reason = source["reason"];
	    }
	}
	export class UnitObjectField {
	    id: string;
	    field: string;
	    display_name: string;
	    category: string;
	    type: string;
	    value: string;
	    display: string;
	    display_raw: string;
	    overridden: boolean;
	    levels?: Record<number, string>;
	
	    static createFrom(source: any = {}) {
	        return new UnitObjectField(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.id = source["id"];
	        this.field = source["field"];
	        this.display_name = source["display_name"];
	        this.category = source["category"];
	        this.type = source["type"];
	        this.value = source["value"];
	        this.display = source["display"];
	        this.display_raw = source["display_raw"];
	        this.overridden = source["overridden"];
	        this.levels = source["levels"];
	    }
	}
	export class UnitObjectDetail {
	    id: string;
	    name: string;
	    base_id?: string;
	    is_custom: boolean;
	    is_edited: boolean;
	    race: string;
	    kind: string;
	    icon_art: string;
	    model_path: string;
	    model_fallbacks: string[];
	    fields: UnitObjectField[];
	
	    static createFrom(source: any = {}) {
	        return new UnitObjectDetail(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.id = source["id"];
	        this.name = source["name"];
	        this.base_id = source["base_id"];
	        this.is_custom = source["is_custom"];
	        this.is_edited = source["is_edited"];
	        this.race = source["race"];
	        this.kind = source["kind"];
	        this.icon_art = source["icon_art"];
	        this.model_path = source["model_path"];
	        this.model_fallbacks = source["model_fallbacks"];
	        this.fields = this.convertValues(source["fields"], UnitObjectField);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class ConvertObjectResult {
	    id: string;
	    detail?: UnitObjectDetail;
	
	    static createFrom(source: any = {}) {
	        return new ConvertObjectResult(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.id = source["id"];
	        this.detail = this.convertValues(source["detail"], UnitObjectDetail);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class ConvertToLuaResultDTO {
	    blockers: ConvertBlockerDTO[];
	
	    static createFrom(source: any = {}) {
	        return new ConvertToLuaResultDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.blockers = this.convertValues(source["blockers"], ConvertBlockerDTO);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class CreateCustomObjectResult {
	    id: string;
	    detail?: UnitObjectDetail;
	
	    static createFrom(source: any = {}) {
	        return new CreateCustomObjectResult(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.id = source["id"];
	        this.detail = this.convertValues(source["detail"], UnitObjectDetail);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class CreateCustomUnitResult {
	    id: string;
	    detail?: UnitObjectDetail;
	
	    static createFrom(source: any = {}) {
	        return new CreateCustomUnitResult(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.id = source["id"];
	        this.detail = this.convertValues(source["detail"], UnitObjectDetail);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class DoodadDTO {
	    creation_number: number;
	    type_id: string;
	    skin_id: string;
	    position: number[];
	    rotation: number;
	    scale: number[];
	    variation: number;
	    life: number;
	    flags: number;
	
	    static createFrom(source: any = {}) {
	        return new DoodadDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.creation_number = source["creation_number"];
	        this.type_id = source["type_id"];
	        this.skin_id = source["skin_id"];
	        this.position = source["position"];
	        this.rotation = source["rotation"];
	        this.scale = source["scale"];
	        this.variation = source["variation"];
	        this.life = source["life"];
	        this.flags = source["flags"];
	    }
	}
	export class GameplayConstantRow {
	    section: string;
	    key: string;
	    value: string;
	
	    static createFrom(source: any = {}) {
	        return new GameplayConstantRow(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.section = source["section"];
	        this.key = source["key"];
	        this.value = source["value"];
	    }
	}
	export class ImportModelOptions {
	    scale: number;
	    upAxis: string;
	    flipV: boolean;
	
	    static createFrom(source: any = {}) {
	        return new ImportModelOptions(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.scale = source["scale"];
	        this.upAxis = source["upAxis"];
	        this.flipV = source["flipV"];
	    }
	}
	export class ImportModelResult {
	    modelPath: string;
	    texturePaths: string[];
	    warnings: string[];
	
	    static createFrom(source: any = {}) {
	        return new ImportModelResult(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.modelPath = source["modelPath"];
	        this.texturePaths = source["texturePaths"];
	        this.warnings = source["warnings"];
	    }
	}
	export class LabeledOption {
	    key: string;
	    label: string;
	
	    static createFrom(source: any = {}) {
	        return new LabeledOption(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.key = source["key"];
	        this.label = source["label"];
	    }
	}
	export class LoadingScreenOption {
	    index: number;
	    label: string;
	
	    static createFrom(source: any = {}) {
	        return new LoadingScreenOption(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.index = source["index"];
	        this.label = source["label"];
	    }
	}
	export class MapStatus {
	    loaded: boolean;
	    path?: string;
	    name?: string;
	    unit_count: number;
	    lua: boolean;
	
	    static createFrom(source: any = {}) {
	        return new MapStatus(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.loaded = source["loaded"];
	        this.path = source["path"];
	        this.name = source["name"];
	        this.unit_count = source["unit_count"];
	        this.lua = source["lua"];
	    }
	}
	export class MinimapDTO {
	    bytes: string;
	    ext: string;
	    found: boolean;
	    generated: boolean;
	
	    static createFrom(source: any = {}) {
	        return new MinimapDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.bytes = source["bytes"];
	        this.ext = source["ext"];
	        this.found = source["found"];
	        this.generated = source["generated"];
	    }
	}
	export class NewMapParams {
	    name: string;
	    width: number;
	    height: number;
	    tileset: string;
	    path: string;
	
	    static createFrom(source: any = {}) {
	        return new NewMapParams(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.name = source["name"];
	        this.width = source["width"];
	        this.height = source["height"];
	        this.tileset = source["tileset"];
	        this.path = source["path"];
	    }
	}
	export class PathingMapDTO {
	    width: number;
	    height: number;
	    cells: number[];
	
	    static createFrom(source: any = {}) {
	        return new PathingMapDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.width = source["width"];
	        this.height = source["height"];
	        this.cells = source["cells"];
	    }
	}
	export class SectionOverride {
	    id: number;
	    lua: string;
	
	    static createFrom(source: any = {}) {
	        return new SectionOverride(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.id = source["id"];
	        this.lua = source["lua"];
	    }
	}
	export class SelectionItemDTO {
	    kind: string;
	    id: number;
	
	    static createFrom(source: any = {}) {
	        return new SelectionItemDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.kind = source["kind"];
	        this.id = source["id"];
	    }
	}
	export class SelectionDTO {
	    items: SelectionItemDTO[];
	    primary: number;
	
	    static createFrom(source: any = {}) {
	        return new SelectionDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.items = this.convertValues(source["items"], SelectionItemDTO);
	        this.primary = source["primary"];
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	
	export class SkyModelOption {
	    path: string;
	    name_key: string;
	    display_name: string;
	
	    static createFrom(source: any = {}) {
	        return new SkyModelOption(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.path = source["path"];
	        this.name_key = source["name_key"];
	        this.display_name = source["display_name"];
	    }
	}
	export class SwapTilesetRequest {
	    newLetter: string;
	    newGroundTilesets: string[];
	    newCliffTilesets: string[];
	    groundFromTo: number[];
	    cliffFromTo: number[];
	
	    static createFrom(source: any = {}) {
	        return new SwapTilesetRequest(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.newLetter = source["newLetter"];
	        this.newGroundTilesets = source["newGroundTilesets"];
	        this.newCliffTilesets = source["newCliffTilesets"];
	        this.groundFromTo = source["groundFromTo"];
	        this.cliffFromTo = source["cliffFromTo"];
	    }
	}
	export class WaterInfo {
	    ok: boolean;
	    offset: number;
	    shallow_min: number[];
	    shallow_max: number[];
	    deep_min: number[];
	    deep_max: number[];
	    texture_file: string;
	    num_textures: number;
	    texture_rate: number;
	
	    static createFrom(source: any = {}) {
	        return new WaterInfo(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.ok = source["ok"];
	        this.offset = source["offset"];
	        this.shallow_min = source["shallow_min"];
	        this.shallow_max = source["shallow_max"];
	        this.deep_min = source["deep_min"];
	        this.deep_max = source["deep_max"];
	        this.texture_file = source["texture_file"];
	        this.num_textures = source["num_textures"];
	        this.texture_rate = source["texture_rate"];
	    }
	}
	export class TerrainDTO {
	    width: number;
	    height: number;
	    center_offset: number[];
	    heights: number[];
	    ground_tex: number[];
	    tileset: string;
	    palette: string[];
	    palette_colors: number[][];
	    palette_textures: string[];
	    layer_height: number[];
	    cliff_tex: number[];
	    cliff_var: number[];
	    ground_var: number[];
	    ramp_flags: number[];
	    cliff_palette: string[];
	    cliff_palette_textures: string[];
	    water_z: number[];
	    has_water: number[];
	    water: WaterInfo;
	    shadow_map: number[];
	    shadow_map_width: number;
	    shadow_map_height: number;
	    cell_skip: number[];
	    sky_model: string;
	    sky_model_from_script: boolean;
	    cliff_displacement: number[];
	
	    static createFrom(source: any = {}) {
	        return new TerrainDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.width = source["width"];
	        this.height = source["height"];
	        this.center_offset = source["center_offset"];
	        this.heights = source["heights"];
	        this.ground_tex = source["ground_tex"];
	        this.tileset = source["tileset"];
	        this.palette = source["palette"];
	        this.palette_colors = source["palette_colors"];
	        this.palette_textures = source["palette_textures"];
	        this.layer_height = source["layer_height"];
	        this.cliff_tex = source["cliff_tex"];
	        this.cliff_var = source["cliff_var"];
	        this.ground_var = source["ground_var"];
	        this.ramp_flags = source["ramp_flags"];
	        this.cliff_palette = source["cliff_palette"];
	        this.cliff_palette_textures = source["cliff_palette_textures"];
	        this.water_z = source["water_z"];
	        this.has_water = source["has_water"];
	        this.water = this.convertValues(source["water"], WaterInfo);
	        this.shadow_map = source["shadow_map"];
	        this.shadow_map_width = source["shadow_map_width"];
	        this.shadow_map_height = source["shadow_map_height"];
	        this.cell_skip = source["cell_skip"];
	        this.sky_model = source["sky_model"];
	        this.sky_model_from_script = source["sky_model_from_script"];
	        this.cliff_displacement = source["cliff_displacement"];
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class TerrainPaletteThumbs {
	    ground: Record<string, string>;
	    cliff: Record<string, string>;
	
	    static createFrom(source: any = {}) {
	        return new TerrainPaletteThumbs(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.ground = source["ground"];
	        this.cliff = source["cliff"];
	    }
	}
	export class TileInfoDTO {
	    fourcc: string;
	    name: string;
	    texture: string;
	    color: number[];
	    thumb: string;
	
	    static createFrom(source: any = {}) {
	        return new TileInfoDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.fourcc = source["fourcc"];
	        this.name = source["name"];
	        this.texture = source["texture"];
	        this.color = source["color"];
	        this.thumb = source["thumb"];
	    }
	}
	export class TilesetInfoDTO {
	    letter: string;
	    name: string;
	    ground: TileInfoDTO[];
	    cliff: TileInfoDTO[];
	
	    static createFrom(source: any = {}) {
	        return new TilesetInfoDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.letter = source["letter"];
	        this.name = source["name"];
	        this.ground = this.convertValues(source["ground"], TileInfoDTO);
	        this.cliff = this.convertValues(source["cliff"], TileInfoDTO);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class TranspileSectionDTO {
	    id: number;
	    label: string;
	    kind: string;
	    original: string;
	    transpiled: string;
	    diagnostics?: forge.Diagnostic[];
	    errors?: string[];
	    preprocess_warnings?: string[];
	
	    static createFrom(source: any = {}) {
	        return new TranspileSectionDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.id = source["id"];
	        this.label = source["label"];
	        this.kind = source["kind"];
	        this.original = source["original"];
	        this.transpiled = source["transpiled"];
	        this.diagnostics = this.convertValues(source["diagnostics"], forge.Diagnostic);
	        this.errors = source["errors"];
	        this.preprocess_warnings = source["preprocess_warnings"];
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class TranspilePreviewDTO {
	    sections: TranspileSectionDTO[];
	
	    static createFrom(source: any = {}) {
	        return new TranspilePreviewDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.sections = this.convertValues(source["sections"], TranspileSectionDTO);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	
	export class TriggerCameraInfoDTO {
	    name: string;
	    target_x: number;
	    target_y: number;
	    distance: number;
	    gg_ref: string;
	
	    static createFrom(source: any = {}) {
	        return new TriggerCameraInfoDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.name = source["name"];
	        this.target_x = source["target_x"];
	        this.target_y = source["target_y"];
	        this.distance = source["distance"];
	        this.gg_ref = source["gg_ref"];
	    }
	}
	export class TriggerDestructableInstanceDTO {
	    creation_number: number;
	    type_id: string;
	    x: number;
	    y: number;
	    name: string;
	    gg_ref: string;
	
	    static createFrom(source: any = {}) {
	        return new TriggerDestructableInstanceDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.creation_number = source["creation_number"];
	        this.type_id = source["type_id"];
	        this.x = source["x"];
	        this.y = source["y"];
	        this.name = source["name"];
	        this.gg_ref = source["gg_ref"];
	    }
	}
	export class TriggerDetailDTO {
	    kind: string;
	    category?: wtg.Category;
	    trigger?: wtg.Trigger;
	    variable?: wtg.Variable;
	
	    static createFrom(source: any = {}) {
	        return new TriggerDetailDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.kind = source["kind"];
	        this.category = this.convertValues(source["category"], wtg.Category);
	        this.trigger = this.convertValues(source["trigger"], wtg.Trigger);
	        this.variable = this.convertValues(source["variable"], wtg.Variable);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class TriggerFunctionMetaDTO {
	    name: string;
	    section: string;
	    argc: number;
	    arg_types: string[];
	    return_type?: string;
	    display_name?: string;
	    parameters_template?: string[];
	    defaults?: string[];
	    limits?: string[];
	    category?: string;
	    script_name?: string;
	    hint?: string;
	
	    static createFrom(source: any = {}) {
	        return new TriggerFunctionMetaDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.name = source["name"];
	        this.section = source["section"];
	        this.argc = source["argc"];
	        this.arg_types = source["arg_types"];
	        this.return_type = source["return_type"];
	        this.display_name = source["display_name"];
	        this.parameters_template = source["parameters_template"];
	        this.defaults = source["defaults"];
	        this.limits = source["limits"];
	        this.category = source["category"];
	        this.script_name = source["script_name"];
	        this.hint = source["hint"];
	    }
	}
	export class TriggerTypeMetaDTO {
	    name: string;
	    base_type?: string;
	    display_name?: string;
	    can_be_global?: boolean;
	    can_compare?: boolean;
	
	    static createFrom(source: any = {}) {
	        return new TriggerTypeMetaDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.name = source["name"];
	        this.base_type = source["base_type"];
	        this.display_name = source["display_name"];
	        this.can_be_global = source["can_be_global"];
	        this.can_compare = source["can_compare"];
	    }
	}
	export class TriggerPresetMetaDTO {
	    name: string;
	    type: string;
	    value: string;
	    display_name: string;
	
	    static createFrom(source: any = {}) {
	        return new TriggerPresetMetaDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.name = source["name"];
	        this.type = source["type"];
	        this.value = source["value"];
	        this.display_name = source["display_name"];
	    }
	}
	export class TriggerFunctionsMetaDTO {
	    functions: TriggerFunctionMetaDTO[];
	    categories?: Record<string, string>;
	    types?: Record<string, string>;
	    presets?: TriggerPresetMetaDTO[];
	    type_meta?: TriggerTypeMetaDTO[];
	
	    static createFrom(source: any = {}) {
	        return new TriggerFunctionsMetaDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.functions = this.convertValues(source["functions"], TriggerFunctionMetaDTO);
	        this.categories = source["categories"];
	        this.types = source["types"];
	        this.presets = this.convertValues(source["presets"], TriggerPresetMetaDTO);
	        this.type_meta = this.convertValues(source["type_meta"], TriggerTypeMetaDTO);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class TriggerTreeNodeDTO {
	    id: number;
	    parent_id: number;
	    kind: string;
	    name: string;
	    description?: string;
	    is_comment?: boolean;
	    is_enabled?: boolean;
	    is_script?: boolean;
	    initially_on?: boolean;
	    run_on_initialization?: boolean;
	    open_state?: boolean;
	
	    static createFrom(source: any = {}) {
	        return new TriggerTreeNodeDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.id = source["id"];
	        this.parent_id = source["parent_id"];
	        this.kind = source["kind"];
	        this.name = source["name"];
	        this.description = source["description"];
	        this.is_comment = source["is_comment"];
	        this.is_enabled = source["is_enabled"];
	        this.is_script = source["is_script"];
	        this.initially_on = source["initially_on"];
	        this.run_on_initialization = source["run_on_initialization"];
	        this.open_state = source["open_state"];
	    }
	}
	export class TriggerTreeDTO {
	    nodes: TriggerTreeNodeDTO[];
	    is_pre_131?: boolean;
	    has_global_jass?: boolean;
	
	    static createFrom(source: any = {}) {
	        return new TriggerTreeDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.nodes = this.convertValues(source["nodes"], TriggerTreeNodeDTO);
	        this.is_pre_131 = source["is_pre_131"];
	        this.has_global_jass = source["has_global_jass"];
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class TriggerMutationResultDTO {
	    tree: TriggerTreeDTO;
	    new_id?: number;
	    detail?: TriggerDetailDTO;
	
	    static createFrom(source: any = {}) {
	        return new TriggerMutationResultDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.tree = this.convertValues(source["tree"], TriggerTreeDTO);
	        this.new_id = source["new_id"];
	        this.detail = this.convertValues(source["detail"], TriggerDetailDTO);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	
	export class TriggerRegionInfoDTO {
	    name: string;
	    creation_number: number;
	    left: number;
	    right: number;
	    top: number;
	    bottom: number;
	    gg_ref: string;
	
	    static createFrom(source: any = {}) {
	        return new TriggerRegionInfoDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.name = source["name"];
	        this.creation_number = source["creation_number"];
	        this.left = source["left"];
	        this.right = source["right"];
	        this.top = source["top"];
	        this.bottom = source["bottom"];
	        this.gg_ref = source["gg_ref"];
	    }
	}
	export class TriggerScriptResultDTO {
	    text?: string;
	    bytes?: number;
	
	    static createFrom(source: any = {}) {
	        return new TriggerScriptResultDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.text = source["text"];
	        this.bytes = source["bytes"];
	    }
	}
	export class TriggerSearchHitDTO {
	    trigger_id: number;
	    trigger_name: string;
	    kind: string;
	    path?: number[];
	    eca_name?: string;
	    snippet: string;
	    category?: string;
	
	    static createFrom(source: any = {}) {
	        return new TriggerSearchHitDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.trigger_id = source["trigger_id"];
	        this.trigger_name = source["trigger_name"];
	        this.kind = source["kind"];
	        this.path = source["path"];
	        this.eca_name = source["eca_name"];
	        this.snippet = source["snippet"];
	        this.category = source["category"];
	    }
	}
	
	
	
	export class TriggerUnitInstanceDTO {
	    creation_number: number;
	    type_id: string;
	    player: number;
	    x: number;
	    y: number;
	    name: string;
	    gg_ref: string;
	
	    static createFrom(source: any = {}) {
	        return new TriggerUnitInstanceDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.creation_number = source["creation_number"];
	        this.type_id = source["type_id"];
	        this.player = source["player"];
	        this.x = source["x"];
	        this.y = source["y"];
	        this.name = source["name"];
	        this.gg_ref = source["gg_ref"];
	    }
	}
	export class UnitDTO {
	    creation_number: number;
	    type_id: string;
	    skin_id: string;
	    player: number;
	    position: number[];
	    rotation: number;
	    scale: number[];
	    hit_points_pct: number;
	    mana_pct: number;
	    hero_level: number;
	    gold_amount: number;
	
	    static createFrom(source: any = {}) {
	        return new UnitDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.creation_number = source["creation_number"];
	        this.type_id = source["type_id"];
	        this.skin_id = source["skin_id"];
	        this.player = source["player"];
	        this.position = source["position"];
	        this.rotation = source["rotation"];
	        this.scale = source["scale"];
	        this.hit_points_pct = source["hit_points_pct"];
	        this.mana_pct = source["mana_pct"];
	        this.hero_level = source["hero_level"];
	        this.gold_amount = source["gold_amount"];
	    }
	}
	
	
	export class UnitObjectListEntity {
	    id: string;
	    name: string;
	    race: string;
	    race_label: string;
	    kind: string;
	    category: string;
	    is_custom: boolean;
	    is_edited: boolean;
	    base_id?: string;
	    campaign: boolean;
	    icon_art: string;
	
	    static createFrom(source: any = {}) {
	        return new UnitObjectListEntity(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.id = source["id"];
	        this.name = source["name"];
	        this.race = source["race"];
	        this.race_label = source["race_label"];
	        this.kind = source["kind"];
	        this.category = source["category"];
	        this.is_custom = source["is_custom"];
	        this.is_edited = source["is_edited"];
	        this.base_id = source["base_id"];
	        this.campaign = source["campaign"];
	        this.icon_art = source["icon_art"];
	    }
	}
	export class WC3InstallStatusDTO {
	    available: boolean;
	    path: string;
	
	    static createFrom(source: any = {}) {
	        return new WC3InstallStatusDTO(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.available = source["available"];
	        this.path = source["path"];
	    }
	}

}

export namespace unitsdoo {
	
	export class AbilityMod {
	    AbilityID: string;
	    Autocast: boolean;
	    Level: number;
	
	    static createFrom(source: any = {}) {
	        return new AbilityMod(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.AbilityID = source["AbilityID"];
	        this.Autocast = source["Autocast"];
	        this.Level = source["Level"];
	    }
	}
	export class InventorySlot {
	    Slot: number;
	    ItemID: string;
	
	    static createFrom(source: any = {}) {
	        return new InventorySlot(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.Slot = source["Slot"];
	        this.ItemID = source["ItemID"];
	    }
	}
	export class ItemDrop {
	    ItemID: string;
	    Chance: number;
	
	    static createFrom(source: any = {}) {
	        return new ItemDrop(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.ItemID = source["ItemID"];
	        this.Chance = source["Chance"];
	    }
	}
	export class Entity {
	    TypeID: string;
	    Variation: number;
	    Position: number[];
	    Rotation: number;
	    Scale: number[];
	    SkinID: string;
	    Flags: number;
	    Player: number;
	    UnknownByte1: number;
	    UnknownByte2: number;
	    HitPointsPct: number;
	    ManaPct: number;
	    MapItemTable: number;
	    ItemDrops: ItemDrop[];
	    GoldAmount: number;
	    TargetAcquisition: number;
	    HeroLevel: number;
	    HeroStr: number;
	    HeroAgi: number;
	    HeroInt: number;
	    Inventory: InventorySlot[];
	    AbilityModifications: AbilityMod[];
	    RandomType: number;
	    RandomData: number[];
	    CustomColor: number;
	    WaygateRegion: number;
	    CreationNumber: number;
	
	    static createFrom(source: any = {}) {
	        return new Entity(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.TypeID = source["TypeID"];
	        this.Variation = source["Variation"];
	        this.Position = source["Position"];
	        this.Rotation = source["Rotation"];
	        this.Scale = source["Scale"];
	        this.SkinID = source["SkinID"];
	        this.Flags = source["Flags"];
	        this.Player = source["Player"];
	        this.UnknownByte1 = source["UnknownByte1"];
	        this.UnknownByte2 = source["UnknownByte2"];
	        this.HitPointsPct = source["HitPointsPct"];
	        this.ManaPct = source["ManaPct"];
	        this.MapItemTable = source["MapItemTable"];
	        this.ItemDrops = this.convertValues(source["ItemDrops"], ItemDrop);
	        this.GoldAmount = source["GoldAmount"];
	        this.TargetAcquisition = source["TargetAcquisition"];
	        this.HeroLevel = source["HeroLevel"];
	        this.HeroStr = source["HeroStr"];
	        this.HeroAgi = source["HeroAgi"];
	        this.HeroInt = source["HeroInt"];
	        this.Inventory = this.convertValues(source["Inventory"], InventorySlot);
	        this.AbilityModifications = this.convertValues(source["AbilityModifications"], AbilityMod);
	        this.RandomType = source["RandomType"];
	        this.RandomData = source["RandomData"];
	        this.CustomColor = source["CustomColor"];
	        this.WaygateRegion = source["WaygateRegion"];
	        this.CreationNumber = source["CreationNumber"];
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	

}

export namespace update {
	
	export class Asset {
	    name: string;
	    url: string;
	    size: number;
	
	    static createFrom(source: any = {}) {
	        return new Asset(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.name = source["name"];
	        this.url = source["url"];
	        this.size = source["size"];
	    }
	}
	export class Release {
	    version: string;
	    tag: string;
	    notes: string;
	    url: string;
	    published: string;
	    installer?: Asset;
	
	    static createFrom(source: any = {}) {
	        return new Release(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.version = source["version"];
	        this.tag = source["tag"];
	        this.notes = source["notes"];
	        this.url = source["url"];
	        this.published = source["published"];
	        this.installer = this.convertValues(source["installer"], Asset);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class CheckResult {
	    current: string;
	    latest: string;
	    isNewer: boolean;
	    isDev: boolean;
	    release?: Release;
	
	    static createFrom(source: any = {}) {
	        return new CheckResult(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.current = source["current"];
	        this.latest = source["latest"];
	        this.isNewer = source["isNewer"];
	        this.isDev = source["isDev"];
	        this.release = this.convertValues(source["release"], Release);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}

}

export namespace w3i {
	
	export class CamDistance {
	    Default: number;
	    Max: number;
	    Min: number;
	
	    static createFrom(source: any = {}) {
	        return new CamDistance(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.Default = source["Default"];
	        this.Max = source["Max"];
	        this.Min = source["Min"];
	    }
	}
	export class Color {
	    R: number;
	    G: number;
	    B: number;
	    A: number;
	
	    static createFrom(source: any = {}) {
	        return new Color(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.R = source["R"];
	        this.G = source["G"];
	        this.B = source["B"];
	        this.A = source["A"];
	    }
	}
	export class Flags {
	    Raw: number;
	    HideMinimapPreview: boolean;
	    ModifyAllyPriorities: boolean;
	    MeleeMap: boolean;
	    Unknown1: boolean;
	    MaskedAreaPartiallyVisible: boolean;
	    FixedPlayerSettings: boolean;
	    CustomForces: boolean;
	    CustomTechtree: boolean;
	    CustomAbilities: boolean;
	    CustomUpgrades: boolean;
	    Unknown2: boolean;
	    CliffShoreWaves: boolean;
	    RollingShoreWaves: boolean;
	    Unknown3: boolean;
	    Unknown4: boolean;
	    ItemClassification: boolean;
	    WaterTinting: boolean;
	    AccurateProbabilityForCalculations: boolean;
	    CustomAbilitySkins: boolean;
	    DisableDenyIcon: boolean;
	    ForceDefaultZoom: boolean;
	    ForceMaxZoom: boolean;
	    ForceMinZoom: boolean;
	
	    static createFrom(source: any = {}) {
	        return new Flags(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.Raw = source["Raw"];
	        this.HideMinimapPreview = source["HideMinimapPreview"];
	        this.ModifyAllyPriorities = source["ModifyAllyPriorities"];
	        this.MeleeMap = source["MeleeMap"];
	        this.Unknown1 = source["Unknown1"];
	        this.MaskedAreaPartiallyVisible = source["MaskedAreaPartiallyVisible"];
	        this.FixedPlayerSettings = source["FixedPlayerSettings"];
	        this.CustomForces = source["CustomForces"];
	        this.CustomTechtree = source["CustomTechtree"];
	        this.CustomAbilities = source["CustomAbilities"];
	        this.CustomUpgrades = source["CustomUpgrades"];
	        this.Unknown2 = source["Unknown2"];
	        this.CliffShoreWaves = source["CliffShoreWaves"];
	        this.RollingShoreWaves = source["RollingShoreWaves"];
	        this.Unknown3 = source["Unknown3"];
	        this.Unknown4 = source["Unknown4"];
	        this.ItemClassification = source["ItemClassification"];
	        this.WaterTinting = source["WaterTinting"];
	        this.AccurateProbabilityForCalculations = source["AccurateProbabilityForCalculations"];
	        this.CustomAbilitySkins = source["CustomAbilitySkins"];
	        this.DisableDenyIcon = source["DisableDenyIcon"];
	        this.ForceDefaultZoom = source["ForceDefaultZoom"];
	        this.ForceMaxZoom = source["ForceMaxZoom"];
	        this.ForceMinZoom = source["ForceMinZoom"];
	    }
	}
	export class Fog {
	    Style: number;
	    StartZ: number;
	    EndZ: number;
	    Density: number;
	    Color: Color;
	
	    static createFrom(source: any = {}) {
	        return new Fog(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.Style = source["Style"];
	        this.StartZ = source["StartZ"];
	        this.EndZ = source["EndZ"];
	        this.Density = source["Density"];
	        this.Color = this.convertValues(source["Color"], Color);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class ForceFlags {
	    Raw: number;
	    Allied: boolean;
	    AlliedVictory: boolean;
	    ShareVision: boolean;
	    ShareUnitControl: boolean;
	    ShareAdvancedUnitControl: boolean;
	
	    static createFrom(source: any = {}) {
	        return new ForceFlags(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.Raw = source["Raw"];
	        this.Allied = source["Allied"];
	        this.AlliedVictory = source["AlliedVictory"];
	        this.ShareVision = source["ShareVision"];
	        this.ShareUnitControl = source["ShareUnitControl"];
	        this.ShareAdvancedUnitControl = source["ShareAdvancedUnitControl"];
	    }
	}
	export class Force {
	    Flags: ForceFlags;
	    PlayerMasks: number;
	    Name: string;
	
	    static createFrom(source: any = {}) {
	        return new Force(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.Flags = this.convertValues(source["Flags"], ForceFlags);
	        this.PlayerMasks = source["PlayerMasks"];
	        this.Name = source["Name"];
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	
	export class IVec4 {
	    A: number;
	    B: number;
	    C: number;
	    D: number;
	
	    static createFrom(source: any = {}) {
	        return new IVec4(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.A = source["A"];
	        this.B = source["B"];
	        this.C = source["C"];
	        this.D = source["D"];
	    }
	}
	export class RandomItemEntry {
	    Chance: number;
	    ID: string;
	
	    static createFrom(source: any = {}) {
	        return new RandomItemEntry(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.Chance = source["Chance"];
	        this.ID = source["ID"];
	    }
	}
	export class RandomItemSet {
	    Items: RandomItemEntry[];
	
	    static createFrom(source: any = {}) {
	        return new RandomItemSet(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.Items = this.convertValues(source["Items"], RandomItemEntry);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class RandomItemTable {
	    CreationNumber: number;
	    Name: string;
	    ItemSets: RandomItemSet[];
	
	    static createFrom(source: any = {}) {
	        return new RandomItemTable(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.CreationNumber = source["CreationNumber"];
	        this.Name = source["Name"];
	        this.ItemSets = this.convertValues(source["ItemSets"], RandomItemSet);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class RandomUnitLine {
	    Chance: number;
	    IDs: string[];
	
	    static createFrom(source: any = {}) {
	        return new RandomUnitLine(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.Chance = source["Chance"];
	        this.IDs = source["IDs"];
	    }
	}
	export class RandomUnitTable {
	    CreationNumber: number;
	    Name: string;
	    Positions: number[];
	    Lines: RandomUnitLine[];
	
	    static createFrom(source: any = {}) {
	        return new RandomUnitTable(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.CreationNumber = source["CreationNumber"];
	        this.Name = source["Name"];
	        this.Positions = source["Positions"];
	        this.Lines = this.convertValues(source["Lines"], RandomUnitLine);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class TechAvailability {
	    PlayerFlags: number;
	    ID: string;
	
	    static createFrom(source: any = {}) {
	        return new TechAvailability(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.PlayerFlags = source["PlayerFlags"];
	        this.ID = source["ID"];
	    }
	}
	export class UpgradeAvailability {
	    PlayerFlags: number;
	    ID: string;
	    Level: number;
	    Availability: number;
	
	    static createFrom(source: any = {}) {
	        return new UpgradeAvailability(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.PlayerFlags = source["PlayerFlags"];
	        this.ID = source["ID"];
	        this.Level = source["Level"];
	        this.Availability = source["Availability"];
	    }
	}
	export class Player {
	    InternalNumber: number;
	    Type: number;
	    Race: number;
	    FixedStartPosition: number;
	    Name: string;
	    StartingPosition: Vec2;
	    AllyLowPriorities: number;
	    AllyHighPriorities: number;
	    EnemyLowPriorities: number;
	    EnemyHighPriorities: number;
	
	    static createFrom(source: any = {}) {
	        return new Player(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.InternalNumber = source["InternalNumber"];
	        this.Type = source["Type"];
	        this.Race = source["Race"];
	        this.FixedStartPosition = source["FixedStartPosition"];
	        this.Name = source["Name"];
	        this.StartingPosition = this.convertValues(source["StartingPosition"], Vec2);
	        this.AllyLowPriorities = source["AllyLowPriorities"];
	        this.AllyHighPriorities = source["AllyHighPriorities"];
	        this.EnemyLowPriorities = source["EnemyLowPriorities"];
	        this.EnemyHighPriorities = source["EnemyHighPriorities"];
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class Prologue {
	    Model: string;
	    Text: string;
	    Title: string;
	    Subtitle: string;
	
	    static createFrom(source: any = {}) {
	        return new Prologue(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.Model = source["Model"];
	        this.Text = source["Text"];
	        this.Title = source["Title"];
	        this.Subtitle = source["Subtitle"];
	    }
	}
	export class LoadingScreen {
	    Number: number;
	    Model: string;
	    Text: string;
	    Title: string;
	    Subtitle: string;
	
	    static createFrom(source: any = {}) {
	        return new LoadingScreen(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.Number = source["Number"];
	        this.Model = source["Model"];
	        this.Text = source["Text"];
	        this.Title = source["Title"];
	        this.Subtitle = source["Subtitle"];
	    }
	}
	export class Vec2 {
	    X: number;
	    Y: number;
	
	    static createFrom(source: any = {}) {
	        return new Vec2(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.X = source["X"];
	        this.Y = source["Y"];
	    }
	}
	export class Info {
	    FileVersion: number;
	    MapVersion: number;
	    EditorVersion: number;
	    GameVersion: number[];
	    Name: string;
	    Author: string;
	    Description: string;
	    SuggestedPlayers: string;
	    CameraLeftBottom: Vec2;
	    CameraRightTop: Vec2;
	    CameraLeftTop: Vec2;
	    CameraRightBottom: Vec2;
	    CameraComplements: IVec4;
	    PlayableWidth: number;
	    PlayableHeight: number;
	    Flags: Flags;
	    Tileset: number;
	    LoadingScreen: LoadingScreen;
	    GameDataSet: number;
	    Prologue: Prologue;
	    Fog: Fog;
	    WeatherID: number;
	    CustomSoundEnv: string;
	    CustomLightTileset: number;
	    WaterColor: Color;
	    Lua: boolean;
	    SupportedModes: number;
	    GameDataVersion: number;
	    CamDistance: CamDistance;
	    Players: Player[];
	    Forces: Force[];
	    AvailableUpgrades: UpgradeAvailability[];
	    AvailableTech: TechAvailability[];
	    RandomUnitTables: RandomUnitTable[];
	    RandomItemTables: RandomItemTable[];
	
	    static createFrom(source: any = {}) {
	        return new Info(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.FileVersion = source["FileVersion"];
	        this.MapVersion = source["MapVersion"];
	        this.EditorVersion = source["EditorVersion"];
	        this.GameVersion = source["GameVersion"];
	        this.Name = source["Name"];
	        this.Author = source["Author"];
	        this.Description = source["Description"];
	        this.SuggestedPlayers = source["SuggestedPlayers"];
	        this.CameraLeftBottom = this.convertValues(source["CameraLeftBottom"], Vec2);
	        this.CameraRightTop = this.convertValues(source["CameraRightTop"], Vec2);
	        this.CameraLeftTop = this.convertValues(source["CameraLeftTop"], Vec2);
	        this.CameraRightBottom = this.convertValues(source["CameraRightBottom"], Vec2);
	        this.CameraComplements = this.convertValues(source["CameraComplements"], IVec4);
	        this.PlayableWidth = source["PlayableWidth"];
	        this.PlayableHeight = source["PlayableHeight"];
	        this.Flags = this.convertValues(source["Flags"], Flags);
	        this.Tileset = source["Tileset"];
	        this.LoadingScreen = this.convertValues(source["LoadingScreen"], LoadingScreen);
	        this.GameDataSet = source["GameDataSet"];
	        this.Prologue = this.convertValues(source["Prologue"], Prologue);
	        this.Fog = this.convertValues(source["Fog"], Fog);
	        this.WeatherID = source["WeatherID"];
	        this.CustomSoundEnv = source["CustomSoundEnv"];
	        this.CustomLightTileset = source["CustomLightTileset"];
	        this.WaterColor = this.convertValues(source["WaterColor"], Color);
	        this.Lua = source["Lua"];
	        this.SupportedModes = source["SupportedModes"];
	        this.GameDataVersion = source["GameDataVersion"];
	        this.CamDistance = this.convertValues(source["CamDistance"], CamDistance);
	        this.Players = this.convertValues(source["Players"], Player);
	        this.Forces = this.convertValues(source["Forces"], Force);
	        this.AvailableUpgrades = this.convertValues(source["AvailableUpgrades"], UpgradeAvailability);
	        this.AvailableTech = this.convertValues(source["AvailableTech"], TechAvailability);
	        this.RandomUnitTables = this.convertValues(source["RandomUnitTables"], RandomUnitTable);
	        this.RandomItemTables = this.convertValues(source["RandomItemTables"], RandomItemTable);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	
	
	
	
	
	
	
	
	
	

}

export namespace wtg {
	
	export class Category {
	    classifier: number;
	    id: number;
	    parent_id: number;
	    name: string;
	    open_state: boolean;
	    is_comment?: boolean;
	
	    static createFrom(source: any = {}) {
	        return new Category(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.classifier = source["classifier"];
	        this.id = source["id"];
	        this.parent_id = source["parent_id"];
	        this.name = source["name"];
	        this.open_state = source["open_state"];
	        this.is_comment = source["is_comment"];
	    }
	}
	export class Parameter {
	    type: number;
	    value: string;
	    has_sub_parameter?: boolean;
	    sub_parameter?: ECA;
	    unknown?: number;
	    is_array?: boolean;
	    array_index?: Parameter;
	    resolved_display?: string;
	
	    static createFrom(source: any = {}) {
	        return new Parameter(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.type = source["type"];
	        this.value = source["value"];
	        this.has_sub_parameter = source["has_sub_parameter"];
	        this.sub_parameter = this.convertValues(source["sub_parameter"], ECA);
	        this.unknown = source["unknown"];
	        this.is_array = source["is_array"];
	        this.array_index = this.convertValues(source["array_index"], Parameter);
	        this.resolved_display = source["resolved_display"];
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class ECA {
	    type: number;
	    group?: number;
	    name: string;
	    enabled: boolean;
	    parameters?: Parameter[];
	    children?: ECA[];
	    has_parameters?: boolean;
	
	    static createFrom(source: any = {}) {
	        return new ECA(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.type = source["type"];
	        this.group = source["group"];
	        this.name = source["name"];
	        this.enabled = source["enabled"];
	        this.parameters = this.convertValues(source["parameters"], Parameter);
	        this.children = this.convertValues(source["children"], ECA);
	        this.has_parameters = source["has_parameters"];
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	
	export class Trigger {
	    classifier: number;
	    id: number;
	    parent_id: number;
	    name: string;
	    description?: string;
	    custom_text?: string;
	    is_comment?: boolean;
	    is_enabled: boolean;
	    is_script?: boolean;
	    initially_on: boolean;
	    run_on_initialization?: boolean;
	    ecas?: ECA[];
	
	    static createFrom(source: any = {}) {
	        return new Trigger(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.classifier = source["classifier"];
	        this.id = source["id"];
	        this.parent_id = source["parent_id"];
	        this.name = source["name"];
	        this.description = source["description"];
	        this.custom_text = source["custom_text"];
	        this.is_comment = source["is_comment"];
	        this.is_enabled = source["is_enabled"];
	        this.is_script = source["is_script"];
	        this.initially_on = source["initially_on"];
	        this.run_on_initialization = source["run_on_initialization"];
	        this.ecas = this.convertValues(source["ecas"], ECA);
	    }
	
		convertValues(a: any, classs: any, asMap: boolean = false): any {
		    if (!a) {
		        return a;
		    }
		    if (a.slice && a.map) {
		        return (a as any[]).map(elem => this.convertValues(elem, classs));
		    } else if ("object" === typeof a) {
		        if (asMap) {
		            for (const key of Object.keys(a)) {
		                a[key] = new classs(a[key]);
		            }
		            return a;
		        }
		        return new classs(a);
		    }
		    return a;
		}
	}
	export class Variable {
	    name: string;
	    type: string;
	    unknown?: number;
	    is_array?: boolean;
	    array_size?: number;
	    is_initialized?: boolean;
	    initial_value?: string;
	    id: number;
	    parent_id: number;
	
	    static createFrom(source: any = {}) {
	        return new Variable(source);
	    }
	
	    constructor(source: any = {}) {
	        if ('string' === typeof source) source = JSON.parse(source);
	        this.name = source["name"];
	        this.type = source["type"];
	        this.unknown = source["unknown"];
	        this.is_array = source["is_array"];
	        this.array_size = source["array_size"];
	        this.is_initialized = source["is_initialized"];
	        this.initial_value = source["initial_value"];
	        this.id = source["id"];
	        this.parent_id = source["parent_id"];
	    }
	}

}

