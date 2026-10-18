---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-react-jsx"
source_path: "examples/reakt/config/boot.rb"
ref: HEAD
---

ENV["BUNDLE_GEMFILE"] ||= File.expand_path("../Gemfile", __dir__)

require "bundler/setup" # Set up gems listed in the Gemfile.
require "bootsnap/setup" # Speed up boot time by caching expensive operations.
