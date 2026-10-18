---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: "examples/encode_file.c"
ref: HEAD
---

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>
#include "../include/blp_lib.h"

static int parse_flags(const char* s, uint8_t* out, size_t max, size_t* out_len) {
    size_t n = 0;
    const char* p = s;
    while (*p && n < max) {
        if (*p == '0' || *p == '1') {
            out[n++] = (uint8_t)(*p - '0');
            p++;
        } else if (*p == ',' || *p == ' ' || *p == ';') {
            p++;
        } else {
            return -1;
        }
    }
    *out_len = n;
    return 0;
}

int main(int argc, char** argv) {
    if (argc < 3) {
        fprintf(stderr, "Usage: %s <input_image> <output.blp> [--quality N] [--mips N] [--flags 1,0,1,...]\n", argv[0]);
        return 2;
    }

    const char* in_path = argv[1];
    const char* out_path = argv[2];
    int quality = 90;
    int mips = 1;
    uint8_t flags[16] = {0};
    size_t flags_len = 0;
    int use_flags = 0;

    for (int i = 3; i < argc; i++) {
        if (strcmp(argv[i], "--quality") == 0 && i + 1 < argc) {
            quality = atoi(argv[++i]);
        } else if (strcmp(argv[i], "--mips") == 0 && i + 1 < argc) {
            mips = atoi(argv[++i]);
        } else if (strcmp(argv[i], "--flags") == 0 && i + 1 < argc) {
            if (parse_flags(argv[++i], flags, 16, &flags_len) != 0) {
                fprintf(stderr, "Invalid --flags format. Use like: 1,1,0,1\n");
                return 2;
            }
            use_flags = 1;
        }
    }

    BlpResult r;
    if (use_flags) {
        r = blp_encode_file_to_blp_with_flags(in_path, out_path, (uint8_t)quality, flags, (uint32_t)flags_len);
    } else {
        r = blp_encode_file_to_blp(in_path, out_path, (uint8_t)quality, (uint32_t)mips);
    }

    if (r != BLP_SUCCESS) {
        fprintf(stderr, "encode failed: %d\n", r);
        return 1;
    }
    printf("Saved: %s\n", out_path);
    return 0;
}
