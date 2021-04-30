cmake_minimum_required (VERSION 3.15)

project ("Suprecessor" VERSION 0.0.1)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)

add_library(SuprecessorApi SHARED "")

add_subdirectory(src)
add_subdirectory(headers)


