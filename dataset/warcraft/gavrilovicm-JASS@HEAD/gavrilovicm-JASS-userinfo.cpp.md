---
type: vendor-doc
source_repo: "https://github.com/gavrilovicm/JASS"
source_path: "userinfo.cpp"
ref: HEAD
---

#include"userinfo.hpp"

std::string* getuserinfo(){

	struct passwd *pw;
	uid_t uid;

	uid = geteuid();
	pw = getpwuid(uid);

	if (pw){
		return new std::string(pw->pw_name);
	}else{
		return new std::string("anonimus");
	}
}

std::string* getworkingdir(std::string* username){
	char wdir[PATH_MAX];
	getcwd(wdir, sizeof(wdir));
	std::string* s = new std::string(wdir);

	std::string* full_home_name = new std::string("/home/"+*username);

	if (s->find(*full_home_name) != std::string::npos){
		s->replace(s->find(*full_home_name), full_home_name->size(), "~");
	}

	delete full_home_name;
	return s;
}
