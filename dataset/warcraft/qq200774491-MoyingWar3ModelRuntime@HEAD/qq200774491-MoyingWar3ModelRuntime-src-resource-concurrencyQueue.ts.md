---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/resource/concurrencyQueue.ts"
ref: HEAD
---

interface QueueItem {
  signal: AbortSignal;
  run: () => Promise<void>;
  reject: (reason: unknown) => void;
}

export class ConcurrencyQueue {
  private readonly pending: QueueItem[] = [];
  private active = 0;
  private disposed = false;

  constructor(private readonly limit: number) {
    if (!Number.isInteger(limit) || limit < 1) {
      throw new RangeError('Concurrency limit must be a positive integer.');
    }
  }

  schedule<T>(task: () => Promise<T>, signal: AbortSignal): Promise<T> {
    if (this.disposed) return Promise.reject(abortError('Resource queue is disposed.'));

    return new Promise<T>((resolve, reject) => {
      const item: QueueItem = {
        signal,
        reject,
        run: async () => {
          if (signal.aborted) throw signal.reason ?? abortError();
          resolve(await task());
        },
      };
      this.pending.push(item);
      this.pump();
    });
  }

  dispose(reason: unknown = abortError('Resource queue is disposed.')): void {
    if (this.disposed) return;
    this.disposed = true;
    for (const item of this.pending.splice(0)) item.reject(reason);
  }

  private pump(): void {
    while (!this.disposed && this.active < this.limit) {
      const item = this.pending.shift();
      if (!item) return;
      if (item.signal.aborted) {
        item.reject(item.signal.reason ?? abortError());
        continue;
      }

      this.active += 1;
      void item.run().catch(item.reject).finally(() => {
        this.active -= 1;
        this.pump();
      });
    }
  }
}

export function abortError(message = 'The operation was aborted.'): DOMException {
  return new DOMException(message, 'AbortError');
}
