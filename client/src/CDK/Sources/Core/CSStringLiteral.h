#ifndef __CDK__CSStringLiteral_h
#define __CDK__CSStringLiteral_h

#include "CSString.h"

#define MIN(a, b) ((a) < (b) ? (a) : (b))

#define MAX_CHAR_ARR_LENGTH     100

#define CHAR_ARR(s) \
CHAR_AT(s, 0), \
CHAR_AT(s, 1), \
CHAR_AT(s, 2), \
CHAR_AT(s, 3), \
CHAR_AT(s, 4), \
CHAR_AT(s, 5), \
CHAR_AT(s, 6), \
CHAR_AT(s, 7), \
CHAR_AT(s, 8), \
CHAR_AT(s, 9), \
CHAR_AT(s, 10), \
CHAR_AT(s, 11), \
CHAR_AT(s, 12), \
CHAR_AT(s, 13), \
CHAR_AT(s, 14), \
CHAR_AT(s, 15), \
CHAR_AT(s, 16), \
CHAR_AT(s, 17), \
CHAR_AT(s, 18), \
CHAR_AT(s, 19), \
CHAR_AT(s, 20), \
CHAR_AT(s, 21), \
CHAR_AT(s, 22), \
CHAR_AT(s, 23), \
CHAR_AT(s, 24), \
CHAR_AT(s, 25), \
CHAR_AT(s, 26), \
CHAR_AT(s, 27), \
CHAR_AT(s, 28), \
CHAR_AT(s, 29), \
CHAR_AT(s, 30), \
CHAR_AT(s, 31), \
CHAR_AT(s, 32), \
CHAR_AT(s, 33), \
CHAR_AT(s, 34), \
CHAR_AT(s, 35), \
CHAR_AT(s, 36), \
CHAR_AT(s, 37), \
CHAR_AT(s, 38), \
CHAR_AT(s, 39), \
CHAR_AT(s, 40), \
CHAR_AT(s, 41), \
CHAR_AT(s, 42), \
CHAR_AT(s, 43), \
CHAR_AT(s, 44), \
CHAR_AT(s, 45), \
CHAR_AT(s, 46), \
CHAR_AT(s, 47), \
CHAR_AT(s, 48), \
CHAR_AT(s, 49), \
CHAR_AT(s, 50), \
CHAR_AT(s, 51), \
CHAR_AT(s, 52), \
CHAR_AT(s, 53), \
CHAR_AT(s, 54), \
CHAR_AT(s, 55), \
CHAR_AT(s, 56), \
CHAR_AT(s, 57), \
CHAR_AT(s, 58), \
CHAR_AT(s, 59), \
CHAR_AT(s, 60), \
CHAR_AT(s, 61), \
CHAR_AT(s, 62), \
CHAR_AT(s, 63), \
CHAR_AT(s, 64), \
CHAR_AT(s, 65), \
CHAR_AT(s, 66), \
CHAR_AT(s, 67), \
CHAR_AT(s, 68), \
CHAR_AT(s, 69), \
CHAR_AT(s, 70), \
CHAR_AT(s, 71), \
CHAR_AT(s, 72), \
CHAR_AT(s, 73), \
CHAR_AT(s, 74), \
CHAR_AT(s, 75), \
CHAR_AT(s, 76), \
CHAR_AT(s, 77), \
CHAR_AT(s, 78), \
CHAR_AT(s, 79), \
CHAR_AT(s, 80), \
CHAR_AT(s, 81), \
CHAR_AT(s, 82), \
CHAR_AT(s, 83), \
CHAR_AT(s, 84), \
CHAR_AT(s, 85), \
CHAR_AT(s, 86), \
CHAR_AT(s, 87), \
CHAR_AT(s, 88), \
CHAR_AT(s, 89), \
CHAR_AT(s, 90), \
CHAR_AT(s, 91), \
CHAR_AT(s, 92), \
CHAR_AT(s, 93), \
CHAR_AT(s, 94), \
CHAR_AT(s, 95), \
CHAR_AT(s, 96), \
CHAR_AT(s, 97), \
CHAR_AT(s, 98), \
CHAR_AT(s, 99), \
CHAR_AT(s, 100)

#define CHAR_AT(s, i) (i < MIN(sizeof(s) / sizeof(*s), MAX_CHAR_ARR_LENGTH) ? s[i] : 0)

template <char... chars>
const string& _S(const char* cstr) {
    CSAssert(strlen(cstr) <= MAX_CHAR_ARR_LENGTH, "out of range");
    static const string str(cstr);
    return str;
};

#define S(cstr)    _S<CHAR_ARR(cstr)>(cstr)

//Usage : 
//json.get(S("key")) is faster than json.get("key"). 
//No need to find string content from table with literal const char*.

#endif
