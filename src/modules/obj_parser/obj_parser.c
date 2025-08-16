// Note: this is not a real parser, probably it will fail
// Im onlying writing this to have some fun
#include "obj_parser.h"
#include "../logger/log.h"

static obj *init_obj() {
  obj *o = malloc(sizeof(obj));
  o->vertecies = darray_create(sizeof(vec3f), DEFAULT_BUFFER_SIZE);
  o->textures = darray_create(sizeof(vec3f), DEFAULT_BUFFER_SIZE);
  o->normals = darray_create(sizeof(vec2f), DEFAULT_BUFFER_SIZE);
  o->faces = darray_create(sizeof(face), DEFAULT_BUFFER_SIZE);
  return o;
}

obj *load_model(const char *path) {
  c_info("Loading model from path %s", path);

  FILE *fp = fopen(path, "r");
  if (!fp) {
    c_error(FILE_OPEN_FAILURE, "Could not open file");
    return NULL;
  }

  obj *obj = init_obj();

  char line_buffer[DEFAULT_BUFFER_SIZE];
  while (fgets(line_buffer, sizeof(line_buffer), fp)) {
    c_info("Line read %s", line_buffer);

    // vertex
    if (strncmp(line_buffer, "v ", 2) == 0) {
      vec3f *v = malloc(sizeof(vec3f));
      sscanf(line_buffer, "v %f %f %f\n", &v->x, &v->y, &v->z);
      c_info("v %f %f %f\n", v->x, v->y, v->z);
      darray_push(obj->vertecies, v);
    }
    // vertex texture
    else if (strncmp(line_buffer, "vt ", 3) == 0) {
      vec2f *vt = malloc(sizeof(vec2f));
      sscanf(line_buffer, "vt %f %f\n", &vt->x, &vt->y);
      c_info("uv %f %f\n", vt->x, vt->y);
      darray_push(obj->textures, vt);
    }
    // vertex normal
    else if (strncmp(line_buffer, "vn ", 3) == 0) {
      vec3f *n = malloc(sizeof(vec3f));
      sscanf(line_buffer, "%f %f %f\n", &n->x, &n->y, &n->z);
      c_info("n %f %f\n", n->x, n->y);
      darray_push(obj->normals, n);
    }
    // face
    else if (strncmp(line_buffer, "f ", 2) == 0) {
      face f;
    }
  }

  return obj;
}
