---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/cli.rs"
ref: HEAD
---

use crate::*;
use clap::{ArgAction, Parser};

// region: Args

lazy_static! {
    pub static ref ARGS: Args = Args::parse();
}

#[derive(Debug, Parser, Clone)]
#[command(author, version, about, after_help = env!("CARGO_PKG_HOMEPAGE"))]
pub struct Args {
    #[arg(hide = true)]
    pub input: String,
    #[arg(hide = true)]
    pub output: Option<String>,

    #[arg(long, short = '1', help = "Convert *.mdl to *.mdx")]
    pub mdl2x: bool,
    #[arg(long, short = '2', help = "Convert *.mdx to *.mdl")]
    pub mdx2l: bool,
    #[arg(long, short = 'B', help = "Make sure colors are in RGB order in mdl files")]
    pub mdl_rgb: bool,

    #[arg(long, short = 'F', help = "Put output files in one directory and ignore hierarchy")]
    pub flat: bool,
    #[arg(long, short = 'f', help = "Overwrite existing output files [default: skip]")]
    pub overwrite: bool,
    #[arg(long, short = 'e', help = "Stop walking the directory hierarchy when an error occurs")]
    pub stop_on_error: bool,
    #[arg(
        long,
        short = 'd',
        default_value_t = 255,
        value_name = "0..255",
        help = "Max depth of directory traversal"
    )]
    pub max_depth: u8,

    #[arg(
        long,
        short = 'p',
        default_value_t = 4,
        value_name = "0..255",
        help = "Max precision of decimal numbers when converted to text",
    )]
    pub precision: u8,
    #[arg(
        long,
        short = 'n',
        value_name = "CR|LF|CRLF",
        value_parser = validate_line_ending,
        default_value = "CRLF",
        help = "Used when writing text files",
    )]
    pub line_ending: String,
    #[arg(
        long,
        short = 'i',
        value_name = "Ns|Nt",
        value_parser = validate_indent,
        default_value = "1t",
        help = "Used when writing text files (e.g. 1t: one tab, 4s: four spaces)",
    )]
    pub indent: String,

    #[arg(long, short, help = "Do not print log messages")]
    pub quiet: bool,
    #[arg(long, short, action = ArgAction::Count, help = "Print verbose log messages (-vv very verbose)")]
    pub verbose: u8,
}

fn validate_line_ending(s: &str) -> Result<String, String> {
    match_istr!(s,
        "CR" => Ok("\r".s()),
        "LF" => Ok("\n".s()),
        "CRLF" => Ok("\r\n".s()),
        _other => Err("must be CR, LF or CRLF".s())
    )
}
fn validate_indent(s: &str) -> Result<String, String> {
    let re = Regex::new(r"^[0-9]{1,4}[st]$").unwrap();
    if re.is_match(s) {
        let len = s.len();
        let n = &s[..len - 1];
        let c = s.as_bytes()[len - 1] as char;
        let c = yesno!(c == 't', '\t', ' ');
        Ok(c.s().repeat(n.parse().unwrap()))
    } else {
        Err("must be 1~4 digits followed by 's' or 't'".s())
    }
}

// endregion
// region: CLI

#[derive(PartialEq)]
enum CheckResult {
    Ok,
    ExpectFileDir,
    ExpectMDL,
    ExpectMDX,
    ExpectMDLX,
}

#[derive(Debug)]
pub struct CLI;

impl CLI {
    pub fn new() -> Self {
        Self {}
    }

    pub fn execute(&self, worker: &mut Worker) -> Result<(), MyError> {
        let input = PathBuf::from(&ARGS.input);
        match self.check_input(&input) {
            CheckResult::ExpectFileDir => EXIT1!("Not an existing file or directory: {:?}", input),
            CheckResult::ExpectMDLX => EXIT1!("Invalid input: {:?}, expect *.mdl or *.mdx", input),
            CheckResult::ExpectMDL => EXIT1!("Invalid input: {:?}, expect *.mdl", input),
            CheckResult::ExpectMDX => EXIT1!("Invalid input: {:?}, expect *.mdx", input),
            CheckResult::Ok => match input.is_dir() {
                false => self.handle_file(worker, input),
                true => self.handle_dir(worker, input),
            },
        }
    }

    fn guess_outext(&self, inext: &str) -> &str {
        if ARGS.mdl2x {
            "mdx"
        } else if ARGS.mdx2l {
            "mdl"
        } else if inext.eq_icase("mdl") {
            "mdx"
        } else {
            "mdl"
        }
    }

