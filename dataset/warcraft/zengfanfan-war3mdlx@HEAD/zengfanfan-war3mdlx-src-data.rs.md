---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/data.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct MdlxData {
    pub version: Version,
    pub model: Model,
    pub sequences: Vec<Sequence>,
    #[dbg(formatter = "fmtx")]
    pub globalseqs: Vec<GlobalSequence>,
    pub textures: Vec<Texture>,
    pub materials: Vec<Material>,
    pub texanims: Vec<TextureAnim>,
    pub geosets: Vec<Geoset>,
    pub geoanims: Vec<GeosetAnim>,
    #[dbg(formatter = "fmtx")]
    pub pivot_points: Vec<PivotPoint>,
    pub cameras: Vec<Camera>,
    pub bones: Vec<Bone>,
    pub helpers: Vec<Helper>,
    pub attachments: Vec<Attachment>,
    pub collisions: Vec<CollisionShape>,
    pub lights: Vec<Light>,
    pub eventobjs: Vec<EventObject>,
    pub particle_emitters: Vec<ParticleEmitter>,
    pub particle_emitters2: Vec<ParticleEmitter2>,
    pub ribbon_emitters: Vec<RibbonEmitter>,
}

impl MdlxData {
    pub fn read(path: &Path) -> Result<Self, MyError> {
        let ret = match path.ext_lower().as_str() {
            "mdl" => match std::fs::read_to_string(path) {
                Err(e) => Err(MyError::Io(e)),
                Ok(s) => Self::read_mdl(&s).or_else(|e| Err(e)),
            },
            "mdx" => match std::fs::read(path) {
                Err(e) => Err(MyError::Io(e)),
                Ok(s) => Self::read_mdx(&s).or_else(|e| Err(e)),
            },
            _ => EXIT1!("Invalid input path: {:?}, expecting *.mdl or *.mdx", path),
        };
        match ret {
            Err(e) => EXIT1!("Failed to read file {:?}: {}.", path, e),
            Ok(mut this) => {
                for (i, a) in this.attachments.iter_mut().enumerate() {
                    a.appear_order = i as i32;
                }
                return Ok(this);
            },
        }
    }

    pub fn write(&mut self, path: &Path) -> Result<(), MyError> {
        if let Some(parent) = path.parent() {
            if let Err(e) = fs::create_dir_all(parent) {
                EXIT1!("creating directory: {}", e);
            }
        }
        match path.ext_lower().as_ref() {
            "mdl" => self.write_mdl(path).or_else(|e| ERR!("Failed to write file {:?}: {}", path, e)),
            "mdx" => self.write_mdx(path).or_else(|e| ERR!("Failed to write file {:?}: {}", path, e)),
            _ => ERR!("Invalid output path: {:?}, expected *.mdl or *.mdx", path),
        }
    }
}
