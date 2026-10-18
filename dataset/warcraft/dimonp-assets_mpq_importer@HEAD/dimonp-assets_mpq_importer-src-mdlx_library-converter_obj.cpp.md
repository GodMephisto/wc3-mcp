---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/mdlx_library/converter_obj.cpp"
ref: HEAD
---

#include <algorithm>
#include <expected>
#include <spanstream>
#include <spdlog/spdlog.h>
#include <mdlx/mdlx.hpp>
#include <assimp/Exporter.hpp>
#include <assimp/scene.h>

#include "assets_mpq_importer/mdlx.hpp"


namespace assmpq::mdlx {

// NOLINTBEGIN(cppcoreguidelines-owning-memory, cppcoreguidelines-pro-bounds-pointer-arithmetic, cppcoreguidelines-pro-bounds-constant-array-index)
static void process_vertices(const wc3lib::mdlx::Geoset& geoset, aiMesh& mesh)
{
    const auto& vertices = geoset.vertices();
    if(vertices.empty()) {
        mesh.mNumVertices = 0;
        mesh.mVertices = nullptr;
        return;
    }

    mesh.mPrimitiveTypes = aiPrimitiveType_TRIANGLE;
    mesh.mNumVertices = static_cast<unsigned>(vertices.size());
    mesh.mVertices = new aiVector3D[vertices.size()];

    for(size_t i = 0; i < vertices.size(); ++i) {
        mesh.mVertices[i] = { vertices[i].x(), vertices[i].y(), vertices[i].z() };
    }
}

static void process_normals(const wc3lib::mdlx::Geoset& geoset, aiMesh& mesh)
{
    const auto& normals = geoset.normals();
    if(normals.empty()) {
        mesh.mNormals = nullptr;
        return;
    }

    mesh.mNormals = new aiVector3D[normals.size()];
    for(size_t i = 0; i < normals.size(); ++i) {
        mesh.mNormals[i] = { normals[i].x(), normals[i].y(), normals[i].z() };
    }
}

static void process_texture_patches(const wc3lib::mdlx::Geoset& geoset, aiMesh& mesh)
{
    const auto& uv_patches = geoset.texturePatches();

    std::ranges::fill(mesh.mTextureCoords, nullptr);

    const size_t patches_size = std::min(uv_patches.size(), static_cast<size_t>(AI_MAX_NUMBER_OF_TEXTURECOORDS));
    for(size_t i = 0; i < patches_size; ++i) {
        const auto& uv_patch = uv_patches[i];

        mesh.mNumUVComponents[i] = 2;
        mesh.mTextureCoords[i] = new aiVector3D[uv_patch.size()];

        for(size_t j = 0; j < uv_patch.size(); ++j) {
            mesh.mTextureCoords[i][j] = aiVector3D(uv_patch[j].x(), 1.0F - uv_patch[j].y(), 0.0F);
        }
    }
}

static void process_faces(const wc3lib::mdlx::Geoset& geoset, aiMesh& mesh)
{
    for(const auto& face_type : geoset.faces()) {
        if (face_type.type() != wc3lib::mdlx::Faces::Type::Triangles) {
            spdlog::error("Only triangle primitives are supported.");
            continue;
        }

        const auto& face_verices = face_type.vertices();
        const auto triangle_faces_size = face_verices.size() / 3;

        mesh.mNumFaces = static_cast<unsigned>(triangle_faces_size);
        mesh.mFaces = new aiFace[triangle_faces_size];

        for(size_t i = 0; i < triangle_faces_size; ++i) {
            mesh.mFaces[i].mNumIndices = 3;
            mesh.mFaces[i].mIndices = new unsigned int[3];
            mesh.mFaces[i].mIndices[0] = static_cast<unsigned>(face_verices[(i * 3) + 0]);
            mesh.mFaces[i].mIndices[1] = static_cast<unsigned>(face_verices[(i * 3) + 1]);
            mesh.mFaces[i].mIndices[2] = static_cast<unsigned>(face_verices[(i * 3) + 2]);
        }
    }
}

static void process_scene(const wc3lib::mdlx::Mdlx& model, aiScene& scene, const std::string& mesh_name)
{
    const size_t geoset_count = model.geosets().size();

    scene.mRootNode = new aiNode();

    scene.mRootNode->mNumMeshes = static_cast<unsigned>(geoset_count);
    scene.mRootNode->mMeshes = new unsigned int[geoset_count];

    scene.mNumMeshes = static_cast<unsigned>(geoset_count);
    scene.mMeshes = new aiMesh*[geoset_count];

    scene.mNumMaterials = static_cast<unsigned>(geoset_count);
    scene.mMaterials = new aiMaterial*[geoset_count];
    for (size_t i = 0; i < scene.mNumMaterials; ++i) {
        scene.mMaterials[i] = new aiMaterial;
    }

    for (size_t geoset_idx = 0; geoset_idx < geoset_count; ++geoset_idx) {
        const auto& geoset = model.geosets().at(geoset_idx);

        aiMesh* mesh_ptr = scene.mMeshes[geoset_idx] = new aiMesh();

        if (geoset_count > 1) {
            mesh_ptr->mName = std::format("{}_{}", mesh_name, geoset_idx);
        } else {
            mesh_ptr->mName = mesh_name;
        }

        mesh_ptr->mMaterialIndex = static_cast<unsigned>(geoset_idx);

        process_vertices(geoset, *mesh_ptr);
        process_normals(geoset, *mesh_ptr);
        process_texture_patches(geoset, *mesh_ptr);
        process_faces(geoset, *mesh_ptr);

        scene.mRootNode->mMeshes[geoset_idx] = static_cast<unsigned>(geoset_idx); // Index of the mesh in scene.mMeshes
    }
}
// NOLINTEND(cppcoreguidelines-owning-memory, cppcoreguidelines-pro-bounds-pointer-arithmetic, cppcoreguidelines-pro-bounds-constant-array-index)

auto convert_mdlx_to_obj_mesh(const std::string& mesh_name, const FileData& mdx_file)-> std::expected<FileData, ErrorMessage>
try {
	wc3lib::mdlx::Mdlx model;

    std::ispanstream input(mdx_file);
    model.read(input);

    aiScene scene;

    process_scene(model, scene, mesh_name);

    Assimp::Exporter exporter;
    const aiExportDataBlob* blob = exporter.ExportToBlob(&scene, "obj");
    if (blob == nullptr) {
        return std::unexpected(std::format("Error exporting scene: {}", exporter.GetErrorString()));
    }

    std::vector<char> output_buffer(blob->size);
    std::ospanstream output(output_buffer, std::ios::out | std::ios::binary);
    output.write(static_cast<const char*>(blob->data), static_cast<std::streamsize>(blob->size));

    return output_buffer;
} catch (std::exception &e) {
    return std::unexpected(e.what());
}


} // namespace assmpq::mdlx
