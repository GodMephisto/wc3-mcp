---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/magic.rs"
ref: HEAD
---

pub struct MdlxMagic;

impl MdlxMagic {
    pub const MDLX: u32 = 0x4D444C58; /* Magic: "MDLX" */
    pub const VERS: u32 = 0x56455253; /* Version */
    pub const MODL: u32 = 0x4D4F444C; /* Model */
    pub const SEQS: u32 = 0x53455153; /* Sequences */
    pub const GLBS: u32 = 0x474C4253; /* Global Sequences */
    pub const TEXS: u32 = 0x54455853; /* Textures */
    pub const PIVT: u32 = 0x50495654; /* Pivot Points */

    pub const MTLS: u32 = 0x4D544C53; /* Materials */
    pub const LAYS: u32 = 0x4C415953; /* - Layers */
    pub const KMTA: u32 = 0x4B4D5441; /* - - Alpha */
    pub const KMTF: u32 = 0x4B4D5446; /* - - Texture ID */

    pub const TXAN: u32 = 0x5458414E; /* Texture Animations */
    pub const KTAT: u32 = 0x4B544154; /* - Translation */
    pub const KTAR: u32 = 0x4B544152; /* - Rotation */
    pub const KTAS: u32 = 0x4B544153; /* - Scaling */

    pub const GEOS: u32 = 0x47454F53; /* Geosets */
    pub const VRTX: u32 = 0x56525458; /* - Vertex Position */
    pub const NRMS: u32 = 0x4E524D53; /* - Vertex Normal (could be missing) */
    pub const PTYP: u32 = 0x50545950; /* - Face Type List */
    pub const PCNT: u32 = 0x50434E54; /* - Vertices Group Count ([index]th face type has [value] vertices) */
    pub const PVTX: u32 = 0x50565458; /* - Vertices of Faces (flat of all face vertices, each 3 vertices form a triangle face) */
    pub const GNDX: u32 = 0x474E4458; /* - Vertex Group (bind [index]th vertex to [value]th matrix group) */
    pub const MTGC: u32 = 0x4D544743; /* - Matrices Group Count ([index]th group has [value] matrices) */
    pub const MATS: u32 = 0x4D415453; /* - Matrices Group Indices (flat of all matrices group) */
    pub const UVAS: u32 = 0x55564153; /* - UVs count */
    pub const UVBS: u32 = 0x55564253; /* - UVs */

    pub const GEOA: u32 = 0x47454F41; /* Geoset Animations */
    pub const KGAO: u32 = 0x4B47414F; /* - Alpha */
    pub const KGAC: u32 = 0x4B474143; /* - Color */

    pub const CAMS: u32 = 0x43414D53; /* Cameras */
    pub const KCTR: u32 = 0x4B435452; /* - Position Translation */
    pub const KCRL: u32 = 0x4B43524C; /* - Rotation */
    pub const KTTR: u32 = 0x4B545452; /* - Target Translation */

    //--------------------------------//* Node *//
    pub const KGTR: u32 = 0x4B475452; /* - Translation */
    pub const KGRT: u32 = 0x4B475254; /* - Rotation */
    pub const KGSC: u32 = 0x4B475343; /* - Scaling */

    pub const BONE: u32 = 0x424F4E45; /* Bones */
    pub const HELP: u32 = 0x48454C50; /* Helpers */
    pub const CLID: u32 = 0x434C4944; /* Collision Shapes */

    pub const ATCH: u32 = 0x41544348; /* Attachments */
    pub const KATV: u32 = 0x4B415456; /* - Visibility */

    pub const EVTS: u32 = 0x45565453; /* Event Objects */
    pub const KEVT: u32 = 0x4B455654; /* - Tracks */

    pub const LITE: u32 = 0x4C495445; /* Lights */
    pub const KLAV: u32 = 0x4B4C4156; /* - Visibility */
    pub const KLAS: u32 = 0x4B4C4153; /* - AttenuationStart */
    pub const KLAE: u32 = 0x4B4C4145; /* - AttenuationEnd */
    pub const KLAC: u32 = 0x4B4C4143; /* - Color */
    pub const KLAI: u32 = 0x4B4C4149; /* - Intensity */
    pub const KLBC: u32 = 0x4B4C4243; /* - Ambient Color */
    pub const KLBI: u32 = 0x4B4C4249; /* - Ambient Intensity */

    pub const PREM: u32 = 0x5052454D; /* Particle Emitters */
    pub const KPEV: u32 = 0x4B504556; /* - Visibility */
    pub const KPEE: u32 = 0x4B504545; /* - EmissionRate */
    pub const KPEG: u32 = 0x4B504547; /* - Gravity */
    pub const KPLN: u32 = 0x4B504C4E; /* - Longitude */
    pub const KPLT: u32 = 0x4B504C54; /* - Latitude */
    pub const KPEL: u32 = 0x4B50454C; /* - LifeSpan */
    pub const KPES: u32 = 0x4B504553; /* - Speed */

    pub const PRE2: u32 = 0x50524532; /* Particle Emitters 2 */
    pub const KP2V: u32 = 0x4B503256; /* - Visibility */
    pub const KP2E: u32 = 0x4B503245; /* - Emission Rate */
    pub const KP2W: u32 = 0x4B503257; /* - Width */
    pub const KP2N: u32 = 0x4B50324E; /* - Length */
    pub const KP2S: u32 = 0x4B503253; /* - Speed */
    pub const KP2L: u32 = 0x4B50324C; /* - Latitude */
    pub const KP2R: u32 = 0x4B503252; /* - Variation */
    pub const KP2G: u32 = 0x4B503247; /* - Gravity */

    pub const RIBB: u32 = 0x52494242; /* Ribbon Emitters */
    pub const KRVS: u32 = 0x4B525653; /* - Visibility */
    pub const KRHA: u32 = 0x4B524841; /* - Height Above */
    pub const KRHB: u32 = 0x4B524842; /* - Height Below */
    pub const KRAL: u32 = 0x4B52414C; /* - Alpha */
    pub const KRCO: u32 = 0x4B52434F; /* - Color */
    pub const KRTX: u32 = 0x4B525458; /* - TextureSlot */
}
