---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/compressonator/external/opencv/release/CMakeLists.txt"
ref: HEAD
---

message(STATUS "(3:R)======================== opencv release" )

include(${CMAKE_CURRENT_LIST_DIR}/../opencv-clone-targets.cmake)
opencv_clone_targets("Release")
set(OpenCV_RELEASE_LIBRARIES ${OPENCV_RELEASE_LIBRARIES} PARENT_SCOPE)
set(OpenV_RELEASE_INCLUDE_DIRS ${OPENCV_RELEASE_INCLUDE_DIRS} PARENT_SCOPE)
