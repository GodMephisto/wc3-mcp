---
type: vendor-doc
source_repo: "https://github.com/Kingwl/w3x-google-translate"
source_path: "src/types.ts"
ref: HEAD
---

export interface TranslateData {
  q: string[];
  source: string;
  target: string;
  format: "text";
}

export interface TranslateResp {
  data: {
    translations: {
      translatedText: string;
    }[];
  };
}

export interface ClientProxyOptions {
  host: string;
  port: number;
}

export interface TaskOptions {
  parallel: number;
  batchSize: number;

  beforeTasks?: (tasks: readonly Task[]) => void;
  afterTasks?: () => void;
}

export interface TranslateOptions {
  key: string;
  proxy?: ClientProxyOptions;
  taskOptions: TaskOptions;
}

type MaybeArray<T> = T | T[];

export interface NeedToBeTranslate {
  Name?: string;

  Tip?: MaybeArray<string>;
  tip?: MaybeArray<string>;

  Ubertip?: MaybeArray<string>;
  ubertip?: MaybeArray<string>;

  Bufftip?: MaybeArray<string>;
  Buffubertip?: MaybeArray<string>;

  untip?: MaybeArray<string>;
  unubertip?: MaybeArray<string>;
}

export interface Task {
  getValue(): string;
  updateValue(value: string): void;
}
