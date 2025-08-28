# Testing the project

To test the project for yourself:

> you will need cmake and opengl, install based on your linux distro/package manager

debian/ubuntu
```sh
sudo apt-get install cmake 

```

arch based
```sh 
sudo pacman -S cmake 

```

windows

> just install [cmake](https://cmake.org/download/) and follow the rest

---

Then, compile the code with cmake:


```sh

# for debug mode
cmake -DCMAKE_BUILD_TYPE=DEBUG -S . -B target/ 

# for release mode
cmake -DCMAKE_BUILD_TYPE=RELEASE  -S . -B target/ 

cmake --build target/

./target/[TODO]

```

- To get help type
```sh
    ./target/[TODO] -h
```

