---
type: vendor-doc
source_repo: "https://github.com/Kingwl/w3x-google-translate"
source_path: "src/api.ts"
ref: HEAD
---

import axios from "axios";
import type {
  ClientProxyOptions,
  TranslateOptions,
  TranslateData,
  TranslateResp,
} from "./types";

function makeAxiosClient(options?: ClientProxyOptions) {
  if (options) {
    return axios.create({
      proxy: {
        protocol: "http",
        host: options.host,
        port: options.port,
      },
    });
  }

  return axios.create();
}

export async function translateApi<T extends string[]>(
  texts: T,
  source: string,
  target: string,
  options: TranslateOptions
): Promise<T> {
  const { key, proxy } = options;

  const data: TranslateData = {
    q: texts,
    source,
    target,
    format: "text",
  };

  const client = makeAxiosClient(proxy);

  const resp = await client.post<TranslateResp>(
    "https://translation.googleapis.com/language/translate/v2",
    data,
    { params: { key } }
  );
  return resp.data.data.translations.map((x) => x.translatedText) as T;
}
