#ifndef __CDK__CSTypes__
#define __CDK__CSTypes__

#include <stdlib.h>
#include <string.h>
#include <inttypes.h>
#include <unicode/uchar.h>
#include <unicode/ustring.h>
#include <cfloat>
#include <math.h>

#ifndef byte
typedef unsigned char byte;
#endif
typedef signed char sbyte;

typedef unsigned short ushort;
typedef unsigned int uint;
typedef int64_t int64;
typedef uint64_t uint64;
typedef UChar uchar;

#ifndef NULL
# define NULL    0
#endif

constexpr float FloatPi = M_PI;
constexpr float FloatPiOverTwo = M_PI_2;
constexpr float FloatPiOverFour = M_PI_4;
constexpr float FloatTwoPi = M_PI * 2;
constexpr float FloatToRadians = M_PI / 180;
constexpr float FloatToDegrees = 180 / M_PI;
constexpr float FloatEpsilon = FLT_EPSILON;
constexpr float FloatMin = FLT_MIN;
constexpr float FloatMax = FLT_MAX;

constexpr double DoublePi = M_PI;
constexpr double DoublePiOverTwo = M_PI_2;
constexpr double DoublePiOverFour = M_PI_4;
constexpr double DoubleTwoPi = M_PI * 2;
constexpr double DoubleToRadians = M_PI / 180;
constexpr double DoubleToDegrees = 180 / M_PI;
constexpr double DoubleEpsilon = DBL_EPSILON;
constexpr double DoubleMin = DBL_MIN;
constexpr double DoubleMax = DBL_MAX;

#endif
