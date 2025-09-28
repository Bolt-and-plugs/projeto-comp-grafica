cmake_minimum_required(VERSION 3.15)

set(project_name comp_grafica)
project(${project_name} VERSION 0.0.1 LANGUAGES C)

set(CMAKE_C_STANDARD 23)
set(CMAKE_C_STANDARD_REQUIRED True)
set(CMAKE_C_EXTENSIONS OFF)

if(NOT CMAKE_BUILD_TYPE)
  set(CMAKE_BUILD_TYPE Release)
endif()

include(FetchContent)

FetchContent_Declare(
  glfw
  GIT_REPOSITORY https://github.com/glfw/glfw.git
  GIT_TAG        3.4
)
set(GLFW_BUILD_EXAMPLES OFF CACHE BOOL "")
set(GLFW_BUILD_TESTS    OFF CACHE BOOL "")
set(GLFW_BUILD_DOCS     OFF CACHE BOOL "")

set(GLAD_API "gl=4.6")
set(GLAD_PROFILE "core")
set(GLAD_LOADER "ON")
set(GLAD_GENERATOR "c")

FetchContent_Declare(
    glad
    GIT_REPOSITORY https://github.com/Dav1dde/glad.git
    GIT_TAG        v2.0.8
    GIT_PROGRESS   TRUE
    SOURCE_SUBDIR  cmake
)

FetchContent_MakeAvailable(glfw glad)

glad_add_library(glad_gl_core_43 STATIC REPRODUCIBLE LOADER API gl:core=4.3)

set(OpenGL_GL_PREFERENCE "GLVND")
find_package(OpenGL REQUIRED)

file(GLOB SRC_FILES src/*.c)
file(GLOB PROJECT_MODULES src/modules/*/*.c)
file(GLOB VENDOR vendor/*/*.c)

add_executable(${project_name}
  ${SRC_FILES}
  ${PROJECT_MODULES}
  ${VENDOR}
)

target_link_libraries(${project_name} PRIVATE
  glfw      
  glad_gl_core_43
  OpenGL::GL
)

target_include_directories(${project_name} PRIVATE
  "${CMAKE_CURRENT_SOURCE_DIR}/src"
  "${CMAKE_CURRENT_SOURCE_DIR}/vendor"
)

target_compile_definitions(${project_name} PRIVATE
  BUILD_TYPE="${CMAKE_BUILD_TYPE}"
)

target_compile_options(${project_name} PRIVATE
  $<$<CONFIG:DEBUG>:-g3 -O0 -Wall -Wextra -Wpedantic -lm>
  $<$<CONFIG:RELEAE>:-lm>
)

if(NOT WIN32)
  target_compile_options(${project_name} PRIVATE $<$<CONFIG:DEBUG>:-fsanitize=address>)
  target_link_options(${project_name} PRIVATE $<$<CONFIG:DEBUG>:-fsanitize=address>)
endif()


message(STATUS "Project Name: ${project_name}")
message(STATUS "Build type: ${CMAKE_BUILD_TYPE}")
