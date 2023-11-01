#ifndef __CDK__CSTime__
#define __CDK__CSTime__

#include "CSString.h"

class CSTime {
private:
    mutable struct tm _tm;
public:
    CSTime();
    CSTime(time_t sec);
	CSTime(int year, int month, int day, int hour = 0, int minute = 0, int second = 0);
private:
	void setTime(time_t sec);
public:
	void setYear(int year);

    inline int year() const {
        return _tm.tm_year + 1900;
    }

	void setMonth(int month);
    
    inline int month() const {
        return _tm.tm_mon + 1;
    }

	void setDay(int day);
    
    inline int day() const {
        return _tm.tm_mday;
    }

    inline int dayOfYear() const {
        return _tm.tm_yday;
    }

	inline int dayOfWeek() const {
		return _tm.tm_wday;
	}
    
	void setHour(int hour);
    
    inline int hour() const {
        return _tm.tm_hour;
    }

	void setMinute(int minute);
    
    inline int minute() const {
        return _tm.tm_min;
    }

	void setSecond(int second);
    
    inline int second() const {
        return _tm.tm_sec;
    }

	void setTime(int year, int month, int day, int hour, int minute, int second);

	int64 diff(const CSTime& other) const;

	void revise(int64 second);

	string stringWithFormat(const char* format) const;

    static int64 currentTimeSecond();
    static double currentTime();
};

#endif
