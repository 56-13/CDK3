#ifndef __CDK__CSGraphics__
#define __CDK__CSGraphics__

#include "CSGraphicsContext.h"
#include "CSZRect.h"
#include "CSFont.h"
#include "CSImage.h"
#include "CSLocaleString.h"
#include "CSStringDisplay.h"
#include "CSRenderState.h"
#include "CSRenderQueue.h"
#include "CSDelegateRenderCommand.h"
#include "CSVertexArray.h"
#include "CSVertexArrayInstance.h"

#define CSStringWidthUnlimited  100000

class CSStreamRenderCommand;

class CSGraphics : public CSObject {
private:
    struct State {
        CSMatrix world;
        CSColor color;
		CSArray<CSMatrix>* worldReserved;
		CSArray<CSColor>* colorReserved;
        const CSFont* font;
        CSColor fontColors[4];
        CSRenderState batch;
        CSRect stencilBounds;
        bool usingStencil;
        bool renderOrder;
        bool mark;
        State* prev;
        
        State(CSRenderTarget* target, State* prev);
        ~State();
        
        void reset(CSRenderTarget* target);
    };
    CSRenderQueue* _queue;
    CSRenderTarget* _target;
    State* _state;
public:
    CSGraphics(CSRenderQueue* queue, CSRenderTarget* target);
    CSGraphics(CSRenderTarget* target);
private:
    ~CSGraphics();
public:
    static inline CSGraphics* graphics(CSRenderQueue* queue, CSRenderTarget* target) {
        return autorelease(new CSGraphics(queue, target));
    }
    static inline CSGraphics* graphics(CSRenderTarget* target) {
        return autorelease(new CSGraphics(target));
    }
#ifdef CDK_IMPL
    static void initialize(CSGraphicsPlatform platform);
    static void finalize();
#endif
    void clear(const CSColor& color);
    void clearStencil();
    void reset();
    void push(bool mark = false);
    void pop(bool mark = false);
    int pushCount() const;

    inline CSRenderQueue* queue() {
        return _queue;
    }
    inline const CSRenderQueue* queue() const {
        return _queue;
    }
    inline void setTarget(CSRenderTarget* target) {
        _state->batch.target = target;
    }
    inline CSRenderTarget* target() {
        return _state->batch.target;
    }
    inline const CSRenderTarget* target() const {
        return _state->batch.target;
    }
    inline void setRenderer(CSRenderer* renderer) {
        _state->batch.renderer = renderer;
    }
    inline CSRenderer* renderer() {
        return _state->batch.renderer;
    }
    inline const CSRenderer* renderer() const {
        return _state->batch.renderer;
    }

    inline void setCamera(const CSCamera& camera) {
        _state->batch.camera = camera;
    }
    inline CSCamera& camera() {
        return _state->batch.camera;
    }
    inline const CSCamera& camera() const {
        return _state->batch.camera;
    }
    inline CSMatrix& world() {
        return _state->world;
    }
    inline const CSMatrix& world() const {
        return _state->world;
    }
    inline CSMatrix worldViewProjection() const {
        return _state->world * _state->batch.camera.viewProjection();
    }
    inline void transform(const CSMatrix& world) {
        _state->world = world * _state->world;
    }
    inline void translate(const CSVector3& v) {
        transform(CSMatrix::translation(v));
    }
    inline void rotateYawPitchRoll(float yaw, float pitch, float roll) {
        transform(CSMatrix::rotationYawPitchRoll(yaw, pitch, roll));
    }
    inline void rotateAxis(const CSVector3& axis, float v) {
        transform(CSMatrix::rotationAxis(axis, v));
    }
    inline void rotateX(float v) {
        transform(CSMatrix::rotationX(v));
    }
    inline void rotateY(float v) {
        transform(CSMatrix::rotationY(v));
    }
    inline void rotateZ(float v) {
        transform(CSMatrix::rotationZ(v));
    }
    inline void scale(const CSVector3 v) {
        transform(CSMatrix::scaling(v));
    }
    inline void scale(float v) {
        transform(CSMatrix::scaling(v));
    }
	void pushTransform();
private:
    void peekTransform(bool pop);
public:
    inline void popTransform() {
        peekTransform(true);
    }
    inline void resetTransform() {
        peekTransform(false);
    }

    void setColor(const CSColor& color, bool inherit = true);
    inline const CSColor& color() const {
        return _state->color;
    }
	void pushColor();
private:
    void peekColor(bool pop);
public:
    inline void popColor() {
        peekColor(true);
    }
    inline void resetColor() {
        peekColor(false);
    }

