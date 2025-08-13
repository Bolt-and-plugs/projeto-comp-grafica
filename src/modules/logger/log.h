#ifndef _LOG
#define _LOG

#include "../../defines.h"
#include <stdarg.h>

typedef enum { DEBUG, INFO, WARN, ERROR, CRIT_ERROR } log_level;

typedef enum {
  DEFAULT_STATUS = -100,
  FILE_OPEN_FAILURE = 403,
  FILE_NOT_FOUND = 404,
  INVALID_INPUT = 600,
  INVALID_ARGS = 601,

} status_code;

#define DEFAULT_LOGGER_BUFFER_SIZE 4096 // bytes

// defining correct macros for logging
#define c_crit_error(s, ...)                                                   \
  c_log(CRIT_ERROR, s, ##__VA_ARGS__, NULL);                                   \
  exit(1);
#define c_error(s, ...) c_log(ERROR, s, ##__VA_ARGS__, NULL)
#define c_warn(s, ...) c_log(WARN, s, ##__VA_ARGS__, NULL)
#define c_info(...) c_log(INFO, DEFAULT_STATUS, ##__VA_ARGS__, NULL)
#define c_debug(s, ...) c_log(DEBUG, s, ##__VA_ARGS__, NULL)

log_level get_min_log_level();

// do not use c_log directly unless you are sure you need to
void c_log(log_level level, status_code status_code, const char *str, ...);

#endif
