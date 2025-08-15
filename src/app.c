#include "app.h"

App app;

bool set_envvar(const char *mode) {
  if (strcmp(mode, "Debug") == 0 || strcmp(mode, "DEBUG") == 0) {
    app.debug = true;
    return true;
  }

  app.debug = false;
  return false;
}

void set_debug_mode() {
//  set debug mode
#ifdef BUILD_TYPE
  bool debug = set_envvar(BUILD_TYPE);
  if (debug)
    c_info("Debug mode set");
#endif
  app.min_log_level = get_min_log_level();
}

void init_app() {
  set_debug_mode();

  char *name = malloc(32);
  strcpy(name, "Titulo");
  app.name = name;
}

void clean_app() {
  glfwDestroyWindow(app.window.glfw_window);
  glfwTerminate();
}

int main(int argc, char **argv) {
  init_app();
  init_window(600, 400);
  init_shader_table();

  while (!glfwWindowShouldClose(app.window.glfw_window)) {
    glClearColor(0.1f, 0.2f, 0.2f, 1.0f);
    glClear(GL_COLOR_BUFFER_BIT);
    glUseProgram(app.st.program);
    render();
    glfwSwapBuffers(app.window.glfw_window);
    glfwPollEvents();
  }

  clean_app();
  return 0;
}
