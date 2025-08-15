#include "shaders.h"
#include "../logger/log.h"

static char *read_shader_file(const char *filename) {
  FILE *file = fopen(filename, "rb");
  if (!file) {
    c_error(SHADER_LOAD_ERROR, "Failed to open shader file: %s\n", filename);
    return NULL;
  }

  fseek(file, 0, SEEK_END);
  long length = ftell(file);
  fseek(file, 0, SEEK_SET);

  char *buffer = (char *)malloc(length + 1);
  if (!buffer) {
    c_error(SHADER_LOAD_ERROR, "Failed to allocate memory for shader source\n");
    fclose(file);
    return NULL;
  }

  fread(buffer, 1, length, file);
  buffer[length] = '\0';
  fclose(file);
  return buffer;
}

static GLuint compile_shader(GLenum type, const char *source) {
  GLuint shader = glCreateShader(type);
  glShaderSource(shader, 1, &source, NULL);
  glCompileShader(shader);

  GLint success;
  glGetShaderiv(shader, GL_COMPILE_STATUS, &success);
  if (!success) {
    GLchar infoLog[512];
    glGetShaderInfoLog(shader, 512, NULL, infoLog);
    c_error(SHADER_COMPILE_ERROR, "Shader compilation failed:\n%s\n", infoLog);
    glDeleteShader(shader);
    return 0;
  }

  return shader;
}

// TODO upscale this create_shader_program for n shaders
static GLuint create_shader_program(GLuint vertexShader,
                                    GLuint fragmentShader) {
  GLuint program = glCreateProgram();
  glAttachShader(program, vertexShader);
  glAttachShader(program, fragmentShader);
  glLinkProgram(program);

  GLint success;
  glGetProgramiv(program, GL_LINK_STATUS, &success);
  if (!success) {
    GLchar infoLog[512];
    glGetProgramInfoLog(program, 512, NULL, infoLog);
    c_error(SHADER_PROGRAM_CREATION_ERROR,
            "Shader program linking failed:\n%s\n", infoLog);
    glDeleteProgram(program);
    return 0;
  }
  return program;
}

bool load_shaders() {
  char *vertexShaderSource = read_shader_file("default.vert");
  char *fragmentShaderSource = read_shader_file("default.frag");

  if (!vertexShaderSource || !fragmentShaderSource) {
    return false;
  }

  GLuint vertexShader = compile_shader(GL_VERTEX_SHADER, vertexShaderSource);
  GLuint fragmentShader =
      compile_shader(GL_FRAGMENT_SHADER, fragmentShaderSource);

  free(vertexShaderSource);
  free(fragmentShaderSource);

  if (!vertexShader || !fragmentShader) {
    return false;
  }

  GLuint shaderProgram = create_shader_program(vertexShader, fragmentShader);

  glDeleteShader(vertexShader);
  glDeleteShader(fragmentShader);

  if (!shaderProgram) {
    // Handle error
    return 1;
  }

  // Use the shader program
  glUseProgram(shaderProgram);

  // ... (rendering code)

  glDeleteProgram(shaderProgram);
  // ... (cleanup)
  return 0;
}
