---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/fields/geoset.rs"
ref: HEAD
---

use crate::*;

#[derive(Dbg, Default)]
pub struct Geoset {
    #[dbg(formatter = "fmtx")]
    pub vertices: Vec<Vec3>,
    #[dbg(formatter = "fmtx")]
    pub normals: Vec<Vec3>,
    #[dbg(skip)]
    nvs_count: u32,
    #[dbg(formatter = "fmtx")]
    pub uvss: Vec<Vec<Vec2>>,
    #[dbg(fmt = "{:?}")]
    pub face_types: Vec<FaceType>,
    #[dbg(formatter = "fmtx")]
    pub face_vtxcnts: Vec<i32>,
    #[dbg(formatter = "fmtx")]
    pub face_vertices: Vec<u16>,
    #[dbg(formatter = "fmtx")]
    pub vtxgrps: Vec<u8>,
    #[dbg(formatter = "fmtx")]
    pub mtxgrpcnts: Vec<i32>,
    #[dbg(formatter = "fmtx")]
    pub mtx_indices: Vec<i32>,

    pub material_id: i32,
    pub sel_group: i32,
    pub sel_type: i32, // 0=None, 4=Unselectable
    pub extent: BoundExtent,
    pub anim_extents: Vec<BoundExtent>,
}

// region: BoundExtent

#[derive(Dbg, Default)]
pub struct BoundExtent {
    #[dbg(formatter = "fmtx")]
    pub bound_radius: f32,
    #[dbg(formatter = "fmtx")]
    pub min_extent: Vec3,
    #[dbg(formatter = "fmtx")]
    pub max_extent: Vec3,
}

impl BoundExtent {
    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        Ok(Self { bound_radius: cur.readx()?, min_extent: cur.readx()?, max_extent: cur.readx()? })
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.bound_radius)?;
        chunk.write(&self.min_extent)?;
        chunk.write(&self.max_extent)?;
        return Ok(());
    }
    pub fn size() -> u32 {
        28 // = 4 + 12 + 12
    }

    pub fn read_mdl(block: &MdlBlock, strict: bool) -> Result<Self, MyError> {
        let mut this = Build!();
        yes!(strict, block.unexpect_frames()?);
        yes!(strict, block.unexpect_blocks()?);
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "BoundsRadius" => this.bound_radius = f.value.to()?,
                "MinimumExtent" => this.min_extent = f.value.to()?,
                "MaximumExtent" => this.max_extent = f.value.to()?,
                _other => yesno!(strict, return f.unexpect(), ()),
            );
        }
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let mut lines: Vec<String> = vec![];
        let indent = indent!(depth);
        lines.pushx_if_n0(&F!("{indent}BoundsRadius"), &self.bound_radius);
        lines.pushx_if_n0(&F!("{indent}MinimumExtent"), &self.min_extent);
        lines.pushx_if_n0(&F!("{indent}MaximumExtent"), &self.max_extent);
        return Ok(lines);
    }
}

// endregion
// region: FaceType

#[derive(Debug, Default, PartialEq, Eq, Clone, Copy)]
pub enum FaceType {
    Points,
    Lines,
    LineLoop,
    LineStrip,
    #[default]
    Triangles,
    TriangleStrip,
    TriangleFan,
    Quads,
    QuadStrip,
    Polygons,
    Error(i32),
}

impl FaceType {
    fn from(v: i32) -> Self {
        match v {
            0 => Self::Points,
            1 => Self::Lines,
            2 => Self::LineLoop,
            3 => Self::LineStrip,
            4 => Self::Triangles,
            5 => Self::TriangleStrip,
            6 => Self::TriangleFan,
            7 => Self::Quads,
            8 => Self::QuadStrip,
            9 => Self::Polygons,
            x => Self::Error(x),
        }
    }

    fn from_str(s: &str) -> Self {
        match_istr!(s,
            "Points" => Self::Points,
            "Lines" => Self::Lines,
            "LineLoop" => Self::LineLoop,
            "LineStrip" => Self::LineStrip,
            "Triangles" => Self::Triangles,
            "TriangleStrip" => Self::TriangleStrip,
            "TriangleFan" => Self::TriangleFan,
            "Quads" => Self::Quads,
            "QuadStrip" => Self::QuadStrip,
            "Polygons" => Self::Polygons,
            _err => Self::Error(-1),
        )
    }

