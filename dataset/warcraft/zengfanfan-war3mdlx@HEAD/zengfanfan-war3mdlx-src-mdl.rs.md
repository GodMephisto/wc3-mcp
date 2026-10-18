---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/mdl.rs"
ref: HEAD
---

use crate::*;

macro_rules! MdlReadType1 {
    ($block:expr, $( $ty:ty => $var:expr ),+ $(,)?) => {
        $(if $block.typ == stringify!($ty) {
            $var = <$ty>::read_mdl(&$block).or_else(|e| ERR!("{}: {}", TNAME!($ty), e))?;
            return Ok(());
        })+
    };
}
macro_rules! MdlReadType2 {
    ($block:expr, $( $ty:ty => $name:expr => $var:expr ),+ $(,)?) => {
        $(if $block.typ == F!("{}s", stringify!($ty)) {
            $block.unexpect_fields()?;
            $block.unexpect_frames()?;
            for a in $block.blocks.iter() {
                if a.typ == $name {
                    let name = yesno!(a.name.is_empty(), $var.len().s(), F!("{:?}",a.name));
                    $var.push(<$ty>::read_mdl(a)
                    .or_else(|e| ERR!("{}[{}]: {}", TNAME!($ty), name, e))?);
                } else {
                    return a.unexpect();
                }
            }
            return Ok(());
        })+
    };
}
macro_rules! MdlReadType3 {
    ($block:expr, $( $ty:ty => $var:expr ),+ $(,)?) => {
        $(if $block.typ == stringify!($ty) {
            let name = yesno!($block.name.is_empty(), $var.len().s(), F!("{:?}",$block.name));
            $var.push(<$ty>::read_mdl(&$block).or_else(|e| ERR!("{}[{}]: {}", TNAME!($ty), name, e))?);
            return Ok(());
        })+
    };
}
macro_rules! MdlReadType4 {
    ($block:expr, $( $ty:ty => $var:expr ),+ $(,)?) => {
        $(if $block.typ == F!("{}s", stringify!($ty)) {
            $block.unexpect_frames()?;
            $block.unexpect_blocks()?;
            for a in $block.fields.iter() {
                $var.push(<$ty>::read_mdl(a).or_else(|e| ERR!("{}: {}", TNAME!($ty), e))?);
            }
            return Ok(());
        })+
    };
}

#[macro_export]
macro_rules! MdlWriteType1 {
    ($lines:ident, $depth:expr, $( $var:expr ),+ $(,)?) => {
        $( $lines.append(&mut $var.write_mdl($depth)?); )+
    };
}
#[macro_export]
macro_rules! MdlWriteType2 {
    ($lines:ident, $depth:expr, $( $name:expr => $var:expr ),+ $(,)?) => {
        $(if !$var.is_empty() {
            let indent = indent!($depth);
            $lines.push(F!("{indent}{} {} {{", $name, $var.len()));
            for a in $var.iter() {
                MdlWriteType1!($lines, $depth+1, a);
            }
            $lines.push(F!("{indent}}}"));
        })+
    };
}
#[macro_export]
macro_rules! MdlWriteType3 {
    ($lines:ident, $depth:expr, $( $name:expr => $var:expr ),+ $(,)?) => {
        $(if !$var.is_empty() {
            for a in $var.iter() {
                $lines.push(F!("{} {{", $name));
                MdlWriteType1!($lines, $depth+1, a);
                $lines.push(F!("}}"));
            }
        })+
    };
}
#[macro_export] // Nodes
macro_rules! MdlWriteType4 {
    ($lines:ident, $depth:expr, $member:expr, $( $name:expr => $var:expr ),+ $(,)?) => {
        $(if !$var.is_empty() {
            for a in $var.iter() {
                paste!{ $lines.push(F!("{} \"{}\" {{", $name, a.$member.escape())); }
                MdlWriteType1!($lines, $depth+1, a);
                $lines.push(F!("}}"));
            }
        })+
    };
}