    inline const CSRenderState& state() const {
        return _state->batch;
    }
    inline void setMaterial(const CSMaterial& material) {
        _state->batch.material = material;
    }
    inline CSMaterial& material() {
        return _state->batch.material;
    }
    inline const CSMaterial& material() const {
        return _state->batch.material;
    }
    inline const CSColor3& fogColor() const {
        return _state->batch.fogColor;
    }
    inline void setFogColor(const CSColor3& fogColor) {
        _state->batch.fogColor = fogColor;
    }
    inline float fogNear() const {
        return _state->batch.fogNear;
    }
    inline void setFogNear(float fogNear) {
        _state->batch.fogNear = fogNear;
    }
    inline float fogFar() const {
        return _state->batch.fogFar;
    }
    inline void setFogFar(float fogFar) {
        _state->batch.fogNear = fogFar;
    }
    inline bool usingFog() const {
        return _state->batch.usingFog();
    }
    inline void clearFog() const {
        _state->batch.fogNear = _state->batch.fogFar = 0;
    }
    inline void setBloomThreshold(float threshold) {
        _state->batch.bloomThreshold = threshold;
    }
    inline float bloomThreshold() const {
        return _state->batch.bloomThreshold;
    }
    void setBrightness(float brightness, bool inherit = true);
    inline float brightness() const {
        return _state->batch.brightness;
    }
    void setContrast(float contrast, bool inherit = true);
    inline float contrast() const {
        return _state->batch.contrast;
    }
    void setSaturation(float saturation, bool inherit = true);
    inline float saturation() const {
        return _state->batch.saturation;
    }
    inline void setFont(const CSFont* font) {
        retain(_state->font, font);
    }
    inline const CSFont* font() const {
        return _state->font;
    }
    static void setDefaultFont(const CSFont* font);
    static const CSFont* defaultFont();
    
    inline void setFontColorV(const CSColor& topColor, const CSColor& bottomColor) {
        _state->fontColors[0] = _state->fontColors[1] = topColor;
        _state->fontColors[2] = _state->fontColors[3] = bottomColor;
    }
    inline void setFontColorH(const CSColor& leftColor, const CSColor& rightColor) {
        _state->fontColors[0] = _state->fontColors[2] = leftColor;
        _state->fontColors[1] = _state->fontColors[3] = rightColor;
    }
    inline void setFontColor(int i, const CSColor& color) {
        _state->fontColors[i] = color;
    }
    inline void resetFontColor() {
        _state->fontColors[0] = _state->fontColors[1] = _state->fontColors[2] = _state->fontColors[3] = CSColor::White;
    }
    inline const CSColor& fontColor(int i) {
        return _state->fontColors[i];
    }

    void setStrokeColor(const CSColor& strokeColor);
    inline const CSColor& strokeColor() const {
        return _state->batch.strokeColor;
    }
    inline void setStrokeMode(CSStrokeMode mode) {
        _state->batch.strokeMode = mode;
    }
    inline CSStrokeMode strokeMode() const {
        return _state->batch.strokeMode;
    }
    inline void setStrokeWidth(int width) {
        _state->batch.strokeWidth = width;
    }
    inline int strokeWidth() const {
        return _state->batch.strokeWidth;
    }
    inline CSPolygonMode polygonMode() const {
        return _state->batch.polygonMode;
    }
    inline void setPolygonMode(CSPolygonMode polygonMode) {
        _state->batch.polygonMode = polygonMode;
    }
    inline CSDepthMode depthMode() const {
        return _state->batch.depthMode;
    }
    inline void setDepthMode(CSDepthMode depthMode) {
        _state->batch.depthMode = depthMode;
    }
    void setStencilMode(CSStencilMode stencilMode);
    inline CSStencilMode stencilMode() const {
        return _state->batch.stencilMode;
    }

    CSRect convertToTargetSpace(const CSRect& rect) const;