    fn to(&self) -> i32 {
        match self {
            Self::Points => 0,
            Self::Lines => 1,
            Self::LineLoop => 2,
            Self::LineStrip => 3,
            Self::Triangles => 4,
            Self::TriangleStrip => 5,
            Self::TriangleFan => 6,
            Self::Quads => 7,
            Self::QuadStrip => 8,
            Self::Polygons => 9,
            Self::Error(x) => *x,
        }
    }
}

// endregion

macro_rules! MdxWriteGeosetChunk {
    ($chunk:expr, $( $id:expr => $var:expr ),+ $(,)?) => {
        $(
            let list = &$var;
            $chunk.write_be(&$id)?;
            $chunk.write(&list.len())?;
            $chunk.write(list)?;
        )+
    };
}
macro_rules! MdxWriteGeosetChunk_CalcSize {
    ($( $var:expr ),+ $(,)?) => {
        {let mut size = 0;
            $(
                let list = &$var;
                size += 8;
                if list.len() > 0 {
                    size += list[0].calc_size() * list.len() as u32;
                }
            )+
        size}
    };
}

impl Geoset {
    pub const ID: u32 = MdlxMagic::GEOS;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build!();

        while cur.left() >= 16 {
            let (id, n) = (cur.read_be::<u32>()?, cur.readx::<u32>()?);
            match id {
                MdlxMagic::VRTX => this.vertices = cur.read_array(n)?,
                MdlxMagic::NRMS => this.normals = cur.read_array(n)?,
                MdlxMagic::PTYP => this.face_types = cur.read_array::<i32>(n)?.convert(|a| FaceType::from(*a)),
                MdlxMagic::PCNT => this.face_vtxcnts = cur.read_array(n)?,
                MdlxMagic::PVTX => this.face_vertices = cur.read_array(n)?,
                MdlxMagic::GNDX => this.vtxgrps = cur.read_array(n)?,
                MdlxMagic::MTGC => this.mtxgrpcnts = cur.read_array(n)?,
                MdlxMagic::MATS => this.mtx_indices = cur.read_array(n)?,
                MdlxMagic::UVAS => this.nvs_count = n,
                MdlxMagic::UVBS => this.uvss.push(cur.read_array(n)?),
                id => {
                    this.material_id = id.swap_bytes() as i32;
                    this.sel_group = n as i32;
                    this.sel_type = cur.readx()?;
                    this.extent = BoundExtent::read_mdx(cur)?;
                    let en = cur.readx()?;
                    for _ in 0..en {
                        this.anim_extents.push(BoundExtent::read_mdx(cur)?);
                    }
                },
            }
        }

