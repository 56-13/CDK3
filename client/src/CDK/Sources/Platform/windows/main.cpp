#ifdef CDK_WINDOWS

#define CDK_IMPL

#include "CSDeviceConfig.h"
#include "CSFile.h"
#include "CSSignal.h"
#include "CSDiagnostics.h"
#include "CSThread.h"
#include "CSResourcePool.h"
#include "CSGraphics.h"
#include "CSAudio.h"
#include "CSApplicationBridge.h"
#include "CSFontImpl.h"
#include "CSTextFieldImpl.h"
#include "CSURLConnectionImpl.h"
#include "CSStringContentTable.h"

#include <direct.h>
#include <windows.h>
#include <GLFW/glfw3.h>
#include <FreeImage.h>

struct Context {
	GLFWwindow* window;
	int width;
	int height;
	bool alive;
	CSPlatformTouch* touch;
	char* clipboard;
};
static Context __context = {};

static void onError(int error, const char* description);
static void onResize(GLFWwindow* window, int width, int height);
static void onMouseEvent(GLFWwindow* window, int button, int action, int mods);
static void onScrollEvent(GLFWwindow* window, double xoffset, double yoffset);
static void onKeyEvent(GLFWwindow* window, int key, int scancode, int action, int mods);
static void onTextInput(GLFWwindow* window, uint codepoint, int mods);

int APIENTRY WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpszCmdParam, int nCmdShow) {
#ifdef CS_CONSOLE_DEBUG
	if (AllocConsole()) {
		freopen("CONIN$", "rb", stdin);
		freopen("CONOUT$", "wb", stdout);
		freopen("CONOUT$", "wb", stderr);

		//std::ios::sync_with_stdio();
	}
#endif

	const char* app = __argv[0];
	int e = strlen(app) - 1;
	while (e >= 0 && app[e] != '\\') e--;
	if (e > 0) {
		char* cwd = (char*)alloca(e + 1);
		strncpy(cwd, app, e);
		cwd[e] = 0;
		chdir(cwd);
		app += e;
	}

	glfwSetErrorCallback(onError);

	if (!glfwInit()) {
		MessageBox(NULL, "GLFW Init fail", "Error", MB_OK);
		exit(-1);
	}

	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
	glfwWindowHint(GLFW_DOUBLEBUFFER, GL_TRUE);

	deviceConfigInitialize();

	int width = deviceConfigWidth();
	int height = deviceConfigHeight();

	CSStringContentTable::initialize();

	int profile = CSFile::initialize();

	GLFWwindow* window;
	{
		char title[64];
		sprintf(title, "%s #%d", app, profile);
		window = glfwCreateWindow(width, height, title, NULL, NULL);
	}
		
	if (!window) {
		CSFile::finalize();
		MessageBox(NULL, "GLFW Init fail", "Error", MB_OK);
		glfwTerminate();
		exit(-1);
	}

	CSThread::initialize(NULL);

	CSTask<bool>* task = CSThread::renderThread()->run<bool>([window]() -> bool {
		glfwMakeContextCurrent(window);

		if (glewInit() != GLEW_OK) return false;

		glfwSetFramebufferSizeCallback(window, onResize);
		glfwSetMouseButtonCallback(window, onMouseEvent);
		glfwSetScrollCallback(window, onScrollEvent);
		glfwSetKeyCallback(window, onKeyEvent);
		glfwSetCharModsCallback(window, onTextInput);
		glfwSwapInterval(1);

		return true;
	});
	if (!task->finish() || !task->result()) {
		CSFile::finalize();
		MessageBox(NULL, "GLEW Init fail", "Error", MB_OK);
		glfwTerminate();
		exit(-1);
	}

	FreeImage_Initialise(TRUE);

	CSSignal::initialize();
	CSGraphics::initialize(CSGraphicsPlatformOpenGL);
	CSAudio::initialize();
	CSFontImpl::initialize();
	CSTextFieldHandleManager::initialize();
	CSURLConnectionHandleManager::initialize();

	glfwGetFramebufferSize(window, &width, &height);

	//this is glfw windows default
	CSSystemRenderTargetDescription renderTargetDesc;
	renderTargetDesc.redBit = 8;
	renderTargetDesc.greenBit = 8;
	renderTargetDesc.blueBit = 8;
	renderTargetDesc.alphaBit = 8;
	renderTargetDesc.depthBit = 24;
	renderTargetDesc.stencilBit = 8;
	renderTargetDesc.samples = 0;

	CSApplication::initialize(width, height, 0, renderTargetDesc);

	__context.window = window;
	__context.width = width;
	__context.height = height;
	__context.alive = true;

	onStart();

	while (!glfwWindowShouldClose(window) && __context.alive) {
		glfwPollEvents();

		CSTextFieldHandle* handle = CSTextFieldHandleManager::sharedManager()->focusedHandle();

		if (handle) handle->timeout();

		if (__context.touch) {
			double x, y;
			glfwGetCursorPos(window, &x, &y);
			if (x < 0 || x > __context.width || y < 0 || y > __context.height) {
				CSApplication::sharedApplication()->touchesCancelled(__context.touch, 1);
				delete __context.touch;
				__context.touch = NULL;
			}
			else if (x != __context.touch->x || y != __context.touch->y) {
				__context.touch->x = x;
				__context.touch->y = y;
				CSApplication::sharedApplication()->touchesMoved(__context.touch, 1);
			}
		}

		CSApplication::sharedApplication()->timeout();

		CSThread::mainThread()->execute();

		CSStringContentTable::sharedTable()->drain();
	}
	
	if (__context.clipboard) free(__context.clipboard);
	if (__context.touch) delete __context.touch;

	CSApplication::sharedApplication()->OnDestroy();
	onDestroy();

	CSApplication::finalize();
	CSURLConnectionHandleManager::finalize();
	CSTextFieldHandleManager::finalize();
	CSFontImpl::finalize();
	CSAudio::finalize();
	CSGraphics::finalize();
	CSSignal::finalize();
	CSThread::finalize();
	CSFile::finalize();
	CSStringContentTable::finalize();

	FreeImage_DeInitialise();

	glfwDestroyWindow(__context.window);
	
	glfwTerminate();

#ifdef CS_CONSOLE_DEBUG
	FreeConsole();
#endif

	return 0;
}

