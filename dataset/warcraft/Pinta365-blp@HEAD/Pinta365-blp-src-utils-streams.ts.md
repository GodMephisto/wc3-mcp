---
type: vendor-doc
source_repo: "https://github.com/Pinta365/blp"
source_path: "src/utils/streams.ts"
ref: HEAD
---

/**
 * Converts a Uint8Array to a ReadableStream for streaming APIs.
 * @param u8 - The Uint8Array to convert.
 * @returns A ReadableStream of Uint8Array.
 */
export function uint8ArrayToStream(u8: Uint8Array): ReadableStream<Uint8Array> {
    return new ReadableStream({
        start(controller) {
            controller.enqueue(u8);
            controller.close();
        },
    });
}
