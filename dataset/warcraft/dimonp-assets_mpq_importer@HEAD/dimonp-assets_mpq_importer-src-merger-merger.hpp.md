---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/merger/merger.hpp"
ref: HEAD
---

#ifndef ASSMPQ_MERGER_H_
#define ASSMPQ_MERGER_H_

#include <vector>
#include <string>

#include <assimp/Importer.hpp> // C++ importer interface
#include <assimp/Exporter.hpp>
#include <assimp/scene.h>     // Output data structure
#include <assimp/postprocess.h> // Post processing flags

namespace assmpq::merger {

inline constexpr ai_real kHighestValue = std::numeric_limits<ai_real>::max();
inline constexpr ai_real kLowestValue = std::numeric_limits<ai_real>::lowest();
inline constexpr ai_real kEpsilonValue = 0.1F;

inline constexpr ai_real kW3MapCellSize = 128.0F;

struct TriFace {
	uint32_t i0, i1, i2;
};

struct MeshData {
	std::string name;
	aiAABB aabb;
    std::vector<aiVector3D> vertices;
    std::vector<aiVector3D> normals;
    std::vector<aiVector2D> uvs; // Assuming only one set of UVs
    std::vector<TriFace> faces;
};

using MeshGroups = std::vector<MeshData>;

auto load_model(const std::string& path, assmpq::merger::MeshGroups& mesh_groups)-> bool;
auto save_model(const std::string& path, const MeshGroups& mesh_groups)-> bool;

void recalculate_aabb(assmpq::merger::MeshData &mesh);
void transform_mesh_to_base_xz(assmpq::merger::MeshData &mesh);
void scale_mesh(assmpq::merger::MeshData &mesh, float scale_factor);
auto split_ramp_mesh(const assmpq::merger::MeshData& mesh, assmpq::merger::MeshData& out_mesh_0, assmpq::merger::MeshData& out_mesh_1)-> bool;

auto get_cliff_key_from_geo_name(const std::string &str_key)-> uint32_t;
auto convert_cliff_key_to_string(uint32_t key)-> std::string;
auto get_ramp_key_from_geo_name(const std::string &str_key, uint8_t part)-> uint32_t;
auto convert_ramp_key_to_string(uint32_t key)-> std::string;

} // namespace assmpq::merger

#endif  /// ASSMPQ_MERGER_H_
