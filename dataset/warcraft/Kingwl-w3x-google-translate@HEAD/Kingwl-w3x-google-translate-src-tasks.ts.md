---
type: vendor-doc
source_repo: "https://github.com/Kingwl/w3x-google-translate"
source_path: "src/tasks.ts"
ref: HEAD
---

import { translateApi } from "./api";
import { zip } from "./utils";
import type { NeedToBeTranslate, Task, TranslateOptions } from "./types";

function createPropertyTask(
  target: Record<string, string>,
  property: string
): Task {
  function getValue(): string {
    return target[property];
  }

  function updateValue(value: string) {
    target[property] = value;
  }

  return {
    getValue,
    updateValue,
  };
}

function createListPropertyTask(
  target: Record<string, string[]>,
  property: string,
  index: number
): Task {
  function getValue(): string {
    return target[property][index];
  }

  function updateValue(value: string) {
    target[property][index] = value;
  }

  return {
    getValue,
    updateValue,
  };
}

const keys: (keyof NeedToBeTranslate)[] = [
  "Name",
  "Bufftip",
  "Buffubertip",
  "Tip",
  "Ubertip",
  "tip",
  "ubertip",
  "untip",
  "unubertip",
];

function makeTask(obj: NeedToBeTranslate) {
  const results: Task[] = [];

  keys.forEach((key) => {
    const vlaue = obj[key];
    if (!vlaue) {
      return;
    }

    if (Array.isArray(vlaue)) {
      vlaue.forEach((_, i) => {
        results.push(
          createListPropertyTask(obj as Record<string, string[]>, key, i)
        );
      });
    }

    results.push(createPropertyTask(obj as Record<string, string>, key));
  });

  return results;
}

async function translateTask(tasks: Task[], options: TranslateOptions) {
  const texts = tasks.map((task) => task.getValue());

  try {
    const results = await translateApi(texts, "en", "zh-CN", options);

    zip(tasks, results).forEach(([task, result]) => {
      task.updateValue(result);
    });
  } catch (e) {
    console.error(e);
  }
}

async function batchRunTasks(
  tasks: Task[],
  options: TranslateOptions,
  report?: (size: number) => void
) {
  const { batchSize } = options.taskOptions;
  const runningTasks = tasks.splice(tasks.length - batchSize - 1, batchSize);
  await translateTask(runningTasks, options);
  report?.(runningTasks.length);
}

async function parallelBatchRunTasks(
  tasks: Task[],
  options: TranslateOptions,
  report?: (size: number) => void
) {
  const { parallel } = options.taskOptions;

  const workers = Array.from({ length: parallel }).map(() =>
    Promise.resolve().then(async () => {
      while (tasks.length) {
        await batchRunTasks(tasks, options, report);
      }
    })
  );
  await Promise.all(workers);
}

export async function parallelBatchTranslateNeedToBeTranslateObject(
  iniObject: Record<string, unknown>,
  options: TranslateOptions,
  report?: (size: number) => void
) {
  const { beforeTasks, afterTasks } = options.taskOptions;

  const tasks: Task[] = [];
  for (const value of Object.values(iniObject)) {
    const needToBeTranslate = value as NeedToBeTranslate;
    tasks.push(...makeTask(needToBeTranslate));
  }

  beforeTasks?.(tasks);

  await parallelBatchRunTasks(tasks, options, report);

  afterTasks?.();
}
