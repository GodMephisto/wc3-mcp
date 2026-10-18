---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/main.rs"
ref: HEAD
---

use bitflags::bitflags;
use byteorder::{BigEndian, LittleEndian, ReadBytesExt, WriteBytesExt};
use chrono::{DateTime, Local};
use derive_debug::Dbg;
use glam::{Vec2, Vec3, Vec4};
use lazy_static::lazy_static;
use paste::paste;
use pest::Parser;
use pest::iterators::Pair;
use pest_derive::Parser;
use pretty_hex::*;
use regex::Regex;
use smart_default::SmartDefault;
use std::collections::HashMap;
use std::collections::HashSet;
use std::env;
use std::fmt::{Debug as stdDebug, Display, Formatter as stdFormatter, Result as stdResult};
use std::fs;
use std::io::{Cursor, Error as ioError, Read, Write};
use std::panic;
use std::path::{Path, PathBuf};
use std::process::ExitCode;
use walkdir::WalkDir;

mod cli;
mod data;
mod extends;
mod fields;
mod mdl;
mod mdx;
mod utils;
mod worker;

use cli::*;
use data::*;
use extends::*;
use fields::*;
use mdl::*;
use utils::*;
use worker::*;

lazy_static! {
    pub static ref StartTime: DateTime<Local> = Local::now();
}

fn _main() -> Result<(), MyError> {
    let cli = CLI::new();
    let mut worker = Worker::init();
    cli.execute(&mut worker)?;
    return worker.join();
}

fn main() -> ExitCode {
    if let Err(e) = _main() {
        elog!("{}", e);
        ExitCode::FAILURE
    } else {
        ExitCode::SUCCESS
    }
}