    void setScissor(const CSRect& scissor);
    inline void clearScissor() {
        setScissor(CSRect::Zero);
    }
    inline const CSRect& scissor() const {
        return _state->batch.scissor;
    }
    inline bool hasScissor() const {
        return _state->batch.scissor != CSRect::Zero;
    }
    inline void setLineWidth(float lineWidth) {
        _state->batch.lineWidth = lineWidth;
    }
    inline float lineWidth() const {
        return _state->batch.lineWidth;
    }
    inline void setLayer(int layer) {
        _state->batch.layer = layer;
    }
    inline int layer() const {
        return _state->batch.layer;
    }
    inline void setRenderOrder(bool renderOrder) {
        _state->renderOrder = renderOrder;
    }
    inline bool renderOrder() const {
        return _state->renderOrder;
    }
    inline void setLightSpaceState(const CSLightSpaceState* state) {
        _state->batch.lightSpaceState = state;
    }
    inline const CSLightSpaceState* lightSpaceState() const {
        return _state->batch.lightSpaceState;
    }
    inline void setRendererParam(const CSObject* param) {
        _state->batch.rendererParam = param;
    }
    inline const CSObject* rendererParam() const {
        return _state->batch.rendererParam;
    }

    inline int remaining() const {
        return _queue->remaining();
    }
    void command(CSRenderCommand* command);
    CSDelegateRenderCommand* command(const std::function<void(CSGraphicsApi*)>& inv, const CSObject* retain0 = NULL, const CSObject* retain1 = NULL, const CSObject* retain2 = NULL);
    void focus();
    void unfocus();
    void render();

    static void applyAlign(CSVector3& point, const CSVector2& size, CSAlign align);
    static void applyAlign(CSVector2& point, const CSVector2& size, CSAlign align);
    static float radianDistance(float radius);

    void drawSkybox(const CSTexture* texture);
    void drawSkybox();

    void drawVertices(CSVertexArray* vertices, CSPrimitiveMode mode, const CSABoundingBox* aabb = NULL, const CSArray<CSVertexArrayInstance>* instances = NULL);
    void drawVertices(CSVertexArray* vertices, CSPrimitiveMode mode, int indexOffset, int indexCount, const CSABoundingBox* aabb = NULL, const CSArray<CSVertexArrayInstance>* instances = NULL);
    void drawVertices(CSVertexArray* vertices, const CSGBuffer* boneBuffer, int boneOffset, CSPrimitiveMode mode, const CSABoundingBox* aabb = NULL, const CSArray<CSVertexArrayInstance>* instances = NULL);
    void drawVertices(CSVertexArray* vertices, const CSGBuffer* boneBuffer, int boneOffset, CSPrimitiveMode mode, int indexOffset, int indexCount, const CSABoundingBox* aabb = NULL, const CSArray<CSVertexArrayInstance>* instances = NULL);

