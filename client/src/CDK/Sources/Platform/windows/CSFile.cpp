#ifdef CDK_WINDOWS

#define CDK_IMPL

#include "CSFile.h"
#include "CSFileRawImpl.h"
#include "CSArray.h"

#include <io.h>
#include <direct.h>
#include <dirent.h>

enum PathType {
	PathTypeError = -1,
	PathTypeBundle,
	PathTypeStorage
};
static const char* BundleRootPath = "./bundle/";
static const int BundleRootPathLen = strlen(BundleRootPath);
static char StorageRootPath[16] = {};
static int StorageRootPathLen = 0;

#ifndef DEBUG
static char __profile[16] = {};
#endif

int CSFile::initialize() {
	int i = 0;
#ifndef DEBUG
	if (__argc == 1) {
		for (;;) {
			sprintf(__profile, "./.run_%d", i);
			if (access(__profile, 0) == -1) break;
			else if (++i >= 4) exit(-1);
		}
	}
	else {
		i = atoi(__argv[1]);
		sprintf(__profile, "./.run_%d", i);
		if (access(__profile, 0) != -1) exit(-1);
	}
	FILE* fp = fopen(__profile, "w");
	fclose(fp);
#endif
	sprintf(StorageRootPath, "./storage_%d/", i);
	StorageRootPathLen = strlen(StorageRootPath);
	mkdir(StorageRootPath);

	CSLog("run with profile:%d", i);

	return i;
}

void CSFile::finalize() {
#ifndef DEBUG
	remove(__profile);
#endif
}

PathType getPathType(const char* path) {
	if (strncmp(path, BundleRootPath, BundleRootPathLen) == 0) return PathTypeBundle;
	else if (strncmp(path, StorageRootPath, StorageRootPathLen) == 0) return PathTypeStorage;
	return PathTypeError;
}

void* CSFile::createRawWithContentOfFile(const char* path, int offset, int& length, bool compression) {
	if (getPathType(path) == PathTypeError) return NULL;

	if (compression) return ::createRawWithCompressedContentOfFile(path, offset, length);
	else return ::createRawWithContentOfFile(path, offset, length);
}

bool CSFile::writeRawToFile(const char* path, const void* data, int length, bool compression) {
	if (getPathType(path) != PathTypeStorage) return NULL;

	if (compression) return ::writeCompressedRawToFile(path, data, length);
	else return ::writeRawToFile(path, data, length);
}

const char* CSFile::findPath(const char* subpath) {
	const char* path = storagePath(subpath);
	if (fileExists(path)) return path;
	path = bundlePath(subpath);
	if (fileExists(path)) return path;
	return NULL;
}

const char* CSFile::storagePath(const char* subpath) {
	return string::cstringWithFormat("%s%s", StorageRootPath, subpath);
}

const char* CSFile::bundlePath(const char* subpath) {
	return string::cstringWithFormat("%s%s", BundleRootPath, subpath);
}

bool CSFile::fileExists(const char* path) {
	return access(path, 0) != -1;
}

bool CSFile::moveFile(const char* srcpath, const char* destpath) {
	if (getPathType(srcpath) != PathTypeStorage || getPathType(destpath) != PathTypeStorage) return false;

	FILE* rfp;
	FILE* wfp;

	if (!(rfp = fopen(srcpath, "rb"))) return false;

	if (!(wfp = fopen(destpath, "wb"))) {
		fclose(rfp);
		return false;
	}

	byte data[4096];
	int length;
	while ((length = fread(data, sizeof(byte), sizeof(data), rfp)) > 0) {
		fwrite(data, 1, length, wfp);
	}
	fflush(wfp);
	fclose(wfp);
	fclose(rfp);
	return true;
}

bool CSFile::deleteFile(const char* path) {
	if (getPathType(path) != PathTypeStorage) return false;

	return remove(path) == 0;
}

bool CSFile::makeDirectory(const char* path) {
	if (getPathType(path) != PathTypeStorage) return false;

	return mkdir(path) == 0;
}

CSArray<string>* CSFile::filePaths(const char* dirpath, bool full) {
	CSArray<string>* paths = CSArray<string>::array();

	if (getPathType(dirpath) != PathTypeError) {
		DIR* dir;
		struct dirent* direntry;

		if ((dir = opendir(dirpath)) != NULL) {
			while ((direntry = readdir(dir)) != NULL) {
				if (direntry->d_name[0] != '.') {
					string path = full ? string::format("%s/%s", dirpath, direntry->d_name) : string(direntry->d_name);
					paths->addObject(path);
				}
			}
		}
	}
	return paths;
}

#endif