        this.validate();
        return Ok(this);
    }

    fn validate(&self) {
        let tn = TNAME!();

        let (nnorm, nvert) = (self.normals.len(), self.vertices.len());
        yes!(nnorm > 0 && nnorm != nvert, wlog!("OMG! {tn} #[normals] {} != {} #[vertices] ?", nnorm, nvert));

        let n = self.uvss.len() as u32;
        yes!(self.nvs_count != n, wlog!("OMG! {tn} #[UVs] {} != {n} ?", self.nvs_count));

        let (n1, n2) = (self.face_vtxcnts.len(), self.face_types.len());
        yes!(n1 != n2, wlog!("OMG! {tn} #[face_vtxcnts] != #[face_types] ?"));

        let (tri, trin) = (FaceType::Triangles, 3);
        for (t, n) in self.face_types.iter().zip(self.face_vtxcnts.iter()) {
            if *t == tri {
                if n % trin != 0 {
                    wlog!("Expecting length of {t:?} (in {}) to be multiple of {trin}, got {n}.", TNAME!());
                }
            } else {
                wlog!("OMG! {} other than {tri:?}({}): {:?}", TNAME!(&tri), tri.to(), self.face_types);
            }
        }
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.calc_mdx_size())?;

        MdxWriteGeosetChunk!(chunk,
            MdlxMagic::VRTX => self.vertices,
            MdlxMagic::NRMS => self.normals,
            MdlxMagic::PTYP => self.face_types.convert(|a| a.to()),
            MdlxMagic::PCNT => self.face_vtxcnts,
            MdlxMagic::PVTX => self.face_vertices,
            MdlxMagic::GNDX => self.vtxgrps,
            MdlxMagic::MTGC => self.mtxgrpcnts,
            MdlxMagic::MATS => self.mtx_indices,
        );

        chunk.write(&self.material_id)?;
        chunk.write(&self.sel_group)?;
        chunk.write(&self.sel_type)?;
        self.extent.write_mdx(chunk)?;
        chunk.write(&self.anim_extents.len())?;
        for a in self.anim_extents.iter() {
            a.write_mdx(chunk)?;
        }

        chunk.write_be(&MdlxMagic::UVAS)?;
        chunk.write(&self.uvss.len())?;
        for a in self.uvss.iter() {
            chunk.write_be(&MdlxMagic::UVBS)?;
            chunk.write(&a.len())?;
            chunk.write(a)?;
        }

        return Ok(());
    }
    pub fn calc_mdx_size(&self) -> u32 {
        let mut sz: u32 = 4; // sz itself

        sz += MdxWriteGeosetChunk_CalcSize!(
            self.vertices,
            self.normals,
            self.face_types.convert(|a| a.to()),
            self.face_vtxcnts,
            self.face_vertices,
            self.vtxgrps,
            self.mtxgrpcnts,
            self.mtx_indices,
        );

        sz += 12; // material_id + sel_group + sel_type
        sz += BoundExtent::size(); // extent
        sz += 4 + BoundExtent::size() * self.anim_extents.len() as u32; // anim_extents

        sz += 8; // "UVAS" + len
        for a in self.uvss.iter() {
            sz += 8; // "UVBS" + len
            sz += a.len() as u32 * Vec2::size();
        }

        return sz;
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        let mut this = Build! { extent: BoundExtent::read_mdl(&block, false)? };
        block.unexpect_frames()?;
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "MaterialID" => this.material_id = f.value.to()?,
                "SelectionGroup" => this.sel_group = f.value.to()?,
                "Unselectable" => this.sel_type |= f.expect_flag(4)?,
                "BoundsRadius" | "MinimumExtent" | "MaximumExtent" => (),
                _other => f.unexpect()?,
            );
        }
        for a in &block.blocks {
            match_istr!(a.typ.as_str(),
                "Vertices" => this.vertices = a.to_array("")?,
                "Normals" => this.normals = a.to_array("")?,
                "TVertices" => this.uvss.push(a.to_array("")?),
                "VertexGroup" => this.vtxgrps = a.to_array("")?,
                "Faces" => {
                    a.unexpect_fields()?;
                    a.unexpect_frames()?;
                    for b in &a.blocks { this.read_mdl_face(b)?; }
                },
                "Groups" => {
                    a.unexpect_frames()?;
                    a.unexpect_blocks()?;
                    for f in &a.fields { this.read_mdl_matrices(f)?; }
                },
                "Anim" => this.anim_extents.push(BoundExtent::read_mdl(&a, true)?),
                _other => a.unexpect()?,
            );
        }
        this.nvs_count = this.uvss.len() as u32;
        this.validate();
        return Ok(this);
    }
    fn read_mdl_face(&mut self, block: &MdlBlock) -> Result<(), MyError> {
        block.unexpect_frames()?;
        block.unexpect_blocks()?;

        let (ts, line) = (block.typ.as_str(), block.line);
        let t = FaceType::from_str(ts);
        if let FaceType::Error(_) = t {
            EXIT1!("Unknown {} {ts:?} at line {line}.", TNAME!(&t));
        }

        no!(t == FaceType::Triangles, wlog!("OMG! Bad {} ({:?}) at line {}.", TNAME!(&t), t, line));
        for f in &block.fields {
            no!(f.name.is_empty(), EXIT1!("Unexpected {:?} (in {}) at line {}.", f.name, ts, f.line));
            let iv: Vec<i32> = f.value.to()?;
            self.face_types.push(t);
            self.face_vtxcnts.push(iv.len() as i32);
            let mut miv = iv.convert(|v| *v as u16);
            self.face_vertices.append(&mut miv);
        }

        return Ok(());
    }
    fn read_mdl_matrices(&mut self, field: &MdlField) -> Result<(), MyError> {
        let (n, v, l) = (&field.name, &field.value, field.line);
        if !n.eq_icase("Matrices") {
            EXIT1!("Unknown GroupType {n:?} at line {l}.");
        }
        let iv: Vec<i32> = v.to()?;
        self.mtxgrpcnts.push(iv.len() as i32);
        let mut miv = iv.convert(|v| *v as i32);
        self.mtx_indices.append(&mut miv);
        return Ok(());
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let (indent, indent2, indent3) = (indent!(depth), indent!(depth + 1), indent!(depth + 2));
        let mut lines: Vec<String> = vec![];

        MdlWriteType2!(lines, depth, "Vertices" => self.vertices);
        MdlWriteType2!(lines, depth, "Normals" => self.normals);
        for uvs in self.uvss.iter() {
            MdlWriteType2!(lines, depth, "TVertices" => *uvs);
        }
        {
            lines.push(F!("{indent}VertexGroup {{"));
            lines.append(&mut self.vtxgrps.iter().map(|x| F!("{indent2}{},", fmtx(x))).collect::<Vec<String>>());
            lines.push(F!("{indent}}}"));
        }
        {
            lines.push(F!("{indent}Faces {} {} {{", self.face_types.len(), self.face_vertices.len()));
            let mut i = 0_usize;
            for (t, n) in self.face_types.iter().zip(self.face_vtxcnts.iter()) {
                let n = *n as usize;
                lines.push(F!("{indent2}{:?} {{", t));
                if let Some(slice) = &self.face_vertices.get(i..i + n) {
                    let s = slice.iter().map(|x| fmtx(x)).collect::<Vec<String>>().join(", ");
                    lines.push(F!("{indent3}{{ {} }},", s));
                }
                lines.push(F!("{indent2}}}"));
                i += n;
            }
            lines.push(F!("{indent}}}"));
        }
        {
            lines.push(F!("{indent}Groups {} {} {{", self.mtxgrpcnts.len(), self.mtx_indices.len()));
            let mut i = 0_usize;
            for n in &self.mtxgrpcnts {
                let n = *n as usize;
                let mut slist: Vec<String> = vec![];
                slist.push(F!("Matrices {{"));
                if let Some(slice) = &self.mtx_indices.get(i..i + n) {
                    let s = slice.iter().map(|x| fmtx(x)).collect::<Vec<String>>().join(", ");
                    slist.push(F!("{}", s));
                }
                slist.push(F!("}},"));
                lines.push(F!("{indent2}{}", slist.join(" ")));
                i += n;
            }
            lines.push(F!("{indent}}}"));
        }

        lines.append(&mut self.extent.write_mdl(depth)?);
        for en in &self.anim_extents {
            lines.push(F!("{indent}Anim {{"));
            lines.append(&mut en.write_mdl(depth + 1)?);
            lines.push(F!("{indent}}}"));
        }

        lines.push_if_nneg1(&F!("{indent}MaterialID"), &self.material_id);
        lines.push_if_nneg1(&F!("{indent}SelectionGroup"), &self.sel_group);
        yes!(self.sel_type != 0, lines.push(F!("{indent}Unselectable,")));

        return Ok(lines);
    }
}

