cmake_minimum_required(VERSION 3.10)
set(project_name comp_grafica)
project(${project_name} VERSION 0.0.1)

set(CMAKE_C_STANDARD 23)
set(CMAKE_C_STANDARD_REQUIRED True)

if(NOT CMAKE_BUILD_TYPE)
  set(CMAKE_BUILD_TYPE RELEASE)
endif()

if(CMAKE_BUILD_TYPE MATCHES DEBUG)
  add_compile_options(-g3 -O0 -Wall -Wextra -Wpedantic)
  if(NOT WIN32)
    add_compile_options(-fsanitize=address)
    add_link_options(-fsanitize=address)
  endif()
endif()

add_definitions(-DBUILD_TYPE="${CMAKE_BUILD_TYPE}")

file(GLOB OS_MODULES src/*/*.c)
file(GLOB SRC_FILES src/*.c)
file(GLOB TEST_FILES test/*.c)

include_directories(src)

add_executable(${project_name}
        ${SRC_FILES}
        ${TEST_FILES}
        ${OS_MODULES}
)

message(STATUS "Build type: ${CMAKE_BUILD_TYPE}")
