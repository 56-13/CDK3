#ifndef __CDK__CSRenderFrame__
#define __CDK__CSRenderFrame__

#include "CSRenderCommand.h"

#include "CSSet.h"

class CSRenderFrame : public CSObject {
private:
    CSArray<CSRenderCommand> _commands;
    CSSet<const CSGraphicsResource*> _reads;
    CSSet<const CSGraphicsResource*> _writes;
    CSArray<CSRenderCommand*> _parallelCommands;
    CSArray<CSRenderCommand*> _parallelLocalCommands;
    int _commandCapacity = 0;
    int _readCapacity = 0;
    int _writeCapacity = 0;
    int _parallelCommandCapacity = 0;
    int _parallelLocalCommandCapacity = 0;
public:
    CSRenderFrame() = default;
    CSRenderFrame(int commandCapacity, int readCapacity, int writeCapacity, int parallelCommandCapacity, int parallelLocalCommandCapacity);
private:
    ~CSRenderFrame() = default;
public:
    inline int commandCapacity() const {
        return _commandCapacity;
    }
    inline int readCapacity() const {
        return _readCapacity;
    }
    inline int writeCapacity() const {
        return _writeCapacity;
    }
    inline int parallelCommandCapacity() const {
        return _parallelCommandCapacity;
    }
    inline int parallelLocalCommandCapacity() const {
        return _parallelLocalCommandCapacity;
    }

    bool command(CSRenderCommand* command);
    
    void render(CSGraphicsApi* api);
private:
    void render(CSGraphicsApi* api, int from, int to, bool background, bool foreground);
};

#endif
