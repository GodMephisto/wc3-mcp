---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/merger/transformations.cpp"
ref: HEAD
---

#include "merger.hpp"

namespace assmpq::merger {

void recalculate_aabb(assmpq::merger::MeshData &mesh)
{
    aiVector3D min(assmpq::merger::kHighestValue, assmpq::merger::kHighestValue, assmpq::merger::kHighestValue);
    aiVector3D max(assmpq::merger::kLowestValue, assmpq::merger::kLowestValue, assmpq::merger::kLowestValue);

    for (const auto& vertex : mesh.vertices) {
        min.x = std::min(vertex.x, min.x);
        min.y = std::min(vertex.y, min.y);
        min.z = std::min(vertex.z, min.z);

        max.x = std::max(vertex.x, max.x);
        max.y = std::max(vertex.y, max.y);
        max.z = std::max(vertex.z, max.z);
    }

    mesh.aabb.mMin = min;
    mesh.aabb.mMax = max;
}

void transform_mesh_to_base_xz(assmpq::merger::MeshData &mesh)
{
    for(size_t i = 0; i < mesh.vertices.size(); i++) {
        // coord
        aiVector3D &vertex = mesh.vertices[i];
        vertex.x = -vertex.x;
        std::swap(vertex.y, vertex.x);
        std::swap(vertex.y, vertex.z);
        vertex.z = -vertex.z;

        // normal
        aiVector3D &normal = mesh.normals[i];
        normal.x = -normal.x;
        std::swap(normal.y, normal.x);
        std::swap(normal.y, normal.z);
        normal.z = -normal.z;
    }

    recalculate_aabb(mesh);
}

void scale_mesh(assmpq::merger::MeshData &mesh, float scale_factor)
{
    for(auto& vertex: mesh.vertices) {
        vertex.x *= scale_factor;
        vertex.y *= scale_factor;
        vertex.z *= scale_factor;
    }

    recalculate_aabb(mesh);
}

auto split_ramp_mesh(const assmpq::merger::MeshData& mesh, assmpq::merger::MeshData& out_mesh_0, assmpq::merger::MeshData& out_mesh_1)-> bool
{
    const aiAABB& aabb = mesh.aabb;
    if (std::abs(std::abs(aabb.mMax.x) - (kW3MapCellSize * 2.0F)) < assmpq::merger::kEpsilonValue) {
        // Spilt by x axis
        for (const auto& face: mesh.faces) {
            aiVector3D vertex0 = mesh.vertices[face.i0];
            aiVector3D vertex1 = mesh.vertices[face.i1];
            aiVector3D vertex2 = mesh.vertices[face.i2];

            if (std::abs(vertex0.x) < (kW3MapCellSize + kEpsilonValue) &&
                std::abs(vertex1.x) < (kW3MapCellSize + kEpsilonValue) &&
                std::abs(vertex2.x) < (kW3MapCellSize + kEpsilonValue)) {

                auto from_idx =  static_cast<uint32_t>(out_mesh_0.vertices.size());

                out_mesh_0.vertices.push_back(vertex0);
                out_mesh_0.vertices.push_back(vertex1);
                out_mesh_0.vertices.push_back(vertex2);

                out_mesh_0.normals.push_back(mesh.normals[face.i0]);
                out_mesh_0.normals.push_back(mesh.normals[face.i1]);
                out_mesh_0.normals.push_back(mesh.normals[face.i2]);

                out_mesh_0.uvs.push_back(mesh.uvs[face.i0]);
                out_mesh_0.uvs.push_back(mesh.uvs[face.i1]);
                out_mesh_0.uvs.push_back(mesh.uvs[face.i2]);

                out_mesh_0.faces.push_back(
                    assmpq::merger::TriFace {
                        .i0 = from_idx,
                        .i1 = from_idx + 1,
                        .i2 = from_idx + 2
                    }
                );
            }

            if (std::abs(vertex0.x) > (kW3MapCellSize - kEpsilonValue) &&
                std::abs(vertex1.x) > (kW3MapCellSize - kEpsilonValue) &&
                std::abs(vertex2.x) > (kW3MapCellSize - kEpsilonValue)) {

                auto from_idx =  static_cast<uint32_t>(out_mesh_1.vertices.size());

                vertex0.x -= kW3MapCellSize;
                vertex1.x -= kW3MapCellSize;
                vertex2.x -= kW3MapCellSize;

                out_mesh_1.vertices.push_back(vertex0);
                out_mesh_1.vertices.push_back(vertex1);
                out_mesh_1.vertices.push_back(vertex2);

                out_mesh_1.normals.push_back(mesh.normals[face.i0]);
                out_mesh_1.normals.push_back(mesh.normals[face.i1]);
                out_mesh_1.normals.push_back(mesh.normals[face.i2]);

                out_mesh_1.uvs.push_back(mesh.uvs[face.i0]);
                out_mesh_1.uvs.push_back(mesh.uvs[face.i1]);
                out_mesh_1.uvs.push_back(mesh.uvs[face.i2]);

                out_mesh_1.faces.push_back(
                    assmpq::merger::TriFace {
                        .i0 = from_idx,
                        .i1 = from_idx + 1,
                        .i2 = from_idx + 2
                    }
                );
            }
        }
    } else if (std::abs(std::abs(aabb.mMin.z) - (kW3MapCellSize * 2.0F)) < assmpq::merger::kEpsilonValue) {
        // Spilt by z axis
        for (const auto& face: mesh.faces) {
            aiVector3D vertex0 = mesh.vertices[face.i0];
            aiVector3D vertex1 = mesh.vertices[face.i1];
            aiVector3D vertex2 = mesh.vertices[face.i2];

            if (std::abs(vertex0.z) < (kW3MapCellSize + kEpsilonValue) &&
                std::abs(vertex1.z) < (kW3MapCellSize + kEpsilonValue) &&
                std::abs(vertex2.z) < (kW3MapCellSize + kEpsilonValue)) {

                auto from_idx =  static_cast<uint32_t>(out_mesh_0.vertices.size());

                out_mesh_0.vertices.push_back(vertex0);
                out_mesh_0.vertices.push_back(vertex1);
                out_mesh_0.vertices.push_back(vertex2);

                out_mesh_0.normals.push_back(mesh.normals[face.i0]);
                out_mesh_0.normals.push_back(mesh.normals[face.i1]);
                out_mesh_0.normals.push_back(mesh.normals[face.i2]);

                out_mesh_0.uvs.push_back(mesh.uvs[face.i0]);
                out_mesh_0.uvs.push_back(mesh.uvs[face.i1]);
                out_mesh_0.uvs.push_back(mesh.uvs[face.i2]);

                out_mesh_0.faces.push_back(
                    assmpq::merger::TriFace {
                        .i0 = from_idx,
                        .i1 = from_idx + 1,
                        .i2 = from_idx + 2
                    }
                );
            }

            if (std::abs(vertex0.z) > (kW3MapCellSize - kEpsilonValue) &&
                std::abs(vertex1.z) > (kW3MapCellSize - kEpsilonValue) &&
                std::abs(vertex2.z) > (kW3MapCellSize - kEpsilonValue)) {

                auto from_idx =  static_cast<uint32_t>(out_mesh_1.vertices.size());

                vertex0.z += kW3MapCellSize;
                vertex1.z += kW3MapCellSize;
                vertex2.z += kW3MapCellSize;

                out_mesh_1.vertices.push_back(vertex0);
                out_mesh_1.vertices.push_back(vertex1);
                out_mesh_1.vertices.push_back(vertex2);

                out_mesh_1.normals.push_back(mesh.normals[face.i0]);
                out_mesh_1.normals.push_back(mesh.normals[face.i1]);
                out_mesh_1.normals.push_back(mesh.normals[face.i2]);

                out_mesh_1.uvs.push_back(mesh.uvs[face.i0]);
                out_mesh_1.uvs.push_back(mesh.uvs[face.i1]);
                out_mesh_1.uvs.push_back(mesh.uvs[face.i2]);

                out_mesh_1.faces.push_back(
                    assmpq::merger::TriFace {
                        .i0 = from_idx,
                        .i1 = from_idx + 1,
                        .i2 = from_idx + 2
                    }
                );
            }
        }
    } else {
        return false;
    }

    out_mesh_0.name = mesh.name + "0";
    out_mesh_1.name = mesh.name + "1";

    recalculate_aabb(out_mesh_0);
    recalculate_aabb(out_mesh_1);
    return true;
}


} // namespace assmpq::merger
