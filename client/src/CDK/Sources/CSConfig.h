#ifndef __CDK__CSConfig__
#define __CDK__CSConfig__

#define CS_ASSERT_DEBUG
#define CS_CONSOLE_DEBUG
#define CS_GL_DEBUG
//#define CS_SECRET_THREAD_SAFE
//#define CS_DISPLAY_LAYER_FRAME
#define CS_DISPLAY_COUNTER
#define CS_DIAGNOSTICS
#define CS_LOCK_TIME
#define CS_DEADLOCK_DETECTION

#define CS_FIXED_SIN_LUT
//#define CS_FIXED_FAST_SIN
#define CS_FIXED_CACHE

#define CS_IOS_CUSTOM_MEMORY_WARNING

//================================================
#ifndef DEBUG

//# undef CS_ASSERT_DEBUG		//TODO
# undef CS_CONSOLE_DEBUG
# undef CS_GL_DEBUG
# undef CS_DISPLAY_LAYER_FRAME
# undef CS_DISPLAY_COUNTER
# undef CS_DIAGNOSTICS
# undef CS_LOCK_TIME

#endif

//================================================

#endif
