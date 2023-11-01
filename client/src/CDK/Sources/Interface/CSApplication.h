#ifndef __CDK__CSApplication__
#define __CDK__CSApplication__

#include "CSDictionary.h"

#include "CSLayer.h"

enum CSMemoryWarningLevel {
    CSMemoryWarningLevelNormal,
    CSMemoryWarningLevelLow,
    CSMemoryWarningLevelCritical
};

enum CSResolution : byte {
    CSResolution720,
    CSResolution1080,
    CSResolution1440,
    CSResolution2160,
    CSResolutionFit
};

struct CSEdgeInsets {
    float top;
    float left;
    float bottom;
    float right;
};

#ifdef CDK_IMPL
typedef struct _CSPlatformTouch {
    uint64 key;
    CSTouchButton button;
    float x;
    float y;
} CSPlatformTouch;
#endif

class CSApplication {
public:
	CSHandler<> OnPause;
	CSHandler<> OnResume;
	CSHandler<> OnDestroy;
	CSHandler<> OnReload;
	CSHandler<CSMemoryWarningLevel> OnMemoryWarning;
	CSHandler<const char*, const CSDictionary<string, string>*> OnReceiveQuery;

    struct Projection {
        float fov = 0;
        float width = 0;
        float height = 0;
        float znear = 1;
        float zfar = 10000;
    };
private:
    string _version;
    CSArray<CSLayer, 1, false> _layers;
    CSDictionary<uint64, CSTouch> _touches;
    CSDictionary<uint64, CSArray<CSLayer>> _touchLayers;
    CSGraphics* _graphics;
    Projection _projection;
    double _timeStamp;
#ifdef CS_DISPLAY_COUNTER
    double _lastFrameStamp;
    int _lastFrameCount = 0;
    int _frameCount = 0;
#endif
    float _keyboardHeight = 0;
    float _touchSensitivity = 0;
    float _popupDarkness = 0.5f;
    float _popupBlur = 0;
    bool _enabled = true;
    bool _refresh = false;
    byte _vsyncFrame = 1;
public:
#ifdef CS_DISPLAY_LAYER_FRAME
    bool displayLayerFrame = true;
#endif
#if defined(CS_DISPLAY_COUNTER) && defined(CS_DIAGNOSTICS)
    bool displayDiagnostics = true;
#endif
    static CSApplication* _instance;
private:
    CSApplication(int width, int height, int framebuffer, const CSSystemRenderTargetDescription& desc);
    ~CSApplication() = default;
public:
    CSApplication(const CSApplication&) = delete;
    CSApplication& operator=(const CSApplication&) = delete;

#ifdef CDK_IMPL
    static void initialize(int width, int height, int framebuffer, const CSSystemRenderTargetDescription& desc);
    static void finalize();
#endif
    static inline CSApplication* sharedApplication() {
        return _instance;
    }

    void setVersion(const string& version);

    inline const string& version() const {
        return _version;
    }
    CSRect frame() const;
    inline CSRect bounds() const {
        CSRect frame = this->frame();
        return CSRect(0, 0, frame.width, frame.height);
    }
    inline CSVector2 origin() const {
        return frame().origin();
    }
    inline CSVector2 size() const {
        return frame().size();
    }
    inline float width() const {
        return frame().width;
    }
    inline float height() const {
        return frame().height;
    }
    inline float center() const {
        return frame().center();
    }
    inline float middle() const {
        return frame().middle();
    }
    inline CSVector2 centerMiddle() const {
        return frame().centerMiddle();
    }
    CSEdgeInsets edgeInsets() const;
    void setResolution(CSResolution resolution);
    CSResolution resolution() const;
    void setProjection(const Projection& projection);
    Projection projection() const;
    float projectionWidth() const;
    float projectionHeight() const;

    inline const CSArray<CSLayer, 1, false>* layers() const {
        return &_layers;
    }
    int layerCount() const;

    inline void setVSyncFrame(int frame) {
        _vsyncFrame = frame;
    }
    inline int vsyncFrame() const {
        return _vsyncFrame;
    }
    inline void refresh() {
        _refresh = true;
    }
    inline bool enabled() const {
        return _enabled;
    }
    void setEnabled(bool enabled);

    inline float touchSensitivity() const {
        return _touchSensitivity;
    }
    inline void setTouchSensitivity(float touchSensitivity) {
        _touchSensitivity = touchSensitivity;
    }
#ifdef CDK_IMPL
    void touchesBegan(const CSPlatformTouch* touches, int count);
    void touchesMoved(const CSPlatformTouch* touches, int count);
    void touchesEnded(const CSPlatformTouch* touches, int count);
    void touchesCancelled(const CSPlatformTouch* touches, int count);
    bool beginTouch(CSLayer* layer, const CSTouch* touch);
    void cancelTouches(CSLayer* layer);
    void wheel(float offset);
    void keyDown(int keyCode);
    void keyUp(int keyCode);
    void backKey();
    void timeout();
    void resize(int width, int height);
    void draw();
    void pause();
    void resume();
    void setKeyboardHeight(float height);
#endif
    inline float keyboardHeight() const {
        return _keyboardHeight;
    }
    void clearLayers(bool transition);
    bool addLayer(CSLayer* layer);
    bool insertLayer(int index, CSLayer* layer);
    void clearLayers(int key, bool transition, bool inclusive, bool masking = false);
    const CSArray<CSLayer, 1, false>* findLayers(int key, bool masking = false);
    void findLayers(int key, CSArray<CSLayer, 1, false>* outArray, bool masking = false);
    CSLayer* findLayer(int key, bool masking = false);

    void insertLayerAsPopup(int index, CSLayer* layer, float darkness, float blur);
    void addLayerAsPopup(CSLayer* layer, float darkness, float blur);

    inline void setPopupDarkness(float darkness) {
        _popupDarkness = darkness;
    }
    inline float popupDarkness() const {
        return _popupDarkness;
    }
    inline void setPopupBlur(float blur) {
        _popupBlur = blur;
    }
    inline float popupBlur() const {
        return _popupBlur;
    }
    inline void insertLayerAsPopup(int index, CSLayer* layer) {
        insertLayerAsPopup(index, layer, _popupDarkness, _popupBlur);
    }
    inline void addLayerAsPopup(CSLayer* layer) {
        addLayerAsPopup(layer, _popupDarkness, _popupBlur);
    }
    void convertToUIFrame(CSRect& frame) const;
    void convertToLocalSpace(CSVector2& point) const;
    bool customEvent(int op, void* param);
    bool screenshot(const char* path) const;
    void openURL(const char* url);
    const char* clipboard();
    void setClipboard(const char* text);
	void shareUrl(const char* title, const char* message, const char* url);
	void finish();
};

extern void onStart();
extern void onPause();
extern void onResume();
extern void onReload();
extern void onMemoryWarning(CSMemoryWarningLevel level);
extern void onDestroy();
extern void onReceiveQuery(const char* url, const CSDictionary<string, string>* queries);

#endif
