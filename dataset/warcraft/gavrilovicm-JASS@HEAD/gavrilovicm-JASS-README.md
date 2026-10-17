---
type: vendor-doc
source_repo: "https://github.com/gavrilovicm/JASS"
source_path: "README.md"
ref: HEAD
---

# JASS (Just Another Shell - Shell)

Command line interpreter for Linux terminal. Implementing various things, from basic 
shell operations (ls, pwd etc.) to arithmetic operations and work with Matlab matrix objects.

## Install

### Dependencies

For this program to run you will need **Bison** and **Flex** installed on your system.

You can install **Bison** with next command:

```bash
# UBUNTU
sudo apt-get update && sudo apt-get install bison
```

**Flex** can be installed with next line:

```bash
# UBUNTU
sudo apt-get update && sudo apt-get install flex
```

For other Linux distributions, quick chat with Google will give you a solution.

------------- 

### Installation

When you have all of previous programs up and running, you can download and install JASS following next
steps:

```bash
# first position your terminal into directorium in witch you want to download and install JASSS
# In my case I used Documents folder

cd ~/Documents/

# cloning repositorium from github

git clone https://github.com/gavrilovicm/JASS

# enter downloaded directorium and install

cd JASS/ && make

# start JASS interpreter by typing this command

./jass

```


## Quick start

### Standard shell commands

JASS supports some standard shell commands from ls(with flags), pwd, clear all the way to new commands
within JASS such as **chusername** that changes username written on shell powerline. Upgrades and new
features are comming.

### Aritmetics

You can use JASS as calculator just like python interpreter can. This is convenient when
in need of some quick calculations.

### Matlab mode

This is mode still in progress but idea is to be able to do some Matrix transformations and operations
with JASS in matlab mode. You can do some more complex tasks and save some results in local variables
and access them any time. There are some rules that need to be followed in Matlab-mode:

- Variable names can only be one upper-case letter.
- All matrix rows need to be the same size.
