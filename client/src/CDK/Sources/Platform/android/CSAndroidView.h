//
//  CSAndroidView.h
//  CDK
//
//  Created by 김찬 on 13. 5. 9..
//  Copyright (c) 2012년 brgames. All rights reserved.
//

#if defined(CDK_ANDROID) && defined(CDK_IMPL)

#ifndef __CDK__CSAndroidView__
#define __CDK__CSAndroidView__

#include "CSView.h"

class CSAndroidView {
private:
	CSView* _view;
	CSGraphics* _graphics;
	int _width;
	int _height;
	int _horizontalEdge;
	int _verticalEdge;
public:
	CSAndroidView(int width, int height, int bufferWidth, int bufferHeight, int horizontalEdge, int verticalEdge);
	~CSAndroidView();

	inline CSView* view() {
		return _view;
	}
    inline CSGraphics* graphics() {
        return _graphics;
    }
    inline int width() const {
		return _width;
	}
	inline int height() const {
		return _height;
	}
	inline int horizontalEdge() const {
		return _horizontalEdge;
	}
	inline int verticalEdge() const {
		return _verticalEdge;
	}

	void touchesBegan(int num, int ids[], float xs[], float ys[]);
	void touchesMoved(int num, int ids[], float xs[], float ys[]);
	void touchesCancelled(int num, int ids[], float xs[], float ys[]);
	void touchesEnded(int num, int ids[], float xs[], float ys[]);
	void backKey();
    void setKeyboardHeight(float height);

    inline int bufferWidth() const {
        return _graphics->target()->width();
    }
    inline int bufferHeight() const {
        return _graphics->target()->height();
    }
    void resizeBuffer(int bufferWidth, int bufferHeight);
    void reload();
    void render(bool refresh);
};

#endif /* defined(__CDK__CSAndroidView__) */

#endif