    void drawPoint(const CSVector3& point);
    void drawLine(const CSVector3& point1, const CSVector3& point2);
    void drawGradientLine(const CSVector3& point1, const CSColor& color1, const CSVector3& point2, const CSColor& color2);
    void drawRect(const CSZRect& rect, bool fill, const CSRect& uv = CSRect::ZeroToOne);
private:
    void drawGradientRect(const CSZRect& rect, const CSColor& leftTopColor, const CSColor& rightTopColor, const CSColor& leftBottomColor, const CSColor& rightBottomColor, bool fill, const CSRect& uv);
public:
    inline void drawGradientRectV(const CSZRect& rect, const CSColor& topColor, const CSColor& bottomColor, bool fill, const CSRect& uv = CSRect::ZeroToOne) {
        drawGradientRect(rect, topColor, topColor, bottomColor, bottomColor, fill, uv);
    }
    inline void drawGradientRectH(const CSZRect& rect, const CSColor& leftColor, const CSColor& rightColor, bool fill, const CSRect& uv = CSRect::ZeroToOne) {
        drawGradientRect(rect, leftColor, rightColor, leftColor, rightColor, fill, uv);
    }
    void drawRoundRect(const CSZRect& rect, float radius, bool fill, CSCorner corner = CSCornerAll, const CSRect& uv = CSRect::ZeroToOne);
private:
    void drawGradientRoundRect(const CSZRect& rect, float radius, const CSColor& leftTopColor, const CSColor& rightTopColor, const CSColor& leftBottomColor, const CSColor& rightBottomColor, bool fill, CSCorner corner, const CSRect& uv);
public:
    inline void drawGradientRoundRectV(const CSZRect& rect, float radius, const CSColor& topColor, const CSColor& bottomColor, bool fill, CSCorner corner = CSCornerAll, const CSRect& uv = CSRect::ZeroToOne) {
        drawGradientRoundRect(rect, radius, topColor, topColor, bottomColor, bottomColor, fill, corner, uv);
    }
    inline void drawGradientRoundRectH(const CSZRect& rect, float radius, const CSColor& leftColor, const CSColor& rightColor, bool fill, CSCorner corner = CSCornerAll, const CSRect& uv = CSRect::ZeroToOne) {
        drawGradientRoundRect(rect, radius, leftColor, rightColor, leftColor, rightColor, fill, corner, uv);
    }
    void drawArc(const CSZRect& rect, float angle1, float angle2, bool fill, const CSRect& uv = CSRect::ZeroToOne);
    inline void drawCircle(const CSZRect& rect, bool fill, const CSRect& uv = CSRect::ZeroToOne) {
        drawArc(rect, 0, FloatTwoPi, fill, uv);
    }
    void drawGradientArc(const CSZRect& rect, float angle1, float angle2, const CSColor& centerColor, const CSColor& surroundColor, const CSRect& uv = CSRect::ZeroToOne);
    void drawGradientCircle(const CSZRect& rect, const CSColor& centerColor, const CSColor& surroundColor, const CSRect& uv = CSRect::ZeroToOne) {
        drawGradientArc(rect, 0, FloatTwoPi, centerColor, surroundColor, uv);
    }
    void drawSphere(const CSVector3& pos, float radius, const CSRect& uv = CSRect::ZeroToOne);
    void drawCapsule(const CSVector3& pos, float height, float radius, const CSRect& uv = CSRect::ZeroToOne);
    void drawCylinder(const CSVector3& pos, float topRadius, float bottomRadius, float height, const CSRect& uv = CSRect::ZeroToOne);
    void drawPyramid(const CSVector3& pos, float size, float height, bool reverse, const CSRect& uv = CSRect::ZeroToOne);
    void drawBox(const CSVector3& min, const CSVector3& max, const CSRect& uv = CSRect::ZeroToOne);
    void drawCube(const CSVector3& pos, float size, const CSRect& uv = CSRect::ZeroToOne);
    void drawHexahedron(const CSVector3& pos, const CSVector2& topSize, const CSVector2& bottomSize, float height, const CSRect& uv = CSRect::ZeroToOne);
    void drawHexahedron(const CSZRect& topRect, const CSZRect& bottomRect, const CSRect& uv = CSRect::ZeroToOne);
private:
    //image
    void drawImageImpl(const CSImage* image, const CSZRect& destRect, const CSRect& frame);
public:
    void drawImage(const CSImage* image, const CSZRect& destRect, const CSRect& srcRect);
    void drawImage(const CSImage* image, const CSZRect& destRect, bool stretch = true);
    void drawImage(const CSImage* image, const CSVector3& point, CSAlign align);
    void drawImageScaled(const CSImage* image, const CSVector3& point, float scale, CSAlign align);
    void drawLineImage(const CSImage* image, float scroll, const CSVector3& src, const CSVector3& dest);
    void drawClockImage(const CSImage* image, float progress, const CSVector3& point, CSAlign align);
    void drawClockImageScaled(const CSImage* image, float progress, const CSVector3& point, float scale, CSAlign align);
    void drawClockImage(const CSImage* image, float progress, const CSZRect& destRect);
    void drawStretchImage(const CSImage* image, const CSZRect& destRect, float horizontal, float vertical);
    void drawStretchImage(const CSImage* image, const CSZRect& destRect);
private:
    void drawShadowFlatImageImpl(const CSImage* image, const CSZRect& destRect, const CSRect& frame, const CSVector2& offset, bool xflip, bool yflip);
public:
    void drawShadowFlatImage(const CSImage* image, const CSZRect& destRect, const CSRect& srcRect, const CSVector2& offset = CSVector2::Zero, bool xflip = false, bool yflip = false);
    void drawShadowFlatImage(const CSImage* image, const CSZRect& destRect, const CSVector2& offset = CSVector2::Zero, bool xflip = false, bool yflip = false);
    void drawShadowFlatImage(const CSImage* image, const CSVector3& point, CSAlign align, const CSVector2& offset = CSVector2::Zero, bool xflip = false, bool yflip = false);
private:
    void drawShadowRotateImageImpl(const CSImage* image, const CSZRect& destRect, const CSRect& frame, const CSVector2& offset, float flatness);
public:
    void drawShadowRotateImage(const CSImage* image, const CSZRect& destRect, const CSRect& srcRect, const CSVector2& offset = CSVector2::Zero, float flatness = 0);
    void drawShadowRotateImage(const CSImage* image, const CSZRect& destRect, const CSVector2& offset = CSVector2::Zero, float flatness = 0);
    void drawShadowRotateImage(const CSImage* image, const CSVector3& point, CSAlign align, const CSVector2& offset = CSVector2::Zero, float flatness = 0);
public:
	//string
	struct StringParam {
        const CSStringDisplay* display;
        int start;
        int end;

