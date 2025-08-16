#ifndef _OBJ
#define _OBJ

#include "defines.h"

#define DEFAULT_BUFFER_SIZE 128

typedef struct __face_idx {
  int vertex_index;
  int texture_index;
  int normal_index;
} face_idx;

typedef struct __face {
  face_idx corners[4];
  int corner_count;
} face;

typedef struct __obj {
  DynamicArray *vertecies; // v -> vertices (x, y, z, -- optional -- w)
  DynamicArray *normals;   // List of vertex normals in (x,y,z) form;
  DynamicArray *textures;  // vt ->textures coord (u, [v, w])
  DynamicArray *faces;     // combining all above
} obj;

obj *load_model(const char *path);

#endif
