---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: "examples/decode_dir.c"
ref: HEAD
---

#define _XOPEN_SOURCE 700
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>
#include <strings.h>
#include <dirent.h>
#include <sys/stat.h>
#include <unistd.h>
#include <limits.h>
#include "../include/blp_lib.h"

static int is_blp(const char* name) {
    const char* dot = strrchr(name, '.');
    return dot && (strcasecmp(dot, ".blp") == 0);
}

static void replace_ext(char* path, size_t cap, const char* new_ext) {
    char* dot = strrchr(path, '.');
    if (dot) *dot = '\0';
    strncat(path, new_ext, cap - strlen(path) - 1);
}

static int ensure_dir_for(const char* file_path) {
    char tmp[PATH_MAX];
    strncpy(tmp, file_path, sizeof(tmp));
    tmp[sizeof(tmp)-1] = '\0';
    char* slash = strrchr(tmp, '/');
    if (!slash) return 0;
    *slash = '\0';
    char cmd[PATH_MAX + 32];
    snprintf(cmd, sizeof(cmd), "mkdir -p '%s'", tmp);
    return system(cmd);
}

static int process_dir(const char* in_root, const char* out_root, int mip_index, int extract_jpeg) {
    size_t root_len = strlen(in_root);
    char cmd[PATH_MAX + 32];
    snprintf(cmd, sizeof(cmd), "find '%s' -type f", in_root);
    FILE* pipe = popen(cmd, "r");
    if (!pipe) {
        perror("popen find");
        return 1;
    }
    char line[PATH_MAX];
    while (fgets(line, sizeof(line), pipe)) {
        size_t len = strlen(line);
        if (len && (line[len-1] == '\n' || line[len-1] == '\r')) line[len-1] = '\0';
        const char* full = line;
        if (!is_blp(full)) continue;
        const char* rel = full + root_len;
        if (*rel == '/') rel++;

        char out_png[PATH_MAX];
        snprintf(out_png, sizeof(out_png), "%s/%s", out_root, rel);
        replace_ext(out_png, sizeof(out_png), ".png");
        ensure_dir_for(out_png);

        BlpResult r = blp_decode_mip_to_png_from_file(full, (uint32_t)mip_index, out_png);
        if (r != BLP_SUCCESS) {
            fprintf(stderr, "decode failed: %s -> %s (%d)\n", full, out_png, r);
        } else {
            printf("Saved: %s\n", out_png);
        }

        if (extract_jpeg) {
            char out_jpg[PATH_MAX];
            snprintf(out_jpg, sizeof(out_jpg), "%s/%s", out_root, rel);
            replace_ext(out_jpg, sizeof(out_jpg), ".jpg");
            ensure_dir_for(out_jpg);
            r = blp_extract_mip_to_jpg_from_file(full, (uint32_t)mip_index, out_jpg);
            if (r == BLP_SUCCESS) {
                printf("Saved (jpg): %s\n", out_jpg);
            }
        }
    }
    pclose(pipe);
    return 0;
}

int main(int argc, char** argv) {
    if (argc < 3) {
        fprintf(stderr, "Usage: %s <input_dir> <output_dir> [--mip N] [--extract-jpeg]\n", argv[0]);
        return 2;
    }
    const char* in_root = argv[1];
    const char* out_root = argv[2];
    int mip = 0;
    int extract = 0;
    for (int i = 3; i < argc; i++) {
        if (strcmp(argv[i], "--mip") == 0 && i + 1 < argc) mip = atoi(argv[++i]);
        else if (strcmp(argv[i], "--extract-jpeg") == 0) extract = 1;
    }
    return process_dir(in_root, out_root, mip, extract);
}
