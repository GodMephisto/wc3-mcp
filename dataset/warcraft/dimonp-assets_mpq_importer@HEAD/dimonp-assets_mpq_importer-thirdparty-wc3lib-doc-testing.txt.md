---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/wc3lib/doc/testing.txt"
ref: HEAD
---

Coverage:
cppcheck -DBOOST_TEST_DYN_LINK -DUNIX -DBLP -DJASS -DMAP -DMPQ -I src --language=c++ --std=c11 --std=c++11  --xml ./src  >"coverage_results.xml"

Memory analysis:
valgrind --tool=memcheck --log-file="wc3converter_memcheck.txt" ../build/src/app/wc3converter
valgrind --tool=callgrind --log-file="wc3converter_cachegrind.txt" ../build/src/app/wc3converter
