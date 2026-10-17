---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/compressonator/external/opencv/debug/CMakeLists.txt"
ref: HEAD
---

message(STATUS "(3:D)======================== opencv debug" )

include(${CMAKE_CURRENT_LIST_DIR}/../opencv-clone-targets.cmake)
opencv_clone_targets("Debug")
set(OpenCV_DEBUG_LIBRARIES ${OPENCV_DEBUG_LIBRARIES} PARENT_SCOPE)
set(OpenCV_DEBUG_INCLUDE_DIRS ${OPENCV_DEBUG_INCLUDE_DIRS} PARENT_SCOPE)
