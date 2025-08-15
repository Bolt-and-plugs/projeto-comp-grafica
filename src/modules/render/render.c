#include "render.h"
#include "../shaders/shaders.h"
#include "../../app.h"

extern App app;

void render(void) {
  glDrawArrays(GL_POINTS, 1, 1);
}
