#ifndef __CDK__CSAnimationFragment__
#define __CDK__CSAnimationFragment__

#include "CSAnimationLoop.h"
#include "CSAnimationFloat.h"
#include "CSAnimationColor.h"

#include "CSSceneObject.h"

#include "CSAudio.h"

class CSAnimationObject;
class CSAnimationDerivation;
class CSAnimationObjectDerivation;
class CSBuffer;

class CSAnimationFragment : public CSObject {
public:
    class Sound : public CSObject {
    public:
        string subpath;
        float volume = 1;
        CSAudioControl control = CSAudioControlEffect;
        byte loop = 1;
        byte priority = 0;
        bool perspective = true;
        float latency = 0;
        float duration = 0;
        float duplication = 0;
        bool stop = false;

        Sound(const string& subpath);
        Sound(CSBuffer* buffer);
        Sound(const Sound* sound);
    private:
        ~Sound() = default;
    public:
        static inline Sound* sound(const string& subpath) {
            return autorelease(new Sound(subpath));
        }
        static inline Sound* soundWithBuffer(CSBuffer* buffer) {
            return autorelease(new Sound(buffer));
        }
        static inline Sound* soundWithSound(const Sound* other) {
            return autorelease(new Sound(other));
        }
    };
    string name;
    uint key = 0;
    CSPtr<CSAnimationFloat> x;
    CSPtr<CSAnimationFloat> y;
    CSPtr<CSAnimationFloat> z;
    CSPtr<CSAnimationFloat> radial;
    CSPtr<CSAnimationFloat> tangential;
    CSPtr<CSAnimationFloat> tangentialAngle;
    float pathDegree = 0;
    CSAnimationLoop pathLoop;
    bool pathUsingSpeed = false;
    bool billboard = false;
    bool reverse = false;
    bool facing = false;
    CSPtr<CSAnimationFloat> rotationX;
    CSPtr<CSAnimationFloat> rotationY;
    CSPtr<CSAnimationFloat> rotationZ;
    float rotationDuration = 0;
    CSAnimationLoop rotationLoop;
    CSPtr<CSAnimationFloat> scaleX;
    CSPtr<CSAnimationFloat> scaleY;
    CSPtr<CSAnimationFloat> scaleZ;
    float scaleDuration = 0;
    CSAnimationLoop scaleLoop;
    bool scaleEach = false;
    bool pivot = false;
    bool stencil = false;
    bool closing = false;
    byte randomWeight = 1;
    byte target = 0;
    string binding;
    float duration = 0;
    float latency = 0;
    CSPtr<Sound> sound;
    bool localeVisible = false;
private:
    mutable bool _localeCurrentContains = false;
    mutable uint _localeCurrentMark = 0;
    CSArray<string>* _locales = NULL;
public:
    CSPtr<CSSceneObjectBuilder> substance;
    CSPtr<CSAnimationDerivation> derivation;

    CSAnimationFragment() = default;
    CSAnimationFragment(CSBuffer* buffer);
    CSAnimationFragment(const CSAnimationFragment* other);
private:
    ~CSAnimationFragment();
public:
    static inline CSAnimationFragment* fragment() {
        return autorelease(new CSAnimationFragment());
    }
    static inline CSAnimationFragment* fragmentWithBuffer(CSBuffer* buffer) {
        return autorelease(new CSAnimationFragment(buffer));
    }
    static inline CSAnimationFragment* fragmentWithFragment(const CSAnimationFragment* other) {
        return autorelease(new CSAnimationFragment(other));
    }

    void addLocale(const string& locale);
    void removeLocale(const string& locale);
    void clearLocales();
    bool checkLocale() const;
    bool hasPivot() const;
    bool hasSubstance() const;
    int resourceCost() const;
    void preload() const;
};

class CSAnimationObjectFragment : public CSObject {
private:
    CSAnimationObject* _root;
    CSAnimationObjectFragment* _parent;
    const CSAnimationFragment* _origin;
    float _progress;
    bool _visible;
    int _xRandom;
    int _yRandom;
    int _zRandom;
    int _radialRandom;
    int _tangentialRandom;
    int _tangentialAngleRandom;
    int _rotationRandom;
    int _scaleRandom;
    int _soundHandle;
    int _soundCounter;
    CSSceneObject* _substance;
    CSAnimationObjectDerivation* _derivation;
public:
    CSMatrix postTransform;

    CSAnimationObjectFragment(const CSAnimationFragment* origin);
private:
    ~CSAnimationObjectFragment();
public:
    inline CSAnimationObject* root() {
        return _root;
    }
    inline const CSAnimationObject* root() const {
        return _root;
    }
    inline CSAnimationObjectFragment* parent() {
        return _parent;
    }
    inline const CSAnimationObjectFragment* parent() const {
        return _parent;
    }
    inline const CSAnimationFragment* origin() const {
        return _origin;
    }
    bool setSubstance(CSSceneObject* substance);
    inline CSSceneObject* substance() {
        return _substance;
    }
    inline const CSSceneObject* substance() const {
        return _substance;
    }
    bool setDerivation(CSAnimationObjectDerivation* derivation);
    inline CSAnimationObjectDerivation* derivation() {
        return _derivation;
    }
    inline const CSAnimationObjectDerivation* derivation() const {
        return _derivation;
    }
#ifdef CDK_IMPL
    bool link(CSAnimationObject* root, CSAnimationObjectFragment* parent);
    void unlink();
#endif
    bool addAABB(CSABoundingBox& result) const;
    void addCollider(CSCollider*& result) const;
    bool getTransform(float progress, const string& name, CSMatrix& result) const;
    void rewind();
    float duration(CSSceneObject::DurationParam param) const;
    float pathDuration() const;
    inline float progress() const {
        return _progress;
    }
    CSSceneObject::UpdateState update(float delta, bool alive, uint inflags, uint& outflags);
    uint show();
    void draw(CSGraphics* graphics, CSInstanceLayer layer);
private:
    void resetRandoms();
    void stopSound();
    void updateVisible(bool visible, uint& outflags);
};

#endif