// region: GeosetAnim

#[derive(Dbg, SmartDefault)]
pub struct GeosetAnim {
    #[default = 1.0]
    pub alpha: f32,
    #[dbg(fmt = "{:?}")]
    pub flags: GeosetAnimFlags,
    #[dbg(formatter = "fmtx")]
    #[default(Vec3::ONE)]
    pub color: Vec3, // RGB
    pub geoset_id: i32,

    #[dbg(formatter = "fmtxx")]
    pub alpha_anim: Option<Animation<f32>>,
    #[dbg(formatter = "fmtxx")]
    pub color_anim: Option<Animation<Vec3>>, // BGR
}
bitflags! {
    #[derive(Debug, Default, Clone, Copy, PartialEq, Eq, PartialOrd, Ord, Hash)]
    pub struct GeosetAnimFlags : u32 {
        const DropShadow = 1 << 0;
        const UseColor = 1 << 1;
    }
}

impl GeosetAnim {
    pub const ID: u32 = MdlxMagic::GEOA;
    const ID_ALPHA: u32 = MdlxMagic::KGAO;
    const ID_COLOR: u32 = MdlxMagic::KGAC;

    pub fn read_mdx(cur: &mut Cursor<&Vec<u8>>) -> Result<Self, MyError> {
        let mut this = Build!();
        this.alpha = cur.readx()?;
        this.flags = GeosetAnimFlags::from_bits_retain(cur.readx()?);
        this.color = cur.readx()?;
        this.geoset_id = cur.readx()?;
        while cur.left() >= 16 {
            match cur.read_be()? {
                Self::ID_ALPHA => this.alpha_anim = Some(Animation::read_mdx(cur)?),
                Self::ID_COLOR => this.color_anim = Some(Animation::read_mdx(cur)?),
                id => return ERR!("Unknown animation in {}: {} (0x{:08X})", TNAME!(), u32_to_ascii(id), id),
            }
        }
        return Ok(this);
    }