		StringParam(const string& str);
		StringParam(const string& str, int offset, int length);
		StringParam(const CSLocaleString* str);

        inline operator bool() const {
			return display != NULL;
		}
		inline bool operator !() const {
			return display == NULL;
		}
	};
private:
    struct ParagraphDisplay {
        const CSFont* font;
        CSVector2 size;
        enum {
            Forward,
            Visible,
            Backward
        } visible;
    };
    static void paragraphDisplays(const StringParam& str, const CSFont* font, CSArray<ParagraphDisplay>* result);

    static CSVector2 stringSizeImpl(const StringParam& str, float width, const CSArray<ParagraphDisplay>* paraDisplays);
    static CSVector2 stringLineSizeImpl(const StringParam& str, int pi, float width, const CSArray<ParagraphDisplay>* paraDisplays);
public:
    static int stringIndex(const StringParam& str, int ci);
	static CSVector2 stringSize(const StringParam& str, const CSFont* font, float width = CSStringWidthUnlimited);
    CSVector2 stringSize(const StringParam& str, float width = CSStringWidthUnlimited) const;
    static int stringCursor(const StringParam& str, const CSFont* font, const CSVector2& target, float width = CSStringWidthUnlimited);
	int stringCursor(const StringParam& str, const CSVector2& target, float width = CSStringWidthUnlimited) const;
	static CSVector2 stringPosition(const StringParam& str, const CSFont* font, float width = CSStringWidthUnlimited);
	CSVector2 stringPosition(const StringParam& str, float width = CSStringWidthUnlimited) const;
    static string shrinkString(const StringParam& str, const CSFont* font, float scroll, float width = CSStringWidthUnlimited);
private:
    void drawStringCharacter(const CSImage* image, const CSVector3& pos, int capacity, CSStreamRenderCommand*& command);
    void drawStringCharacterEnd(CSStreamRenderCommand*& command);
    void drawStringImpl(const StringParam& str, const CSZRect& destRect, float scroll, const CSArray<ParagraphDisplay>* paraDisplays);
public:
    void drawStringParagraphs(const StringParam& str, const CSVector3& point);
    void drawStringScrolled(const StringParam& str, const CSZRect& destRect, float scroll);
    inline void drawString(const StringParam& str, const CSZRect& destRect) {
        drawStringScrolled(str, destRect, 0);
    }
    void drawString(const StringParam& str, CSZRect destRect, CSAlign align);
    float drawString(const StringParam& str, const CSVector3& point, float width = CSStringWidthUnlimited);
    float drawString(const StringParam& str, CSVector3 point, CSAlign align, float width = CSStringWidthUnlimited);
    void drawStringScaled(const StringParam& str, const CSZRect& destRect);
    void drawStringScaled(const StringParam& str, CSZRect destRect, CSAlign align);
    void drawStringScaled(const StringParam& str, const CSVector3& point, float width);
    void drawStringScaled(const StringParam& str, CSVector3 point, CSAlign align, float width);
    void drawStringTruncated(const StringParam& str, const CSVector3& point, float width);
    void drawStringTruncated(StringParam str, CSVector3 point, CSAlign align, float width);
public:
    //globalization
    float drawDigit(int value, bool comma, const CSVector3& point);
    float drawDigit(int value, bool comma, const CSVector3& point, CSAlign align);
    float drawDigitImages(int value, const CSArray<CSImage>* images, bool comma, int offset, const CSVector3& point, CSAlign align = CSAlignNone, float spacing = 0);
    
public:
    //FX
    void blur(float intensity);
    void blur(const CSRect& rect, float intensity);
    void blurDepth(float distance, float range, float intensity);
    void blurDepth(const CSRect& rect, float distance, float range, float intensity);
    void blurDirection(const CSVector2& dir);
    void blurDirection(const CSRect& rect, const CSVector2& dir);
    void blurCenter(const CSVector2& center, float range);
    void blurCenter(const CSRect& rect, const CSVector2& center, float range);

    void lens(const CSVector3& center, float radius, float convex);
    void wave(const CSVector3& center, float radius, float thickness);
};

#endif
