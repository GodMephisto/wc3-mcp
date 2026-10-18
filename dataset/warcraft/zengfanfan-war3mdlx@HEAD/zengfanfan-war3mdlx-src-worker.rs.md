---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/worker.rs"
ref: HEAD
---

use crate::*;
use std::sync::atomic::{AtomicBool, Ordering};
use std::sync::mpsc::{Receiver, Sender, channel};
use std::sync::{Arc, Mutex};
use std::thread;

// region: Job

#[derive(Debug)]
struct Job {
    input: PathBuf,
    output: PathBuf,
}

#[derive(Debug)]
enum JobResult {
    Ok,
    Err,
}

// endregion
// region: StopSignal

#[derive(Clone, Default)]
struct StopSignal {
    signal: Arc<AtomicBool>,
}
impl StopSignal {
    pub fn set(&self) {
        self.signal.store(true, Ordering::SeqCst)
    }
    pub fn get(&self) -> bool {
        self.signal.load(Ordering::SeqCst)
    }
}
lazy_static! {
    static ref STOP: StopSignal = StopSignal { signal: Arc::new(AtomicBool::new(false)) };
}

// endregion

#[derive(Default)]
pub struct Worker {
    start: i128,
    total: i32,
    ok: i32,
    skip: i32,
    fail: i32,
    workers: Vec<thread::JoinHandle<()>>,
    jobtx: Option<Sender<Job>>,
    resrx: Option<Receiver<JobResult>>,
}

impl Worker {
    pub fn init() -> Self {
        let (jobtx, jobrx) = channel::<Job>();
        let (restx, resrx) = channel::<JobResult>();
        let jobrx = Arc::new(Mutex::new(jobrx)); // to share it across threads
        let mut this = Build! { start: timestamp_ms(), jobtx: Some(jobtx), resrx: Some(resrx) };
        let stop_signal = STOP.clone();

        let old_hook = panic::take_hook();
        panic::set_hook(Box::new(move |info| {
            STOP.set();
            old_hook(info);
        }));

        let ncpus = std::thread::available_parallelism().map(|n| n.get()).unwrap_or(1).max(1);
        for id in 0..ncpus {
            let jobrx = Arc::clone(&jobrx);
            let restx = restx.clone();
            let stop = stop_signal.clone();
            let handle = thread::spawn(move || Self::thread(id, jobrx, restx, stop));
            this.workers.push(handle);
        }

        return this;
    }

    pub fn skip_job(&mut self) {
        self.total += 1;
        self.skip += 1;
    }
    pub fn add_job(&mut self, input: PathBuf, output: PathBuf) {
        yes!(STOP.get(), return);
        self.total += 1;
        if let Some(tx) = &self.jobtx {
            _ = tx.send(Job { input, output })
        }
    }

    fn thread(_id: usize, rx: Arc<Mutex<Receiver<Job>>>, tx: Sender<JobResult>, stop: StopSignal) {
        let stop_on_error = *stop_on_error!();
        let mut dead = false;
        while !(dead || stop.get()) {
            // *Q: why using let instead of directly assigning?
            // *A: To release lock as soon as possible.
            let job = {
                let receiver = rx.lock().unwrap();
                receiver.recv() // block until a job is available
            };
            dead = match job {
                Err(_) => true, // caused by drop(sender)
                Ok(job) => {
                    match MdlxData::read(&job.input).and_then(|mut a| a.write(&job.output)) {
                        Ok(_) => tx.send(JobResult::Ok).is_err(),
                        Err(e) => {
                            elog!("{}", e);
                            yes!(stop_on_error, stop.set());
                            tx.send(JobResult::Err).is_err()
                        },
                    }
                },
            };
        }
    }

    fn handle(&mut self) {
        let stop_on_error = *stop_on_error!();
        if let Some(rx) = &self.resrx {
            while let Ok(result) = rx.recv() {
                if let JobResult::Ok = result {
                    self.ok += 1;
                } else {
                    self.fail += 1;
                    yes!(stop_on_error, break);
                }
            }
        }
    }

    pub fn join(mut self) -> Result<(), MyError> {
        self.jobtx = None; // close the sender
        self.handle(); // ?: do not return error, just log and keep going
        self.resrx = None; // close the receiver

        for h in self.workers {
            // ?: do not return error, just ignore and keep going
            _ = h.join();
        }

        let time = timestamp_ms() - self.start;
        let (ok, skip, error) = (self.ok, self.skip, self.fail);
        print!("Converted {ok} files");
        yes!(skip > 0, print!(", {skip} skipped"));
        yes!(error > 0, print!(", {error} errors"));
        println!(", cost {}.{:03}s.", time / 1000, time % 1000);

        return Ok(());
    }
}
