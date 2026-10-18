---
type: vendor-doc
source_repo: "https://github.com/diasurgical/mpqfs"
source_path: "src/mpq_pkware.c"
ref: HEAD
---

/*
 * mpqfs — Minimal MPQ v1 archive reader/writer
 * SPDX-License-Identifier: MIT
 *
 * Public PKWARE DCL implode / explode wrappers.
 *
 * These functions expose the internal pkimplode() / pkexplode() routines
 * (from mpq_implode.h / mpq_explode.h) as part of the public mpqfs API,
 * allowing consumers (e.g. DevilutionX) to compress and decompress
 * arbitrary data using the PKWARE DCL format without pulling in a
 * separate PKWare library.
 */

#include "../include/mpqfs/mpqfs.h"
#include "mpq_explode.h" /* pkexplode(), PK_OK, PK_ERR_* */
#include "mpq_implode.h" /* pkimplode(), PK_OK, PK_ERR_* */
#include <stddef.h>
#include <stdint.h>

int mpqfs_pk_implode(const uint8_t *src, size_t srcSize,
    uint8_t *dst, size_t *dstSize,
    int dictBits)
{
	if (!src || !dst || !dstSize)
		return PK_ERR_INPUT;

	return pkimplode(src, srcSize, dst, dstSize, dictBits);
}

int mpqfs_pk_explode(const uint8_t *src, size_t srcSize,
    uint8_t *dst, size_t *dstSize)
{
	if (!src || !dst || !dstSize)
		return PK_ERR_INPUT;

	return pkexplode(src, srcSize, dst, dstSize);
}