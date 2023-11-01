#define CDK_IMPL

#include "CSRenderFrame.h"

#include "CSThread.h"
#include "CSDiagnostics.h"

#include "CSGraphicsContext.h"

static constexpr int SequentialRenderLimit = 2;

CSRenderFrame::CSRenderFrame(int commandCapacity, int readCapacity, int writeCapacity, int parallelCommandCapacity, int parallelLocalCommandCapacity) :
    _commands(commandCapacity),
    _reads(readCapacity),
    _writes(writeCapacity),
    _parallelCommands(parallelCommandCapacity),
    _parallelLocalCommands(parallelLocalCommandCapacity)
{
    
}

bool CSRenderFrame::command(CSRenderCommand* command) {
    if (!command->submit()) return false;

    synchronized(&_commands) {
        _commands.addObject(command);
    }
    return true;
}

void CSRenderFrame::render(CSGraphicsApi* api, int from, int to, bool background, bool foreground) {
    if (CSGraphicsContext::sharedContext()->isSupportParallel()) {
        for (int i = from; i < to; i++) _parallelCommands.addObject(_commands.objectAtIndex(i));

        if (_parallelCommandCapacity < _parallelCommands.count()) _parallelCommandCapacity = _parallelCommands.count();

        CSArray<CSTaskBase*> tasks(_parallelLocalCommands.capacity());

        while (_parallelCommands.count()) {
            {
                int i = 0;
                while (i < _parallelCommands.count()) {
                    CSRenderCommand* command = _parallelCommands.objectAtIndex(i);
                    if (command->parallel(&_reads, &_writes)) {
                        _parallelLocalCommands.addObject(command);
                        _parallelCommands.removeObjectAtIndex(i);
                    }
                    else i++;
                }
                if (_readCapacity < _reads.count()) _readCapacity = _reads.count();
                if (_writeCapacity < _writes.count()) _writeCapacity = _writes.count();
                _reads.removeAllObjects();
                _writes.removeAllObjects();
            }

            if (_parallelLocalCommandCapacity < _parallelLocalCommands.count()) _parallelLocalCommandCapacity = _parallelLocalCommands.count();

            if (_parallelLocalCommands.count() > SequentialRenderLimit) {
                foreach (CSRenderCommand*, command, &_parallelLocalCommands) {
                    tasks.addObject(CSThreadPool::run<void>([command, background, foreground] {
                        //사용중인 명령버퍼가 없다면 멀티쓰레드에서 명령버퍼를 풀링하고 해당 쓰레드에 붙이고 뗌, 추후 코드체크
                        CSGraphicsApi* api = CSGraphicsContext::sharedContext()->attachRenderThread();
                        command->render(api, background, foreground);           //TODO:공유자원의 fence문제. 추후 실제 Metal / Vulcan을 작업하게 되면 확인
                        CSGraphicsContext::sharedContext()->detachRenderThread();
                    }));
                }
                CSTaskBase::finishAll(&tasks);
                tasks.removeAllObjects();
            }
            else {
                for (int i = 0; i < _parallelLocalCommands.count(); i++) {
                    CSRenderCommand* command = _parallelLocalCommands.objectAtIndex(i);
                    command->render(api, background, foreground);
                }
            }
            _parallelLocalCommands.removeAllObjects();
        }
    }
    else
    {
        for (int i = from; i < to; i++) {
            CSRenderCommand* command = _commands.objectAtIndex(i);
            command->render(api, background, foreground);
        }
    }
}

void CSRenderFrame::render(CSGraphicsApi* api) {
    _commandCapacity = _commands.count();

    for (int i = _commands.count() - 1; i >= 0; i--) {
        CSRenderCommand* command = _commands.objectAtIndex(i);

        command->parallel(&_reads, &_writes);

        CSRenderCommand* batch = NULL;

        int j = i - 1;
        while (j >= 0) {
            CSRenderCommand* nextCommand = _commands.objectAtIndex(j);

            bool flag0 = command->findBatch(nextCommand, batch);
            bool flag1 = nextCommand->parallel(&_reads, &_writes);
            if (flag0 || flag1) j--;
            else break;
        }

        if (batch) {
            command->batch(batch);
            _commands.removeObjectAtIndex(i);
        }
        if (_readCapacity < _reads.count()) _readCapacity = _reads.count();
        if (_writeCapacity < _writes.count()) _writeCapacity = _writes.count();
        _reads.removeAllObjects();
        _writes.removeAllObjects();
    }

    CSDiagnostics::renderCommand(_commandCapacity, _commands.count());

    int i = 0;
    while (i < _commands.count()) {
        CSRenderCommand* command = _commands.objectAtIndex(i);
        int layer = command->layer();
        int end = i + 1;
        while (end < _commands.count()) {
            if (_commands.objectAtIndex(i)->layer() == layer) end++;
        }
        if (layer == 0 || i + 1 == end) render(api, i, end, true, true);
        else {
            render(api, i, end, true, false);
            render(api, i, end, false, true);
        }
        i = end;
    }
}