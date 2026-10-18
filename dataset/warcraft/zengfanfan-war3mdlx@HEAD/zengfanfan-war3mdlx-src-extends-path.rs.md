---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/extends/path.rs"
ref: HEAD
---

use crate::*;
use path_clean::PathClean;

pub trait _ExtendPath {
    fn ext(&self) -> &str;
    fn ext_lower(&self) -> String;
    fn base_name(&self) -> &str;
    fn same_as(&self, other: &Self) -> bool;
    fn shorten(&self, max_len: usize) -> String;
    fn fmtx(&self) -> String;
    fn relative_to(&self, other: &Self) -> &Self;
}

impl _ExtendPath for Path {
    fn ext(&self) -> &str {
        self.extension().and_then(|s| s.to_str()).unwrap_or("")
    }
    fn ext_lower(&self) -> String {
        self.ext().to_lowercase()
    }

    fn base_name(&self) -> &str {
        match self.file_name() {
            Some(s) => s.to_str().unwrap_or(""),
            None => "",
        }
    }

    fn same_as(&self, other: &Self) -> bool {
        let ca = fs::canonicalize(self).ok();
        let cb = fs::canonicalize(other).ok();
        if let (Some(pa), Some(pb)) = (ca, cb) {
            return pa == pb;
        }

        let pa = self.clean();
        let pb = other.clean();

        #[cfg(windows)]
        let pa = PathBuf::from(pa.to_string_lossy().to_lowercase());
        #[cfg(windows)]
        let pb = PathBuf::from(pb.to_string_lossy().to_lowercase());

        return pa == pb;
    }

    fn shorten(&self, max_len: usize) -> String {
        let s = self.to_string_lossy();
        yes!(s.len() <= max_len, return s.to_string());

        let path = self.clean();
        let s = path.to_string_lossy();
        yes!(s.len() <= max_len, return s.to_string());

        let ellipsis = "...";
        yes!(max_len <= ellipsis.len(), return ellipsis.to_string());
        let keep = max_len - ellipsis.len();

        let left = keep / 2;
        let right = keep - left;

        let start = &s[..left];
        let end = &s[s.len() - right..];

        return F!("{start}{ellipsis}{end}");
    }

    fn fmtx(&self) -> String {
        fmtx(&self.shorten(60))
    }

    fn relative_to(&self, other: &Self) -> &Self {
        self.strip_prefix(other).unwrap_or(self)
    }
}
