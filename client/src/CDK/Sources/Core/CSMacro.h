#ifndef __CDK__CSMacro__
#define __CDK__CSMacro__

template <typename B, typename D>
struct derived_host {
    operator const B*() const;
    operator D*();
};

template <typename B, typename D>
struct derived {
private:
    template <typename T>
    static std::true_type check(D*, T);
    static std::false_type check(const B*, int);
public:    
    static const bool value = decltype(check(derived_host<B, D>(), int()))::value;
};

#define countof(arr)    (sizeof(arr) / sizeof(arr[0]))

template <class T> struct non_deduced { using type = T; };
template <class T> using non_deduced_t = typename non_deduced<T>::type;

template <bool C, typename T = void>
struct enable_if {
    typedef T type;
};

template<typename T>
struct enable_if<false, T> {};

#endif
