#ifndef __APP
#define __APP

#include "modules/logger/log.h"
#include "modules/window/window.h"
#include "modules/render/render.h"
#include "modules/shaders/shaders.h"
#include "modules/shaders/shaders.h"

typedef struct __App {
  const char *name;
  bool debug;
  log_level min_log_level;
  window window;
  shader_table st;
} App;


int main(int argc, char **argv);

#endif
