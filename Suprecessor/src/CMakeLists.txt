file (GLOB COLLECTED_SOURCES "*.cpp")
target_sources(SuprecessorApi PRIVATE
	${COLLECTED_SOURCES}
)
target_include_directories(SuprecessorApi PRIVATE
	${CMAKE_CURRENT_SOURCE_DIR}
)