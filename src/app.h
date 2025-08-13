#ifndef __APP
#define __APP

#include "modules/logger/log.h"

typedef struct __App {
  const char *name;
  bool debug;
  log_level min_log_level;
} App;


int main(int argc, char **argv);

#endif
