---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: "examples/encode_dir.c"
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

static int is_image(const char* name) {
    const char* dot = strrchr(name, '.');
    if (!dot) return 0;
    return strcasecmp(dot, ".png") == 0 || strcasecmp(dot, ".jpg") == 0 || strcasecmp(dot, ".jpeg") == 0 || strcasecmp(dot, ".tga") == 0 || strcasecmp(dot, ".bmp") == 0;
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

static int process_dir(const char* in_root, const char* out_root, int quality, int mips) {
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
        if (!is_image(full)) continue;
        const char* rel = full + root_len;
        if (*rel == '/') rel++;
        char out_path[PATH_MAX];
        snprintf(out_path, sizeof(out_path), "%s/%s", out_root, rel);
        replace_ext(out_path, sizeof(out_path), ".blp");
        ensure_dir_for(out_path);
        BlpResult r = blp_encode_file_to_blp(full, out_path, (uint8_t)quality, (uint32_t)mips);
        if (r != BLP_SUCCESS) {
            fprintf(stderr, "encode failed: %s -> %s (%d)\n", full, out_path, r);
        } else {
            printf("Saved: %s\n", out_path);
        }
    }
    pclose(pipe);
    return 0;
}

int main(int argc, char** argv) {
    if (argc < 3) {
        fprintf(stderr, "Usage: %s <input_dir> <output_dir> [--quality N] [--mips N]\n", argv[0]);
        return 2;
    }
    const char* in_root = argv[1];
    const char* out_root = argv[2];
    int quality = 90;
    int mips = 8;
    for (int i = 3; i < argc; i++) {
        if (strcmp(argv[i], "--quality") == 0 && i + 1 < argc) quality = atoi(argv[++i]);
        else if (strcmp(argv[i], "--mips") == 0 && i + 1 < argc) mips = atoi(argv[++i]);
    }
    return process_dir(in_root, out_root, quality, mips);
}
