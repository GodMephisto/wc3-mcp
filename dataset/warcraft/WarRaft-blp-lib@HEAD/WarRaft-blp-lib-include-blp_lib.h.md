---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-lib"
source_path: "include/blp_lib.h"
ref: HEAD
---

#ifndef BLP_LIB_H
#define BLP_LIB_H

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

/**
 * Structure for storing BLP image
 */
typedef struct {
    uint32_t width;        /**< Image width in pixels */
    uint32_t height;       /**< Image height in pixels */
    uint8_t* data;         /**< Pointer to image data in RGBA format */
    uint32_t data_len;     /**< Data size in bytes */
} BlpImage;

/**
 * Operation result codes
 */
typedef enum {
    BLP_SUCCESS = 0,       /**< Operation completed successfully */
    BLP_INVALID_INPUT = -1, /**< Invalid input parameters */
    BLP_PARSE_ERROR = -2,   /**< BLP file parsing error */
    BLP_MEMORY_ERROR = -3,  /**< Memory allocation error */
    BLP_UNKNOWN_ERROR = -99 /**< Unknown error */
} BlpResult;

/**
 * Loads BLP file from data buffer
 * 
 * @param data Pointer to BLP file data
 * @param data_len Data length in bytes
 * @param out_image Pointer to BlpImage structure for storing result
 * @return BLP_SUCCESS on success, error code on failure
 * 
 * @note Memory for image data is allocated inside the function.
 *       Use blp_free_image() to free the memory.
 */
/**
 * Note: By default only the top (base) mip level is decoded for performance.
 * If you need more mip levels, use the encoding/decoding functions that accept
 * mip visibility flags from the Rust API, or extend the C API accordingly.
 */
BlpResult blp_load_from_buffer(const uint8_t* data, uint32_t data_len, BlpImage* out_image);

/**
 * Loads BLP file from filesystem
 * 
 * @param filename Path to BLP file (null-terminated string)
 * @param out_image Pointer to BlpImage structure for storing result
 * @return BLP_SUCCESS on success, error code on failure
 * 
 * @note Memory for image data is allocated inside the function.
 *       Use blp_free_image() to free the memory.
 */
BlpResult blp_load_from_file(const char* filename, BlpImage* out_image);

/**
 * Frees memory allocated for BlpImage
 * 
 * @param image Pointer to BlpImage structure to free
 * 
 * @note After calling this function, the BlpImage structure becomes invalid.
 */
void blp_free_image(BlpImage* image);

/**
 * Gets library version information
 * 
 * @return Pointer to version string (static memory, no need to free)
 */
const char* blp_get_version(void);

/**
 * Checks if data buffer is a valid BLP file
 * 
 * @param data Pointer to data to check
 * @param data_len Data length in bytes
 * @return 1 if data is a valid BLP file, 0 otherwise
 */
int blp_is_valid(const uint8_t* data, uint32_t data_len);

/**
 * Decode a specific mip level to a PNG file (from in-memory .blp bytes).
 */
BlpResult blp_decode_mip_to_png_from_buffer(const uint8_t* blp_data,
                                            uint32_t blp_len,
                                            uint32_t mip_index,
                                            const char* output_png_path);

/**
 * Decode a specific mip level to a PNG file (from a .blp file on disk).
 */
BlpResult blp_decode_mip_to_png_from_file(const char* blp_path,
                                          uint32_t mip_index,
                                          const char* output_png_path);

/**
 * Extract a specific mip as a raw JPEG file (JPEG-BLP only) from in-memory bytes.
 */
BlpResult blp_extract_mip_to_jpg_from_buffer(const uint8_t* blp_data,
                                             uint32_t blp_len,
                                             uint32_t mip_index,
                                             const char* output_jpg_path);

/**
 * Extract a specific mip as a raw JPEG file (JPEG-BLP only) from a .blp file on disk.
 */
BlpResult blp_extract_mip_to_jpg_from_file(const char* blp_path,
                                           uint32_t mip_index,
                                           const char* output_jpg_path);

/**
 * Encode an input image file (PNG/JPEG/etc.) into BLP on disk.
 *
 * @param input_image_path Path to input image file (UTF-8, null-terminated)
 * @param output_blp_path Path to output .blp file (UTF-8, null-terminated)
 * @param quality Quality 0..100 (higher = better quality, larger file)
 * @param mip_count Number of mip levels to generate (min 1)
 * @return BLP_SUCCESS on success, error code otherwise
 */
BlpResult blp_encode_file_to_blp(const char* input_image_path,
                                 const char* output_blp_path,
                                 uint8_t quality,
                                 uint32_t mip_count);

/**
 * Encode an input image file (PNG/JPEG/etc.) into BLP on disk using explicit mip flags.
 *
 * @param input_image_path Path to input image file (UTF-8, null-terminated)
 * @param output_blp_path Path to output .blp file (UTF-8, null-terminated)
 * @param quality Quality 0..100
 * @param mip_visible Array of flags (0/1), each entry indicates whether to include
 *                    mip level with that index. Missing entries are treated as false.
 * @param mip_visible_len Length of the mip_visible array
 */
BlpResult blp_encode_file_to_blp_with_flags(const char* input_image_path,
                                            const char* output_blp_path,
                                            uint8_t quality,
                                            const uint8_t* mip_visible,
                                            uint32_t mip_visible_len);

/**
 * Encode in-memory image bytes (PNG/JPEG/etc.) to BLP on disk.
 */
BlpResult blp_encode_bytes_to_blp(const uint8_t* image_bytes,
                                  uint32_t image_len,
                                  const char* output_blp_path,
                                  uint8_t quality,
                                  uint32_t mip_count);

/**
 * Encode in-memory image bytes (PNG/JPEG/etc.) to BLP on disk with explicit mip flags.
 */
BlpResult blp_encode_bytes_to_blp_with_flags(const uint8_t* image_bytes,
                                             uint32_t image_len,
                                             const char* output_blp_path,
                                             uint8_t quality,
                                             const uint8_t* mip_visible,
                                             uint32_t mip_visible_len);

#ifdef __cplusplus
}
#endif

#endif /* BLP_LIB_H */