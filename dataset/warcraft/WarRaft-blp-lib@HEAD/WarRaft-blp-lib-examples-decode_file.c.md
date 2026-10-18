---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: "examples/decode_file.c"
ref: HEAD
---

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>
#include "../include/blp_lib.h"

int main(int argc, char** argv) {
    if (argc < 4) {
        fprintf(stderr, "Usage: %s <input.blp> <mip_index> <output.png> [--extract-jpeg <output.jpg>]\n", argv[0]);
        return 2;
    }

    const char* in_path = argv[1];
    int mip_index = atoi(argv[2]);
    const char* out_png = argv[3];

    BlpResult r = blp_decode_mip_to_png_from_file(in_path, (uint32_t)mip_index, out_png);
    if (r != BLP_SUCCESS) {
        fprintf(stderr, "decode failed: %d\n", r);
        return 1;
    }
    printf("Saved PNG: %s\n", out_png);

    if (argc >= 6 && strcmp(argv[4], "--extract-jpeg") == 0) {
        const char* out_jpg = argv[5];
        r = blp_extract_mip_to_jpg_from_file(in_path, (uint32_t)mip_index, out_jpg);
        if (r == BLP_SUCCESS) {
            printf("Saved raw JPEG: %s\n", out_jpg);
        } else {
            fprintf(stderr, "extract jpeg failed: %d\n", r);
        }
    }

    return 0;
}