static void onError(int error, const char* description) {
	CSErrorLog("Error:%s", description);
}

static void onResize(GLFWwindow* window, int width, int height) {
	if (width && height) {
		__context.width = width;
		__context.height = height;
		CSApplication::sharedApplication()->resize(width, height);
	}
}

static void onMouseEvent(GLFWwindow* window, int button, int action, int mods) {
	double x, y;
	glfwGetCursorPos(window, &x, &y);

	switch (action) {
		case GLFW_PRESS:
			if (button == GLFW_MOUSE_BUTTON_LEFT && CSTextFieldHandleManager::sharedManager()->touchHandle(CSVector2(x, y))) {
				break;
			}
			if (!__context.touch) {
				__context.touch = new CSPlatformTouch();
				__context.touch->key = 0;
				__context.touch->button = (CSTouchButton)button;
				__context.touch->x = x;
				__context.touch->y = y;
				CSApplication::sharedApplication()->touchesBegan(__context.touch, 1);
			}
			break;
		case GLFW_RELEASE:
			if (__context.touch) {
				CSApplication::sharedApplication()->touchesEnded(__context.touch, 1);
				delete __context.touch;
				__context.touch = NULL;
			}
			break;
	}
}

static void onScrollEvent(GLFWwindow* window, double xoffset, double yoffset) {
	CSApplication::sharedApplication()->wheel(yoffset);
}

static void onKeyEvent(GLFWwindow* window, int key, int scancode, int action, int mods) {
	CSTextFieldHandle* handle = CSTextFieldHandleManager::sharedManager()->focusedHandle();

	if (handle) {
		if (action == GLFW_PRESS) {
			switch (key) {
				case GLFW_KEY_LEFT:
					handle->moveLeftCursor(false);
					break;
				case GLFW_KEY_RIGHT:
					handle->moveRightCursor(false);
					break;
				case GLFW_KEY_BACKSPACE:
					handle->moveLeftCursor(true);
					break;
				case GLFW_KEY_DELETE:
					handle->moveRightCursor(true);
					break;
				case GLFW_KEY_HOME:
					handle->moveStartCursor();
					break;
				case GLFW_KEY_END:
					handle->moveEndCursor();
					break;
				case GLFW_KEY_ENTER:
					handle->complete();
					break;
			}
		}
	}
	else {
		if (action == GLFW_PRESS) {
			if (key == GLFW_KEY_ESCAPE) CSApplication::sharedApplication()->backKey();
			else CSApplication::sharedApplication()->keyDown(key);
		}
		else if (action == GLFW_RELEASE) {
			if (key != GLFW_KEY_ESCAPE) CSApplication::sharedApplication()->keyUp(key);
		}
	}
}

static void onTextInput(GLFWwindow *window, uint codepoint, int mods) {
	CSTextFieldHandle* handle = CSTextFieldHandleManager::sharedManager()->focusedHandle();
	
	if (handle) {
		handle->inputCharacter(codepoint);
	}
}

//============================================================================================================

void CSApplicationBridge::setVersion(const char* version) {

}

CSRect CSApplicationBridge::frame() {
	return CSRect(0, 0, __context.width, __context.height);
}

CSEdgeInsets CSApplicationBridge::edgeInsets() {
	CSEdgeInsets rtn = {};

	return rtn;
}

void CSApplicationBridge::setResolution(CSResolution resolution) {
	CSLog("setResolution not supported on windows:%d", resolution);
}

CSResolution CSApplicationBridge::resolution() {
	return CSResolutionFit;
}

void CSApplicationBridge::draw(CSGraphics* graphics) {
	CSTextFieldHandleManager::sharedManager()->draw(graphics);
}

void CSApplicationBridge::swapBuffer() {
	CSGraphicsContext::sharedContext()->invoke(false, [](CSGraphicsApi* api) {
		glfwSwapBuffers(__context.window);

		CSResourcePool::sharedPool()->purgeCycle();

		CSDiagnostics::renderCycle();
	});
}

void CSApplicationBridge::openURL(const char* url) {
	CSLog("open url:%s", url);
}

const char* CSApplicationBridge::clipboard() {
	return __context.clipboard;
}

void CSApplicationBridge::setClipboard(const char* text) {
	if (__context.clipboard) {
		free(__context.clipboard);
		__context.clipboard = NULL;
	}
	if (text) __context.clipboard = strdup(text);
}

void CSApplicationBridge::shareUrl(const char* title, const char* message, const char* url) {
	CSLog("share url:%s", url);
}

void CSApplicationBridge::finish() {
	__context.alive = false;
}

#endif
