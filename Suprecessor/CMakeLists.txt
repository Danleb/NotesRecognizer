cmake_minimum_required (VERSION 3.15)

project ("Suprecessor" VERSION 0.0.1)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)

add_library(SuprecessorObj OBJECT "")
set_property(TARGET SuprecessorObj PROPERTY POSITION_INDEPENDENT_CODE 1)

add_library(SuprecessorApi SHARED $<TARGET_OBJECTS:SuprecessorObj>)
add_library(SuprecessorLib STATIC $<TARGET_OBJECTS:SuprecessorObj>)
add_executable(SuprecessorDev "")
target_link_libraries(SuprecessorDev SuprecessorObj)

add_subdirectory(src)
add_subdirectory(headers)


