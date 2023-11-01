#define CDK_IMPL

#include "CSLocaleString.h"

#include "CSBuffer.h"

uint CSLocaleString::_localeMark = 1;
string CSLocaleString::_locale;
char CSLocaleString::_digitGroupSeperator = '\0';

const string CSLocaleString::DefaultLocale("en");

CSLocaleString::LocaleValue::LocaleValue(const string& locale, const string& value) : locale(locale), value(value) {
    
}

CSLocaleString::LocaleValue::LocaleValue(CSBuffer* buffer) : locale(buffer->readString()), value(buffer->readString()) {

}

CSLocaleString::LocaleValue::LocaleValue(const byte*& data) : locale(readString(data)), value(readString(data)) {

}

//========================================================================================================================

CSLocaleString::CSLocaleString(CSBuffer* buffer) : _localeValues(buffer) {
    
}

CSLocaleString::CSLocaleString(const byte*& data) : _localeValues(0) {
    int count = readLength(data);
    _localeValues.setCapacity(count);
    for (int i = 0; i < count; i++) new (&_localeValues.addObject()) LocaleValue(data);
}

const string& CSLocaleString::value() const {
    if (_currentLocaleMark != _localeMark) {
        _currentLocaleMark = _localeMark;
        _currentLocaleValue.clear();
        
        if (_locale) {
            string defaultValue;
            foreach (const LocaleValue&, lv, &_localeValues) {
                if (lv.locale == _locale) {
                    _currentLocaleValue = lv.value;
                    return _currentLocaleValue;
                }
                if (lv.locale == DefaultLocale) defaultValue = lv.value;
            }
            _currentLocaleValue = defaultValue;
        }
    }
    return _currentLocaleValue;
}

string CSLocaleString::localeValue(const string& locale, bool useDefault) const {
    string defaultValue;
    foreach (const LocaleValue&, lv, &_localeValues) {
        if (lv.locale == locale) return lv.value;
        if (useDefault && lv.locale == DefaultLocale) defaultValue = lv.value;
    }
    return defaultValue;
}

void CSLocaleString::setLocaleValue(const string& locale, const string& value) {
    _currentLocaleMark = 0;
    if (value) {
        foreach (LocaleValue&, lv, &_localeValues) {
            if (lv.locale == locale) {
                lv.value = value;
                return;
            }
        }
        new (&_localeValues.addObject()) LocaleValue(locale, value);
    }
    else {
        for (int i = 0; i < _localeValues.count(); i++) {
            if (_localeValues.objectAtIndex(i).locale == locale) {
                _localeValues.removeObjectAtIndex(i);
                break;
            }
        }
    }
}

int CSLocaleString::resourceCost() const {
    int cost = sizeof(CSLocaleString) + _localeValues.capacity() * sizeof(LocaleValue);
    foreach (const LocaleValue&, lv, &_localeValues) cost += lv.value.resourceCost();
    return cost;
}

void CSLocaleString::setLocale(const string& locale) {
    if (_locale != locale) {
        _localeMark++;
        _locale = locale;
    }
}