    pub fn write_mdx(&self, chunk: &mut MdxChunk) -> Result<(), MyError> {
        chunk.write(&self.calc_mdx_size())?;
        chunk.write(&self.alpha)?;
        chunk.write(&self.flags.bits())?;
        chunk.write(&self.color)?;
        chunk.write(&self.geoset_id)?;
        MdxWriteAnim!(chunk,
            Self::ID_ALPHA => self.alpha_anim,
            Self::ID_COLOR => self.color_anim,
        );
        return Ok(());
    }
    pub fn calc_mdx_size(&self) -> u32 {
        let mut sz: u32 = 28; // sz + alpha + flags + color + geoset_id
        sz += self.alpha_anim.calc_mdx_size();
        sz += self.color_anim.calc_mdx_size();
        return sz;
    }

    pub fn read_mdl(block: &MdlBlock) -> Result<Self, MyError> {
        block.unexpect_frames()?;
        let mut this = Build!();
        for f in &block.fields {
            match_istr!(f.name.as_str(),
                "Alpha" => this.alpha = f.value.to()?,
                "UseColor" => this.flags |= f.expect_flag(GeosetAnimFlags::UseColor)?,
                "DropShadow" => this.flags |= f.expect_flag(GeosetAnimFlags::DropShadow)?,
                "Color" => {
                    this.color = f.value.to()?;
                    this.flags |= GeosetAnimFlags::UseColor;
                },
                "GeosetID" => this.geoset_id = f.value.to()?,
                _other => (),
            );
        }
        for f in &block.blocks {
            match_istr!(f.typ.as_str(),
                "Alpha" => this.alpha_anim = Some(Animation::read_mdl(f)?),
                "Color" => {
                    this.color_anim = Some(Animation::read_mdl(f)?);
                    this.flags |= GeosetAnimFlags::UseColor;
                },
                _other => (),
            );
        }
        if *mdl_rgb!() {
            this.color_anim = this.color_anim.map(|a| a.convert(|v| v.reverse()));
        }
        return Ok(this);
    }

    pub fn write_mdl(&self, depth: u8) -> Result<Vec<String>, MyError> {
        let indent = indent!(depth);
        let mut lines: Vec<String> = vec![];
        lines.push_if_nneg1(&F!("{indent}GeosetId"), &self.geoset_id);
        lines.push_if(self.flags.contains(GeosetAnimFlags::DropShadow), F!("{indent}DropShadow,"));

        MdlWriteAnimBoth!(lines, depth, "Alpha" => self.alpha_anim => 1.0 => self.alpha);
        if self.flags.contains(GeosetAnimFlags::UseColor) {
            if let Some(anim) = &self.color_anim {
                if *mdl_rgb!() {
                    MdlWriteAnim!(lines, depth, "Color" => anim.convert(|v| v.reverse()));
                } else {
                    MdlWriteAnim!(lines, depth, "Color" => anim);
                }
            } else {
                MdlWriteAnimStatic!(lines, depth, "Color" => self.color);
            }
        }

        return Ok(lines);
    }
}

// endregion
