---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/merger/wavefront_obj.cpp"
ref: HEAD
---

#include <assimp/Importer.hpp> // C++ importer interface
#include <assimp/Exporter.hpp>
#include <assimp/scene.h>     // Output data structure
#include <assimp/postprocess.h> // Post processing flags
#include <spdlog/spdlog.h>

#include "merger.hpp"

namespace assmpq::merger {

// NOLINTBEGIN(cppcoreguidelines-owning-memory, cppcoreguidelines-pro-bounds-pointer-arithmetic)

auto load_model(const std::string& path, assmpq::merger::MeshGroups& mesh_groups)-> bool
{
    Assimp::Importer importer;

    // Read the file. aiProcess_Triangulate ensures all faces are triangles.
    // aiProcess_GenSmoothNormals generates smooth normals if not present.
    const aiScene *scene_ptr = importer.ReadFile(path,
        static_cast<unsigned>(aiProcess_Triangulate) |
		static_cast<unsigned>(aiProcess_GenBoundingBoxes) |
        static_cast<unsigned>(aiProcess_GenSmoothNormals) |
		static_cast<unsigned>(aiProcess_JoinIdenticalVertices));

    // Check for errors
    if ((scene_ptr == nullptr) || ((scene_ptr->mFlags & AI_SCENE_FLAGS_INCOMPLETE) != 0U) || scene_ptr->mRootNode == nullptr) {
        spdlog::error("ERROR::ASSIMP::Importer: {}", importer.GetErrorString());
        return false;
    }

	// Process all meshes in the scene
    for (size_t i = 0; i < scene_ptr->mNumMeshes; ++i) {
        const aiMesh* mesh_ptr = scene_ptr->mMeshes[i];

		assmpq::merger::MeshData meshData;

        // Process vertices
        for (size_t j = 0; j < mesh_ptr->mNumVertices; ++j) {
            meshData.vertices.emplace_back(mesh_ptr->mVertices[j].x, mesh_ptr->mVertices[j].y, mesh_ptr->mVertices[j].z);

            // Process normals
            if (mesh_ptr->HasNormals()) {
				meshData.normals.emplace_back(mesh_ptr->mNormals[j].x, mesh_ptr->mNormals[j].y, mesh_ptr->mNormals[j].z);
            }

            // Process texture coordinates
            if (mesh_ptr->HasTextureCoords(0)) { // Assuming one set of UVs
				meshData.uvs.emplace_back(mesh_ptr->mTextureCoords[0][j].x, mesh_ptr->mTextureCoords[0][j].y);
            }
        }

        // Process all faces
        for (size_t j = 0; j < mesh_ptr->mNumFaces; ++j) {
            aiFace face = mesh_ptr->mFaces[j];

			if (face.mNumIndices != 3) {
				spdlog::error("ERROR::ASSIMP::Importer::Face with {} indices not supported", face.mNumIndices);
				return false;
			}

			meshData.faces.push_back(assmpq::merger::TriFace{
                .i0 = face.mIndices[0], .i1 = face.mIndices[1], .i2 = face.mIndices[2]
            });
        }

		meshData.aabb = mesh_ptr->mAABB;

		mesh_groups.push_back(meshData);
    }

    return true;
}

auto save_model(const std::string& path, const MeshGroups& mesh_groups)-> bool
{
	aiScene scene;
 	scene.mRootNode = new aiNode(); // Create a root node

	scene.mRootNode->mNumMeshes = static_cast<unsigned>(mesh_groups.size());
    scene.mRootNode->mMeshes = new unsigned int[mesh_groups.size()];

	scene.mNumMeshes = static_cast<unsigned>(mesh_groups.size());
    scene.mMeshes = new aiMesh*[mesh_groups.size()];

	scene.mNumMaterials = static_cast<unsigned>(mesh_groups.size());
	scene.mMaterials = new aiMaterial*[mesh_groups.size()];
	for (size_t i = 0; i < scene.mNumMaterials; ++i) {
        scene.mMaterials[i] = new aiMaterial;
    }

	for(size_t i = 0; i < mesh_groups.size(); ++i) {
		const MeshData& meshData = mesh_groups[i];

		aiMesh* mesh = scene.mMeshes[i] = new aiMesh();
		mesh->mName = meshData.name;
		mesh->mMaterialIndex = static_cast<unsigned>(i);

		// Process vertices
		mesh->mPrimitiveTypes = aiPrimitiveType_TRIANGLE;
		mesh->mNumVertices = static_cast<unsigned>(meshData.vertices.size());
		mesh->mVertices = new aiVector3D[meshData.vertices.size()];

		for(size_t j = 0; j < meshData.vertices.size(); ++j) {
			mesh->mVertices[j] = meshData.vertices[j];
		}

		// Process normals
		if(!meshData.normals.empty()) {
			mesh->mNormals = new aiVector3D[meshData.vertices.size()];
			for(size_t j = 0; j < meshData.vertices.size(); ++j) {
				mesh->mNormals[j] = meshData.normals[j];
			}
		} else {
			mesh->mNormals = nullptr;
		}

		// Process texture coordinates
		if(!meshData.uvs.empty()) {
			mesh->mNumUVComponents[0] = 2;
			mesh->mTextureCoords[0] = new aiVector3D[meshData.vertices.size()];
			for(size_t j = 0; j < meshData.vertices.size(); ++j) {
				mesh->mTextureCoords[0][j] = aiVector3D(meshData.uvs[j].x, meshData.uvs[j].y, 0.0F);
			}
		} else {
			mesh->mTextureCoords[0] = nullptr;
		}

		// Process faces
		const size_t numFaces = meshData.faces.size(); // triangles only
		mesh->mNumFaces = static_cast<unsigned>(numFaces);
		mesh->mFaces = new aiFace[numFaces];

		for(size_t j = 0; j < numFaces; ++j) {
			mesh->mFaces[j].mNumIndices = 3;
			mesh->mFaces[j].mIndices = new unsigned int[3];
			mesh->mFaces[j].mIndices[0] = meshData.faces[j].i0;
			mesh->mFaces[j].mIndices[1] = meshData.faces[j].i1;
			mesh->mFaces[j].mIndices[2] = meshData.faces[j].i2;
		}

		// Link mesh to root node
    	scene.mRootNode->mMeshes[i] = static_cast<unsigned>(i); // Index of the mesh in scene.mMeshes
	}

	// Write the scene to a file
	Assimp::Exporter exporter;
	const aiReturn ret = exporter.Export(&scene, "obj", path);
	if(ret == aiReturn_SUCCESS) {
		return true;
	} else {
		spdlog::error("ERROR::ASSIMP::Exporter: {}", exporter.GetErrorString());
	}

	return false;
}

// NOLINTEND(cppcoreguidelines-owning-memory, cppcoreguidelines-pro-bounds-pointer-arithmetic)

} // namespace assmpq::merger
