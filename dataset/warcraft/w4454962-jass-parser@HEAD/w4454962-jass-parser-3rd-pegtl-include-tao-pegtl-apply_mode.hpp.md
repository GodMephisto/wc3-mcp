---
type: vendor-doc
source_repo: "https://github.com/w4454962/jass-parser"
source_path: "3rd/pegtl/include/tao/pegtl/apply_mode.hpp"
ref: HEAD
---

// Copyright (c) 2014-2022 Dr. Colin Hirsch and Daniel Frey
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at https://www.boost.org/LICENSE_1_0.txt)

#ifndef TAO_PEGTL_APPLY_MODE_HPP
#define TAO_PEGTL_APPLY_MODE_HPP

namespace tao::pegtl
{
   enum class apply_mode : bool
   {
      action = true,
      nothing = false
   };

}  // namespace tao::pegtl

#endif
