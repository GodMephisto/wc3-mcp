---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/resource/asyncCache.ts"
ref: HEAD
---

export class AsyncCache<T> {
  private readonly values = new Map<string, T>();
  private readonly pending = new Map<string, Promise<T>>();

  getOrCreate(key: string, factory: () => Promise<T>): Promise<T> {
    const value = this.values.get(key);
    if (value !== undefined) return Promise.resolve(value);

    const existing = this.pending.get(key);
    if (existing) return existing;

    const promise = factory().then((created) => {
      this.values.set(key, created);
      return created;
    }).finally(() => {
      this.pending.delete(key);
    });
    this.pending.set(key, promise);
    return promise;
  }

  clear(dispose?: (value: T) => void): void {
    if (dispose) {
      for (const value of this.values.values()) dispose(value);
    }
    this.values.clear();
    this.pending.clear();
  }
}
