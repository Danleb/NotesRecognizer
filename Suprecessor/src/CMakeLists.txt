file (GLOB COLLECTED_SOURCES "*.cpp")
target_sources(SuprecessorObj PRIVATE
	${COLLECTED_SOURCES}
)
target_include_directories(SuprecessorObj PRIVATE
	${CMAKE_CURRENT_SOURCE_DIR}
)

add_subdirectory(dev)
