#ifndef __CDK__CSUpdatePass__
#define __CDK__CSUpdatePass__

class CSSceneObject;

class CSUpdatePass {
private:
	int _pass = 0;
public:
	static constexpr int Max = 8;

	bool addPrecedence(const CSSceneObject* obj);
	inline bool remaining() const {
		return _pass < Max;
	}
	inline operator int() const {
		return _pass;
	}
};

#endif

