---
type: vendor-doc
source_repo: "https://github.com/gavrilovicm/JASS"
source_path: "userinfo.hpp"
ref: HEAD
---

#ifndef USERINFO_HPP
#define USERINFO_HPP
#include<iostream>
#include<unistd.h>
#include<pwd.h>
#include<climits>

#define clear() printf("\033[H\033[J")

std::string* getuserinfo();
std::string* getworkingdir(std::string* username);

#endif