#[derive(Parser)]
#[grammar = "mdl.pest"]
pub struct MdlParser;

impl MdlxData {
    pub fn write_mdl(&mut self, path: &Path) -> Result<(), MyError> {
        let mut lines: Vec<String> = vec![];
        Self::write_mdl_comment(&mut lines, path);

        MdlWriteType1!(lines, 0, self.version, self.model);
        MdlWriteType2!(lines, 0,
            "Sequences"         => self.sequences,
            "GlobalSequences"   => self.globalseqs,
            "Textures"          => self.textures,
            "Materials"         => self.materials,
            "TextureAnims"      => self.texanims,
            "PivotPoints"       => self.pivot_points,
        );
        MdlWriteType3!(lines, 0,
            "Geoset"            => self.geosets,
            "GeosetAnim"        => self.geoanims,
        );
        MdlWriteType4!(lines, 0, base.name,
            "Bone"              => self.bones,
            "Light"             => self.lights,
            "Helper"            => self.helpers,
            "Attachment"        => self.attachments,
            "ParticleEmitter"   => self.particle_emitters,
            "ParticleEmitter2"  => self.particle_emitters2,
            "RibbonEmitter"     => self.ribbon_emitters,
            "EventObject"       => self.eventobjs,
            "CollisionShape"    => self.collisions,
        );
        MdlWriteType4!(lines, 0, name, "Camera" => self.cameras );

        let line_ending = line_ending!();
        let text = lines.join(line_ending) + line_ending;
        return Ok(std::fs::write(path, text)?);
    }
    fn write_mdl_comment(lines: &mut Vec<String>, path: &Path) {
        let width = 48;
        let comments = vec![
            F!("{:?}", path.file_name().unwrap()),
            F!("Generated by {} {}", env!("CARGO_PKG_NAME"), env!("CARGO_PKG_VERSION")),
            StartTime.format("%Y-%m-%d %H:%M:%S (%Z)").to_string(),
            F!("{}", env!("CARGO_PKG_HOMEPAGE")),
        ];
        lines.push(F!("//+{}+", "-".repeat(width)));
        for c in comments {
            lines.push(F!("//|{:^w$}|", c, w = width));
        }
        lines.push(F!("//+{}+", "-".repeat(width)));
    }

    pub fn read_mdl(input: &str) -> Result<Self, MyError> {
        let mdl = MdlParser::parse(Rule::file, input).map_err(|e| F!("Failed to parse mdl: {}", e))?;
        let mut this = MdlxData::default();

        for pair in mdl {
            if let Rule::file = pair.as_rule() {
                for p in pair.into_inner() {
                    if let Rule::block = p.as_rule() {
                        this.parse_mdl_block(MdlBlock::from(p, "")?)?;
                    }
                }
                break; // only 1 [file] rule
            }
        }

        return Ok(this);
    }

    fn parse_mdl_block(&mut self, block: MdlBlock) -> Result<(), MyError> {
        MdlReadType1!(block,
            Version     => self.version,
            Model       => self.model,
        );
        MdlReadType2!(block,
            Sequence    => "Anim"       => self.sequences,
            Texture     => "Bitmap"     => self.textures,
            TextureAnim => "TVertexAnim"=> self.texanims,
            Material    => "Material"   => self.materials,
        );
        MdlReadType3!(block,
            Geoset          => self.geosets,
            GeosetAnim      => self.geoanims,
            Bone            => self.bones,
            Light           => self.lights,
            Helper          => self.helpers,
            Attachment      => self.attachments,
            ParticleEmitter => self.particle_emitters,
            ParticleEmitter2=> self.particle_emitters2,
            RibbonEmitter   => self.ribbon_emitters,
            EventObject     => self.eventobjs,
            CollisionShape  => self.collisions,
            Camera          => self.cameras,
        );
        MdlReadType4!(block,
            GlobalSequence  => self.globalseqs,
            PivotPoint      => self.pivot_points,
        );
        return block.unexpect();
    }
}