    fn handle_file(&self, worker: &mut Worker, input: PathBuf) -> Result<(), MyError> {
        let mut output = match &ARGS.output {
            Some(o) => PathBuf::from(o),
            None => input.with_extension(self.guess_outext(input.ext())),
        };

        let opath = output.display().to_string();
        match self.check_output(&output) {
            CheckResult::ExpectFileDir => EXIT1!("Not a file or directory: {}", opath),
            CheckResult::ExpectMDLX => EXIT1!("Invalid path: {}, expect *.mdl or *.mdx", opath),
            CheckResult::ExpectMDL => EXIT1!("Invalid path: {}, expect *.mdl", opath),
            CheckResult::ExpectMDX => EXIT1!("Invalid path: {}, expect *.mdx", opath),
            _ok => yes!(output.is_dir(), output = output.join(input.base_name())),
        };

        yes!(input.same_as(&output), EXIT!("Input and output are the same, do nothing."));

        self.process_file(worker, input, output);
        EXIT!();
    }

    fn handle_dir(&self, worker: &mut Worker, input: PathBuf) -> Result<(), MyError> {
        let output = match ARGS.output.as_ref() {
            Some(s) => PathBuf::from(s),
            None => input.to_path_buf(),
        };
        if !output.is_dir() {
            EXIT1!("Output is not an existing directory: {}", output.fmtx());
        }

        let max_depth = ARGS.max_depth as usize + 1;
        for entry in WalkDir::new(&input).max_depth(max_depth).into_iter().filter_map(|e| e.ok()) {
            let ifile = entry.into_path();
            if !ifile.is_file() || self.check_input(&ifile) != CheckResult::Ok {
                continue;
            }

            let ofile = match ARGS.flat {
                true => output.join(ifile.base_name()),
                false => output.join(ifile.relative_to(&input)),
            }
            .with_extension(self.guess_outext(ifile.ext()));

            self.process_file(worker, ifile, ofile);
        }

        EXIT!();
    }

    fn check_input(&self, path: &Path) -> CheckResult {
        let ext = path.ext_lower();
        if path.is_dir() {
            CheckResult::Ok
        } else if path.is_file() {
            if ARGS.mdl2x && ext != "mdl" {
                CheckResult::ExpectMDL
            } else if ARGS.mdx2l && ext != "mdx" {
                CheckResult::ExpectMDX
            } else if ext != "mdl" && ext != "mdx" {
                CheckResult::ExpectMDLX
            } else {
                CheckResult::Ok
            }
        } else {
            CheckResult::ExpectFileDir
        }
    }

    fn check_output(&self, path: &Path) -> CheckResult {
        let ext = path.ext_lower();
        if path.is_dir() {
            CheckResult::Ok
        } else {
            if ARGS.mdl2x && ext != "mdx" {
                CheckResult::ExpectMDX
            } else if ARGS.mdx2l && ext != "mdl" {
                CheckResult::ExpectMDL
            } else if ext != "mdl" && ext != "mdx" {
                CheckResult::ExpectMDLX
            } else {
                CheckResult::Ok
            }
        }
    }

    fn process_file(&self, worker: &mut Worker, input: PathBuf, output: PathBuf) {
        if output.exists() && !ARGS.overwrite {
            log!("Skipped existing output: {}", output.fmtx());
            worker.skip_job();
        } else {
            log!("Converting {} -> {} ...", input.fmtx(), output.fmtx());
            worker.add_job(input, output);
        }
    }
}

// endregion
// region: [global] args

macro_rules! getter {
    ($($field:ident),+) => {
        $(
            #[macro_export]
            macro_rules! $field {
                () => {
                    &ARGS.$field
                };
            }
        )+
    };
}

getter!(line_ending, precision, stop_on_error, mdl_rgb);

// endregion
// region: [global] log level

lazy_static! {
    pub static ref _LOG_LEVEL: LogLevel = init_log_level();
}

fn init_log_level() -> LogLevel {
    yesno!(
        ARGS.quiet,
        LogLevel::Warn,
        match ARGS.verbose {
            1 => LogLevel::Verbose,
            2 => LogLevel::Verbose2,
            3.. => LogLevel::Verbose3,
            _ => LogLevel::default(),
        }
    )
}

pub fn log_level() -> &'static LogLevel {
    &_LOG_LEVEL
}

// endregion
// region: [global] indent

lazy_static! {
    static ref INDENTS: HashMap<u8, String> = init_indents();
}

fn init_indents() -> HashMap<u8, String> {
    let mut m = HashMap::new();
    let s = &ARGS.indent;
    for i in 0..10 {
        m.insert(i, s.repeat(i as usize));
    }
    return m;
}

pub fn _indent(depth: u8) -> &'static str {
    match &depth {
        0 => "",
        1 => ARGS.indent.as_str(),
        d => INDENTS.get(d).expect(&F!("Invalid indent depth: {d}")),
    }
}

#[macro_export]
macro_rules! indent {
    () => {
        Args.indent
    };
    ($depth:expr) => {
        _indent($depth)
    };
}

// endregion
